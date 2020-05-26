// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Edge.Publisher.Services {
    using Microsoft.Azure.IIoT.OpcUa.Edge.Publisher.Models;
    using Microsoft.Azure.IIoT.OpcUa.Publisher;
    using Microsoft.Azure.IIoT.Exceptions;
    using Microsoft.Azure.IIoT.Module;
    using Microsoft.Azure.IIoT.Serializers;
    using Serilog;
    using Prometheus;
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Dataflow engine
    /// </summary>
    public class DataFlowProcessingEngine : IProcessingEngine, IDisposable {

        /// <inheritdoc/>
        public bool IsRunning { get; private set; }

        /// <inheritdoc/>
        public string Name => _trigger.Id;

        /// <summary>
        /// Create engine
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="encoder"></param>
        /// <param name="events"></param>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        /// <param name="identity"></param>
        public DataFlowProcessingEngine(IMessageTrigger trigger, IMessageEncoder encoder,
            IEventEmitter events, IEngineConfiguration config, ILogger logger,
            IIdentity identity) {

            _config = config ?? throw new ArgumentNullException(nameof(config));
            _trigger = trigger ?? throw new ArgumentNullException(nameof(trigger));
            _events = events ?? throw new ArgumentNullException(nameof(events));
            _messageEncoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));

            _trigger.OnMessage += MessageTriggerMessageReceived;

            if (_config.BatchSize.HasValue && _config.BatchSize.Value > 1) {
                _dataSetMessageBufferSize = _config.BatchSize.Value;
            }

            if (config.BatchSize.HasValue &&
                config.BatchSize.Value > 1) {
                _dataSetMessageBufferSize = config.BatchSize.Value;
            }
            if (config.MaxMessageSize.HasValue &&
                config.MaxMessageSize.Value > 0) {
                _maxEncodedMessageSize = config.MaxMessageSize.Value;
            }
        }

        /// <inheritdoc/>
        public void Dispose() {
            _trigger.OnMessage -= MessageTriggerMessageReceived;
            _diagnosticsOutputTimer?.Dispose();
            _batchTriggerIntervalTimer?.Dispose();
        }

        /// <inheritdoc/>
        public async Task RunAsync(CancellationToken cancellationToken) {
            if (_messageEncoder == null) {
                throw new NotInitializedException();
            }

            try {
                if (IsRunning) {
                    return;
                }
                IsRunning = true;
                _diagnosticStart = DateTime.UtcNow;
                if (_config.DiagnosticsInterval.HasValue && _config.DiagnosticsInterval > TimeSpan.Zero){
                    _diagnosticsOutputTimer = new Timer(DiagnosticsOutputTimer_Elapsed, null,
                        _config.DiagnosticsInterval.Value,
                        _config.DiagnosticsInterval.Value);
                }

                if (_config.BatchTriggerInterval.HasValue && _config.BatchTriggerInterval > TimeSpan.Zero){
                    _batchTriggerIntervalTimer = new Timer(BatchTriggerIntervalTimer_Elapsed, null,
                        _config.BatchTriggerInterval.Value,
                        _config.BatchTriggerInterval.Value);
                }
                _encodingBlock = new TransformManyBlock<DataSetMessageModel[], NetworkMessageModel>(
                    async input =>
                        (_dataSetMessageBufferSize == 1)
                            ? await _messageEncoder.EncodeAsync(input)
                            : await _messageEncoder.EncodeBatchAsync(input, (int)_maxEncodedMessageSize),
                    new ExecutionDataflowBlockOptions {
                        CancellationToken = cancellationToken
                    });

                _batchDataSetMessageBlock = new BatchBlock<DataSetMessageModel>(
                    _dataSetMessageBufferSize,
                    new GroupingDataflowBlockOptions {
                        CancellationToken = cancellationToken
                    });

                _batchNetworkMessageBlock = new BatchBlock<NetworkMessageModel>(
                    _networkMessageBufferSize,
                    new GroupingDataflowBlockOptions {
                        CancellationToken = cancellationToken
                    });

                _sinkBlock = new ActionBlock<NetworkMessageModel[]>(
                    async input => await SendAsync(input),
                    new ExecutionDataflowBlockOptions {
                        CancellationToken = cancellationToken
                    });
                _batchDataSetMessageBlock.LinkTo(_encodingBlock);
                _encodingBlock.LinkTo(_batchNetworkMessageBlock);
                _batchNetworkMessageBlock.LinkTo(_sinkBlock);

                await _trigger.RunAsync(cancellationToken);
            }
            finally {
                IsRunning = false;
            }
        }

        /// <summary>
        /// Send messages
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        private async Task SendAsync(IEnumerable<NetworkMessageModel> messages) {
            if (messages == null) {
                return;
            }
            var messageObjects = messages.ToList();
            try {
                var messagesCount = messageObjects.Count;
                _logger.Verbose("Sending {count} objects to IoT Hub...", messagesCount);

                if (_sentMessagesCount > kMessageCounterResetThreshold) {
                    _logger.Debug("Message counter has been reset to prevent overflow. " +
                        "So far, {SentMessagesCount} messages has been sent to IoT Hub.",
                        _sentMessagesCount);
                    kMessagesSent.Set(_sentMessagesCount);
                    _sentMessagesCount = 0;
                }
                using (kSendingDuration.NewTimer()) {
                    var m = messageObjects.First();
                    if (messagesCount == 1) {
                        await _events.SendEventAsync(m.Body,
                            m.ContentType, m.MessageSchema, m.ContentEncoding);
                    }
                    else {
                        await _events.SendEventAsync(messageObjects.Select(m => m.Body),
                            m.ContentType, m.MessageSchema, m.ContentEncoding);
                    }
                }
                _sentMessagesCount += messagesCount;
                kMessagesSent.WithLabels(_iotHubMessageSinkGuid).Set(_sentMessagesCount);
            }
            catch (Exception ex) {
                _logger.Error(ex, "Error while sending messages to IoT Hub.");
                // we do not set the block into a faulted state.
            }
        }

        /// <summary>
        /// Diagnostics timer
        /// </summary>
        /// <param name="state"></param>
        private void DiagnosticsOutputTimer_Elapsed(object state) {
            var totalDuration = DateTime.UtcNow - _diagnosticStart;

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine($"   DIAGNOSTICS INFORMATION for Engine: {Name}");
            sb.AppendLine("   =======================");
            sb.AppendLine($"   # Ingress data changes (from OPC)  : {_trigger?.DataChangesCount}" +
                $" #/s : {_trigger?.DataChangesCount / totalDuration.TotalSeconds}");
            sb.AppendLine($"   # Ingress value changes (from OPC) : {_trigger?.ValueChangesCount}" +
                $" #/s : {_trigger?.ValueChangesCount / totalDuration.TotalSeconds}");
            sb.AppendLine($"   # Ingress BatchBlock buffer count  : {_batchDataSetMessageBlock?.OutputCount}");
            sb.AppendLine($"   # EncodingBlock input/output count : {_encodingBlock?.InputCount}/{_encodingBlock?.OutputCount}");
            sb.AppendLine($"   # Outgress Batch Block buffer count: {_batchNetworkMessageBlock?.OutputCount}");
            sb.AppendLine($"   # Outgress Synk input buffer count : {_sinkBlock?.InputCount}");
            sb.AppendLine($"   # Outgress message count (IoT Hub) : {_sentMessagesCount}" +
                $" #/s : {_sentMessagesCount / totalDuration.TotalSeconds}");
            sb.AppendLine("   =======================");
            sb.AppendLine($"   # Number of connection retries since last error: {_trigger.NumberOfConnectionRetries}");
            sb.AppendLine("   =======================");
            // TODO: Use structured logging!
            _logger.Information(sb.ToString());

            kValueChangesCount.WithLabels(_identity.DeviceId ?? "",
                _identity.ModuleId ?? "", Name).Set(_trigger.ValueChangesCount);
            kDataChangesCount.WithLabels(_identity.DeviceId ?? "",
                _identity.ModuleId ?? "", Name).Set(_trigger.DataChangesCount);
            kNumberOfConnectionRetries.WithLabels(_identity.DeviceId ?? "",
                _identity.ModuleId ?? "", Name).Set(_trigger.NumberOfConnectionRetries);
            kSentMessagesCount.WithLabels(_identity.DeviceId ?? "",
                _identity.ModuleId ?? "", Name).Set(_sentMessagesCount);
        }

        /// <summary>
        /// Batch trigger interval
        /// </summary>
        /// <param name="state"></param>
        private void BatchTriggerIntervalTimer_Elapsed(object state) {
            _batchDataSetMessageBlock.TriggerBatch();
        }

        /// <summary>
        /// Message received handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MessageTriggerMessageReceived(object sender, DataSetMessageModel args) {
            _batchDataSetMessageBlock.Post(args);
        }

        private readonly int _dataSetMessageBufferSize = 1;
        private readonly int _networkMessageBufferSize = 1;
        private Timer _batchTriggerIntervalTimer;
        private readonly uint _maxEncodedMessageSize = 256 * 1024;

        private readonly IEngineConfiguration _config;
        private readonly IEventEmitter _events;
        private readonly IMessageEncoder _messageEncoder;
        private readonly IMessageTrigger _trigger;
        private readonly ILogger _logger;
        private readonly IIdentity _identity;

        private BatchBlock<DataSetMessageModel> _batchDataSetMessageBlock;
        private BatchBlock<NetworkMessageModel> _batchNetworkMessageBlock;

        private Timer _diagnosticsOutputTimer;
        private DateTime _diagnosticStart = DateTime.UtcNow;

        private TransformManyBlock<DataSetMessageModel[], NetworkMessageModel> _encodingBlock;
        private ActionBlock<NetworkMessageModel[]> _sinkBlock;
        private long _sentMessagesCount;

        private const long kMessageCounterResetThreshold = long.MaxValue - 10000;
        private readonly string _iotHubMessageSinkGuid = Guid.NewGuid().ToString();
        private static readonly Gauge kMessagesSent = Metrics.CreateGauge(
            "iiot_edge_publisher_messages", "Number of messages sent to IotHub",
                new GaugeConfiguration {
                    LabelNames = new[] { "runid" }
                });
        private static readonly Histogram kSendingDuration = Metrics.CreateHistogram(
            "iiot_edge_publisher_messages_duration", "Histogram of message sending durations");
        private static readonly GaugeConfiguration kGaugeConfig = new GaugeConfiguration {
            LabelNames = new[] { "deviceid", "module", "triggerid" }
        };
        private static readonly Gauge kValueChangesCount = Metrics.CreateGauge(
            "iiot_edge_publisher_value_changes", "invoke value changes in trigger", kGaugeConfig);
        private static readonly Gauge kDataChangesCount = Metrics.CreateGauge(
            "iiot_edge_publisher_data_changes", "invoke data changes in trigger", kGaugeConfig);
        private static readonly Gauge kSentMessagesCount = Metrics.CreateGauge(
            "iiot_edge_publisher_sent_messages", "messages sent to IoTHub", kGaugeConfig);
        private static readonly Gauge kNumberOfConnectionRetries = Metrics.CreateGauge(
            "iiot_edge_publisher_connection_retries", "OPC UA connect retries", kGaugeConfig);
    }
}

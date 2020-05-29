// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Edge.Publisher.Services {
    using Microsoft.Azure.IIoT.OpcUa.Edge.Publisher.Models;
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Models;
    using Microsoft.Azure.IIoT.OpcUa.Core.Models;
    using Microsoft.Azure.IIoT.Crypto;
    using Microsoft.Azure.IIoT.Serializers;
    using Microsoft.Azure.IIoT.Module;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Loads published nodes file and configures the engine
    /// </summary>
    public class PublishedNodesFileLoader {

        /// <summary>
        /// Create published nodes file loader
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="serializer"></param>
        /// <param name="identity"></param>
        /// <param name="legacyCliModelProvider"></param>
        /// <param name="logger"></param>
        /// <param name="cryptoProvider"></param>
        public PublishedNodesFileLoader(IWriterGroupProcessingEngine engine,
            IJsonSerializer serializer, ILegacyCliModelProvider legacyCliModelProvider,
            IIdentity identity, ILogger logger, ISecureElement cryptoProvider = null) {

            _legacyCliModel = legacyCliModelProvider.LegacyCliModel
                ?? throw new ArgumentNullException(nameof(legacyCliModelProvider));

            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(logger));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _cryptoProvider = cryptoProvider;
            _lastSetOfWriterIds = new HashSet<string>();

            var directory = Path.GetDirectoryName(_legacyCliModel.PublishedNodesFile);

            if (string.IsNullOrWhiteSpace(directory)) {
                directory = Environment.CurrentDirectory;
            }

            RefreshFromFile();
            var file = Path.GetFileName(_legacyCliModel.PublishedNodesFile);
            _fileSystemWatcher = new FileSystemWatcher(directory, file);
            _fileSystemWatcher.Changed += (s, e) => RefreshFromFile();
            _fileSystemWatcher.EnableRaisingEvents = true;

           // _engine.DiagnosticsInterval = group.Priority;
        }

        /// <summary>
        /// Read monitored item job from reader
        /// </summary>
        /// <param name="publishedNodesFile"></param>
        /// <returns></returns>
        internal WriterGroupModel Read(TextReader publishedNodesFile) {
            var sw = Stopwatch.StartNew();
            _logger.Debug("Reading published nodes file ({elapsed}", sw.Elapsed);
            var items = _serializer.Deserialize<List<PublishedNodesEntryModel>>(
                publishedNodesFile);
            _logger.Information(
                "Read {count} items from published nodes file in {elapsed}",
                items.Count, sw.Elapsed);
            sw.Restart();

            var writerGroup = new WriterGroupModel {
                // MessagingMode = legacyCliModel.MessagingMode,  TODO
                // DiagnosticsInterval = _config.DiagnosticsInterval,  TODO
                MessageType = _legacyCliModel.NetworkMessageType,
                PublishingInterval = _legacyCliModel.BatchTriggerInterval,
                BatchSize = _legacyCliModel.BatchSize,
                MaxNetworkMessageSize = _legacyCliModel.MaxMessageSize,
                WriterGroupId = "Publisher",
                DataSetWriters = ToDataSetWriters(items, _legacyCliModel).ToList(),
                MessageSettings = new WriterGroupMessageSettingsModel {
                    NetworkMessageContentMask =
                        NetworkMessageContentMask.PublisherId |
                        NetworkMessageContentMask.WriterGroupId |
                        NetworkMessageContentMask.NetworkMessageNumber |
                        NetworkMessageContentMask.SequenceNumber |
                        NetworkMessageContentMask.PayloadHeader |
                        NetworkMessageContentMask.Timestamp |
                        NetworkMessageContentMask.DataSetClassId |
                        NetworkMessageContentMask.NetworkMessageHeader |
                        NetworkMessageContentMask.DataSetMessageHeader
                }
            };
            _logger.Information("Converted items to jobs in {elapsed}", sw.Elapsed);
            return writerGroup;
        }

        /// <summary>
        /// Reconfigures the engine from the published nodes file stream
        /// </summary>
        /// <param name="reader"></param>
        internal void ConfigureEngineFromStream(TextReader reader) {
            var group = Read(reader);
            var publisherId = $"LegacyPublisher_{_identity.DeviceId}_{_identity.ModuleId}";

            group.DataSetWriters.ForEach(d => {
                d.DataSet.ExtensionFields ??= new Dictionary<string, string>();
                d.DataSet.ExtensionFields["PublisherId"] = publisherId;
                d.DataSet.ExtensionFields["DataSetWriterId"] = d.DataSetWriterId;
            });

            lock (_fileLock) {
                // Update under lock
                _engine.Priority =
                    group.Priority;
                _engine.BatchSize =
                    group.BatchSize;
                _engine.PublishingInterval =
                    group.PublishingInterval;
                _engine.DataSetOrdering =
                    group.MessageSettings?.DataSetOrdering;
                _engine.GroupVersion =
                    group.MessageSettings?.GroupVersion;
                _engine.HeaderLayoutUri =
                    group.HeaderLayoutUri;
                _engine.WriterGroupId =
                    group.WriterGroupId;
                _engine.KeepAliveTime =
                    group.KeepAliveTime;
                _engine.MaxNetworkMessageSize =
                    group.MaxNetworkMessageSize;
                _engine.NetworkMessageContentMask =
                    group.MessageSettings?.NetworkMessageContentMask;
                _engine.PublishingOffset =
                    group.MessageSettings?.PublishingOffset?.ToList();
                _engine.SamplingOffset =
                    group.MessageSettings?.SamplingOffset;

                var dataSetWriterIds = group?.DataSetWriters?
                    .Select(w => w.DataSetWriterId)
                    .ToHashSet() ?? new HashSet<string>();
                _lastSetOfWriterIds.ExceptWith(dataSetWriterIds);
                _engine.RemoveWriters(_lastSetOfWriterIds);
                _engine.AddWriters(group.DataSetWriters);
                _lastSetOfWriterIds = dataSetWriterIds;
            }
        }

        /// <summary>
        /// Load from file
        /// </summary>
        private void RefreshFromFile() {
            var retryCount = 3;
            while (true) {
                try {
                    var currentFileHash = GetChecksum(_legacyCliModel.PublishedNodesFile);
                    if (currentFileHash != _lastKnownFileHash) {
                        _logger.Information("File {publishedNodesFile} has changed, reloading...",
                            _legacyCliModel.PublishedNodesFile);
                        _lastKnownFileHash = currentFileHash;
                        using (var reader = new StreamReader(_legacyCliModel.PublishedNodesFile)) {
                            ConfigureEngineFromStream(reader);
                        }
                    }
                    break; // Success
                }
                catch (IOException ex) {
                    retryCount--;
                    if (retryCount > 0) {
                        _logger.Debug("Error while loading job from file, retrying...");
                    }
                    else {
                        _logger.Error(ex,
                            "Error while loading job from file. Retry expired, giving up.");
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Get a checksum for the current file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static string GetChecksum(string file) {
            using (var stream = File.OpenRead(file))
            using (var sha = new SHA256Managed())  {
                var checksum = sha.ComputeHash(stream);
                return BitConverter.ToString(checksum);
            }
        }

        /// <summary>
        /// Read monitored item job from reader
        /// </summary>
        /// <param name="items"></param>
        /// <param name="legacyCliModel">The legacy command line arguments</param>
        /// <returns></returns>
        private IEnumerable<DataSetWriterModel> ToDataSetWriters(
            IEnumerable<PublishedNodesEntryModel> items, LegacyCliModel legacyCliModel) {
            if (items == null) {
                return Enumerable.Empty<DataSetWriterModel>();
            }
            try {
                return items
                    // Group by connection
                    .GroupBy(item => new ConnectionModel {
                        OperationTimeout = legacyCliModel.OperationTimeout,
                        Endpoint = new EndpointModel {
                            Url = item.EndpointUrl.OriginalString,
                            SecurityMode = item.UseSecurity == false ?
                                        SecurityMode.None : SecurityMode.Best
                        },
                        User = item.OpcAuthenticationMode != OpcAuthenticationMode.UsernamePassword ?
                                null : ToUserNamePasswordCredentialAsync(item).Result
                        },
                        // Select and batch nodes into published data set sources
                        item => GetNodeModels(item, legacyCliModel.ScaleTestCount.GetValueOrDefault(1)),
                        // Comparer for connection information
                        new FuncCompare<ConnectionModel>((x, y) => x.IsSameAs(y)))
                    .Select(group => group
                        // Flatten all nodes for the same connection and group by publishing interval
                        // then batch in chunks for max 1000 nodes and create data sets from those.
                        .Flatten()
                        .GroupBy(n => n.OpcPublishingInterval)
                        .SelectMany(n => n
                            .Distinct((a, b) => a.Id == b.Id && a.DisplayName == b.DisplayName &&
                                        a.OpcSamplingInterval == b.OpcSamplingInterval)
                            .Batch(1000))
                        .Select(opcNodes => new PublishedDataSetSourceModel {
                            Connection = group.Key.Clone(),
                            SubscriptionSettings = new PublishedDataSetSourceSettingsModel {
                                PublishingInterval = GetPublishingIntervalFromNodes(opcNodes, legacyCliModel),
                                ResolveDisplayName = legacyCliModel.FetchOpcNodeDisplayName
                            },
                            PublishedVariables = new PublishedDataItemsModel {
                                PublishedData = opcNodes
                                    .Select(node => new PublishedDataSetVariableModel {
                                        // this is the monitopred item id, not the nodeId!
                                        // Use the display name if any otherwisw the nodeId
                                        Id = string.IsNullOrEmpty(node.DisplayName)
                                            ? node.Id : node.DisplayName,
                                        PublishedVariableNodeId = node.Id,
                                        PublishedVariableDisplayName = node.DisplayName,
                                        SamplingInterval = node.OpcSamplingIntervalTimespan ??
                                            legacyCliModel.DefaultSamplingInterval ?? (TimeSpan?)null,
                                        HeartbeatInterval = node.HeartbeatInterval == null ? (TimeSpan?)null :
                                            TimeSpan.FromSeconds(node.HeartbeatInterval.Value),
                                        // Force the queue size to 2 so that we avoid data
                                        // loss in case publishing interval and sampling interval are equal
                                        QueueSize = 2
                                        // TODO: skip first?
                                        // SkipFirst = opcNode.SkipFirst,
                                    })
                                    .ToList()
                            }
                        }))
                    .SelectMany(dataSetSourceBatches => dataSetSourceBatches
                        .Select(dataSetSource => new DataSetWriterModel {
                            DataSetWriterId = $"{dataSetSource.Connection.Endpoint.Url}_" +
                                $"{dataSetSource.GetHashSafe()}",
                            DataSet = new PublishedDataSetModel {
                                DataSetSource = dataSetSource.Clone(),
                            },
                            DataSetFieldContentMask =
                                DataSetFieldContentMask.StatusCode |
                                DataSetFieldContentMask.SourceTimestamp |
                                (legacyCliModel.FullFeaturedMessage ? DataSetFieldContentMask.ServerTimestamp : 0) |
                                DataSetFieldContentMask.NodeId |
                                DataSetFieldContentMask.DisplayName |
                                DataSetFieldContentMask.ApplicationUri |
                                (legacyCliModel.FullFeaturedMessage ? DataSetFieldContentMask.EndpointUrl : 0) |
                                (legacyCliModel.FullFeaturedMessage ? DataSetFieldContentMask.ExtensionFields : 0),
                            MessageSettings = new DataSetWriterMessageSettingsModel() {
                                DataSetMessageContentMask =
                                    (legacyCliModel.FullFeaturedMessage ? DataSetContentMask.Timestamp : 0) |
                                    DataSetContentMask.MetaDataVersion |
                                    DataSetContentMask.DataSetWriterId |
                                    DataSetContentMask.MajorVersion |
                                    DataSetContentMask.MinorVersion |
                                    (legacyCliModel.FullFeaturedMessage ? DataSetContentMask.SequenceNumber : 0)
                            }
                        }))
                    .ToList();
            }
            catch (Exception ex) {
                _logger.Error(ex, "failed to convert the published nodes.");
            }
            return Enumerable.Empty<DataSetWriterModel>();
        }

        /// <summary>
        /// Get the node models from entry
        /// </summary>
        /// <param name="item"></param>
        /// <param name="scaleTestCount"></param>
        /// <returns></returns>
        private IEnumerable<OpcNodeModel> GetNodeModels(PublishedNodesEntryModel item,
            int scaleTestCount = 1) {

            if (item.OpcNodes != null) {
                foreach (var node in item.OpcNodes) {
                    if (string.IsNullOrEmpty(node.Id)) {
                        node.Id = node.ExpandedNodeId;
                    }
                    if (string.IsNullOrEmpty(node.DisplayName)) {
                        node.DisplayName = node.Id;
                    }
                    if (scaleTestCount == 1) {
                        yield return node;
                    }
                    else {
                        for (var i = 0; i < scaleTestCount; i++) {
                            yield return new OpcNodeModel {
                                Id = node.Id,
                                DisplayName = $"{node.DisplayName}_{i}",
                                ExpandedNodeId = node.ExpandedNodeId,
                                HeartbeatInterval = node.HeartbeatInterval,
                                HeartbeatIntervalTimespan = node.HeartbeatIntervalTimespan,
                                OpcPublishingInterval = node.OpcPublishingInterval,
                                OpcPublishingIntervalTimespan = node.OpcPublishingIntervalTimespan,
                                OpcSamplingInterval = node.OpcSamplingInterval,
                                OpcSamplingIntervalTimespan = node.OpcSamplingIntervalTimespan,
                                SkipFirst = node.SkipFirst
                            };
                        }
                    }
                }
            }
            if (item.NodeId?.Identifier != null) {
                yield return new OpcNodeModel {
                    Id = item.NodeId.Identifier,
                };
            }
        }

        /// <summary>
        /// Extract publishing interval from nodes
        /// </summary>
        /// <param name="opcNodes"></param>
        /// <param name="legacyCliModel">The legacy command line arguments</param>
        /// <returns></returns>
        private static TimeSpan? GetPublishingIntervalFromNodes(IEnumerable<OpcNodeModel> opcNodes,
            LegacyCliModel legacyCliModel) {
            var interval = opcNodes
                .FirstOrDefault(x => x.OpcPublishingInterval != null)?.OpcPublishingIntervalTimespan;
            return interval ?? legacyCliModel.DefaultPublishingInterval;
        }

        /// <summary>
        /// Convert to credential model
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private async Task<CredentialModel> ToUserNamePasswordCredentialAsync(
            PublishedNodesEntryModel entry) {
            var user = entry.OpcAuthenticationUsername;
            var password = entry.OpcAuthenticationPassword;
            if (string.IsNullOrEmpty(user)) {
                if (_cryptoProvider == null || string.IsNullOrEmpty(entry.EncryptedAuthUsername)) {
                    return null;
                }

                const string kInitializationVector = "alKGJdfsgidfasdO"; // See previous publisher
                var userBytes = await _cryptoProvider.DecryptAsync(kInitializationVector,
                    Convert.FromBase64String(entry.EncryptedAuthUsername));
                user = Encoding.UTF8.GetString(userBytes);
                if (entry.EncryptedAuthPassword != null) {
                    var passwordBytes = await _cryptoProvider.DecryptAsync(kInitializationVector,
                        Convert.FromBase64String(entry.EncryptedAuthPassword));
                    password = Encoding.UTF8.GetString(passwordBytes);
                }
            }
            return new CredentialModel {
                Type = CredentialType.UserName,
                Value = _serializer.FromObject(new { user, password })
            };
        }

        /// <summary>
        /// Describing an entry in the node list
        /// </summary>
        [DataContract]
        public class OpcNodeModel {

            /// <summary> Node Identifier </summary>
            [DataMember(EmitDefaultValue = false)]
            public string Id { get; set; }

            /// <summary> Also </summary>
            [DataMember(EmitDefaultValue = false)]
            public string ExpandedNodeId { get; set; }

            /// <summary> Sampling interval </summary>
            [DataMember(EmitDefaultValue = false)]
            public int? OpcSamplingInterval { get; set; }

            /// <summary>
            /// OpcSamplingInterval as TimeSpan.
            /// </summary>
            [IgnoreDataMember]
            public TimeSpan? OpcSamplingIntervalTimespan {
                get => OpcSamplingInterval.HasValue ?
                    TimeSpan.FromMilliseconds(OpcSamplingInterval.Value) : (TimeSpan?)null;
                set => OpcSamplingInterval = value != null ?
                    (int)value.Value.TotalMilliseconds : (int?)null;
            }

            /// <summary> Publishing interval </summary>
            [DataMember(EmitDefaultValue = false)]
            public int? OpcPublishingInterval { get; set; }

            /// <summary>
            /// OpcPublishingInterval as TimeSpan.
            /// </summary>
            [IgnoreDataMember]
            public TimeSpan? OpcPublishingIntervalTimespan {
                get => OpcPublishingInterval.HasValue ?
                    TimeSpan.FromMilliseconds(OpcPublishingInterval.Value) : (TimeSpan?)null;
                set => OpcPublishingInterval = value != null ?
                    (int)value.Value.TotalMilliseconds : (int?)null;
            }

            /// <summary> Display name </summary>
            [DataMember(EmitDefaultValue = false)]
            public string DisplayName { get; set; }

            /// <summary> Heartbeat </summary>
            [DataMember(EmitDefaultValue = false)]
            public int? HeartbeatInterval { get; set; }

            /// <summary>
            /// Heartbeat interval as TimeSpan.
            /// </summary>
            [IgnoreDataMember]
            public TimeSpan? HeartbeatIntervalTimespan {
                get => HeartbeatInterval.HasValue ?
                    TimeSpan.FromSeconds(HeartbeatInterval.Value) : (TimeSpan?)null;
                set => HeartbeatInterval = value != null ?
                    (int)value.Value.TotalSeconds : (int?)null;
            }

            /// <summary> Skip first value </summary>
            [DataMember(EmitDefaultValue = false)]
            public bool? SkipFirst { get; set; }
        }

        /// <summary>
        /// Node id serialized as object
        /// </summary>
        [DataContract]
        public class NodeIdModel {
            /// <summary> Identifier </summary>
            [DataMember(EmitDefaultValue = false)]
            public string Identifier { get; set; }
        }

        /// <summary>
        /// Contains the nodes which should be
        /// </summary>
        [DataContract]
        public class PublishedNodesEntryModel {

            /// <summary> The endpoint URL of the OPC UA server. </summary>
            [DataMember(IsRequired = true)]
            public Uri EndpointUrl { get; set; }

            /// <summary> Secure transport should be used to </summary>
            [DataMember(EmitDefaultValue = false)]
            public bool? UseSecurity { get; set; }

            /// <summary> The node to monitor in "ns=" syntax. </summary>
            [DataMember(EmitDefaultValue = false)]
            public NodeIdModel NodeId { get; set; }

            /// <summary> authentication mode </summary>
            [DataMember(EmitDefaultValue = false)]
            public OpcAuthenticationMode OpcAuthenticationMode { get; set; }

            /// <summary> encrypted username </summary>
            [DataMember(EmitDefaultValue = false)]
            public string EncryptedAuthUsername { get; set; }

            /// <summary> encrypted password </summary>
            [DataMember]
            public string EncryptedAuthPassword { get; set; }

            /// <summary> plain username </summary>
            [DataMember(EmitDefaultValue = false)]
            public string OpcAuthenticationUsername { get; set; }

            /// <summary> plain password </summary>
            [DataMember]
            public string OpcAuthenticationPassword { get; set; }

            /// <summary> Nodes defined in the collection. </summary>
            [DataMember(EmitDefaultValue = false)]
            public List<OpcNodeModel> OpcNodes { get; set; }
        }

        /// <summary>
        /// Enum that defines the authentication method
        /// </summary>
        [DataContract]
        public enum OpcAuthenticationMode {
            /// <summary> Anonymous authentication </summary>
            [EnumMember]
            Anonymous,
            /// <summary> Username/Password authentication </summary>
            [EnumMember]
            UsernamePassword
        }

        private readonly FileSystemWatcher _fileSystemWatcher;
        private readonly LegacyCliModel _legacyCliModel;
        private readonly IWriterGroupProcessingEngine _engine;
        private readonly IJsonSerializer _serializer;
        private readonly ISecureElement _cryptoProvider;
        private readonly IIdentity _identity;
        private readonly ILogger _logger;
        private readonly object _fileLock = new object();
        private string _lastKnownFileHash;
        private HashSet<string> _lastSetOfWriterIds;
    }
}
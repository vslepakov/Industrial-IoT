// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Edge.Publisher.Services {
    using Microsoft.Azure.IIoT.Crypto;
    using Microsoft.Azure.IIoT.Serializers;
    using Microsoft.Azure.IIoT.Module;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Linq;

    /// <summary>
    /// Loads published nodes file and configures the engine
    /// </summary>
    public class PublishedNodesLoader {

        /// <summary>
        /// Create published nodes file loader
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="serializer"></param>
        /// <param name="identity"></param>
        /// <param name="legacyCliModelProvider"></param>
        /// <param name="logger"></param>
        /// <param name="cryptoProvider"></param>
        public PublishedNodesLoader(IWriterGroupProcessingEngine engine,
            IJsonSerializer serializer, ILegacyCliModelProvider legacyCliModelProvider,
            IIdentity identity, ILogger logger, ISecureElement cryptoProvider = null) {

            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _lastSetOfWriterIds = new HashSet<string>();

            _file = new PublishedNodesFile(serializer, legacyCliModelProvider,
                logger, cryptoProvider);
            if (string.IsNullOrWhiteSpace(_file.FileName)) {
                throw new ArgumentNullException(nameof(_file.FileName));
            }

            var directory = Path.GetDirectoryName(_file.FileName);
            if (string.IsNullOrWhiteSpace(directory)) {
                directory = Environment.CurrentDirectory;
            }

            RefreshFromFile();
            var file = Path.GetFileName(_file.FileName);
            _fileSystemWatcher = new FileSystemWatcher(directory, file);
            _fileSystemWatcher.Changed += (s, e) => RefreshFromFile();
            _fileSystemWatcher.EnableRaisingEvents = true;

           // _engine.DiagnosticsInterval = group.Priority;
        }

        /// <summary>
        /// Reconfigures the engine from the published nodes file stream
        /// </summary>
        /// <param name="reader"></param>
        internal void ConfigureEngineFromStream(TextReader reader) {
            var publisherId = $"LegacyPublisher_{_identity.DeviceId}_{_identity.ModuleId}";
            var group = _file.Read(publisherId, reader);

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
                    var currentFileHash = GetChecksum(_file.FileName);
                    if (currentFileHash != _lastKnownFileHash) {
                        _logger.Information("File {publishedNodesFile} has changed, reloading...",
                            _file.FileName);
                        _lastKnownFileHash = currentFileHash;
                        using (var reader = new StreamReader(_file.FileName)) {
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


        private readonly FileSystemWatcher _fileSystemWatcher;
        private readonly IWriterGroupProcessingEngine _engine;
        private readonly IIdentity _identity;
        private readonly PublishedNodesFile _file;
        private readonly ILogger _logger;
        private readonly object _fileLock = new object();
        private string _lastKnownFileHash;
        private HashSet<string> _lastSetOfWriterIds;
    }
}
// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Registry.Services {
    using Microsoft.Azure.IIoT.OpcUa.Registry.Models;
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Models;
    using Microsoft.Azure.IIoT.OpcUa.Publisher;
    using Microsoft.Azure.IIoT.Hub.Models;
    using Microsoft.Azure.IIoT.Hub;
    using Microsoft.Azure.IIoT.Serializers;
    using Microsoft.Azure.IIoT.Exceptions;
    using System.Threading.Tasks;
    using Serilog;
    using System;
    using System.Threading;
    using System.Collections.Generic;

    /// <summary>
    /// Manages writer group twins
    /// </summary>
    public sealed class WriterGroupTwins : IWriterGroupRegistryListener,
        IDataSetWriterRegistryListener {

        /// <summary>
        /// Create registry services
        /// </summary>
        /// <param name="iothub"></param>
        /// <param name="logger"></param>
        /// <param name="serializer"></param>
        public WriterGroupTwins(IIoTHubTwinServices iothub, IJsonSerializer serializer,
            ILogger logger) {
            _iothub = iothub ?? throw new ArgumentNullException(nameof(iothub));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        /// <inheritdoc/>
        public async Task OnWriterGroupAddedAsync(PublisherOperationContextModel context,
            WriterGroupInfoModel writerGroup) {

            var group = writerGroup.ToWriterGroupRegistration();
            await _iothub.CreateOrUpdateAsync(group.ToDeviceTwin(_serializer),
                false, CancellationToken.None);
        }

        /// <inheritdoc/>
        public async Task OnWriterGroupUpdatedAsync(PublisherOperationContextModel context,
            WriterGroupInfoModel writerGroup) {
            if (writerGroup?.WriterGroupId == null) {
                // Should not happen
                throw new ArgumentNullException(nameof(writerGroup.WriterGroupId));
            }
            while (true) {
                try {
                    var twin = await _iothub.FindAsync(
                        WriterGroupRegistrationEx.ToDeviceId(writerGroup.WriterGroupId));
                    if (twin == null) {
                        _logger.Warning("Missed add group event - try recreating twin...");
                        twin = await _iothub.CreateOrUpdateAsync(
                            writerGroup.ToWriterGroupRegistration().ToDeviceTwin(_serializer),
                            false, CancellationToken.None);
                        return; // done
                    }
                    // Convert to writerGroup registration
                    var registration = twin.ToEntityRegistration() as WriterGroupRegistration;
                    if (registration == null) {
                        _logger.Fatal("Unexpected - twin is not a writerGroup registration.");
                        return; // nothing else to do other than delete and recreate.
                    }
                    twin = await _iothub.PatchAsync(registration.Patch(
                        writerGroup.ToWriterGroupRegistration(), _serializer));
                    break;
                }
                catch (ResourceOutOfDateException ex) {
                    // Retry create/update
                    _logger.Debug(ex, "Retry updating writerGroup...");
                }
            }
        }

        /// <inheritdoc/>
        public async Task OnDataSetWriterAddedAsync(PublisherOperationContextModel context,
            DataSetWriterInfoModel dataSetWriter) {
            var writerGroupId = dataSetWriter?.WriterGroupId;
            if (string.IsNullOrEmpty(writerGroupId)) {
                // Should not happen
                throw new ArgumentNullException(nameof(dataSetWriter.WriterGroupId));
            }
            await PatchTwinAsync(WriterGroupRegistrationEx.ToDeviceId(writerGroupId),
                dataSetWriter.DataSetWriterId);
        }

        /// <inheritdoc/>
        public async Task OnDataSetWriterUpdatedAsync(PublisherOperationContextModel context,
            string dataSetWriterId, DataSetWriterInfoModel dataSetWriter) {
            var writerGroupId = dataSetWriter?.WriterGroupId;
            if (string.IsNullOrEmpty(writerGroupId)) {
                // Find twins where this dataset is defined - should only be one and patch it
                var twins = await _iothub.QueryAllDeviceTwinsAsync(
                    $"SELECT * FROM devices WHERE " +
                    $"IS_DEFINED(properties.desired.{IdentityType.DataSet}_{dataSetWriterId}) AND " +
                    $"tags.{nameof(EntityRegistration.DeviceType)} = '{IdentityType.WriterGroup}' ");
                foreach (var twin in twins) {
                    await PatchTwinAsync(twin.Id, dataSetWriterId);
                }
                return;
            }
            await PatchTwinAsync(WriterGroupRegistrationEx.ToDeviceId(writerGroupId),
                dataSetWriterId);
        }

        /// <inheritdoc/>
        public async Task OnDataSetWriterRemovedAsync(PublisherOperationContextModel context,
            DataSetWriterInfoModel dataSetWriter) {
            if (string.IsNullOrEmpty(dataSetWriter?.WriterGroupId)) {
                // Should not happen
                throw new ArgumentNullException(nameof(dataSetWriter.WriterGroupId));
            }
            await PatchTwinAsync(WriterGroupRegistrationEx.ToDeviceId(dataSetWriter.WriterGroupId),
                dataSetWriter.DataSetWriterId, true);
        }

        /// <inheritdoc/>
        public async Task OnWriterGroupRemovedAsync(PublisherOperationContextModel context,
            string writerGroupId) {
            try {
                // Force delete
                await _iothub.DeleteAsync(WriterGroupRegistrationEx.ToDeviceId(writerGroupId));
            }
            catch (Exception ex) {
                // Retry create/update
                _logger.Error(ex, "Deleting writerGroup failed...");
            }
        }

        /// <summary>
        /// Add or remove the writer from the group
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="dataSetWriterId"></param>
        /// <param name="remove"></param>
        /// <returns></returns>
        private async Task PatchTwinAsync(string deviceId, string dataSetWriterId,
            bool remove = false) {
            try {
                await _iothub.PatchAsync(new DeviceTwinModel {
                    Id = deviceId,
                    Properties = new TwinPropertiesModel {
                        Desired = new Dictionary<string, VariantValue> {
                            [IdentityType.DataSet + "_" + dataSetWriterId] =
                        remove ? null : _serializer.FromObject(
                            new {
                                DataSetWriterId = dataSetWriterId,
                                // This will cause an update
                                Tag = Guid.NewGuid().ToString(),
                            })
                        }
                    }
                });
            }
            catch (Exception ex) {
                // Retry create/update
                _logger.Error(ex, "Updating writer table in writerGroup failed...");
            }
        }

        private readonly IIoTHubTwinServices _iothub;
        private readonly IJsonSerializer _serializer;
        private readonly ILogger _logger;
    }
}

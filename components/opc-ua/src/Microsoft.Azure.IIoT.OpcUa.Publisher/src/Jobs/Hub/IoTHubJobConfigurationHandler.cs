// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Publisher.Jobs {
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Models;
    using Microsoft.Azure.IIoT.Hub;
    using Microsoft.Azure.IIoT.Hub.Models;
    using System;
    using System.Threading.Tasks;
    using Serilog;

    /// <summary>
    /// IoT hub based job event handler
    /// </summary>
    public class IoTHubJobConfigurationHandler : IJobEventHandler {

        /// <summary>
        /// Create event handler
        /// </summary>
        /// <param name="ioTHubTwinServices"></param>
        /// <param name="logger"></param>
        public IoTHubJobConfigurationHandler(IIoTHubTwinServices ioTHubTwinServices,
            ILogger logger) {
            _ioTHubTwinServices = ioTHubTwinServices;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task OnJobCreatedAsync(IJobService manager, JobInfoModel job) {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task OnJobCreatingAsync(IJobService manager, JobInfoModel job) {
            if (job.JobConfiguration == null) {
                return;
            }
            try {
                var jobDeviceId = job.Id;
                var deviceTwin = await _ioTHubTwinServices.FindAsync(jobDeviceId);
                if (deviceTwin == null) {
                    deviceTwin = new DeviceTwinModel {
                        Id = jobDeviceId
                    };
                    await _ioTHubTwinServices.CreateOrUpdateAsync(deviceTwin, true);
                }
                var cs = await _ioTHubTwinServices.GetConnectionStringAsync(deviceTwin.Id);
                job.JobConfiguration.ConnectionString = cs.ToString();
                _logger.Debug("Added connection string to job {id}", jobDeviceId);
            }
            catch (Exception ex) {
                _logger.Error(ex, "Error while creating IoT Device.");
            }
        }

        /// <inheritdoc/>
        public Task OnJobDeletingAsync(IJobService manager, JobInfoModel job) {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task OnJobDeletedAsync(IJobService manager, JobInfoModel job) {
            var jobDeviceId = job.Id;
            try {
                await _ioTHubTwinServices.DeleteAsync(jobDeviceId);
            }
            catch (Exception ex) {
                _logger.Error(ex, "Failed to delete device job {id}", jobDeviceId);
            }
        }

        private readonly IIoTHubTwinServices _ioTHubTwinServices;
        private readonly ILogger _logger;
    }
}
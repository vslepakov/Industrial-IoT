// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Publisher.Storage.Default {
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Models;
    using System.Linq;

    /// <summary>
    /// Job model extensions
    /// </summary>
    public static class LegacyJobModelEx {

        /// <summary>
        /// Convert to storage
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static LegacyDemandModel ToDocumentModel(this DemandModel model) {
            if (model == null) {
                return null;
            }
            return new LegacyDemandModel {
                Key = model.Key,
                Value = model.Value,
                Operator = model.Operator
            };
        }

        /// <summary>
        /// Convert to Service model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static DemandModel ToServiceModel(this LegacyDemandModel model) {
            if (model == null) {
                return null;
            }
            return new DemandModel {
                Key = model.Key,
                Value = model.Value,
                Operator = model.Operator
            };
        }

        /// <summary>
        /// Convert to storage
        /// </summary>
        /// <param name="job"></param>
        /// <param name="etag"></param>
        /// <returns></returns>
        public static LegacyJobDocument ToDocumentModel(this JobInfoModel job,
            string etag = null) {
            if (job?.LifetimeData == null) {
                return null;
            }
            return new LegacyJobDocument {
                ETag = etag,
                Id = job.Id,
                JobId = job.Id,
                Name = job.Name,
                JobConfiguration = new LegacyJobConfigDocument {
                    JobId = job.Id,
                    Job = job.JobConfiguration // TODO - clone
                },
                Demands = job.Demands?.Select(d => d.ToDocumentModel()).ToList(),
                DesiredActiveAgents = job.RedundancyConfig?.DesiredActiveAgents ?? 1,
                DesiredPassiveAgents = job.RedundancyConfig?.DesiredPassiveAgents ?? 0,
                Created = job.LifetimeData.Created,
                ProcessingStatus = job.LifetimeData.ProcessingStatus?
                    .ToDictionary(k => k.Key, v => v.Value.ToDocumentModel()),
                Status = job.LifetimeData.Status,
                Updated = job.LifetimeData.Updated
            };
        }

        /// <summary>
        /// Create document
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static LegacyProcessingStatus ToDocumentModel(
            this ProcessingStatusModel model) {
            return new LegacyProcessingStatus {
                LastKnownHeartbeat = model.LastKnownHeartbeat,
                LastKnownState = model.LastKnownState,
                ProcessMode = model.ProcessMode
            };
        }

        /// <summary>
        /// Create framework model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ProcessingStatusModel ToFrameworkModel(
            this LegacyProcessingStatus model) {
            return new ProcessingStatusModel {
                LastKnownHeartbeat = model.LastKnownHeartbeat,
                LastKnownState = model.LastKnownState,
                ProcessMode = model.ProcessMode
            };
        }

        /// <summary>
        /// Convert to Service model
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static JobInfoModel ToFrameworkModel(this LegacyJobDocument document) {
            if (document == null) {
                return null;
            }
            return new JobInfoModel {
                Id = document.JobId,
                Name = document.Name,
                GenerationId = document.ETag,
                JobConfiguration = document.JobConfiguration?.Job,
                Demands = document.Demands?.Select(d => d.ToServiceModel()).ToList(),
                RedundancyConfig = new RedundancyConfigModel {
                    DesiredActiveAgents = document.DesiredActiveAgents,
                    DesiredPassiveAgents = document.DesiredPassiveAgents
                },
                LifetimeData = new JobLifetimeDataModel {
                    Created = document.Created,
                    ProcessingStatus = document.ProcessingStatus?
                        .ToDictionary(k => k.Key, v => v.Value.ToFrameworkModel()),
                    Status = document.Status,
                    Updated = document.Updated
                }
            };
        }
    }
}
// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Publisher.Storage.Default {
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Models;
    using System;
    using System.Linq;

    /// <summary>
    /// Writer Group model extensions
    /// </summary>
    public static class WriterGroupDocumentEx {

        /// <summary>
        /// Convert to storage
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static WriterGroupDocument ToDocumentModel(this WriterGroupInfoModel model) {
            if (model == null) {
                return null;
            }
            return new WriterGroupDocument {
                BatchSize = model.BatchSize,
                PublishingInterval = model.PublishingInterval,
                DataSetOrdering = model.MessageSettings?.DataSetOrdering,
                ETag = model.GenerationId,
                GroupVersion = model.MessageSettings?.GroupVersion,
                HeaderLayoutUri = model.HeaderLayoutUri,
                Id = model.WriterGroupId,
                KeepAliveTime = model.KeepAliveTime,
                LocaleIds = model.LocaleIds?.ToList(),
                MaxNetworkMessageSize = model.MaxNetworkMessageSize,
                MessageType = model.MessageType,
                Name = model.Name,
                NetworkMessageContentMask = model.MessageSettings?.NetworkMessageContentMask,
                Priority = model.Priority,
                PublishingOffset = model.MessageSettings?.PublishingOffset,
                SamplingOffset = model.MessageSettings?.SamplingOffset,
                SiteId = model.SiteId,
                Updated = model.Updated?.Time ?? DateTime.UtcNow,
                UpdatedAuditId = model.Updated?.AuthorityId,
                Created = model.Created?.Time ?? DateTime.UtcNow,
                CreatedAuditId = model.Created?.AuthorityId,
                ClassType = WriterGroupDocument.ClassTypeName
            };
        }

        /// <summary>
        /// Convert to Service model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static WriterGroupInfoModel ToFrameworkModel(this WriterGroupDocument model) {
            if (model == null) {
                return null;
            }
            return new WriterGroupInfoModel {
                BatchSize = model.BatchSize,
                PublishingInterval = model.PublishingInterval,
                GenerationId = model.ETag,
                HeaderLayoutUri = model.HeaderLayoutUri,
                WriterGroupId = model.Id,
                KeepAliveTime = model.KeepAliveTime,
                LocaleIds = model.LocaleIds?.ToList(),
                MaxNetworkMessageSize = model.MaxNetworkMessageSize,
                MessageType = model.MessageType,
                Name = model.Name,
                Priority = model.Priority,
                SiteId = model.SiteId,
                MessageSettings = new WriterGroupMessageSettingsModel {
                    DataSetOrdering = model.DataSetOrdering,
                    GroupVersion = model.GroupVersion,
                    NetworkMessageContentMask = model.NetworkMessageContentMask,
                    PublishingOffset = model.PublishingOffset,
                    SamplingOffset = model.SamplingOffset
                },
                Updated = model.Updated == null ? null : new PublisherOperationContextModel {
                    Time = model.Updated.Value,
                    AuthorityId = model.UpdatedAuditId
                },
                Created = model.Created == null ? null : new PublisherOperationContextModel {
                    Time = model.Created.Value,
                    AuthorityId = model.CreatedAuditId
                },
                SecurityGroupId = null,
                SecurityKeyServices = null,
                SecurityMode = null // TODO
            };
        }
    }
}
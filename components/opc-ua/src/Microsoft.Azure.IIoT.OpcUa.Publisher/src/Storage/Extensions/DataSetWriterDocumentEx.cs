// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Publisher.Storage.Default {
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Models;

    /// <summary>
    /// DataSet Writer model extensions
    /// </summary>
    public static class DataSetWriterDocumentEx {

        /// <summary>
        /// Convert to storage
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static DataSetWriterDocument ToDocumentModel(this DataSetWriterInfoModel model) {
            if (model == null) {
                return null;
            }
            return new DataSetWriterDocument {
                Id = model.DataSetWriterId,
                ETag = model.GenerationId,
                WriterGroupId = model.WriterGroupId,
                DataSetFieldContentMask = model.DataSetFieldContentMask,
                DataSetMetaDataSendInterval = model.DataSetMetaDataSendInterval,
                KeyFrameCount = model.KeyFrameCount,
                KeyFrameInterval = model.KeyFrameInterval,
                ConfiguredSize = model.MessageSettings?.ConfiguredSize,
                DataSetMessageContentMask = model.MessageSettings?.DataSetMessageContentMask,
                DataSetOffset = model.MessageSettings?.DataSetOffset,
                NetworkMessageNumber = model.MessageSettings?.NetworkMessageNumber,
                CredentialType = model.DataSet?.User?.Type,
                Credential = model.DataSet?.User?.Value,
                EndpointId = model.DataSet?.EndpointId,
                DiagnosticsLevel = model.DataSet?.DiagnosticsLevel,
                OperationTimeout = model.DataSet?.OperationTimeout,
                Updated = model.Updated?.Time,
                UpdatedAuditId = model.Updated?.AuthorityId,
                Created = model.Created?.Time,
                CreatedAuditId = model.Created?.AuthorityId,
                ExtensionFields = model.DataSet?.ExtensionFields,
                DataSetName = model.DataSet?.Name,
                SubscriptionLifeTimeCount = model.DataSet?.SubscriptionSettings?.LifeTimeCount,
                MaxKeepAliveCount = model.DataSet?.SubscriptionSettings?.MaxKeepAliveCount,
                MaxNotificationsPerPublish =
                    model.DataSet?.SubscriptionSettings?.MaxNotificationsPerPublish,
                SubscriptionPriority = model.DataSet?.SubscriptionSettings?.Priority,
                PublishingInterval = model.DataSet?.SubscriptionSettings?.PublishingInterval,
                ResolveDisplayName = model.DataSet?.SubscriptionSettings?.ResolveDisplayName,
                ClassType = DataSetWriterDocument.ClassTypeName
            };
        }

        /// <summary>
        /// Convert to Service model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static DataSetWriterInfoModel ToFrameworkModel(this DataSetWriterDocument model) {
            if (model == null) {
                return null;
            }
            return new DataSetWriterInfoModel {
                DataSetWriterId = model.Id,
                GenerationId = model.ETag,
                WriterGroupId = model.WriterGroupId,
                DataSet = new PublishedDataSetSourceInfoModel {
                    User = model.CredentialType == null ? null :
                        new Core.Models.CredentialModel {
                            Type = model.CredentialType,
                            Value = model.Credential
                        },
                    OperationTimeout = model.OperationTimeout,
                    DiagnosticsLevel = model.DiagnosticsLevel,
                    EndpointId = model.EndpointId,
                    ExtensionFields = model.ExtensionFields,
                    Name = model.DataSetName,
                    SubscriptionSettings = new PublishedDataSetSourceSettingsModel {
                        LifeTimeCount = model.SubscriptionLifeTimeCount,
                        MaxKeepAliveCount = model.MaxKeepAliveCount,
                        MaxNotificationsPerPublish = model.MaxNotificationsPerPublish,
                        Priority = model.SubscriptionPriority,
                        PublishingInterval = model.PublishingInterval,
                        ResolveDisplayName = model.ResolveDisplayName
                    }
                },
                DataSetFieldContentMask = model.DataSetFieldContentMask,
                DataSetMetaDataSendInterval = model.DataSetMetaDataSendInterval,
                KeyFrameCount = model.KeyFrameCount,
                KeyFrameInterval = model.KeyFrameInterval,
                MessageSettings = new DataSetWriterMessageSettingsModel {
                    ConfiguredSize = model.ConfiguredSize,
                    DataSetMessageContentMask = model.DataSetMessageContentMask,
                    DataSetOffset = model.DataSetOffset,
                    NetworkMessageNumber = model.NetworkMessageNumber
                },
                Created = model.Created == null ? null : new PublisherOperationContextModel {
                    AuthorityId = model.CreatedAuditId,
                    Time = model.Created.Value
                },
                Updated = model.Updated == null ? null : new PublisherOperationContextModel {
                    AuthorityId = model.UpdatedAuditId,
                    Time = model.Updated.Value
                }
            };
        }
    }
}
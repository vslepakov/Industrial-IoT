// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Publisher.Storage.Default {
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Models;
    using System;

    /// <summary>
    /// Variable model extensions
    /// </summary>
    public static class DataSetEntityDocumentEx {

        /// <summary>
        /// Create unique dataset variable id from dataset writer id
        /// </summary>
        /// <param name="dataSetWriterId"></param>
        /// <param name="variableId"></param>
        /// <returns></returns>
        public static string GetDocumentId(string dataSetWriterId,
            string variableId) {
            return dataSetWriterId + "_" + variableId;
        }

        /// <summary>
        /// Create unique dataset event id from dataset writer id
        /// </summary>
        /// <param name="dataSetWriterId"></param>
        /// <returns></returns>
        public static string GetDocumentId(string dataSetWriterId) {
            return dataSetWriterId + "_EventDefinition";
        }

        /// <summary>
        /// Convert to storage
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dataSetWriterId"></param>
        /// <returns></returns>
        public static DataSetEntityDocument ToDocumentModel(
            this PublishedDataSetEventsModel model, string dataSetWriterId) {
            if (model == null) {
                return null;
            }
            return new DataSetEntityDocument {
                ETag = model.GenerationId,
                DataSetWriterId = dataSetWriterId,
                Id = GetDocumentId(dataSetWriterId),
                BrowsePath = model.BrowsePath,
                DiscardNew = model.DiscardNew,
                MonitoringMode = model.MonitoringMode,
                QueueSize = model.QueueSize,
                TriggerId = model.TriggerId,
                EventNotifier = model.EventNotifier,
                FilterElements = model.Filter?.Elements,
                SelectedFields = model.SelectedFields,
                VariableId = null,
                Attribute = null,
                DataChangeFilter = null,
                DeadbandType = null,
                DeadbandValue = null,
                DisplayName = null,
                HeartbeatInterval = null,
                IndexRange = null,
                MetaDataProperties = null,
                NodeId = null,
                SamplingInterval = null,
                SubstituteValue = null,
                Updated = model.Updated?.Time ?? DateTime.UtcNow,
                UpdatedAuditId = model.Updated?.AuthorityId,
                Created = model.Created?.Time ?? DateTime.UtcNow,
                CreatedAuditId = model.Created?.AuthorityId,
                Type = DataSetEntityDocument.EventSet,
                ClassType = DataSetEntityDocument.ClassTypeName
            };
        }

        /// <summary>
        /// Convert to storage
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dataSetWriterId"></param>
        /// <returns></returns>
        public static DataSetEntityDocument ToDocumentModel(
            this PublishedDataSetVariableModel model, string dataSetWriterId) {
            if (model == null) {
                return null;
            }
            return new DataSetEntityDocument {
                ETag = model.GenerationId,
                Id = GetDocumentId(dataSetWriterId, model.Id),
                VariableId = model.Id,
                DataSetWriterId = dataSetWriterId,
                Attribute = model.Attribute,
                BrowsePath = model.BrowsePath,
                DataChangeFilter = model.DataChangeFilter,
                DeadbandType = model.DeadbandType,
                DeadbandValue = model.DeadbandValue,
                DiscardNew = model.DiscardNew,
                DisplayName = model.PublishedVariableDisplayName,
                HeartbeatInterval = model.HeartbeatInterval,
                IndexRange = model.IndexRange,
                MetaDataProperties = model.MetaDataProperties,
                MonitoringMode = model.MonitoringMode,
                NodeId = model.PublishedVariableNodeId,
                QueueSize = model.QueueSize,
                SamplingInterval = model.SamplingInterval,
                SubstituteValue = model.SubstituteValue?.Copy(),
                TriggerId = model.TriggerId,
                EventNotifier = null,
                FilterElements = null,
                SelectedFields = null,
                Updated = model.Updated?.Time ?? DateTime.UtcNow,
                UpdatedAuditId = model.Updated?.AuthorityId,
                Created = model.Created?.Time ?? DateTime.UtcNow,
                CreatedAuditId = model.Created?.AuthorityId,
                Type = DataSetEntityDocument.Variable,
                ClassType = DataSetEntityDocument.ClassTypeName
            };
        }

        /// <summary>
        /// Convert to Service model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static PublishedDataSetEventsModel ToEventDataSetModel(
            this DataSetEntityDocument model) {
            if (model == null) {
                return null;
            }
            return new PublishedDataSetEventsModel {
                GenerationId = model.ETag,
                Id = model.DataSetWriterId,
                BrowsePath = model.BrowsePath,
                DiscardNew = model.DiscardNew,
                MonitoringMode = model.MonitoringMode,
                QueueSize = model.QueueSize,
                TriggerId = model.TriggerId,
                EventNotifier = model.EventNotifier,
                Filter = new Core.Models.ContentFilterModel {
                    Elements = model.FilterElements,
                },
                SelectedFields = model.SelectedFields,
                Updated = model.Updated == null ? null : new PublisherOperationContextModel {
                    Time = model.Updated.Value,
                    AuthorityId = model.UpdatedAuditId
                },
                Created = model.Created == null ? null : new PublisherOperationContextModel {
                    Time = model.Created.Value,
                    AuthorityId = model.CreatedAuditId
                }
            };
        }

        /// <summary>
        /// Convert to Service model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static PublishedDataSetVariableModel ToDataSetVariableModel(
            this DataSetEntityDocument model) {
            if (model == null) {
                return null;
            }
            return new PublishedDataSetVariableModel {
                Id = model.VariableId,
                GenerationId = model.ETag,
                Attribute = model.Attribute,
                BrowsePath = model.BrowsePath,
                DataChangeFilter = model.DataChangeFilter,
                DeadbandType = model.DeadbandType,
                DeadbandValue = model.DeadbandValue,
                DiscardNew = model.DiscardNew,
                PublishedVariableDisplayName = model.DisplayName,
                HeartbeatInterval = model.HeartbeatInterval,
                IndexRange = model.IndexRange,
                MetaDataProperties = model.MetaDataProperties,
                MonitoringMode = model.MonitoringMode,
                PublishedVariableNodeId = model.NodeId,
                QueueSize = model.QueueSize,
                SamplingInterval = model.SamplingInterval,
                SubstituteValue = model.SubstituteValue?.Copy(),
                TriggerId = model.TriggerId,
                Updated = model.Updated == null ? null : new PublisherOperationContextModel {
                    Time = model.Updated.Value,
                    AuthorityId = model.UpdatedAuditId
                },
                Created = model.Created == null ? null : new PublisherOperationContextModel {
                    Time = model.Created.Value,
                    AuthorityId = model.CreatedAuditId
                }
            };
        }
    }
}
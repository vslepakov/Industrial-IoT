// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Api.Publisher.Models {
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Events.v2.Models;

    /// <summary>
    /// Event extensions
    /// </summary>
    public static class EventExtensions {

        /// <summary>
        /// Convert to api model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static WriterGroupEventApiModel ToApiModel(
            this WriterGroupEventModel model) {
            return new WriterGroupEventApiModel {
                EventType = (WriterGroupEventType)model.EventType,
                Id = model.Id,
                WriterGroup = model.WriterGroup.ToApiModel()
            };
        }

        /// <summary>
        /// Convert to api model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static DataSetWriterEventApiModel ToApiModel(
            this DataSetWriterEventModel model) {
            return new DataSetWriterEventApiModel {
                EventType = (DataSetWriterEventType)model.EventType,
                Id = model.Id,
                DataSetWriter = model.DataSetWriter.ToApiModel()
            };
        }
    }
}
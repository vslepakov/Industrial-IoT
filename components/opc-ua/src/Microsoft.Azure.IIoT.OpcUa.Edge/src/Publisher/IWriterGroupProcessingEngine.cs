// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Edge.Publisher {
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Models;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Writer group processor
    /// </summary>
    public interface IWriterGroupProcessingEngine {

        /// <summary>
        /// Message schema to use
        /// </summary>
        string MessageSchema { get; set; }

        /// <summary>
        /// Dataset writer group identifier
        /// </summary>
        string WriterGroupId { get; set; }

        /// <summary>
        /// Group version
        /// </summary>
        uint? GroupVersion { get; set; }

        /// <summary>
        /// Network message content
        /// </summary>
        NetworkMessageContentMask? NetworkMessageContentMask { get; set; }

        /// <summary>
        /// Max network message size
        /// </summary>
        uint? MaxNetworkMessageSize { get; set; }

        /// <summary>
        /// Header layout uri
        /// </summary>
        string HeaderLayoutUri { get; set; }

        /// <summary>
        /// Batch buffer size (Publisher extension)
        /// </summary>
        int? BatchSize { get; set; }

        /// <summary>
        /// Publishing interval
        /// </summary>
        TimeSpan? PublishingInterval { get; set; }

        /// <summary>
        /// Keep alive time
        /// </summary>
        TimeSpan? KeepAliveTime { get; set; }

        /// <summary>
        /// Uadp dataset ordering
        /// </summary>
        DataSetOrderingType? DataSetOrdering { get; set; }

        /// <summary>
        /// Uadp Sampling offset
        /// </summary>
        double? SamplingOffset { get; set; }

        /// <summary>
        /// Publishing offset for uadp messages
        /// </summary>
        List<double> PublishingOffset { get; set; }

        /// <summary>
        /// Priority of the writer group
        /// </summary>
        byte? Priority { get; set; }

        /// <summary>
        /// Diagnostic interval
        /// </summary>
        TimeSpan? DiagnosticsInterval { get; set; }

        /// <summary>
        /// Add writers to the group
        /// </summary>
        /// <param name="dataSetWriters"></param>
        /// <returns></returns>
        void AddWriters(IEnumerable<DataSetWriterModel> dataSetWriters);

        /// <summary>
        /// Remove writers from the group
        /// </summary>
        /// <param name="dataSetWriters"></param>
        /// <returns></returns>
        void RemoveWriters(IEnumerable<string> dataSetWriters);

        /// <summary>
        /// Remove all writers
        /// </summary>
        void RemoveAllWriters();
    }
}
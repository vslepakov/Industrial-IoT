// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Modules.OpcUa.Publisher.Controllers {
    using Microsoft.Azure.IIoT.Module.Framework;
    using Microsoft.Azure.IIoT.OpcUa.Api.Publisher.Models;
    using Microsoft.Azure.IIoT.OpcUa.Edge.Publisher;
    using Microsoft.Azure.IIoT.Hub;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Writer group settings controller
    /// </summary>
    [Version(1)]
    [Version(2)]
    public class WriterGroupSettingsController : ISettingsController {

        /// <summary>
        /// Network message schema to produce
        /// </summary>
        public string MessageSchema {
            get => _processor.MessageSchema;
            set => _processor.MessageSchema = value;
        }

        /// <summary>
        /// Dataset writer group identifier
        /// </summary>
        public string WriterGroupId {
            get => _processor.WriterGroupId;
            set => _processor.WriterGroupId = value;
        }

        /// <summary>
        /// Group version
        /// </summary>
        public uint? GroupVersion {
            get => _processor.GroupVersion;
            set => _processor.GroupVersion = value;
        }

        /// <summary>
        /// Max network message size
        /// </summary>
        public uint? MaxNetworkMessageSize {
            get => _processor.MaxNetworkMessageSize;
            set => _processor.MaxNetworkMessageSize = value;
        }

        /// <summary>
        /// Header layout uri
        /// </summary>
        public string HeaderLayoutUri {
            get => _processor.HeaderLayoutUri;
            set => _processor.HeaderLayoutUri = value;
        }

        /// <summary>
        /// Batch buffer size (Publisher extension)
        /// </summary>
        public int? BatchSize {
            get => _processor.BatchSize;
            set => _processor.BatchSize = value;
        }

        /// <summary>
        /// Publishing interval
        /// </summary>
        public TimeSpan? PublishingInterval {
            get => _processor.PublishingInterval;
            set => _processor.PublishingInterval = value;
        }

        /// <summary>
        /// Keep alive time
        /// </summary>
        public TimeSpan? KeepAliveTime {
            get => _processor.KeepAliveTime;
            set => _processor.KeepAliveTime = value;
        }

        /// <summary>
        /// Uadp Sampling offset
        /// </summary>
        public double? SamplingOffset {
            get => _processor.SamplingOffset;
            set => _processor.SamplingOffset = value;
        }

        /// <summary>
        /// Publishing offset for uadp messages
        /// </summary>
        public Dictionary<string, double> PublishingOffset {
            get => _processor.PublishingOffset.EncodeAsDictionary();
            set => _processor.PublishingOffset = value.DecodeAsList();
        }

        /// <summary>
        /// Priority of the writer group
        /// </summary>
        public byte? Priority {
            get => _processor.Priority;
            set => _processor.Priority = value;
        }

        /// <summary>
        /// Uadp dataset ordering
        /// </summary>
        public DataSetOrderingType? DataSetOrdering {
            get => (DataSetOrderingType?)_processor.DataSetOrdering;
            set => _processor.DataSetOrdering =
                (IIoT.OpcUa.Publisher.Models.DataSetOrderingType?)value;
        }

        /// <summary>
        /// Network message content
        /// </summary>
        public NetworkMessageContentMask? NetworkMessageContentMask {
            get => (NetworkMessageContentMask?)_processor.NetworkMessageContentMask;
            set => _processor.NetworkMessageContentMask =
                (IIoT.OpcUa.Publisher.Models.NetworkMessageContentMask?)value;
        }

        ///// <summary>
        ///// State of the processor
        ///// </summary>
        //public EndpointConnectivityState State {
        //    get => _processor.State;
        //    set { /* Only reporting */ }
        //}

        /// <summary>
        /// Create controller with service
        /// </summary>
        public WriterGroupSettingsController(IWriterGroupProcessingEngine processor) {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        private readonly IWriterGroupProcessingEngine _processor;
    }
}

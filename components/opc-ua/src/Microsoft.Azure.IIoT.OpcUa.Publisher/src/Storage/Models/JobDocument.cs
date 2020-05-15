// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Publisher.Storage.Default {
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Models;
    using Microsoft.Azure.IIoT.Serializers;
    using System.Runtime.Serialization;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Job document
    /// </summary>
    [DataContract]
    public class JobDocument {

        /// <summary>
        /// id
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Etag
        /// </summary>
        [DataMember(Name = "_etag")]
        public string ETag { get; set; }

        /// <summary>
        /// Document type
        /// </summary>
        [DataMember]
        public string ClassType { get; set; } = ClassTypeName;
        /// <summary/>
        public static readonly string ClassTypeName = "WriterGroupJob";

        /// <summary>
        /// Identifier of the writer group
        /// </summary>
        [DataMember]
        public string WriterGroupId { get; set; }

        /// <summary>
        /// Writer group generation last synchronized
        /// </summary>
        [DataMember]
        public string WriterGroupGenerationId { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Number of desired active agents
        /// </summary>
        [DataMember]
        public int DesiredActiveAgents { get; set; }

        /// <summary>
        /// Number of passive agents
        /// </summary>
        [DataMember]
        public int DesiredPassiveAgents { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        public JobStatus Status { get; set; }

        /// <summary>
        /// Processing status
        /// </summary>
        [DataMember]
        public Dictionary<string, ProcessingStatus> ProcessingStatus { get; set; }

        /// <summary>
        /// Updated at
        /// </summary>
        [DataMember]
        public DateTime Updated { get; set; }

        /// <summary>
        /// Created at
        /// </summary>
        [DataMember]
        public DateTime Created { get; set; }
    }

    /// <summary>
    /// processing status
    /// </summary>
    [DataContract]
    public class ProcessingStatus {

        /// <summary>
        /// Last known heartbeat
        /// </summary>
        [DataMember]
        public DateTime? LastKnownHeartbeat { get; set; }

        /// <summary>
        /// Last known state
        /// </summary>
        [DataMember]
        public VariantValue LastKnownState { get; set; }

        /// <summary>
        /// Processing mode
        /// </summary>
        [DataMember]
        public ProcessMode? ProcessMode { get; set; }
    }
}
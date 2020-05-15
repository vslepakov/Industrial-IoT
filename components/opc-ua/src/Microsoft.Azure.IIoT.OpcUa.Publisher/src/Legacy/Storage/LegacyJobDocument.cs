// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Publisher.Storage.Default {
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Models;
    using System.Runtime.Serialization;
    using System;
    using System.Collections.Generic;
    using Microsoft.Azure.IIoT.Serializers;


    /// <summary>
    /// Job model
    /// </summary>
    [DataContract]
    public class LegacyJobConfigDocument {

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
        public static readonly string ClassTypeName = "JobConfig";

        /// <summary>
        /// Identifier of the job document
        /// </summary>
        [DataMember]
        public string JobId { get; set; }

        /// <summary>
        /// Job description
        /// </summary>
        [DataMember]
        public WriterGroupJobModel Job { get; set; }
    }

    /// <summary>
    /// processing status
    /// </summary>
    [DataContract]
    public class LegacyProcessingStatus {

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

    /// <summary>
    /// Demand model
    /// </summary>
    [DataContract]
    public class LegacyDemandModel {

        /// <summary>
        /// Key
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// Match operator
        /// </summary>
        [DataMember]
        public DemandOperators? Operator { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        [DataMember]
        public string Value { get; set; }
    }

    /// <summary>
    /// Job model
    /// </summary>
    [DataContract]
    public class LegacyJobDocument {

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
        public static readonly string ClassTypeName = "Job";

        /// <summary>
        /// Identifier of the job document
        /// </summary>
        [DataMember]
        public string JobId { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Configuration type
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// Job configuration
        /// </summary>
        [DataMember]
        public LegacyJobConfigDocument JobConfiguration { get; set; }

        /// <summary>
        /// Demands
        /// </summary>
        [DataMember]
        public List<LegacyDemandModel> Demands { get; set; }

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
        public Dictionary<string, LegacyProcessingStatus> ProcessingStatus { get; set; }

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
}
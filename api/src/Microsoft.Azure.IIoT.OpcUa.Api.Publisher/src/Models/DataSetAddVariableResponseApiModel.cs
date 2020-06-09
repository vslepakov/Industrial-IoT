// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Api.Publisher.Models {
    using Microsoft.Azure.IIoT.OpcUa.Api.Core.Models;
    using System.Runtime.Serialization;

    /// <summary>
    /// Result of a variable registration
    /// </summary>
    [DataContract]
    public class DataSetAddVariableResponseApiModel {

        /// <summary>
        /// New id variable was registered under
        /// </summary>
        [DataMember(Name = "id", Order = 0)]
        public string Id { get; set; }

        /// <summary>
        /// Generation id
        /// </summary>
        [DataMember(Name = "generationId", Order = 1)]
        public string GenerationId { get; set; }

        /// <summary>
        /// Diagnostics information in case of partial success
        /// </summary>
        [DataMember(Name = "errorInfo", Order = 2,
            EmitDefaultValue = false)]
        public ServiceResultApiModel ErrorInfo { get; set; }
    }
}
// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Api.Publisher.Models {
    using System.Runtime.Serialization;

    /// <summary>
    /// Network message type
    /// </summary>
    [DataContract]
    public enum NetworkMessageType {

        /// <summary>
        /// Json encoding
        /// </summary>
        [EnumMember]
        Json,

        /// <summary>
        /// Uadp encoding
        /// </summary>
        [EnumMember]
        Uadp
    }
}

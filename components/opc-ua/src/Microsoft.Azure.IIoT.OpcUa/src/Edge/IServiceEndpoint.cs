// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Edge {

    /// <summary>
    /// Configuration of edge service endpoint url
    /// </summary>
    public interface IServiceEndpoint {

        /// <summary>
        /// Returns service endpoint url
        /// </summary>
        string ServiceEndpointUrl { get; }
    }
}
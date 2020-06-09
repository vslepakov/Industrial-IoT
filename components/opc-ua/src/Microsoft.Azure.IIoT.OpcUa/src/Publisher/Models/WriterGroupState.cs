// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Publisher {

    /// <summary>
    /// Writer group state
    /// </summary>
    public enum WriterGroupState {

        /// <summary>
        /// Publishing is suspended or pending
        /// </summary>
        Pending,

        /// <summary>
        /// Publishing is ongoing
        /// </summary>
        Publishing
    }
}
// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Publisher.Models {

    /// <summary>
    /// Job status
    /// </summary>
    public enum JobStatus {

        /// <summary>
        /// Active
        /// </summary>
        Active,

        /// <summary>
        /// Job completed
        /// </summary>
        Completed,

        /// <summary>
        /// Job cancelled
        /// </summary>
        Canceled,

        /// <summary>
        /// Error
        /// </summary>
        Error,

        /// <summary>
        /// Removed
        /// </summary>
        Deleted
    }
}
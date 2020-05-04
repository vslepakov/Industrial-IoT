// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Publisher {

    /// <summary>
    /// Common IIoT agent related twin properties
    /// </summary>
    public static class TwinProperties {

        /// <summary>
        /// Access connection string
        /// </summary>
        public const string ConnectionString = "connectionString";

        /// <summary>
        /// Job orchestrator url
        /// </summary>
        public const string JobOrchestratorUrl = nameof(JobOrchestratorUrl);
    }
}

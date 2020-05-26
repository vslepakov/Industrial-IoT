// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Publisher.Jobs.Runtime {
    using Microsoft.Azure.IIoT.Utils;
    using Microsoft.Extensions.Configuration;
    using System;

    /// <summary>
    /// Job Orchestrator configuration
    /// </summary>
    public class OrchestratorConfig : ConfigBase, IOrchestratorConfig {

        /// <summary>
        /// Keys
        /// </summary>
        private const string kJobStaleTimeKey = "JobStaleTime";

        /// <inheritdoc/>
        public TimeSpan JobStaleTime => GetDurationOrDefault(kJobStaleTimeKey,
            () => TimeSpan.FromMinutes(15));

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="configuration"></param>
        public OrchestratorConfig(IConfiguration configuration) :
            base(configuration) {
        }
    }
}
// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Publisher {
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Models;
    using System;
    using Autofac;

    /// <summary>
    /// Lifetime scope factory for processing engines
    /// </summary>
    public interface IProcessingEngineContainerFactory {

        /// <summary>
        /// Create container scope for a job.
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="publisherId"></param>
        /// <param name="job"></param>
        /// <returns></returns>
        Action<ContainerBuilder> GetJobContainerScope(string agentId,
            string publisherId, WriterGroupJobModel job);
    }
}
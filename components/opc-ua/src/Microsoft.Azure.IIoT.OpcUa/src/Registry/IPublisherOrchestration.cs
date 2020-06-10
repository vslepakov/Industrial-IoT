// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


namespace Microsoft.Azure.IIoT.OpcUa.Registry.Services {
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Orchestrates and places worker groups across publishers
    /// </summary>
    public interface IWorkerGroupOrchestration {

        /// <summary>
        /// Place worker groups
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task UpdateGroupPlacementAsync(CancellationToken token);
    }
}
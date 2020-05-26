// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Publisher {
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Models;
    using Microsoft.Azure.IIoT.Serializers;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Job processing engine
    /// </summary>
    public interface IProcessingEngine {

        /// <summary>
        /// Identifier of the engine
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Engine running
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Run engine
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RunAsync(CancellationToken cancellationToken);
    }
}
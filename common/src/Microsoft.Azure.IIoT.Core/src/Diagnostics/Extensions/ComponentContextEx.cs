// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Autofac {
    using System;
    using Prometheus;
    using Microsoft.Azure.IIoT.Utils;

    /// <summary>
    /// Component context extensions
    /// </summary>
    public static class ComponentContextEx {

        /// <summary>
        /// Start logging
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IDisposable StartMetricsServer(this IComponentContext context) {
            IMetricServer server = null;
            context.TryResolve(out server);
            return new Disposable(() => server?.Stop());
        }
    }
}

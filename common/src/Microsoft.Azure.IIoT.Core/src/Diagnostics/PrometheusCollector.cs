// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Diagnostics {
    using Microsoft.Azure.IIoT.Diagnostics.Runtime;
    using Autofac;

    /// <summary>
    /// Prometheus module
    /// </summary>
    public class PrometheusCollector : Module {

        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder) {

            // Register prometheus logging
            builder.RegisterType<PrometheusCollectorHost>()
                .AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<LogAnalyticsConfig>()
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<LogAnalyticsMetricsHandler>()
                .AsImplementedInterfaces().InstancePerLifetimeScope()
                .PropertiesAutowired(
                    PropertyWiringOptions.AllowCircularDependencies);
        }
    }
}

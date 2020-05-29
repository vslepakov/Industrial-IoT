// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Publisher {
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Services;
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Default;
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Storage.Default;
    using Autofac;

    /// <summary>
    /// Injected publisher services
    /// </summary>
    public sealed class PublisherServices : Module {

        /// <summary>
        /// Load the module
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder) {

            builder.RegisterType<WriterGroupServices>()
                .AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<DataSetWriterEventBroker>()
                .AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<WriterGroupEventBroker>()
                .AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<WriterGroupDatabase>()
                .AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DataSetWriterDatabase>()
                .AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DataSetEntityDatabase>()
                .AsImplementedInterfaces().SingleInstance();

            base.Load(builder);
        }
    }
}

// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Edge.Publisher.Runtime {
    using Microsoft.Azure.IIoT.OpcUa.Edge.Publisher.Engine;
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Models;
    using Microsoft.Azure.IIoT.OpcUa.Publisher;
    using Microsoft.Azure.IIoT.Module.Framework.Client;
    using Microsoft.Azure.IIoT.Serializers;
    using Autofac;
    using System;

    /// <summary>
    /// Container builder for data set writer jobs
    /// </summary>
    public class WriterGroupJobContainerFactory : IProcessingEngineContainerFactory {

        /// <summary>
        /// Create job scope factory
        /// </summary>
        /// <param name="clientConfig"></param>
        public WriterGroupJobContainerFactory(IModuleConfig clientConfig) {
            _clientConfig = clientConfig ?? throw new ArgumentNullException(nameof(clientConfig));
        }

        /// <inheritdoc/>
        public Action<ContainerBuilder> GetJobContainerScope(string agentId, string publisherId,
            WriterGroupJobModel job) {
            return builder => {
                // Register job configuration
                builder.RegisterInstance(job.ToWriterGroupJobConfiguration(publisherId))
                    .AsImplementedInterfaces();

                // Register default serializers...
                builder.RegisterModule<NewtonSoftJsonModule>();

                // Register processing engine - trigger, transform, sink
                builder.RegisterType<DataFlowProcessingEngine>()
                    .AsImplementedInterfaces().InstancePerLifetimeScope();
                builder.RegisterType<WriterGroupMessageTrigger>()
                    .AsImplementedInterfaces().InstancePerLifetimeScope();
                switch (job.MessagingMode) {
                    case MessagingMode.Samples:
                        builder.RegisterType<MonitoredItemMessageEncoder>()
                            .AsImplementedInterfaces().InstancePerLifetimeScope();
                        break;
                    case MessagingMode.PubSub:
                        builder.RegisterType<NetworkMessageEncoder>()
                            .AsImplementedInterfaces().InstancePerLifetimeScope();
                        break;
                    default:
                        builder.RegisterType<MonitoredItemMessageEncoder>()
                            .AsImplementedInterfaces().InstancePerLifetimeScope();
                        break;
                }
                builder.RegisterType<IoTHubMessageSink>()
                    .AsImplementedInterfaces().InstancePerLifetimeScope();
                builder.RegisterInstance(_clientConfig.Clone(job.ConnectionString))
                    .AsImplementedInterfaces();
            };
        }

        private readonly IModuleConfig _clientConfig;
    }
}
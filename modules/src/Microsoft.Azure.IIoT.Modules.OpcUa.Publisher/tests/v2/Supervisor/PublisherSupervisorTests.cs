// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Modules.OpcUa.Publisher.v2.Supervisor {
    using Microsoft.Azure.IIoT.Modules.OpcUa.Publisher.Tests;
    using Microsoft.Azure.IIoT.OpcUa.Registry;
    using Microsoft.Azure.IIoT.OpcUa.Registry.Models;
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Models;
    using Microsoft.Azure.IIoT.OpcUa.Registry.Services;
    using Microsoft.Azure.IIoT.Serializers.NewtonSoft;
    using Microsoft.Azure.IIoT.Serializers;
    using Microsoft.Azure.IIoT.Hub;
    using Autofac;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class PublisherSupervisorTests {

        [Fact]
        public async Task TestListPublishers() {
            using (var harness = new PublisherModuleFixture()) {
                await harness.RunTestAsync(async (device, module, services) => {

                    // Setup
                    var registry = services.Resolve<IPublisherRegistry>();

                    // Act
                    var supervisors = await registry.ListAllPublishersAsync();

                    // Assert
                    Assert.Single(supervisors);
                    Assert.True(supervisors.Single().Connected.Value);
                    Assert.True(supervisors.Single().OutOfSync.Value);
                    Assert.Equal(device, PublisherModelEx.ParseDeviceId(supervisors.Single().Id, out var moduleId));
                    Assert.Equal(module, moduleId);
                });
            }
        }

        [Fact]
        public async Task TestGetPublisher() {
            using (var harness = new PublisherModuleFixture()) {
                await harness.RunTestAsync(async (device, module, services) => {

                    // Setup
                    var registry = services.Resolve<IPublisherRegistry>();

                    // Act
                    var supervisor = await registry.GetPublisherAsync(PublisherModelEx.CreatePublisherId(device, module));

                    // Assert
                    Assert.True(supervisor.Connected.Value);
                    Assert.True(supervisor.OutOfSync.Value);
                });
            }
        }

        [Fact]
        public async Task TestGetPublisherStatus() {
            using (var harness = new PublisherModuleFixture()) {
                await harness.RunTestAsync(async (device, module, services) => {

                    // Setup
                    var diagnostics = services.Resolve<IPublisherDiagnostics>();

                    // Act
                    var status = await diagnostics.GetPublisherStatusAsync(PublisherModelEx.CreatePublisherId(device, module));

                    // Assert
                    Assert.Equal(status.DeviceId, device);
                    Assert.Equal(status.ModuleId, module);
                    Assert.Empty(status.Entities);
                });
            }
        }

        [Fact]
        public async Task TestWriterGroupPlacementAsync() {
            using (var harness = new PublisherModuleFixture()) {
                await harness.RunTestAsync(async (device, module, services) => {

                    // Setup
                    var publisherId = PublisherModelEx.CreatePublisherId(device, module);
                    var activation = services.Resolve<IPublisherOrchestration>();
                    var hub = services.Resolve<IIoTHubTwinServices>();
                    var twin = new WriterGroupInfoModel {
                        WriterGroupId = "ua326029342304923"
                    }.ToWriterGroupRegistration().ToDeviceTwin(_serializer);
                    await hub.CreateOrUpdateAsync(twin);
                    var registry = services.Resolve<IWriterGroupStatus>();
                    var activations = await registry.ListAllWriterGroupActivationsAsync();
                    Assert.Empty(activations); // Nothing yet activated

                    // Act
                    await activation.SynchronizeWriterGroupPlacementsAsync();
                    activations = await registry.ListAllWriterGroupActivationsAsync();
                    var wg2 = activations.FirstOrDefault();
                    var diagnostics = services.Resolve<IPublisherDiagnostics>();
                    var status = await diagnostics.GetPublisherStatusAsync(publisherId);

                    // Assert
                    Assert.Equal(device, status.DeviceId);
                    Assert.Equal(module, status.ModuleId);
                    Assert.Single(status.Entities);
                    Assert.Equal(wg2.Id, status.Entities.Single().Id);
                    Assert.Equal(EntityActivationState.ActivatedAndConnected, status.Entities.Single().ActivationState);
                    Assert.Equal(EntityActivationState.ActivatedAndConnected, wg2.ActivationState);
                });
            }
        }

        private readonly IJsonSerializer _serializer = new NewtonSoftJsonSerializer();
    }
}

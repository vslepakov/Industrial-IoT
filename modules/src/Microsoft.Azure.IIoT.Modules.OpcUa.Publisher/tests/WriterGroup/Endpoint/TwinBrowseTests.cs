// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Modules.OpcUa.Publisher.Twin.Endpoint {
    using Microsoft.Azure.IIoT.Modules.OpcUa.Publisher.Tests;
    using Microsoft.Azure.IIoT.OpcUa.Registry.Models;
    using Microsoft.Azure.IIoT.OpcUa.Core.Models;
    using Microsoft.Azure.IIoT.OpcUa.Testing.Fixtures;
    using Microsoft.Azure.IIoT.OpcUa.Testing.Tests;
    using Microsoft.Azure.IIoT.OpcUa.Twin;
    using System.Net;
    using Xunit;
    using Autofac;

    [Collection(PublishCollection.Name)]
    public class TwinBrowseTests : IClassFixture<PublisherModuleFixture> {

        public TwinBrowseTests(TestServerFixture server, PublisherModuleFixture module) {
            _server = server;
            _module = module;
        }

      // private BrowseServicesTests<string> GetTests() {
      //     var endpoint = new EndpointRegistrationModel {
      //         Endpoint = new EndpointModel {
      //             Url = $"opc.tcp://{Dns.GetHostName()}:{_server.Port}/UA/SampleServer",
      //             Certificate = _server.Certificate?.RawData?.ToThumbprint()
      //         },
      //         Id = "testid",
      //         SupervisorId = SupervisorModelEx.CreateSupervisorId(
      //           _module.DeviceId, _module.ModuleId)
      //     };
      //     endpoint = _module.RegisterAndActivateTwinId(endpoint);
      //     return new BrowseServicesTests<string>(
      //         () => _module.HubContainer.Resolve<IBrowseServices<string>>(), endpoint.Id);
      // }

        private readonly TestServerFixture _server;
        private readonly PublisherModuleFixture _module;

      //  [Fact]
      //  public async Task NodeBrowseInRootTest1Async() {
      //      await GetTests().NodeBrowseInRootTest1Async();
      //  }

    }
}

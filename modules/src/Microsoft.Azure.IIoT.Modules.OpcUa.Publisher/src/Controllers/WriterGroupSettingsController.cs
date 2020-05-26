// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Modules.OpcUa.Publisher.Controllers {
    using Microsoft.Azure.IIoT.OpcUa.Registry.Models;
    using Microsoft.Azure.IIoT.OpcUa.Core.Models;
    using Microsoft.Azure.IIoT.OpcUa.Edge;
    using Microsoft.Azure.IIoT.Module.Framework;
    using Microsoft.Azure.IIoT.Hub;
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using Prometheus;

    /// <summary>
    /// Endpoint settings controller
    /// </summary>
    [Version(1)]
    [Version(2)]
    public class WriterGroupSettingsController : ISettingsController {

      //  /// <summary>
      //  /// State of the endpoint
      //  /// </summary>
      //  public EndpointConnectivityState State {
      //      get => _twin.State;
      //      set { /* Only reporting */ }
      //  }
      //
      //  /// <summary>
      //  /// Create controller with service
      //  /// </summary>
      //  public WriterGroupSettingsController() {
      //  }
      //
      //  /// <summary>
      //  /// Apply endpoint update
      //  /// </summary>
      //  /// <returns></returns>
      //  public Task ApplyAsync() {
      //  }

    }
}

// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Modules.OpcUa.Publisher.Controllers {
    using Microsoft.Azure.IIoT.Modules.OpcUa.Publisher.Filters;
    using Microsoft.Azure.IIoT.Module.Framework;
    using Microsoft.Azure.IIoT.OpcUa.Api.Registry.Models;
    using Microsoft.Azure.IIoT.OpcUa.Registry;
    using Microsoft.Azure.IIoT.OpcUa.Edge;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Supervisor controller
    /// </summary>
    [Version(1)]
    [Version(2)]
    [ExceptionsFilter]
    public class SupervisorMethodsController : IMethodController {

        /// <summary>
        /// Create controller with service
        /// </summary>
        /// <param name="supervisor"></param>
        /// <param name="activator"></param>
        public SupervisorMethodsController(ISupervisorServices supervisor,
            IActivationServices<string> activator) {
            _supervisor = supervisor ?? throw new ArgumentNullException(nameof(supervisor));
            _activator = activator ?? throw new ArgumentNullException(nameof(activator));
        }

        /// <summary>
        /// Reset supervisor
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ResetAsync() {
            await _supervisor.ResetAsync();
            return true;
        }

        /// <summary>
        /// Get status
        /// </summary>
        /// <returns></returns>
        public async Task<SupervisorStatusApiModel> GetStatusAsync() {
            var result = await _supervisor.GetStatusAsync();
            return result.ToApiModel();
        }

        /// <summary>
        /// Activate writer group
        /// </summary>
        /// <param name="writerGroupId"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public async Task<bool> ActivateWriterGroupAsync(string writerGroupId, string secret) {
            if (string.IsNullOrEmpty(writerGroupId)) {
                throw new ArgumentNullException(nameof(writerGroupId));
            }
            if (string.IsNullOrEmpty(secret)) {
                throw new ArgumentNullException(nameof(secret));
            }
            if (!secret.IsBase64()) {
                throw new ArgumentException("not base64", nameof(secret));
            }
            await _activator.ActivateAsync(writerGroupId, secret);
            return true;
        }

        /// <summary>
        /// Deactivate writer group
        /// </summary>
        /// <param name="writerGroupId"></param>
        /// <returns></returns>
        public async Task<bool> DeactivateWriterGroupAsync(string writerGroupId) {
            if (string.IsNullOrEmpty(writerGroupId)) {
                throw new ArgumentNullException(nameof(writerGroupId));
            }
            await _activator.DeactivateAsync(writerGroupId);
            return true;
        }

        private readonly IActivationServices<string> _activator;
        private readonly ISupervisorServices _supervisor;
    }
}

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.IIoT.Crypto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.Client.HsmAuthentication {
    /// <summary>
    /// Authentication method that uses HSM to get a SAS token. 
    /// </summary>
    internal class ModuleAuthenticationWithHsm : ModuleAuthenticationWithTokenRefresh {
        private readonly ISecureElement _signatureProvider;
        private readonly string _generationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleAuthenticationWithHsm"/> class.
        /// </summary>
        /// <param name="signatureProvider">Provider for the token signature.</param>
        /// <param name="deviceId">Device Identifier.</param>
        /// <param name="moduleId">Module Identifier.</param>
        /// <param name="generationId"></param>
        public ModuleAuthenticationWithHsm(ISecureElement signatureProvider, string deviceId, string moduleId, string generationId) : base(deviceId, moduleId) {
            _signatureProvider = signatureProvider ?? throw new ArgumentNullException(nameof(signatureProvider));
            _generationId = generationId ?? throw new ArgumentNullException(nameof(generationId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iotHub">IotHub hostname</param>
        /// <param name="suggestedTimeToLive">Suggested time to live seconds</param>
        /// <returns></returns>
        protected override async Task<string> SafeCreateNewToken(string iotHub, int suggestedTimeToLive) {
            var startTime = DateTime.UtcNow;
            var audience = SasTokenBuilder.BuildAudience(iotHub, DeviceId, ModuleId);
            var expiresOn = SasTokenBuilder.BuildExpiresOn(startTime, TimeSpan.FromSeconds(suggestedTimeToLive));
            var data = string.Join("\n", new List<string> { audience, expiresOn });
            var signature = await _signatureProvider.SignAsync(Encoding.UTF8.GetBytes(data), "primary").ConfigureAwait(false);

            return SasTokenBuilder.BuildSasToken(audience, Convert.ToBase64String(signature), expiresOn);
        }
    }
}

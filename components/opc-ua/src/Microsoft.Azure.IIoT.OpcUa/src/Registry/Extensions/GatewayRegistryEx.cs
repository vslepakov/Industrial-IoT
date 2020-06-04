// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Registry {
    using Microsoft.Azure.IIoT.Exceptions;
    using Microsoft.Azure.IIoT.OpcUa.Registry.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Edge Gateway registry extensions
    /// </summary>
    public static class GatewayRegistryEx {

        /// <summary>
        /// Find edge gateway.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="gatewayId"></param>
        /// <param name="onlyServerState"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<GatewayInfoModel> FindGatewayAsync(
            this IGatewayRegistry service, string gatewayId,
            bool onlyServerState = false, CancellationToken ct = default) {
            try {
                return await service.GetGatewayAsync(gatewayId, onlyServerState, ct);
            }
            catch (ResourceNotFoundException) {
                return null;
            }
        }

        /// <summary>
        /// List all edge gateways
        /// </summary>
        /// <param name="service"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<List<GatewayModel>> ListAllGatewaysAsync(
            this IGatewayRegistry service, CancellationToken ct = default) {
            var publishers = new List<GatewayModel>();
            var result = await service.ListGatewaysAsync(null, null, ct);
            publishers.AddRange(result.Items);
            while (result.ContinuationToken != null) {
                result = await service.ListGatewaysAsync(result.ContinuationToken,
                    null, ct);
                publishers.AddRange(result.Items);
            }
            return publishers;
        }

        /// <summary>
        /// Query all edge gateways
        /// </summary>
        /// <param name="service"></param>
        /// <param name="query"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<List<GatewayModel>> QueryAllGatewaysAsync(
            this IGatewayRegistry service, GatewayQueryModel query,
            CancellationToken ct = default) {
            var supervisors = new List<GatewayModel>();
            var result = await service.QueryGatewaysAsync(query, null, ct);
            supervisors.AddRange(result.Items);
            while (result.ContinuationToken != null) {
                result = await service.ListGatewaysAsync(result.ContinuationToken,
                    null, ct);
                supervisors.AddRange(result.Items);
            }
            return supervisors;
        }
    }
}

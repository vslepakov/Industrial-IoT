// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Publisher.Services {
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Jobs;
    using Microsoft.Azure.IIoT.OpcUa.Twin;
    using Microsoft.Azure.IIoT.OpcUa.Twin.Models;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Twin sourced publishing jobs. Fills publish jobs using twin either by
    /// browsing or through bulk import from an uploaded nodeset.
    /// </summary>
    public sealed class BulkPublishService<T> : IBulkPublishInitiator<T> {

        /// <summary>
        /// Create client
        /// </summary>
        /// <param name="transfer"></param>
        /// <param name="endpoint"></param>
        public BulkPublishService(ITransferServices<T> transfer, IOrchestratorEndpoint endpoint) {
            _transfer = transfer ?? throw new ArgumentNullException(nameof(transfer));
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }

        /// <inhertidoc/>
        public async Task PublishAsync(T endpoint) {
            if (endpoint == null) {
                throw new ArgumentNullException(nameof(endpoint));
            }
            await _transfer.ModelUploadStartAsync(endpoint, new ModelUploadStartRequestModel {
                UploadEndpointUrl = _endpoint.JobOrchestratorUrl,
                AuthorizationHeader = null
            });
        }

        private readonly ITransferServices<T> _transfer;
        private readonly IOrchestratorEndpoint _endpoint;
    }
}

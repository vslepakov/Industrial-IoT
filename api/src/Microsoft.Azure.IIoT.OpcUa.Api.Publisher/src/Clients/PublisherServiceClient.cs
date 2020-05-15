// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Api.Publisher.Clients {
    using Microsoft.Azure.IIoT.OpcUa.Api.Publisher.Models;
    using Microsoft.Azure.IIoT.Http;
    using Microsoft.Azure.IIoT.Serializers.NewtonSoft;
    using Microsoft.Azure.IIoT.Serializers;
    using System;
    using System.Threading.Tasks;
    using System.Threading;

    /// <summary>
    /// Implementation of twin service api.
    /// </summary>
    public sealed class PublisherServiceClient : IPublisherServiceApi {

        /// <summary>
        /// Create service client
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="config"></param>
        /// <param name="serializer"></param>
        public PublisherServiceClient(IHttpClient httpClient, IPublisherConfig config,
            ISerializer serializer) :
            this(httpClient, config?.OpcUaPublisherServiceUrl, serializer) {
        }

        /// <summary>
        /// Create service client
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="serviceUri"></param>
        /// <param name="serializer"></param>
        public PublisherServiceClient(IHttpClient httpClient, string serviceUri,
            ISerializer serializer) {
            if (string.IsNullOrWhiteSpace(serviceUri)) {
                throw new ArgumentNullException(nameof(serviceUri),
                    "Please configure the Url of the endpoint micro service.");
            }
            _serializer = serializer ?? new NewtonSoftJsonSerializer();
            _serviceUri = serviceUri.TrimEnd('/');
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <inheritdoc/>
        public async Task<string> GetServiceStatusAsync(CancellationToken ct) {
            var request = _httpClient.NewRequest($"{_serviceUri}/healthz",
                Resource.Platform);
            try {
                var response = await _httpClient.GetAsync(request, ct).ConfigureAwait(false);
                response.Validate();
                return response.GetContentAsString();
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }

        /// <inheritdoc/>
        public Task<DataSetWriterAddResponseApiModel> AddDataSetWriterAsync(
            DataSetWriterAddRequestApiModel request, CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<DataSetWriterApiModel> GetDataSetWriterAsync(string dataSetWriterId,
            CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task UpdateDataSetWriterAsync(string dataSetWriterId,
            DataSetWriterUpdateRequestApiModel request, CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<DataSetAddEventResponseApiModel> AddEventDataSetAsync(string dataSetWriterId,
            DataSetAddEventRequestApiModel request, CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<PublishedDataSetEventsApiModel> GetEventDataSetAsync(string dataSetWriterId,
            CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task UpdateEventDataSetAsync(string dataSetWriterId,
            DataSetUpdateEventRequestApiModel request,
            CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task RemoveEventDataSetAsync(string dataSetWriterId, string generationId,
            CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<DataSetAddVariableResponseApiModel> AddDataSetVariableAsync(
            string dataSetWriterId, DataSetAddVariableRequestApiModel request,
            CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task UpdateDataSetVariableAsync(string dataSetWriterId, string variableId,
            DataSetUpdateVariableRequestApiModel request,
            CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<PublishedDataSetVariableListApiModel> ListDataSetVariablesAsync(
            string dataSetWriterId, string continuation, int? pageSize,
            CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<PublishedDataSetVariableListApiModel> QueryDataSetVariablesAsync(
            string dataSetWriterId, PublishedDataSetVariableQueryApiModel query,
            CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task RemoveDataSetVariableAsync(string dataSetWriterId, string variableId,
            string generationId, CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<DataSetWriterInfoListApiModel> ListDataSetWritersAsync(string continuation,
            int? pageSize, CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<DataSetWriterInfoListApiModel> QueryDataSetWritersAsync(
            DataSetWriterInfoQueryApiModel query, int? pageSize, CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task RemoveDataSetWriterAsync(string dataSetWriterId, string generationId,
            CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<WriterGroupAddResponseApiModel> AddWriterGroupAsync(
            WriterGroupAddRequestApiModel request, CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<WriterGroupApiModel> GetWriterGroupAsync(string writerGroupId,
            CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task UpdateWriterGroupAsync(string writerGroupId,
            WriterGroupUpdateRequestApiModel request, CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<WriterGroupInfoListApiModel> ListWriterGroupsAsync(string continuation,
            int? pageSize, CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<WriterGroupInfoListApiModel> QueryWriterGroupsAsync(
            WriterGroupInfoQueryApiModel query, int? pageSize, CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task RemoveWriterGroupAsync(string writerGroupId, string generationId,
            CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<DataSetAddVariableBatchResponseApiModel> AddVariablesToDataSetWriterAsync(
            string dataSetWriterId, DataSetAddVariableBatchRequestApiModel request, CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<DataSetAddVariableBatchResponseApiModel> AddVariablesToDefaultDataSetWriterAsync(
            string endpointId, DataSetAddVariableBatchRequestApiModel request, CancellationToken ct) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<DataSetRemoveVariableBatchResponseApiModel> RemoveVariablesFromDataSetWriterAsync(
            string dataSetWriterId, DataSetRemoveVariableBatchRequestApiModel request, CancellationToken ct) {
            throw new NotImplementedException();
        }

        private readonly IHttpClient _httpClient;
        private readonly ISerializer _serializer;
        private readonly string _serviceUri;
    }
}

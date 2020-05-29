// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Edge.Publisher {
    using Microsoft.Azure.IIoT.OpcUa.Edge.Publisher.Models;
    using Microsoft.Azure.IIoT.OpcUa.Publisher.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Encoder to encode data set writer messages
    /// </summary>
    public interface INetworkMessageEncoder {

        /// <summary>
        /// Messaging encoding
        /// </summary>
        string MessageScheme { get; }

        /// <summary>
        /// Encodes the list of messages into single message list
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        IEnumerable<NetworkMessageModel> Encode(
            IEnumerable<DataSetMessageModel> message);

        /// <summary>
        /// Encodes the list of messages into batched message list
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="maxMessageSize"></param>
        /// <returns></returns>
        IEnumerable<NetworkMessageModel> EncodeBatch(
            IEnumerable<DataSetMessageModel> messages, int maxMessageSize);
    }
}
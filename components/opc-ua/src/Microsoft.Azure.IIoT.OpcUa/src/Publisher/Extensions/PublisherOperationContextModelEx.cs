// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Publisher.Models {
    /// <summary>
    /// Operation context extensions
    /// </summary>
    public static class PublisherOperationContextModelEx {

        /// <summary>
        /// Convert to Service model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static PublisherOperationContextModel Clone(this PublisherOperationContextModel model) {
            if (model == null) {
                return null;
            }
            return new PublisherOperationContextModel {
                AuthorityId = model.AuthorityId,
                Time = model.Time
            };
        }
    }
}
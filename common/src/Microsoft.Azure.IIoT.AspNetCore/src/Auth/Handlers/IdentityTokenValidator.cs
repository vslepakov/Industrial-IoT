// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.AspNetCore.Auth.Clients {
    using Microsoft.Azure.IIoT.Auth;
    using Microsoft.Azure.IIoT.Auth.Models;
    using Microsoft.Azure.IIoT.Storage;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Identity token validator
    /// </summary>
    public class IdentityTokenValidator : IIdentityTokenValidator {

        /// <summary>
        /// Create validator
        /// </summary>
        /// <param name="identityTokenRetriever"></param>
        /// <param name="distributedCache"></param>
        public IdentityTokenValidator(IIdentityTokenStore identityTokenRetriever,
            ICache distributedCache) {
            _distributedCache = distributedCache ??
                throw new ArgumentNullException(nameof(distributedCache));
            _identityTokenRetriever = identityTokenRetriever ??
                throw new ArgumentNullException(nameof(identityTokenRetriever));
        }

        /// <inheritdoc/>
        public async Task ValidateToken(string scheme, IdentityTokenModel token) {
            if (token?.Identity == null) {
                throw new UnauthorizedAccessException();
            }
            var originalKey = await _distributedCache.GetStringAsync(token.Identity);
            if (originalKey == token.Key) {
                return;
            }
            var currentToken = await _identityTokenRetriever.GetIdentityTokenAsync(
                token.Identity);
            await _distributedCache.SetStringAsync(token.Identity,
                currentToken.Key, currentToken.Expires);
            if (currentToken.Expires != token.Expires ||
                currentToken.Expires < DateTime.UtcNow ||
                currentToken.Key != token.Key) {
                throw new UnauthorizedAccessException();
            }
        }

        private readonly ICache _distributedCache;
        private readonly IIdentityTokenStore _identityTokenRetriever;
    }
}
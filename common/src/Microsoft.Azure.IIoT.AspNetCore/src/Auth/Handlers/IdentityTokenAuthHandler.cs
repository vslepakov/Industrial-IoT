// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.AspNetCore.Auth.Clients {
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.IIoT.Auth;
    using Microsoft.Azure.IIoT.Auth.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Security.Claims;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    /// <summary>
    /// Access token authentication handler
    /// </summary>
    public class IdentityTokenAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions> {

        /// <summary>
        /// Create authentication handler
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="encoder"></param>
        /// <param name="clock"></param>
        /// <param name="context"></param>
        /// <param name="validator"></param>
        public IdentityTokenAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
            IHttpContextAccessor context, IIdentityTokenValidator validator) :
            base(options, logger, encoder, clock) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <inheritdoc/>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
            var request = _context.HttpContext.Request;
            if (!request.Headers.ContainsKey("Authorization")) {
                return AuthenticateResult.Fail("Missing Authorization header");
            }
            try {
                var authorization = request.Headers["Authorization"][0].Split(' ');
                var scheme = authorization[0].Trim();
                var token = authorization[1].Trim().ToIdentityToken();
                await _validator.ValidateToken(scheme, token);

                var claims = new[] { new Claim(ClaimTypes.NameIdentifier, token.Identity) };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex) {
                return AuthenticateResult.Fail(ex);
            }
        }

        private readonly IHttpContextAccessor _context;
        private readonly IIdentityTokenValidator _validator;
    }
}
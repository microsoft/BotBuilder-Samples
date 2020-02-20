// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Connector.SkillAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.Bot.Sample.EchoBot.Authentication
{
    /// <summary>
    /// Sample claims validator that loads an allowed list from configuration if present
    /// and checks that requests are coming from allowed parent bots.
    /// </summary>
    public class CustomAllowedCallersClaimsValidator : ClaimsValidator
    {
        private readonly IList<string> _allowedCallers;

        public CustomAllowedCallersClaimsValidator(IList<string> allowedCallers)
        {
            // AllowedCallers is the setting in web.config file
            // that consists of the list of parent bot IDs that are allowed to access the skill.
            // To add a new parent bot simply go to the AllowedCallers and add
            // the parent bot's Microsoft app ID to the list.

            _allowedCallers = allowedCallers ?? throw new ArgumentNullException(nameof(allowedCallers));
            if (!_allowedCallers.Any())
            {
                throw new ArgumentNullException(nameof(allowedCallers), "AllowedCallers must contain at least one element of '*' or valid MicrosoftAppId(s).");
            }
        }

        /// <summary>
        /// This method is called from JwtTokenValidation.ValidateClaimsAsync
        /// </summary>
        /// <param name="claims"></param>
        public override Task ValidateClaimsAsync(IList<Claim> claims)
        {
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            if (!claims.Any())
            {
                throw new UnauthorizedAccessException("ValidateClaimsAsync.claims parameter must contain at least one element.");
            }

            if (SkillValidation.IsSkillClaim(claims))
            {
                // if _allowedCallers has one item of '*', allow all parent bot calls and do not validate the appid from claims
                if (_allowedCallers.Count == 1 && _allowedCallers[0] == "*")
                {
                    return Task.CompletedTask;
                }

                // Check that the appId claim in the skill request is in the list of skills configured for this bot.
                var appId = JwtTokenValidation.GetAppIdFromClaims(claims).ToUpperInvariant();
                if (_allowedCallers.Contains(appId))
                {
                    return Task.CompletedTask;
                }

                throw new UnauthorizedAccessException($"Received a request from a bot with an app ID of \"{appId}\". To enable requests from this caller, add the app ID to your configuration file.");
            }

            throw new UnauthorizedAccessException($"ValidateClaimsAsync called without a Skill claim in claims.");
        }
    }
}

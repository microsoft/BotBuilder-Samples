// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Connector.SkillAuthentication;
using System;
using System.Collections.Generic;
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
            // that consists of the list of parent bot ids that are allowed to access the skill
            // to add a new parent bot simply go to the AllowedCallers and add
            // the parent bot's microsoft app id to the list

            _allowedCallers = allowedCallers ?? throw new ArgumentNullException(nameof(allowedCallers));
        }

        public override Task ValidateClaimsAsync(IList<Claim> claims)
        {
            // if _allowedCallers has one item of '*', allow all parent bot calls and do not validate the appid from claims
            if (SkillValidation.IsSkillClaim(claims) && !(_allowedCallers.Count == 1 && _allowedCallers[0] == "*"))
            {
                // Check that the appId claim in the skill request is in the list of skills configured for this bot.
                var appId = JwtTokenValidation.GetAppIdFromClaims(claims).ToUpperInvariant();
                if (!_allowedCallers.Contains(appId))
                {
                    throw new UnauthorizedAccessException($"Received a request from a bot with an app ID of \"{appId}\". To enable requests from this caller, add the app ID to your configuration file.");
                }
            }

            return Task.CompletedTask;
        }
    }
}

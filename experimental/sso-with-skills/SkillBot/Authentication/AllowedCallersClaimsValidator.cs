// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples.SkillBot.Authentication
{
    /// <summary>
    /// Sample claims validator that loads an allowed list from configuration if present
    /// and checks that requests are coming from allowed parent bots.
    /// </summary>
    public class AllowedCallersClaimsValidator : ClaimsValidator
    {
        private const string ConfigKey = "AllowedCallers";
        private readonly List<string> _allowedCallers;

        public AllowedCallersClaimsValidator(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            // AllowedCallers is the setting in appsettings.json file
            // that consists of the list of parent bot ids that are allowed to access the skill
            // to add a new parent bot simply go to the AllowedCallers and add
            // the parent bot's microsoft app id to the list
            var section = config.GetSection(ConfigKey);
            var appsList = section.Get<string[]>();
            _allowedCallers = appsList != null ? new List<string>(appsList) : null;
        }

        public override Task ValidateClaimsAsync(IList<Claim> claims)
        {
            // if _allowedCallers is null we allow all calls
            if (_allowedCallers != null && SkillValidation.IsSkillClaim(claims))
            {
                // Check that the appId claim in the skill request is in the list of skills configured for this bot.
                var appId = JwtTokenValidation.GetAppIdFromClaims(claims);
                if (!_allowedCallers.Contains(appId))
                {
                    throw new UnauthorizedAccessException($"Received a request from a bot with an app ID of \"{appId}\". To enable requests from this caller, add the app ID to your configuration file.");
                }
            }

            return Task.CompletedTask;
        }
    }
}

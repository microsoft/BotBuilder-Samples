// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.Authentication;

namespace Microsoft.BotBuilderSamples.SimpleRootBot.Authentication
{
    /// <summary>
    /// Sample claims validator that loads an allowed list from configuration if present
    /// and checks that responses are coming from configured skills.
    /// </summary>
    public class AllowedSkillsClaimsValidator : ClaimsValidator
    {
        private readonly List<string> _allowedCallers;

        public AllowedSkillsClaimsValidator(SkillsConfiguration skillsConfig)
        {
            if (skillsConfig == null)
            {
                throw new ArgumentNullException(nameof(skillsConfig));
            }

            // Load the appIds for the configured skills (we will only allow responses from skills we have configured).
            _allowedCallers = (from skill in skillsConfig.Skills.Values select skill.AppId.ToUpperInvariant()).ToList();
        }

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

                throw new UnauthorizedAccessException($"Received a message from a bot with an app ID of \"{appId}\". To enable requests from this caller, add the app ID to your configuration file.");
            }

            throw new UnauthorizedAccessException($"ValidateClaimsAsync called without a Skill claim in claims.");
        }
    }
}

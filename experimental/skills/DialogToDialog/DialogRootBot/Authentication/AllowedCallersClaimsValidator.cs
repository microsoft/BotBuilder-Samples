// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.Authentication;

namespace Microsoft.BotBuilderSamples.DialogRootBot.Authentication
{
    /// <summary>
    /// Sample claims validator that loads an allowed list from configuration if present
    /// and checks that responses are coming from configured skills.
    /// </summary>
    public class AllowedCallersClaimsValidator : ClaimsValidator
    {
        private readonly List<string> _allowedSkills;

        public AllowedCallersClaimsValidator(SkillsConfiguration skillsConfig)
        {
            if (skillsConfig == null)
            {
                throw new ArgumentNullException(nameof(skillsConfig));
            }

            // Load the appIds for the configured skills (we will only allow responses from skills we have configured).
            _allowedSkills = (from skill in skillsConfig.Skills.Values select skill.AppId).ToList();
        }

        public override Task ValidateClaimsAsync(IList<Claim> claims)
        {
            if (SkillValidation.IsSkillClaim(claims))
            {
                // Check that the appId claim in the skill request is in the list of skills configured for this bot.
                var appId = JwtTokenValidation.GetAppIdFromClaims(claims);
                if (!_allowedSkills.Contains(appId))
                {
                    throw new UnauthorizedAccessException($"The application received a request from \"{appId}\" that is not configured. Update your configuration file to enable this skill");
                }
            }

            return Task.CompletedTask;
        }
    }
}

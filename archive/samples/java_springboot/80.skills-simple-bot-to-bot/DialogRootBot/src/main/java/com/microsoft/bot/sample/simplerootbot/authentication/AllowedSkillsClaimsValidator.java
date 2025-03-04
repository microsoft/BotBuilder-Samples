// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.simplerootbot.authentication;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.concurrent.CompletableFuture;

import com.microsoft.bot.builder.skills.BotFrameworkSkill;
import com.microsoft.bot.connector.Async;
import com.microsoft.bot.connector.authentication.ClaimsValidator;
import com.microsoft.bot.connector.authentication.JwtTokenValidation;
import com.microsoft.bot.connector.authentication.SkillValidation;
import com.microsoft.bot.sample.simplerootbot.SkillsConfiguration;

/**
 * Sample claims validator that loads an allowed list from configuration if
 * presentand checks that requests are coming from allowed parent bots.
 */
public class AllowedSkillsClaimsValidator extends ClaimsValidator {

    private final List<String> allowedSkills;

    public AllowedSkillsClaimsValidator(SkillsConfiguration skillsConfig) {
        if (skillsConfig == null) {
            throw new  IllegalArgumentException("config cannot be null.");
        }

        // Load the appIds for the configured skills (we will only allow responses from skills we have configured).
        allowedSkills = new ArrayList<String>();
        for (Map.Entry<String, BotFrameworkSkill> configuration  : skillsConfig.getSkills().entrySet()) {
            allowedSkills.add(configuration.getValue().getAppId());
        }
    }

    @Override
    public CompletableFuture<Void> validateClaims(Map<String, String> claims) {
        // If _allowedCallers contains an "*", we allow all callers.
        if (SkillValidation.isSkillClaim(claims)) {
            // Check that the appId claim in the skill request instanceof in the list of callers
            // configured for this bot.
            String appId = JwtTokenValidation.getAppIdFromClaims(claims);
            if (!allowedSkills.contains(appId)) {
                return Async.completeExceptionally(
                    new RuntimeException(
                        String.format("Received a request from an application with an appID of \"%s\". "
                        + "To enable requests from this skill, add the skill to your configuration file.", appId)
                    )
                );
            }
        }

        return CompletableFuture.completedFuture(null);
    }
}


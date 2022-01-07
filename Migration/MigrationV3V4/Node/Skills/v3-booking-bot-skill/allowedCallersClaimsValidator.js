// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const skills = require('botbuilder/skills-validator');
const path = require('path');

// Import required bot configuration.
const ENV_FILE = path.join(__dirname, '.env');
require('dotenv').config({ path: ENV_FILE });

// Load the AppIds for the configured callers (we will only allow responses from skills we have configured).
// process.env.AllowedCallers is the list of parent bot Ids that are allowed to access the skill
// to add a new parent bot simply go to the .env file and add
// the parent bot's Microsoft AppId to the list under AllowedCallers, e.g.:
//  AllowedCallers=195bd793-4319-4a84-a800-386770c058b2,38c74e7a-3d01-4295-8e66-43dd358920f8
const allowedCallers = [process.env.ROOT_BOT_APP_ID]; // process.env.AllowedCallers ? process.env.AllowedCallers.split(',') : undefined;

/**
 * Sample claims validator that loads an allowed list from configuration if present
 * and checks that requests are coming from allowed parent bots.
 * @param claims An array of Claims decoded from the HTTP request's auth header
 */
const allowedCallersClaimsValidator = async (claims) => {
    if (!allowedCallers || allowedCallers.length == 0) {
        throw new Error(`DefaultAuthenticationConfiguration allowedCallers must contain at least one element of '*' or valid MicrosoftAppId(s).`);
    }
    if (!claims || claims.length < 1) {
        throw new Error(`DefaultAuthenticationConfiguration.validateClaims.claims parameter must contain at least one element.`);
    }
    // If allowedCallers is undefined we allow all calls
    // If allowedCallers contains '*' we allow all callers
    if (skills.SkillValidation.isSkillClaim(claims)) {
                
        if(allowedCallers[0] === '*') {
            return;
        }
        // Check that the appId claim in the skill request is in the list of skills configured for this bot.
        const appId = skills.JwtTokenValidation.getAppIdFromClaims(claims);
        if (allowedCallers.includes(appId)) {
            return;
        }
        throw new Error(`Received a request from a bot with an app ID of "${ appId }". To enable requests from this caller, add the app ID to your configuration file.`);
    }
    throw new Error(`DefaultAuthenticationConfiguration.validateClaims called without a Skill claim in claims.`);
};

module.exports.allowedCallersClaimsValidator = allowedCallersClaimsValidator;

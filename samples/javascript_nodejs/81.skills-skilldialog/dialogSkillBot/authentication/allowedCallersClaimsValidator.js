// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { JwtTokenValidation, SkillValidation } = require('botframework-connector');

// Load the AppIds for the configured callers (we will only allow responses from skills we have configured).
// process.env.AllowedCallers is the list of parent bot Ids that are allowed to access the skill
// to add a new parent bot simply go to the .env file and add
// the parent bot's Microsoft AppId to the list under AllowedCallers, e.g.:
//  AllowedCallers=195bd793-4319-4a84-a800-386770c058b2,38c74e7a-3d01-4295-8e66-43dd358920f8
const allowedCallers = process.env.AllowedCallers ? process.env.AllowedCallers.split(',') : undefined;

/**
 * Sample claims validator that loads an allowed list from configuration if present
 * and checks that requests are coming from allowed parent bots.
 * @param claims An array of Claims decoded from the HTTP request's auth header
 */
const allowedCallersClaimsValidator = async (claims) => {
    // If allowedCallers is undefined or contains '*' we allow all calls
    if (allowedCallers && !allowedCallers.includes('*') && SkillValidation.isSkillClaim(claims)) {
        // Check that the appId claim in the skill request is in the list of skills configured for this bot.
        const appId = JwtTokenValidation.getAppIdFromClaims(claims);
        if (!allowedCallers.includes(appId)) {
            throw new Error(`Received a request from a bot with an app ID of "${ appId }". To enable requests from this caller, add the app ID to your configuration file.`);
        }
    }
};

module.exports.allowedCallersClaimsValidator = allowedCallersClaimsValidator;
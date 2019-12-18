// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ValidateClaims } = require('botframework-connector');

/**
 * Sample claims validator that loads an allowed list from configuration if present
 * and checks that requests are coming from allowed parent bots.
 */
class AllowedCallerssClaimsValidator extends ValidateClaims {
    constructor(skillsConfig) {
        super();
        this.allowedCallers = {};

        // Load the appIds for the configured callers (we will only allow responses from skills we have configured).

        // AllowedCallers is the setting in appsettings.json file
        // that consists of the list of parent bot ids that are allowed to access the skill
        // to add a new parent bot simply go to the AllowedCallers and add
        // the parent bot's microsoft app id to the list
    }
}

module.exports.AllowedCallerssClaimsValidator = AllowedCallerssClaimsValidator;
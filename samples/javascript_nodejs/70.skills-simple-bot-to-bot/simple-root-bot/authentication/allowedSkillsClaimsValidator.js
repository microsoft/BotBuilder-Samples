// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ValidateClaims } = require('botframework-connector');

/**
 * Sample claims validator that loads an allowed list from configuration if present
 * and checks that responses are coming from configured skills.
 */
class AllowedSkillsClaimsValidator extends ValidateClaims {
    constructor(skillsConfig) {
        super();
        this.allowedSkills = {};

        // Load the appIds for the configured skills (we will only allow responses from skills we have configured).
    }
}

module.exports.AllowedSkillsClaimsValidator = AllowedSkillsClaimsValidator;
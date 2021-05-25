// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/**
 * A helper class that loads Skills information from configuration.
 */
class SkillsConfiguration {
    constructor() {
        this.skillsData = {};

        // Note: we only have two skills in this sample but we could load more if needed.
        const botFrameworkSimpleSkill = {
            id: process.env.SkillSimpleId,
            appId: process.env.SkillSimpleAppId,
            skillEndpoint: process.env.SkillSimpleEndpoint
        };

        const botFrameworkBookingSkill = {
            id: process.env.SkillBookingId,
            appId: process.env.SkillBookingAppId,
            skillEndpoint: process.env.SkillBookingEndpoint
        };

        this.skillsData[botFrameworkSimpleSkill.id] = botFrameworkSimpleSkill;
        this.skillsData[botFrameworkBookingSkill.id] = botFrameworkBookingSkill;

        this.skillHostEndpointValue = process.env.SkillHostEndpoint;
        if (!this.skillHostEndpointValue) {
            throw new Error('[SkillsConfiguration]: Missing configuration parameter. SkillHostEndpoint is required');
        }
    }

    get skills() {
        return this.skillsData;
    }

    get skillHostEndpoint() {
        return this.skillHostEndpointValue;
    }
}

module.exports.SkillsConfiguration = SkillsConfiguration;

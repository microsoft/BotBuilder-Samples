// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { SkillConversationIdFactoryBase, TurnContext } = require('botbuilder');

/**
 * A SkillConversationIdFactory that uses an in memory dictionary
 * to store and retrieve ConversationReference instances.
 */
class SkillConversationIdFactory extends SkillConversationIdFactoryBase {
    constructor() {
        super();
        this.refs = {};
        this.skillId = process.env.SkillId;
        this.disableCreateWithOptions = false;
        this.disableGetSkillConversationReference = false;
    }

    async createSkillConversationIdWithOptions(options) {
        if (this.disableCreateWithOptions) super.createSkillConversationIdWithOptions();
        this.refs[this.skillId] = {
            conversationReference: TurnContext.getConversationReference(options.activity),
            oAuthScope: options.fromBotOAuthScope
        };
        return this.skillId;
    }

    async getSkillConversationReference(skillConversationId) {
        if (this.disableGetSkillConversationReference) super.createSkillConversationIdWithOptions();
        return this.refs[skillConversationId];
    }

    async deleteConversationReference(skillConversationId) {
        this.refs[skillConversationId] = undefined;
    }
}

module.exports.SkillConversationIdFactory = SkillConversationIdFactory;

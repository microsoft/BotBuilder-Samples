// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { SkillConversationIdFactoryBase, TurnContext } = require('botbuilder');

/**
 * A SkillConversationIdFactory that uses an in memory dictionary
 *to store and retrieve ConversationReference instances.
 */
class SkillConversationIdFactory extends SkillConversationIdFactoryBase {
    constructor() {
        super();
        this.refs = {};
        this.skillId = process.env.SkillId;
    }

    async createSkillConversationId(conversationReference) {
        const key = `${ conversationReference.conversation.id }-${ conversationReference.channelId }-skillconvo`;
        this.refs[key] = conversationReference;
        return key;
    }

    async createSkillConversationIdWithOptions(opts) {
        this.refs[this.skillId] = { oAuthScope: opts.fromOAuthScope, conversationReference: TurnContext.getConversationReference(opts.activity) };
        return this.skillId;
    }

    async getConversationReference(skillConversationId) {
        return this.refs[skillConversationId];
    }

    async deleteConversationReference(skillConversationId) {
        this.refs[skillConversationId] = undefined;
    }

    async getSkillConversationReference(skillConversationId) {
        return await this.getConversationReference(skillConversationId);
    }
}

module.exports.SkillConversationIdFactory = SkillConversationIdFactory;
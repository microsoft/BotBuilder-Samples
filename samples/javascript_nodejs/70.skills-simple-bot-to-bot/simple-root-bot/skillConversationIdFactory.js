
const { SkillConversationIdFactoryBase } = require('botbuilder');

class SkillConversationIdFactory extends SkillConversationIdFactoryBase {
    constructor() {
        super();
        this.refs = {};
    }

    async createSkillConversationId(conversationReference) {
        const key = `${ conversationReference.conversation.id }-${ conversationReference.channelId }-skillconvo`;
        this.refs[key] = conversationReference;
        return key;
    }

    async getConversationReference(skillConversationId) {
        return this.refs[skillConversationId];
    }

    async deleteConversationReference(skillConversationId) {
        this.refs[skillConversationId] = undefined;
    }
}

module.exports = { SkillConversationIdFactory: SkillConversationIdFactory };

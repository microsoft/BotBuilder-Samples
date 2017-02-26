exports.createTelemetry = function (session, properties) {
    var data = {
        conversationData: JSON.stringify(session.conversationData),
        privateConversationData: JSON.stringify(session.privateConversationData),
        userData: JSON.stringify(session.userData),
        conversationId: session.message.address.conversation.id,
        userId: session.message.address.user.id
    };

    if (properties) {
        for (property in properties) {
            data[property] = properties[property];
        }
    }

    return data;
};
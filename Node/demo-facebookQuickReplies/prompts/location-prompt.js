var builder = require('botbuilder');
var quickReplies = require('../facebook/quickreplies');

exports.beginDialog = function (session, options) {
    session.beginDialog('location', options || {});
}

// Custom prompt that requests the user's current location
// using a Facebook Quick Reply.
exports.create = function (bot) {
    var prompt = new builder.IntentDialog()
        .onBegin(function (session, args) {
            var message = new builder.Message(session).text('Please share your location...');
            message = quickReplies.AddQuickReplies(session, message, [
                new quickReplies.QuickReplyLocation()
            ]);

            session.send(message);
        })
        .matches(/(give up|quit|skip)/i, function (session) {
            // Return 'false' to indicate they gave up
            session.endDialogWithResult({ response: false });
        })
        .onDefault(function (session) {
            if (session.message.sourceEvent.message && session.message.sourceEvent.message.attachments) {
                var attachment = session.message.sourceEvent.message.attachments[0];
                if (attachment.type == 'location') {
                    session.endDialogWithResult({ response: { entity: {
                        title: attachment.title,
                        coordinates: attachment.payload.coordinates
                    }}})
                }
            } else {
                session.send("Sorry, try again.");
            }
        });
        bot.dialog('location', prompt);
}
// Constructs a quick reply
function QuickReplyText(title, text, imgUrl) {
    var quickReply = {
        content_type: "text",
        title: title,
        payload: text
    }

    // Images are optional.
    if (imgUrl) {
        quickReply.image_url = imgUrl;
    }

    return quickReply;
}

function QuickReplyLocation() {
    var quickReply = {
        content_type: 'location'
    }

    return quickReply;
}

// Adds quick replies to the message.
function AddQuickReplies(session, message, quickReplys) {
    if (session.message.source == 'facebook') {
        message.sourceEvent({
            facebook: {
                quick_replies: quickReplys
            }
        })
    }

    return message;
}

module.exports.AddQuickReplies = AddQuickReplies;
module.exports.QuickReplyText = QuickReplyText;
module.exports.QuickReplyLocation = QuickReplyLocation;
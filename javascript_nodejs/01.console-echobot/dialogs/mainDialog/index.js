// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

module.exports = async function(context) {
    // check to see if this activity is an incoming message
    // (it could theoretically be another type of activity)
    if (context.activity.type === 'message') {
        // check to see if the user sent a simple "quit" message
        if (context.activity.text.toLowerCase() === 'quit') {
            // send a reply
            context.sendActivity(`Bye!`);
            process.exit();
        } else if (context.activity.text) {
            // echo the message text back to the user.
            return context.sendActivity(`I heard you say "${ context.activity.text }"`);
        }
    }
}
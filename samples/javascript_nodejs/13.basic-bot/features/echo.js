// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

module.exports = function(bot) {
    /**
     * Bind a handler to the onMessage event that will echo back any text received.
     */
    bot.onMessage(async (context, next) => {
        // UNCOMMENT TO ENABLE ECHO BOT FEATURE
        // await context.sendActivity(`Echo: You said "${ context.activity.text }"`);
        await next();
    });
};

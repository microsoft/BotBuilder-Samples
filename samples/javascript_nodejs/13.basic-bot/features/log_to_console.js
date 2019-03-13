// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

module.exports = function(bot) {
    /**
     * Bind a handler to the onActivity event that will log every incoming activity to the console.
     */
    bot.onTurn(async (context, next) => {
        // UNCOMMENT TO ENABLE LOG TO CONSOLE FEATURE
        // console.log('Activity:', context.activity);
        await next();
    });
};

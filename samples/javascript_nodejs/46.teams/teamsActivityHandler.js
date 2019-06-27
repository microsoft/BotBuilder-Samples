// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler, ActivityTypes } = require('botbuilder');

class TeamsActivityHandler extends ActivityHandler {

    constructor() {
        super();
        this.onUnrecognizedActivityType(async (context, next) => {

            const runDialogs = async () => {
                await this.handle(context, 'Dialog', async () => {
                    // noop
                });
            };

            switch (context.activity.type) {
            case ActivityTypes.MessageReaction:
                await this.handle(context, 'MessageReaction', async () => {
                    if (context.activity.reactionsAdded && context.activity.reactionsAdded.length > 0) {
                        await this.handle(context, 'ReactionsAdded', runDialogs);
                    } else if (context.activity.reactionsRemoved && context.activity.reactionsRemoved.length > 0) {
                        await this.handle(context, 'ReactionsRemoved', runDialogs);
                    } else {
                        await runDialogs();
                    }
                });
                break;
            case ActivityTypes.Invoke:
                await this.handle(context, 'Invoke', runDialogs);
                break;
            default:
                await next();
            }
        });
    }

    onMessageReaction(handler) {
        return this.on('MessageReaction', handler);
    }

    onReactionsAdded(handler) {
        return this.on('ReactionsAdded', handler);
    }

    onReactionsRemoved(handler) {
        return this.on('ReactionsRemoved', handler);
    }

    onInvoke(handler) {
        return this.on('Invoke', handler);
    }
}

module.exports.TeamsActivityHandler = TeamsActivityHandler;

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
            case ActivityTypes.Invoke:
                await this.handle(context, 'Invoke', runDialogs);
                break;
            default:
                await next();
            }
        });
    }

    onInvoke(handler) {
        return this.on('Invoke', handler);
    }
}

module.exports.TeamsActivityHandler = TeamsActivityHandler;

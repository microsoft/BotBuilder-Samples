// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { BotAdapter } = require('botbuilder');

// This custom BotAdapter supports scenarios that only Send Activities. Update and Delete Activity
// are not supported.
// Rather than sending the outbound Activities directly as the BotFrameworkAdapter does this class
// buffers them in a list. The list is exposed as a public property.
class DialogHostAdapter extends BotAdapter {

    constructor() {
        super();
        this.response = [];
    }

    get activities() {
        return this.response;
    }

    async sendActivities(turnContext, activities) {
        this.response.push(...activities);
    }

    async deleteActivity(turnContext) {
        throw new Error('not implemented');
    }

    async updateActivity(tunrContext) {
        throw new Error('not implemented');
    }
}

module.exports.DialogHostAdapter = DialogHostAdapter;

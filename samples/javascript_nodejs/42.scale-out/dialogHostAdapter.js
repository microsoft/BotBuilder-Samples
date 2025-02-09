// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

class DialogHostAdapter {
    constructor() {
        this._response = [];
    }

    get Activities() {
        return this._response;
    }

    async sendActivities(turnContext, activities, cancellationToken) {
        for (const activity of activities) {
            this._response.push(activity);
        }

        return [];
    }

    // Not Implemented
    async deleteActivity(turnContext, reference, cancellationToken) {
        throw new Error('Not Implemented');
    }

    async updateActivity(turnContext, activity, cancellationToken) {
        throw new Error('Not Implemented');
    }
}

module.exports.DialogHostAdapter = DialogHostAdapter;

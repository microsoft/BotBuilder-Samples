// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { DialogTurnStatus } = require('botbuilder-dialogs');

class TurnResult {
    /**
     * 
     * @param {DialogTurnStatus} status 
     * @param {Object} result 
     */
    constructor(status, result) {
        this.status = status ? status : DialogTurnStatus.empty;
        this.result = result ? result : {};
    }
}

module.exports = TurnResult;

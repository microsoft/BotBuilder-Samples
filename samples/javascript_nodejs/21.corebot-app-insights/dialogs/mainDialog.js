// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ComponentDialog } = require('botbuilder-dialogs');

class MainDialog extends ComponentDialog {
    constructor() {
        super('MainDialog');
    }
}

module.exports.MainDialog = MainDialog;
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { DialogTurnStatus, WaterfallDialog, ComponentDialog } = require('botbuilder-dialogs');

const DIALOG_NAME = 'confirmationDialog';

class ConfirmDialog extends ComponentDialog {
    constructor(botConfig, reservationsPropertyAccessor, turnCounterPropertyAccessor, onTurnPropertyAccessor) {
        super(DIALOG_NAME);

    }
    
};

ConfirmDialog.Name = DIALOG_NAME;

module.exports = ConfirmDialog;
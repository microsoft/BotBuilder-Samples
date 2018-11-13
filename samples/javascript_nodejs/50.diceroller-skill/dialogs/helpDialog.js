// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { CardFactory, MessageFactory, InputHints } = require('botbuilder');
const { Dialog } = require('botbuilder-dialogs');
const { getText } = require('../lg');
const ssml = require('../ssml');

/**
 * Single turn dialog that will send the user a help card.
 */
class HelpDialog extends Dialog {
    /**
     * Abstract method called when the dialog is started.
     * @param {DialogContext} dc Dialog context for the current turn of conversation.
     * @param {object} options (Optional) options passed to the dialog in the DialogContext.beginDialog() call.
     */
    async beginDialog(dc, options) {
        // Format help card and message
        const card = CardFactory.heroCard(getText('en', 'help_title'), undefined, ['Roll Dice', 'Play Craps']);
        const msg = MessageFactory.attachment(card);
        msg.speak = ssml.speak(getText('en', 'help_ssml'));
        msg.inputHint = InputHints.AcceptingInput;

        // Send card and end dialog
        await dc.context.sendActivity(msg);
        return await dc.endDialog();
    }
}

module.exports.HelpDialog = HelpDialog;

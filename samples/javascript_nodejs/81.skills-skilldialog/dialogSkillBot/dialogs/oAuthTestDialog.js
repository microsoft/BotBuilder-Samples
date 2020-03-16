// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { InputHints, MessageFactory } = require('botbuilder');
const { ComponentDialog, OAuthPrompt, WaterfallDialog } = require('botbuilder-dialogs');

class OAuthTestDialog extends ComponentDialog {
    constructor() {
        super(OAuthTestDialog.name);
        this.connectionName = process.env.ConnectionName;

        if (!this.connectionName) throw new Error('[OAuthTestDialog]: Missing environment variable. \'ConnectionName\' (name of OAuth connection) is required');

        this.addDialog(new OAuthPrompt(OAuthPrompt.name, {
            connectionName: this.connectionName,
            text: `Please Sign In to connection: ${ this.connectionName }`,
            title: 'Sign In',
            timeout: 300000 // User has 5 minutes to log in
        }))
            .addDialog(new WaterfallDialog(WaterfallDialog.name, [
                this.promptStep.bind(this),
                this.loginStep.bind(this)
            ]));

        this.initialDialogId = WaterfallDialog.name;
    }

    async promptStep(stepContext) {
        return await stepContext.beginDialog(OAuthPrompt.name);
    }

    async loginStep(stepContext) {
        const tokenResponse = stepContext.result;
        if (tokenResponse != null) {
            // Show the token
            const loggedInMessage = 'You are now logged in.';
            await stepContext.context.sendActivity(MessageFactory.text(loggedInMessage, loggedInMessage, InputHints.IgnoringInput));
            const showTokenMessage = 'Here is your token:';
            await stepContext.context.sendActivity(MessageFactory.text(`${ showTokenMessage }\n${ tokenResponse.token }`, showTokenMessage, InputHints.IgnoringInput));

            // Sign out
            const botAdapter = stepContext.context.adapter;
            await botAdapter.signOutUser(stepContext.context, this.connectionName);
            const signOutMessage = 'I have signed you out';
            await stepContext.context.sendActivity(MessageFactory.text(signOutMessage, signOutMessage, InputHints.IgnoringInput));

            return await stepContext.endDialog();
        }

        const tryAgainMessage = 'Login was not successful. Please try again.';
        await stepContext.context.sendActivity(MessageFactory.text(tryAgainMessage, tryAgainMessage, InputHints.IgnoringInput));
        return await stepContext.endDialog();
    }
}

module.exports.OAuthTestDialog = OAuthTestDialog;

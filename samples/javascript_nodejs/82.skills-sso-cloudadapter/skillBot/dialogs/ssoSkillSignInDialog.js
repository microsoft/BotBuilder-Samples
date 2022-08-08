// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ComponentDialog, OAuthPrompt, WaterfallDialog } = require('botbuilder-dialogs');

const OAUTH_PROMPT = 'OAuthPrompt';
const WATERFALL_DIALOG = 'WaterfallDialog';
const SSO_SIGNIN_DIALOG = 'SsoSkillSignInDialog';

class SsoSkillSignInDialog extends ComponentDialog {
    /**
     * @param {string} connectionName
     */
    constructor(connectionName) {
        super(SSO_SIGNIN_DIALOG);

        this.addDialog(new OAuthPrompt(OAUTH_PROMPT, {
            connectionName: connectionName,
            text: 'Sign in to the Skill using Azure AD',
            title: 'Sign In'
        }));

        this.addDialog(new WaterfallDialog(WATERFALL_DIALOG, [
            this.signInStep.bind(this),
            this.displayToken.bind(this)
        ]));

        this.initialDialogId = WATERFALL_DIALOG;
    }

    /**
     * @param {import('botbuilder-dialogs').WaterfallStepContext} stepContext
     */
    async signInStep(stepContext) {
    // This prompt won't show if the user is signed in to the root using SSO.
        return stepContext.beginDialog(OAUTH_PROMPT);
    }

    /**
     * @param {import('botbuilder-dialogs').WaterfallStepContext} stepContext
     */
    async displayToken(stepContext) {
        const { result } = stepContext;
        if (!result || !result.token) {
            await stepContext.context.sendActivity('No token was provided for the skill.');
        } else {
            await stepContext.context.sendActivity(`Here is your token for the skill: ${ result.token }`);
        }

        return stepContext.endDialog();
    }
}

module.exports.SsoSkillSignInDialog = SsoSkillSignInDialog;

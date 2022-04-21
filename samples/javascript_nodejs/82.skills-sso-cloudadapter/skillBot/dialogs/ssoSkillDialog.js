// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { MessageFactory, InputHints } = require('botbuilder');
const { ComponentDialog, ChoicePrompt, ChoiceFactory, DialogTurnStatus, WaterfallDialog } = require('botbuilder-dialogs');
const { SsoSkillSignInDialog } = require('./ssoSkillSignInDialog');

const ACTION_PROMPT = 'ActionStepPrompt';
const WATERFALL_DIALOG = 'WaterfallDialog';
const SSO_SKILL_DIALOG = 'SsoSkillDialog';
const SSO_SIGNIN_DIALOG = 'SsoSkillSignInDialog';

class SsoSkillDialog extends ComponentDialog {
    /**
     * @param {string} connectionName
     */
    constructor(connectionName) {
        super(SSO_SKILL_DIALOG);

        if (!connectionName) throw new Error('[SsoSkillDialog]: \'connectionName\' is not set in configuration.');
        this.connectionName = connectionName;

        this.addDialog(new SsoSkillSignInDialog(connectionName))
            .addDialog(new ChoicePrompt(ACTION_PROMPT))
            .addDialog(new WaterfallDialog(WATERFALL_DIALOG, [
                this.promptActionStep.bind(this),
                this.handleActionStep.bind(this),
                this.promptFinalStep.bind(this)
            ]));

        this.initialDialogId = WATERFALL_DIALOG;
    }

    /**
     * @param {import('botbuilder-dialogs').WaterfallStepContext} stepContext
     */
    async promptActionStep(stepContext) {
        const messageText = 'What SSO action would you like to perform on the skill?';
        const repromptMessageText = 'That was not a valid choice, please select a valid choice.';

        return stepContext.prompt(ACTION_PROMPT, {
            prompt: MessageFactory.text(messageText, messageText, InputHints.ExpectingInput),
            retryPrompt: MessageFactory.text(repromptMessageText, repromptMessageText, InputHints.ExpectingInput),
            choices: await this.getPromptChoices(stepContext)
        });
    }

    /**
     * @param {import('botbuilder-dialogs').WaterfallStepContext} stepContext
     */
    async getPromptChoices(stepContext) {
        // Try to get the token for the current user to determine if it is logged in or not.
        const choices = new Set();
        const userTokenClient = stepContext.context.turnState.get(stepContext.context.adapter.UserTokenClientKey);
        const tokenResponse = await userTokenClient.getUserToken(
            stepContext.context.activity.from.id,
            this.connectionName,
            stepContext.context.activity.channelId,
            null
        );

        // Present different choices depending on the user's sign in status.
        if (!tokenResponse || !tokenResponse.token) {
            choices.add('Login to the skill');
        } else {
            choices.add('Logout from the skill');
            choices.add('Show token');
        }

        choices.add('End');

        return ChoiceFactory.toChoices([...choices]);
    }

    /**
     * @param {import('botbuilder-dialogs').WaterfallStepContext} stepContext
     */
    async handleActionStep(stepContext) {
        const action = stepContext.result.value.toLowerCase();
        const userTokenClient = stepContext.context.turnState.get(stepContext.context.adapter.UserTokenClientKey);

        switch (action) {
            case 'login to the skill': {
                // The SsoSkillSignInDialog will just show the user token if the user logged on to the root bot.
                return stepContext.beginDialog(SSO_SIGNIN_DIALOG);
            }
            case 'logout from the skill': {
                // This will just clear the token from the skill.
                const { activity } = stepContext.context;
                await userTokenClient.signOutUser(activity.from.id, this.connectionName, activity.channelId);
                await stepContext.context.sendActivity('You have been signed out.');
                return stepContext.next();
            }
            case 'show token': {
                const tokenResponse = await userTokenClient.getUserToken(
                    stepContext.context.activity.from.id,
                    this.connectionName,
                    stepContext.context.activity.channelId,
                    null
                );

                if (!tokenResponse || !tokenResponse.token) {
                    await stepContext.context.sendActivity('User has no cached token.');
                } else {
                    await stepContext.context.sendActivity(`Here is your current SSO token: ${ tokenResponse.token }`);
                }

                return stepContext.next();
            }
            case 'end': {
                // Ends the interaction with the skill.
                return { status: DialogTurnStatus.complete };
            }
            default: {
                // This should never be hit since the previous prompt validates the choice.
                throw new Error(`Unrecognized action: ${ action }`);
            }
        }
    }

    /**
     * @param {import('botbuilder-dialogs').WaterfallStepContext} stepContext
     */
    async promptFinalStep(stepContext) {
        // Restart the dialog (we will exit when the user says end).
        return stepContext.replaceDialog(this.initialDialogId);
    }
}

module.exports.SsoSkillDialog = SsoSkillDialog;

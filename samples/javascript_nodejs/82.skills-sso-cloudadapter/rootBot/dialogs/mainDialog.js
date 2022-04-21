// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, InputHints, MessageFactory } = require('botbuilder');
const { ChoicePrompt, ComponentDialog, SkillDialog, WaterfallDialog } = require('botbuilder-dialogs');
const { SsoSignInDialog } = require('./ssoSignInDialog');

const MAIN_DIALOG = 'MainDialog';
const SSO_SIGNIN_DIALOG = 'SsoSignInDialog';
const SKILL_DIALOG = 'SkillDialog';
const WATERFALL_DIALOG = 'WaterfallDialog';

/**
 * The main dialog for this bot. It uses a SkillDialog to call skills.
 */
class MainDialog extends ComponentDialog {
    constructor(auth, conversationState, conversationIdFactory, skillsConfig) {
        super(MAIN_DIALOG);

        if (!auth) throw new Error('[MainDialog]: Missing parameter \'auth\' is required');
        this.auth = auth.createBotFrameworkClient();

        const botId = process.env.MicrosoftAppId;
        if (!botId) throw new Error('[MainDialog]: MicrosoftAppId is not set in configuration');

        this.connectionName = process.env.ConnectionName;
        if (!this.connectionName) throw new Error('[MainDialog]: ConnectionName is not set in configuration');

        // We use a single skill in this example.
        const targetSkillId = 'SkillBot';
        this.ssoSkill = skillsConfig.skills[targetSkillId];
        if (!this.ssoSkill) throw new Error(`[MainDialog]: Skill with ID ${ targetSkillId } not found in configuration`);

        this.activeSkillPropertyName = `${ MAIN_DIALOG }.activeSkillProperty`;

        this.addDialog(new ChoicePrompt('ActionStepPrompt'))
            .addDialog(new SsoSignInDialog(this.connectionName))
            .addDialog(new SkillDialog(this.createSkillDialogOptions(skillsConfig, botId, conversationIdFactory, conversationState), SKILL_DIALOG));

        this.addDialog(new WaterfallDialog(WATERFALL_DIALOG, [
            this.promptActionStep.bind(this),
            this.handleActionStep.bind(this),
            this.promptFinalStep.bind(this)
        ]));

        // Create state property to track the active skill.
        this.activeSkillProperty = conversationState.createProperty(this.activeSkillPropertyName);

        // The initial child Dialog to run.
        this.initialDialogId = WATERFALL_DIALOG;
    }

    /**
     * // Helper to create a SkillDialogOptions instance for the SSO skill.
     */
    createSkillDialogOptions(skillsConfig, botId, conversationIdFactory, conversationState) {
        return {
            botId: botId,
            conversationIdFactory: conversationIdFactory,
            skillClient: this.auth,
            skillHostEndpoint: skillsConfig.skillHostEndpoint,
            conversationState: conversationState,
            skill: this.ssoSkill
        };
    }

    /**
     * Render a prompt to select the action to perform with the skill.
     */
    async promptActionStep(stepContext) {
        const messageText = 'What SSO action do you want to perform?';
        const repromptMessageText = 'That was not a valid choice, please select a valid choice.';
        const options = {
            prompt: MessageFactory.text(messageText, messageText, InputHints.ExpectingInput),
            retryPrompt: MessageFactory.text(repromptMessageText, repromptMessageText, InputHints.ExpectingInput),
            choices: await this.getPromptOptions(stepContext)
        };

        return await stepContext.prompt('ActionStepPrompt', options);
    }

    /**
     * Creates the prompt choices based on the current sign in status.
     */
    async getPromptOptions(stepContext) {
        const promptChoices = [];
        const userTokenClient = stepContext.context.turnState.get(stepContext.context.adapter.UserTokenClientKey);
        const tokenResponse = await userTokenClient.getUserToken(
            stepContext.context.activity.from.id,
            this.connectionName,
            stepContext.context.activity.channelId,
            null
        );

        // Show different options if the user is signed in on the parent or not.
        if (!tokenResponse || !tokenResponse.token) {
            // User is not signed in.
            promptChoices.push({ value: 'Login to the root bot' });
            // Token exchange will fail when the root is not logged on and the skill should
            // show a regular OAuthPrompt.
            promptChoices.push({ value: 'Call Skill (without SSO)' });
        } else {
            // User is signed in to the parent
            promptChoices.push({ value: 'Logout from the root bot' });
            promptChoices.push({ value: 'Show token' });
            promptChoices.push({ value: 'Call Skill (with SSO)' });
        }
        return promptChoices;
    }

    async handleActionStep(stepContext) {
        const action = stepContext.result.value.toLowerCase();
        const userTokenClient = stepContext.context.turnState.get(stepContext.context.adapter.UserTokenClientKey);

        switch (action) {
            case 'login to the root bot': {
                return await stepContext.beginDialog(SSO_SIGNIN_DIALOG, null);
            }
            case 'logout from the root bot': {
                const { activity } = stepContext.context;
                await userTokenClient.signOutUser(activity.from.id, this.connectionName, activity.channelId);
                await stepContext.context.sendActivity('You have been signed out.');
                return await stepContext.next();
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
                return await stepContext.next();
            }
            case 'call skill (with sso)':
            case 'call skill (without sso)': {
                const beginSkillActivity = {
                    type: ActivityTypes.Event,
                    name: 'SSO'
                };
                // Save active skill in state (this is use in case of errors).
                await this.activeSkillProperty.set(stepContext.context, this.ssoSkill);
                return await stepContext.beginDialog(SKILL_DIALOG, { activity: beginSkillActivity });
            }
            default: {
                // This should never be hit since the previous prompt validates the choice.
                throw new Error(`[MainDialog]: Unrecognized action: ${ action }`);
            }
        }
    }

    async promptFinalStep(stepContext) {
        // Clear active skill in state.
        await this.activeSkillProperty.delete(stepContext.context);

        // Restart the dialog (we will exit when the user says end).
        return await stepContext.replaceDialog(this.initialDialogId, null);
    }
}
module.exports.MainDialog = MainDialog;

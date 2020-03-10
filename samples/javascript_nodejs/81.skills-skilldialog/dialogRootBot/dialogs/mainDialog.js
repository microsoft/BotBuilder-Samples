// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, DeliveryModes, InputHints, MessageFactory } = require('botbuilder');
const { ChoicePrompt, ComponentDialog, DialogSet, DialogTurnStatus, SkillDialog, WaterfallDialog } = require('botbuilder-dialogs');
const { TangentDialog } = require('../dialogs/tangentDialog');

/**
 * The main dialog for this bot. It uses a SkillDialog to call skills.
 * TODO: add params
 */
class MainDialog extends ComponentDialog {
    constructor(conversationState, skillsConfig, skillClient, conversationIdFactory, tangentDialog) {
        super(MainDialog.name);

        if (!conversationState) throw new Error('[MainDialog]: Missing parameter \'conversationState\' is required');
        if (!skillsConfig) throw new Error('[MainDialog]: Missing parameter \'skillsConfig\' is required');
        if (!skillClient) throw new Error('[MainDialog]: Missing parameter \'skillClient\' is required');
        if (!tangentDialog) throw new Error('[MainDialog]: Missing parameter \'tangentDialog\' is required');
        if (!conversationIdFactory) throw new Error('[MainDialog]: Missing parameter \'conversationIdFactory\' is required');

        if (!process.env.MicrosoftAppId) throw new Error('[MainDialog]: Missing parameter \'MicrosoftAppId\' is required');
        if (!process.env.SkillHostEndpoint) throw new Error('[MainDialog]: Missing parameter \'SkillHostEndpoint\' is required');

        this.activeSkillPropertyName = `${ MainDialog.name }.activeSkillProperty`;
        this.activeSkillProperty = conversationState.createProperty(this.activeSkillPropertyName);
        this.skillsConfig = skillsConfig;
        this.selectedSkillKey = `${ MainDialog.name }.selectedSkillKey`;

        Object.keys(skillsConfig.skills).forEach((skillId) => {
            const skillInfo = skillsConfig.skills[skillId];

            const skillOptions = {
                botId: process.env.MicrosoftAppId,
                conversationIdFactory,
                conversationState,
                skill: skillInfo,
                skillHostEndpoint: process.env.SkillHostEndpoint,
                skillClient
            };

            this.addDialog(new SkillDialog(skillOptions, skillInfo.id));
        });

        // Define the main dialog and its related components.
        // This is a sample "book a flight" dialog.
        this.addDialog(new ChoicePrompt(ChoicePrompt.name))
            .addDialog(tangentDialog)
            .addDialog(new WaterfallDialog(WaterfallDialog.name, [
                this.selectSkillStep.bind(this),
                this.selectSkillActionStep.bind(this),
                this.callSkillActionStep.bind(this),
                this.finalStep.bind(this)
            ]));

        this.initialDialogId = WaterfallDialog.name;
    }

    /**
     * The run method handles the incoming activity (in the form of a TurnContext) and passes it through the dialog system.
     * If no dialog is active, it will start the default dialog.
     * @param {*} turnContext
     * @param {*} accessor
     */
    async run(turnContext, accessor) {
        const dialogSet = new DialogSet(accessor);
        dialogSet.add(this);

        const dialogContext = await dialogSet.createContext(turnContext);
        const results = await dialogContext.continueDialog();
        if (results.status === DialogTurnStatus.empty) {
            await dialogContext.beginDialog(this.id);
        }
    }

    async onContinueDialog(innerDc) {
        const activeSkill = await this.activeSkillProperty.get(innerDc.context, () => null);
        const activity = innerDc.context.activity;
        if (activeSkill != null && activity.type === ActivityTypes.Message && activity.text.toLowerCase() === 'abort') {
            // Cancel all dialogs when the user says "abort"
            await innerDc.cancelAllDialogs();
            return await innerDc.replaceDialog(this.initialDialogId, { text: 'Canceled! \n\n What skill would you like to call?' });
        }

        if (activeSkill != null && activity.type === ActivityTypes.Message && activity.text.toLowerCase() === 'tangent') {
            // Start tangent dialog
            return await innerDc.beginDialog(TangentDialog.name);
        }

        return await super.onContinueDialog(innerDc);
    }

    /**
     * Render a prompt to select the skill to call.
     */
    async selectSkillStep(stepContext) {
        // Create the PromptOptions from the skill configuration which contains the list of configured skills
        const messageText = stepContext.options && stepContext.options.text ? stepContext.options.text : 'What skill would you like to call?';
        const repromptMessageText = 'That was not a valid choice, please select a valid skill.';
        const options = {
            prompt: MessageFactory.text(messageText, messageText, InputHints.ExpectingInput),
            retryPrompt: MessageFactory.text(repromptMessageText, repromptMessageText, InputHints.ExpectingInput),
            choices: Object.keys(this.skillsConfig.skills)
        };

        // Prompt the user to select a skill.
        return await stepContext.prompt(ChoicePrompt.name, options);
    }

    /**
     * Render a prompt to select the action for the skill.
     */
    async selectSkillActionStep(stepContext) {
        // Get the skill info based on the selected skill
        const selectedSkillId = stepContext.result.value;
        const selectedSkill = this.skillsConfig.skills[selectedSkillId];

        // Remember the skill selected by the user
        stepContext.values[this.selectedSkillKey] = selectedSkill;

        // Create the PromptOptions with the actions supported by the selected skill
        const messageText = `What action would you like to call in **${ selectedSkill.id }**?`;
        const repromptMessageText = 'That was not a valid choice, please select a valid action.';
        const options = {
            prompt: MessageFactory.text(messageText, messageText, InputHints.ExpectingInput),
            retryPrompt: MessageFactory.text(repromptMessageText, repromptMessageText, InputHints.ExpectingInput),
            choices: this.getSkillActions(selectedSkill)
        };

        // Prompt the user to select a skill action
        return await stepContext.prompt(ChoicePrompt.name, options);
    }

    /**
     * The SkillDialog has ended, render the results (if any) and restart MainDialog
     * @param {*} stepContext
     */
    async finalStep(stepContext) {
        const activeSkill = await this.activeSkillProperty.get(stepContext.context, () => null);

        if (stepContext.result != null) {
            let message = `Skill "${ activeSkill.id }" invocation complete.`;
            message += `\nResult: ${ JSON.stringify(stepContext.result, null, 2) }`;
            await stepContext.context.sendActivity(MessageFactory.text(message, message, InputHints.IgnoringInput));
        }

        // Clear the skill selected by the user
        stepContext.values[this.selectedSkillKey] = null;

        // Clear active skill in state
        await this.activeSkillProperty.delete(stepContext.context);

        // Restart the main dialog with a different message the second time around
        return await stepContext.replaceDialog(this.initialDialogId, { text: `Done with "${ activeSkill.id }". \n\n What skill would you like to call?` });
    }

    async callSkillActionStep(stepContext) {
        const selectedSkill = stepContext.values[this.selectedSkillKey];

        let skillActivity;
        switch (selectedSkill.id) {
        // Echo skill only handles message activities, send a dummy utterance to get it started
        case 'EchoSkillBot':
            skillActivity = MessageFactory.text('Start echo skill');
            break;
        case 'DialogSkillBot':
            skillActivity = this.createDialogSkillBotActivity(stepContext.result);
            break;
        default:
            throw new Error(`Unknown target skill id: ${ selectedSkill.id }`);
        }

        // Create the BeginSkillDialogOptions
        const skillDialogArgs = { activity: skillActivity };

        // We are manually creating the activity to send to the skill, ensure we add the ChannelData and Properties
        // from the original activity so the skill gets them.
        // Note: this is not necessary if we are just forwarding the current activity from context.
        skillDialogArgs.activity.channelData = stepContext.context.activity.channelData;
        skillDialogArgs.activity.properties = stepContext.context.activity.properties;

        // Comment or uncomment this line if you need to enable or disable buffered replies
        skillDialogArgs.activity.deliveryMode = DeliveryModes.ExpectReplies;

        // Save active skill in state
        await this.activeSkillProperty.set(stepContext.context, selectedSkill);

        // Start the skillDialog instance with the arguments
        return await stepContext.beginDialog(selectedSkill.id, skillDialogArgs);
    }

    /**
     * Helper method to create Choice elements for the actions supported by the skill
     */
    getSkillActions(skill) {
        // Note: the bot would probably render this by reading the skill manifest
        // We are just using hardcoded skill actions here for simplicity
        const choices = [];
        switch (skill.id) {
        case 'EchoSkillBot':
            choices.push({ value: 'Messages' });
            break;
        case 'DialogSkillBot':
            choices.push({ value: 'm:some message for tomorrow' });
            choices.push({ value: 'BookFlight' });
            choices.push({ value: 'OAuthTest' });
            choices.push({ value: 'mv:some message with value' });
            choices.push({ value: 'BookFlightWithValues' });
            break;
        }

        return choices;
    }

    /**
     * // Helper method to create the activity to be sent to the DialogSkillBot using selected type and values
     */
    createDialogSkillBotActivity(selectedOption) {
        // Note: in a real bot, the dialogArgs will be created dynamically based on the conversation
        // and what each action requires, here we hardcode the values to make things simpler.

        // Send a message activity to the skill
        if (selectedOption.value.toLowerCase().startsWith('m:')) {
            const activity = MessageFactory.text(selectedOption.substr(2).trim());
            return activity;
        }

        // Send a message activity to the skill with some artificial parameters in value
        if (selectedOption.value.toLowerCase().startsWith('mv:')) {
            const activity = MessageFactory.text(selectedOption.substr(3).trim());
            activity.value = { destination: 'New York' };
            return activity;
        }

        // Send an event activity to the skill with "OAuthTest" in the name
        if (selectedOption.value.toLowerCase() === 'oauthtest') {
            return { type: ActivityTypes.Event, name: 'OAuthTest' };
        }

        // Send an event activity to the skill with "BookFlight" in the name
        if (selectedOption.value.toLowerCase() === 'bookflight') {
            return { type: ActivityTypes.Event, name: 'BookFlight' };
        }

        // Send an event activity to the skill "BookFlight" in the name and some testing values
        if (selectedOption.value.toLowerCase() === 'bookflightwithvalues') {
            return {
                type: ActivityTypes.Event,
                name: 'BookFlight',
                value: {
                    destination: 'New York',
                    origin: 'Seattle'
                }
            };
        }

        throw new Error(`Unable to create dialogArgs for ${ selectedOption.value }`);
    }
}

module.exports.MainDialog = MainDialog;

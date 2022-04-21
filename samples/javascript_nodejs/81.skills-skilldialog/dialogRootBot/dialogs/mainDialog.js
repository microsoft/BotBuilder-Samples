// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, InputHints, MessageFactory } = require('botbuilder');
const { ChoicePrompt, ComponentDialog, DialogSet, DialogTurnStatus, SkillDialog, WaterfallDialog } = require('botbuilder-dialogs');

const MAIN_DIALOG = 'MainDialog';
const SKILL_PROMPT = 'SkillPrompt';
const SKILL_ACTION_PROMPT = 'SkillActionPrompt';
const WATERFALL_DIALOG = 'WaterfallDialog';

const SKILL_ACTION_BOOK_FLIGHT = 'BookFlight';
const SKILL_ACTION_BOOK_FLIGHT_WITH_INPUT_PARAMETERS = 'BookFlight with input parameters';
const SKILL_ACTION_GET_WEATHER = 'GetWeather';
const SKILL_ACTION_MESSAGE = 'Message';

/**
 * The main dialog for this bot. It uses a SkillDialog to call skills.
 */
class MainDialog extends ComponentDialog {
    constructor(conversationState, skillsConfig, skillClient, conversationIdFactory) {
        super(MAIN_DIALOG);

        if (!conversationState) throw new Error('[MainDialog]: Missing parameter \'conversationState\' is required');
        if (!skillsConfig) throw new Error('[MainDialog]: Missing parameter \'skillsConfig\' is required');
        if (!skillClient) throw new Error('[MainDialog]: Missing parameter \'skillClient\' is required');
        if (!conversationIdFactory) throw new Error('[MainDialog]: Missing parameter \'conversationIdFactory\' is required');

        this.activeSkillPropertyName = `${ MAIN_DIALOG }.activeSkillProperty`;
        this.activeSkillProperty = conversationState.createProperty(this.activeSkillPropertyName);
        this.skillsConfig = skillsConfig;
        this.selectedSkillKey = `${ MAIN_DIALOG }.selectedSkillKey`;

        // Use helper method to add SkillDialog instances for the configured skills.
        this.addSkillDialogs(conversationState, conversationIdFactory, skillClient, skillsConfig, process.env.MicrosoftAppId);

        // Define the main dialog and its related components.
        // Add ChoicePrompt to render available skills.
        this.addDialog(new ChoicePrompt(SKILL_PROMPT))
            // Add ChoicePrompt to render skill actions.
            .addDialog(new ChoicePrompt(SKILL_ACTION_PROMPT, this.skillActionPromptValidator))
            // Add main waterfall dialog for this bot.
            .addDialog(new WaterfallDialog(WATERFALL_DIALOG, [
                this.selectSkillStep.bind(this),
                this.selectSkillActionStep.bind(this),
                this.callSkillActionStep.bind(this),
                this.finalStep.bind(this)
            ]));

        this.initialDialogId = WATERFALL_DIALOG;
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
            // Cancel all dialogs when the user says abort.
            // The SkillDialog automatically sends an EndOfConversation message to the skill to let the
            // skill know that it needs to end its current dialogs, too.
            await innerDc.cancelAllDialogs();
            return await innerDc.replaceDialog(this.initialDialogId, { text: 'Canceled! \n\n What skill would you like to call?' });
        }

        return await super.onContinueDialog(innerDc);
    }

    /**
     * Render a prompt to select the skill to call.
     */
    async selectSkillStep(stepContext) {
        // Create the PromptOptions from the skill configuration which contains the list of configured skills.
        const messageText = stepContext.options && stepContext.options.text ? stepContext.options.text : 'What skill would you like to call?';
        const repromptMessageText = 'That was not a valid choice, please select a valid skill.';
        const options = {
            prompt: MessageFactory.text(messageText, messageText, InputHints.ExpectingInput),
            retryPrompt: MessageFactory.text(repromptMessageText, repromptMessageText, InputHints.ExpectingInput),
            choices: Object.keys(this.skillsConfig.skills)
        };

        // Prompt the user to select a skill.
        return await stepContext.prompt(SKILL_PROMPT, options);
    }

    /**
     * Render a prompt to select the action for the skill.
     */
    async selectSkillActionStep(stepContext) {
        // Get the skill info based on the selected skill.
        const selectedSkillId = stepContext.result.value;
        const selectedSkill = this.skillsConfig.skills[selectedSkillId];

        // Remember the skill selected by the user.
        stepContext.values[this.selectedSkillKey] = selectedSkill;

        // Create the PromptOptions with the actions supported by the selected skill.
        const messageText = `Select an action # to send to **${ selectedSkill.id }** or just type in a message and it will be forwarded to the skill`;
        const options = {
            prompt: MessageFactory.text(messageText, messageText, InputHints.ExpectingInput),
            choices: this.getSkillActions(selectedSkill)
        };

        // Prompt the user to select a skill action.
        return await stepContext.prompt(SKILL_ACTION_PROMPT, options);
    }

    /**
     * Starts the SkillDialog based on the user's selections.
     */
    async callSkillActionStep(stepContext) {
        const selectedSkill = stepContext.values[this.selectedSkillKey];

        let skillActivity;
        switch (selectedSkill.id) {
            case 'DialogSkillBot':
                skillActivity = this.createDialogSkillBotActivity(stepContext.result.value, stepContext.context);
                break;
            // We can add other case statements here if we support more than one skill.
            default:
                throw new Error(`Unknown target skill id: ${ selectedSkill.id }`);
        }

        // Create the BeginSkillDialogOptions and assign the activity to send.
        const skillDialogArgs = { activity: skillActivity };

        // Save active skill in state.
        await this.activeSkillProperty.set(stepContext.context, selectedSkill);

        // Start the skillDialog instance with the arguments.
        return await stepContext.beginDialog(selectedSkill.id, skillDialogArgs);
    }

    /**
     * The SkillDialog has ended, render the results (if any) and restart MainDialog.
     */
    async finalStep(stepContext) {
        const activeSkill = await this.activeSkillProperty.get(stepContext.context, () => null);

        // Check if the skill returned any results and display them.
        if (stepContext.result != null) {
            let message = `Skill "${ activeSkill.id }" invocation complete.`;
            message += `\nResult: ${ JSON.stringify(stepContext.result, null, 2) }`;
            await stepContext.context.sendActivity(message, message, InputHints.IgnoringInput);
        }

        // Clear the skill selected by the user.
        stepContext.values[this.selectedSkillKey] = null;

        // Clear active skill in state.
        await this.activeSkillProperty.delete(stepContext.context);

        // Restart the main dialog with a different message the second time around.
        return await stepContext.replaceDialog(this.initialDialogId, { text: `Done with "${ activeSkill.id }". \n\n What skill would you like to call?` });
    }

    /**
     * Helper method that creates and adds SkillDialog instances for the configured skills.
     */
    async addSkillDialogs(conversationState, conversationIdFactory, skillClient, skillsConfig, botId) {
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

            // Add a SkillDialog for the selected skill.
            this.addDialog(new SkillDialog(skillOptions, skillInfo.id));
        });
    }

    /**
     * Helper method to create Choice elements for the actions supported by the skill.
     */
    getSkillActions(skill) {
        // Note: The bot would probably render this by reading the skill manifest.
        // We are just using hardcoded skill actions here for simplicity.
        const choices = [];
        switch (skill.id) {
            case 'DialogSkillBot':
                choices.push({ value: SKILL_ACTION_BOOK_FLIGHT });
                choices.push({ value: SKILL_ACTION_BOOK_FLIGHT_WITH_INPUT_PARAMETERS });
                choices.push({ value: SKILL_ACTION_GET_WEATHER });
                break;
        }

        return choices;
    }

    /**
     * Helper method to create the activity to be sent to the DialogSkillBot using selected type and values.
     */
    createDialogSkillBotActivity(selectedOption, turnContext) {
        // Note: In a real bot, the dialogArgs will be created dynamically based on the conversation
        // and what each action requires; here we hardcode the values to make things simpler.

        // Just forward the message activity to the skill with whatever the user said.
        if (selectedOption.toLowerCase() === SKILL_ACTION_MESSAGE.toLowerCase()) {
            // Note: Message activities also support input parameters but we are not using them in this sample.
            return turnContext.activity;
        }

        let activity;
        // Send an event activity to the skill with "BookFlight" in the name.
        if (selectedOption.toLowerCase() === SKILL_ACTION_BOOK_FLIGHT.toLowerCase()) {
            activity = { type: ActivityTypes.Event, name: SKILL_ACTION_BOOK_FLIGHT };
        }

        // Send an event activity to the skill "BookFlight" in the name and some testing values.
        if (selectedOption.toLowerCase() === SKILL_ACTION_BOOK_FLIGHT_WITH_INPUT_PARAMETERS.toLowerCase()) {
            activity = {
                type: ActivityTypes.Event,
                name: SKILL_ACTION_BOOK_FLIGHT,
                value: {
                    destination: 'New York',
                    origin: 'Seattle'
                }
            };
        }

        // Send an activity to the skill with "GetWeather" in the name and some testing values.
        if (selectedOption.toLowerCase() === SKILL_ACTION_GET_WEATHER.toLowerCase()) {
            activity = {
                type: ActivityTypes.Event,
                name: SKILL_ACTION_GET_WEATHER,
                value: {
                    latitude: 47.614891,
                    longitude: -122.195801
                }
            };
        }

        if (!activity) {
            throw new Error(`Unable to create dialogArgs for ${ selectedOption }`);
        }

        // We are manually creating the activity to send to the skill; ensure we add the ChannelData and Properties
        // from the original activity so the skill gets them.
        // Note: This is not necessary if we are just forwarding the current activity from context.
        activity.channelData = turnContext.activity.channelData;
        activity.properties = turnContext.activity.properties;

        return activity;
    }

    /**
     * This validator defaults to Message if the user doesn't select an existing option.
     */
    async skillActionPromptValidator(promptContext) {
        if (!promptContext.recognized.succeeded) {
            promptContext.recognized.value = { value: SKILL_ACTION_MESSAGE };
        }

        return true;
    }
}

module.exports.MainDialog = MainDialog;

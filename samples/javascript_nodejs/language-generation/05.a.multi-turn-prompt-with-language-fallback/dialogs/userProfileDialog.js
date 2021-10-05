// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const {
    ChoiceFactory,
    ChoicePrompt,
    ComponentDialog,
    ConfirmPrompt,
    DialogSet,
    DialogTurnStatus,
    NumberPrompt,
    TextPrompt,
    WaterfallDialog
} = require('botbuilder-dialogs');

const { Templates, MultiLanguageLG } = require('botbuilder-lg');
const { ActivityFactory } = require('botbuilder');
const { UserProfile } = require('../userProfile');

const CHOICE_PROMPT = 'CHOICE_PROMPT';
const CONFIRM_PROMPT = 'CONFIRM_PROMPT';
const NAME_PROMPT = 'NAME_PROMPT';
const NUMBER_PROMPT = 'NUMBER_PROMPT';
const USER_PROFILE = 'USER_PROFILE';
const WATERFALL_DIALOG = 'WATERFALL_DIALOG';

class UserProfileDialog extends ComponentDialog {
    constructor(userState) {
        super('userProfileDialog');

        this.userProfile = userState.createProperty(USER_PROFILE);

        this.addDialog(new TextPrompt(NAME_PROMPT));
        this.addDialog(new ChoicePrompt(CHOICE_PROMPT));
        this.addDialog(new ConfirmPrompt(CONFIRM_PROMPT));
        this.addDialog(new NumberPrompt(NUMBER_PROMPT, this.agePromptValidator));

        this.addDialog(new WaterfallDialog(WATERFALL_DIALOG, [
            this.transportStep.bind(this),
            this.nameStep.bind(this),
            this.nameConfirmStep.bind(this),
            this.ageStep.bind(this),
            this.confirmStep.bind(this),
            this.summaryStep.bind(this)
        ]));

        this.initialDialogId = WATERFALL_DIALOG;

        // Define per locale LG files.
        const templatesPerLocale = new Map();
        templatesPerLocale.set('', Templates.parseFile(`${ __dirname }/../Resources/UserProfileDialog.lg`));
        templatesPerLocale.set('fr', Templates.parseFile(`${ __dirname }/../Resources/UserProfileDialog.fr-fr.lg`));
        this.lgTemplates = new MultiLanguageLG(templatesPerLocale);
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

    async transportStep(step) {
        // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
        // Running a prompt here means the next WaterfallStep will be run when the users response is received.
        return await step.prompt(CHOICE_PROMPT, {
            prompt: ActivityFactory.fromObject(this.lgTemplates.generate('ModeOfTransportPrompt', undefined, step.context.activity.locale)),
            choices: ChoiceFactory.toChoices(['Car', 'Bus', 'Bicycle'])
        });
    }

    async nameStep(step) {
        step.values.transport = step.result.value;
        return await step.prompt(NAME_PROMPT, ActivityFactory.fromObject(this.lgTemplates.generate('AskForName', undefined, step.context.activity.locale)));
    }

    async nameConfirmStep(step) {
        step.values.name = step.result;

        // We can send messages to the user at any point in the WaterfallStep.
        await step.context.sendActivity(ActivityFactory.fromObject(this.lgTemplates.generate('AckName', step.values, step.context.activity.locale)));

        // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
        return await step.prompt(CONFIRM_PROMPT, ActivityFactory.fromObject(this.lgTemplates.generate('AgeConfirmPrompt', undefined, step.context.activity.locale)), ['yes', 'no']);
    }

    async ageStep(step) {
        if (step.result) {
            // User said "yes" so we will be prompting for the age.
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is a Prompt Dialog.
            const promptOptions = {
                prompt: ActivityFactory.fromObject(this.lgTemplates.generate('AskForAge', undefined, step.context.activity.locale)),
                retryPrompt: ActivityFactory.fromObject(this.lgTemplates.generate('AskForAge.reprompt', undefined, step.context.activity.locale))
            };

            return await step.prompt(NUMBER_PROMPT, promptOptions);
        } else {
            // User said "no" so we will skip the next step. Give -1 as the age.
            return await step.next(-1);
        }
    }

    async confirmStep(step) {
        step.values.age = step.result;

        // We can send messages to the user at any point in the WaterfallStep.
        await step.context.sendActivity(ActivityFactory.fromObject(this.lgTemplates.generate('AgeReadBack', {
            userAge: step.values.age
        }, step.context.activity.locale)));

        // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is a Prompt Dialog.
        return await step.prompt(CONFIRM_PROMPT, { prompt: ActivityFactory.fromObject(this.lgTemplates.generate('ConfirmPrompt', undefined, step.context.activity.locale)) });
    }

    async summaryStep(step) {
        if (step.result) {
            // Get the current profile object from user state.
            const userProfile = await this.userProfile.get(step.context, new UserProfile());

            userProfile.transport = step.values.transport;
            userProfile.name = step.values.name;
            userProfile.age = step.values.age;

            await step.context.sendActivity(ActivityFactory.fromObject(this.lgTemplates.generate('SummaryReadout', userProfile, step.context.activity.locale)));
        } else {
            await step.context.sendActivity(ActivityFactory.fromObject(this.lgTemplates.generate('NoProfileReadBack', undefined, step.context.activity.locale)));
        }

        // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is the end.
        return await step.endDialog();
    }

    async agePromptValidator(promptContext) {
        // This condition is our validation rule. You can also change the value at this point.
        return promptContext.recognized.succeeded && promptContext.recognized.value > 0 && promptContext.recognized.value < 150;
    }
}

module.exports.UserProfileDialog = UserProfileDialog;

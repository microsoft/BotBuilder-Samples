// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { MessageFactory } = require('botbuilder');
const {
    AttachmentPrompt,
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
const { Channels } = require('botbuilder-core');
const { UserProfile } = require('../userProfile');

const ATTACHMENT_PROMPT = 'ATTACHMENT_PROMPT';
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
        this.addDialog(new AttachmentPrompt(ATTACHMENT_PROMPT, this.picturePromptValidator));

        this.addDialog(new WaterfallDialog(WATERFALL_DIALOG, [
            this.transportStep.bind(this),
            this.nameStep.bind(this),
            this.nameConfirmStep.bind(this),
            this.ageStep.bind(this),
            this.pictureStep.bind(this),
            this.confirmStep.bind(this),
            this.summaryStep.bind(this)
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

    async transportStep(step) {
        // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
        // Running a prompt here means the next WaterfallStep will be run when the user's response is received.
        return await step.prompt(CHOICE_PROMPT, {
            prompt: 'Please enter your mode of transport.',
            choices: ChoiceFactory.toChoices(['Car', 'Bus', 'Bicycle'])
        });
    }

    async nameStep(step) {
        step.values.transport = step.result.value;
        return await step.prompt(NAME_PROMPT, 'Please enter your name.');
    }

    async nameConfirmStep(step) {
        step.values.name = step.result;

        // We can send messages to the user at any point in the WaterfallStep.
        await step.context.sendActivity(`Thanks ${ step.result }.`);

        // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
        return await step.prompt(CONFIRM_PROMPT, 'Do you want to give your age?', ['yes', 'no']);
    }

    async ageStep(step) {
        if (step.result) {
            // User said "yes" so we will be prompting for the age.
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            const promptOptions = { prompt: 'Please enter your age.', retryPrompt: 'The value entered must be greater than 0 and less than 150.' };

            return await step.prompt(NUMBER_PROMPT, promptOptions);
        } else {
            // User said "no" so we will skip the next step. Give -1 as the age.
            return await step.next(-1);
        }
    }

    async pictureStep(step) {
        step.values.age = step.result;

        const msg = step.values.age === -1 ? 'No age given.' : `I have your age as ${ step.values.age }.`;

        // We can send messages to the user at any point in the WaterfallStep.
        await step.context.sendActivity(msg);

        if (step.context.activity.channelId === Channels.msteams) {
            // This attachment prompt example is not designed to work for Teams attachments, so skip it in this case
            await step.context.sendActivity('Skipping attachment prompt in Teams channel...');
            return await step.next(undefined);
        } else {
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            var promptOptions = {
                prompt: 'Please attach a profile picture (or type any message to skip).',
                retryPrompt: 'The attachment must be a jpeg/png image file.'
            };

            return await step.prompt(ATTACHMENT_PROMPT, promptOptions);
        }
    }

    async confirmStep(step) {
        step.values.picture = step.result && step.result[0];

        // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
        return await step.prompt(CONFIRM_PROMPT, { prompt: 'Is this okay?' });
    }

    async summaryStep(step) {
        if (step.result) {
            // Get the current profile object from user state.
            const userProfile = await this.userProfile.get(step.context, new UserProfile());

            userProfile.transport = step.values.transport;
            userProfile.name = step.values.name;
            userProfile.age = step.values.age;
            userProfile.picture = step.values.picture;

            let msg = `I have your mode of transport as ${ userProfile.transport } and your name as ${ userProfile.name }`;
            if (userProfile.age !== -1) {
                msg += ` and your age as ${ userProfile.age }`;
            }

            msg += '.';
            await step.context.sendActivity(msg);
            if (userProfile.picture) {
                try {
                    await step.context.sendActivity(MessageFactory.attachment(userProfile.picture, 'This is your profile picture.'));
                } catch {
                    await step.context.sendActivity('A profile picture was saved but could not be displayed here.');
                }
            }
        } else {
            await step.context.sendActivity('Thanks. Your profile will not be kept.');
        }

        // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is the end.
        return await step.endDialog();
    }

    async agePromptValidator(promptContext) {
        // This condition is our validation rule. You can also change the value at this point.
        return promptContext.recognized.succeeded && promptContext.recognized.value > 0 && promptContext.recognized.value < 150;
    }

    async picturePromptValidator(promptContext) {
        if (promptContext.recognized.succeeded) {
            var attachments = promptContext.recognized.value;
            var validImages = [];

            attachments.forEach(attachment => {
                if (attachment.contentType === 'image/jpeg' || attachment.contentType === 'image/png') {
                    validImages.push(attachment);
                }
            });

            promptContext.recognized.value = validImages;

            // If none of the attachments are valid images, the retry prompt should be sent.
            return !!validImages.length;
        }
        else {
            await promptContext.context.sendActivity('No attachments received. Proceeding without a profile picture...');

            // We can return true from a validator function even if Recognized.Succeeded is false.
            return true;
        }
    }
}

module.exports.UserProfileDialog = UserProfileDialog;

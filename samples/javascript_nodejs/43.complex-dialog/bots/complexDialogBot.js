// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, ActivityHandler } = require('botbuilder');
const { DialogSet, WaterfallDialog, TextPrompt, NumberPrompt, ChoicePrompt, DialogTurnStatus } = require('botbuilder-dialogs');

// Define state property accessor names.
const DIALOG_STATE_PROPERTY = 'dialogStateProperty';
const USER_PROFILE_PROPERTY = 'userProfileProperty';

const WELCOME_TEXT = 'Welcome to ComplexDialogBot. This bot provides a complex conversation, with multiple dialogs. Type anything to get started.';

// Define the dialog and prompt names for the bot.
const TOP_LEVEL_DIALOG = 'dialog-topLevel';
const REVIEW_SELECTION_DIALOG = 'dialog-reviewSelection';
const NAME_PROMPT = 'prompt-name';
const AGE_PROMPT = 'prompt-age';
const SELECTION_PROMPT = 'prompt-companySelection';

// Define a 'done' response for the company selection prompt.
const DONE_OPTION = 'done';

// Define value names for values tracked inside the dialogs.
const USER_INFO = 'value-userInfo';
const COMPANIES_SELECTED = 'value-companiesSelected';

// Define the company choices for the company selection prompt.
const COMPANY_OPTIONS = [
    'Adatum Corporation', 'Contoso Suites', 'Graphic Design Institute', 'Wide World Importers'
];

class ComplexDialogBot extends ActivityHandler {
    /**
     *
     * @param {ConversationState} conversation state object
     * @param {UserState} user state object
     */
    constructor(conversationState, userState) {
        super();
        // Create the state property accessors and save the state management objects.
        this.dialogStateAccessor = conversationState.createProperty(DIALOG_STATE_PROPERTY);
        this.userProfileAccessor = userState.createProperty(USER_PROFILE_PROPERTY);
        this.conversationState = conversationState;
        this.userState = userState;

        // Create a dialog set for the bot. It requires a DialogState accessor, with which
        // to retrieve the dialog state from the turn context.
        this.dialogs = new DialogSet(this.dialogStateAccessor);

        // Add the prompts we need to the dialog set.
        this.dialogs
            .add(new TextPrompt(NAME_PROMPT))
            .add(new NumberPrompt(AGE_PROMPT))
            .add(new ChoicePrompt(SELECTION_PROMPT));

        // Add the dialogs we need to the dialog set.
        this.dialogs.add(new WaterfallDialog(TOP_LEVEL_DIALOG)
            .addStep(this.nameStep.bind(this))
            .addStep(this.ageStep.bind(this))
            .addStep(this.startSelectionStep.bind(this))
            .addStep(this.acknowledgementStep.bind(this)));
        this.dialogs.add(new WaterfallDialog(REVIEW_SELECTION_DIALOG)
            .addStep(this.selectionStep.bind(this))
            .addStep(this.loopStep.bind(this)));
    }

    /**
     *
     * @param {TurnContext} on turn context object.
     */
    async onTurn(turnContext) {
        if (turnContext.activity.type === ActivityTypes.Message) {
            // Run the DialogSet - let the framework identify the current state of the dialog from
            // the dialog stack and figure out what (if any) is the active dialog.
            const dialogContext = await this.dialogs.createContext(turnContext);
            const results = await dialogContext.continueDialog();
            switch (results.status) {
            case DialogTurnStatus.cancelled:
            case DialogTurnStatus.empty:
                // If there is no active dialog, we should clear the user info and start a new dialog.
                await this.userProfileAccessor.set(turnContext, {});
                await this.userState.saveChanges(turnContext);
                await dialogContext.beginDialog(TOP_LEVEL_DIALOG);
                break;
            case DialogTurnStatus.complete:
                // If we just finished the dialog, capture and display the results.
                const userInfo = results.result;
                const status = 'You are signed up to review ' +
                    (userInfo.companiesToReview.length === 0 ? 'no companies' : userInfo.companiesToReview.join(' and ')) + '.';
                await turnContext.sendActivity(status);
                await this.userProfileAccessor.set(turnContext, userInfo);
                await this.userState.saveChanges(turnContext);
                break;
            case DialogTurnStatus.waiting:
                // If there is an active dialog, we don't need to do anything here.
                break;
            }
            await this.conversationState.saveChanges(turnContext);
        } else if (turnContext.activity.type === ActivityTypes.ConversationUpdate) {
            if (turnContext.activity.membersAdded && turnContext.activity.membersAdded.length > 0) {
                await this.sendWelcomeMessage(turnContext);
            }
        } else {
            await turnContext.sendActivity(`[${ turnContext.activity.type } event detected]`);
        }
    }

    // Sends a welcome message to any users who joined the conversation.
    async sendWelcomeMessage(turnContext) {
        for (var idx in turnContext.activity.membersAdded) {
            if (turnContext.activity.membersAdded[idx].id !== turnContext.activity.recipient.id) {
                await turnContext.sendActivity(WELCOME_TEXT);
            }
        }
    }

    async nameStep(stepContext) {
        // Create an object in which to collect the user's information within the dialog.
        stepContext.values[USER_INFO] = {};

        // Ask the user to enter their name.
        return await stepContext.prompt(NAME_PROMPT, 'Please enter your name.');
    }

    async ageStep(stepContext) {
        // Set the user's name to what they entered in response to the name prompt.
        stepContext.values[USER_INFO].name = stepContext.result;

        // Ask the user to enter their age.
        return await stepContext.prompt(AGE_PROMPT, 'Please enter your age.');
    }

    async startSelectionStep(stepContext) {
        // Set the user's age to what they entered in response to the age prompt.
        stepContext.values[USER_INFO].age = stepContext.result;

        if (stepContext.result < 25) {
            // If they are too young, skip the review selection dialog, and pass an empty list to the next step.
            await stepContext.context.sendActivity('You must be 25 or older to participate.');
            return await stepContext.next([]);
        } else {
            // Otherwise, start the review selection dialog.
            return await stepContext.beginDialog(REVIEW_SELECTION_DIALOG);
        }
    }

    async acknowledgementStep(stepContext) {
        // Set the user's company selection to what they entered in the review-selection dialog.
        const list = stepContext.result || [];
        stepContext.values[USER_INFO].companiesToReview = list;

        // Thank them for participating.
        await stepContext.context.sendActivity(`Thanks for participating, ${ stepContext.values[USER_INFO].name }.`);

        // Exit the dialog, returning the collected user information.
        return await stepContext.endDialog(stepContext.values[USER_INFO]);
    }

    async selectionStep(stepContext) {
        // Continue using the same selection list, if any, from the previous iteration of this dialog.
        const list = Array.isArray(stepContext.options) ? stepContext.options : [];
        stepContext.values[COMPANIES_SELECTED] = list;

        // Create a prompt message.
        let message;
        if (list.length === 0) {
            message = 'Please choose a company to review, or `' + DONE_OPTION + '` to finish.';
        } else {
            message = `You have selected **${ list[0] }**. You can review an addition company, ` +
                'or choose `' + DONE_OPTION + '` to finish.';
        }

        // Create the list of options to choose from.
        const options = list.length > 0
            ? COMPANY_OPTIONS.filter(function(item) { return item !== list[0]; })
            : COMPANY_OPTIONS.slice();
        options.push(DONE_OPTION);

        // Prompt the user for a choice.
        return await stepContext.prompt(SELECTION_PROMPT, {
            prompt: message,
            retryPrompt: 'Please choose an option from the list.',
            choices: options
        });
    }

    async loopStep(stepContext) {
        // Retrieve their selection list, the choice they made, and whether they chose to finish.
        const list = stepContext.values[COMPANIES_SELECTED];
        const choice = stepContext.result;
        const done = choice.value === DONE_OPTION;

        if (!done) {
            // If they chose a company, add it to the list.
            list.push(choice.value);
        }

        if (done || list.length > 1) {
            // If they're done, exit and return their list.
            return await stepContext.endDialog(list);
        } else {
            // Otherwise, repeat this dialog, passing in the list from this iteration.
            return await stepContext.replaceDialog(REVIEW_SELECTION_DIALOG, list);
        }
    }
}

module.exports.ComplexDialogBot = ComplexDialogBot;

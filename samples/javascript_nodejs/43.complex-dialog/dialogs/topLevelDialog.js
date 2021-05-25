// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ComponentDialog, NumberPrompt, TextPrompt, WaterfallDialog } = require('botbuilder-dialogs');
const { ReviewSelectionDialog, REVIEW_SELECTION_DIALOG } = require('./reviewSelectionDialog');
const { UserProfile } = require('../userProfile');

const TOP_LEVEL_DIALOG = 'TOP_LEVEL_DIALOG';

const WATERFALL_DIALOG = 'WATERFALL_DIALOG';
const TEXT_PROMPT = 'TEXT_PROMPT';
const NUMBER_PROMPT = 'NUMBER_PROMPT';

class TopLevelDialog extends ComponentDialog {
    constructor() {
        super(TOP_LEVEL_DIALOG);
        this.addDialog(new TextPrompt(TEXT_PROMPT));
        this.addDialog(new NumberPrompt(NUMBER_PROMPT));

        this.addDialog(new ReviewSelectionDialog());

        this.addDialog(new WaterfallDialog(WATERFALL_DIALOG, [
            this.nameStep.bind(this),
            this.ageStep.bind(this),
            this.startSelectionStep.bind(this),
            this.acknowledgementStep.bind(this)
        ]));

        this.initialDialogId = WATERFALL_DIALOG;
    }

    async nameStep(stepContext) {
        // Create an object in which to collect the user's information within the dialog.
        stepContext.values.userInfo = new UserProfile();

        const promptOptions = { prompt: 'Please enter your name.' };

        // Ask the user to enter their name.
        return await stepContext.prompt(TEXT_PROMPT, promptOptions);
    }

    async ageStep(stepContext) {
        // Set the user's name to what they entered in response to the name prompt.
        stepContext.values.userInfo.name = stepContext.result;

        const promptOptions = { prompt: 'Please enter your age.' };

        // Ask the user to enter their age.
        return await stepContext.prompt(NUMBER_PROMPT, promptOptions);
    }

    async startSelectionStep(stepContext) {
        // Set the user's age to what they entered in response to the age prompt.
        stepContext.values.userInfo.age = stepContext.result;

        if (stepContext.result < 25) {
            // If they are too young, skip the review selection dialog, and pass an empty list to the next step.
            await stepContext.context.sendActivity('You must be 25 or older to participate.');

            return await stepContext.next();
        } else {
            // Otherwise, start the review selection dialog.
            return await stepContext.beginDialog(REVIEW_SELECTION_DIALOG);
        }
    }

    async acknowledgementStep(stepContext) {
        // Set the user's company selection to what they entered in the review-selection dialog.
        const userProfile = stepContext.values.userInfo;
        userProfile.companiesToReview = stepContext.result || [];

        await stepContext.context.sendActivity(`Thanks for participating ${ userProfile.name }`);

        // Exit the dialog, returning the collected user information.
        return await stepContext.endDialog(userProfile);
    }
}

module.exports.TopLevelDialog = TopLevelDialog;
module.exports.TOP_LEVEL_DIALOG = TOP_LEVEL_DIALOG;

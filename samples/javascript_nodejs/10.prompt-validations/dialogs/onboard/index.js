// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ComponentDialog, WaterfallDialog } = require('botbuilder-dialogs');
const moment = require('moment');

// Import a few specialized prompt classes.
const { AgePrompt } = require('../../prompts/agePrompt');
const { ColorPrompt } = require('../../prompts/colorPrompt');
const { DobPrompt } = require('../../prompts/dobPrompt');
const { NamePrompt } = require('../../prompts/namePrompt');

const START_DIALOG = 'start';
const HELLO_USER = 'welcome_back';

const GET_NAME_PROMPT = 'namePrompt';
const GET_AGE_PROMPT = 'agePrompt';
const GET_DOB_PROMPT = 'dobPrompt';
const GET_COLOR_PROMPT = 'colorPrompt';

const USER_NAME_PROPERTY = 'name';
const AGE_PROPERTY = 'age';
const DOB_PROPERTY = 'dob';
const COLOR_PROPERTY = 'color';

class OnboardingDialog extends ComponentDialog {
    /**
     *
     * @param {string} dialogId A unique identifier for this dialog.
     * @param {BotStatePropertyAccessor} userProfile A property used to store the user's name.
     */
    constructor(dialogId, userProfile) {
        super(dialogId);

        this.userProfile = userProfile;

        // Create a dialog flow that captures a series of values from a user.
        this.addDialog(new WaterfallDialog(START_DIALOG, [
            this.promptForName.bind(this),
            this.promptForAge.bind(this),
            this.promptForDob.bind(this),
            this.promptForColor.bind(this),
            this.captureColor.bind(this),
            this.completeProfile.bind(this)
        ]));

        // This dialog loads and displays the information previously provided by the user.
        this.addDialog(new WaterfallDialog(HELLO_USER, [
            this.displayProfile.bind(this)
        ]));

        // Add prompts
        // GET_NAME_PROMPT will validate that the user's response is between 1 and 50 chars in length.
        this.addDialog(new NamePrompt(GET_NAME_PROMPT));

        // GET_AGE_PROMPT will validate an age between 1 and 99.
        this.addDialog(new AgePrompt(GET_AGE_PROMPT));

        // GET_DOB_PROMPT will validate a date between 8/24/1918 and 8/24/2018.
        this.addDialog(new DobPrompt(GET_DOB_PROMPT));

        // GET_COLOR_PROMPT provides a validation error when a valid choice is not made.
        this.addDialog(new ColorPrompt(GET_COLOR_PROMPT));
    }

    // If a user name is already set, switch to the HELLO_USER dialog.
    // Otherwise, continue with collecting values from the user.
    async promptForName(step) {
        const user = await this.userProfile.get(step.context, {});
        if (user.name) {
            return await step.replaceDialog(HELLO_USER);
        } else {
            return await step.prompt(GET_NAME_PROMPT, `What is your name, human?`);
        }
    }

    // Collect user name, then prompt for age.
    async promptForAge(step) {
        // Capture the response from the previous turn in step.values
        // which will be stored through the end of the dialog.
        step.values[USER_NAME_PROPERTY] = step.result;
        return await step.prompt(GET_AGE_PROMPT, `What is your age?`);
    }

    // Collect age, then prompt for date of birth.
    async promptForDob(step) {
        step.values[AGE_PROPERTY] = step.result;
        return await step.prompt(GET_DOB_PROMPT, `What is your date of birth?`);
    }

    // Collect date of birth, then prompt for favorite color.
    async promptForColor(step) {
        step.values[DOB_PROPERTY] = step.result[0].value;
        const choices = ['red', 'blue', 'green'];
        return await step.prompt(GET_COLOR_PROMPT, `Finally, what is your favorite color?`, choices);
    }

    // Collect favorite color and continue.
    async captureColor(step) {
        step.values[COLOR_PROPERTY] = step.result.value;
        return await step.next();
    }

    // With all values in hand, we can now store them in our model and complete.
    async completeProfile(step) {
        // Initialize user as a blank object into which we can store values.
        const user = {};

        // Extract collected values and add them to the user profile object.
        user[USER_NAME_PROPERTY] = step.values[USER_NAME_PROPERTY];
        user[AGE_PROPERTY] = step.values[AGE_PROPERTY];
        user[DOB_PROPERTY] = step.values[DOB_PROPERTY];
        user[COLOR_PROPERTY] = step.values[COLOR_PROPERTY];
        await this.userProfile.set(step.context, user);

        await step.context.sendActivity(`Your profile is complete! Thank you.`);

        // Transition to the display of the profile data.
        return await step.beginDialog(HELLO_USER);
    }

    async displayProfile(step) {
        const user = await this.userProfile.get(step.context, {});

        const text = [
            `You asked me to call you "${ user[USER_NAME_PROPERTY] }".`,
            `You were born on ${ moment(user[DOB_PROPERTY]).format('MMM Do, YYYY') } and claim to be ${ user[AGE_PROPERTY] }.`,
            `Your favorite color is ${ user[COLOR_PROPERTY] }.`
        ];

        await step.context.sendActivity(text.join(' '));
        return await step.endDialog();
    }
}

module.exports.OnboardingDialog = OnboardingDialog;

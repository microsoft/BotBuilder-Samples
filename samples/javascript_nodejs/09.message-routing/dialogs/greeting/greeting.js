// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ComponentDialog, WaterfallDialog, TextPrompt } = require('botbuilder-dialogs');
// User state for greeting dialog
const { GreetingState } = require('./greetingState');

// Minimum lengh requirements for city and name
const CITY_LENGTH_MIN = 5;
const NAME_LENGTH_MIN = 3;

// Dialog IDs
const PROFILE_DIALOG = 'profileDialog';

// Prompt IDs
const NAME_PROMPT = 'namePrompt';
const CITY_PROMPT = 'cityPrompt';

/**
 * Demonstrates the following concepts:
<<<<<<< HEAD
 *  Use a subclass of ComponentDialog to implement a mult-turn conversation
=======
 *  Use a subclass of ComponentDialog to implement a multi-turn conversation
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
 *  Use a Waterflow dialog to model multi-turn conversation flow
 *  Use custom prompts to validate user input
 *  Store conversation and user state
 *
 * @param {String} dialogId unique identifier for this dialog instance
 * @param {Object} greetingStateAccessor property accessor for user state
 * @param {Object} userState user state
 */
class Greeting extends ComponentDialog {
    constructor(dialogId, greetingStateAccessor, userState) {
        super(dialogId);

        // validate what was passed in
<<<<<<< HEAD
        if (!dialogId) throw ('Missing parameter.  dialogId is required');
        if (!greetingStateAccessor) throw ('Missing parameter.  greetingStateAccessor is required');
        if (!userState) throw ('Missing parameter.  userState is required');
=======
        if (!dialogId) throw new Error('Missing parameter.  dialogId is required');
        if (!greetingStateAccessor) throw new Error('Missing parameter.  greetingStateAccessor is required');
        if (!userState) throw new Error('Missing parameter.  userState is required');
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

        // Add control flow dialogs
        this.addDialog(new WaterfallDialog(PROFILE_DIALOG, [
            this.initializeStateStep.bind(this),
            this.promptForNameStep.bind(this),
            this.promptForCityStep.bind(this),
            this.displayGreetingStateStep.bind(this)
        ]));

        // Add text prompts for name and city
        this.addDialog(new TextPrompt(NAME_PROMPT, this.validateName));
        this.addDialog(new TextPrompt(CITY_PROMPT, this.validateCity));

        // Save off our state accessor for later use
        this.greetingStateAccessor = greetingStateAccessor;
        this.userState = userState;
    }
    /**
     * Waterfall Dialog step functions.
     *
     * Initialize our state.  See if the WaterfallDialog has state pass to it
     * If not, then just new up an empty GreetingState object
     *
<<<<<<< HEAD
     * @param {DialogContext} dc context for this dialog
     * @param {WaterfallStepContext} step contextual information for the current step being executed
     */
    async initializeStateStep(dc, step) {
        let greetingState = await this.greetingStateAccessor.get(dc.context);
        if (greetingState === undefined) {
            if (step.options && step.options.greetingState) {
                await this.greetingStateAccessor.set(dc.context, step.options.greetingState);
            } else {
                await this.greetingStateAccessor.set(dc.context, new GreetingState());
            }
            await this.userState.saveChanges(dc.context);
=======
     * @param {WaterfallStepContext} step contextual information for the current step being executed
     */
    async initializeStateStep(step) {
        let greetingState = await this.greetingStateAccessor.get(step.context);
        if (greetingState === undefined) {
            if (step.options && step.options.greetingState) {
                await this.greetingStateAccessor.set(step.context, step.options.greetingState);
            } else {
                await this.greetingStateAccessor.set(step.context, new GreetingState());
            }
            await this.userState.saveChanges(step.context);
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
        }
        return await step.next();
    }
    /**
     * Waterfall Dialog step functions.
     *
     * Using a text prompt, prompt the user for their name.
     * Only prompt if we don't have this information already.
     *
<<<<<<< HEAD
     * @param {DialogContext} dc context for this dialog
     * @param {WaterfallStepContext} step contextual information for the current step being executed
     */
    async promptForNameStep(dc, step) {
        const greetingState = await this.greetingStateAccessor.get(dc.context);
        // if we have everything we need, greet user and return
        if (greetingState !== undefined && greetingState.name !== undefined && greetingState.city !== undefined) {
            return await this.greetUser(dc);
        }
        if (!greetingState.name) {
            // prompt for name, if missing
            return await dc.prompt(NAME_PROMPT, 'What is your name?');
=======
     * @param {WaterfallStepContext} step contextual information for the current step being executed
     */
    async promptForNameStep(step) {
        const greetingState = await this.greetingStateAccessor.get(step.context);
        // if we have everything we need, greet user and return
        if (greetingState !== undefined && greetingState.name !== undefined && greetingState.city !== undefined) {
            return await this.greetUser(step);
        }
        if (!greetingState.name) {
            // prompt for name, if missing
            return await step.prompt(NAME_PROMPT, 'What is your name?');
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
        } else {
            return await step.next();
        }
    }
    /**
     * Waterfall Dialog step functions.
     *
     * Using a text prompt, prompt the user for the city in which they live.
     * Only prompt if we don't have this information already.
     *
<<<<<<< HEAD
     * @param {DialogContext} dc context for this dialog
     * @param {WaterfallStepContext} step contextual information for the current step being executed
     */
    async promptForCityStep(dc, step) {
        // save name, if prompted for
        const greetingState = await this.greetingStateAccessor.get(dc.context);
=======
     * @param {WaterfallStepContext} step contextual information for the current step being executed
     */
    async promptForCityStep(step) {
        // save name, if prompted for
        const greetingState = await this.greetingStateAccessor.get(step.context);
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
        if (greetingState.name === undefined && step.result) {
            let lowerCaseName = step.result;
            // capitalize and set name
            greetingState.name = lowerCaseName.charAt(0).toUpperCase() + lowerCaseName.substr(1);
<<<<<<< HEAD
            await this.greetingStateAccessor.set(dc.context, greetingState);
            await this.userState.saveChanges(dc.context);
        }
        if (!greetingState.city) {
            return await dc.prompt(CITY_PROMPT, `Hello ${greetingState.name}, what city do you live in?`);
=======
            await this.greetingStateAccessor.set(step.context, greetingState);
            await this.userState.saveChanges(step.context);
        }
        if (!greetingState.city) {
            return await step.prompt(CITY_PROMPT, `Hello ${ greetingState.name }, what city do you live in?`);
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
        } else {
            return await step.next();
        }
    }
    /**
     * Waterfall Dialog step functions.
     *
     * Having all the data we need, simply display a summary back to the user.
     *
<<<<<<< HEAD
     * @param {DialogContext} dc context for this dialog
     * @param {WaterfallStepContext} step contextual information for the current step being executed
     */
    async displayGreetingStateStep(dc, step) {
        // Save city, if prompted for
        const greetingState = await this.greetingStateAccessor.get(dc.context);
=======
     * @param {WaterfallStepContext} step contextual information for the current step being executed
     */
    async displayGreetingStateStep(step) {
        // Save city, if prompted for
        const greetingState = await this.greetingStateAccessor.get(step.context);
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
        if (greetingState.city === undefined && step.result) {
            let lowerCaseCity = step.result;
            // capitalize and set city
            greetingState.city = lowerCaseCity.charAt(0).toUpperCase() + lowerCaseCity.substr(1);
<<<<<<< HEAD
            await this.greetingStateAccessor.set(dc.context, greetingState);
            await this.userState.saveChanges(dc.context);
        }
        return await this.greetUser(dc);
    }
    /**
     * Validator function to verify that user name meets required constraints.
     * 
     * @param {DialogContext} context for this dialog
     * @param {PromptValidatorContext} prompt context for this prompt
     */
    async validateName(context, prompt) {
        // Validate that the user entered a minimum lenght for their name
        const value = (prompt.recognized.value || '').trim();
        if (value.length >= NAME_LENGTH_MIN) {
            prompt.end(value);
        } else {
            await context.sendActivity(`Names need to be at least ${NAME_LENGTH_MIN} characters long.`);
=======
            await this.greetingStateAccessor.set(step.context, greetingState);
            await this.userState.saveChanges(step.context);
        }
        return await this.greetUser(step);
    }
    /**
     * Validator function to verify that user name meets required constraints.
     *
     * @param {PromptValidatorContext} prompt context for this prompt
     */
    async validateName(prompt) {
        // Validate that the user entered a minimum length for their name
        prompt.activeDialog = await this.greetingStateAccessor;
        prompt.context = await prompt.context;
        const value = (prompt.recognized.value || '').trim();
        if (value.length >= NAME_LENGTH_MIN) {
            // Returns true if the user's name meets the required length
            return true;
        } else {
            // Returns false if the user's name fails to meet the required length
            await prompt.context.sendActivity(`Names need to be at least ${ NAME_LENGTH_MIN } characters long.`);
            return false;
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
        }
    }
    /**
     * Validator function to verify if city meets required constraints.
<<<<<<< HEAD
     * 
     * @param {DialogContext} context for this dialog
     * @param {PromptValidatorContext} prompt context for this prompt
     */
    async validateCity(context, prompt) {
        // Validate that the user entered a minimum lenght for their name
        const value = (prompt.recognized.value || '').trim();
        if (value.length >= CITY_LENGTH_MIN) {
            prompt.end(value);
        } else {
            await context.sendActivity(`City names needs to be at least ${CITY_LENGTH_MIN} characters long.`);
=======
     *
     * @param {PromptValidatorContext} prompt context for this prompt
     */
    async validateCity(prompt) {
        // Validate that the user entered a minimum length for their name
        const value = (prompt.recognized.value || '').trim();
        if (value.length >= CITY_LENGTH_MIN) {
            // Returns true if the user's city meets the required length
            return true;
        } else {
            // Returns false if the user's city fails to meet the required length
            await prompt.context.sendActivity(`City names needs to be at least ${ CITY_LENGTH_MIN } characters long.`);
            return false;
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
        }
    }
    /**
     * Helper function to greet user with information in greetingState.
<<<<<<< HEAD
     * 
     * @param {DialogContext} dc context for this dialog
     */
    async greetUser(dc) {
        const greetingState = await this.greetingStateAccessor.get(dc.context);
        // Display to the user their profile information and end dialog
        await dc.context.sendActivity(`Hi ${greetingState.name}, from ${greetingState.city}, nice to meet you!`);
        return await dc.end();
    }
}

module.exports.GreetingDialog = Greeting;
=======
     *
     * @param {WaterfallStepContext} step context for this dialog
     */
    async greetUser(step) {
        const greetingState = await this.greetingStateAccessor.get(step.context);
        // Display to the user their profile information and end dialog
        await step.context.sendActivity(`Hi ${ greetingState.name }, from ${ greetingState.city }, nice to meet you!`);
        return await step.endDialog();
    }
}

module.exports.GreetingDialog = Greeting;
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

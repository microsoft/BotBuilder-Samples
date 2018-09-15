// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
import { StatePropertyAccessor, TurnContext } from 'botbuilder';
import { ComponentDialog, WaterfallDialog, TextPrompt, DialogContext, WaterfallStepContext, PromptValidatorContext, Dialog } from 'botbuilder-dialogs';
import { GreetingState } from './greetingState';

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
 *  Use a subclass of ComponentDialog to implement a mult-turn conversation
 *  Use a Waterflow dialog to model multi-turn conversation flow
 *  Use custom prompts to validate user input
 *  Store conversation and user state
 *
 * @param {String} dialogId unique identifier for this dialog instance
 * @param {PropertyStateAccessor} greetingStateAccessor property accessor for user state
 */
export class GreetingDialog extends ComponentDialog {
    
    private greetingStateAccessor: StatePropertyAccessor<GreetingState>;

    constructor(dialogId: string, greetingStateAccessor: StatePropertyAccessor<GreetingState>) {
        super(dialogId);
    
        // validate what was passed in
        if (!dialogId) throw ('Missing parameter.  dialogId is required');
        if (!greetingStateAccessor) throw ('Missing parameter.  greetingStateAccessor is required');
    
        // Save off our state accessor for later use
        this.greetingStateAccessor = greetingStateAccessor;

        // Add control flow dialogs
        this.addDialog(new WaterfallDialog<GreetingState>(PROFILE_DIALOG, [
          this.initializeStateStep.bind(this),
          this.promptForNameStep.bind(this),
          this.promptForCityStep.bind(this),
          this.displayGreetingStateStep.bind(this)
        ]));
    
        // Add text prompts for name and city
        this.addDialog(new TextPrompt(NAME_PROMPT, this.validateName));
        this.addDialog(new TextPrompt(CITY_PROMPT, this.validateCity));
      }

    /**
     * Waterfall Dialog step functions.
     *
     * Initialize our state.  See if the WaterfallDialog has state pass to it
     * If not, then just new up an empty GreetingState object
     *
     * @param {DialogContext} dc context for this dialog
     * @param {WaterfallStepContext} step contextual information for the current step being executed
     */
    private initializeStateStep = async (dc: DialogContext, step: WaterfallStepContext<GreetingState>) => {
        const greetingState = await this.greetingStateAccessor.get(dc.context);
        if (greetingState === undefined) { 
            await this.greetingStateAccessor.set(dc.context, new GreetingState());        
        }
        return await step.next();
    }
    /**
     * Waterfall Dialog step functions.
     *
     * Using a text prompt, prompt the user for their name.
     * Only prompt if we don't have this information already.
     *
     * @param {DialogContext} dc context for this dialog
     * @param {WaterfallStepContext} step contextual information for the current step being executed
     */
    private promptForNameStep = async (dc: DialogContext, step: WaterfallStepContext<GreetingState>) => {
        const greetingState = await this.greetingStateAccessor.get(dc.context);
        // if we have everything we need, greet user and return
        if(greetingState !== undefined && greetingState.name !== undefined && greetingState.city !== undefined) {
            return await this.greetUser(dc);
        }
        if(!greetingState.name) {
            // prompt for name, if missing
            return await dc.prompt(NAME_PROMPT, 'What is your name?');
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
     * @param {DialogContext} dc context for this dialog
     * @param {WaterfallStepContext} step contextual information for the current step being executed
     */
    private promptForCityStep = async (dc: DialogContext, step: WaterfallStepContext<GreetingState>) => {
        // save name, if prompted for
        const greetingState = await this.greetingStateAccessor.get(dc.context);
        if(greetingState.name === undefined && step.result) {
            let lowerCaseName = step.result;
            // capitalize and set name
            greetingState.name = lowerCaseName.charAt(0).toUpperCase() + lowerCaseName.substr(1);
            await this.greetingStateAccessor.set(dc.context, greetingState);
        }
        if (!greetingState.city) {
            // prompt for city, if missing
            return await dc.prompt(CITY_PROMPT, `Hello ${greetingState.name}, what city do you live in?`);
        } else {
            return await step.next();
        }
    }
    /**
     * Waterfall Dialog step functions.
     *
     * Having all the data we need, simply display a summary back to the user.
     *
     * @param {DialogContext} dc context for this dialog
     * @param {WaterfallStepContext} step contextual information for the current step being executed
     */
    private displayGreetingStateStep = async (dc: DialogContext, step: WaterfallStepContext<GreetingState>) => {
        // Save city, if prompted for
        const greetingState = await this.greetingStateAccessor.get(dc.context);
        if (step.result) {
            let lowerCaseCity = step.result;
            // capitalize and set city
            greetingState.city = lowerCaseCity.charAt(0).toUpperCase() + lowerCaseCity.substr(1);
            await this.greetingStateAccessor.set(dc.context, greetingState);
        }
        return await this.greetUser(dc);
    }
    /**
     * Validator function to verify that user name meets required constraints.
     * 
     * @param {DialogContext} context for this dialog
     * @param {PromptValidatorContext} prompt context for this prompt
     */
    private validateName = async (context: TurnContext, prompt: PromptValidatorContext<String>) => {
        // Validate that the user entered a minimum lenght for their name
        const value = (prompt.recognized.value || '').trim();
        if (value.length >= NAME_LENGTH_MIN) {
            prompt.end(value);
        } else {
            await context.sendActivity(`Names need to be at least ${NAME_LENGTH_MIN} characters long.`);
        }
    }
    /**
     * Validator function to verify if city meets required constraints.
     * 
     * @param {DialogContext} context for this dialog
     * @param {PromptValidatorContext} prompt context for this prompt
     */
    private validateCity = async (context: TurnContext, prompt: PromptValidatorContext<String>) => {
        // Validate that the user entered a minimum lenght for their name
        const value = (prompt.recognized.value || '').trim();
        if (value.length >= CITY_LENGTH_MIN) {
            prompt.end(value);
        } else {
            await context.sendActivity(`City names needs to be at least ${CITY_LENGTH_MIN} characters long.`);
        }
    }
    /**
     * Helper function to greet user with information in greetingState.
     * 
     * @param {DialogContext} dc context for this dialog
     */
    async greetUser(dc: DialogContext) {
        const greetingState = await this.greetingStateAccessor.get(dc.context);
        // Display to the user their profile information and end dialog
        await dc.context.sendActivity(`Hi ${greetingState.name}, from ${greetingState.city}, nice to meet you!`);
        await dc.context.sendActivity(`You can always say 'My name is <your name> to reintroduce yourself to me.`);
        return await dc.end();
    }
}
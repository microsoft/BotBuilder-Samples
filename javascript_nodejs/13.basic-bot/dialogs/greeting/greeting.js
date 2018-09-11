// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ComponentDialog, WaterfallDialog } = require('botbuilder-dialogs');

const { NamePrompt, CityPrompt } = require('../shared/greetingPrompts');

// User state for greeting dialog
const GreetingState = require('./greetingState');
const GREETING_STATE_PROPERTY = 'greetingState';
const NAME_VALUE = 'greetingName';
const CITY_VALUE = 'greetingCity';

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
class Greeting extends ComponentDialog {
  constructor(dialogId, greetingStateAccessor) {
    super(dialogId);

    // validate what was passed in
    if (!dialogId) throw ('Missing parameter.  dialogId is required');
    if (!greetingStateAccessor) throw ('Missing parameter.  greetingStateAccessor is required');

    // Add control flow dialogs
    this.addDialog(new WaterfallDialog(PROFILE_DIALOG, [
      this.initializeStateStep,
      this.promptForNameStep,
      this.promptForCityStep,
      this.displayGreetingStateStep
    ]));

    // Add supporting prompts for name and city
    this.addDialog(new NamePrompt(NAME_PROMPT));
    this.addDialog(new CityPrompt(CITY_PROMPT));

    // Save off our state accessor for later use
    this.greetingStateAccessor = greetingStateAccessor;
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
  async initializeStateStep(dc, step) {
    // if (step.options && step.options.greetingState) {
    //   await this.greetingStateAccessor.set(dc.context, step.options.greetingState);
    // } else {
    //   await this.greetingStateAccessor.set(dc.context, new GreetingState());
    // }

    if (step.options && step.options.greetingState) {
      step.values[NAME_VALUE] = step.options.greetingState.name;
      step.values[CITY_VALUE] = step.options.greetingState.city;
    }
    return await step.next();
  }

  /**
   * Waterfall Dialog step functions.
   *
   * Using a custom prompt, prompt the user for their name.
   * Only prompt if we don't have this information already.
   *
   * @param {DialogContext} dc context for this dialog
   * @param {WaterfallStepContext} step contextual information for the current step being executed
   */
  async promptForNameStep(dc, step) {
    // prompt for name, if missing
    // const greetingState = await this.greetingStateAccessor.get(dc.context);
    // if(!greetingState.name) {
    //   return await dc.prompt(NAME_PROMPT, 'What is your name?');
    // } else {
    //   return await step.next();
    // }

    // Prompt for name if missing
    if (!step.values[NAME_VALUE]) {
      return await dc.prompt(NAME_PROMPT, `What is your name?`);
    } else {
      return await step.next();
    }
  }

  /**
   * Waterfall Dialog step functions.
   *
   * Using a custom prompt, prompt the user for the city in which they live.
   * Only prompt if we don't have this information already.
   *
   * @param {DialogContext} dc context for this dialog
   * @param {WaterfallStepContext} step contextual information for the current step being executed
   */
  async promptForCityStep(dc, step) {
    // // save name, if prompted for
    // const greetingState = await this.greetingStateAccessor.get(dc.context);
    // if (step.result) {
    //   greetingState.name = step.result;
    //   await that.greetingStateAccessor.set(dc.context, greetingState);
    // }

    // if (!greetingState.city) {
    //   return await dc.prompt(CITY_PROMPT, `${greetingState.name}, what city do you live in?`);
    // } else {
    //   return await step.next();
    // }

    // Save name if prompted for
    if (step.result) {
      step.values[NAME_VALUE] = step.result;
    }

    // Prompt for city if missing
    if (!step.values[CITY_VALUE]) {
      return await dc.prompt(CITY_PROMPT, `${step.values[NAME_VALUE]}, what city do you live in?`);
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
  async displayGreetingStateStep(dc, step) {
    // // Save city, if prompted for
    // const greetingState = await that.greetingStateAccessor.get(dc.context);
    // if (step.result) {
    //   greetingState.city = step.result;
    //   await this.greetingStateAccessor.set(dc.context, greetingState);
    // }

    // // Display to the user their profile information and end dialog
    // await dc.context.sendActivity(`${greetingState.name}, you live in ${greetingState.city}.`);
    // return await dc.end();

    // Save city if prompted for
    if (step.result) {
      step.values[CITY_VALUE] = step.result;
    }

    await dc.context.sendActivity(`Ok ${step.values[NAME_VALUE]}, I've got you living in ${step.values[CITY_VALUE]}.`);
    await dc.context.sendActivity(`I'll go ahead an update your profile with that information.`);
    return await dc.end();
  }
}

module.exports = Greeting;
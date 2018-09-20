// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, CardFactory } = require('botbuilder');
const { LuisRecognizer } = require('botbuilder-ai');
const { DialogSet, DialogTurnStatus } = require('botbuilder-dialogs');

const WelcomeCard = require('./dialogs/welcome');
const GreetingDialog = require('./dialogs/greeting');

// Greeting Dialog ID
const GREETING_DIALOG = 'greetingDialog';

// State Accessor Properties
const DIALOG_STATE_PROPERTY = 'dialogState';
const GREETING_STATE_PROPERTY = 'greetingState';

// this is the LUIS service type entry in the .bot file.
const LUIS_CONFIGURATION = '<%= botName %>-LUIS';

// Supported LUIS Intents
const GREETING_INTENT = 'Greeting';
const CANCEL_INTENT = 'Cancel';
const HELP_INTENT = 'Help';
const NONE_INTENT = 'None';

/**
 * Demonstrates the following concepts:
 *  Displaying a Welcome Card, using Adaptive Card technology
 *  Use LUIS to model Greetings, Help, and Cancel interations
 *  Use a Waterflow dialog to model multi-turn conversation flow
 *  Use custom prompts to validate user input
 *  Store conversation and user state
 *  Handle conversation interruptions
 */
class Bot {
  /**
   * Constructs the three pieces necessary for this bot to operate:
   * 1. StatePropertyAccessor for conversation state
   * 2. StatePropertyAccess for user state3
   * 3. LUIS client
   * 4. DialogSet to handle our GreetingDialog
   *
   * @param {ConversationState} conversationState property accessor
   * @param {UserState} userState property accessor
   * @param {BotConfiguration} botConfig contents of the .bot file
   */
  constructor(conversationState, userState, botConfig) {
    if (!conversationState) throw ('Missing parameter.  conversationState is required');
    if (!userState) throw ('Missing parameter.  userState is required');
    if (!botConfig) throw ('Missing parameter.  botConfig is required');

    // add the LUIS recogizer
    const luisConfig = botConfig.findServiceByNameOrId(LUIS_CONFIGURATION);
    this.luisRecognizer = new LuisRecognizer({
      applicationId: luisConfig.appId,
      azureRegion: luisConfig.region,
      // CAUTION: Its better to assign and use a subscription key instead of authoring key here.
      endpointKey: luisConfig.authoringKey
    });

    // Create the property accessors for user and conversation state
    this.greetingState = userState.createProperty(GREETING_STATE_PROPERTY);
    this.dialogState = conversationState.createProperty(DIALOG_STATE_PROPERTY);

    // Create top-level dialog(s)
    this.dialogs = new DialogSet(this.dialogState);

    this.dialogs.add(new GreetingDialog(GREETING_DIALOG, this.greetingState));
  }

  /**
   * Driver code that does one of the following:
   * 1. Display a welcome card upon startup
   * 2. Use LUIS to recognize intents
   * 3. Start a greeting dialog
   * 4. Optionally handle Cancel or Help interruptions
   *
   * @param {Context} context turn context from the adapter
   */
  async onTurn(context) {
    // Create a dialog context
    const dc = await this.dialogs.createContext(context);

    if (context.activity.type === ActivityTypes.Message) {
      // Perform a call to LUIS to retrieve results for the current activity message.
      const results = await this.luisRecognizer.recognize(context);
      const topIntent = LuisRecognizer.topIntent(results);

      // handle conversation interrupts first
      const interrupted = await this.isTurnInterrupted(dc, results);
      if (interrupted) {
        return;
      }

      // Continue the current dialog
      const dialogResult = await dc.continue();

      switch (dialogResult.status) {
        case DialogTurnStatus.empty:
          switch (topIntent) {
            case GREETING_INTENT:
              await dc.begin(GREETING_DIALOG);
              break;

            case NONE_INTENT:
            default:
              // help or no intent identified, either way, let's provide some help
              // to the user
              await dc.context.sendActivity(`I didn't understand what you just said to me.`);
              break;
          }

        case DialogTurnStatus.waiting:
          // The active dialog is waiting for a response from the user, so do nothing
          break;

        case DialogTurnStatus.complete:
          await dc.end();
          break;

        default:
          await dc.cancelAll();
          break;

      }

    } else if (context.activity.type === 'conversationUpdate' && context.activity.membersAdded[0].name === 'Bot') {
      // When activity type is "conversationUpdate" and the member joining the conversation is the bot
      // we will send our Welcome Adaptive Card.  This will only be sent once, when the Bot joins conversation
      // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
      const welcomeCard = CardFactory.adaptiveCard(WelcomeCard);
      await context.sendActivity({ attachments: [welcomeCard] });
    }
  }

  /**
   * Look at the LUIS results and determine if we need to handle
   * an interruptions due to a Help or Cancel intent
   *
   * @param {DialogContext} dc - dialog context
   * @param {LuisResults} luisResults - LUIS recognizer results
   */
  async isTurnInterrupted(dc, luisResults) {
    const intents = luisResults.intents;
    const topIntent = LuisRecognizer.topIntent(luisResults);

    // see if there are anh conversation interrupts we need to handle
    if (topIntent === CANCEL_INTENT && intents[CANCEL_INTENT].score > 0.8) {
      if (dc.activeDialog) {
        await dc.cancelAll();
        await dc.context.sendActivity(`Ok.  I've cancelled our last activity.`);
      } else {
        await dc.context.sendActivity(`I don't have anything to cancel.`);
      }
      return true;        // handled the interrupt
    }

    if (topIntent === HELP_INTENT && intents[HELP_INTENT].score > 0.8) {
      if (dc.activeDialog) {
        await dc.cancelAll();
      }
      await dc.context.sendActivity(`Let me try to provide some help.`);
      await dc.context.sendActivity(`I understand greetings, being asked for help, or being asked to cancel what I am doing.`);
      return true;        // handled the interrupt
    }
    return false;           // did not handle the interrupt
  }
}

module.exports = Bot;
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { StatePropertyAccessor, ActivityTypes, CardFactory, ConversationState, UserState, RecognizerResult, TurnContext } from 'botbuilder';
import { LuisRecognizer, LuisApplication } from 'botbuilder-ai';
import { DialogSet, DialogTurnStatus, DialogContext, DialogState, DialogTurnResult } from 'botbuilder-dialogs';
import { WelcomeCard } from './dialogs/welcome';
import { GreetingState, GreetingDialog } from './dialogs/greeting';
import { BotConfiguration, LuisService } from 'botframework-config';
// Greeting Dialog ID
const GREETING_DIALOG = 'greetingDialog';

// State Accessor Properties
const DIALOG_STATE_PROPERTY = 'dialogState';
const GREETING_STATE_PROPERTY = 'greetingState';

// this is the LUIS service type entry in the .bot file.
const LUIS_CONFIGURATION = 'basic-bot-LUIS';

// Supported LUIS Intents
const GREETING_INTENT = 'Greeting';
const CANCEL_INTENT = 'Cancel';
const HELP_INTENT = 'Help';
const NONE_INTENT = 'None';

// Supported LUIS Entities, defined in ./dialogs/greeting/resources/greeting.lu
const USER_NAME_ENTITIES = ['userName', 'userName_paternAny'];
const USER_LOCATION_ENTITIES = ['userLocation', 'userLocation_patternAny'];

/**
 * Demonstrates the following concepts:
 *  Displaying a Welcome Card, using Adaptive Card technology
 *  Use LUIS to model Greetings, Help, and Cancel interations
 *  Use a Waterflow dialog to model multi-turn conversation flow
 *  Use custom prompts to validate user input
 *  Store conversation and user state
 *  Handle conversation interruptions
 */
export class Bot {

    private greetingStateAccessor: StatePropertyAccessor<GreetingState>;
    private dialogState: StatePropertyAccessor<DialogState>;
    private luisRecognizer: LuisRecognizer;
    private readonly dialogs: DialogSet;

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
    constructor(conversationState: ConversationState, userState: UserState, botConfig: BotConfiguration) {
        if (!conversationState) throw ('Missing parameter.  conversationState is required');
        if (!userState) throw ('Missing parameter.  userState is required');
        if (!botConfig) throw ('Missing parameter.  botConfig is required');

        // add the LUIS recogizer
        let luisConfig: LuisService;
        luisConfig = <LuisService>botConfig.findServiceByNameOrId(LUIS_CONFIGURATION);
        if (!luisConfig || !luisConfig.appId) throw ('Missing LUIS configuration. Please follow README.MD to create required LUIS applications.\n\n')
        this.luisRecognizer = new LuisRecognizer({
            applicationId: luisConfig.appId,
            // CAUTION: Its better to assign and use a subscription key instead of authoring key here.
            endpointKey: luisConfig.authoringKey
        });

        // Create the property accessors for user and conversation state
        this.greetingStateAccessor = userState.createProperty(GREETING_STATE_PROPERTY);
        this.dialogState = conversationState.createProperty(DIALOG_STATE_PROPERTY);

        // Create top-level dialog(s)
        this.dialogs = new DialogSet(this.dialogState);
        this.dialogs.add(new GreetingDialog(GREETING_DIALOG, this.greetingStateAccessor));
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

    public onTurn = async (context: TurnContext) => {
        // Create a dialog context
        const dc = await this.dialogs.createContext(context);

        if (context.activity.type === ActivityTypes.Message) {
            let dialogResult: DialogTurnResult;

            // Perform a call to LUIS to retrieve results for the current activity message.
            const results = await this.luisRecognizer.recognize(context);
            const topIntent = LuisRecognizer.topIntent(results);
            
            // update greeting state with any entities captured
            await this.updateGreetingState(results, context);

            // Evaluate if we have an interruption.
            const interrupted = await this.isTurnInterrupted(dc, results);
            if (interrupted) {
                if (dc.activeDialog !== undefined) {
                    // issue a re-prompt on the active dialog
                    await dc.reprompt();
                } // Else: We dont have an active dialog so nothing to continue here.
            } else {
                // this is not an interruption. So continue any active dialogs.
                dialogResult = await dc.continue();
            }

            // if no one has responded, 
            if (!dc.context.responded) {
                // examine results from active dialog
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
    private isTurnInterrupted = async (dc: DialogContext, luisResults: RecognizerResult) => {
        const topIntent = LuisRecognizer.topIntent(luisResults);

        // see if there are anh conversation interrupts we need to handle
        if (topIntent === CANCEL_INTENT) {
            if (dc.activeDialog) {
                await dc.cancelAll();
                await dc.context.sendActivity(`Ok.  I've cancelled our last activity.`);
            } else {
                await dc.context.sendActivity(`I don't have anything to cancel.`);
            }
            return true;        // this is an interruption
        }

        if (topIntent === HELP_INTENT) {
            await dc.context.sendActivity(`Let me try to provide some help.`);
            await dc.context.sendActivity(`I understand greetings, being asked for help, or being asked to cancel what I am doing.`);
            return true;        // this is an interruption
        }
        return false;           // this is not an interruption
    }

    /**
     * Helper function to update greeting state with entities returned by LUIS.
     * 
     * @param {LuisResults} luisResults - LUIS recognizer results
     * @param {DialogContext} dc - dialog context
     */
    private updateGreetingState = async (luisResult: RecognizerResult, context: TurnContext) => {
        // Do we have any entities? 
        if(Object.keys(luisResult.entities).length !== 1) {
            // get greetingState object using the accessor
            let greetingState = await this.greetingStateAccessor.get(context); 
            if (greetingState === undefined) greetingState = new GreetingState();
            // see if we have any user name entities
            USER_NAME_ENTITIES.forEach(name => {
                if (luisResult.entities[name] !== undefined) {
                    let lowerCaseName = luisResult.entities[name][0];
                    // capitalize and set user name
                    greetingState.name = lowerCaseName.charAt(0).toUpperCase() + lowerCaseName.substr(1);
                }
            });
            USER_LOCATION_ENTITIES.forEach(city => {
                if (luisResult.entities[city] !== undefined) {
                    let lowerCaseCity = luisResult.entities[city][0];
                    // capitalize and set user name
                    greetingState.city = lowerCaseCity.charAt(0).toUpperCase() + lowerCaseCity.substr(1);
                }
            }); 
            // set the new values
            await this.greetingStateAccessor.set(context, greetingState);
        }
    }

};
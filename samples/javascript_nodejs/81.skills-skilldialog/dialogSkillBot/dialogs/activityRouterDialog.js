/* eslint-disable no-case-declarations */
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, DeliveryModes, InputHints, MessageFactory } = require('botbuilder');
const { ChoicePrompt, ComponentDialog, DialogSet, DialogTurnResult, DialogTurnStatus, WaterfallDialog } = require('botbuilder-dialogs');
const { LuisRecognizer } = require('botbuilder-ai');
const { BookingDialog } = require('../dialogs/bookingDialog');

/**
 * A root dialog that can route activities sent to the skill by different dialogs
 * TODO: add params
 */
class ActivityRouterDialog extends ComponentDialog {
    constructor(conversationState, luisRecognizer = undefined) {
        super(ActivityRouterDialog.name);

        if (!conversationState) throw new Error('[MainDialog]: Missing parameter \'conversationState\' is required');

        this.luisRecognizer = luisRecognizer;

        // Define the main dialog and its related components.
        // This is a sample "book a flight" dialog.
        this.addDialog(new BookingDialog())
            .addDialog(new OAuthTestDialog())
            .addDialog(new WaterfallDialog(WaterfallDialog.name, [
                this.processActivity.bind(this)
            ]));

        this.initialDialogId = WaterfallDialog.name;
    }

    async processActivity(stepContext) {
        // A skill can send trace activities if needed
        const traceActivity = {
            type: ActivityTypes.Trace,
            timestamp: new Date(),
            text: 'ActivityRouterDialog.processActivity()',
            label: `Got activityType: ${ stepContext.context.activity.type }`
        };
        await stepContext.context.sendActivity(traceActivity);

        switch (stepContext.context.activity.type) {
            case ActivityTypes.Message:
                return await onMessageActivity(stepContext);
            case ActivityTypes.Invoke:
                return await onInvokeActivity(stepContext);
            case ActivityTypes.Event:
                return await onEventActivity(stepContext);
            default:
                // We didn't get an activity type we can handle.
                await stepContext.context.sendActivity(
                    MessageFactory.text(
                        `Unrecognized ActivityType: "${ stepContext.context.activity.type }".`,
                        undefined,
                        InputHints.IgnoringInput
                    ));
                return new DialogTurnResult(DialogTurnStatus.complete);
        }
    }

    /**
     * This method performs different tasks based on event name
     */
    async onEventActivity(stepContext) {
        const activity = stepContext.context.activity;
        const traceActivity = {
            type: ActivityTypes.Trace,
            timestamp: new Date(),
            text: 'ActivityRouterDialog.onEventActivity()',
            label: `Name: ${ activity.name }, Value: ${ JSON.stringify(activity.value) }`
        };
        await stepContext.context.sendActivity(traceActivity);

        // Resolve what to execute based on the event name
        switch (activity.name) {
            case 'BookFlight':
                // Start the booking dialog with booking details from activity.value, if available
                return await stepContext.beginDialog(BookingDialog.name, activity.value || {});
            case 'OAuthTest':
                // Start the OAuthTestDialog
                return await stepContext.beginDialog(OAuthTestDialog.name);
            default:
                // We didn't get an event name we can handle
                await stepContext.context.sendActivity(
                    MessageFactory.text(
                        `Unrecognized EventName: "${ stepContext.context.activity.name }".`,
                        undefined,
                        InputHints.IgnoringInput
                    ));
                return new DialogTurnResult(DialogTurnStatus.complete);
        }
    }

    /**
     * This method responds right away using invokeResponse based on the activity name property
     */
    async onInvokeActivity(stepContext) {
        const activity = stepContext.context.activity;
        const traceActivity = {
            type: ActivityTypes.Trace,
            timestamp: new Date(),
            text: 'ActivityRouterDialog.onInvokeActivity()',
            label: `Name: ${ activity.name }, Value: ${ JSON.stringify(activity.value) }`
        };
        await stepContext.context.sendActivity(traceActivity);

        // Resolve what to execute based on the invoke name
        switch (activity.name) {
            case 'GetWeather':
                const lookingIntoItMessage = 'Getting your weather forecast...';
                await stepContext.context.sendActivity(MessageFactory.text(
                    `${ lookingIntoItMessage } \n\nValue parameters: ${ JSON.stringify(activity.value || {}) }`,
                    lookingIntoItMessage,
                    InputHints.IgnoringInput
                ));

                // Create and return an invoke activity with the weather results
                const invokeResponseActivity = {
                    type: 'invokeResponse',
                    value: {
                        body: [
                            'Ney York, NY, Clear, 56 F',
                            'Bellevue, WA, Mostly Cloudy, 48 F'
                        ],
                        status: 200
                    }
                };
                await stepContext.context.sendActivity(invokeResponseActivity);
                break;
            default:
                // We didn't get an invoke name we can handle
                await stepContext.context.sendActivity(
                    MessageFactory.text(
                        `Unrecognized InvokeName: "${ stepContext.context.activity.name }".`,
                        undefined,
                        InputHints.IgnoringInput
                    ));
                break;
        }
        return new DialogTurnResult(DialogTurnStatus.complete);
    }

    /**
     * This method just gets a message activity and runs it through LUIS.
     * A developer can choose to start a dialog based on the LUIS results (not implemented here). 
     */
    async onMessageActivity(stepContext) {
        const activity = stepContext.context.activity;
        const traceActivity = {
            type: ActivityTypes.Trace,
            timestamp: new Date(),
            text: 'ActivityRouterDialog.onMessageActivity()',
            label: `Text: ${ activity.text }, Value: ${ JSON.stringify(activity.value) }`
        };
        await stepContext.context.sendActivity(traceActivity);

        if (!this.luisRecognizer || !this.luisRecognizer.isConfigured) {
            await stepContext.context.sendActivity(MessageFactory.text(
                'NOTE: LUIS is not configured. TO enable all capabilities, please add \'LuisAppId\', \'LuisAPIKey\' and \'LuisAPIHostName\' to the appsettings.json file.',
                undefined,
                InputHints.IgnoringInput
            ));
        } else {
            // Call LUIS with the utterance
            const luisResult = await this.luisRecognizer.recognize(stepContext.context);
            const topIntent = LuisRecognizer.topIntent(luisResult);

            // Create a message showing the LUIS result
            let resultString = '';
            resultString += `LUIS results for "${ activity.text }":\n`;
            resultString += `Intent: "${ topIntent }", Score: ${ luisResult.intents[topIntent].score }\n`;
            resultString += `Entities found: ${ luisResult.entities.length - 1 }\n`;
            luisResult.entities.forEach((luisResultEntity) => {
                if (!luisResultEntity.instance) {
                    resultString += `\n* ${ luisResultEntity }`;
                }
            });

            await stepContext.context.sendActivity(MessageFactory.text(resultString, undefined, InputHints.IgnoringInput));
        }

        return new DialogTurnResult(DialogTurnStatus.complete);
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
}

module.exports.ActivityRouterDialog = ActivityRouterDialog;

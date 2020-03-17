// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, InputHints, MessageFactory } = require('botbuilder');
const { ComponentDialog, DialogTurnStatus, WaterfallDialog } = require('botbuilder-dialogs');
const { LuisRecognizer } = require('botbuilder-ai');
const { BookingDialog } = require('./bookingDialog');

const ACTIVITY_ROUTER_DIALOG = 'activityRouterDialog';
const WATERFALL_DIALOG = 'waterfallDialog';
const BOOKING_DIALOG = 'bookingDialog';

/**
 * A root dialog that can route activities sent to the skill to different sub-dialogs.
 */
class ActivityRouterDialog extends ComponentDialog {
    constructor(conversationState, luisRecognizer = undefined) {
        super(ACTIVITY_ROUTER_DIALOG);

        if (!conversationState) throw new Error('[MainDialog]: Missing parameter \'conversationState\' is required');

        this.luisRecognizer = luisRecognizer;

        // Define the main dialog and its related components.
        // This is a sample "book a flight" dialog.
        this.addDialog(new BookingDialog())
            .addDialog(new WaterfallDialog(WATERFALL_DIALOG, [
                this.processActivity.bind(this)
            ]));

        this.initialDialogId = WATERFALL_DIALOG;
    }

    async processActivity(stepContext) {
        // A skill can send trace activities, if needed.
        const traceActivity = {
            type: ActivityTypes.Trace,
            timestamp: new Date(),
            text: 'ActivityRouterDialog.processActivity()',
            label: `Got activityType: ${ stepContext.context.activity.type }`
        };
        await stepContext.context.sendActivity(traceActivity);

        switch (stepContext.context.activity.type) {
            case ActivityTypes.Event:
                return await this.onEventActivity(stepContext);
            case ActivityTypes.Message:
                return await this.onMessageActivity(stepContext);
            default:
                // We didn't get an activity type we can handle.
                await stepContext.context.sendActivity(
                    MessageFactory.text(
                        `Unrecognized ActivityType: "${ stepContext.context.activity.type }".`,
                        undefined,
                        InputHints.IgnoringInput
                    ));
                return { status: DialogTurnStatus.complete };
        }
    }

    /**
     * This method performs different tasks based on event name.
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

        // Resolve what to execute based on the event name.
        switch (activity.name) {
            case 'BookFlight':
                return await this.beginBookFlight(stepContext);
            case 'GetWeather':
                return await this.beginGetWeather(stepContext);
            default:
                // We didn't get an event name we can handle.
                await stepContext.context.sendActivity(
                    MessageFactory.text(
                        `Unrecognized EventName: "${ stepContext.context.activity.name }".`,
                        undefined,
                        InputHints.IgnoringInput
                    ));
                return { status: DialogTurnStatus.complete };
        }
    }

    /**
     * This method just gets a message activity and runs it through LUIS.
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
                'NOTE: LUIS is not configured. To enable all capabilities, please add \'LuisAppId\', \'LuisAPIKey\' and \'LuisAPIHostName\' to the appsettings.json file.',
                undefined,
                InputHints.IgnoringInput
            ));
        } else {
            // Call LUIS with the utterance.
            const luisResult = await this.luisRecognizer.executeLuisQuery(stepContext.context);
            const topIntent = LuisRecognizer.topIntent(luisResult);

            // Create a message showing the LUIS result.
            let resultString = '';
            resultString += `LUIS results for "${ activity.text }":\n`;
            resultString += `Intent: "${ topIntent }", Score: ${ luisResult.intents[topIntent].score }\n`;

            await stepContext.context.sendActivity(MessageFactory.text(resultString, undefined, InputHints.IgnoringInput));

            switch (topIntent.intent) {
                case 'BookFlight':
                    return await this.beginBookFlight(stepContext);
                case 'GetWeather':
                    return await this.beginGetWeather(stepContext);
                default:
                    // Catch all for unhandled intents.
                    const didntUnderstandMessageText = `Sorry, I didn't get that. Please try asking in a different way (intent was ${ topIntent.intent })`;
                    const didntUnderstandMessage = MessageFactory.text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                    await stepContext.context.sendActivity(didntUnderstandMessage);
                    break;
            }
        }

        return { status: DialogTurnStatus.complete };
    }

    async beginGetWeather(stepContext) {
        const activity = stepContext.context.activity;
        const location = activity.value || {};

        // We haven't implemented the GetWeatherDialog so we just display a TODO message.
        const getWeatherMessageText = `TODO: get weather for here (lat: ${ location.latitude }, long: ${ location.longitude })`;
        const getWeatherMessage = MessageFactory.text(getWeatherMessageText, getWeatherMessageText, InputHints.IgnoringInput);
        await stepContext.context.sendActivity(getWeatherMessage);
        return { status: DialogTurnStatus.complete };
    }

    async beginBookFlight(stepContext) {
        const activity = stepContext.context.activity;
        const bookingDetails = activity.value || {};

        // Start the booking dialog.
        const bookingDialog = this.findDialog(BOOKING_DIALOG);
        return await stepContext.beginDialog(bookingDialog.id, bookingDetails);
    }
}

module.exports.ActivityRouterDialog = ActivityRouterDialog;

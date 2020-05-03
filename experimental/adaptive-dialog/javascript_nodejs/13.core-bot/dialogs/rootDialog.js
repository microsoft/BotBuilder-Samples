// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { TimexProperty } = require('@microsoft/recognizers-text-data-types-timex-expression');
const { MessageFactory, InputHints } = require('botbuilder');
const { LuisRecognizer } = require('botbuilder-ai');

const path = require('path');
const { ComponentDialog, ListStyle } = require('botbuilder-dialogs');
const { ActivityTemplate, AdaptiveDialog, Case, ChoiceInput, ForEach, IfCondition, LuisAdaptiveRecognizer, OnConversationUpdateActivity, OnUnknownIntent, RepeatDialog, SendActivity, SwitchCondition, TemplateEngineLanguageGenerator } = require('botbuilder-dialogs-adaptive');
const { ArrayExpression, BoolExpression, EnumExpression, StringExpression } = require('adaptive-expressions');
const { Templates } = require('botbuilder-lg');

const ROOT_DIALOG = 'mainWaterfallDialog';

class RootDialog extends ComponentDialog {
    constructor() {
        super(ROOT_DIALOG);
        
        const lgFile = Templates.parseFile(path.join(__dirname, "rootDialog.lg"));
        // There are no steps associated with this dialog.
        // This dialog will react to user input using its own Recognizer's output and Rules.
        const rootDialog = new AdaptiveDialog(ROOT_DIALOG).configure({
            generator: new TemplateEngineLanguageGenerator(lgFile),
            // Add a recognizer to this adaptive dialog.
            // For this dialog, we will use the LUIS recognizer based on the FlightBooking.json
            // found under CognitiveModels folder.
            recognizer: this.createLuisRecognizer(),
            triggers: [
                new OnConversationUpdateActivity(this.welcomeUserSteps()),
            ]
        });
        
        // Add named dialogs to the DialogSet. These names are saved in the dialog state.
        this.addDialog(rootDialog);

        // The initial child Dialog to run.
        this.initialDialogId = ROOT_DIALOG;
    }

    welcomeUserSteps() {
        // Iterate through membersAdded list and greet user added to the conversation.
        return [ 
            new ForEach().configure({
                itemsProperty: new StringExpression('turn.activity.membersAdded'),
                actions: [
                    // Note: Some channels send two conversation update events - one for the Bot added to the conversation and another for user.
                    // Filter cases where the bot itself is the recipient of the message.
                    new IfCondition().configure({
                        condition: new BoolExpression('$foreach.value.name != turn.activity.recipient.name'),
                        actions: [
                            new SendActivity("${WelcomeCard()}")
                        ]
                    })
                ]
            })
        ];
    }

    createLuisRecognizer() {
        return new LuisAdaptiveRecognizer().configure(
            {
                endpoint: new StringExpression(process.env.LuisAPIHostName),
                endpointKey: new StringExpression(process.env.LuisAPIKey),
                applicationId: new StringExpression(process.env.LuisAppId)
            }
        );
    }

    // /**
    //  * First step in the waterfall dialog. Prompts the user for a command.
    //  * Currently, this expects a booking request, like "book me a flight from Paris to Berlin on march 22"
    //  * Note that the sample LUIS model will only recognize Paris, Berlin, New York and London as airport cities.
    //  */
    // async introStep(stepContext) {
    //     if (!this.luisRecognizer.isConfigured) {
    //         const messageText = 'NOTE: LUIS is not configured. To enable all capabilities, add `LuisAppId`, `LuisAPIKey` and `LuisAPIHostName` to the .env file.';
    //         await stepContext.context.sendActivity(messageText, null, InputHints.IgnoringInput);
    //         return await stepContext.next();
    //     }

    //     const messageText = stepContext.options.restartMsg ? stepContext.options.restartMsg : 'What can I help you with today?\nSay something like "Book a flight from Paris to Berlin on March 22, 2020"';
    //     const promptMessage = MessageFactory.text(messageText, messageText, InputHints.ExpectingInput);
    //     return await stepContext.prompt('TextPrompt', { prompt: promptMessage });
    // }

    // /**
    //  * Second step in the waterfall.  This will use LUIS to attempt to extract the origin, destination and travel dates.
    //  * Then, it hands off to the bookingDialog child dialog to collect any remaining details.
    //  */
    // async actStep(stepContext) {
    //     const bookingDetails = {};

    //     if (!this.luisRecognizer.isConfigured) {
    //         // LUIS is not configured, we just run the BookingDialog path.
    //         return await stepContext.beginDialog('bookingDialog', bookingDetails);
    //     }

    //     // Call LUIS and gather any potential booking details. (Note the TurnContext has the response to the prompt)
    //     const luisResult = await this.luisRecognizer.executeLuisQuery(stepContext.context);
    //     switch (LuisRecognizer.topIntent(luisResult)) {
    //     case 'BookFlight': {
    //         // Extract the values for the composite entities from the LUIS result.
    //         const fromEntities = this.luisRecognizer.getFromEntities(luisResult);
    //         const toEntities = this.luisRecognizer.getToEntities(luisResult);

    //         // Show a warning for Origin and Destination if we can't resolve them.
    //         await this.showWarningForUnsupportedCities(stepContext.context, fromEntities, toEntities);

    //         // Initialize BookingDetails with any entities we may have found in the response.
    //         bookingDetails.destination = toEntities.airport;
    //         bookingDetails.origin = fromEntities.airport;
    //         bookingDetails.travelDate = this.luisRecognizer.getTravelDate(luisResult);
    //         console.log('LUIS extracted these booking details:', JSON.stringify(bookingDetails));

    //         // Run the BookingDialog passing in whatever details we have from the LUIS call, it will fill out the remainder.
    //         return await stepContext.beginDialog('bookingDialog', bookingDetails);
    //     }

    //     case 'GetWeather': {
    //         // We haven't implemented the GetWeatherDialog so we just display a TODO message.
    //         const getWeatherMessageText = 'TODO: get weather flow here';
    //         await stepContext.context.sendActivity(getWeatherMessageText, getWeatherMessageText, InputHints.IgnoringInput);
    //         break;
    //     }

    //     default: {
    //         // Catch all for unhandled intents
    //         const didntUnderstandMessageText = `Sorry, I didn't get that. Please try asking in a different way (intent was ${ LuisRecognizer.topIntent(luisResult) })`;
    //         await stepContext.context.sendActivity(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
    //     }
    //     }

    //     return await stepContext.next();
    // }

    // /**
    //  * Shows a warning if the requested From or To cities are recognized as entities but they are not in the Airport entity list.
    //  * In some cases LUIS will recognize the From and To composite entities as a valid cities but the From and To Airport values
    //  * will be empty if those entity values can't be mapped to a canonical item in the Airport.
    //  */
    // async showWarningForUnsupportedCities(context, fromEntities, toEntities) {
    //     const unsupportedCities = [];
    //     if (fromEntities.from && !fromEntities.airport) {
    //         unsupportedCities.push(fromEntities.from);
    //     }

    //     if (toEntities.to && !toEntities.airport) {
    //         unsupportedCities.push(toEntities.to);
    //     }

    //     if (unsupportedCities.length) {
    //         const messageText = `Sorry but the following airports are not supported: ${ unsupportedCities.join(', ') }`;
    //         await context.sendActivity(messageText, messageText, InputHints.IgnoringInput);
    //     }
    // }

    // /**
    //  * This is the final step in the main waterfall dialog.
    //  * It wraps up the sample "book a flight" interaction with a simple confirmation.
    //  */
    // async finalStep(stepContext) {
    //     // If the child dialog ("bookingDialog") was cancelled or the user failed to confirm, the Result here will be null.
    //     if (stepContext.result) {
    //         const result = stepContext.result;
    //         // Now we have all the booking details.

    //         // This is where calls to the booking AOU service or database would go.

    //         // If the call to the booking service was successful tell the user.
    //         const timeProperty = new TimexProperty(result.travelDate);
    //         const travelDateMsg = timeProperty.toNaturalLanguage(new Date(Date.now()));
    //         const msg = `I have you booked to ${ result.destination } from ${ result.origin } on ${ travelDateMsg }.`;
    //         await stepContext.context.sendActivity(msg, msg, InputHints.IgnoringInput);
    //     }

    //     // Restart the main dialog with a different message the second time around
    //     return await stepContext.replaceDialog(this.initialDialogId, { restartMsg: 'What else can I do for you?' });
    // }
}

module.exports.RootDialog = RootDialog;

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { TurnContext } = require('botbuilder');
const { LuisRecognizer } = require('botbuilder-ai');

class FlightBookingRecognizer {
    constructor(config) {
        const luisIsConfigured = config && config.LuisAppId && config.LuisAPIKey && config.LuisAPIHostName;
        if (luisIsConfigured) {
            this.recognizer = new LuisRecognizer({
                applicationId: config.LuisAppId,
                endpointKey: config.LuisAPIKey,
                endpoint: `https://${ config.LuisAPIHostName }`
            }, {}, true);
        }
    }

    isConfigured() {
        return (this.recognizer !== undefined);
    }

    /**
     * Returns an object with preformatted LUIS results for the bot's dialogs to consume.
     * @param {TurnContext} context
     */
    async executeLuisQuery(context) {
        // Before seing the user's utterance to LUIS, remove any at mentions the bot may
        // have received when the user messaged the bot.
        // After sending getting the results from LUIS, set the original activity's text
        // back to its original value.
        const originalText = context.activity.text;
        TurnContext.removeRecipientMention(context.activity);

        const recognizedResults = await this.recognizer.recognize(context);

        context.activity.text = originalText;
        return recognizedResults;
    }

    getFromEntities(result) {
        let fromValue, fromAirportValue;
        if (result.entities.$instance.From) {
            fromValue = result.entities.$instance.From[0].text;
        }
        if (fromValue && result.entities.From[0].Airport) {
            fromAirportValue = result.entities.From[0].Airport[0][0];
        }

        return { from: fromValue, airport: fromAirportValue };
    }

    getToEntities(result) {
        let toValue, toAirportValue;
        if (result.entities.$instance.To) {
            toValue = result.entities.$instance.To[0].text;
        }
        if (toValue && result.entities.To[0].Airport) {
            toAirportValue = result.entities.To[0].Airport[0][0];
        }

        return { to: toValue, airport: toAirportValue };
    }

    /**
     * This value will be a TIMEX. And we are only interested in a Date so grab the first result and drop the Time part.
     * TIMEX is a format that represents DateTime expressions that include some ambiguity. e.g. missing a Year.
     */
    getTravelDate(result) {
        const datetimeEntity = result.entities['datetime'];
        if (!datetimeEntity || !datetimeEntity[0]) return undefined;

        const timex = datetimeEntity[0]['timex'];
        if (!timex || !timex[0]) return undefined;

        const datetime = timex[0].split('T')[0];
        return datetime;
    }
}

module.exports.FlightBookingRecognizer = FlightBookingRecognizer;

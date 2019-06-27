// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { BookingDetails } from './bookingDetails';

import { RecognizerResult, TurnContext } from 'botbuilder';

import { LuisRecognizer } from 'botbuilder-ai';

export class LuisHelper {
    /**
     * Returns an object with preformatted LUIS results for the bot's dialogs to consume.
     * @param {TurnContext} context
     */
    public static async executeLuisQuery(context: TurnContext): Promise<BookingDetails> {
        const bookingDetails = new BookingDetails();

        try {
            const recognizer = new LuisRecognizer({
                applicationId: process.env.LuisAppId,
                endpoint: `https://${ process.env.LuisAPIHostName }`,
                endpointKey: process.env.LuisAPIKey,
            }, {}, true);

            const recognizerResult = await recognizer.recognize(context);

            const intent = LuisRecognizer.topIntent(recognizerResult);

            bookingDetails.intent = intent;

            if (intent === 'Book_flight') {
                // We need to get the result from the LUIS JSON which at every level returns an array

                bookingDetails.destination = LuisHelper.parseCompositeEntity(recognizerResult, 'To', 'Airport');
                bookingDetails.origin = LuisHelper.parseCompositeEntity(recognizerResult, 'From', 'Airport');

                // This value will be a TIMEX. And we are only interested in a Date so grab the first result and drop the Time part.
                // TIMEX is a format that represents DateTime expressions that include some ambiguity. e.g. missing a Year.
                bookingDetails.travelDate = LuisHelper.parseDatetimeEntity(recognizerResult);
            }
        } catch (err) {
            console.warn(`LUIS Exception: ${ err } Check your LUIS configuration`);
        }
        return bookingDetails;
    }

    private static parseCompositeEntity(result: RecognizerResult, compositeName: string, entityName: string): string {
        const compositeEntity = result.entities[compositeName];
        if (!compositeEntity || !compositeEntity[0]) {
            return undefined;
        }

        const entity = compositeEntity[0][entityName];
        if (!entity || !entity[0]) {
            return undefined;
        }

        const entityValue = entity[0][0];
        return entityValue;
    }

    private static parseDatetimeEntity(result: RecognizerResult): string {
        const datetimeEntity = result.entities.datetime;
        if (!datetimeEntity || !datetimeEntity[0]) {
            return undefined;
        }

        const timex = datetimeEntity[0].timex;
        if (!timex || !timex[0]) {
            return undefined;
        }

        const datetime = timex[0].split('T')[0];
        return datetime;
    }
}

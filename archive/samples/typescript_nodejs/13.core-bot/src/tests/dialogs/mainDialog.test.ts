/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
import { TextPrompt } from 'botbuilder-dialogs';
import { DialogTestClient, DialogTestLogger } from 'botbuilder-testing';
import { BookingDialog } from '../../dialogs/bookingDialog';
import { FlightBookingRecognizer } from '../../dialogs/flightBookingRecognizer';
import { MainDialog } from '../../dialogs/mainDialog';
const assert = require('assert');
const moment = require('moment-timezone');

// tslint:disable max-classes-per-file
/**
 * A mock FlightBookingRecognizer for our main dialog tests that takes
 * a mock luis result and can set as isConfigured === false.
 */
class MockFlightBookingRecognizer extends FlightBookingRecognizer {
    private isLuisConfigured: boolean;
    constructor(isConfigured, private mockResult?: any) {
        super(isConfigured);
        this.isLuisConfigured = isConfigured;
        this.mockResult = mockResult;
    }

    public async executeLuisQuery(context) {
        return this.mockResult;
    }

    get isConfigured() {
        return (this.isLuisConfigured);
    }
}

/**
 * A simple mock for Booking dialog that just returns a preset booking info for tests.
 */
class MockBookingDialog extends BookingDialog {
    constructor() {
        super('bookingDialog');
    }

    public async beginDialog(dc, options) {
        const bookingDetails = {
            destination: 'Seattle',
            origin: 'New York',
            travelDate: '2025-07-08'
        };
        await dc.context.sendActivity(`${ this.id } mock invoked`);
        return await dc.endDialog(bookingDetails);
    }
}

/**
 * A specialized mock for BookingDialog that displays a dummy TextPrompt.
 * The dummy prompt is used to prevent the MainDialog waterfall from moving to the next step
 * and assert that the main dialog was called.
 */
class MockBookingDialogWithPrompt extends BookingDialog {
    constructor() {
        super('bookingDialog');
    }

    public async beginDialog(dc, options) {
        dc.dialogs.add(new TextPrompt('MockDialog'));
        return await dc.prompt('MockDialog', { prompt: `${ this.id } mock invoked` });
    }
}

describe('MainDialog', () => {
    it('Shows message if LUIS is not configured and calls BookingDialogDirectly', async () => {
        const mockRecognizer = new MockFlightBookingRecognizer(false);
        const mockBookingDialog = new MockBookingDialogWithPrompt();
        const sut = new MainDialog(mockRecognizer, mockBookingDialog);
        const client = new DialogTestClient('test', sut, null, [new DialogTestLogger()]);

        const reply = await client.sendActivity('hi');
        assert.strictEqual(reply.text, 'NOTE: LUIS is not configured. To enable all capabilities, add `LuisAppId`, `LuisAPIKey` and `LuisAPIHostName` to the .env file.', 'Did not warn about missing luis');
    });

    it('Shows prompt if LUIS is configured', async () => {
        const mockRecognizer = new MockFlightBookingRecognizer(true);
        const mockBookingDialog = new MockBookingDialog();
        const sut = new MainDialog(mockRecognizer, mockBookingDialog);
        const client = new DialogTestClient('test', sut, null, [new DialogTestLogger()]);

        const reply = await client.sendActivity('hi');
        const weekLaterDate = moment().add(7, 'days').format('MMMM D, YYYY');
        assert.strictEqual(reply.text, `What can I help you with today?\nSay something like "Book a flight from Paris to Berlin on ${ weekLaterDate }"`, 'Did not show prompt');
    });

    describe('Invokes tasks based on LUIS intent', () => {
        // Create array with test case data.
        const testCases = [
            { utterance: 'I want to book a flight', intent: 'BookFlight', invokedDialogResponse: 'bookingDialog mock invoked', taskConfirmationMessage: 'I have you booked to Seattle from New York' },
            { utterance: 'What\'s the weather like?', intent: 'GetWeather', invokedDialogResponse: 'TODO: get weather flow here', taskConfirmationMessage: undefined },
            { utterance: 'bananas', intent: 'None', invokedDialogResponse: 'Sorry, I didn\'t get that. Please try asking in a different way (intent was None)', taskConfirmationMessage: undefined }
        ];

        testCases.map((testData) => {
            it(testData.intent, async () => {
                // Create LuisResult for the mock recognizer.
                const mockLuisResult = JSON.parse(`{"intents": {"${ testData.intent }": {"score": 1}}, "entities": {"$instance": {}}}`);
                const mockRecognizer = new MockFlightBookingRecognizer(true, mockLuisResult);
                const bookingDialog = new MockBookingDialog();
                const sut = new MainDialog(mockRecognizer, bookingDialog);
                const client = new DialogTestClient('test', sut, null, [new DialogTestLogger()]);

                // Execute the test case
                console.log(`Test Case: ${ testData.intent }`);
                let reply = await client.sendActivity('Hi');
                const weekLaterDate = moment().add(7, 'days').format('MMMM D, YYYY');
                assert.strictEqual(reply.text, `What can I help you with today?\nSay something like "Book a flight from Paris to Berlin on ${ weekLaterDate }"`);

                reply = await client.sendActivity(testData.utterance);
                assert.strictEqual(reply.text, testData.invokedDialogResponse);

                // The Booking dialog displays an additional confirmation message, assert that it is what we expect.
                if (testData.taskConfirmationMessage) {
                    reply = client.getNextReply();
                    assert(reply.text.startsWith(testData.taskConfirmationMessage));
                }

                // Validate that the MainDialog starts over once the task is completed.
                reply = client.getNextReply();
                assert.strictEqual(reply.text, 'What else can I do for you?');
            });
        });
    });

    describe('Shows unsupported cities warning', () => {
        // Create array with test case data.
        const testCases = [
            { jsonFile: 'FlightToMadrid.json', expectedMessage: 'Sorry but the following airports are not supported: madrid' },
            { jsonFile: 'FlightFromMadridToChicago.json', expectedMessage: 'Sorry but the following airports are not supported: madrid, chicago' },
            { jsonFile: 'FlightFromCdgToJfk.json', expectedMessage: 'Sorry but the following airports are not supported: cdg' },
            { jsonFile: 'FlightFromParisToNewYork.json', expectedMessage: 'bookingDialog mock invoked' }
        ];

        testCases.map((testData) => {
            it(testData.jsonFile, async () => {
                // Create LuisResult for the mock recognizer.
                const mockLuisResult = require(`../../../testResources/${ testData.jsonFile }`);
                const mockRecognizer = new MockFlightBookingRecognizer(true, mockLuisResult);
                const bookingDialog = new MockBookingDialog();
                const sut = new MainDialog(mockRecognizer, bookingDialog);
                const client = new DialogTestClient('test', sut, null, [new DialogTestLogger()]);

                // Execute the test case
                console.log(`Test Case: ${ mockLuisResult.text }`);
                let reply = await client.sendActivity('Hi');
                const weekLaterDate = moment().add(7, 'days').format('MMMM D, YYYY');
                assert.strictEqual(reply.text, `What can I help you with today?\nSay something like "Book a flight from Paris to Berlin on ${ weekLaterDate }"`);

                reply = await client.sendActivity(mockLuisResult.text);
                assert.strictEqual(reply.text, testData.expectedMessage);
            });
        });
    });
});

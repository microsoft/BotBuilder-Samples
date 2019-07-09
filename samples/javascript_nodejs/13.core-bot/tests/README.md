
# core-bot tests <!-- omit in toc -->

Bot Framework v4 core bot tests sample.

This project uses the [botbuilder-testing](https://botbuilder.myget.org/feed/botbuilder-v4-js-daily/package/npm/botbuilder-testing) package, [mocha](https://github.com/mochajs/mocha) to create unit tests for the [core-bot](../../13.core-bot) bot.

This project shows how to:

- Create unit tests for dialogs and bots
- Create different types of data driven tests using mocha tests
- Create mock objects for the different dependencies of a dialog (i.e. LUIS recognizers, other dialogs, configuration, etc.)
- Assert the activities returned by a dialog turn against expected values
- Assert the results returned by a dialog

## Overview

In this sample, dialogs are unit tested through the `DialogTestClient` class which provides a mechanism for testing them in isolation outside of a bot and without having to deploy your code to a web service.

This class is used to write unit tests for dialogs that test their responses on a turn-by-turn basis. Any dialog built using the botbuilder dialogs library should work.

Here is a simple example on how a test that uses `DialogTestClient` looks like:

```javascript
const sut = new BookingDialog();
const testClient = new DialogTestClient(sut);

let reply = await testClient.sendActivity('hi');
assert.strictEqual(reply.text, 'Where would you like to travel to?');

reply = await testClient.sendActivity('Seattle');
assert.strictEqual(reply.text, 'Where are you traveling from?');

reply = await testClient.sendActivity('New York');
assert.strictEqual(reply.text, 'When would you like to travel?');

reply = await testClient.sendActivity('tomorrow');
assert.strictEqual(reply.text, 'OK, I will book a flight from Seattle to New York for tomorrow, Is this Correct?');

reply = await testClient.sendActivity('yes');
assert.strictEqual(reply.text, 'Sure thing, wait while I finalize your reservation...');

reply = await testClient.getNextReply();
assert.strictEqual(reply.text, 'All set, I have booked your flight to Seattle for tomorrow');
```

The project includes several examples on how to test different bot components:

- [cancelAndHelpDialog.test](dialogs/cancelAndHelpDialog.test.js) shows how to write a simple data driven test for `CancelAndHelpDialog` using a test case array.
- [bookingDialog.test](dialogs/bookingDialog.test.js) shows how to write a data driven test using a `bookingDialogTestCases` module to generate the test cases.
- [mainDialog.test](dialogs/mainDialog.test.js) showcases how to use mock objects to mock the dialog's LUIS and `BookingDialog` dependencies to test `MainDialog` in isolation.
- [dialogAndWelcomeBot.test](bots/dialogAndWelcomeBot.test.js) provides an example on how to write a test for the bot's `ActivityHandler` using `TestAdapter`.

## Further reading

- [Unit Testing Bots](https://review.docs.microsoft.com/en-us/azure/bot-service/test-bot-js?view=azure-bot-service-4.0&branch=pr-en-us-1588)
- [Mocha](https://github.com/mochajs/mocha)
- [Bot Testing](https://github.com/microsoft/botframework-sdk/blob/master/specs/testing/testing.md)

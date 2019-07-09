
# CoreBot.Tests <!-- omit in toc -->

Bot Framework v4 core bot tests sample.

This project uses the [Microsoft.Bot.Builder.Testing](http://aka.ms/bot-test-package) package, [XUnit](https://xunit.net/) and [Moq](https://github.com/moq/moq) to create unit tests for the [CoreBot](../13.core-bot) bot.

This project shows how to:

- Create unit tests for dialogs, bots and controllers
- Create different types of data driven tests using XUnit `Theory` tests
- Create mock objects for the different dependencies of a dialog (i.e. LUIS recognizers, other dialogs, configuration, etc.)
- Assert the activities returned by a dialog turn against expected values
- Assert the results returned by a dialog

## Overview

In this sample, dialogs are unit tested through the `DialogTestClient` class which provides a mechanism for testing them in isolation outside of a bot and without having to deploy your code to a web service.

This class is used to write unit tests for dialogs that test their responses on a turn-by-turn basis. Any dialog built using the botbuilder dialogs library should work.

Here is a simple example on how a test that uses `DialogTestClient` looks like:

```csharp
var sut = new BookingDialog();
var testClient = new DialogTestClient(Channels.Msteams, sut);

var reply = await testClient.SendActivityAsync<IMessageActivity>("hi");
Assert.Equal("Where would you like to travel to?", reply.Text);

reply = await testClient.SendActivityAsync<IMessageActivity>("Seattle");
Assert.Equal("Where are you traveling from?", reply.Text);

reply = await testClient.SendActivityAsync<IMessageActivity>("New York");
Assert.Equal("When would you like to travel?", reply.Text);

reply = await testClient.SendActivityAsync<IMessageActivity>("tomorrow");
Assert.Equal("OK, I will book a flight from Seattle to New York for tomorrow, Is this Correct?", reply.Text);

reply = await testClient.SendActivityAsync<IMessageActivity>("yes");
Assert.Equal("Sure thing, wait while I finalize your reservation...", reply.Text);

reply = testClient.GetNextReply<IMessageActivity>();
Assert.Equal("All set, I have booked your flight to Seattle for tomorrow", reply.Text);
```

The project includes several examples on how to test different bot components:

- [CancelAndHelpDialogTests](Dialogs/CancelAndHelpDialogTests.cs) shows how to write a simple data driven test for `CancelAndHelpDialog` using `InlineData`.
- [BookingDialogTests](Dialogs/BookingDialogTests.cs) shows how to write `Theory` tests using the `MemberData` attribute that call a helper `BookingDialogTestsDataGenerator` class to generate the test cases.
- [MainDialogTests](Dialogs/MainDialogTests.cs) showcases how to use mock objects to mock the dialog's LUIS and `BookingDialog` dependencies to test `MainDialog` in isolation.
- [DialogAndWelcomeBotTests](Bots/DialogAndWelcomeBotTests.cs) provides an example on how to write a test for the bot's `ActivityHandler` using `TestAdapter`.

## Further reading

- [Unit Testing Bots](https://review.docs.microsoft.com/en-us/azure/bot-service/test-bot?view=azure-bot-service-4.0&branch=pr-en-us-1588)
- [XUnit](https://xunit.net/)
- [Moq](https://github.com/moq/moq)
- [Bot Testing](https://github.com/microsoft/botframework-sdk/blob/master/specs/testing/testing.md)

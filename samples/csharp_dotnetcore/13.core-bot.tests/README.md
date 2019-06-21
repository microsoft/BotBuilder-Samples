
# CoreBot.Tests <!-- omit in toc -->

Bot Framework v4 core bot tests sample.

This project uses the [Microsoft.Bot.Builder.Testing](https://botbuilder.myget.org/feed/botbuilder-v4-dotnet-daily/package/nuget/Microsoft.Bot.Builder.Testing) package, [XUnit](https://xunit.net/) and [Moq](https://github.com/moq/moq) to create unit tests for the [CoreBot](https://github.com/microsoft/BotBuilder-Samples/tree/master/samples/csharp_dotnetcore/13.core-bot) bot.

This project shows how to:

- Create unit tests for dialogs, bots and controllers
- Create different types of data driven tests using XUnit Theory tests
- Create mock objects for the different dependencies of a dialog (i.e. LUIS recognizers, other dialogs, configuration, etc.)
- How to assert the activities returned by a bot turn against the expected values.
- How to assert the DialogTurnStatus returned by a bot.

## Table of Contents <!-- omit in toc -->

- [Testing Dialogs](#Testing-Dialogs)
  - [Asserting activities](#Asserting-activities)
- [Analyzing test output](#Analyzing-test-output)
- [Data Driven Tests](#Data-Driven-Tests)
  - [Data Driven Tests that take complex objects](#Data-Driven-Tests-that-take-complex-objects)
- [Using Mocks](#Using-Mocks)
  - [mocking LUIS results](#mocking-LUIS-results)
  - [mocking Dialogs](#mocking-Dialogs)
  - [mocking other objects](#mocking-other-objects)

## Testing Dialogs

Dialogs are unit tested through the `DialogTestClient` class which provides a mechanism for testing them in isolation outside of a bot and without having to deploy your code to a web service.

This class is used to write unit tests for dialogs that test their responses on a turn-by-turn basis. Any dialog built using the botbuilder dialogs library should work.

Here is an simple example on how a test that uses DialogTestClient looks like:

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
Assert.Equal("Sure thing, wait while I finalize your reservation...", reply.Text);

```

The first parameter for `DialogTestClient` is the target channel. This allows you to test different rendering logic based on the target channel for your bot (Teams, Slack, Cortana, etc.). If you are uncertain about your target channel, you can use the Emulator or Test channel ids but keep in mind that some components may behave differently depending on the current channel, for example, ConfirmPrompt renders the Yes/No options differently for the Test and Emulator channels. Your custom dialog may also have conditional logic that you may want to test.

The second parameter is the dialog being tested, **"sut"** stands for "System Under Test", we use this acronym across the tests in this sample.

The `DialogTestClient` constructor provides additional parameters that allow you to further customize the behavior of pass parameters to the dialog being tested if needed. You can pass initialization data, add custom middlewares or use your own TestAdapter and message processing callback. We use the first two parameters in several tests in this example.

The `SendActivityAsync<IActivity>` method allows you send a text utterances or an activity to your dialog and returns the first message sent by it. The `<T>` parameter is used to return a strong typed instance of the reply so you can assert it without having to cast it.

`SendActivityAsync<IActivity>` returns the first reply from the dialog, but in some scenarios your bot may send several messages in response to a single utterance, in this cases you can use the `GetNextReply<IActivity>` method to pop the next dialog message from the response queue. `GetNextReply<IActivity>` will return null if there are no further messages.

### Asserting activities

This sample only asserts the Text properties of returned activites. In more complex bots your may want to assert other properties like Speak, InputHints, ChannelData etc. You may want to do this by hand or consider using other frameworks like FluentAssertions to write custom assertions and simplify your code. 


## Analyzing test output

TODO

## Data Driven Tests

[WIP]

In most cases the bot logic is static and the different execution paths in a conversation are based on the user utterances. The dialog tests use XUnit Theory tests that 
This project uses XUnit Theory tests that allow us to create data driven tests (also known as parametrized test).

Consider the following simple test for a `DateResolverDialog`:

![Bot Framework Samples](../../../docs/media/CoreBot.Tests/DataDrivenVSExplorer.png)

### Data Driven Tests that take complex objects

[WIP]

BookingDialog receives and returns a BookingDetails object with the reservation info. The VS IDE can only expand data driven tests that use 


## Using Mocks

[WIP]

Mocks allow us to configure the dependencies of a dialog and ensure the are in a know state during the execution of the test without having to rely on external resources like databases, LUIS models or other objects.

In order to make your dialog easier to test, you may need to inject the external dependencies that your'd like to replace by mock objects in the dialog constructor so you can replace them during testing. For example, MainDialog could have instantiated BookingDialog inside the constuctor but that would have prevented us from testing MainDialog without running BookingDialog. So we added BookingDialog as a constructor parameter so we can replace it with a mock object during testing. 

These dependencies are resolved though dependency injection at runtime when the bot is running.

### mocking LUIS results

In some cases you may want to implement your LUIS results through code, for example:


But LUIS results are sometime complex, so it may be also useful to capture the desired result as a json file, add that file as an embedded resource to your project and desrialized it into a LUIS result as follows.

### mocking Dialogs

[WIP]

MainDialog invokes BookingDialog to obtain the BookingDetails object. In the MainDialogTests we want to test maindialog independently of what goes on in BookingDialog so we can mock the dependent dialog as follows:

And then we create the sut (System Under Test) using the mock booking dialog as follows:

### mocking other objects

[WIP]

This sample uses mock for other objects like Configuration (see XYZ), Logger and other objects (see controllertests) for example.

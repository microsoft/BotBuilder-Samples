# CoreBot.Tests

Bot Framework v4 core bot sample.

This project uses the [Microsoft.Bot.Builder.Testing](https://botbuilder.myget.org/feed/botbuilder-v4-dotnet-daily/package/nuget/Microsoft.Bot.Builder.Testing) package, [XUnit](https://xunit.net/) and [Moq](https://github.com/moq/moq) to create unit tests for the [CoreBot](https://github.com/microsoft/BotBuilder-Samples/tree/master/samples/csharp_dotnetcore/13.core-bot) bot.

This project shows how to:

- Create unit tests for dialogs, bots and controllers
- Create different types of data driven tests using XUnit Theory tests
- Create mock objects for the different dependencies of a dialog (i.e. LUIS recognizers, other dialogs, configuration, etc.)

## Overview

Dialogs are unit tested through the `DialogTestClient` class which provides a mechanism for testing them in isolation outside of a bot, and without having to set up a working adapter.

This class can be used to write unit tests for dialogs that test responses on a step-by-step basis.  Any dialog built with `botbuilder-dialogs` should work.

## Analyzing test output


### Data Driven Tests

In most cases the bot logic is static and the different execution paths in a conversation are based on the user utterances. The dialog tests use XUnit Theory tests that 
This project uses XUnit Theory tests that allow us to create data driven tests (also known as parametrized test).

Consider the following simple test for a `DateResolverDialog`:

![Bot Framework Samples](../../../docs/media/CoreBot.Tests/DataDrivenVSExplorer.png)

## parametrized tests that take complex objects



## About Mocks

Mocks allow us to configure the dependencies of a dialog and ensure the are in a know state during the execution of the test without having to rely on external resources like databases, LUIS models or other objects.

### mocking LUIS results

### mocking Dialogs results

### mocking other objects


### Install

First, install this library from npm:
```bash
npm install --save botbuilder-testing
```

## Unit Tests for Dialogs

`DialogTestClient` provides a mechanism for testing dialogs outside of a bot, and without having to set up a working adapter.
This class can be used to write unit tests for dialogs that test responses on a step-by-step basis.  Any dialog built with `botbuilder-dialogs` should work.

Use the DialogTestClient to drive unit tests of your dialogs.

To create a test client:
```javascript
let client = new DialogTestClient(dialog_to_test, dialog_options, OptionalMiddlewares);
```

To "send messages" through the dialog:
```javascript
let reply = await client.sendActivity('test');
```

To check for additional messages:
```javascript
reply = await client.getNextReply();
```

Here is a sample unit test using assert:

```javascript
const { DialogTestClient, DialogTestLogger } = require('botbuilder-testing');
const assert = require('assert');

let my_dialog = new SomeDialog();
let options = { ... dialog options ... };

// set up a test client with a logger middleware that logs messages in and out
let client = new DialogTestClient(my_dialog, options, [new DialogTestLogger()]);

// send a test message, catch the reply
let reply = await client.sendActivity('hello');
assert(reply.text == 'hello yourself', 'first message was wrong');
// expecting 2 messages in a row?
reply = await client.getNextReply();
assert(reply.text == 'second message', 'second message as wrong');

// test end state
assert(client.dialogTurnResult.status == 'empty', 'dialog is not empty');
```

[Additional examples are available here](tests/)

### DialogTestLogger

This additional helper class will cause the messages in your dialog to be logged to the console.
By default, the transcript will be logged with the `mocha-logger` package. You may also provide
your own logger:

```javascript
let testlogger = new DialogTestLogger(console);
```


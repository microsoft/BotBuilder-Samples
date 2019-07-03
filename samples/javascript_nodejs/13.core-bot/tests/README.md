
# core-bot tests <!-- omit in toc -->

Bot Framework v4 core bot tests sample.

This project uses the [botbuilder-testing](https://botbuilder.myget.org/feed/botbuilder-v4-dotnet-daily/package/nuget/botbuilder-testing) package, [mocha](https://github.com/mochajs/mocha) to create unit tests for the [core-bot](https://github.com/microsoft/BotBuilder-Samples/tree/master/samples/javascript_nodejs/13.core-bot) bot.

This project shows how to:

- Create unit tests for dialogs, bots and controllers
- Create different types of data driven tests using XUnit Theory tests
- Create mock objects for the different dependencies of a dialog (i.e. LUIS recognizers, other dialogs, configuration, etc.)
- Assert the activities returned by a dialog turn against expected values
- Assert the results returned by a dialog

## Table of Contents <!-- omit in toc -->

- [Testing Dialogs](#Testing-Dialogs)
  - [Asserting activities](#Asserting-activities)
  - [Passing parameters to your dialogs](#Passing-parameters-to-your-dialogs)
  - [Asserting dialog turn results](#Asserting-dialog-turn-results)
  - [Analyzing test output](#Analyzing-test-output)
- [Data Driven Tests](#Data-Driven-Tests)
  - [Theory tests using InlineData](#Theory-tests-using-InlineData)
  - [Theory tests using MemberData and complex types](#Theory-tests-using-MemberData-and-complex-types)
- [Using Mocks](#Using-Mocks)
  - [Mocking Dialogs](#Mocking-Dialogs)
  - [Mocking LUIS results](#Mocking-LUIS-results)
  - [Mock Factory](#Mock-Factory)
- [References](#References)

## Testing Dialogs

Dialogs are unit tested through the `DialogTestClient` class which provides a mechanism for testing them in isolation outside of a bot and without having to deploy your code to a web service.

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

**TODO: review if we can add a required channel parameter to DialogTestClient, if not, word this section appropriately.**
**TODO: are there constants for ChannleIds in JS? **

The first parameter for `DialogTestClient` is the target channel. This allows you to test different rendering logic based on the target channel for your bot (Teams, Slack, Cortana, etc.). If you are uncertain about your target channel, you can use the Emulator or Test channel ids but keep in mind that some components may behave differently depending on the current channel, for example, ConfirmPrompt renders the Yes/No options differently for the Test and Emulator channels. You can also use this parameter to test conditional rendering logic in your dialog based on the channel ID.

The second parameter is an instance of the dialog being tested (Note: **"sut"** stands for "System Under Test", we use this acronym across the tests in this sample).

The `DialogTestClient` constructor provides additional parameters that allow you to further customize the client behavior or pass parameters to the dialog being tested if needed. You can pass initialization data, add custom middlewares, use your own `TestAdapter` or pass a `ConversationState` instance. We use the first two parameters in several tests in this example.

The `sendActivity` method allows you send a text utterance or an `Activity` to your dialog and returns the first message it receives.

`sendActivity` returns the first reply from the dialog, but in some scenarios your bot may send several messages in response to a single utterance, in this cases `DialogTestClient` will queue the replies and you can use the `getNextReply` method to pop the next message from the response queue.

```javascript
reply = await testClient.getNextReply();
assert.strictEqual(reply.text, 'All set, I have booked your flight to Seattle for tomorrow');
```

`getNextReply` returns null if there are no further messages.

### Asserting activities

The code in this project only asserts the `text` property of the returned activities. In more complex bots your may want to assert other properties like `speak`, `inputHint`, `channelData`, etc.

```javascript
assert.strictEqual(reply.text, 'Sure thing, wait while I finalize your reservation...');
assert.strictEqual(reply.speak, 'One moment please...');
assert.strictEqual(reply.inputHint, InputHints.IgnoringInput);
```

You can do this by checking each property individually as shown above, you can write your own helper utilities for asserting activities or you can use other assertion libraries to write custom assertions and simplify your test code.

### Passing parameters to your dialogs

The `DialogTestClient` constructor has an `initialDialogOptions` parameter that can be used to pass parameters to your dialog. For example, the `MainDialog` in this sample, initializes a `BookingDetails` object from the LUIS results with the entities it could resolve from the user's utterance and passes this object in the `BeginDialogAsync` call to invoke `BookingDialog`.

You can implement this in a test as follows:

```javascript
const inputDialogParams = {
    destination: 'Seattle',
    travelDate: formatDate(new Date().setDate(now.getDate() + 1))
};

const sut = new BookingDialog();
const testClient = new DialogTestClient(Channels.Msteams, sut, inputDialogParams);
```

`BookingDialog` will receive this parameter and can access it in the test the same way as if it would have been invoked from `MainDialog`.

```javascript
async destinationStep(stepContext) {
    const bookingDetails = stepContext.options;
    ...
}
```

### Asserting dialog turn results

Some dialogs like `BookingDialog` or `DateResolverDialog` return a value to the calling dialog. The `DialogTextClient` object exposes a `DialogTurnResult` property that can be used to analyze and assert the results returned by the dialog.

For example:

```javascript
const sut = new BookingDialog();
const testClient = new DialogTestClient(Channels.Msteams, sut);

let reply = await testClient.sendActivity('hi');
assert.strictEqual(reply.text, 'Where would you like to travel to?');

...

const bookingResults = (BookingDetails)testClient.DialogTurnResult.Result;
Assert.Equal('New York', bookingResults?.Origin);
Assert.Equal('Seattle', bookingResults?.Destination);
Assert.Equal('2019-06-21', bookingResults?.TravelDate);
```

The `DialogTurnResult` can also be used to inspect and assert intermediate results returned by the steps in a waterfall.

### Analyzing test output

Sometimes it is necessary to read a unit test transcript so we can analyze the test execution without having to debug the test.
The [botbuilder-testing](https://botbuilder.myget.org/feed/botbuilder-v4-dotnet-daily/package/nuget/botbuilder-testing) package includes an `XUnitOutputMiddleware` that logs the messages sent and received by the dialog to the console.

To use this middleware, your test needs to expose a constructor that receives an `ITestOutputHelper` object that is provided by the XUnit test runner and create a `XUnitOutputMiddleware` that will be passed to `DialogTestClient` trough the `middlewares` parameter.

```javascript
public class BookingDialogTests
{
    private readonly XUnitOutputMiddleware[] _middlewares;

    public BookingDialogTests(ITestOutputHelper output)
        : base(output)
    {
        _middlewares = new[] { new XUnitOutputMiddleware(output) };
    }

    [Fact]
    public async Task SomeBookingDialogTest()
    {
        // Arrange
        const sut = new BookingDialog();
        const testClient = new DialogTestClient(Channels.Msteams, sut, middlewares: _middlewares);

        ...
    }
}
```

Here is an example of what the `XUnitOutputMiddleware` logs to the output window when is configured:

![XUnitMiddlewareOutput](../../../docs/media/CoreBot.Tests/javascript/XUnitMiddlewareOutput.png)

This output will be also logged on the build server during the CI builds and helps analyze build failures.

For additional information on sending test output to the console when using XUnit see [Capturing Output](https://xunit.net/docs/capturing-output.html) in the XUnit documentation.

## Data Driven Tests

In most cases the dialog logic is static and the different execution paths in a conversation are based on the user utterances. Rather than writing a single unit test for each variant in the conversation it is easier to use data driven tests (also known as parameterized test).

For example, the sample test in the overview section of this documents shows how to test one execution flow, but what happens if the user says no to the confirmation? what if they use a different date?, etc.

Data driven tests allow us to test all these permutations without having to rewrite the tests.

In this project, we use `Theory` tests from XUnit to parameterize tests.

### Theory tests using InlineData

The following test checks that a dialog gets cancelled when the user says 'cancel'.

```javascript
[Fact]
public async Task ShouldBeAbleToCancel()
{
    const sut = new TestCancelAndHelpDialog();
    const testClient = new DialogTestClient(Channels.Test, sut);

    var reply = await testClient.sendActivity('Hi');
    Assert.Equal('Hi there', reply.text);
    Assert.Equal(DialogTurnStatus.Waiting, testClient.DialogTurnResult.Status);

    reply = await testClient.sendActivity('cancel');
    Assert.Equal('Cancelling...', reply.text);
    Assert.Equal(DialogTurnStatus.Cancelled, testClient.DialogTurnResult.Status);
}
```

Consider that later on we need to be able to handle other utterances for cancel like 'quit', 'never mind' and 'stop it'. Rather than writing 3 more repetitive tests for each new utterance, we can refactor the test as a `Theory` test that uses `InlineData` to define the parameters for each test case:

```javascript
[Theory]
[InlineData('hi', 'Hi there', 'cancel')]
[InlineData('hi', 'Hi there', 'quit')]
[InlineData('hi', 'Hi there', 'never mind')]
[InlineData('hi', 'Hi there', 'stop it')]
public async Task ShouldBeAbleToCancel(string utterance, string response, string cancelUtterance)
{
    const sut = new TestCancelAndHelpDialog();
    const testClient = new DialogTestClient(Channels.Test, sut, middlewares: _middlewares);

    var reply = await testClient.sendActivity(utterance);
    Assert.Equal(response, reply.text);
    Assert.Equal(DialogTurnStatus.Waiting, testClient.DialogTurnResult.Status);

    reply = await testClient.sendActivity(cancelUtterance);
    Assert.Equal('Cancelling...', reply.text);
    Assert.Equal(DialogTurnStatus.Cancelled, testClient.DialogTurnResult.Status);
}
```

The new test will be executed 4 times with the different parameters and each case will show as a child item under the `ShouldBeAbleToCancel` test in [Mocha Test Explorer](https://marketplace.visualstudio.com/items?itemName=hbenl.vscode-mocha-test-adapter&ssr=false#overview) plugin for VSCode. If any of them fail like shown below, the developer can right click and debug the scenario that failed rather than re-running the entire set of tests.

![Bot Framework Samples](../../../docs/media/CoreBot.Tests/javascript/InlineDataTestResults.png)

### Theory tests using MemberData and complex types

`InlineData` is useful for small data driven tests that receive simple value type parameters (string, int, etc.).

The `BookingDialog` receives a `BookingDetails` object and returns a new `BookingDetails` object. A non parameterized version of a test for this dialog would look as follows:

```javascript
[Fact]
public async Task DialogFlow()
{
    // Initial parameters
    var initialBookingDetails = new BookingDetails
    {
        Origin = 'Seattle',
        Destination = null,
        TravelDate = null,
    };

    // Expected booking details
    var expectedBookingDetails = new BookingDetails
    {
        Origin = 'Seattle',
        Destination = 'New York',
        TravelDate = '2019-06-25',
    };

    const sut = new BookingDialog();
    const testClient = new DialogTestClient(Channels.Test, sut, initialBookingDetails);

    // Act/Assert
    var reply = await testClient.sendActivity('hi');
    ...

    var bookingResults = (BookingDetails)testClient.DialogTurnResult.Result;
    Assert.Equal(expectedBookingDetails.Origin, bookingResults?.Origin);
    Assert.Equal(expectedBookingDetails.Destination, bookingResults?.Destination);
    Assert.Equal(expectedBookingDetails.TravelDate, bookingResults?.TravelDate);
}
```

To parameterize this test, we created a `BookingDialogTestCase` class that contains our test case data: the initial `BookingDetails` object, the expected `BookingDialogTestCase` and an array of strings containing the utterances sent from the user and the expected replies from the dialog.

```javascript
public class BookingDialogTestCase
{
    public BookingDetails InitialBookingDetails { get; set; }

    public string[,] UtterancesAndReplies { get; set; }

    public BookingDetails ExpectedBookingDetails { get; set; }
}
```

We also created a helper `BookingDialogTestsDataGenerator` class that exposes a `IEnumerable<object[]> BookingFlows()` method that returns a collection of the test cases to be used by the test.

In order to display each test case as a separate test in VS Test Explorer, the test runner requires that complex types like `BookingDialogTestCase` implement `IXunitSerializable`, to simplify this, the `BotBuilder.Testing` framework provides a `TestDataObject` class that Implements this interface and can be used to wrap the test case data without having to implement `IXunitSerializable`. Here is a fragment of `IEnumerable<object[]> BookingFlows()` that shows how the two classes are used:

```javascript
public static class BookingDialogTestsDataGenerator
{
    public static IEnumerable<object[]> BookingFlows()
    {
        // Create the first test case object
        var testCaseData = new BookingDialogTestCase
        {
            InitialBookingDetails = new BookingDetails(),
            UtterancesAndReplies = new[,]
            {
                { 'hi', 'Where would you like to travel to?' },
                { 'Seattle', 'Where are you traveling from?' },
                { 'New York', 'When would you like to travel?' },
                { 'tomorrow', $'Please confirm, I have you traveling to: Seattle from: New York on: {DateTime.Now.AddDays(1):yyyy-MM-dd}. Is this correct? (1) Yes or (2) No' },
                { 'yes', null },
            },
            ExpectedBookingDetails = new BookingDetails
            {
                Destination = 'Seattle',
                Origin = 'New York',
                TravelDate = $'{DateTime.Now.AddDays(1):yyyy-MM-dd}',
            }, 
        };
        // wrap the test case object into TestDataObject and return it.
        yield return new object[] { new TestDataObject(testCaseData) };

        // Create the second test case object
        testCaseData = new BookingDialogTestCase
        {
            InitialBookingDetails = new BookingDetails
            {
                Destination = 'Seattle',
                Origin = 'New York',
                TravelDate = null,
            },
            UtterancesAndReplies = new[,]
            {
                { 'hi', 'When would you like to travel?' },
                { 'tomorrow', $'Please confirm, I have you traveling to: Seattle from: New York on: {DateTime.Now.AddDays(1):yyyy-MM-dd}. Is this correct? (1) Yes or (2) No' },
                { 'yes', null },
            },
            ExpectedBookingDetails = new BookingDetails
            {
                Destination = 'Seattle',
                Origin = 'New York',
                TravelDate = $'{DateTime.Now.AddDays(1):yyyy-MM-dd}',
            }, 
        };
        // wrap the test case object into TestDataObject and return it.
        yield return new object[] { new TestDataObject(testCaseData) };
    }
}
```

Once we created an object to store the test data and a class that exposes a collection of test cases, we use the XUnit `MemberData` attribute instead of `InlineData` to feed the data into the test, the first parameter for `MemberData` is the name of the static function that returns the collection of test cases and the second attribute is the type of the class that exposes this method.

```javascript
[Theory]
[MemberData(nameof(BookingDialogTestsDataGenerator.BookingFlows), MemberType = typeof(BookingDialogTestsDataGenerator))]
public async Task DialogFlowUseCases(TestDataObject testData)
{
    // Get the test data instance from TestDataObject
    var bookingTestData = testData.GetObject<BookingDialogTestCase>();
    const sut = new BookingDialog();
    const testClient = new DialogTestClient(Channels.Test, sut, bookingTestData.InitialBookingDetails);

    // Iterate over the utterances and replies array.
    for (var i = 0; i < bookingTestData.UtterancesAndReplies.GetLength(0); i++)
    {
        var reply = await testClient.sendActivity(bookingTestData.UtterancesAndReplies[i, 0]);
        Assert.Equal(bookingTestData.UtterancesAndReplies[i, 1], reply?.Text);
    }

    // Assert the resulting BookingDetails object
    var bookingResults = (BookingDetails)testClient.DialogTurnResult.Result;
    Assert.Equal(bookingTestData.ExpectedBookingDetails?.Origin, bookingResults?.Origin);
    Assert.Equal(bookingTestData.ExpectedBookingDetails?.Destination, bookingResults?.Destination);
    Assert.Equal(bookingTestData.ExpectedBookingDetails?.TravelDate, bookingResults?.TravelDate);
}
```

Here is an example of the results for `DialogFlowUseCases` in Visual Studio Test Explorer when the test is executed:

![BookingDialogTests](../../../docs/media/CoreBot.Tests/javascript/BookingDialogTestsResults.png)

## Using Mocks

Mocks allow us to configure the dependencies of a dialog and ensure they are in a known state during the execution of the test without having to rely on external resources like databases, LUIS models or other objects.

In order to make your dialog easier to test and reduce its dependencies on external objects, you may need to inject the external dependencies in the dialog constructor. 

For example, instead of instantiating `BookingDialog` in `MainDialog`:

```javascript
public MainDialog()
    : base(nameof(MainDialog))
{
    ...
    AddDialog(new BookingDialog());
    ...
}
```

We pass an instance of `BookingDialog` as a constructor parameter

```javascript
public MainDialog(BookingDialog bookingDialog)
    : base(nameof(MainDialog))
{
    ...
    AddDialog(bookingDialog);
    ...
}
```

This allow us to write tests for `MainDialog` that use a mock instance of `BookingDialog`:

```javascript
// Create the mock object
var mockDialog = new Mock<BookingDialog>();

// Use the mock object to instantiate MainDialog
const sut = new MainDialog(mockDialog.Object);

const testClient = new DialogTestClient(Channels.Test, sut);
```

In this example, we use [Moq](https://github.com/moq/moq) to create mock objects and the `Setup` and `Returns` methods to configure their behavior.

### Mocking Dialogs

As described above, `MainDialog` invokes `BookingDialog` to obtain the `BookingDetails` object. We implement and configure a mock instance of `BookingDialog` as follows:

```javascript
// Create the BookingDetails instance we want the mock object to return.
var expectedBookingDialogResult = new BookingDetails()
{
    Destination = 'Seattle',
    Origin = 'New York',
    TravelDate = $'{DateTime.UtcNow.AddDays(1):yyyy-MM-dd}'
};

// Create the mock object for BookingDialog.
var mockDialog = new Mock<BookingDialog>();
mockDialog
    .Setup(x => x.BeginDialogAsync(It.IsAny<DialogContext>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
    .Returns(async (DialogContext dialogContext, object options, CancellationToken cancellationToken) =>
    {
        // Send a generic activity so we can assert that the dialog was invoked.
        await dialogContext.Context.SendActivityAsync($'{mockDialogNameTypeName} mock invoked', cancellationToken: cancellationToken);

        // Return the BookingDetails we need without executing the dialog logic.
        return await dialogContext.EndDialogAsync(expectedBookingDialogResult, cancellationToken);
    });

// Create the sut (System Under Test) using the mock booking dialog.
const sut = new MainDialog(mockDialog.Object);
```

### Mocking LUIS results

In simple scenarios, you can implement mock LUIS results through code as follows:

```javascript
var mockRecognizer = new Mock<IRecognizer>();
mockRecognizer
    .Setup(x => x.RecognizeAsync<FlightBooking>(It.IsAny<ITurnContext>(), It.IsAny<CancellationToken>()))
    .Returns(() =>
    {
        var luisResult = new FlightBooking
        {
            Intents = new Dictionary<FlightBooking.Intent, IntentScore>
            {
                { FlightBooking.Intent.BookFlight, new IntentScore() { Score = 1 } },
            },
            Entities = new FlightBooking._Entities(),
        };
        return Task.FromResult(luisResult);
    });
```

But LUIS results are sometimes complex, in these situations, it is simpler to capture the desired result as a json file, add it as an embedded resource to your project and deserialize it into a LUIS result. Here is an example:

```javascript
var mockRecognizer = new Mock<IRecognizer>();
mockRecognizer
    .Setup(x => x.RecognizeAsync<FlightBooking>(It.IsAny<ITurnContext>(), It.IsAny<CancellationToken>()))
    .Returns(() =>
    {
        // Deserialize the LUIS result from embedded json file in the TestData folder.
        var bookingResult = GetEmbeddedTestData($'{GetType().Namespace}.TestData.FlightToMadrid.json');

        // Return the deserialized LUIS result.
        return Task.FromResult(bookingResult);
    });
```

### Mock Factory

This example includes a helper `SimpleMockFactory` class that simplifies the creation of commonly used mocks and helps declutter the testing code.

However, you may still need to create and configure specific mock objects that better serve the test case in some specific scenarios.

## References

- [Mocha](https://github.com/mochajs/mocha)
- [Bot Testing](https://github.com/microsoft/botframework-sdk/blob/master/specs/testing/testing.md)

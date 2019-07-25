// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Tests.Common;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Testing;
using Microsoft.Bot.Builder.Testing.XUnit;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

using CoreBot.CognitiveModels;
using CoreBot.Dialogs;

namespace CoreBot.Tests.Dialogs
{
    public class MainDialogTests : BotTestBase
    {
        private readonly BookingDialog _mockBookingDialog;
        private readonly Mock<ILogger<MainDialog>> _mockLogger;

        public MainDialogTests(ITestOutputHelper output)
            : base(output)
        {
            _mockLogger = new Mock<ILogger<MainDialog>>();
            var expectedBookingDialogResult = new BookingDetails()
            {
                Destination = "Seattle",
                Origin = "New York",
                TravelDate = $"{DateTime.UtcNow.AddDays(1):yyyy-MM-dd}"
            };
            _mockBookingDialog = SimpleMockFactory.CreateMockDialog<BookingDialog>(expectedBookingDialogResult).Object;
        }

        [Fact]
        public async Task ShowsMessageIfLuisNotConfiguredAndCallsBookDialogDirectly()
        {
            // Arrange
            var mockRecognizer = SimpleMockFactory.CreateMockLuisRecognizer<FlightBookingRecognizer>(null, constructorParams: new Mock<IConfiguration>().Object);
            mockRecognizer.Setup(x => x.IsConfigured).Returns(false);

            // Create a specialized mock for BookingDialog that displays a dummy TextPrompt.
            // The dummy prompt is used to prevent the MainDialog waterfall from moving to the next step
            // and assert that the dialog was called.
            var mockDialog = new Mock<BookingDialog>();
            mockDialog
                .Setup(x => x.BeginDialogAsync(It.IsAny<DialogContext>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns(async (DialogContext dialogContext, object options, CancellationToken cancellationToken) =>
                {
                    dialogContext.Dialogs.Add(new TextPrompt("MockDialog"));
                    return await dialogContext.PromptAsync("MockDialog", new PromptOptions() { Prompt = MessageFactory.Text($"{nameof(BookingDialog)} mock invoked") }, cancellationToken);
                });

            var sut = new MainDialog(mockRecognizer.Object, mockDialog.Object, _mockLogger.Object);
            var testClient = new DialogTestClient(Channels.Test, sut, middlewares: new[] { new XUnitDialogTestLogger(Output) });

            // Act/Assert
            var reply = await testClient.SendActivityAsync<IMessageActivity>("hi");
            Assert.Equal("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.", reply.Text);

            reply = testClient.GetNextReply<IMessageActivity>();
            Assert.Equal("BookingDialog mock invoked", reply.Text);
        }

        [Fact]
        public async Task ShowsPromptIfLuisIsConfigured()
        {
            // Arrange
            var mockRecognizer = SimpleMockFactory.CreateMockLuisRecognizer<FlightBookingRecognizer>(null, constructorParams: new Mock<IConfiguration>().Object);
            mockRecognizer.Setup(x => x.IsConfigured).Returns(true);
            var sut = new MainDialog(mockRecognizer.Object, _mockBookingDialog, _mockLogger.Object);
            var testClient = new DialogTestClient(Channels.Test, sut, middlewares: new[] { new XUnitDialogTestLogger(Output) });

            // Act/Assert
            var reply = await testClient.SendActivityAsync<IMessageActivity>("hi");
            Assert.Equal("What can I help you with today?\nSay something like \"Book a flight from Paris to Berlin on March 22, 2020\"", reply.Text);
         }

        [Theory]
        [InlineData("I want to book a flight", "BookFlight", "BookingDialog mock invoked", "I have you booked to Seattle from New York")]
        [InlineData("What's the weather like?", "GetWeather", "TODO: get weather flow here", null)]
        [InlineData("bananas", "None", "Sorry, I didn't get that. Please try asking in a different way (intent was None)", null)]
        public async Task TaskSelector(string utterance, string intent, string invokedDialogResponse, string taskConfirmationMessage)
        {
            // Create a mock recognizer that returns the expected intent.
            var mockLuisRecognizer = SimpleMockFactory.CreateMockLuisRecognizer<FlightBookingRecognizer, FlightBooking>(
                new FlightBooking
                {
                    Intents = new Dictionary<FlightBooking.Intent, IntentScore>
                    {
                        { Enum.Parse<FlightBooking.Intent>(intent), new IntentScore() { Score = 1 } },
                    },
                    Entities = new FlightBooking._Entities(),
                },
                new Mock<IConfiguration>().Object);
            mockLuisRecognizer.Setup(x => x.IsConfigured).Returns(true);
            var sut = new MainDialog(mockLuisRecognizer.Object, _mockBookingDialog, _mockLogger.Object);
            var testClient = new DialogTestClient(Channels.Test, sut, middlewares: new[] { new XUnitDialogTestLogger(Output) });

            // Execute the test case
            Output.WriteLine($"Test Case: {intent}");
            var reply = await testClient.SendActivityAsync<IMessageActivity>("hi");
            Assert.Equal("What can I help you with today?\nSay something like \"Book a flight from Paris to Berlin on March 22, 2020\"", reply.Text);

            reply = await testClient.SendActivityAsync<IMessageActivity>(utterance);
            Assert.Equal(invokedDialogResponse, reply.Text);

            // The Booking dialog displays an additional confirmation message, assert that it is what we expect.
            if (!string.IsNullOrEmpty(taskConfirmationMessage))
            {
                reply = testClient.GetNextReply<IMessageActivity>();
                Assert.StartsWith(taskConfirmationMessage, reply.Text);
            }

            // Validate that the MainDialog starts over once the task is completed.
            reply = testClient.GetNextReply<IMessageActivity>();
            Assert.Equal("What else can I do for you?", reply.Text);
        }

        [Theory]
        [InlineData("FlightToMadrid.json", "Sorry but the following airports are not supported: madrid")]
        [InlineData("FlightFromMadridToChicago.json", "Sorry but the following airports are not supported: madrid,chicago")]
        [InlineData("FlightFromCdgToJfk.json", "Sorry but the following airports are not supported: cdg")]
        [InlineData("FlightFromParisToNewYork.json", "BookingDialog mock invoked")]
        public async Task ShowsUnsupportedCitiesWarning(string jsonFile, string expectedMessage)
        {
            // Load the LUIS result json and create a mock recognizer that returns the expected result.
            var luisResultJson = GetEmbeddedTestData($"{GetType().Namespace}.TestData.{jsonFile}");
            var mockLuisResult = JsonConvert.DeserializeObject<FlightBooking>(luisResultJson);
            var mockLuisRecognizer = SimpleMockFactory.CreateMockLuisRecognizer<FlightBookingRecognizer, FlightBooking>(
                mockLuisResult,
                new Mock<IConfiguration>().Object);
            mockLuisRecognizer.Setup(x => x.IsConfigured).Returns(true);

            var sut = new MainDialog(mockLuisRecognizer.Object, _mockBookingDialog, _mockLogger.Object);
            var testClient = new DialogTestClient(Channels.Test, sut, middlewares: new[] { new XUnitDialogTestLogger(Output) });

            // Execute the test case
            Output.WriteLine($"Test Case: {mockLuisResult.Text}");
            var reply = await testClient.SendActivityAsync<IMessageActivity>("hi");
            Assert.Equal("What can I help you with today?\nSay something like \"Book a flight from Paris to Berlin on March 22, 2020\"", reply.Text);

            reply = await testClient.SendActivityAsync<IMessageActivity>(mockLuisResult.Text);
            Assert.Equal(expectedMessage, reply.Text);
        }

        /// <summary>
        /// Loads the embedded json resource with the LUIS as a string.
        /// </summary>
        private string GetEmbeddedTestData(string resourceName)
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}

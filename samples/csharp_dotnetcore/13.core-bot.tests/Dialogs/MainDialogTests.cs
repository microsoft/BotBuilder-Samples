// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Testing;
using Microsoft.Bot.Builder.Testing.XUnit;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Microsoft.BotBuilderSamples.Dialogs;
using Microsoft.BotBuilderSamples.Tests.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;

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
        public void DialogConstructor()
        {
            var sut = new MainDialog(null, _mockBookingDialog, _mockLogger.Object);

            Assert.Equal("MainDialog", sut.Id);
            Assert.IsType<TextPrompt>(sut.FindDialog("TextPrompt"));
            Assert.NotNull(sut.FindDialog("BookingDialog"));
            Assert.IsType<WaterfallDialog>(sut.FindDialog("WaterfallDialog"));
        }

        [Fact]
        public async Task ShowsMessageIfLuisNotConfiguredAndCallsBookDialogDirectly()
        {
            // Arrange
            var mockRecognizer = SimpleMockFactory.CreateMockLuisRecognizer<FlightBookingRecognizer>(null, constructorParams: new Mock<IConfiguration>().Object);
            mockRecognizer.Setup(x => x.IsConfigured).Returns(false);

            // Create a specialized mock for BookingDialog that displays a dummy TextPrompt.
            // The dummy prompt is used to prevent the MainDialog waterfall from moving to the next step
            // and assert the dialog was called.
            var mockDialog = new Mock<BookingDialog>();
            mockDialog
                .Setup(x => x.BeginDialogAsync(It.IsAny<DialogContext>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns(async (DialogContext dialogContext, object options, CancellationToken cancellationToken) =>
                {
                    dialogContext.Dialogs.Add(new TextPrompt("MockDialog"));
                    return await dialogContext.PromptAsync("MockDialog", new PromptOptions(){Prompt = MessageFactory.Text($"{nameof(BookingDialog)} mock invoked")}, cancellationToken);
                });

            var sut = new MainDialog(mockRecognizer.Object, mockDialog.Object, _mockLogger.Object);
            var testClient = new DialogTestClient(Channels.Test, sut, middlewares: new[] { new XUnitOutputMiddleware(Output) });

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
            var testClient = new DialogTestClient(Channels.Test, sut, middlewares: new[] { new XUnitOutputMiddleware(Output) });

            // Act/Assert
            var reply = await testClient.SendActivityAsync<IMessageActivity>("hi");
            Assert.Equal("What can I help you with today?", reply.Text);
        }

        [Theory]
        [InlineData("I want to book a flight", "BookFlight", "BookingDialog mock invoked", "I have you booked to Seattle from New York")]
        [InlineData("What's the weather like?", "GetWeather", "TODO: get weather flow here", null)]
        [InlineData("bananas", "None", "Sorry, I didn't get that. Please try asking in a different way (intent was None)", null)]
        public async Task TaskSelector(string utterance, string intent, string invokedDialogResponse, string taskConfirmationMessage)
        {
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
            var testClient = new DialogTestClient(Channels.Test, sut, middlewares: new[] { new XUnitOutputMiddleware(Output) });

            var reply = await testClient.SendActivityAsync<IMessageActivity>("hi");
            Assert.Equal("What can I help you with today?", reply.Text);

            reply = await testClient.SendActivityAsync<IMessageActivity>(utterance);
            Assert.Equal(invokedDialogResponse, reply.Text);

            // The Booking dialog displays an additional confirmation message, assert that is what we expect
            if (!string.IsNullOrEmpty(taskConfirmationMessage))
            {
                reply = testClient.GetNextReply<IMessageActivity>();
                Assert.StartsWith(taskConfirmationMessage, reply.Text);
            }

            reply = testClient.GetNextReply<IMessageActivity>();
            Assert.Equal("What else can I do for you?", reply.Text);
        }
    }
}

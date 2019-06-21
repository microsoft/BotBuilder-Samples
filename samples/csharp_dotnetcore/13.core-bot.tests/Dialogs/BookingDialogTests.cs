// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using CoreBot.Tests.Dialogs.TestData;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Testing;
using Microsoft.Bot.Builder.Testing.XUnit;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Microsoft.BotBuilderSamples.Dialogs;
using Microsoft.BotBuilderSamples.Tests.Framework;
using Xunit;
using Xunit.Abstractions;

namespace CoreBot.Tests.Dialogs
{
    public class BookingDialogTests : BotTestBase
    {
        private readonly XUnitOutputMiddleware[] _middlewares;

        public BookingDialogTests(ITestOutputHelper output)
            : base(output)
        {
            _middlewares = new[] { new XUnitOutputMiddleware(output) };
        }

        [Fact]
        public void DialogConstructor()
        {
            var sut = new BookingDialog();

            Assert.Equal("BookingDialog", sut.Id);
            Assert.IsType<TextPrompt>(sut.FindDialog("TextPrompt"));
            Assert.IsType<ConfirmPrompt>(sut.FindDialog("ConfirmPrompt"));
            Assert.IsType<DateResolverDialog>(sut.FindDialog("DateResolverDialog"));
            Assert.IsType<WaterfallDialog>(sut.FindDialog("WaterfallDialog"));
        }

        [Theory]
        [MemberData(nameof(BookingDialogTestsDataGenerator.BookingFlows), MemberType = typeof(BookingDialogTestsDataGenerator))]
        public async Task DialogFlowUseCases(TestDataObject testData)
        {
            // Arrange
            var bookingTestData = testData.GetObject<BookingDialogTestCase>();
            var sut = new BookingDialog();
            var testClient = new DialogTestClient(Channels.Test, sut, bookingTestData.InitialBookingDetails, _middlewares);

            // Act/Assert
            Output.WriteLine($"Test Case: {bookingTestData.Name}");
            for (var i = 0; i < bookingTestData.UtterancesAndReplies.GetLength(0); i++)
            {
                var reply = await testClient.SendActivityAsync<IMessageActivity>(bookingTestData.UtterancesAndReplies[i, 0]);
                Assert.Equal(bookingTestData.UtterancesAndReplies[i, 1], reply?.Text);
            }

            var bookingResults = (BookingDetails)testClient.DialogTurnResult.Result;
            Assert.Equal(bookingTestData.ExpectedBookingDetails?.Origin, bookingResults?.Origin);
            Assert.Equal(bookingTestData.ExpectedBookingDetails?.Destination, bookingResults?.Destination);
            Assert.Equal(bookingTestData.ExpectedBookingDetails?.TravelDate, bookingResults?.TravelDate);
        }

        [Theory]
        [MemberData(nameof(BookingDialogTestsDataGenerator.CancelFlows), MemberType = typeof(BookingDialogTestsDataGenerator))]
        public async Task ShouldBeAbleToCancelAtAnyTime(TestDataObject testData)
        {
            // Arrange
            var bookingTestData = testData.GetObject<BookingDialogTestCase>();
            var sut = new BookingDialog();
            var testClient = new DialogTestClient(Channels.Test, sut, bookingTestData.InitialBookingDetails, _middlewares);

            // Act/Assert
            Output.WriteLine($"Test Case: {bookingTestData.Name}");
            for (var i = 0; i < bookingTestData.UtterancesAndReplies.GetLength(0); i++)
            {
                var reply = await testClient.SendActivityAsync<IMessageActivity>(bookingTestData.UtterancesAndReplies[i, 0]);
                Assert.Equal(bookingTestData.UtterancesAndReplies[i, 1], reply.Text);
            }

            Assert.Equal(DialogTurnStatus.Cancelled, testClient.DialogTurnResult.Status);
            Assert.Null(testClient.DialogTurnResult.Result);
        }
    }
}

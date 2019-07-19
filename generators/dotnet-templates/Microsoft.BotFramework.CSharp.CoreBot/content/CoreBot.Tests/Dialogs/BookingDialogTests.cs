// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with CoreBot .NET Template version __vX.X.X__

using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Testing;
using Microsoft.Bot.Builder.Testing.XUnit;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Xunit;
using Xunit.Abstractions;

using CoreBot;
using CoreBot.Dialogs;
using CoreBot.Tests.Common;
using CoreBot.Tests.Dialogs.TestData;


namespace CoreBot.Tests.Dialogs
{
    public class BookingDialogTests : BotTestBase
    {
        private readonly IMiddleware[] _middlewares;

        public BookingDialogTests(ITestOutputHelper output)
            : base(output)
        {
            _middlewares = new IMiddleware[] { new XUnitDialogTestLogger(output) };
        }

        [Theory]
        [MemberData(nameof(BookingDialogTestsDataGenerator.BookingFlows), MemberType = typeof(BookingDialogTestsDataGenerator))]
        public async Task DialogFlowUseCases(TestDataObject testData)
        {
            // Arrange
            var bookingTestData = testData.GetObject<BookingDialogTestCase>();
            var sut = new BookingDialog();
            var testClient = new DialogTestClient(Channels.Test, sut, bookingTestData.InitialBookingDetails, _middlewares);

            // Execute the test case
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

            // Execute the test case
            Output.WriteLine($"Test Case: {bookingTestData.Name}");
            for (var i = 0; i < bookingTestData.UtterancesAndReplies.GetLength(0); i++)
            {
                var reply = await testClient.SendActivityAsync<IMessageActivity>(bookingTestData.UtterancesAndReplies[i, 0]);
                Assert.Equal(bookingTestData.UtterancesAndReplies[i, 1], reply.Text);
            }

            Assert.Equal(DialogTurnStatus.Complete, testClient.DialogTurnResult.Status);
            Assert.Null(testClient.DialogTurnResult.Result);
        }
    }
}

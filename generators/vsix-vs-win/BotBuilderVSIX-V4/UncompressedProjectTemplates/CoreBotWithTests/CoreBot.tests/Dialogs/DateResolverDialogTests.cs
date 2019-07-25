// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using CoreBot.Tests.Common;
using CoreBot.Tests.Dialogs.TestData;
using Microsoft.Bot.Builder.Testing;
using Microsoft.Bot.Builder.Testing.XUnit;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Xunit;
using Xunit.Abstractions;

using CoreBot.Dialogs;

namespace CoreBot.Tests.Dialogs
{
    public class DateResolverDialogTests : BotTestBase
    {
        public DateResolverDialogTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Theory]
        [MemberData(nameof(DateResolverDialogTestsDataGenerator.DateResolverCases), MemberType = typeof(DateResolverDialogTestsDataGenerator))]
        public async Task DialogFlowTests(TestDataObject testData)
        {
            // Arrange
            var testCaseData = testData.GetObject<DateResolverDialogTestCase>();
            var sut = new DateResolverDialog();
            var testClient = new DialogTestClient(Channels.Test, sut, testCaseData.InitialData, new[] { new XUnitDialogTestLogger(Output) });

            // Execute the test case
            Output.WriteLine($"Test Case: {testCaseData.Name}");
            Output.WriteLine($"\r\nDialog Input: {testCaseData.InitialData}");
            for (var i = 0; i < testCaseData.UtterancesAndReplies.GetLength(0); i++)
            {
                var reply = await testClient.SendActivityAsync<IMessageActivity>(testCaseData.UtterancesAndReplies[i, 0]);
                Assert.Equal(testCaseData.UtterancesAndReplies[i, 1], reply?.Text);
            }

            Output.WriteLine($"\r\nDialog result: {testClient.DialogTurnResult.Result}");
            Assert.Equal(testCaseData.ExpectedResult, testClient.DialogTurnResult.Result);
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Tests.Common;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Testing;
using Microsoft.Bot.Builder.Testing.XUnit;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples.Dialogs;
using Xunit;
using Xunit.Abstractions;

namespace CoreBot.Tests.Dialogs
{
    public class CancelAndHelpDialogTests : BotTestBase
    {
        private readonly XUnitDialogTestLogger[] _middlewares;

        public CancelAndHelpDialogTests(ITestOutputHelper output)
            : base(output)
        {
            _middlewares = new[] { new XUnitDialogTestLogger(output) };
        }

        [Theory]
        [InlineData("cancel")]
        [InlineData("quit")]
        public async Task ShouldBeAbleToCancel(string cancelUtterance)
        {
            var sut = new TestCancelAndHelpDialog();
            var testClient = new DialogTestClient(Channels.Test, sut, middlewares: _middlewares);

            // Execute the test case
            var reply = await testClient.SendActivityAsync<IMessageActivity>("Hi");
            Assert.Equal("Hi there", reply.Text);
            Assert.Equal(DialogTurnStatus.Waiting, testClient.DialogTurnResult.Status);

            reply = await testClient.SendActivityAsync<IMessageActivity>(cancelUtterance);
            Assert.Equal("Cancelling...", reply.Text);
            Assert.Equal(DialogTurnStatus.Cancelled, testClient.DialogTurnResult.Status);
        }

        [Theory]
        [InlineData("help")]
        [InlineData("?")]
        public async Task ShouldBeAbleToGetHelp(string cancelUtterance)
        {
            var sut = new TestCancelAndHelpDialog();
            var testClient = new DialogTestClient(Channels.Test, sut, middlewares: _middlewares);

            // Execute the test case
            var reply = await testClient.SendActivityAsync<IMessageActivity>("Hi");
            Assert.Equal("Hi there", reply.Text);
            Assert.Equal(DialogTurnStatus.Waiting, testClient.DialogTurnResult.Status);

            reply = await testClient.SendActivityAsync<IMessageActivity>(cancelUtterance);
            Assert.Equal("Show help here", reply.Text);
            Assert.Equal(DialogTurnStatus.Waiting, testClient.DialogTurnResult.Status);
        }

        /// <summary>
        /// A concrete instance of <see cref="CancelAndHelpDialog"/> for testing.
        /// </summary>
        private class TestCancelAndHelpDialog : CancelAndHelpDialog
        {
            public TestCancelAndHelpDialog()
                : base(nameof(TestCancelAndHelpDialog))
            {
                AddDialog(new TextPrompt(nameof(TextPrompt)));
                var steps = new WaterfallStep[]
                {
                    PromptStep,
                    FinalStep,
                };
                AddDialog(new WaterfallDialog("testWaterfall", steps));
                InitialDialogId = "testWaterfall";
            }

            private async Task<DialogTurnResult> PromptStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Hi there") }, cancellationToken);
            }

            private Task<DialogTurnResult> FinalStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}

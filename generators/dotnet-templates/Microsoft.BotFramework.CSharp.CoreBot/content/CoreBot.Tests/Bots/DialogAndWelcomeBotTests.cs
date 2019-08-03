// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with CoreBot .NET Template version __vX.X.X__

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Xunit;

using CoreBot.Bots;
using CoreBot.Tests.Common;

namespace CoreBot.Tests.Bots
{
    public class DialogAndWelcomeBotTests
    {
        [Fact]
        public async Task ReturnsWelcomeCardOnConversationUpdate()
        {
            // Arrange
            var mockRootDialog = SimpleMockFactory.CreateMockDialog<Dialog>(null, "mockRootDialog");
            var memoryStorage = new MemoryStorage();
            var sut = new DialogAndWelcomeBot<Dialog>(new ConversationState(memoryStorage), new UserState(memoryStorage), mockRootDialog.Object, null);

            // Create conversationUpdate activity
            var conversationUpdateActivity = new Activity
            {
                Type = ActivityTypes.ConversationUpdate,
                MembersAdded = new List<ChannelAccount>
                {
                    new ChannelAccount { Id = "theUser" },
                },
                Recipient = new ChannelAccount { Id = "theBot" },
            };
            var testAdapter = new TestAdapter(Channels.Test);

            // Act
            // Send the conversation update activity to the bot.
            await testAdapter.ProcessActivityAsync(conversationUpdateActivity, sut.OnTurnAsync, CancellationToken.None);

            // Assert we got the welcome card
            var reply = (IMessageActivity)testAdapter.GetNextReply();
            Assert.Equal(1, reply.Attachments.Count);
            Assert.Equal("application/vnd.microsoft.card.adaptive", reply.Attachments.FirstOrDefault()?.ContentType);

            // Assert that we started the main dialog.
            reply = (IMessageActivity)testAdapter.GetNextReply();
            Assert.Equal("Dialog mock invoked", reply.Text);
        }
    }
}

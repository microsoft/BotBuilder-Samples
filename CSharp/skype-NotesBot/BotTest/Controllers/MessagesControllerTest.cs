using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Bot;
using Bot.Controllers;
using Bot.Models;
using Microsoft.Bot.Connector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace BotTest.Controllers
{
    [TestClass]
    public sealed class MessagesControllerTest : DialogTestBase
    {
        [Test]
        [TestCase(ContactRelationUpdateActionTypes.Add, true)]
        [TestCase(ContactRelationUpdateActionTypes.Add, false)]
        [TestCase(ContactRelationUpdateActionTypes.Remove, false)]
        public async Task CheckOnContactRelationUpdate(string actionType, bool isBrandNewUser)
        {
            var mongoDatabase = new Mock<IMongoDatabase>();
            mongoDatabase.Object.CreateCollection(BotConstants.AllUsers);
            var userCollection = new Mock<IMongoCollection<User>>();

            if (isBrandNewUser)
            {
                userCollection.Setup(a => a.FindAsync<User>(
                        It.IsAny<FilterDefinition<User>>(),
                        null,
                        CancellationToken.None))
                    .Returns(SetupUserResultInFirstBatch());
                mongoDatabase.Setup(f => f.GetCollection<User>(BotConstants.AllUsers, null))
                    .Returns(userCollection.Object);
            }
            else
            {
                userCollection.Setup(a => a.FindOneAndUpdateAsync<User>(
                        It.IsAny<FilterDefinition<User>>(),
                        It.IsAny<UpdateDefinition<User>>(),
                        null,
                        CancellationToken.None))
                    .Returns(Task.FromResult(It.IsAny<User>()));
            }

            var activityMessageId = Guid.NewGuid().ToString();
            const string userName = "Arun Srinivasan";
            const string userId = "29:1rjPwPi-pFr4-Sx1_1sdCDgHAZw4l7WA91eRX-6ZRG0c";
            const string serviceUrl = "https://smba.trafficmanager.net/apis/";
            const string channelId = "skype";
            var activity = new Activity
            {
                Id = activityMessageId,
                Type = ActivityTypes.ContactRelationUpdate,
                Action = ContactRelationUpdateActionTypes.Add,
                From = new ChannelAccount(userId, userName),
                Recipient = new ChannelAccount(BotConstants.BotId, BotConstants.BotName),
                ServiceUrl = serviceUrl,
                ChannelId = channelId,
                Conversation = new ConversationAccount {Id = userId},
                Attachments = Array.Empty<Attachment>(),
                Entities = Array.Empty<Entity>()
            };

            var connectorClient = new TestConnectorClient(new Uri(activity.ServiceUrl));
            connectorClient.MockedConversations = new TestConversations(connectorClient);

            var messagesController = new MessagesController(mongoDatabase.Object, connectorClient)
            {
                Configuration = new HttpConfiguration(),
                Request = new HttpRequestMessage()
            };

            // Act
            var response = await messagesController.Post(activity);
            var responseMessage = await response.Content.ReadAsStringAsync();

            // Assert
            switch (actionType)
            {
                case ContactRelationUpdateActionTypes.Add:
                    Assert.IsNotEmpty(responseMessage);
                    break;
                case ContactRelationUpdateActionTypes.Remove:
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    break;
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task CheckOnConversationUpdate(bool isBotAdded)
        {
            var mongoDatabase = new Mock<IMongoDatabase>();
            mongoDatabase.Object.CreateCollection(BotConstants.AllUsers);

            var activityMessageId = Guid.NewGuid().ToString();
            const string userName = "Arun Srinivasan";
            const string userId = "29:1rjPwPi-pFr4-Sx1_1sdCDgHAZw4l7WA91eRX-6ZRG0c";
            const string serviceUrl = "https://smba.trafficmanager.net/apis/";
            const string channelId = "skype";
            var activity = new Activity
            {
                Id = activityMessageId,
                Type = ActivityTypes.ConversationUpdate,
                Action = null,
                From = new ChannelAccount(userId, userName),
                Recipient = new ChannelAccount(BotConstants.BotId, BotConstants.BotName),
                ServiceUrl = serviceUrl,
                ChannelId = channelId,
                Conversation = new ConversationAccount {Id = userId},
                Attachments = Array.Empty<Attachment>(),
                Entities = Array.Empty<Entity>(),
                MembersAdded = isBotAdded
                    ? new List<ChannelAccount> {new ChannelAccount(BotConstants.BotId, BotConstants.BotName)}
                    : new List<ChannelAccount> {new ChannelAccount(string.Empty, string.Empty)}
            };


            var connectorClient = new TestConnectorClient(new Uri(activity.ServiceUrl));
            connectorClient.MockedConversations = new TestConversations(connectorClient);

            var messagesController = new MessagesController(mongoDatabase.Object, connectorClient)
            {
                Configuration = new HttpConfiguration(),
                Request = new HttpRequestMessage()
            };

            // Act
            var response = await messagesController.Post(activity);
            var responseMessage = await response.Content.ReadAsStringAsync();

            // Assert
            switch (isBotAdded)
            {
                case true:
                    Assert.IsNotEmpty(responseMessage);
                    break;
                case false:
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    break;
            }
        }

        private static Task<IAsyncCursor<User>> SetupUserResultInFirstBatch(User user = null)
        {
            var mockCursor = new Mock<IAsyncCursor<User>>();
            mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));
            mockCursor.SetupSequence(c => c.Current).Returns(new List<User> {user}).Returns(null);
            return Task.FromResult(mockCursor.Object);
        }
    }
}
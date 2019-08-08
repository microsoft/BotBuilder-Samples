// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Handoff;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Rest;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Bot.Builder.Handoff.UnitTests
{
    [TestClass]
    public class HandoffTests
    {
        [TestMethod]
        public async Task BotFrameworkHttpAdapter_does_not_support_handoff()
        {
            await Assert.ThrowsExceptionAsync<NotSupportedException>(async () =>
            {
                var adapter = new BotFrameworkHttpAdapter();
                ITurnContext turnContext = new TurnContext(adapter, new Schema.Activity());
                await turnContext.InitiateHandoffAsync(new IActivity []{ }, 0);
            });
        }

        [TestMethod]
        public async Task Test_InitiateHandoffAsync_Request()
        {
            var messageHandler = new MockHttpMessageHandler();

            string testValue = "cookie123";
            var handoffRequest = await GetHandoffRequestAsync(messageHandler, testValue);

            Assert.IsTrue(messageHandler.LastRequestUri.ToString().EndsWith("/v3/conversations/123/handoff"));
            Assert.AreEqual(messageHandler.LastRequestMethod, HttpMethod.Post);
            Assert.IsTrue(messageHandler.LastRequestContent.Contains($"\"{testValue}\""));
        }

        [TestMethod]
        public async Task Test_InitiateHandoffAsync_Request_Error()
        {
            await Assert.ThrowsExceptionAsync<ErrorResponseException>(async () =>
            {
                var messageHandler = new MockHttpMessageHandler();
                messageHandler.StatusCode = System.Net.HttpStatusCode.BadRequest;
                var handoffRequest = await GetHandoffRequestAsync(messageHandler, string.Empty);
            });
        }

        [TestMethod]
        public async Task Test_HandoffRequest()
        {
            var messageHandler = new MockHttpMessageHandler();
            var handoffRequest = await GetHandoffRequestAsync(messageHandler, "nothing");

            messageHandler.ResponseContent = "\"completed\"";
            var completed = await handoffRequest.IsCompletedAsync();
            Assert.IsTrue(completed);

            Assert.IsTrue(messageHandler.LastRequestUri.ToString().EndsWith("/v3/conversations/123/handoff"));
            Assert.AreEqual(messageHandler.LastRequestMethod, HttpMethod.Get);

            messageHandler.ResponseContent = "\"something else\"";
            var notcompleted = await handoffRequest.IsCompletedAsync();
            Assert.IsFalse(notcompleted);
        }

        [TestMethod]
        public async Task Test_HandoffRequest_Error()
        {
            var messageHandler = new MockHttpMessageHandler();
            var handoffRequest = await GetHandoffRequestAsync(messageHandler, "nothing");

            messageHandler.ResponseContent = "\"error\"";
            messageHandler.StatusCode = System.Net.HttpStatusCode.NotFound;
            var completed = await handoffRequest.IsCompletedAsync();
            Assert.IsFalse(completed);
        }

        private async Task<HandoffRequest> GetHandoffRequestAsync(HttpMessageHandler messageHandler, string testValue)
        {
            var adapter = new BotFrameworkHttpAdapterWithHandoff();
            var turnContext = new TurnContext(adapter, new Activity { Conversation = new ConversationAccount { Id = "123" } });

            //var mockClient = new MockConnectorClient();

            var mockHttpClient = new HttpClient(messageHandler);

            var mockClient = new Mock<IConnectorClient>();
            mockClient.Setup(c => c.Conversations).Returns(new MockConversations(mockHttpClient));

            //var connector = new MockConnector(mockHttpClient);
            //var connector = new Mock<ConnectorClient>(mockHttpClient);

            //var mockConversations = new Mock<IServiceOperations<ConnectorClient>>();
            //mockConversations.Setup(c => c.Client).Returns(connector);

            //mockClient.Conversations = new MockConversations(mockHttpClient);

            turnContext.TurnState.Add<IConnectorClient>(mockClient.Object);

            var activity = new Activity() as IActivity;
            var handoffRequest = await turnContext.InitiateHandoffAsync(new IActivity[] { activity }, new { TestObject = testValue });

            return handoffRequest;
        }
    }
}

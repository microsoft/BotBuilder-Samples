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
                await turnContext.InitiateHandoffAsync(null, null);
            });
        }

        [TestMethod]
        public async Task Test_InitiateHandoffAsync_Request()
        {
            var messageHandler = new MockHttpMessageHandler();

            string testValue = "cookie123";
            var handoffRequest = await GetHandoffRequestAsync(messageHandler, testValue);

            var lastRequest = messageHandler.LastRequest;

            Assert.IsTrue(lastRequest.RequestUri.ToString().EndsWith("/v3/conversations/123/handoff"));
            Assert.AreEqual(lastRequest.Method, HttpMethod.Post);
            Assert.IsTrue(lastRequest.AsFormattedString().Contains($"\"{testValue}\""));
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

            messageHandler.ResponseContent = "\"something else\"";
            var notcompleted = await handoffRequest.IsCompletedAsync();
            Assert.IsFalse(notcompleted);
        }

        [TestMethod]
        public async Task Test_HandoffRequest_Error()
        {
            var messageHandler = new MockHttpMessageHandler();
            var handoffRequest = await GetHandoffRequestAsync(messageHandler, "nothing");

            messageHandler.ResponseContent = "\"completed\"";
            messageHandler.StatusCode = System.Net.HttpStatusCode.BadRequest;
            var completed = await handoffRequest.IsCompletedAsync();
            Assert.IsFalse(completed);
        }

        private async Task<IHandoffRequest> GetHandoffRequestAsync(HttpMessageHandler messageHandler, string testValue)
        {
            var adapter = new BotFrameworkHttpAdapterWithHandoff();
            var turnContext = new TurnContext(adapter, new Activity { Conversation = new ConversationAccount { Id = "123" } });

            var mockClient = new MockConnectorClient();

            var mockHttpClient = new HttpClient(messageHandler);

            mockClient.Conversations = new MockConversations(mockHttpClient);

            turnContext.TurnState.Add<IConnectorClient>(mockClient);

            var activity = new Activity() as IActivity;
            var handoffRequest = await turnContext.InitiateHandoffAsync(new IActivity[] { activity }, new { TestObject = testValue });

            return handoffRequest;
        }
    }
}

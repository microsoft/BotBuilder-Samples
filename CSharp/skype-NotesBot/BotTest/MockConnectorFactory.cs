using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using Microsoft.Rest;
using Moq;

namespace BotTest
{
    public class MockConnectorFactory : IConnectorClientFactory
    {
        private readonly string _botId;
        private readonly IBotDataStore<BotData> _memoryDataStore = new InMemoryDataStore();
        private StateClient _stateClient;

        public MockConnectorFactory(string botId)
        {
            SetField.NotNull(out _botId, nameof(botId), botId);
        }

        public IConnectorClient MakeConnectorClient()
        {
            var client = new Mock<ConnectorClient> {CallBase = true};
            return client.Object;
        }

        public IStateClient MakeStateClient()
        {
            return _stateClient ?? (_stateClient = MockIBots(this).Object);
        }

        private IAddress AddressFrom(string channelId, string userId, string conversationId)
        {
            var address = new Address
            (
                _botId,
                channelId,
                userId ?? "AllUsers",
                conversationId ?? "AllConversations",
                "InvalidServiceUrl"
            );
            return address;
        }

        private async Task<HttpOperationResponse<object>> UpsertData(string channelId, string userId,
            string conversationId, BotStoreType storeType, BotData data)
        {
            var result = new HttpOperationResponse<object> {Request = new HttpRequestMessage()};
            try
            {
                var address = AddressFrom(channelId, userId, conversationId);
                await _memoryDataStore.SaveAsync(address, storeType, data, CancellationToken.None);
            }
            catch (HttpException e)
            {
                result.Body = e.Data;
                result.Response = new HttpResponseMessage {StatusCode = HttpStatusCode.PreconditionFailed};
                return result;
            }
            catch (Exception)
            {
                result.Response = new HttpResponseMessage {StatusCode = HttpStatusCode.InternalServerError};
                return result;
            }

            result.Body = data;
            result.Response = new HttpResponseMessage {StatusCode = HttpStatusCode.OK};
            return result;
        }

        private async Task<HttpOperationResponse<object>> GetData(string channelId, string userId,
            string conversationId, BotStoreType storeType)
        {
            var result = new HttpOperationResponse<object> {Request = new HttpRequestMessage()};
            var address = AddressFrom(channelId, userId, conversationId);
            var data = await _memoryDataStore.LoadAsync(address, storeType, CancellationToken.None);
            result.Body = data;
            result.Response = new HttpResponseMessage {StatusCode = HttpStatusCode.OK};
            return result;
        }

        private static Mock<StateClient> MockIBots(MockConnectorFactory mockConnectorFactory)
        {
            var botsClient = new Mock<StateClient>(MockBehavior.Loose);

            botsClient.Setup(d => d.BotState.SetConversationDataWithHttpMessagesAsync(It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<BotData>(), It.IsAny<Dictionary<string, List<string>>>(),
                    It.IsAny<CancellationToken>()))
                .Returns<string, string, BotData, Dictionary<string, List<string>>, CancellationToken>(
                    async (channelId, conversationId, data, headers, token) => await mockConnectorFactory.UpsertData(
                        channelId, null, conversationId,
                        BotStoreType.BotConversationData, data));

            botsClient.Setup(d => d.BotState.GetConversationDataWithHttpMessagesAsync(It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .Returns<string, string, Dictionary<string, List<string>>, CancellationToken>(
                    async (channelId, conversationId, headers, token) => await mockConnectorFactory.GetData(channelId,
                        null, conversationId,
                        BotStoreType.BotConversationData));


            botsClient.Setup(d => d.BotState.SetUserDataWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<BotData>(), It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .Returns<string, string, BotData, Dictionary<string, List<string>>, CancellationToken>(
                    async (channelId, userId, data, headers, token) => await mockConnectorFactory.UpsertData(channelId,
                        userId, null, BotStoreType.BotUserData,
                        data));

            botsClient.Setup(d => d.BotState.GetUserDataWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .Returns<string, string, Dictionary<string, List<string>>, CancellationToken>(
                    async (channelId, userId, headers, token) => await mockConnectorFactory.GetData(channelId, userId,
                        null, BotStoreType.BotUserData));

            botsClient.Setup(d => d.BotState.SetPrivateConversationDataWithHttpMessagesAsync(It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<BotData>(),
                    It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .Returns<string, string, string, BotData, Dictionary<string, List<string>>, CancellationToken>(
                    async (channelId, conversationId, userId, data, headers, token) => await mockConnectorFactory
                        .UpsertData(channelId, userId, conversationId,
                            BotStoreType.BotPrivateConversationData, data));

            botsClient.Setup(d => d.BotState.GetPrivateConversationDataWithHttpMessagesAsync(It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, List<string>>>(),
                    It.IsAny<CancellationToken>()))
                .Returns<string, string, string, Dictionary<string, List<string>>, CancellationToken>(
                    async (channelId, conversationId, userId, headers, token) => await mockConnectorFactory.GetData(
                        channelId, userId, conversationId,
                        BotStoreType.BotPrivateConversationData));

            return botsClient;
        }
    }
}
using System;
using Microsoft.Bot.Connector;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.Handoff.UnitTests
{
    internal class MockConnectorClient : IConnectorClient
    {
        public Uri BaseUri { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public JsonSerializerSettings SerializationSettings => throw new NotImplementedException();

        public JsonSerializerSettings DeserializationSettings => throw new NotImplementedException();

        public ServiceClientCredentials Credentials => throw new NotImplementedException();

        public IAttachments Attachments => throw new NotImplementedException();

        public IConversations Conversations { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

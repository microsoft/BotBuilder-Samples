using System;
using Microsoft.Bot.Connector;

namespace BotTest
{
    internal sealed class TestConnectorClient : ConnectorClient
    {
        public TestConnectorClient(Uri uri) : base(uri)
        {
            MockedConversations = new TestConversations(this);
        }

        public override IConversations Conversations => MockedConversations;

        public IConversations MockedConversations { private get; set; }
    }
}
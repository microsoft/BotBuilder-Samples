// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Net.Http;
using Microsoft.Bot.Connector;

namespace Microsoft.Bot.Builder.Handoff.UnitTests
{
    internal class MockConnector : ConnectorClient
    {
        public MockConnector(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }
    }
}

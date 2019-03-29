// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// A utility to get client for Event Hub.
    /// </summary>
    internal static class EventHubHelper
    {
        public const string EventHubConnectionString = "eventHub.connectionString";
        public const string EventHubName = "eventHub.name";

        public static EventHubClient GetEventHubClient(IConfiguration eventHubConfig)
        {
            var eventHubConnectionString = eventHubConfig[EventHubConnectionString];
            var eventHubName = eventHubConfig[EventHubName];
            var connStrBuilder = new EventHubsConnectionStringBuilder(eventHubConnectionString)
            {
                EntityPath = eventHubName,
            };

            var eventHubClient = EventHubClient.CreateFromConnectionString(connStrBuilder.ToString());
            return eventHubClient;
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// Logs transcript activity events to Event Hub.
    /// </summary>
    public class EventHubSender : IEventHubSender
    {
        private readonly EventHubClient _eventHubClient;

        public EventHubSender(IConfiguration config)
        {
            _eventHubClient = EventHubHelper.GetEventHubClient(config);
        }

        /// <summary>
        /// Creates an empty `EventDataBatch`.
        /// </summary>
        public EventDataBatch CreateBatch() => _eventHubClient.CreateBatch();

        /// <summary>
        /// Sends the batch when it contains one or more events.
        /// </summary>
        public async Task SendAsync(EventDataBatch events)
        {
            if (events.Count > 0)
            {
                // Uncomment the following line to view the number of events sent in a batch.
                //Debug.Print($"Sending {events.Count} events in a batch.");

                await _eventHubClient.SendAsync(events);
            }
        }
    }
}

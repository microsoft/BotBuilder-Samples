// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// Creates an Event Hub batch, and sends transcript activity events to the Event Hub.
    /// </summary>
    public interface IEventHubSender
    {
        Task SendAsync(EventDataBatch events);

        EventDataBatch CreateBatch();
    }
}

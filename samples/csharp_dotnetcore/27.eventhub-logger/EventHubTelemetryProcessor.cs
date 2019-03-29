// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// A telemetry processor for event hub.
    /// It extracts a transcript activity from an `ITelemetry` item, and queues it into an in-memory queue for processing,
    /// and then remove the transcript activity from the item, given that Application Insights logging is more lightweight.
    /// </summary>
    public class EventHubTelemetryProcessor : ITelemetryProcessor
    {
        private ITelemetryProcessor _next;
        private ITranscriptQueue _queue;

        public EventHubTelemetryProcessor(ITelemetryProcessor next, ITranscriptQueue queue)
        {
            _queue = queue;

            // Next TelemetryProcessor in the chain
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            var hasTrans = TelemetryHelper.HasTranscript(item);
            if (hasTrans)
            {
                var transItem = TelemetryHelper.GetTranscript(item);

                // Uncomment the following line to view the transcript activity in debugger.
                // Debug.Print($"eventhub: {transItem}");

                // Queue this for sending to event hub
                _queue.Enqueue(transItem);
            }

            // Remove the original transcript activity for Application Insights.
            var newItem = hasTrans ? TelemetryHelper.RemoveTranscriptForEvent(item) : item;

            // Send the item to the next TelemetryProcessor
            _next.Process(newItem);
        }
    }
}

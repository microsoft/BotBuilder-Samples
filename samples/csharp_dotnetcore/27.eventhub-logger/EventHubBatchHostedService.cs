// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// A hosted background services for Event Hub sender.
    /// Gather a batch of transcript activity events for a max delay and a max size, ~250KB as allowed by Event Hub.
    /// Send the batch via a sender, when either the delay or size limit met first.
    /// </summary>
    public class EventHubHostedService : BackgroundService
    {
        private const string EventHubBatchDelaySeconds = "eventhub.batchDelaySec";
        private const int DefaultEventHubBatchDelaySeconds = 5;

        private readonly ITranscriptQueue _queue;
        private readonly IEventHubSender _sender;
        private readonly TimeSpan _batchDelay;
        private readonly ILogger _logger;

        public EventHubHostedService(
            ITranscriptQueue queue,
            IEventHubSender sender,
            IConfiguration configuration,
            ILogger<EventHubHostedService> logger)
        {
            _queue = queue;
            _sender = sender;
            _logger = logger;

            var seconds = configuration.GetValue<int>(EventHubBatchDelaySeconds);
            seconds = seconds < 1 ? DefaultEventHubBatchDelaySeconds : seconds;
            _batchDelay = TimeSpan.FromSeconds(seconds);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Event Hub Hosted Service is starting.");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (var events = await GetBatchForDelayAsync(_batchDelay, cancellationToken))
                    {
                        await _sender.SendAsync(events);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error occurred: {ex.Message}.");
                }
            }

            _logger.LogInformation("Event Hub Hosted Service is stopping.");
        }

        private async Task<EventDataBatch> GetBatchForDelayAsync(TimeSpan batchDelay, CancellationToken cancellationToken)
        {
            using (var delaySource = new CancellationTokenSource(batchDelay))
            using (var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(delaySource.Token, cancellationToken))
            {
                return await GetBatchAsync(linkedSource.Token);
            }
        }

        private async Task<EventDataBatch> GetBatchAsync(CancellationToken cancellationToken)
        {
            var batch = _sender.CreateBatch();
            while (!cancellationToken.IsCancellationRequested)
            {
                var (found, message) = await TryPeekWithDelayAsync();
                if (!found)
                {
                    continue;
                }

                var eventData = new EventData(Encoding.UTF8.GetBytes(message));
                var added = batch.TryAdd(eventData);
                if (!added)
                {
                    break;
                }

                _queue.TryDequeue();
            }

            return batch;
        }

        private async Task<(bool, string)> TryPeekWithDelayAsync()
        {
            var (found, message) = _queue.TryPeek();
            if (!found)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(50));
                (found, message) = _queue.TryPeek();
            }

            return (found, message);
        }
    }
}

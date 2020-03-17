// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples.DialogRootBot.Middleware
{
    /// <summary>
    /// Uses an ILogger instance to log user and bot messages. It filters out ContinueConversation events coming from skill responses.
    /// </summary>
    public class LoggerMiddleware : IMiddleware
    {
        private readonly ILogger<BotFrameworkHttpAdapter> _logger;

        public LoggerMiddleware(ILogger<BotFrameworkHttpAdapter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            // Note: skill responses will show as ContinueConversation events; we don't log those.
            // We only log incoming messages from users.
            if (turnContext.Activity.Type != ActivityTypes.Event && turnContext.Activity.Name != "ContinueConversation")
            {
                var message = $"User said: {turnContext.Activity.Text} Type: \"{turnContext.Activity.Type}\" Name: \"{turnContext.Activity.Name}\"";
                _logger.LogInformation(message);
            }

            // Register outgoing handler.
            turnContext.OnSendActivities(OutgoingHandler);

            // Continue processing messages.
            await next(cancellationToken);
        }

        private async Task<ResourceResponse[]> OutgoingHandler(ITurnContext turnContext, List<Activity> activities, Func<Task<ResourceResponse[]>> next)
        {
            foreach (var activity in activities)
            {
                var message = $"Bot said: {activity.Text} Type: \"{activity.Type}\" Name: \"{activity.Name}\"";
                _logger.LogInformation(message);
            }

            return await next();
        }
    }
}

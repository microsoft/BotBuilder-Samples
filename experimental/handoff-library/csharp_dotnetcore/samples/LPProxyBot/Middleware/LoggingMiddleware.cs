using System;
using Microsoft.Bot.Builder;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using System.Linq;
using LPProxyBot.Bots;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Concurrent;

namespace LPProxyBot
{
    public class LoggingMiddleware : Microsoft.Bot.Builder.IMiddleware
    {
        private BotState _conversationState;

        public LoggingMiddleware(ConversationState conversationState)
        {
            _conversationState = conversationState;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            // Route the conversation based on whether it's been escalated
            var conversationStateAccessors = _conversationState.CreateProperty<LoggingConversationData>(nameof(LoggingConversationData));
            var conversationData = await conversationStateAccessors.GetAsync(turnContext, () => new LoggingConversationData());

            conversationData.ConversationLog.Add(turnContext.Activity);
            await _conversationState.SaveChangesAsync(turnContext);

            turnContext.OnSendActivities(async (sendTurnContext, activities, nextSend) =>
            {
                conversationData.ConversationLog.AddRange(activities);
                await _conversationState.SaveChangesAsync(turnContext);
                // run full pipeline
                var responses = await nextSend().ConfigureAwait(false);
                return responses;
            });

            await next(cancellationToken);
        }
    }
}

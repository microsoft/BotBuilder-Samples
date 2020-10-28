// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.9.2

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImmediateAcceptBot.BackgroundQueue;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace ImmediateAcceptBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly string _botId;
        private readonly int _shutdownTimeoutSeconds;

        public EchoBot(IConfiguration config, IBackgroundTaskQueue taskQueue)
        {
            if (taskQueue == null)
            {
                throw new ArgumentNullException(nameof(taskQueue));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _shutdownTimeoutSeconds = config.GetValue<int>("ShutdownTimeoutSeconds");
            _botId = config.GetValue<string>("MicrosoftAppId");
            _botId = string.IsNullOrEmpty(_botId) ? Guid.NewGuid().ToString() : _botId;
            _taskQueue = taskQueue;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var text = turnContext.Activity.Text.ToLower().Trim();
            var hasText = !string.IsNullOrEmpty(text);
            bool pause = hasText && text.Contains("pause");
            bool background = hasText && text.Contains("background");

            if(background && pause)
            {
                await turnContext.SendActivityAsync($"I cannot do a 'pause' and 'background' both.  These are exclusive commands.", cancellationToken: cancellationToken).ConfigureAwait(false);
                await SendHelp(turnContext, cancellationToken);
            }
            else if (pause || background)
            {
                await HandlePauseOrBackground(turnContext, text, cancellationToken);
            }
            else if (!hasText || text.Contains("help"))
            {
                await SendHelp(turnContext, cancellationToken);
            }
            else
            {
                var replyText = $"Echo: {text} (send 'help' if you are not sure what to do)";
                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }
        }

        private async Task HandlePauseOrBackground(ITurnContext turnContext, string text, CancellationToken cancellationToken)
        {
            var splitText = text.Split(" ");
            int seconds = splitText.Select((s, i) => int.TryParse(s, out i) ? i : 0).FirstOrDefault(i => i > 0);

            // validate seconds were received in the text
            if (seconds < 1 || seconds > _shutdownTimeoutSeconds)
            {
                await turnContext.SendActivityAsync($"Please enter seconds < {_shutdownTimeoutSeconds} > 0, and processing type.  Example: 4 seconds or 4 background", cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                // if the user sent a message with, send it back to them after the pause/background timeout
                string message = splitText.Length > 2 ? splitText.Where(word =>
                                                                        !new[]
                                                                        {
                                                                                "seconds",
                                                                                "second",
                                                                                "pause",
                                                                                "background",
                                                                                seconds.ToString()
                                                                        }
                                                                        .Contains(word.ToLower()))
                                                                        .Aggregate(string.Empty, (current, next) => current + " " + next) : string.Empty;
                if (text.Contains("pause"))
                {
                    await turnContext.SendActivityAsync($"okay, pausing {seconds} seconds", cancellationToken: cancellationToken);
                    Thread.Sleep(TimeSpan.FromSeconds(seconds));
                    await turnContext.SendActivityAsync($"finished pausing {seconds} seconds {message}", cancellationToken: cancellationToken);
                }
                else
                {
                    await turnContext.SendActivityAsync($"okay, I will background message you after: {seconds} seconds", cancellationToken: cancellationToken).ConfigureAwait(false);
                    _taskQueue.QueueBackgroundWorkItem(async cancelToken => await ProactiveMessageCallbackAsync(turnContext.Adapter, turnContext.Activity.GetConversationReference(), message, seconds, cancelToken).ConfigureAwait(false));

                }
            }
        }

        private async Task ProactiveMessageCallbackAsync(BotAdapter adapter, ConversationReference conversationReference, string message, int seconds, CancellationToken cancellationToken)
        {
            // Pause on the background thread for the number of seconds specified, then load the conversation and message the user.
            // This simulates a long running process.
            Thread.Sleep(TimeSpan.FromSeconds(seconds));

            await adapter.ContinueConversationAsync(_botId, conversationReference, async (innerContext, innerCancellationToken) =>
            { 
                await innerContext.SendActivityAsync(string.IsNullOrWhiteSpace(message) ? $"background notice after {seconds} seconds" : $"background msg {seconds} {message}"); 
                // Could load a dialog stack here, and resume
            }, cancellationToken);
        }

        private async Task SendHelp(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text("send: 4 seconds   ...  and i will pause for 4 seconds while processing your message."), cancellationToken);
            await turnContext.SendActivityAsync(MessageFactory.Text("send: 4 background   ...  and i will push your message to an additional background thread to process for 4 seconds."), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Hi!  I'm a background processing bot. All incoming messages are processed on a background thread."), cancellationToken);
                    await SendHelp(turnContext, cancellationToken);
                }
            }
        }
    }
}

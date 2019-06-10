// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Console_EchoBot
{
    public class ConsoleAdapter : BotAdapter
    {
        public ConsoleAdapter()
            : base()
        {
        }

        // Adds middleware to the adapter's pipeline.
        public new ConsoleAdapter Use(IMiddleware middleware)
        {
            base.Use(middleware);
            return this;
        }

        // Performs the actual translation of input coming from the console
        // into the "Activity" format that the Bot consumes.
        public async Task ProcessActivityAsync(BotCallbackHandler callback = null)
        {
            while (true)
            {
                var msg = Console.ReadLine();
                if (msg == null)
                {
                    break;
                }

                // Performing the conversion from console text to an Activity for
                // which the system handles all messages (from all unique services).
                // All processing is performed by the broader bot pipeline on the Activity
                // object.
                var activity = new Activity()
                {
                    Text = msg,

                    // Note on ChannelId:
                    // The Bot Framework channel is identified by a unique ID.
                    // For example, "skype" is a common channel to represent the Skype service.
                    // We are inventing a new channel here.
                    ChannelId = "console",
                    From = new ChannelAccount(id: "user", name: "User1"),
                    Recipient = new ChannelAccount(id: "bot", name: "Bot"),
                    Conversation = new ConversationAccount(id: "Convo1"),
                    Timestamp = DateTime.UtcNow,
                    Id = Guid.NewGuid().ToString(),
                    Type = ActivityTypes.Message,
                };

                using (var context = new TurnContext(this, activity))
                {
                    await this.RunPipelineAsync(context, callback, default(CancellationToken)).ConfigureAwait(false);
                }
            }
        }

        // Sends activities to the conversation.
        public override async Task<ResourceResponse[]> SendActivitiesAsync(ITurnContext context, Activity[] activities, CancellationToken cancellationToken)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (activities == null)
            {
                throw new ArgumentNullException(nameof(activities));
            }

            if (activities.Length == 0)
            {
                throw new ArgumentException("Expecting one or more activities, but the array was empty.", nameof(activities));
            }

            var responses = new ResourceResponse[activities.Length];

            for (var index = 0; index < activities.Length; index++)
            {
                var activity = activities[index];

                switch (activity.Type)
                {
                    case ActivityTypes.Message:
                        {
                            IMessageActivity message = activity.AsMessageActivity();

                            // A message exchange between user and bot can contain media attachments
                            // (e.g., image, video, audio, file).  In this particular example, we are unable
                            // to create Attachments to messages, but this illustrates processing.
                            if (message.Attachments != null && message.Attachments.Any())
                            {
                                var attachment = message.Attachments.Count == 1 ? "1 attachment" : $"{message.Attachments.Count()} attachments";
                                Console.WriteLine($"{message.Text} with {attachment} ");
                            }
                            else
                            {
                                Console.WriteLine($"{message.Text}");
                            }
                        }

                        break;

                    case ActivityTypesEx.Delay:
                        {
                            // The Activity Schema doesn't have a delay type build in, so it's simulated
                            // here in the Bot. This matches the behavior in the Node connector.
                            int delayMs = (int)((Activity)activity).Value;
                            await Task.Delay(delayMs, cancellationToken).ConfigureAwait(false);
                        }

                        break;

                    case ActivityTypes.Trace:
                        // Do not send trace activities unless you know that the client needs them.
                        // For example: BF protocol only sends Trace Activity when talking to emulator channel.
                        break;

                    default:
                        Console.WriteLine("Bot: activity type: {0}", activity.Type);
                        break;
                }

                responses[index] = new ResourceResponse(activity.Id);
            }

            return responses;
        }

        // Normally, replaces an existing activity in the conversation.
        // Not implemented for this sample.
        public override Task<ResourceResponse> UpdateActivityAsync(ITurnContext turnContext, Activity activity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        // Deletes an existing activity in the conversation.
        // Not implemented for this sample.
        public override Task DeleteActivityAsync(ITurnContext turnContext, ConversationReference reference, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

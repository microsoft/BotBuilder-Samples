// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples.FacebookModel;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class FacebookBot : ActivityHandler
    {
        // These are the options provided to the user when they message the bot
        const string FacebookPageIdOption = "Facebook Id";
        const string QuickRepliesOption = "Quick Replies";
        const string PostBackOption = "PostBack";

        protected readonly ILogger Logger;

        public FacebookBot(ILogger<FacebookBot> logger)
        {
            Logger = logger;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Processing a Message Activity.");

            // Show choices if the Facebook Payload from ChannelData is not handled
            if (!await ProcessFacebookPayload(turnContext, turnContext.Activity.ChannelData, cancellationToken))
                await ShowChoices(turnContext, cancellationToken);
        }

        protected override async Task OnEventActivityAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Processing an Event Activity.");

            // Analyze Facebook payload from EventActivity.Value
            await ProcessFacebookPayload(turnContext, turnContext.Activity.Value, cancellationToken);
        }

        private static async Task ShowChoices(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            // Create choices for the prompt
            var choices = new List<Choice>();
            choices.Add(new Choice() { Value = QuickRepliesOption, Action = new CardAction() { Title = QuickRepliesOption, Type = ActionTypes.PostBack, Value = QuickRepliesOption } });
            choices.Add(new Choice() { Value = FacebookPageIdOption, Action = new CardAction() { Title = FacebookPageIdOption, Type = ActionTypes.PostBack, Value = FacebookPageIdOption } });
            choices.Add(new Choice() { Value = PostBackOption, Action = new CardAction() { Title = PostBackOption, Type = ActionTypes.PostBack, Value = PostBackOption } });

            // Create the prompt message
            var message = ChoiceFactory.ForChannel(turnContext.Activity.ChannelId, choices, "What Facebook feature would you like to try? Here are some quick replies to choose from!");
            await turnContext.SendActivityAsync(message, cancellationToken);
        }

        private async Task<bool> ProcessFacebookPayload(ITurnContext turnContext, object data, CancellationToken cancellationToken)
        {
            // At this point we know we are on Facebook channel, and can consume the Facebook custom payload
            // present in channelData.
            try
            {
                var facebookPayload = (data as JObject)?.ToObject<FacebookPayload>();

                if (facebookPayload != null)
                {
                    // PostBack
                    if (facebookPayload.PostBack != null)
                    {
                        await OnFacebookPostBack(turnContext, facebookPayload.PostBack, cancellationToken);
                        return true;
                    }

                    // Optin
                    else if (facebookPayload.Optin != null)
                    {
                        await OnFacebookOptin(turnContext, facebookPayload.Optin, cancellationToken);
                        return true;
                    }

                    // Quick reply
                    else if (facebookPayload.Message?.QuickReply != null)
                    {
                        await OnFacebookQuickReply(turnContext, facebookPayload.Message.QuickReply, cancellationToken);
                        return true;
                    }

                    // Echo
                    else if (facebookPayload.Message?.IsEcho != null && facebookPayload.Message.IsEcho)
                    {
                        await OnFacebookEcho(turnContext, facebookPayload.Message, cancellationToken);
                        return true;
                    }

                    // TODO: Handle other events that you're interested in...
                }
            }
            catch(JsonSerializationException e)
            {
                if (turnContext.Activity.ChannelId != Bot.Connector.Channels.Facebook)
                {
                    await turnContext.SendActivityAsync("This sample is intended to be used with a Facebook bot.");
                }
                else
                {
                    throw;
                }
            }
          
            return false;
        }

        protected virtual async Task OnFacebookOptin(ITurnContext turnContext, FacebookOptin optin, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Optin message received.");

            // TODO: Your optin event handling logic here...
        }

        protected virtual async Task OnFacebookEcho(ITurnContext turnContext, FacebookMessage facebookMessage, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Echo message received.");

            // TODO: Your echo event handling logic here...
        }

        protected virtual async Task OnFacebookPostBack(ITurnContext turnContext, FacebookPostback postBack, CancellationToken cancellationToken)
        {
            Logger.LogInformation("PostBack message received.");

            // TODO: Your PostBack handling logic here...

            // Answer the postback, and show choices
            var reply = MessageFactory.Text("Are you sure?");
            await turnContext.SendActivityAsync(reply, cancellationToken);
            await ShowChoices(turnContext, cancellationToken);
        }

        protected virtual async Task OnFacebookQuickReply(ITurnContext turnContext, FacebookQuickReply quickReply, CancellationToken cancellationToken)
        {
            Logger.LogInformation("QuickReply message received.");

            // TODO: Your quick reply event handling logic here...

            // Process the message by checking the Activity.Text.  The FacebookQuickReply could also contain a json payload.

            // Initially the bot offers to showcase 3 Facebook features: Quick replies, PostBack and getting the Facebook Page Name.
            switch (turnContext.Activity.Text)
            {
                // Here we showcase how to obtain the Facebook page id.
                // This can be useful for the Facebook multi-page support provided by the Bot Framework.
                // The Facebook page id from which the message comes from is in turnContext.Activity.Recipient.Id.
                case FacebookPageIdOption:
                    {
                        var reply = MessageFactory.Text($"This message comes from the following Facebook Page: {turnContext.Activity.Recipient.Id}");
                        await turnContext.SendActivityAsync(reply, cancellationToken);
                        await ShowChoices(turnContext, cancellationToken);

                        break;
                    }

                // Here we send a HeroCard with 2 options that will trigger a Facebook PostBack.
                case PostBackOption:
                    {
                        var card = new HeroCard
                        {
                            Text = "Is 42 the answer to the ultimate question of Life, the Universe, and Everything?",
                            Buttons = new List<CardAction>
                                    {
                                        new CardAction() { Title = "Yes", Type = ActionTypes.PostBack, Value = "Yes" },
                                        new CardAction() { Title = "No", Type = ActionTypes.PostBack, Value = "No" },
                                    },
                        };
                        
                        var reply = MessageFactory.Attachment(card.ToAttachment());
                        await turnContext.SendActivityAsync(reply, cancellationToken);

                        break;
                    }

                // By default we offer the users different actions that the bot supports, through quick replies.
                case QuickRepliesOption:
                default:
                    {
                        await ShowChoices(turnContext, cancellationToken);

                        break;
                    }
            }
        }
    }
}

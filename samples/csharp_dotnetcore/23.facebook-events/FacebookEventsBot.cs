// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Facebook_Events_Bot.FacebookModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace Facebook_Events_Bot
{
    /// <summary>
    /// Represents a bot that processes incoming activities.
    /// For each interaction from the user, an instance of this class is called.
    /// This is a Transient lifetime service. Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single Turn, should be carefully managed.
    /// For example, the <see cref="MemoryStorage"/> object and associated
    /// <see cref="IStatePropertyAccessor{T}"/> object are created with a singleton lifetime.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    public class FacebookEventsBot : IBot
    {
        private const string DialogId = "question";

        /// <summary>
        /// The <see cref="DialogSet"/> that contains all the Dialogs that can be used at runtime.
        /// </summary>
        private readonly DialogSet _dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookEventsBot"/> class.
        /// </summary>
        /// <param name="accessors">The state accessors this instance will be needing at runtime.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1#windows-eventlog-provider"/>
        public FacebookEventsBot(BotAccessors accessors)
        {
            if (accessors == null)
            {
                throw new ArgumentNullException(nameof(accessors));
            }

            _dialogs = new DialogSet(accessors.ConversationDialogState);
            _dialogs.Add(new AttachmentPrompt(DialogId));
        }

        /// <summary>
        /// Every conversation turn for our NLP Dispatch Bot will call this method.
        /// There are no dialogs used, since it's "single turn" processing, meaning a single
        /// request and response, with no stateful conversation.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            const string facebookPageNameOption = "Facebook Page Name";
            const string quickRepliesOption = "Quick Replies";
            const string postBackOption = "PostBack";

            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            // Check if we are on the Facebook channel.
            if (turnContext.Activity.ChannelId == Channel.Channels.Facebook)
            {
                // Analyze Facebook payload from channel data.
                ProcessFacebookPayload(turnContext.Activity.ChannelData);

                // Initially the bot offers to showcase 3 Facebook features: Quick replies, PostBack and getting the Facebook Page Name.
                // Below we also show how to get the messaging_optin payload separately as well.
                switch (turnContext.Activity.Text)
                {
                    // Here we showcase how to obtain the Facebook page name.
                    // This can be useful for the Facebook multi-page support provided by the Bot Framework.
                    // The Facebook page name from which the message comes from is in turnContext.Activity.Recipient.Name.
                    case facebookPageNameOption:
                        {
                            var reply = turnContext.Activity.CreateReply($"This message comes from the following Facebook Page: {turnContext.Activity.Recipient.Name}");
                            await turnContext.SendActivityAsync(reply);
                            break;
                        }

                    // Here we send a HeroCard with 2 options that will trigger a Facebook PostBack.
                    case postBackOption:
                        {
                            var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
                            var results = await dialogContext.ContinueDialogAsync(cancellationToken);

                            var card = new HeroCard
                            {
                                Text = "Is 42 the answer to the ultimate question of Life, the Universe, and Everything?",
                                Buttons = new List<CardAction>
                                    {
                                        new CardAction() { Title = "Yes", Type = ActionTypes.PostBack, Value = "Yes" },
                                        new CardAction() { Title = "No", Type = ActionTypes.PostBack, Value = "No" },
                                    },
                            };

                            var reply = turnContext.Activity.CreateReply();
                            reply.Attachments = new List<Attachment> { card.ToAttachment() };
                            await turnContext.SendActivityAsync(reply);

                            break;
                        }

                    // By default we offer the users different actions that the bot supports, through quick replies.
                    case quickRepliesOption:
                    default:
                        {
                            var reply = turnContext.Activity.CreateReply("What Facebook feature would you like to try? Here are some quick replies to choose from!");
                            reply.SuggestedActions = new SuggestedActions()
                            {
                                Actions = new List<CardAction>()
                                {
                                    new CardAction() { Title = quickRepliesOption, Type = ActionTypes.PostBack, Value = quickRepliesOption },
                                    new CardAction() { Title = facebookPageNameOption, Type = ActionTypes.PostBack, Value = facebookPageNameOption },
                                    new CardAction() { Title = postBackOption, Type = ActionTypes.PostBack, Value = postBackOption },
                                },
                            };
                            await turnContext.SendActivityAsync(reply);
                            break;
                        }
                }
            }
            else
            {
                // Check if we are on the Facebook channel.
                if (turnContext.Activity.ChannelId == Channel.Channels.Facebook)
                {
                    // Here we can check for messaging_optin webhook event.
                    // Facebook Documentation for Message optin:
                    // https://developers.facebook.com/docs/messenger-platform/reference/webhook-events/messaging_optins/
                }

                await turnContext.SendActivityAsync($"Received activity of type {turnContext.Activity.Type}.");
            }
        }

        /// <summary>
        /// Process a Facebook payload from channel data, to handle optin events, postbacks and quick replies.
        /// NOTE: This is a simplification of the Facebook object model. There are many more events and payloads
        /// that could be captured here. We only show some key features that are commonly used. The <see cref="FacebookPayload"/> class
        /// can be extended to account for more complete models according to developers' needs.
        /// </summary>
        /// <param name="channelData">The activity channel data.</param>
        private void ProcessFacebookPayload(object channelData)
        {
            // At this point we know we are on Facebook channel, and can consume the Facebook custom payload
            // present in channelData.
            var facebookPayload = (channelData as JObject)?.ToObject<FacebookPayload>();

            if (facebookPayload != null)
            {
                // PostBack
                if (facebookPayload.PostBack != null)
                {
                    OnFacebookPostBack(facebookPayload.PostBack);
                }

                // Optin
                else if (facebookPayload.Optin != null)
                {
                    OnFacebookOptin(facebookPayload.Optin);
                }

                // Quick reply
                else if (facebookPayload.Message?.QuickReply != null)
                {
                    OnFacebookQuickReply(facebookPayload.Message.QuickReply);
                }

                // TODO: Handle other events that you're interested in...
            }
        }

        private void OnFacebookOptin(FacebookOptin optin)
        {
            // TODO: Your optin event handling logic here...
        }

        private void OnFacebookPostBack(FacebookPostback postBack)
        {
            // TODO: Your PostBack handling logic here...
        }

        private void OnFacebookQuickReply(FacebookQuickReply quickReply)
        {
            // TODO: Your quick reply event handling logic here...
        }
    }
}

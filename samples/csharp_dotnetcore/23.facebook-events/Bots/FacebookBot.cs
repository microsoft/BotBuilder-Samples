// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples.FacebookModel;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Bots
{
    // This IBot implementation can run any type of Dialog. The use of type parameterization is to allows multiple different bots
    // to be run at different endpoints within the same project. This can be achieved by defining distinct Controller types
    // each with dependency on distinct IBot types, this way ASP Dependency Injection can glue everything together without ambiguity.
    // The ConversationState is used by the Dialog system. The UserState isn't, however, it might have been used in a Dialog implementation,
    // and the requirement is that all BotState objects are saved at the end of a turn.
    public class FacebookBot<T> : ActivityHandler where T : Dialog
    {
        protected readonly Dialog Dialog;
        protected readonly BotState ConversationState;
        protected readonly ILogger Logger;

        public FacebookBot(ConversationState conversationState, T dialog, ILogger<FacebookBot<T>> logger)
        {
            ConversationState = conversationState;
            Dialog = dialog;
            Logger = logger;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Running dialog with Message Activity.");

            // Run the Dialog with the new message Activity.
            await Dialog.Run(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }

        protected override Task OnEventActivityAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            // Analyze Facebook payload from channel data.
            ProcessFacebookPayload(turnContext.Activity.ChannelData);

            return base.OnEventActivityAsync(turnContext, cancellationToken);
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

        protected virtual void OnFacebookOptin(FacebookOptin optin)
        {
            // TODO: Your optin event handling logic here...
        }

        protected virtual void OnFacebookPostBack(FacebookPostback postBack)
        {
            // TODO: Your PostBack handling logic here...
        }

        protected virtual void OnFacebookQuickReply(FacebookQuickReply quickReply)
        {
            // TODO: Your quick reply event handling logic here...
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Bots
{
    /*
     * After installing this bot you will need to click on the 3 dots to pull up the extension menu to select the bot. Once you do you do 
     * see the extension pop a task module.
     */

    public class ActionBasedMessagingExtensionFetchTaskBot : TeamsActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"echo: {turnContext.Activity.Text}"), cancellationToken);
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionFetchTaskAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionQuery query, CancellationToken cancellationToken)
        {
            return AdaptiveCardHelper.CreateTaskModuleAdaptiveCardResponse();
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            var submittedData = JsonConvert.DeserializeObject<SubmitExampleData>(action.Data.ToString());
            var adaptiveCard = submittedData.ToAdaptiveCard();
            return adaptiveCard.ToMessagingExtensionBotMessagePreviewResponse();
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionBotMessagePreviewEditAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            var submitData = action.ToSubmitExampleData();
            return AdaptiveCardHelper.CreateTaskModuleAdaptiveCardResponse(
                                                        submitData.Question,
                                                        bool.Parse(submitData.MultiSelect),
                                                        submitData.Option1,
                                                        submitData.Option2,
                                                        submitData.Option3);
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionBotMessagePreviewSendAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            var submitData = action.ToSubmitExampleData();
            var adaptiveCard = submitData.ToAdaptiveCard();
            var responseActivity = Activity.CreateMessageActivity();
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = adaptiveCard,
            };
            responseActivity.Attachments.Add(attachment);
            try
            {
                // Send to channel where messaging extension invoked.
                var channelId = turnContext.Activity.TeamsGetChannelId();
                await turnContext.TeamsCreateConversationAsync(channelId, responseActivity);

                // Send card to "General" channel.
                var teamDetails = await TeamsInfo.GetTeamDetailsAsync(turnContext);
                await turnContext.TeamsCreateConversationAsync(teamDetails.Id, responseActivity);
            }
            catch (Exception ex)
            {
                // In group chat or personal scope..
                await turnContext.SendActivityAsync($"In Group Chat or Personal Teams scope. Sending card to compose-only.");
            }

            return adaptiveCard.ToComposeExtensionResultResponse();
        }

        protected override async Task OnTeamsMessagingExtensionCardButtonClickedAsync(ITurnContext<IInvokeActivity> turnContext, JObject obj, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("OnTeamsMessagingExtensionCardButtonClickedAsync Value: " + JsonConvert.SerializeObject(turnContext.Activity.Value));
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
    }
}

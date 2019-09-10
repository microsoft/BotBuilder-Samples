// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams.Internal;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class CardActionsBot : TeamsActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            if (turnContext.Activity.Text != null)
            {
                var teamsContext = new TeamsContext(turnContext, null);
                string actualText = teamsContext.GetActivityTextWithoutMentions();

                if (actualText.Equals("1"))
                {
                    var adaptiveCard = new AdaptiveCard();
                    adaptiveCard.Body.Add(new AdaptiveTextBlock("Bot Builder actions"));

                    var action1 = new CardAction(ActionTypes.ImBack, "imBack", null, null, null, "text");
                    var action2 = new CardAction(ActionTypes.MessageBack, "message back", null, null, null, JObject.Parse(@"{ ""key"" : ""value"" }"));
                    var action3 = new CardAction(ActionTypes.MessageBack, "message back local echo", null, "text received by bots", "display text message back", JObject.Parse(@"{ ""key"" : ""value"" }"));
                    var action4 = new CardAction("invoke", "invoke", null, null, null, JObject.Parse(@"{ ""key"" : ""value"" }"));
                    adaptiveCard.Actions.Add(action1.ToAdaptiveCardAction());
                    adaptiveCard.Actions.Add(action2.ToAdaptiveCardAction());
                    adaptiveCard.Actions.Add(action3.ToAdaptiveCardAction());
                    adaptiveCard.Actions.Add(action4.ToAdaptiveCardAction());

                    var replyActivity = MessageFactory.Attachment(adaptiveCard.ToAttachment());
                    await turnContext.SendActivityAsync(replyActivity, cancellationToken);
                }
                else if (actualText.Equals("2"))
                {
                    var taskModuleAction = new TaskModuleAction("Launch Task Module", @"{ ""hiddenKey"": ""hidden value from task module launcher"" }");

                    var adaptiveCard = new AdaptiveCard();
                    adaptiveCard.Body.Add(new AdaptiveTextBlock("Task Module Adaptive Card"));
                    adaptiveCard.Actions.Add(taskModuleAction.ToAdaptiveCardAction());

                    var replyActivity = MessageFactory.Attachment(adaptiveCard.ToAttachment());
                    await turnContext.SendActivityAsync(replyActivity, cancellationToken);
                }
                else if (actualText.Equals("3"))
                {
                    var adaptiveCard = new AdaptiveCard();
                    adaptiveCard.Body.Add(new AdaptiveTextBlock("Bot Builder actions"));
                    adaptiveCard.Body.Add(new AdaptiveTextInput { Id = "x" });
                    adaptiveCard.Actions.Add(new AdaptiveSubmitAction { Type = "Action.Submit", Title = "Action.Submit", Data = new JObject { { "key", "value" } } });

                    var replyActivity = MessageFactory.Attachment(adaptiveCard.ToAttachment());
                    await turnContext.SendActivityAsync(replyActivity, cancellationToken);
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"You said: {turnContext.Activity.Text}"), cancellationToken);
                }
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"App sent a message with null text"), cancellationToken);

                // TODO: process the response from the card

                if (turnContext.Activity.Value != null)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"but with value {turnContext.Activity.Value}"), cancellationToken);
                }
            }
        }

        protected override async Task<InvokeResponse> OnInvokeActivityAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Name != null)
            {
                return await base.OnInvokeActivityAsync(turnContext, cancellationToken);
            }

            // TODO: process the response from the card

            return new InvokeResponse { Status = 200 };
        }
    }
}

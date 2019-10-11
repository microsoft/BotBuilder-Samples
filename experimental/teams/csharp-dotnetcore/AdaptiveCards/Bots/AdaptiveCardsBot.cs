// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
    public class AdaptiveCardsBot : TeamsActivityHandler
    {
        /*
         * You can @mention the bot the text "1", "2", or "3". "1" will send back adaptive cards. "2" will send back a 
         * task module that contains an adpative card. "3" will return an adpative card that contains BF card actions.
         * 
         */
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            if (turnContext.Activity.Text != null)
            {
                turnContext.Activity.RemoveRecipientMention();
                string actualText = turnContext.Activity.Text;

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

        protected override async Task<TaskModuleTaskInfo> OnTeamsTaskModuleFetchAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("OnTeamsTaskModuleFetchAsync TaskModuleRequest: " + JsonConvert.SerializeObject(taskModuleRequest));
            await turnContext.SendActivityAsync(reply, cancellationToken);

            var adaptiveCard = new AdaptiveCard();
            adaptiveCard.Body.Add(new AdaptiveTextBlock("This is an Adaptive Card within a Task Module"));
            adaptiveCard.Actions.Add(new AdaptiveSubmitAction { Type = "Action.Submit", Title = "Action.Submit", Data = new JObject { { "submitLocation", "taskModule" } } });

            return new TaskModuleTaskInfo()
            {
                Card = adaptiveCard.ToAttachment(),
                Height = 200,
                Width = 400,
                Title = "Task Module Example",
            };
        }

        protected override async Task<TaskModuleResponseBase> OnTeamsTaskModuleSubmitAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"OnTeamsTaskModuleSubmitAsync value: { JsonConvert.SerializeObject(taskModuleRequest) }"), cancellationToken);
            return new TaskModuleMessageResponse { Value = "Thanks!" };
        }

        protected override async Task<InvokeResponse> OnTeamsCardActionInvokeAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"OnTeamsCardActionInvokeAsync value: {turnContext.Activity.Value}"), cancellationToken);
            return new InvokeResponse() { Status = 200 };
        }
    }
}

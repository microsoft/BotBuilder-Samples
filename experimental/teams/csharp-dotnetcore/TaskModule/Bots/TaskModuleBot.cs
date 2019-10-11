// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.Bots
{
    /*
     * This bot can be installed at any scope. If you @mention the bot you will get the task module. You can interact with the task module to
     * hit the other functions.
     */
    public class TaskModuleBot : TeamsActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Attachment(this.GetTaskModuleHeroCard());
            await turnContext.SendActivityAsync(reply);
        }

        protected override async Task<TaskModuleTaskInfo> OnTeamsTaskModuleFetchAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("OnTeamsTaskModuleFetchAsync TaskModuleRequest: " + JsonConvert.SerializeObject(taskModuleRequest));
            await turnContext.SendActivityAsync(reply);

            return new TaskModuleTaskInfo()
            {
                Card = this.GetTaskModuleAdaptiveCard(),
                Height = 200,
                Width = 400,
                Title = "Adaptive Card: Inputs",
            };
        }

        protected override async Task<TaskModuleResponseBase> OnTeamsTaskModuleSubmitAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("OnTeamsTaskModuleFetchAsync Value: " + JsonConvert.SerializeObject(taskModuleRequest));
            await turnContext.SendActivityAsync(reply);

            return new TaskModuleMessageResponse() { Value = "Thanks!" };
        }

        private Attachment GetTaskModuleHeroCard()
        {
            return new HeroCard()
            {
                Title = "Task Module Invocation from Hero Card",
                Subtitle = "This is a hero card with a Task Module Action button.  Click the button to show an Adaptive Card within a Task Module.",
                Buttons = new List<CardAction>()
                    {
                        new TaskModuleAction("Adaptive Card", new { data = "adaptivecard" }),
                    },
            }.ToAttachment();
        }

        private Attachment GetTaskModuleAdaptiveCard()
        {
            var card = new AdaptiveCard(new AdaptiveSchemaVersion("1.0"))
            {
                Body = new List<AdaptiveElement>()
                {
                    new AdaptiveTextBlock() { Text = "Enter Text Here" },
                    new AdaptiveTextInput()
                    {
                        Id = "usertext",
                        Spacing = AdaptiveSpacing.None,
                        IsMultiline = true,
                        Placeholder = "add some text and submit",
                    },
                },
                Actions = new List<AdaptiveAction>()
                {
                    new AdaptiveSubmitAction() { Title = "Submit" },
                },
            };

            return new Attachment
            {
                Content = card,
                ContentType = AdaptiveCards.AdaptiveCard.ContentType,
            };
        }
    }
}

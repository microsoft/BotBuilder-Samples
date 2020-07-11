// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.DependencyInjection;
using TaskModuleFactorySample.Dialogs.Helper;
using TaskModuleFactorySample.Services;

namespace TaskModuleFactorySample.Dialogs
{
    public class SampleDialog : SkillDialogBase
    {
        private readonly BotSettings _botSettings;

        public SampleDialog(
            IServiceProvider serviceProvider)
            : base(nameof(SampleDialog), serviceProvider)
        {
            _botSettings = serviceProvider.GetService<BotSettings>();
            var sample = new WaterfallStep[]
            {
                // NOTE: Uncomment these lines to include authentication steps to this dialog
                // GetAuthTokenAsync,
                // AfterGetAuthTokenAsync,
                CreateTicketTeamsTaskModuleAsync,
                EndAsync,
            };

            AddDialog(new WaterfallDialog(nameof(SampleDialog), sample));
            AddDialog(new TextPrompt(DialogIds.NamePrompt));

            InitialDialogId = nameof(SampleDialog);
        }

        private async Task<DialogTurnResult> PromptForNameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // NOTE: Uncomment the following lines to access LUIS result for this turn.
            // var luisResult = stepContext.Context.TurnState.Get<LuisResult>(StateProperties.SkillLuisResult);
            var prompt = TemplateEngine.GenerateActivityForLocale("NamePrompt");
            return await stepContext.PromptAsync(DialogIds.NamePrompt, new PromptOptions { Prompt = prompt }, cancellationToken);
        }

        private async Task<DialogTurnResult> GreetUserAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            dynamic data = new { Name = stepContext.Result.ToString() };
            var response = TemplateEngine.GenerateActivityForLocale("HaveNameMessage", data);
            await stepContext.Context.SendActivityAsync(response);

            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        protected async Task<DialogTurnResult> CreateTicketTeamsTaskModuleAsync(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Send Create Ticket TaskModule Card
            var reply = sc.Context.Activity.CreateReply();
            reply.Attachments = new List<Attachment>()
            {
                new Microsoft.Bot.Schema.Attachment() { ContentType = AdaptiveCard.ContentType, Content = TicketDialogHelper.GetUserInputIncidentCard(_botSettings.MicrosoftAppId) }
            };

            // Get ActivityId for purpose of mapping
            ResourceResponse resourceResponse = await sc.Context.SendActivityAsync(reply, cancellationToken);
          
            return await sc.EndDialogAsync();
        }

        private Task<DialogTurnResult> EndAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private static class DialogIds
        {
            public const string NamePrompt = "namePrompt";
        }
    }
}

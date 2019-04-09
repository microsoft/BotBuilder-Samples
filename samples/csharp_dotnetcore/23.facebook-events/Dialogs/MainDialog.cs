// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        // These are the options provided to the user when they message the bot
        const string FacebookPageNameOption = "Facebook Page Name";
        const string QuickRepliesOption = "Quick Replies";
        const string PostBackOption = "PostBack";

        protected readonly IConfiguration Configuration;
        protected readonly ILogger Logger;

        public MainDialog(IConfiguration configuration, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            Configuration = configuration;
            Logger = logger;

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ChoiceStepAsync,
                ShowResultStep,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ChoiceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Logger.LogInformation("MainDialog.ChoiceStepAsync");
            var turnContext = stepContext.Context;

            var text = turnContext.Activity.Text;
            // If the text is one of the quick reply options, continue to the ShowResultStep
            if (new[] { QuickRepliesOption, FacebookPageNameOption, PostBackOption }.Contains(text))
            {
                return await stepContext.NextAsync();
            }
            else
            {
                return await ShowChoices(stepContext, cancellationToken);
            }
        }

        private static async Task<DialogTurnResult> ShowChoices(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Create options for the prompt
            var options = new PromptOptions()
            {
                Prompt = stepContext.Context.Activity.CreateReply("What Facebook feature would you like to try? Here are some quick replies to choose from!"),
                Choices = new List<Choice>(),
            };

            // Add the choices for the prompt.
            options.Choices.Add(new Choice() { Value = QuickRepliesOption, Action = new CardAction() { Title = QuickRepliesOption, Type = ActionTypes.PostBack, Value = QuickRepliesOption } });
            options.Choices.Add(new Choice() { Value = FacebookPageNameOption, Action = new CardAction() { Title = FacebookPageNameOption, Type = ActionTypes.PostBack, Value = FacebookPageNameOption } });
            options.Choices.Add(new Choice() { Value = PostBackOption, Action = new CardAction() { Title = PostBackOption, Type = ActionTypes.PostBack, Value = PostBackOption } });

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
        }

        private async Task<DialogTurnResult> ShowResultStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Logger.LogInformation("MainDialog.ShowResultStep");
            var turnContext = stepContext.Context;

            // Initially the bot offers to showcase 3 Facebook features: Quick replies, PostBack and getting the Facebook Page Name.
            switch (turnContext.Activity.Text)
            {
                // Here we showcase how to obtain the Facebook page name.
                // This can be useful for the Facebook multi-page support provided by the Bot Framework.
                // The Facebook page name from which the message comes from is in turnContext.Activity.Recipient.Name.
                case FacebookPageNameOption:
                    {
                        var reply = turnContext.Activity.CreateReply($"This message comes from the following Facebook Page: {turnContext.Activity.Recipient.Name}");
                        await turnContext.SendActivityAsync(reply);
                        await ShowChoices(stepContext, cancellationToken);

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

                        var reply = turnContext.Activity.CreateReply();
                        reply.Attachments = new List<Attachment> { card.ToAttachment() };
                        await turnContext.SendActivityAsync(reply);

                        break;
                    }

                // By default we offer the users different actions that the bot supports, through quick replies.
                case QuickRepliesOption:
                default:
                    {
                        await ShowChoices(stepContext, cancellationToken);

                        break;
                    }
            }
            return await stepContext.EndDialogAsync();
        }
    }
}

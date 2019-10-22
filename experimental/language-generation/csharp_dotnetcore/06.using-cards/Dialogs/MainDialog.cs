// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Bot.Builder.LanguageGeneration;
using System.IO;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;
using ActivityBuilder = Microsoft.Bot.Builder.Dialogs.Adaptive.Generators.ActivityGenerator;

namespace Microsoft.BotBuilderSamples
{
    public class MainDialog : ComponentDialog
    {
        protected readonly ILogger _logger;
        private TemplateEngine _lgEngine;
        
        public MainDialog(ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _logger = logger;

            // combine path for cross platform support
            string[] paths = { ".", "Resources", "MainDialog.LG" };
            string mainDialogLGFile = Path.Combine(paths);
            paths = new string[] { ".", "Resources", "Cards.LG" };
            string cardsLGFile = Path.Combine(paths);
            string[] lgFiles = { mainDialogLGFile, cardsLGFile };

            // For simple LG resolution, we will call the TemplateEngine directly. 
            _lgEngine = new TemplateEngine().AddFiles(lgFiles);

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ChoiceCardStepAsync,
                ShowCardStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ChoiceCardStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("MainDialog.ChoiceCardStepAsync");

            // Create options for the prompt
            var options = new PromptOptions()
            {
                Prompt = ActivityBuilder.GenerateFromLG(_lgEngine.EvaluateTemplate("CardChoice")),
                Choices = new List<Choice>(),
            };

            // Add the choices for the prompt.
            options.Choices.Add(new Choice() { Value = "Adaptive card" });
            options.Choices.Add(new Choice() { Value = "Animation card" });
            options.Choices.Add(new Choice() { Value = "Audio card" });
            options.Choices.Add(new Choice() { Value = "Hero card" });
            options.Choices.Add(new Choice() { Value = "Receipt card" });
            options.Choices.Add(new Choice() { Value = "Signin card" });
            options.Choices.Add(new Choice() { Value = "Thumbnail card" });
            options.Choices.Add(new Choice() { Value = "Video card" });
            options.Choices.Add(new Choice() { Value = "All cards" });

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
        }

        private async Task<DialogTurnResult> ShowCardStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("MainDialog.ShowCardStepAsync");

            // Reply to the activity we received with an activity.
            var reply = stepContext.Context.Activity.CreateReply();

            // Cards are sent as Attachments in the Bot Framework.
            // So we need to create a list of attachments on the activity.
            reply.Attachments = new List<Attachment>();

            // Get the text from the activity to use to show the correct card
            var text = stepContext.Context.Activity.Text.ToLowerInvariant().Split(' ')[0];

            // Decide which type of card(s) we are going to show the user
            if (text.StartsWith("hero"))
            {
                // Display a HeroCard.
                await stepContext.Context.SendActivityAsync(ActivityBuilder.GenerateFromLG(_lgEngine.EvaluateTemplate("HeroCard")));
            }
            else if (text.StartsWith("thumb"))
            {
                // Display a ThumbnailCard.
                await stepContext.Context.SendActivityAsync(ActivityBuilder.GenerateFromLG(_lgEngine.EvaluateTemplate("ThumbnailCard")));
            }
            else if (text.StartsWith("sign"))
            {
                // Display a SignInCard.
                await stepContext.Context.SendActivityAsync(ActivityBuilder.GenerateFromLG(_lgEngine.EvaluateTemplate("SigninCard")));
            }
            else if (text.StartsWith("animation"))
            {
                // Display an AnimationCard.
                await stepContext.Context.SendActivityAsync(ActivityBuilder.GenerateFromLG(_lgEngine.EvaluateTemplate("AnimationCard")));
            }
            else if (text.StartsWith("video"))
            {
                // Display a VideoCard
                await stepContext.Context.SendActivityAsync(ActivityBuilder.GenerateFromLG(_lgEngine.EvaluateTemplate("VideoCard")));
            }
            else if (text.StartsWith("audio"))
            {
                // Display an AudioCard
                await stepContext.Context.SendActivityAsync(ActivityBuilder.GenerateFromLG(_lgEngine.EvaluateTemplate("AudioCard")));
            }
            else if (text.StartsWith("receipt"))
            {
                // Display a ReceiptCard.
                reply.Attachments.Add(Cards.GetReceiptCard().ToAttachment());
                // Send the card(s) to the user as an attachment to the activity
                await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            }
            else if (text.StartsWith("adaptive"))
            {
                await stepContext.Context.SendActivityAsync(ActivityBuilder.GenerateFromLG(_lgEngine.EvaluateTemplate("AdaptiveCard")));
            }
            else
            {
                // Display a carousel of all the rich card types.
                await stepContext.Context.SendActivityAsync(ActivityBuilder.GenerateFromLG(_lgEngine.EvaluateTemplate("AllCards")));
            }

            // Give the user instructions about what to do next
            await stepContext.Context.SendActivityAsync(ActivityBuilder.GenerateFromLG(_lgEngine.EvaluateTemplate("CardStartOverResponse")), cancellationToken);

            return await stepContext.EndDialogAsync();
        }
    }
}

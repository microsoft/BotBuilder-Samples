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
            string fullPath = Path.Combine(paths);
            _lgEngine = TemplateEngine.FromFiles(fullPath);
            paths = new string[] { ".", "Resources", "Cards.LG" };
            fullPath = Path.Combine(paths);
            _lgEngine = TemplateEngine.FromFiles(fullPath);
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
                Prompt = stepContext.Context.Activity.CreateReply(_lgEngine.EvaluateTemplate("CardChoice", null)),
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

            // Get the text from the activity to use to show the correct card
            var text = stepContext.Context.Activity.Text.ToLowerInvariant().Split(' ')[0];

            // Decide which type of card(s) we are going to show the user
            if (text.StartsWith("hero"))
            {
                // Display a HeroCard.
                //await stepContext.Context.SendActivitiesAsync()
            }
            else if (text.StartsWith("thumb"))
            {
                // Display a ThumbnailCard.
                //reply.Attachments.Add(Cards.GetThumbnailCard().ToAttachment());
            }
            else if (text.StartsWith("receipt"))
            {
                // Display a ReceiptCard.
                //reply.Attachments.Add(Cards.GetReceiptCard().ToAttachment());
            }
            else if (text.StartsWith("sign"))
            {
                // Display a SignInCard.
                //reply.Attachments.Add(Cards.GetSigninCard().ToAttachment());
            }
            else if (text.StartsWith("animation"))
            {
                // Display an AnimationCard.
                //reply.Attachments.Add(Cards.GetAnimationCard().ToAttachment());
            }
            else if (text.StartsWith("video"))
            {
                // Display a VideoCard
                //reply.Attachments.Add(Cards.GetVideoCard().ToAttachment());
            }
            else if (text.StartsWith("audio"))
            {
                // Display an AudioCard
                //reply.Attachments.Add(Cards.GetAudioCard().ToAttachment());
            }
            else if (text.StartsWith("adaptive"))
            {
                //reply.Attachments.Add(Cards.CreateAdaptiveCardAttachment());
            }
            else
            {
                // Display a carousel of all the rich card types.
                //reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                //reply.Attachments.Add(Cards.CreateAdaptiveCardAttachment());
                //reply.Attachments.Add(Cards.GetHeroCard().ToAttachment());
                //reply.Attachments.Add(Cards.GetThumbnailCard().ToAttachment());
                //reply.Attachments.Add(Cards.GetReceiptCard().ToAttachment());
                //reply.Attachments.Add(Cards.GetSigninCard().ToAttachment());
                //reply.Attachments.Add(Cards.GetAnimationCard().ToAttachment());
                //reply.Attachments.Add(Cards.GetVideoCard().ToAttachment());
                //reply.Attachments.Add(Cards.GetAudioCard().ToAttachment());
            }

            // Send the card(s) to the user as an attachment to the activity
            //await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            // Give the user instructions about what to do next
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(_lgEngine.EvaluateTemplate("CardStartOverResponse", null)), cancellationToken);

            return await stepContext.EndDialogAsync();
        }
    }
}

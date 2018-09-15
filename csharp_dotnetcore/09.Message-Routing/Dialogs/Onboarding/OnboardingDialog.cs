// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace MessageRoutingBot
{
    /// <summary>
    /// Onboarding dialog to gather user information, such as name, email, etc. This dialog is interruptable.
    /// </summary>
    public class OnboardingDialog : RoutingSampleDialog
    {
        // Constants
        public const string Name = "onboarding";
        public const string NamePrompt = "namePrompt";
        public const string EmailPrompt = "emailPrompt";
        public const string LocationPrompt = "locationPrompt";

        // Fields
        private static OnboardingResponses _responder = new OnboardingResponses();

        /// <summary>
        /// Initializes a new instance of the <see cref="OnboardingDialog"/> class.
        /// </summary>
        /// <param name="botServices">The <see cref="BotServices"/> for the bot.</param>
        public OnboardingDialog(BotServices botServices)
            : base(botServices, Name)
        {
            var onboarding = new WaterfallStep[]
            {
                AskForName,
                AskForEmail,
                AskForLocation,
                FinishOnboardingDialog,
            };

            AddDialog(new WaterfallDialog(Name, onboarding));
            AddDialog(new TextPrompt(NamePrompt));
            AddDialog(new TextPrompt(EmailPrompt));
            AddDialog(new TextPrompt(LocationPrompt));
        }

        private async Task<DialogTurnResult> AskForName(DialogContext dc, WaterfallStepContext args, CancellationToken cancellationToken)
        {
            return await dc.PromptAsync(NamePrompt, new PromptOptions()
            {
                Prompt = await _responder.RenderTemplate(dc.Context, "en", OnboardingResponses._namePrompt),
            });
        }

        private async Task<DialogTurnResult> AskForEmail(DialogContext dc, WaterfallStepContext args, CancellationToken cancellationToken)
        {
            var name = dc.ActiveDialog.State["name"] = args.Result;

            await _responder.ReplyWith(dc.Context, OnboardingResponses._haveName, new { name });

            return await dc.PromptAsync(EmailPrompt, new PromptOptions()
            {
                Prompt = await _responder.RenderTemplate(dc.Context, "en", OnboardingResponses._emailPrompt),
            });
        }

        private async Task<DialogTurnResult> AskForLocation(DialogContext dc, WaterfallStepContext args, CancellationToken cancellationToken)
        {
            var email = dc.ActiveDialog.State["email"] = args.Result;

            await _responder.ReplyWith(dc.Context, OnboardingResponses._haveEmail, new { email });

            return await dc.PromptAsync(LocationPrompt, new PromptOptions()
            {
                Prompt = await _responder.RenderTemplate(dc.Context, "en", OnboardingResponses._locationPrompt),
            });
        }

        private async Task<DialogTurnResult> FinishOnboardingDialog(DialogContext dc, WaterfallStepContext args, CancellationToken cancellationToken)
        {
            var name = dc.ActiveDialog.State["name"];
            var email = dc.ActiveDialog.State["email"];
            var location = dc.ActiveDialog.State["location"] = args.Result;

            await _responder.ReplyWith(dc.Context, OnboardingResponses._haveLocation, new { name, location });
            return await dc.EndAsync();
        }
    }
}

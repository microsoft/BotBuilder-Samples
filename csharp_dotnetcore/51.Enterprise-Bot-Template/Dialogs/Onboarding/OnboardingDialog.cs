// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace EnterpriseBot
{
    public class OnboardingDialog : EnterpriseDialog
    {
        // Constants
        public const string NamePrompt = "namePrompt";
        public const string EmailPrompt = "emailPrompt";
        public const string LocationPrompt = "locationPrompt";

        // Fields
        private static OnboardingResponses _responder = new OnboardingResponses();

        public OnboardingDialog(BotServices botServices)
            : base(botServices, nameof(OnboardingDialog))
        {
            InitialDialogId = nameof(OnboardingDialog);

            var onboarding = new WaterfallStep[]
            {
                AskForName,
                AskForEmail,
                AskForLocation,
                FinishOnboardingDialog,
            };

            AddDialog(new WaterfallDialog(InitialDialogId, onboarding));
            AddDialog(new TextPrompt(NamePrompt));
            AddDialog(new TextPrompt(EmailPrompt));
            AddDialog(new TextPrompt(LocationPrompt));
        }

        public async Task<DialogTurnResult> AskForName(WaterfallStepContext sc, CancellationToken cancellationToken)
        {
            return await sc.PromptAsync(NamePrompt, new PromptOptions()
            {
                Prompt = await _responder.RenderTemplate(sc.Context, "en", OnboardingResponses._namePrompt),
            });
        }

        public async Task<DialogTurnResult> AskForEmail(WaterfallStepContext sc, CancellationToken cancellationToken)
        {
            var name = sc.Values["name"] = sc.Result;

            await _responder.ReplyWith(sc.Context, OnboardingResponses._haveName, new { name });

            return await sc.PromptAsync(EmailPrompt, new PromptOptions()
            {
                Prompt = await _responder.RenderTemplate(sc.Context, "en", OnboardingResponses._emailPrompt),
            });
        }

        public async Task<DialogTurnResult> AskForLocation(WaterfallStepContext sc, CancellationToken cancellationToken)
        {
            var email = sc.Values["email"] = sc.Result;

            await _responder.ReplyWith(sc.Context, OnboardingResponses._haveEmail, new { email });

            return await sc.PromptAsync(LocationPrompt, new PromptOptions()
            {
                Prompt = await _responder.RenderTemplate(sc.Context, "en", OnboardingResponses._locationPrompt),
            });
        }

        public async Task<DialogTurnResult> FinishOnboardingDialog(WaterfallStepContext sc, CancellationToken cancellationToken)
        {
            var name = sc.Values["name"];
            var email = sc.Values["email"];
            var location = sc.Values["location"] = sc.Result;

            await _responder.ReplyWith(sc.Context, OnboardingResponses._haveLocation, new { name, location });
            return await sc.EndAsync();
        }
    }
}

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
        public const string Name = "onboarding";
        public const string NamePrompt = "namePrompt";
        public const string EmailPrompt = "emailPrompt";
        public const string LocationPrompt = "locationPrompt";

        // Fields
        private static OnboardingResponses _responder = new OnboardingResponses();

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

        public async Task<DialogTurnResult> AskForName(DialogContext dc, WaterfallStepContext args, CancellationToken cancellationToken)
        {
            return await dc.PromptAsync(NamePrompt, new PromptOptions()
            {
                Prompt = await _responder.RenderTemplate(dc.Context, "en", OnboardingResponses._namePrompt),
            });
        }

        public async Task<DialogTurnResult> AskForEmail(DialogContext dc, WaterfallStepContext args, CancellationToken cancellationToken)
        {
            var name = dc.ActiveDialog.State["name"] = args.Result;

            await _responder.ReplyWith(dc.Context, OnboardingResponses._haveName, new { name });

            return await dc.PromptAsync(EmailPrompt, new PromptOptions()
            {
                Prompt = await _responder.RenderTemplate(dc.Context, "en", OnboardingResponses._emailPrompt),
            });
        }

        public async Task<DialogTurnResult> AskForLocation(DialogContext dc, WaterfallStepContext args, CancellationToken cancellationToken)
        {
            var email = dc.ActiveDialog.State["email"] = args.Result;

            await _responder.ReplyWith(dc.Context, OnboardingResponses._haveEmail, new { email });

            return await dc.PromptAsync(LocationPrompt, new PromptOptions()
            {
                Prompt = await _responder.RenderTemplate(dc.Context, "en", OnboardingResponses._locationPrompt),
            });
        }

        public async Task<DialogTurnResult> FinishOnboardingDialog(DialogContext dc, WaterfallStepContext args, CancellationToken cancellationToken)
        {
            var name = dc.ActiveDialog.State["name"];
            var email = dc.ActiveDialog.State["email"];
            var location = dc.ActiveDialog.State["location"] = args.Result;

            await _responder.ReplyWith(dc.Context, OnboardingResponses._haveLocation, new { name, location });
            return await dc.EndAsync();
        }
    }
}

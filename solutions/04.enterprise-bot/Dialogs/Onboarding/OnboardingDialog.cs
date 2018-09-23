// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
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
        private IStatePropertyAccessor<OnboardingState> _accessor;
        private OnboardingState _state;

        public OnboardingDialog(BotServices botServices, IStatePropertyAccessor<OnboardingState> accessor)
            : base(botServices, nameof(OnboardingDialog))
        {
            _accessor = accessor;
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
            _state = await _accessor.GetAsync(sc.Context);

            if (!string.IsNullOrEmpty(_state.Name))
            {
                return await sc.NextAsync(_state.Name);
            }
            else
            {
                return await sc.PromptAsync(NamePrompt, new PromptOptions()
                {
                    Prompt = await _responder.RenderTemplate(sc.Context, "en", OnboardingResponses._namePrompt),
                });
            }
        }

        public async Task<DialogTurnResult> AskForEmail(WaterfallStepContext sc, CancellationToken cancellationToken)
        {
            _state = await _accessor.GetAsync(sc.Context);
            var name = _state.Name = (string)sc.Result;

            await _responder.ReplyWith(sc.Context, OnboardingResponses._haveName, new { name });

            if (!string.IsNullOrEmpty(_state.Email))
            {
                return await sc.NextAsync(_state.Email);
            }
            else
            {
                return await sc.PromptAsync(EmailPrompt, new PromptOptions()
                {
                    Prompt = await _responder.RenderTemplate(sc.Context, "en", OnboardingResponses._emailPrompt),
                });
            }
        }

        public async Task<DialogTurnResult> AskForLocation(WaterfallStepContext sc, CancellationToken cancellationToken)
        {
            _state = await _accessor.GetAsync(sc.Context);
            var email = _state.Email = (string)sc.Result;

            await _responder.ReplyWith(sc.Context, OnboardingResponses._haveEmail, new { email });

            if (!string.IsNullOrEmpty(_state.Location))
            {
                return await sc.NextAsync(_state.Location);
            }
            else
            {
                return await sc.PromptAsync(LocationPrompt, new PromptOptions()
                {
                    Prompt = await _responder.RenderTemplate(sc.Context, "en", OnboardingResponses._locationPrompt),
                });
            }
        }

        public async Task<DialogTurnResult> FinishOnboardingDialog(WaterfallStepContext sc, CancellationToken cancellationToken)
        {
            _state = await _accessor.GetAsync(sc.Context);
            _state.Location = (string)sc.Result;

            await _responder.ReplyWith(sc.Context, OnboardingResponses._haveLocation, new { _state.Name, _state.Location });
            return await sc.EndDialogAsync();
        }
    }
}

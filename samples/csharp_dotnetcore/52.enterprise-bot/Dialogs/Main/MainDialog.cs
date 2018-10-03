// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Luis;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace EnterpriseBot
{
    public class MainDialog : RouterDialog
    {
        private BotServices _services;
        private UserState _userState;
        private ConversationState _conversationState;
        private MainResponses _responder = new MainResponses();

        public MainDialog(BotServices services, ConversationState conversationState, UserState userState)
            : base(nameof(MainDialog))
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _conversationState = conversationState;
            _userState = userState;

            AddDialog(new OnboardingDialog(_services, _userState.CreateProperty<OnboardingState>(nameof(OnboardingState))));
            AddDialog(new EscalateDialog(_services));
        }

        protected override async Task OnStartAsync(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var onboardingAccessor = _userState.CreateProperty<OnboardingState>(nameof(OnboardingState));
            var onboardingState = await onboardingAccessor.GetAsync(innerDc.Context, () => new OnboardingState());

            var view = new MainResponses();
            await view.ReplyWith(innerDc.Context, MainResponses.Intro);

            if (string.IsNullOrEmpty(onboardingState.Name))
            {
                // This is the first time the user is interacting with the bot, so gather onboarding information.
                await innerDc.BeginDialogAsync(nameof(OnboardingDialog));
            }
        }

        protected override async Task RouteAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Check dispatch result
            var dispatchResult = await _services.DispatchRecognizer.RecognizeAsync<Dispatch>(dc.Context, CancellationToken.None);
            var intent = dispatchResult.TopIntent().intent;

            if (intent == Dispatch.Intent.l_General)
            {
                // If dispatch result is general luis model
                var luisService = _services.LuisServices["<YOUR MS BOT NAME>_General"];
                var result = await luisService.RecognizeAsync<General>(dc.Context, CancellationToken.None);

                var generalIntent = result?.TopIntent().intent;

                // switch on general intents
                switch (generalIntent)
                {
                    case General.Intent.Greeting:
                        {
                            // send greeting response
                            await _responder.ReplyWith(dc.Context, MainResponses.Greeting);
                            break;
                        }

                    case General.Intent.Help:
                        {
                            // send help response
                            await _responder.ReplyWith(dc.Context, MainResponses.Help);
                            break;
                        }

                    case General.Intent.Cancel:
                        {
                            // Send cancelled response.
                            await _responder.ReplyWith(dc.Context, MainResponses.Cancelled);

                            // Cancel any active dialogs on the stack.
                            await dc.CancelAllDialogsAsync();
                            break;
                        }

                    case General.Intent.Escalate:
                        {
                            // Start escalate dialog.
                            await dc.BeginDialogAsync(nameof(EscalateDialog));
                            break;
                        }

                    case General.Intent.None:
                    default:
                        {
                            // No intent was identified, send confused message.
                            await _responder.ReplyWith(dc.Context, MainResponses.Confused);
                            break;
                        }
                }
            }
            else if (intent == Dispatch.Intent.q_FAQ)
            {
                var qnaService = _services.QnAServices["<YOUR_QNA_SERVICE_NAME>"];
                var answers = await qnaService.GetAnswersAsync(dc.Context);

                if (answers != null && answers.Count() > 0)
                {
                    await dc.Context.SendActivityAsync(answers[0].Answer);
                }
            }
        }

        protected override async Task CompleteAsync(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken))
        {
            // The active dialogs stack ended with a complete status.
            await _responder.ReplyWith(innerDc.Context, MainResponses.Completed);
        }
    }
}

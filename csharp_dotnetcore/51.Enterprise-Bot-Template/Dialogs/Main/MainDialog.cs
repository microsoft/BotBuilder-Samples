// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Luis;
using Microsoft.Bot.Builder.Dialogs;

namespace EnterpriseBot
{
    public class MainDialog : RouterDialog
    {
        private BotServices _services;
        private MainResponses _responder = new MainResponses();
        private Dictionary<Regex, string> _regexMap = new Dictionary<Regex, string>()
        {
            { new Regex("hi", RegexOptions.IgnoreCase), General.Intent.Greeting.ToString() },
            { new Regex("hello", RegexOptions.IgnoreCase), General.Intent.Greeting.ToString() },
            { new Regex("help", RegexOptions.IgnoreCase), General.Intent.Help.ToString() },
            { new Regex("cancel", RegexOptions.IgnoreCase), General.Intent.Cancel.ToString() },
            { new Regex("escalate", RegexOptions.IgnoreCase), General.Intent.Escalate.ToString() },
        };

        public MainDialog(BotServices services)
            : base(nameof(MainDialog))
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));

            AddDialog(new OnboardingDialog(_services));
            AddDialog(new EscalateDialog(_services));
        }

        protected override async Task OnStart(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var view = new MainResponses();
            await view.ReplyWith(innerDc.Context, MainResponses.Intro);

            // This is the first time the user is interacting with the bot, so gather onboarding information.
            await innerDc.BeginAsync(nameof(OnboardingDialog));
        }

        protected override async Task RouteAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Check dispatch result
            var dispatchResult = await _services.DispatchRecognizer.RecognizeAsync<Dispatch>(dc.Context, CancellationToken.None);
            var intent = dispatchResult.TopIntent().intent;

            if (intent == Dispatch.Intent.l_General)
            {
                var regexRecognizer = new RegexRecognizer(_regexMap);
                var result = await regexRecognizer.RecognizeAsync<General>(dc.Context, CancellationToken.None);

                if (result == null)
                {
                    // If dispatch result is general luis model
                    var luisService = _services.LuisServices["EnterpriseBot-General"];
                    result = await luisService.RecognizeAsync<General>(dc.Context, CancellationToken.None);
                }

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
                            // send cancelled response
                            await _responder.ReplyWith(dc.Context, MainResponses.Cancelled);

                            // Cancel any active dialogs on the stack
                            await dc.CancelAllAsync();
                            break;
                        }

                    case General.Intent.Escalate:
                        {
                            // start escalate dialog
                            await dc.BeginAsync(nameof(EscalateDialog));
                            break;
                        }

                    case General.Intent.None:
                    default:
                        {
                            // No intent was identified, send confused message
                            await _responder.ReplyWith(dc.Context, MainResponses.Confused);
                            break;
                        }
                }
            }
            else if (intent == Dispatch.Intent.q_FAQ)
            {
                var qnaService = _services.QnAServices["EnterpriseBot-FAQ"];
                var answers = await qnaService.GetAnswersAsync(dc.Context);

                if (answers != null && answers.Count() > 0)
                {
                    await dc.Context.SendActivityAsync(answers[0].Answer);
                }
            }
        }

        protected override async Task CompleteAsync(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken)) =>
            // The active dialog's stack ended with a complete status
            await _responder.ReplyWith(innerDc.Context, MainResponses.Completed);
    }
}

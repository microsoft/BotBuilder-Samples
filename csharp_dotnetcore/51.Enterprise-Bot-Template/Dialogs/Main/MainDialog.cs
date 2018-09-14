// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Luis;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace EnterpriseBot
{
    public class MainDialog : RouterDialog
    {
        // Fields
        private static MainResponses _responder = new MainResponses();
        private readonly BotServices _services;

        public MainDialog(BotServices services)
            : base(nameof(MainDialog))
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));

            AddDialog(new OnboardingDialog(_services));
            AddDialog(new EscalateDialog(_services));
        }

        protected override async Task<DialogTurnResult> RouteAsync(DialogContext dc, DialogTurnResult result, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dc.Context.Activity.Type == ActivityTypes.Message)
            {
                // If an active dialog is waiting, continue dialog
                switch (result.Status)
                {
                    case DialogTurnStatus.Empty:
                        {
                            // No dialog is currently on the stack and we haven't responded to the user
                            // Check dispatch result
                            var dispatchResult = await _services.DispatchRecognizer.RecognizeAsync<Dispatch>(dc.Context, CancellationToken.None);
                            var intent = dispatchResult.TopIntent().intent;

                            if (intent == Dispatch.Intent.l_General)
                            {
                                // If dispatch result is general luis model
                                var luisService = _services.LuisServices["EnterpriseBot-General"];
                                var luisResult = await luisService.RecognizeAsync<General>(dc.Context, CancellationToken.None);
                                var luisIntent = luisResult?.TopIntent().intent;

                                // switch on general intents
                                switch (luisIntent)
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

                            break;
                        }

                    case DialogTurnStatus.Waiting:
                        {
                            // The active dialog is waiting for a response from the user, so do nothing
                            break;
                        }

                    case DialogTurnStatus.Complete:
                        {
                            // The active dialog's stack ended with a complete status
                            await _responder.ReplyWith(dc.Context, MainResponses.Completed);

                            // End active dialog
                            await dc.EndAsync();
                            break;
                        }

                    default:
                        {
                            // The active dialog's stack ended with an error status
                            await _responder.ReplyWith(dc.Context, MainResponses.Confused);

                            // End active dialog
                            await dc.EndAsync();
                            break;
                        }
                }
            }
            else if (dc.Context.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                // greet when added to conversation
                var activity = dc.Context.Activity.AsConversationUpdateActivity();
                if (activity.MembersAdded.Where(m => m.Id == activity.Recipient.Id).Any())
                {
                    var view = new MainResponses();
                    await view.ReplyWith(dc.Context, MainResponses.Intro);

                    // This is the first time the user is interacting with the bot, so gather onboarding information.
                    await dc.BeginAsync(nameof(OnboardingDialog));
                }
            }
            else
            {
                HandleSystemMessage(dc.Context);
            }

            return new DialogTurnResult(DialogTurnStatus.Waiting);
        }

        private void HandleSystemMessage(ITurnContext context)
        {
            switch (context.Activity.Type)
            {
                case ActivityTypes.ContactRelationUpdate:
                case ActivityTypes.DeleteUserData:
                case ActivityTypes.EndOfConversation:
                case ActivityTypes.Typing:
                case ActivityTypes.Event:
                    // Add any additional logic for process these system messages here.
                    break;
            }
        }
    }
}

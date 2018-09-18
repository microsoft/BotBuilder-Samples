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

namespace MessageRoutingBot
{
    /// <summary>
    /// Entry point for the dialogs of the <see cref="MessageRoutingBot"/>.
    /// </summary>
    public class MainDialog : ComponentDialog
    {
        // Constants
        public const string Name = "MainDialog";

        // Fields
        private static MainResponses _responder = new MainResponses();
        private readonly BotServices _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainDialog"/> class.
        /// </summary>
        /// <param name="services">The <see cref="BotServices"/> for the bot.</param>
        public MainDialog(BotServices services)
            : base(Name)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));

            AddDialog(new OnboardingDialog(_services));
            AddDialog(new EscalateDialog(_services));
        }

        protected override async Task<DialogTurnResult> OnDialogBeginAsync(DialogContext dc, DialogOptions options, CancellationToken cancellationToken)
        {
            // Override default begin() logic with bot orchestration logic
            return await OnDialogContinueAsync(dc, cancellationToken);
        }

        protected override async Task<DialogTurnResult> OnDialogContinueAsync(DialogContext dc, CancellationToken cancellationToken)
        {
            if (dc.Context.Activity.Type == ActivityTypes.Message)
            {
                // If an active dialog is waiting, continue dialog
                var result = await dc.ContinueAsync();

                switch (result.Status)
                {
                    case DialogTurnStatus.Empty:
                        {
                            // No dialog is currently on the stack and we haven't responded to the user
                            // Check dispatch result
                            var luisService = _services.LuisServices["General.luis"];
                            var luisResult = await luisService.RecognizeAsync<MessageRoutingBot_General>(dc.Context, CancellationToken.None);
                            var luisIntent = luisResult?.TopIntent().intent;

                            // switch on general intents
                            switch (luisIntent)
                            {
                                case MessageRoutingBot_General.Intent.Greeting:
                                    {
                                        // send greeting response
                                        await _responder.ReplyWith(dc.Context, MainResponses.Greeting);
                                        break;
                                    }

                                case MessageRoutingBot_General.Intent.Help:
                                    {
                                        // send help response
                                        await _responder.ReplyWith(dc.Context, MainResponses.Help);
                                        break;
                                    }

                                case MessageRoutingBot_General.Intent.Cancel:
                                    {
                                        // send cancelled response
                                        await _responder.ReplyWith(dc.Context, MainResponses.Cancelled);

                                        // Cancel any active dialogs on the stack
                                        await dc.CancelAllAsync();
                                        break;
                                    }

                                case MessageRoutingBot_General.Intent.Escalate:
                                    {
                                        // start escalate dialog
                                        await dc.BeginAsync(OnboardingDialog.Name);
                                        break;
                                    }

                                case MessageRoutingBot_General.Intent.None:
                                default:
                                    {
                                        // No intent was identified, send confused message
                                        await _responder.ReplyWith(dc.Context, MainResponses.Confused);
                                        break;
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
            else
            {
                await HandleSystemMessage(dc.Context);
            }

            return new DialogTurnResult(DialogTurnStatus.Waiting);
        }

        private async Task HandleSystemMessage(ITurnContext context)
        {
            switch (context.Activity.Type)
            {
                case ActivityTypes.ContactRelationUpdate:
                case ActivityTypes.DeleteUserData:
                case ActivityTypes.EndOfConversation:
                case ActivityTypes.Typing:
                case ActivityTypes.Event:
                    var ev = context.Activity.AsEventActivity();
                    await context.SendActivityAsync($"Received event: {ev.Name}");
                    break;

                case ActivityTypes.ConversationUpdate:
                    // greet when added to conversation
                    var activity = context.Activity.AsConversationUpdateActivity();
                    if (activity.MembersAdded.Where(m => m.Id == activity.Recipient.Id).Any())
                    {
                        var view = new MainResponses();
                        await view.ReplyWith(context, MainResponses.Intro);
                    }

                    break;
            }
        }
    }
}

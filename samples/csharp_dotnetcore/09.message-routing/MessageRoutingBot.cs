// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// Main entry point and orchestration for bot.
    /// </summary>
    public class MessageRoutingBot : IBot
    {
        // Supported LUIS Intents
        public const string GreetingIntent = "Greeting";
        public const string CancelIntent = "Cancel";
        public const string HelpIntent = "Help";
        public const string NoneIntent = "None";

        private readonly IStatePropertyAccessor<GreetingState> _greetingStateAccessor;
        private readonly IStatePropertyAccessor<DialogState> _dialogStateAccessor;
        private readonly UserState _userState;
        private readonly ConversationState _conversationState;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRoutingBot"/> class.
        /// </summary>
        /// <param name="userState">The state object that stores the user state.</param>
        /// <param name="conversationState">The state object that stores the dialog state.</param>
        /// <param name="loggerFactory">Logger factory.</param>
        public MessageRoutingBot(UserState userState, ConversationState conversationState, ILoggerFactory loggerFactory)
        {
            _userState = userState ?? throw new ArgumentNullException(nameof(userState));
            _conversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));

            _greetingStateAccessor = _userState.CreateProperty<GreetingState>(nameof(GreetingState));
            _dialogStateAccessor = _conversationState.CreateProperty<DialogState>(nameof(DialogState));

            Dialogs = new DialogSet(_dialogStateAccessor);
            Dialogs.Add(new GreetingDialog(_greetingStateAccessor, loggerFactory));
        }

        private DialogSet Dialogs { get; set; }

        /// <summary>
        /// Run every turn of the conversation. Handles orchestration of messages.
        /// </summary>
        /// <param name="turnContext">Bot Turn Context.</param>
        /// <param name="cancellationToken">Task CancellationToken.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var activity = turnContext.Activity;

            // Create a dialog context
            var dc = await Dialogs.CreateContextAsync(turnContext);

            if (activity.Type == ActivityTypes.Message)
            {
                // Recognize what the user said.
                var topIntent = await GreetingHelpCancelRecognizer(turnContext, cancellationToken);

                // Handle conversation interrupts first.
                var interrupted = await IsTurnInterruptedAsync(dc, topIntent);
                if (interrupted)
                {
                    // Bypass the dialog.
                    await _conversationState.SaveChangesAsync(turnContext);
                    await _userState.SaveChangesAsync(turnContext);
                    return;
                }

                // Continue the current dialog
                var dialogResult = await dc.ContinueDialogAsync();

                // if no one has responded,
                if (!dc.Context.Responded)
                {
                    // examine results from active dialog
                    switch (dialogResult.Status)
                    {
                        case DialogTurnStatus.Empty:
                            switch (topIntent)
                            {
                                case GreetingIntent:
                                    await dc.BeginDialogAsync(nameof(GreetingDialog));
                                    break;

                                case NoneIntent:
                                default:
                                    // Help or no intent identified, either way, let's provide some help.
                                    // to the user
                                    await dc.Context.SendActivityAsync("I didn't understand what you just said to me.");
                                    break;
                            }

                            break;

                        case DialogTurnStatus.Waiting:
                            // The active dialog is waiting for a response from the user, so do nothing.
                            break;

                        case DialogTurnStatus.Complete:
                            await dc.EndDialogAsync();
                            break;

                        default:
                            await dc.CancelAllDialogsAsync();
                            break;
                    }
                }
            }
            else if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (activity.MembersAdded != null)
                {
                    // Iterate over all new members added to the conversation.
                    foreach (var member in activity.MembersAdded)
                    {
                        // Greet anyone that was not the target (recipient) of this message.
                        // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
                        if (member.Id != activity.Recipient.Id)
                        {
                            var welcomeCard = CreateAdaptiveCardAttachment();
                            var response = CreateResponse(activity, welcomeCard);
                            await dc.Context.SendActivityAsync(response);
                        }
                    }
                }
            }

            await _conversationState.SaveChangesAsync(turnContext);
            await _userState.SaveChangesAsync(turnContext);
        }

        // Determine if an interruption has occurred before we dispatch to any active dialog.
        private async Task<bool> IsTurnInterruptedAsync(DialogContext dc, string topIntent)
        {
            // See if there are any conversation interrupts we need to handle.
            if (topIntent.Equals(CancelIntent))
            {
                if (dc.ActiveDialog != null)
                {
                    await dc.CancelAllDialogsAsync();
                    await dc.Context.SendActivityAsync("Ok. I've canceled our last activity.");
                }
                else
                {
                    await dc.Context.SendActivityAsync("I don't have anything to cancel.");
                }

                return true;        // Handled the interrupt.
            }

            if (topIntent.Equals(HelpIntent))
            {
                await dc.Context.SendActivityAsync("Let me try to provide some help.");
                await dc.Context.SendActivityAsync("I understand greetings, being asked for help, or being asked to cancel what I am doing.");
                if (dc.ActiveDialog != null)
                {
                    await dc.RepromptDialogAsync();
                }

                return true;        // Handled the interrupt.
            }

            return false;           // Did not handle the interrupt.
        }

        // Create an attachment message response.
        private Activity CreateResponse(Activity activity, Attachment attachment)
        {
            var response = activity.CreateReply();
            response.Attachments = new List<Attachment>() { attachment };
            return response;
        }

        // Load attachment from file.
        private Attachment CreateAdaptiveCardAttachment()
        {
            // combine path for cross platform support
            string[] paths = { ".", "Dialogs", "Welcome", "Resources", "welcomeCard.json" };
            var card = File.ReadAllText(Path.Combine(paths));

            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(card),
            };
        }

        private async Task<string> GreetingHelpCancelRecognizer(ITurnContext context, CancellationToken cancellationToken)
        {
            var patterns = new Dictionary<Regex, string>()
            {
                { new Regex(@"\?+", RegexOptions.IgnoreCase), HelpIntent },
                { new Regex("help", RegexOptions.IgnoreCase), HelpIntent },
                { new Regex(@"cancel", RegexOptions.IgnoreCase), CancelIntent },
                { new Regex(@"stop", RegexOptions.IgnoreCase), CancelIntent },
                { new Regex(@"hi", RegexOptions.IgnoreCase), GreetingIntent },
                { new Regex(@"hello", RegexOptions.IgnoreCase), GreetingIntent },
                { new Regex(@"hola", RegexOptions.IgnoreCase), GreetingIntent },
                { new Regex(@"yo", RegexOptions.IgnoreCase), GreetingIntent },
            };
            var recognizer = new RegexRecognizer(patterns);
            var result = await recognizer.RecognizeAsync(context, cancellationToken);
            return result.GetTopScoringIntent().intent;
        }
    }
}

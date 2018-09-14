using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
    public class MainDialog : ComponentDialog
    {
        // Supported LUIS Main Dialog Intents
        public const string GreetingIntent = "Greeting";
        public const string HelpIntent = "Help";

        /// <summary>
        /// Key in the bot config (.bot file) for the LUIS instance.
        /// In the .bot file, multiple instances of LUIS can be configured.
        /// </summary>
        public static readonly string LuisKey = "BasicBotLUIS";

        private readonly BotServices _services;
        private readonly ILogger _logger;

        public MainDialog(BotServices services, BasicBotAccessors accessors, ILogger logger)
            : base(nameof(MainDialog))
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            AddDialog(new GreetingDialog(services, accessors.DialogStateProperty, accessors.GreetingStateProperty, logger));
            AddDialog(new NamePrompt(nameof(NamePrompt)));
            AddDialog(new CityPrompt(nameof(CityPrompt)));
        }

        protected override async Task<DialogTurnResult> OnDialogBeginAsync(DialogContext dc, DialogOptions options, CancellationToken cancellationToken)
        {
            // Override default begin() logic with bot orchestration logic
            return await OnDialogContinueAsync(dc, cancellationToken).ConfigureAwait(false);
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
                        // No dialog is currently on the stack and we haven't responded to the user.
                        // Perform a call to LUIS to retrieve results for the current activity message.
                        var luisResults = await _services.LuisServices[LuisKey].RecognizeAsync(dc.Context, cancellationToken).ConfigureAwait(false);
                        var topIntent = luisResults?.GetTopScoringIntent();
                        var interruptResult = InterruptionStatus.NoAction;
                        if (topIntent != null)
                        {
                            // See if there are any conversation interrupts we need to handle
                            switch (topIntent.Value.intent)
                            {
                                case GreetingIntent:
                                    if (topIntent.Value.score > .5)
                                    {
                                        await dc.BeginAsync(nameof(GreetingDialog), null, cancellationToken);
                                    }

                                    break;

                                case HelpIntent:
                                    if (topIntent.Value.score > .5)
                                    {
                                        interruptResult = await OnMainHelpAsync(dc).ConfigureAwait(false);
                                    }

                                    break;

                                default:
                                    interruptResult = await OnConfusedAsync(dc).ConfigureAwait(false);
                                    break;
                            }
                        }
                        else
                        {
                            _logger.LogError($"No LUIS intent identified.");
                            interruptResult = await OnConfusedAsync(dc).ConfigureAwait(false);
                        }

                        break;

                    case DialogTurnStatus.Waiting:
                        // The active dialog is waiting for a response from the user, so do nothing
                        break;

                    case DialogTurnStatus.Complete:
                        await dc.EndAsync();
                        break;

                    default:
                        // The active dialog's stack ended with an error status
                        // End active dialog
                        await dc.EndAsync();
                        break;
                }
            }
            else
            {
                await HandleSystemMessageAsync(dc.Context);
            }

            return new DialogTurnResult(DialogTurnStatus.Waiting);
        }

        /// <summary>
        /// Handle help requests.
        /// </summary>
        /// <param name="dc">The current <see cref="DialogContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the <see cref="InterruptionStatus"/>.</returns>
        protected virtual async Task<InterruptionStatus> OnMainHelpAsync(DialogContext dc)
        {
            if (dc.ActiveDialog != null)
            {
                await dc.CancelAllAsync();
            }

            await dc.Context.SendActivityAsync("Welcome to the Basic Bot.");
            await dc.Context.SendActivityAsync("I understand greetings, being asked for help, or being asked to cancel what I am doing.");

            // Signal the conversation was interrupted and should immediately continue
            return InterruptionStatus.Interrupted;
        }

        /// <summary>
        /// Handle help requests.
        /// </summary>
        /// <param name="dc">The current <see cref="DialogContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the <see cref="InterruptionStatus"/>.</returns>
        protected virtual async Task<InterruptionStatus> OnConfusedAsync(DialogContext dc)
        {
            await dc.Context.SendActivityAsync("Didn't quite understand that.");
            await dc.Context.SendActivityAsync("I understand greetings, being asked for help, or being asked to cancel what I am doing.");

            // Signal the conversation was interrupted and should immediately continue
            return InterruptionStatus.Interrupted;
        }

        // Handle System Messages.
        private async Task HandleSystemMessageAsync(ITurnContext context)
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
                    // Greet user when added to conversation.
                    var activity = context.Activity.AsConversationUpdateActivity();
                    if (activity.MembersAdded.Where(m => m.Id == activity.Recipient.Id).Any())
                    {
                        // When activity type is "conversationUpdate" and the member joining the conversation is the bot
                        // we will send our Welcome Adaptive Card.  This will only be sent once, when the Bot joins conversation
                        // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
                        var welcomeCard = CreateAdaptiveCardAttachment();
                        var response = CreateResponse(context.Activity, welcomeCard);
                        await context.SendActivityAsync(response).ConfigureAwait(false);
                    }

                    break;
            }
        }

        // Create an attachment message response.
        private Activity CreateResponse(Activity activity, Attachment attachment)
        {
            var response = activity.CreateReply();
            response.Attachments = new List<Attachment>() { attachment };
            return response;
        }

        // Load attachment from file
        private Attachment CreateAdaptiveCardAttachment()
        {
            var adaptiveCard = File.ReadAllText(@".\Dialogs\Welcome\Resources\welcomeCard.json");
            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCard),
            };
        }
    }
}

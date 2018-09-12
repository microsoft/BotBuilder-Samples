// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
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
    /// For each interaction from the user, an instance of this class is created and
    /// the OnTurnAsync method is called.
    /// This is a transient lifetime service.  Transient lifetime services are created
    /// each time they're requested. For each <see cref="Activity"/> received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    /// <seealso cref="https://docs.microsoft.com/en-us/dotnet/api/microsoft.bot.ibot?view=botbuilder-dotnet-preview"/>
    public class BasicBot : IBot
    {
        // Supported LUIS Intents
        public const string GreetingIntent = "Greeting";
        public const string CancelIntent = "Cancel";
        public const string HelpIntent = "Help";
        public const string NoneIntent = "None";

        /// <summary>
        /// Key in the bot config (.bot file) for the LUIS instance.
        /// In the .bot file, multiple instances of LUIS can be configured.
        /// </summary>
        public static readonly string LuisKey = "BasicBot";

        // Greeting Dialog ID
        public static readonly string GreetingDialogId = "greetingDialog";

        /// <summary>
        /// Services configured from the .bot file.
        /// </summary>
        private readonly BotServices _services;

        /// <summary>
        /// Accessors (and associated State managers).
        /// </summary>
        private readonly BasicBotAccessors _accessors;

        /// <summary>
        /// Top level dialog(s).
        /// </summary>
        private readonly DialogSet _dialogs;

        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicBot"/> class.
        /// </summary>
        /// <param name="services">Services configured from the .bot file.</param>
        /// <param name="accessors">A class containing <see cref="IStatePropertyAccessor{T}"/> used to manage state.</param>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory"/> that hooked to the Azure App Service provider.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1#windows-eventlog-provider"/>        
        /// <seealso cref="BotConfiguration"/>
        public BasicBot(BotServices services, BasicBotAccessors accessors, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new System.ArgumentNullException(nameof(loggerFactory));
            }

            _services = services ?? throw new System.ArgumentNullException(nameof(services));
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));

            _logger = loggerFactory.CreateLogger<BasicBot>();
            _logger.LogTrace("BasicBot turn start.");

            if (!_services.LuisServices.ContainsKey(LuisKey))
            {
                throw new System.ArgumentException($"Invalid configuration.  Please check your '.bot' file for a LUIS service named '{LuisKey}'.");
            }

            // Create top-level dialog(s)
            _dialogs = new DialogSet(_accessors.DialogStateProperty);

            _dialogs.Add(new GreetingDialog(GreetingDialogId, _accessors.DialogStateProperty, _accessors.GreetingStateProperty));
            _dialogs.Add(new NamePrompt(GreetingDialog.NamePrompt));
            _dialogs.Add(new CityPrompt(GreetingDialog.CityPrompt));
        }

        /// <summary>
        /// Every conversation turn for our Basic Bot will call this method.
        /// </summary>
        /// <param name="context">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        public async Task OnTurnAsync(ITurnContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Run the DialogSet - let the framework identify the current state of the dialog from
            // the dialog stack and figure out what (if any) is the active dialog.
            DialogContext dc = await _dialogs.CreateContextAsync(context);

            if (context.Activity.Type == ActivityTypes.Message)
            {
                // Perform a call to LUIS to retrieve results for the current activity message.
                var luisResults = await _services.LuisServices[LuisKey].RecognizeAsync(context, cancellationToken);
                var topIntent = luisResults?.GetTopScoringIntent();

                // handle conversation interrupts first
                if (await IsTurnInterruptedAsync(dc, luisResults))
                {
                    // Canceled dialogs, save conversation state.
                    await _accessors.ConversationState.SaveChangesAsync(context);
                    return;
                }

                // Continue the current dialog
                var dialogResult = await dc.ContinueAsync();

                switch (dialogResult.Status)
                {
                    case DialogTurnStatus.Empty:
                        switch (topIntent.Value.intent)
                        {
                            case GreetingIntent:
                                await dc.BeginAsync(GreetingDialogId).ConfigureAwait(false);
                                break;

                            case NoneIntent:
                            default:
                                // help or no intent identified, either way, let's provide some help
                                // to the user
                                await dc.Context.SendActivityAsync("I didn't understand what you just said to me.");
                                break;
                        }

                        break;

                    case DialogTurnStatus.Waiting:
                        // The active dialog is waiting for a response from the user, so do nothing
                        break;

                    case DialogTurnStatus.Complete:
                        await dc.EndAsync();
                        break;

                    default:
                        await dc.CancelAllAsync();
                        break;
                }
            }
            else if (context.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (context.Activity.MembersAdded[0].Name.ToLowerInvariant().Equals("bot"))
                {
                    // When activity type is "conversationUpdate" and the member joining the conversation is the bot
                    // we will send our Welcome Adaptive Card.  This will only be sent once, when the Bot joins conversation
                    // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
                    var welcomeCard = CreateAdaptiveCardAttachment();
                    var response = CreateResponse(context.Activity, welcomeCard);
                    await context.SendActivityAsync(response).ConfigureAwait(false);
                }
            }

            await _accessors.ConversationState.SaveChangesAsync(context);
            await _accessors.UserState.SaveChangesAsync(context);
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

        // Determines if a dialog was cancelled.
        private async Task<bool> IsTurnInterruptedAsync(DialogContext dc, RecognizerResult luisResults)
        {
            var intents = luisResults.Intents;
            var topIntent = luisResults?.GetTopScoringIntent();

            // see if there are anh conversation interrupts we need to handle
            if (topIntent.Value.intent.Equals(CancelIntent) && intents[CancelIntent].Score > 0.8)
            {
                if (dc.ActiveDialog != null)
                {
                    await dc.CancelAllAsync().ConfigureAwait(false);
                    await dc.Context.SendActivityAsync("Ok. I've cancelled our last activity.");
                }
                else
                {
                    await dc.Context.SendActivityAsync("I don't have anything to cancel.");
                }

                return true;        // handled the interrupt
            }

            if (topIntent.Value.intent.Equals(HelpIntent) && intents[HelpIntent].Score > 0.8)
            {
                if (dc.ActiveDialog != null)
                {
                    await dc.CancelAllAsync();
                }

                await dc.Context.SendActivityAsync("Let me try to provide some help.");
                await dc.Context.SendActivityAsync("I understand greetings, being asked for help, or being asked to cancel what I am doing.");
                return true;        // handled the interrupt
            }

            return false;           // did not handle the interrupt
        }
    }
}

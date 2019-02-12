// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// For each interaction from the user, an instance of this class is created and
    /// the OnTurnAsync method is called.
    /// This is a transient lifetime service. Transient lifetime services are created
    /// each time they"re requested. For each <see cref="Activity"/> received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    /// <seealso cref="https://docs.microsoft.com/en-us/dotnet/api/microsoft.bot.ibot?view=botbuilder-dotnet-preview"/>
    public class CafeBot : IBot
    {
        // Supported LUIS Intents.
        public const string GreetingIntent = "Greeting";
        public const string HelpIntent = "Help";

        // Conversation State properties.
        public const string OnTurnPropertyName = "onTurnStateProperty";
        public const string DialogStateProperty = "dialogStateProperty";

        /// <summary>
        /// Key in the bot config (.bot file) for the LUIS instances.
        /// In the .bot file, multiple instances of LUIS can be configured.
        /// </summary>
        public static readonly string LuisConfiguration = "cafeDispatchModel";

        // Greeting Dialog ID.
        public static readonly string GreetingDialogId = "greetingDialog";

        /// <summary>
        /// Services configured from the .bot file.
        /// </summary>
        private readonly BotServices _services;

        private readonly IStatePropertyAccessor<OnTurnProperty> _onTurnAccessor;
        private readonly IStatePropertyAccessor<DialogState> _dialogAccessor;

        /// <summary>
        /// Top level dialog(s).
        /// </summary>
        private readonly DialogSet _dialogs;

        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CafeBot"/> class.
        /// </summary>
        /// <param name="userState">The <see cref="UserState"/> for properties at user scope.</param>
        /// <param name="conversationState">The <see cref="ConversationState"/> for properties at conversation scope.</param>
        /// <param name="services">The <see cref="BotServices"/> which holds clients for external services.</param>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory"/> that hooked to the Azure App Service provider.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1#windows-eventlog-provider"/>
        /// <seealso cref="BotConfiguration"/>
        public CafeBot(UserState userState, ConversationState conversationState, BotServices services, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new System.ArgumentNullException(nameof(loggerFactory));
            }

            ConversationState = conversationState ?? throw new System.ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new System.ArgumentNullException(nameof(userState));
            _services = services ?? throw new System.ArgumentNullException(nameof(services));

            // Create state property accessors.
            _onTurnAccessor = conversationState.CreateProperty<OnTurnProperty>(OnTurnPropertyName);
            _dialogAccessor = conversationState.CreateProperty<DialogState>(DialogStateProperty);

            // Create logger.
            _logger = loggerFactory.CreateLogger<CafeBot>();
            _logger.LogTrace("CafeBot turn start.");

            if (!_services.LuisServices.ContainsKey(LuisConfiguration))
            {
                throw new System.ArgumentException($"Invalid configuration. Please check your '.bot' file for a LUIS service named {LuisConfiguration}.");
            }

            // Create top-level dialog.
            _dialogs = new DialogSet(_dialogAccessor);
            _dialogs.Add(new MainDispatcher(services, _onTurnAccessor, userState, conversationState, loggerFactory));
        }

        /// <summary>
        /// Gets the <see cref="ConversationState"/> object for the conversation.
        /// </summary>
        /// <value>The <see cref="ConversationState"/> object.</value>
        public ConversationState ConversationState { get; }

        /// <summary>
        /// Gets the <see cref="UserState"/> object for the conversation.
        /// </summary>
        /// <value>The <see cref="UserState"/> object.</value>
        public UserState UserState { get; }

        /// <summary>
        /// Every conversation turn for our Basic Bot will call this method.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // See https://aka.ms/about-bot-activity-message to learn more about message and other activity types.
            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    // Process on turn input (card or NLP) and gather new properties.
                    // OnTurnProperty object has processed information from the input message activity.
                    var onTurnProperties = await DetectIntentAndEntitiesAsync(turnContext);
                    if (onTurnProperties == null)
                    {
                        break;
                    }

                    // Set the state with gathered properties (intent/ entities) through the onTurnAccessor.
                    await _onTurnAccessor.SetAsync(turnContext, onTurnProperties);

                    // Create dialog context.
                    var dc = await _dialogs.CreateContextAsync(turnContext);

                    // Continue outstanding dialogs.
                    await dc.ContinueDialogAsync();

                    // Begin main dialog if no outstanding dialogs/ no one responded.
                    if (!dc.Context.Responded)
                    {
                        await dc.BeginDialogAsync(nameof(MainDispatcher));
                    }

                    break;
                case ActivityTypes.ConversationUpdate:
                    if (turnContext.Activity.MembersAdded != null)
                    {
                        await SendWelcomeMessageAsync(turnContext);
                    }

                    break;
                default:
                    // Handle other activity types as needed.
                    await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected");
                    break;
            }

            await ConversationState.SaveChangesAsync(turnContext);
            await UserState.SaveChangesAsync(turnContext);
        }

        private async Task<OnTurnProperty> DetectIntentAndEntitiesAsync(ITurnContext turnContext)
        {
            // Handle card input (if any), update state and return.
            if (turnContext.Activity.Value != null)
            {
                if (turnContext.Activity.Value is JObject)
                {
                    return OnTurnProperty.FromCardInput((JObject)turnContext.Activity.Value);
                }
            }

            // Acknowledge attachments from user.
            if (turnContext.Activity.Attachments != null && turnContext.Activity.Attachments.Count > 0)
            {
                await turnContext.SendActivityAsync("Thanks for sending me that attachment. I'm still learning to process attachments.");
                return null;
            }

            // Nothing to do for this turn if there is no text specified.
            if (string.IsNullOrWhiteSpace(turnContext.Activity.Text) || string.IsNullOrWhiteSpace(turnContext.Activity.Text.Trim()))
            {
                return null;
            }

            // Call to LUIS recognizer to get intent + entities.
            var luisResults = await _services.LuisServices[LuisConfiguration].RecognizeAsync(turnContext, default(CancellationToken));

            // Return new instance of on turn property from LUIS results.
            // Leverages static fromLUISResults method.
            return OnTurnProperty.FromLuisResults(luisResults);
        }

        private async Task SendWelcomeMessageAsync(ITurnContext turnContext)
        {
            // Check to see if any new members were added to the conversation.
            if (turnContext.Activity.MembersAdded.Count > 0)
            {
                // Iterate over all new members added to the conversation.
                foreach (var member in turnContext.Activity.MembersAdded)
                {
                    // Greet anyone that was not the target (recipient) of this message
                    // the 'bot' is the recipient for events from the channel,
                    // turnContext.activity.membersAdded == turnContext.activity.recipient.Id indicates the
                    // bot was added to the conversation.
                    if (member.Id != turnContext.Activity.Recipient.Id)
                    {
                        // Welcome user.
                        await turnContext.SendActivityAsync("Hello, I am the Contoso Cafe Bot!");
                        await turnContext.SendActivityAsync("I can help book a table and more..");
                        var activity = turnContext.Activity.CreateReply();
                        activity.Attachments = new List<Attachment> { Helpers.CreateAdaptiveCardAttachment(new []{ ".", "Dialogs", "Welcome", "Resources", "welcomeCard.json" }), };

                        // Send welcome card.
                        await turnContext.SendActivityAsync(activity);
                    }
                }
            }
        }
    }
}

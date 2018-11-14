// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace BasicBot
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
        public const string HelpIntent = "Help";

        /// <summary>
        /// Key in the bot config (.bot file) for the LUIS instances.
        /// In the .bot file, multiple instances of LUIS can be configured.
        /// </summary>
        public static readonly string LuisKey = "BasicBotLUIS";

        // Greeting Dialog ID
        public static readonly string GreetingDialogId = "greetingDialog";

        /// <summary>
        /// Services configured from the .bot file.
        /// </summary>
        private readonly BotServices _services;

        /// <summary>
        /// Top level dialog(s).
        /// </summary>
        private readonly DialogSet _dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicBot"/> class.
        /// </summary>
        /// <param name="userState">The <see cref="UserState"/> for properties at user scope.</param>
        /// <param name="conversationState">The <see cref="ConversationState"/> for properties at conversation scope.</param>
        /// <param name="services">The <see cref="BotServices"/> which holds clients for external services.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1#windows-eventlog-provider"/>
        /// <seealso cref="BotConfiguration"/>
        public BasicBot(UserState userState, ConversationState conversationState, BotServices services)
        {
            ConversationState = conversationState ?? throw new System.ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new System.ArgumentNullException(nameof(userState));
            _services = services ?? throw new System.ArgumentNullException(nameof(services));

            if (!_services.LuisServices.ContainsKey(LuisKey))
            {
                throw new System.ArgumentException($"Invalid configuration.  Please check your '.bot' file for a LUIS service named '{LuisKey}'.");
            }

            // Create top-level dialog(s)
            _dialogs = new DialogSet(ConversationState.CreateProperty<DialogState>(nameof(BasicBot)));
            _dialogs.Add(new MainDialog(services, UserState));
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
        /// <param name="context">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        public async Task OnTurnAsync(ITurnContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Run the DialogSet - let the framework identify the current state of the dialog from
            // the dialog stack and figure out what (if any) is the active dialog.
            var dc = await _dialogs.CreateContextAsync(context);
            var dialogResult = await dc.ContinueDialogAsync();

            if (dialogResult.Status == DialogTurnStatus.Empty)
            {
                await dc.BeginDialogAsync(nameof(MainDialog));
            }
        }
    }
}

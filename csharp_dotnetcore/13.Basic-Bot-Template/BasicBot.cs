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
            _dialogs.Add(new MainDialog(services, accessors, _logger));
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
            var dialogResult = await dc.ContinueAsync();

            if (dialogResult.Status == DialogTurnStatus.Empty)
            {
                await dc.BeginAsync(MainDialog.DialogName);
            }

            await _accessors.ConversationState.SaveChangesAsync(context);
            await _accessors.UserState.SaveChangesAsync(context);
        }
    }
}

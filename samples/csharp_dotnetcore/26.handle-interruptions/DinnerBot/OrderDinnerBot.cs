// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace DinnerBot
{
    /// <summary>
    /// Represents a bot that processes incoming activities.
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// This is a Transient lifetime service.  Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// For example, the <see cref="MemoryStorage"/> object and associated
    /// <see cref="IStatePropertyAccessor{T}"/> object are created with a singleton lifetime.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    public class OrderDinnerBot : IBot
    {
        private readonly BotAccessors _accessors;
        private readonly OrderDinnerDialogs _dialogs;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDinnerBot"/> class.
        /// </summary>
        /// <param name="accessors">A class containing <see cref="IStatePropertyAccessor{T}"/> used to manage state.</param>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory"/> that is hooked to the Azure App Service provider.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1#windows-eventlog-provider"/>
        public OrderDinnerBot(BotAccessors accessors, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new System.ArgumentNullException(nameof(loggerFactory));
            }

            _logger = loggerFactory.CreateLogger<OrderDinnerBot>();
            _logger.LogTrace("EchoBot turn start.");
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));
            _dialogs = new OrderDinnerDialogs(_accessors.DialogStateAccessor);
        }

        /// <summary>
        /// Every conversation turn for our Echo Bot will call this method.
        /// There are no dialogs used, since it's "single turn" processing, meaning a single
        /// request and response.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet"/>
        /// <seealso cref="ConversationState"/>
        /// <seealso cref="IMiddleware"/>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            var dc = await _dialogs.CreateContextAsync(turnContext, cancellationToken);

            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {

                // Check for top-level interruptions.
                string utterance = turnContext.Activity.Text.Trim().ToLowerInvariant();

                if (utterance == "help")
                {
                    // Start a general help dialog. Dialogs already on the stack remain and will continue
                    // normally if the help dialog exits normally.
                    // await dc.BeginDialogAsync(OrderDinnerDialogs.HelpDialogId, null, cancellationToken);
                }
                else if (utterance == "cancel")
                {
                    // Cancel any dialog on the stack.
                    await turnContext.SendActivityAsync("Canceled.", cancellationToken: cancellationToken);
                    await dc.CancelAllDialogsAsync(cancellationToken);
                }
                else
                {
                    await dc.ContinueDialogAsync(cancellationToken);

                    // Check whether we replied. If not then clear the dialog stack and present the main menu.
                    if (!turnContext.Responded)
                    {
                        await dc.CancelAllDialogsAsync(cancellationToken);
                        await dc.BeginDialogAsync(OrderDinnerDialogs.MainDialogId, null, cancellationToken);
                    }
                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                var activity = turnContext.Activity.AsConversationUpdateActivity();
                if (activity.MembersAdded.Any(member => member.Id != activity.Recipient.Id))
                {
                    await dc.BeginDialogAsync(OrderDinnerDialogs.MainDialogId, null, cancellationToken);
                }
            }

            await _accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }
    }
}

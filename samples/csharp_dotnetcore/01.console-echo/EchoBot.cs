// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Console_EchoBot
{
    /// <summary>
    /// Represents a bot that echo's back the original input from the user.
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// This is a Transient lifetime service.  Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// </summary>
    /// <remarks>A <see cref="BotAdapter"/> passes incoming activities from the user's
    /// channel to the bot's <see cref="OnTurnAsync(ITurnContext, CancellationToken)"/> method.
    /// In this case, the Activity messages originate from console input, which flows through
    /// sample's <see cref="ConsoleAdapter"/> and the Bot Framework pipeline which calls this
    /// object.
    /// </remarks>
    /// <seealso cref="IMiddleware"/>
    /// <seealso cref="https://docs.microsoft.com/en-us/dotnet/api/microsoft.bot.ibot?view=botbuilder-dotnet-preview"/>
    public class EchoBot : IBot
    {
        /// <summary>
        /// Every Conversation turn for our EchoBot will call this method. In here
        /// the bot checks the <see cref="Activity"/> type to verify it's a <see cref="ActivityTypes.Message"/>
        /// message, and then echoes the user's typing back to them.
        /// </summary>
        /// <param name="turnContext">Turn scoped <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
        /// <seealso cref="https://docs.microsoft.com/en-us/dotnet/api/microsoft.bot.ibot.onturn?view=botbuilder-dotnet-preview#Microsoft_Bot_IBot_OnTurn_Microsoft_Bot_Builder_ITurnContext_"/>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Handle Message activity type, which is the main activity type within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Echo back to the user whatever they typed.
                await turnContext.SendActivityAsync($"You sent '{turnContext.Activity.Text}'");
            }
            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected");
            }
        }
    }
}
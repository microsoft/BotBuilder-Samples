// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace EchoBotWithCounter
{
    /// <summary>
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// This lifetime is according to request dependency scope. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// </summary>
    /// <seealso cref="https://msdn.microsoft.com/en-us/library/system.net.http.httprequestmessageextensions.getdependencyscope%28v=vs.108%29.aspx?f=255&MSPPError=-2147217396"/>
    public class EchoWithCounterBot : IBot
    {
        private readonly EchoBotAccessors _accessors;

        /// <summary>
        /// Initializes a new instance of the <see cref="EchoWithCounterBot"/> class.
        /// </summary>
        /// <param name="accessors">A class containing <see cref="IStatePropertyAccessor{T}"/> used to manage state.</param>
        public EchoWithCounterBot(EchoBotAccessors accessors)
        {
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));
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
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Get the conversation state and default to empty counter state when no state exists.
                var oldState = await _accessors.CounterState.GetAsync(turnContext, () => new CounterState());

                // Bump the turn count for this conversation.
                var newState = new CounterState { TurnCount = oldState.TurnCount + 1 };

                // Set the state
                await _accessors.CounterState.SetAsync(turnContext, newState);

                // Save the new turn count into the conversation state.
                await _accessors.ConversationState.SaveChangesAsync(turnContext);

                // Echo back to the user whatever they typed with turn count.
                var responseMessage = $"Turn count: `{newState.TurnCount}`<br>" +
                                      $"You sent : '`{turnContext.Activity.Text}`'<br>";
                await turnContext.SendActivityAsync(responseMessage);
            }
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
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
    public class SimplePromptBot : IBot
    {
        /// <summary>
        /// The <see cref="DialogSet"/> that contains all the Dialogs that can be used at runtime.
        /// </summary>
        private readonly DialogSet _dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplePromptBot"/> class.
        /// </summary>
        /// <param name="accessors">The state accessors this instance will be needing at runtime.</param>
        public SimplePromptBot(BotAccessors accessors)
        {
            _dialogs = new DialogSet(accessors.ConversationDialogState);
            _dialogs.Add(new TextPrompt("name"));
        }

        /// <summary>
        /// This controls what happens when an <see cref="Activity"/> gets sent to the bot.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            // We are only interested in Message Activities.
            if (turnContext.Activity.Type != ActivityTypes.Message)
            {
                return;
            }

            // Run the DialogSet - let the framework identify the current state of the dialog from
            // the dialog stack and figure out what (if any) is the active dialog.
            var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
            var results = await dialogContext.ContinueAsync(cancellationToken);

            // If the DialogTurnStatus is Empty we should start a new dialog.
            if (results.Status == DialogTurnStatus.Empty)
            {
                // A prompt dialog can be started directly on from the DialogContext. The prompt text is given in the PromptOptions.
                await dialogContext.PromptAsync(
                    "name",
                    new PromptOptions { Prompt = MessageFactory.Text("please enter your name") },
                    cancellationToken);
            }

            // We had a dialog run (it was the prompt) now it is complete.
            else if (results.Status == DialogTurnStatus.Complete)
            {
                // Check for a result.
                if (results.Result != null)
                {
                    // And finish by sending a message to the user. Next time ContinueAsync is called it will return DialogTurnStatus.Empty.
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Thank you, I have your name as '{results.Result}'."));
                }
            }
        }
    }
}

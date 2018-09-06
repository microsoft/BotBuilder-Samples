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
    public class MultiTurnPromptsBot : IBot
    {
        /// <summary>
        /// We will be needing the user state accessor within the bot logic.
        /// </summary>
        private IStatePropertyAccessor<UserProfile> _userProfileAccessor;

        /// <summary>
        /// The <see cref="DialogSet"/> that contains all the Dialogs that can be used at runtime.
        /// </summary>
        private DialogSet _dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiTurnPromptsBot"/> class.
        /// </summary>
        /// <param name="accessors">A class containing <see cref="IStatePropertyAccessor{T}"/> used to manage state.</param>
        public MultiTurnPromptsBot(BotAccessors accessors)
        {
            // We need a handle on the accessor so we can load the data when we have the turn context.
            _userProfileAccessor = accessors.UserProfile;

            // The DialogSet needs a DialogState accessor, it will call it when it has a turn context.
            _dialogs = new DialogSet(accessors.ConversationDialogState);

            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {
                NameStepAsync,
                NameConfirmStepAsync,
                AgeStepAsync,
                ConfirmStepAsync,
                SummaryStepAsync,
            };

            // Add all the different named dialogs to the DialogSet. It is these names that will be saved in the serialized dialog state.
            _dialogs.Add(new WaterfallDialog("details", waterfallSteps));
            _dialogs.Add(new TextPrompt("name"));
            _dialogs.Add(new NumberPrompt<int>("age"));
            _dialogs.Add(new ConfirmPrompt("confirm"));
        }

        /// <summary>
        /// This controls what happens when an activity gets sent to the bot.
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
                await dialogContext.BeginAsync("details", null, cancellationToken);
            }
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>. This contains the main business logic of the bot.
        /// </summary>
        /// <param name="dc">The <see cref="DialogContext"/> that gives access to the core runtime.</param>
        /// <param name="stepContext">The <see cref="WaterfallStepContext="/> for additional runtime state associated with this executing <see cref="Waterfall"/> instance.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private static async Task<DialogTurnResult> NameStepAsync(DialogContext dc, WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the users response is received.
            return await dc.PromptAsync("name", new PromptOptions { Prompt = MessageFactory.Text("Please enter your name.") }, cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>. This contains the main business logic of the bot.
        /// </summary>
        /// <param name="dc">The <see cref="DialogContext"/> that gives access to the core runtime.</param>
        /// <param name="stepContext">The <see cref="WaterfallStepContext="/> for additional runtime state associated with this executing <see cref="Waterfall"/> instance.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> NameConfirmStepAsync(DialogContext dc, WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the current profile object from user state.
            var userProfile = await _userProfileAccessor.GetAsync(dc.Context, () => new UserProfile(), cancellationToken);

            // Update the profile.
            userProfile.Name = (string)stepContext.Result;

            // We can send messages to the user at any point in the WaterfallStep.
            await dc.Context.SendActivityAsync(MessageFactory.Text($"Thanks {stepContext.Result}."), cancellationToken);

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is a Prompt Dialog.
            return await dc.PromptAsync("confirm", new PromptOptions { Prompt = MessageFactory.Text("Would you like to give your age?") }, cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>. This contains the main business logic of the bot.
        /// </summary>
        /// <param name="dc">The <see cref="DialogContext"/> that gives access to the core runtime.</param>
        /// <param name="stepContext">The <see cref="WaterfallStepContext="/> for additional runtime state associated with this executing <see cref="Waterfall"/> instance.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> AgeStepAsync(DialogContext dc, WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                // User said "yes" so we will be prompting for the age.

                // Get the current profile object from user state.
                var userProfile = await _userProfileAccessor.GetAsync(dc.Context, () => new UserProfile(), cancellationToken);

                // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is a Prompt Dialog.
                return await dc.PromptAsync("age", new PromptOptions { Prompt = MessageFactory.Text("Please enter your age.") }, cancellationToken);
            }
            else
            {
                // User said "no" so we will skip the next step. Give -1 as the age.
                return await stepContext.NextAsync(-1, cancellationToken);
            }
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>. This contains the main business logic of the bot.
        /// </summary>
        /// <param name="dc">The <see cref="DialogContext"/> that gives access to the core runtime.</param>
        /// <param name="stepContext">The <see cref="WaterfallStepContext="/> for additional runtime state associated with this executing <see cref="Waterfall"/> instance.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> ConfirmStepAsync(DialogContext dc, WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the current profile object from user state.
            var userProfile = await _userProfileAccessor.GetAsync(dc.Context, () => new UserProfile(), cancellationToken);

            // Update the profile.
            userProfile.Age = (int)stepContext.Result;

            // We can send messages to the user at any point in the WaterfallStep.
            if (userProfile.Age == -1)
            {
                await dc.Context.SendActivityAsync(MessageFactory.Text($"No age given."), cancellationToken);
            }
            else
            {
                // We can send messages to the user at any point in the WaterfallStep.
                await dc.Context.SendActivityAsync(MessageFactory.Text($"I have your age as {userProfile.Age}."), cancellationToken);
            }

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is a Prompt Dialog.
            return await dc.PromptAsync("confirm", new PromptOptions { Prompt = MessageFactory.Text("Is this ok?") }, cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>. This contains the main business logic of the bot.
        /// </summary>
        /// <param name="dc">The <see cref="DialogContext"/> that gives access to the core runtime.</param>
        /// <param name="stepContext">The <see cref="WaterfallStepContext="/> for additional runtime state associated with this executing <see cref="Waterfall"/> instance.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> SummaryStepAsync(DialogContext dc, WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                // Get the current profile object from user state.
                var userProfile = await _userProfileAccessor.GetAsync(dc.Context, () => new UserProfile(), cancellationToken);

                // We can send messages to the user at any point in the WaterfallStep.
                if (userProfile.Age == -1)
                {
                    await dc.Context.SendActivityAsync(MessageFactory.Text($"I have your name as {userProfile.Name}."), cancellationToken);
                }
                else
                {
                    await dc.Context.SendActivityAsync(MessageFactory.Text($"I have your name as {userProfile.Name} and age as {userProfile.Age}."), cancellationToken);
                }
            }
            else
            {
                // We can send messages to the user at any point in the WaterfallStep.
                await dc.Context.SendActivityAsync(MessageFactory.Text("Thanks. Your profile will not be kept."), cancellationToken);
            }

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is the end.
            return await dc.EndAsync(cancellationToken);
        }
    }
}

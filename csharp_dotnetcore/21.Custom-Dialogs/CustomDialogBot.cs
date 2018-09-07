// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text;

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
    public class CustomDialogBot : IBot
    {
        private readonly BotAccessors _accessors;

        /// <summary>
        /// The <see cref="DialogSet"/> that contains all the Dialogs that can be used at runtime.
        /// </summary>
        private DialogSet _dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDialogBot"/> class.
        /// </summary>
        /// <param name="accessors">A class containing <see cref="IStatePropertyAccessor{T}"/> used to manage state.</param>
        public CustomDialogBot(BotAccessors accessors)
        {
            _accessors = accessors ?? throw new ArgumentNullException(nameof(accessors));

            // The DialogSet needs a DialogState accessor, it will call it when it has a turn context.
            _dialogs = new DialogSet(accessors.ConversationDialogState);

            var name_slots = new List<SlotDetails>
            {
                new SlotDetails { Name = "first", PromptId = "text", PromptText = "Please enter your first name." },
                new SlotDetails { Name = "last", PromptId = "text", PromptText = "Please enter your last name." },
            };

            var address_slots = new List<SlotDetails>
            {
                new SlotDetails { Name = "street", PromptId = "text", PromptText = "Please enter the street." },
                new SlotDetails { Name = "city", PromptId = "text", PromptText = "Please enter the city." },
                new SlotDetails { Name = "zip", PromptId = "text", PromptText = "Please enter the zip." },
            };

            var slots = new List<SlotDetails>
            {
                new SlotDetails { Name = "fullname", PromptId = "fullname" },
                new SlotDetails { Name = "age", PromptId = "number", PromptText = "Please enter your age." },
                new SlotDetails { Name = "shoesize", PromptId = "number", PromptText = "Please enter your shoe size." },
                new SlotDetails { Name = "nickname", PromptId = "text", PromptText = "Please enter your nickname." },
                new SlotDetails { Name = "address", PromptId = "address" },
            };

            // Add the various dialogs we will be using to teh DialogSet
            _dialogs.Add(new SlotFillingDialog("address", address_slots));
            _dialogs.Add(new SlotFillingDialog("fullname", name_slots));
            _dialogs.Add(new TextPrompt("text"));
            _dialogs.Add(new NumberPrompt<int>("number", defaultLocale: Culture.English));
            _dialogs.Add(new SlotFillingDialog("slot-dialog", slots));

            // Add a simple two step Waterfall to test the slot dialog.
            _dialogs.Add(new WaterfallDialog(
                "root",
                new WaterfallStep[]
                {
                    async (dc, wc, ct) =>
                    {
                        return await dc.BeginAsync("slot-dialog").ConfigureAwait(false);
                    },
                    async (dc, wc, ct) =>
                    {
                        var result = wc.Result as IDictionary<string, object>;

                        if (result != null && result.Count > 0)
                        {
                            var fullname = (IDictionary<string, object>)result["fullname"];
                            await dc.Context.SendActivityAsync(MessageFactory.Text($"{fullname["first"]} {fullname["last"]}"), ct);

                            await dc.Context.SendActivityAsync(MessageFactory.Text($"{result["shoesize"]} {result["nickname"]}"), ct);

                            var address = (IDictionary<string, object>)result["address"];
                            await dc.Context.SendActivityAsync(MessageFactory.Text($"{address["street"]} {address["city"]} {address["zip"]}"), ct);
                        }

                        return Dialog.EndOfTurn;
                    },
                }));
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
                await dialogContext.BeginAsync("root", null, cancellationToken);
            }

            // Save the dialog state into the conversation state.
            await _accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }
    }
}

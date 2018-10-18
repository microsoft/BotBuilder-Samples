// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
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
    /// This is a Transient lifetime service. Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// For example, the <see cref="MemoryStorage"/> object and associated
    /// <see cref="IStatePropertyAccessor{T}"/> object are created with a singleton lifetime.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    public class CustomDialogBot : IBot
    {
        private const string WelcomeText = "Welcome to Custom Dialog Bot. This bot uses a custom dialog that executes a data driven flow.  Type anything to get started.";

        /// <summary>
        /// A handle on the property accessor and BotState used by the dialog state.
        /// </summary>
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

            // The DialogSet needs a DialogState accessor which will be used during the turn.
            _dialogs = new DialogSet(accessors.ConversationDialogState);

            // Rather than explicitly coding a Waterfall we have only to declare what properties we want collected.
            // In this example we will want two text prompts to run, one for the first name and one for the last.
            var fullname_slots = new List<SlotDetails>
            {
                new SlotDetails("first", "text", "Please enter your first name."),
                new SlotDetails("last", "text", "Please enter your last name."),
            };

            // This defines an address dialog that collects street, city and zip properties.
            var address_slots = new List<SlotDetails>
            {
                new SlotDetails("street", "text", "Please enter the street."),
                new SlotDetails("city", "text", "Please enter the city."),
                new SlotDetails("zip", "text", "Please enter the zip."),
            };

            // Dialogs can be nested and the slot filling dialog makes use of that. In this example some of the child
            // dialogs are slot filling dialogs themselves.
            var slots = new List<SlotDetails>
            {
                new SlotDetails("fullname", "fullname"),
                new SlotDetails("age", "number", "Please enter your age."),
                new SlotDetails("shoesize", "shoesize", "Please enter your shoe size.", "You must enter a size between 0 and 16. Half sizes are acceptable."),
                new SlotDetails("address", "address"),
            };

            // Add the various dialogs that will be used to the DialogSet.
            _dialogs.Add(new SlotFillingDialog("address", address_slots));
            _dialogs.Add(new SlotFillingDialog("fullname", fullname_slots));
            _dialogs.Add(new TextPrompt("text"));
            _dialogs.Add(new NumberPrompt<int>("number", defaultLocale: Culture.English));
            _dialogs.Add(new NumberPrompt<float>("shoesize", ShoeSizeAsync, defaultLocale: Culture.English));
            _dialogs.Add(new SlotFillingDialog("slot-dialog", slots));

            // Defines a simple two step Waterfall to test the slot dialog.
            _dialogs.Add(new WaterfallDialog("root", new WaterfallStep[] { StartDialogAsync, ProcessResultsAsync }));
        }

        /// <summary>
        /// This controls what happens when an activity gets sent to the bot.
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
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Run the DialogSet - let the framework identify the current state of the dialog from
                // the dialog stack and figure out what (if any) is the active dialog.
                var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
                var results = await dialogContext.ContinueDialogAsync(cancellationToken);

                // If the DialogTurnStatus is Empty we should start a new dialog.
                if (results.Status == DialogTurnStatus.Empty)
                {
                    await dialogContext.BeginDialogAsync("root", null, cancellationToken);
                }
            }

            // Processes ConversationUpdate Activities to welcome the user.
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    await SendWelcomeMessageAsync(turnContext, cancellationToken);
                }
            }
            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} activity detected", cancellationToken: cancellationToken);
            }

            // Save the dialog state into the conversation state.
            await _accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        /// <summary>
        /// Sends a welcome message to the user.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var reply = turnContext.Activity.CreateReply();
                    reply.Text = WelcomeText;
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
            }
        }

        /// <summary>
        /// This is an example of a custom validator. This example can be directly used on a float NumberPrompt.
        /// Returning true indicates the recognized value is acceptable. Returning false will trigger re-prompt behavior.
        /// </summary>
        /// <param name="promptContext">The <see cref="PromptValidatorContext"/> gives the validator code access to the runtime, including the recognized value and the turn context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A an asynchronous Task of bool indicating validation success as true.</returns>
        private Task<bool> ShoeSizeAsync(PromptValidatorContext<float> promptContext, CancellationToken cancellationToken)
        {
            var shoesize = promptContext.Recognized.Value;

            // show sizes can range from 0 to 16
            if (shoesize >= 0 && shoesize <= 16)
            {
                // we only accept round numbers or half sizes
                if (Math.Floor(shoesize) == shoesize || Math.Floor(shoesize * 2) == shoesize * 2)
                {
                    // indicate success by returning the value
                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> StartDialogAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Start the child dialog. This will run the top slot dialog than will complete when all the properties are gathered.
            return await stepContext.BeginDialogAsync("slot-dialog", null, cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> ProcessResultsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // To demonstrate that the slot dialog collected all the properties we will echo them back to the user.
            if (stepContext.Result is IDictionary<string, object> result && result.Count > 0)
            {
                var fullname = (IDictionary<string, object>)result["fullname"];
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"{fullname["first"]} {fullname["last"]}"), cancellationToken);

                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"{result["shoesize"]}"), cancellationToken);

                var address = (IDictionary<string, object>)result["address"];
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"{address["street"]} {address["city"]} {address["zip"]}"), cancellationToken);
            }

            // Remember to call EndAsync to indicate to the runtime that this is the end of our waterfall.
            return await stepContext.EndDialogAsync();
        }
    }
}

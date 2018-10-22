// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace BasicBot
{
    /// <summary>
    /// The <see cref="RouterDialog"/> is the entry point or "main" dialog.
    /// </summary>
    /// <remarks>
    /// The <see cref="RouterDialog"/> is the first dialog that runs when a user begins a conversation.
    /// Derived classes typically perform the following:
    /// - Start message.
    ///   Display the initial message the user sees when they begin a conversation.
    /// - Help.
    ///   Provide the user about the commands the bot can process.
    /// - Start other dialogs to perform more complex operations.
    ///   Determine the user's intentions and begin the appropriate <see cref="Dialog"/> to service
    ///   the request.
    /// </remarks>
    public abstract class RouterDialog : ComponentDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouterDialog"/> class.
        /// </summary>
        /// <param name="dialogId">Identifier for the dialog system.  The identifier must be unique
        /// within the containing <see cref="ComponentDialog"/>.</param>
        public RouterDialog(string dialogId)
            : base(dialogId)
        {
        }

        protected override Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options, CancellationToken cancellationToken = default(CancellationToken)) => OnContinueDialogAsync(innerDc, cancellationToken);

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var activity = innerDc.Context.Activity;

            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    {
                        var result = await innerDc.ContinueDialogAsync();

                        switch (result.Status)
                        {
                            case DialogTurnStatus.Empty:
                                {
                                    await RouteAsync(innerDc);
                                    break;
                                }

                            case DialogTurnStatus.Complete:
                                {
                                    await CompleteAsync(innerDc);

                                    // End active dialog
                                    await innerDc.EndDialogAsync();
                                    break;
                                }

                            default:
                                {
                                    break;
                                }
                        }

                        break;
                    }

                case ActivityTypes.Event:
                    {
                        await OnEventAsync(innerDc);
                        break;
                    }

                case ActivityTypes.ConversationUpdate:
                    {
                        await OnStartAsync(innerDc);
                        break;
                    }

                default:
                    {
                        await OnSystemMessageAsync(innerDc);
                        break;
                    }
            }

            return EndOfTurn;
        }

        protected override Task OnEndDialogAsync(ITurnContext context, DialogInstance instance, DialogReason reason, CancellationToken cancellationToken = default(CancellationToken)) => base.OnEndDialogAsync(context, instance, reason, cancellationToken);

        protected override Task OnRepromptDialogAsync(ITurnContext turnContext, DialogInstance instance, CancellationToken cancellationToken = default(CancellationToken)) => base.OnRepromptDialogAsync(turnContext, instance, cancellationToken);

        /// <summary>
        /// Called when the inner dialog stack is empty.
        /// </summary>
        /// <param name="innerDc">The dialog context for the component.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected abstract Task RouteAsync(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Called when the inner dialog stack is complete.
        /// </summary>
        /// <param name="innerDc">The dialog context for the component.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual Task CompleteAsync(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken)) => Task.CompletedTask;

        /// <summary>
        /// Called when an event activity is received.
        /// </summary>
        /// <param name="innerDc">The dialog context for the component.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual Task OnEventAsync(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken)) => Task.CompletedTask;

        /// <summary>
        /// Called when a system activity is received.
        /// </summary>
        /// <param name="innerDc">The dialog context for the component.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual Task OnSystemMessageAsync(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken)) => Task.CompletedTask;

        /// <summary>
        /// Called when a conversation update activity is received.
        /// </summary>
        /// <param name="innerDc">The dialog context for the component.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual Task OnStartAsync(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken)) => Task.CompletedTask;
    }
}

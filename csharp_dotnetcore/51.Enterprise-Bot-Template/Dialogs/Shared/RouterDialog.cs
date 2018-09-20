using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace EnterpriseBot
{
    public abstract class RouterDialog : ComponentDialog
    {
        public RouterDialog(string dialogId)
            : base(dialogId)
        {
        }

        protected override Task<DialogTurnResult> OnDialogBeginAsync(DialogContext innerDc, object options, CancellationToken cancellationToken = default(CancellationToken)) => OnDialogContinueAsync(innerDc, cancellationToken);

        protected override async Task<DialogTurnResult> OnDialogContinueAsync(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var activity = innerDc.Context.Activity;

            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    {
                        var result = await innerDc.ContinueAsync();

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
                                    await innerDc.EndAsync();
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

        protected override Task OnDialogEndAsync(ITurnContext context, DialogInstance instance, DialogReason reason, CancellationToken cancellationToken = default(CancellationToken)) => base.OnDialogEndAsync(context, instance, reason, cancellationToken);

        protected override Task OnDialogRepromptAsync(ITurnContext turnContext, DialogInstance instance, CancellationToken cancellationToken = default(CancellationToken)) => base.OnDialogRepromptAsync(turnContext, instance, cancellationToken);

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

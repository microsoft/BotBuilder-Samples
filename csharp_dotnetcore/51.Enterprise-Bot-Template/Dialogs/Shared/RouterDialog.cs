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
                                    await OnComplete(innerDc);

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
                        await OnEvent(innerDc);
                        break;
                    }

                case ActivityTypes.ConversationUpdate:
                    {
                        await OnStart(innerDc);
                        break;
                    }

                default:
                    {
                        await OnSystemMessage(innerDc);
                        break;
                    }
            }

            return EndOfTurn;
        }

        protected override Task OnDialogEndAsync(ITurnContext context, DialogInstance instance, DialogReason reason, CancellationToken cancellationToken = default(CancellationToken)) => base.OnDialogEndAsync(context, instance, reason, cancellationToken);

        protected override Task OnDialogRepromptAsync(ITurnContext turnContext, DialogInstance instance, CancellationToken cancellationToken = default(CancellationToken)) => base.OnDialogRepromptAsync(turnContext, instance, cancellationToken);

        protected abstract Task RouteAsync(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken));

        protected virtual Task OnStart(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken)) => Task.CompletedTask;

        protected virtual Task OnComplete(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken)) => Task.CompletedTask;

        protected virtual Task OnEvent(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken)) => Task.CompletedTask;

        protected virtual Task OnSystemMessage(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken)) => Task.CompletedTask;
    }
}

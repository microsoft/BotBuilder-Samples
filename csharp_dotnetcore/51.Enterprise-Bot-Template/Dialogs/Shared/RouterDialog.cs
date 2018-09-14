using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace EnterpriseBot
{
    public abstract class RouterDialog : ComponentDialog
    {
        public RouterDialog(string dialogId)
            : base(dialogId)
        {
        }

        protected override Task<DialogTurnResult> OnDialogBeginAsync(DialogContext innerDc, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
            return OnDialogContinueAsync(innerDc, cancellationToken);
        }

        protected override async Task<DialogTurnResult> OnDialogContinueAsync(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await innerDc.ContinueAsync();
            return await RouteAsync(innerDc, result);
        }

        protected override Task OnDialogEndAsync(ITurnContext context, DialogInstance instance, DialogReason reason, CancellationToken cancellationToken = default(CancellationToken)) => base.OnDialogEndAsync(context, instance, reason, cancellationToken);

        protected override Task OnDialogRepromptAsync(ITurnContext turnContext, DialogInstance instance, CancellationToken cancellationToken = default(CancellationToken)) => base.OnDialogRepromptAsync(turnContext, instance, cancellationToken);

        protected abstract Task<DialogTurnResult> RouteAsync(DialogContext innerDc, DialogTurnResult result, CancellationToken cancellationToken = default(CancellationToken));
    }
}

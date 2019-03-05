// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.BotBuilderSamples
{
    public class CancelAndHelpDialog : ComponentDialog
    {
        public CancelAndHelpDialog(string id)
            : base(id)
        {
        }

        protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options, CancellationToken cancellationToken)
        {
            await InterceptHelpAndCancel(innerDc, cancellationToken);

            return await base.OnBeginDialogAsync(innerDc, options, cancellationToken);
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            await InterceptHelpAndCancel(innerDc, cancellationToken);

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }

        private async Task InterceptHelpAndCancel(DialogContext innerDc, CancellationToken cancellationToken)
        {
            var text = innerDc.Context.Activity.Text.ToLowerInvariant();

            switch (text)
            {
                case "help":
                    await innerDc.Context.SendActivityAsync($"Show Help");
                    break;
                case "cancel":
                    await innerDc.Context.SendActivityAsync($"Cancelling");
                    await innerDc.CancelAllDialogsAsync();
                    break;
            }
        }
    }
}

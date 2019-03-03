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
            await innerDc.Context.SendActivityAsync($"OnBeginDialogAsync: {innerDc.Context.Activity.Text}");

            // check for help or cancel (no need to use LUIS for this but we could)

            return await base.OnBeginDialogAsync(innerDc, options, cancellationToken);
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            await innerDc.Context.SendActivityAsync($"OnContinueDialogAsync: {innerDc.Context.Activity.Text}");

            // check for help or cancel (no need to use LUIS for this but we could)

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Luis;
using Microsoft.Bot.Builder.Dialogs;

namespace EnterpriseBot
{
    public class EnterpriseDialog : InterruptableDialog
    {
        // Fields
        private readonly BotServices _services;
        private readonly CancelResponses _responder = new CancelResponses();

        public EnterpriseDialog(BotServices botServices, string dialogId)
            : base(dialogId)
        {
            _services = botServices;

            AddDialog(new CancelDialog());
        }

        protected override async Task<InterruptionStatus> OnDialogInterruptionAsync(DialogContext dc, CancellationToken cancellationToken)
        {
            // Check dispatch intent.
            var luisService = _services.LuisServices["<YOUR MSBOT NAME>_General"];
            var luisResult = await luisService.RecognizeAsync<General>(dc.Context, cancellationToken);
            var intent = luisResult.TopIntent().intent;

            switch (intent)
            {
                case General.Intent.Cancel:
                    {
                        return await OnCancel(dc);
                    }

                case General.Intent.Help:
                    {
                        return await OnHelp(dc);
                    }
            }

            return InterruptionStatus.NoAction;
        }

        protected virtual async Task<InterruptionStatus> OnCancel(DialogContext dc)
        {
            if (dc.ActiveDialog.Id != nameof(CancelDialog))
            {
                // Don't start restart cancel dialog.
                await dc.BeginDialogAsync(nameof(CancelDialog));

                // Signal that the dialog is waiting on user response.
                return InterruptionStatus.Waiting;
            }

            // Else, continue
            return InterruptionStatus.NoAction;
        }

        protected virtual async Task<InterruptionStatus> OnHelp(DialogContext dc)
        {
            var view = new MainResponses();
            await view.ReplyWith(dc.Context, MainResponses.Help);

            // Signal the conversation was interrupted and should immediately continue.
            return InterruptionStatus.Interrupted;
        }
    }
}

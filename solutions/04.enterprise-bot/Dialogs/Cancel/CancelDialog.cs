// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace EnterpriseBot
{
    public class CancelDialog : ComponentDialog
    {
        // Constants
        public const string CancelPrompt = "cancelPrompt";

        // Fields
        private static CancelResponses _responder = new CancelResponses();

        public CancelDialog()
            : base(nameof(CancelDialog))
        {
            InitialDialogId = nameof(CancelDialog);

            var cancel = new WaterfallStep[]
            {
                AskToCancel,
                FinishCancelDialog,
            };

            AddDialog(new WaterfallDialog(InitialDialogId, cancel));
            AddDialog(new ConfirmPrompt(CancelPrompt));
        }

        public static async Task<DialogTurnResult> AskToCancel(WaterfallStepContext sc, CancellationToken cancellationToken) => await sc.PromptAsync(CancelPrompt, new PromptOptions()
        {
            Prompt = await _responder.RenderTemplate(sc.Context, "en", CancelResponses._confirmPrompt),
        });

        public static async Task<DialogTurnResult> FinishCancelDialog(WaterfallStepContext sc, CancellationToken cancellationToken) => await sc.EndDialogAsync((bool)sc.Result);

        protected override async Task<DialogTurnResult> EndComponentAsync(DialogContext outerDc, object result, CancellationToken cancellationToken)
        {
            var doCancel = (bool)result;

            if (doCancel)
            {
                // If user chose to cancel
                await _responder.ReplyWith(outerDc.Context, CancelResponses._cancelConfirmed);

                // Cancel all in outer stack of component i.e. the stack the component belongs to
                return await outerDc.CancelAllDialogsAsync();
            }
            else
            {
                // else if user chose not to cancel
                await _responder.ReplyWith(outerDc.Context, CancelResponses._cancelDenied);

                // End this component. Will trigger reprompt/resume on outer stack
                return await outerDc.EndDialogAsync();
            }
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace MessageRoutingBot
{
    public class CancelDialog : ComponentDialog
    {
        // Constants
        public const string Name = "cancel";
        public const string CancelPrompt = "cancelPrompt";

        // Fields
        private static CancelResponses _responder = new CancelResponses();

        public CancelDialog()
            : base(Name)
        {
            var cancel = new WaterfallStep[]
            {
                AskToCancel,
                FinishCancelDialog,
            };

            AddDialog(new WaterfallDialog(Name, cancel));
            AddDialog(new ConfirmPrompt(CancelPrompt));
        }

        public static async Task<DialogTurnResult> AskToCancel(DialogContext dc, WaterfallStepContext args, CancellationToken cancellationToken)
        {
            return await dc.PromptAsync(CancelPrompt, new PromptOptions()
            {
                Prompt = await _responder.RenderTemplate(dc.Context, "en", CancelResponses._confirmPrompt),
            });
        }

        public static async Task<DialogTurnResult> FinishCancelDialog(DialogContext dc, WaterfallStepContext args, CancellationToken cancellationToken)
        {
            return await dc.EndAsync((bool)args.Result);
        }

        protected override async Task<DialogTurnResult> EndComponentAsync(DialogContext outerDc, object result, CancellationToken cancellationToken)
        {
            bool doCancel = (bool)result;

            if (doCancel)
            {
                // If user chose to cancel
                await _responder.ReplyWith(outerDc.Context, CancelResponses._cancelConfirmed);

                // Cancel all in outer stack of component i.e. the stack the component belongs to
                return await outerDc.CancelAllAsync();
            }
            else
            {
                // else if user chose not to cancel
                await _responder.ReplyWith(outerDc.Context, CancelResponses._cancelDenied);

                // End this component. Will trigger reprompt/resume on outer stack
                return await outerDc.EndAsync();
            }
        }
    }
}

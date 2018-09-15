// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace MessageRoutingBot
{
    /// <summary>
    /// Dialog to handle Cancellation of interruptable dialogs.
    /// </summary>
    public class CancelDialog : ComponentDialog
    {
        // Constants
        public const string Name = "cancel";
        public const string CancelPrompt = "cancelPrompt";

        // Fields
        private static CancelResponses _responder = new CancelResponses();

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelDialog"/> class.
        /// </summary>
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

        private static async Task<DialogTurnResult> AskToCancel(DialogContext dc, WaterfallStepContext args, CancellationToken cancellationToken)
        {
            return await dc.PromptAsync(CancelPrompt, new PromptOptions()
            {
                Prompt = await _responder.RenderTemplate(dc.Context, "en", CancelResponses.confirmPrompt),
            });
        }

        private static async Task<DialogTurnResult> FinishCancelDialog(DialogContext dc, WaterfallStepContext args, CancellationToken cancellationToken)
        {
            return await dc.EndAsync((bool)args.Result);
        }

        protected override async Task<DialogTurnResult> EndComponentAsync(DialogContext outerDc, object result, CancellationToken cancellationToken)
        {
            bool doCancel = (bool)result;

            if (doCancel)
            {
                // If user chose to cancel
                await _responder.ReplyWith(outerDc.Context, CancelResponses.cancelConfirmed);

                // Cancel all in outer stack of component i.e. the stack the component belongs to
                return await outerDc.CancelAllAsync();
            }
            else
            {
                // else if user chose not to cancel
                await _responder.ReplyWith(outerDc.Context, CancelResponses.cancelDenied);

                // End this component. Will trigger reprompt/resume on outer stack
                return await outerDc.EndAsync();
            }
        }
    }
}

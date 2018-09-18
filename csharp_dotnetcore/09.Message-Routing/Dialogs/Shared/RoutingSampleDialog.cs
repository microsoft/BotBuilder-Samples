// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Luis;
using Microsoft.Bot.Builder.Dialogs;

namespace MessageRoutingBot
{
    /// <summary>
    /// Interruptable dialog specialization for the Message Routing sample.
    /// </summary>
    public class RoutingSampleDialog : InterruptableDialog
    {
        private readonly BotServices _services;
        private readonly CancelResponses _responder = new CancelResponses();

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingSampleDialog"/> class.
        /// </summary>
        /// <param name="botServices">The <see cref=" BotServices" />for the bot.</param>
        /// <param name="dialogId">Id of the dialog.</param>
        public RoutingSampleDialog(BotServices botServices, string dialogId)
            : base(dialogId)
        {
            _services = botServices;

            AddDialog(new CancelDialog());
        }

        /// <summary>
        /// Handle dialog interruption.
        /// </summary>
        /// <param name="dc">The current <see cref="DialogContext"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to control cancellation of asynchronous tasks.</param>
        /// <returns>A <see cref="Task"/> representing the <see cref="InterruptionStatus"/>.</returns>
        protected override async Task<InterruptionStatus> OnDialogInterruptionAsync(DialogContext dc, CancellationToken cancellationToken)
        {
            const string luisName = "General.luis";

            var luisService = _services.LuisServices[luisName];
            var luisResult = await luisService.RecognizeAsync<MessageRoutingBot_General>(dc.Context, cancellationToken);
            var intent = luisResult.TopIntent().intent;

            switch (intent)
            {
                case MessageRoutingBot_General.Intent.Cancel:
                    {
                        return await OnCancel(dc);
                    }

                case MessageRoutingBot_General.Intent.Help:
                    {
                        return await OnHelp(dc);
                    }
            }

            return InterruptionStatus.NoAction;
        }

        /// <summary>
        /// Handle dialog cancellation.
        /// </summary>
        /// <param name="dc">The current <see cref="DialogContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the <see cref="InterruptionStatus"/>.</returns>
        protected virtual async Task<InterruptionStatus> OnCancel(DialogContext dc)
        {
            if (dc.ActiveDialog.Id != CancelDialog.Name)
            {
                // Don't start restart cancel dialog
                await dc.BeginAsync(CancelDialog.Name);

                // Signal that the dialog is waiting on user response
                return InterruptionStatus.Waiting;
            }

            // Else, continue
            return InterruptionStatus.NoAction;
        }

        /// <summary>
        /// Handle help requests.
        /// </summary>
        /// <param name="dc">The current <see cref="DialogContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the <see cref="InterruptionStatus"/>.</returns>
        protected virtual async Task<InterruptionStatus> OnHelp(DialogContext dc)
        {
            var view = new MainResponses();
            await view.ReplyWith(dc.Context, MainResponses.Help);

            // Signal the conversation was interrupted and should immediately continue
            return InterruptionStatus.Interrupted;
        }
    }
}

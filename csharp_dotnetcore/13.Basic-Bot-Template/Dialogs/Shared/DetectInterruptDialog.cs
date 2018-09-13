// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// Detects interrupts using a LUIS model.
    /// </summary>
    /// <remarks>This inherits from <see cref="InterruptableDialog"/> and implements help and cancel
    /// interruptions. This peeks at input before it reaches any derived class and detects if
    /// an "interruption" (ie, "help", "cancel") was performed by the user that signals
    /// the user is not following the dialog prompts.</remarks>
    public class DetectInterruptDialog : InterruptableDialog
    {
        // Supported LUIS Interrupt Intents
        public const string CancelIntent = "Cancel";
        public const string HelpIntent = "Help";
        public const string NoneIntent = "None";

        /// <summary>
        /// Key in the bot config (.bot file) for the LUIS instance.
        /// In the .bot file, multiple instances of LUIS can be configured.
        /// </summary>
        /// <remarks>A common LUIS model is used for both interruptions and greetings.
        /// A distinct LUIS model and/or other method(s) (ie, QnA) could be used here to determine
        /// interruptions.</remarks>
        public static readonly string LuisKey = "BasicBotLUIS";

        /// <summary>
        /// Initializes a new instance of the <see cref="DetectInterruptDialog"/> class.
        /// </summary>
        /// <param name="botServices">The <see cref=" BotServices" />for the bot.</param>
        /// <param name="dialogId">Id of the dialog.</param>
        public DetectInterruptDialog(BotServices botServices, string dialogId, ILogger logger)
            : base(dialogId)
        {
            Services = botServices;
            Logger = logger;
        }

        // External services (ie, LUIS)
        protected BotServices Services { get; }

        // Logging for trace/error/exceptions.
        protected ILogger Logger { get; }

        /// <summary>
        /// Handle dialog interruption.
        /// </summary>
        /// <param name="dc">The current <see cref="DialogContext"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to control cancellation of asynchronous tasks.</param>
        /// <returns>A <see cref="Task"/> representing the <see cref="InterruptionStatus"/>.</returns>
        /// <remarks>Determines of there are any interruptions that were entered by the user (using a LUIS model).</remarks>
        protected override async Task<InterruptionStatus> OnDialogInterruptionAsync(DialogContext dc, CancellationToken cancellationToken)
        {
            // Perform a call to LUIS to retrieve results for the current activity message.
            var luisResults = await Services.LuisServices[LuisKey].RecognizeAsync(dc.Context, cancellationToken).ConfigureAwait(false);
            var topIntent = luisResults?.GetTopScoringIntent();

            // See if there are any conversation interrupts we need to handle
            switch (topIntent.Value.intent)
            {
                case CancelIntent:
                    if (topIntent.Value.score > .5)
                    {
                        return await OnCancelAsync(dc);
                    }

                    break;

                case HelpIntent:
                    if (topIntent.Value.score > .5)
                    {
                        return await OnHelpAsync(dc);
                    }

                    break;
            }

            Logger.LogTrace($"No interruption. '{topIntent.Value.intent}' LUIS intent will be processed by dialog.");

            return InterruptionStatus.NoAction;
        }

        /// <summary>
        /// Handle dialog cancellation.
        /// </summary>
        /// <param name="dc">The current <see cref="DialogContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the <see cref="InterruptionStatus"/>.</returns>
        protected virtual async Task<InterruptionStatus> OnCancelAsync(DialogContext dc)
        {
            if (dc.ActiveDialog != null)
            {
                await dc.CancelAllAsync().ConfigureAwait(false);
                await dc.Context.SendActivityAsync("Ok. I've cancelled our last activity.");
            }
            else
            {
                await dc.Context.SendActivityAsync("I don't have anything to cancel.");
            }

            // Else, continue
            return InterruptionStatus.NoAction;
        }

        /// <summary>
        /// Handle help requests.
        /// </summary>
        /// <param name="dc">The current <see cref="DialogContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the <see cref="InterruptionStatus"/>.</returns>
        /// <remarks>The help here is provided in the context of the overarching dialog.  This is distinct
        /// from the help that is displayed from the MainDialog.</remarks>
        protected virtual async Task<InterruptionStatus> OnHelpAsync(DialogContext dc)
        {
            // Help within the dialog cancels the dialog.
            // When the user sees the help below, they are no longer within the context of the dialog.
            if (dc.ActiveDialog != null)
            {
                await dc.CancelAllAsync();
            }

            await dc.Context.SendActivityAsync("Let me try to provide some help.");
            await dc.Context.SendActivityAsync("I understand greetings, being asked for help, or being asked to cancel what I am doing.");

            // Signal the conversation was interrupted and should immediately continue
            return InterruptionStatus.Interrupted;
        }
    }
}

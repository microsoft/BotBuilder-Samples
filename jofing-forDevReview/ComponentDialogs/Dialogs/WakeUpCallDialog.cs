namespace ComponentDialogs
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Recognizers.Text;

    /// <summary>
    /// Defines the wake-up-call dialog.
    /// </summary>
    public class WakeUpCallDialog : ComponentDialog
    {
        /// <summary>
        /// State information associated with this dialog.
        /// </summary>
        public class WakeUpInfo
        {
            public string Time { get; set; }
        }

        /// <summary>
        /// IDs for the prompts used in this dialog.
        /// </summary>
        private struct PromptIds
        {
            public const string AlarmTime = "datePrompt";
        }

        /// <summary>
        /// IDs for the values stored in dialog state.
        /// </summary>
        private class Values
        {
            public const string WakeUpInfo = "wakeUpInfo";
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WakeUpCallDialog"/> class.
        /// </summary>
        /// <param name="dialogId">The ID to assign to the main dialog in the set.</param>
        public WakeUpCallDialog(string dialogId) : base(dialogId)
        {
            // Indicate which dialog is the main dialog.
            InitialDialogId = Id;

            // Define the prompts used in this conversation flow.
            AddDialog(new DateTimePrompt(PromptIds.AlarmTime, defaultLocale: Culture.English));

            // Define the conversation flow using a waterfall model.
            AddDialog(new WaterfallDialog(Id, new WaterfallStep[]
            {
            async (dc, step, cancellationToken) =>
            {
                // Prompt for the alarm time.
                return await dc.PromptAsync(PromptIds.AlarmTime, new PromptOptions
                {
                    Prompt = MessageFactory.Text($"what time would you like your alarm set for?"),
                });
            },
            async (dc, step, cancellationToken) =>
            {
                // Get the alarm time and "sign off".
                var resolution = step.Result as IList<DateTimeResolution>;
                var time = resolution.FirstOrDefault()?.Value;
                await dc.Context.SendActivityAsync($"Your alarm is set to {time}.");

                // End the dialog, returning the wake up alarm information.
                return await dc.EndAsync(new WakeUpInfo { Time = time });
            }
            }));
        }
    }
}

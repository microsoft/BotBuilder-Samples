namespace ComponentDialogs
{
    using System.Linq;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Choices;
    using Microsoft.Recognizers.Text;

    /// <summary>
    /// Defines the reserve-table dialog.
    /// </summary>
    public class ReserveTableDialog : ComponentDialog
    {
        /// <summary>
        /// State information associated with this dialog.
        /// </summary>
        public class TableInfo
        {
            public string Number { get; set; }
        }

        /// <summary>
        /// IDs for the prompts used in this dialog.
        /// </summary>
        private struct PromptIds
        {
            public const string ChooseTable = "choicePrompt";
        }

        /// <summary>
        /// IDs for the values stored in dialog state.
        /// </summary>
        private class Values
        {
            public const string TableInfo = "tableInfo";
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ReserveTableDialog"/> class.
        /// </summary>
        /// <param name="dialogId">The ID to assign to the main dialog in the set.</param>
        public ReserveTableDialog(string dialogId) : base(dialogId)
        {
            // Indicate which dialog is the main dialog.
            InitialDialogId = Id;

            // Define the prompts used in this conversation flow.
            AddDialog(new ChoicePrompt(PromptIds.ChooseTable, defaultLocale: Culture.English));

            // Define the conversation flow using a waterfall model.
            AddDialog(new WaterfallDialog(Id, new WaterfallStep[]
            {
            async (dc, step, cancellationToken) =>
            {
                // Prompt for the table number.
                string[] choices = new string[] { "1", "2", "3", "4", "5", "6" };
                return await dc.PromptAsync("choicePrompt", new PromptOptions
                {
                    Prompt = MessageFactory.Text("Which table would you like to reserve?"),
                    Choices = choices.Select(s => new Choice { Value = s }).ToList()
                });
            },
            async (dc, step, cancellationToken) =>
            {
                // Get the table number and "sign off".
                var choice = step.Result as FoundChoice;
                await dc.Context.SendActivityAsync(
                    $"Sounds great; we will reserve table number {choice.Value} for you.");

                // End the dialog, returning the table information.
                return await dc.EndAsync(new TableInfo { Number = choice.Value });
            }
            }));
        }
    }
}

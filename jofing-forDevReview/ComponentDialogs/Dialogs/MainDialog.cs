namespace ComponentDialogs
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;

    /// <summary>
    /// Defines the bot's main dialog set.
    /// </summary>
    public class MainDialog : DialogSet
    {
        /// <summary>The ID of the main dialog in the set.</summary>
        public const string MainDialogId = "main";

        /// <summary>
        /// IDs for the other dialogs used in this dialog set.
        /// </summary>
        public struct DialogIds
        {
            public const string CheckIn = "checkIn";
            public const string ReserveTable = "reserveTable";
            public const string WakeUp = "wakeUp";
        }

        /// <summary>
        /// Defines the command phrases that the dialog can recognize.
        /// </summary>
        private struct Commands
        {
            public const string ReserveTable = "Reserve table";
            public const string WakeUp = "Set alarm";

            public static List<string> MenuOptions { get; }
                = new List<string> { Commands.ReserveTable, Commands.WakeUp };
        }

        /// <summary>
        /// The state property accessor for the user profile.
        /// </summary>
        private IStatePropertyAccessor<UserProfile> UserProfileAccessor { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="MainDialog"/> class.
        /// </summary>
        /// <param name="dialogState">The state property accessor for dialog state.</param>
        /// <param name="userProfileAccessor">The state property accessor for the user
        /// profile.</param>
        public MainDialog(
            IStatePropertyAccessor<DialogState> dialogState,
            IStatePropertyAccessor<UserProfile> userProfileAccessor)
            : base(dialogState)
        {
            // Set the instance property.
            UserProfileAccessor = userProfileAccessor;

            // Define the dialogs used in this conversation flow.
            Add(new CheckInDialog(DialogIds.CheckIn));
            Add(new ReserveTableDialog(DialogIds.ReserveTable));
            Add(new WakeUpCallDialog(DialogIds.WakeUp));

            // Define the conversation flow using a waterfall model.
            Add(new WaterfallDialog(MainDialogId, new WaterfallStep[]
            {
            async (dc, step, cancellationToken) =>
            {
                // The user profile should always be initialized before we enter this,
                // so we don't need to provide a default value factory in this call.
                var userProfile = await UserProfileAccessor.GetAsync(dc.Context);

                // Ask the user to enter a menu option, and end the turn.
                await dc.Context.SendActivityAsync(
                    MessageFactory.SuggestedActions(
                        Commands.MenuOptions,
                        $"Hi {userProfile.Guest.Name}, how can I help you?"));
                return Dialog.EndOfTurn;
            },
            async (dc, step, cancellationToken) =>
            {
                // Decide which dialog to start, based on the user input.
                string input = step.Result as string;

                if (string.Equals(input, Commands.ReserveTable, StringComparison.InvariantCultureIgnoreCase))
                {
                    return await dc.BeginAsync(DialogIds.ReserveTable);
                }
                else if (string.Equals(input, Commands.WakeUp, StringComparison.InvariantCultureIgnoreCase))
                {
                    return await dc.BeginAsync(DialogIds.WakeUp);
                }
                else
                {
                    await dc.Context.SendActivityAsync(
                        "Sorry, I don't understand that command. Please choose an option from the list.");
                    return await step.NextAsync();
                }
            },
            async (dc, step, cancellationToken) =>
            {
                // Get the user profile from the turn context.
                var userProfile = await UserProfileAccessor.GetAsync(dc.Context);

                // Update the profile based on the result of the previous step.
                switch (step.Result)
                {
                    case ReserveTableDialog.TableInfo tableInfo:
                        userProfile.Table = tableInfo;
                        break;

                    case WakeUpCallDialog.WakeUpInfo wakeUpInfo:
                        userProfile.WakeUp = wakeUpInfo;
                        break;
                }

                // Show the main menu again.
                return await dc.ReplaceAsync(MainDialogId);
            }
            }));
        }
    }
}

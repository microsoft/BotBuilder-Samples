namespace ComponentDialogs
{
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Recognizers.Text;

    /// <summary>
    /// Defines the guest check-in dialog.
    /// </summary>
    public class CheckInDialog : ComponentDialog
    {
        /// <summary>
        /// State information associated with this dialog.
        /// </summary>
        public class GuestInfo
        {
            public string Name { get; set; }
            public int Room { get; set; }
        }

        /// <summary>
        /// IDs for the prompts used in this dialog.
        /// </summary>
        private struct PromptIds
        {
            public const string GuestName = "textPrompt";
            public const string RoomNumber = "numberPrompt";
        }

        /// <summary>
        /// IDs for the values stored in dialog state.
        /// </summary>
        private class Values
        {
            public const string GuestInfo = "guestInfo";
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CheckInDialog"/> class.
        /// </summary>
        /// <param name="dialogId">The ID to assign to the main dialog in the set.</param>
        public CheckInDialog(string dialogId) : base(dialogId)
        {
            // Indicate which dialog is the main dialog.
            InitialDialogId = Id;

            // Define the prompts used in this conversation flow.
            AddDialog(new TextPrompt(PromptIds.GuestName));
            AddDialog(new NumberPrompt<int>(PromptIds.RoomNumber, defaultLocale: Culture.English));

            // Define the conversation flow using a waterfall model.
            AddDialog(new WaterfallDialog(Id, new WaterfallStep[]
            {
                async (dc, step, cancellationToken) =>
                {
                    // Initialize guest information and prompt for the guest's name.
                    step.Values[Values.GuestInfo] = new GuestInfo();
                    return await dc.PromptAsync(PromptIds.GuestName, new PromptOptions
                    {
                        Prompt = MessageFactory.Text("What is your name?")
                    });
                },
                async (dc, step, cancellationToken) =>
                {
                    // Save the name and prompt for the room number.
                    string name = step.Result as string;
                    GuestInfo guestInfo = step.Values[Values.GuestInfo] as GuestInfo;
                    guestInfo.Name = name;

                    return await dc.PromptAsync(PromptIds.RoomNumber, new PromptOptions
                    {
                        Prompt = MessageFactory.Text(
                            $"Hi {name}. What room will you be staying in?")
                    });
                },
                async (dc, step, cancellationToken) =>
                {
                    // Save the room number and "sign off".
                    int room = (int)step.Result;
                    GuestInfo guestInfo = step.Values[Values.GuestInfo] as GuestInfo;
                    guestInfo.Room = room;

                    await dc.Context.SendActivityAsync(
                        $"Great, room {room} is ready for you.<br/>Enjoy your stay!");

                    // End the dialog, and return the guest info.
                    return await dc.EndAsync(guestInfo);
                }
            }));
        }
    }
}

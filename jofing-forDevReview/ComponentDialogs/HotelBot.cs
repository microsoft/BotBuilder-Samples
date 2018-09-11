namespace ComponentDialogs
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Schema;

    /// <summary>
    /// Defines the hotel bot.
    /// </summary>
    public class HotelBot : IBot
    {
        /// <summary>
        /// State property accessor for the user profile.
        /// </summary>
        private IStatePropertyAccessor<UserProfile> UserProfileAccessor { get; }

        /// <summary>
        /// An instance of the main dialog set for the dialog.
        /// </summary>
        private MainDialog MainDialog { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="HotelBot"/> class.
        /// </summary>
        /// <param name="userProfileAccessor">State property accessor for the user profile.</param>
        /// <param name="mainDialog">An instance of the main dialog set for the dialog.</param>
        public HotelBot(IStatePropertyAccessor<UserProfile> userProfileAccessor, MainDialog mainDialog)
        {
            // Set the bot's instance properties.
            UserProfileAccessor = userProfileAccessor;
            MainDialog = mainDialog;
        }

        /// <summary>
        /// Every Conversation turn for our bot calls this method.
        /// </summary>
        /// <param name="turnContext">The current turn context.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task OnTurnAsync(
            ITurnContext turnContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get the user profile from the turn context.
            var userProfile = await UserProfileAccessor.GetAsync(turnContext, () => new UserProfile());

            // Establish dialog state from the conversation state.
            var dc = await MainDialog.CreateContextAsync(turnContext);

            // Handle activity types that we understand.
            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.ConversationUpdate:

                    // When a user starts a conversation with the bot, greet them and start
                    // the appropriate dialog in the set.
                    var activity = turnContext.Activity.AsConversationUpdateActivity();
                    if (activity.MembersAdded.Any(member => member.Id != activity.Recipient.Id))
                    {
                        await turnContext.SendActivityAsync("Welcome to our hotel!");

                        //  Start the appropriate dialog in the set, based on the user profile.
                        await BeginADialog(userProfile, dc);
                    }

                    break;

                case ActivityTypes.Message:

                    // Continue any current dialog.
                    var turnResult = await dc.ContinueAsync();
                    if (turnResult.Status == DialogTurnStatus.Complete
                        && turnResult.Result is CheckInDialog.GuestInfo info)
                    {
                        // Save output from the check in dialog.
                        userProfile.Guest = info;

                        //  Start the appropriate dialog in the set, based on the user profile.
                        await BeginADialog(userProfile, dc);
                    }

                    // Some channels don't send a conversation update activity.
                    if (!turnContext.Responded)
                    {
                        //  Start the appropriate dialog in the set, based on the user profile.
                        await BeginADialog(userProfile, dc);
                    }

                    break;
            }
        }

        /// <summary>
        /// Starts the appropriate dialog in the set, based on the user profile.
        /// </summary>
        /// <param name="userProfile">The user profile.</param>
        /// <param name="dc">The dialog context for the bot's dialog set.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        private static async Task BeginADialog(UserProfile userProfile, DialogContext dc)
        {
            // If we don't yet have the guest's info, start the check-in dialog.
            if (string.IsNullOrEmpty(userProfile?.Guest?.Name))
            {
                await dc.BeginAsync(MainDialog.DialogIds.CheckIn, new PromptOptions());
            }
            // Otherwise, start our bot's main dialog.
            else
            {
                await dc.BeginAsync(MainDialog.MainDialogId, new PromptOptions());
            }
        }
    }
}

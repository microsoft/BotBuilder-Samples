namespace UserDataBot
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Integration;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Options;

    /// <summary>Defines the bot for the persisting user data tutorial.</summary>
    public class UserDataBot : IBot
    {
        /// <summary>The state property accessor for user data.</summary>
        private IStatePropertyAccessor<UserData> UserDataAccessor { get; }

        /// <summary>The dialog set that has the dialog to use.</summary>
        private GreetingsDialog GreetingsDialog { get; }

        /// <summary>Create a new instance of the bot.</summary>
        /// <param name="options">The options to use for our app.</param>
        /// <param name="greetingsDialog">An instance of the dialog set.</param>
        public UserDataBot(IOptions<BotFrameworkOptions> options, GreetingsDialog greetingsDialog)
        {
            // Retrieve the user state middleware from the options, and create the state property accessor.
            BotStateSet stateSet = options.Value.Middleware.OfType<BotStateSet>().FirstOrDefault();
            UserState userState = stateSet.BotStates.OfType<UserState>().FirstOrDefault();
            UserDataAccessor = userState.CreateProperty<UserData>("UserDataBot.UserData");

            // Record the dialog set to use to get the user's name. 
            GreetingsDialog = greetingsDialog;
        }

        /// <summary>Handles incoming activities to the bot.</summary>
        /// <param name="turnContext">The context object for the current turn.</param>        
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Retrieve user data from state.
            UserData userData = await UserDataAccessor.GetAsync(turnContext, () => new UserData());

            // Establish context for our dialog from the turn context.
            DialogContext dc = await GreetingsDialog.CreateContextAsync(turnContext);

            // Handle coversation update, message, and delete user data activities.
            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.ConversationUpdate:

                    // Greet any user that is added to the conversation.
                    IConversationUpdateActivity activity = turnContext.Activity.AsConversationUpdateActivity();
                    if (activity.MembersAdded.Any(member => member.Id != activity.Recipient.Id))
                    {
                        if (userData.Name is null)
                        {
                            // If we don't already have their name, start a dialog to collect it.
                            await turnContext.SendActivityAsync("Welcome to the User Data bot.");
                            await dc.BeginAsync(GreetingsDialog.MainDialog);
                        }
                        else
                        {
                            // Otherwise, greet them by name.
                            await turnContext.SendActivityAsync($"Hi {userData.Name}! Welcome back to the User Data bot.");
                        }
                    }

                    break;

                case ActivityTypes.Message:

                    // If there's a dialog running, continue it.
                    if (dc.ActiveDialog != null)
                    {
                        var dialogTurnResult = await dc.ContinueAsync();
                        if (dialogTurnResult.Status == DialogTurnStatus.Complete
                            && dialogTurnResult.Result is string name
                            && !string.IsNullOrWhiteSpace(name))
                        {
                            // If it completes successfully and returns a valid name, save the name and greet the user.
                            userData.Name = name;
                            await turnContext.SendActivityAsync($"Pleased to meet you {userData.Name}.");
                        }
                    }
                    // Else, if we don't have the user's name yet, ask for it.
                    else if (userData.Name is null)
                    {
                        await dc.BeginAsync(GreetingsDialog.MainDialog);
                    }
                    // Else, echo the user's message text.
                    else
                    {
                        await turnContext.SendActivityAsync($"{userData.Name} said, '{turnContext.Activity.Text}'.");
                    }

                    break;

                case ActivityTypes.DeleteUserData:

                    // Delete the user's data.
                    userData.Name = null;
                    await turnContext.SendActivityAsync("I have deleted your user data.");

                    break;
            }
        }
    }
}

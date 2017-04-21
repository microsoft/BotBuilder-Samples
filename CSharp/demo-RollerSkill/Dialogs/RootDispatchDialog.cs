namespace RollerSkillBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Scorables;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class RootDispatchDialog : DispatchDialog
    {
        // generic activity handler.
        [MethodBind]
        [ScorableGroup(0)]
        public async Task ActivityHandler(IDialogContext context, IActivity activity)
        {
            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    this.ContinueWithNextGroup();
                    break;

                case ActivityTypes.ConversationUpdate:
                case ActivityTypes.ContactRelationUpdate:
                case ActivityTypes.Typing:
                case ActivityTypes.DeleteUserData:
                case ActivityTypes.Ping:
                default:
                    break;
            }
        }

        [RegexPattern("(roll|role|throw|shoot).*(dice|die|dye|bones)")]
        [RegexPattern("new game")]
        [ScorableGroup(1)]
        public async Task NewGame(IDialogContext context, IActivity activity)
        {
            context.Call(new CreateGameDialog(), AfterGameCreated);
        }

        [RegexPattern("(roll|role|throw|shoot) again")]
        [ScorableGroup(1)]
        public async Task RollAgain(IDialogContext context, IActivity activity)
        {
            await PlayGame(context);
        }

        [RegexPattern("(play|start).*(craps)")]
        [ScorableGroup(1)]
        public async Task PlayCraps(IDialogContext context, IActivity activity)
        {
            var crapsGame = new GameData("Craps", 6, 2, 0);
            await PlayGame(context, crapsGame);
        }

        [RegexPattern("help")]
        [ScorableGroup(1)]
        public async Task Help(IDialogContext context, IActivity activity)
        {
            await this.Default(context, activity);
        }

        [MethodBind]
        [ScorableGroup(2)]
        public async Task Default(IDialogContext context, IActivity activity)
        {
            context.Call(new HelpDialog(), AfterDialog);
        }

        private static async Task PlayGame(IDialogContext context, GameData gameData = null)
        {
            context.Call(new PlayGameDialog(gameData), AfterDialog);
        }

        private static async Task AfterGameCreated(IDialogContext context, IAwaitable<GameData> result)
        {
            var game = await result;

            await PlayGame(context, game);
        }

        private static async Task AfterDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Done<object>(null);
        }
    }
}
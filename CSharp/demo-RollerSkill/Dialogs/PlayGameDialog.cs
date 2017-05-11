namespace RollerSkillBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using RollerSkillBot.Properties;

    [Serializable]
    public class PlayGameDialog : IDialog<object>
    {
        private const string RollAgainOptionValue = "roll again";

        private const string NewGameOptionValue = "new game";

        private GameData gameData;

        public PlayGameDialog(GameData gameData)
        {
            this.gameData = gameData;
        }

        public async Task StartAsync(IDialogContext context)
        {
            if (this.gameData == null)
            {
                if (!context.UserData.TryGetValue<GameData>(Utils.GameDataKey, out this.gameData))
                {
                    // User started session with "roll again" so let's just send them to
                    // the 'CreateGameDialog'
                    context.Done<object>(null);
                }
            }

            int total = 0;
            var randomGenerator = new Random();
            var rolls = new List<int>();

            // Generate Rolls
            for (int i = 0; i < this.gameData.Count; i++)
            {
                var roll = randomGenerator.Next(1, this.gameData.Sides);
                total += roll;
                rolls.Add(roll);
            }

            // Format rolls results
            var result = string.Join(" . ", rolls.ToArray());
            bool multiLine = rolls.Count > 5;

            var card = new HeroCard()
            {
                Subtitle = string.Format(
                    this.gameData.Count > 1 ? Resources.CardSubtitlePlural : Resources.CardSubtitleSingular,
                    this.gameData.Count,
                    this.gameData.Sides),
                Buttons = new List<CardAction>()
                {
                    new CardAction(ActionTypes.ImBack, "Roll Again", value: RollAgainOptionValue),
                    new CardAction(ActionTypes.ImBack, "New Game", value: NewGameOptionValue)
                }
            };

            if (multiLine)
            {
                card.Text = result;
            }
            else
            {
                card.Title = result;
            }

            var message = context.MakeMessage();
            message.Attachments = new List<Attachment>()
            {
                card.ToAttachment()
            };

            // Determine bots reaction for speech purposes
            string reaction = "normal";

            var min = this.gameData.Count;
            var max = this.gameData.Count * this.gameData.Sides;
            var score = total / max;
            if (score == 1)
            {
                reaction = "Best";
            }
            else if (score == 0)
            {
                reaction = "Worst";
            }
            else if (score <= 0.3)
            {
                reaction = "Bad";
            }
            else if (score >= 0.8)
            {
                reaction = "Good";
            }

            // Check for special craps rolls
            if (this.gameData.Type == "Craps")
            {
                switch (total)
                {
                    case 2:
                    case 3:
                    case 12:
                        reaction = "CrapsLose";
                        break;
                    case 7:
                        reaction = "CrapsSeven";
                        break;
                    case 11:
                        reaction = "CrapsEleven";
                        break;
                    default:
                        reaction = "CrapsRetry";
                        break;
                }
            }

            // Build up spoken response
            var spoken = string.Empty;
            if (this.gameData.Turns == 0)
            {
                spoken += Utils.RandomPick(Resources.ResourceManager.GetString($"Start{this.gameData.Type}GameSSML"));
            }

            spoken += Utils.RandomPick(Resources.ResourceManager.GetString($"{reaction}RollReactionSSML"));

            message.Speak = SSMLHelper.Speak(spoken);

            // Increment number of turns and store game to roll again
            this.gameData.Turns++;
            context.UserData.SetValue<GameData>(Utils.GameDataKey, this.gameData);

            // Send card and bots reaction to user.
            message.InputHint = InputHints.AcceptingInput;
            await context.PostAsync(message);

            context.Done<object>(null);
        }
    }
}
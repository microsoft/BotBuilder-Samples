namespace RollerSkillBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using RollerSkillBot.Properties;

    [Serializable]
    public class HelpDialog : IDialog<object>
    {
        private const string RollDiceOptionValue = "roll some dice";

        private const string PlayCrapsOptionValue = "play craps";

        public async Task StartAsync(IDialogContext context)
        {
            var message = context.MakeMessage();
            message.Speak = SSMLHelper.Speak(Resources.HelpSSML);
            message.InputHint = InputHints.AcceptingInput;

            message.Attachments = new List<Attachment>
            {
                new HeroCard(Resources.HelpTitle)
                {
                    Buttons = new List<CardAction>
                    {
                        new CardAction(ActionTypes.ImBack, "Roll Dice", value: RollDiceOptionValue),
                        new CardAction(ActionTypes.ImBack, "Play Craps", value: PlayCrapsOptionValue)
                    }
                }.ToAttachment()
            };

            await context.PostAsync(message);

            context.Done<object>(null);
        }
    }
}
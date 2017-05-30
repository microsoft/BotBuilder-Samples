namespace BotBuilder.Samples.AdaptiveCards
{
    using Microsoft.Bot.Builder.Dialogs;
    using System;
    using System.Threading.Tasks;

    [Serializable]
    public class FlightsDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Fail(new NotImplementedException("Flights Dialog is not implemented and is instead being used to show context.Fail"));
        }
    }
}
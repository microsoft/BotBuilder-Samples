namespace MultiDialogsBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class FlightsDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Fail(new NotImplementedException("Flights Dialog is not implemented and is instead being used to show context.Fail"));
        }
    }
}
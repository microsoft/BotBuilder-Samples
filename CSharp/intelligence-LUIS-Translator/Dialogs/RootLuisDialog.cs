namespace LuisBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        private const string EntityHotelName = "Hotel";

        private const string EntitiyLocation = "Location";

        private const string EntityName = "Name";

        public RootLuisDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"],
            ConfigurationManager.AppSettings["LuisSubscriptionKey"],
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await this.EchoToUser(context, result);
        }

        [LuisIntent("SearchHotels")]
        public async Task Search(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await this.EchoToUser(context, result);
        }

        [LuisIntent("ShowHotelsReviews")]
        public async Task Reviews(IDialogContext context, LuisResult result)
        {
            await this.EchoToUser(context, result);
        }

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await this.EchoToUser(context, result);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            await this.EchoToUser(context, result);
        }

        private async Task EchoToUser(IDialogContext context, LuisResult result)
        {
            await this.ShowTranslation(context, result).ConfigureAwait(false);
            await this.ShowLuisResult(context, result).ConfigureAwait(false);
            context.Wait(MessageReceived);
        }

        private async Task ShowTranslation(IDialogContext context, LuisResult result)
        {
            string translationMessage = $"Your input message after translation is " + result.Query;
            await context.PostAsync(translationMessage);
        }


        private async Task ShowLuisResult(IDialogContext context, LuisResult result)
        {
            string luisMessage = $"After using LUIS recognition:\nthe top intent was: {result.Intents[0].Intent}, with score {result.Intents[0].Score}";
            await context.PostAsync(luisMessage);
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Zummer.Handlers;

namespace Zummer.Dialogs
{
    /// <summary>
    /// The top-level natural language dialog for sample.
    /// </summary>
    [Serializable]
    internal sealed class MainDialog : LuisDialog<object>
    {
        private readonly IHandlerFactory handlerFactory;

        public MainDialog(ILuisService luis, IHandlerFactory handlerFactory)
            : base(luis)
        {
            SetField.NotNull(out this.handlerFactory, nameof(handlerFactory), handlerFactory);
        }

        [LuisIntent(ZummerStrings.GreetingIntentName)]
        public async Task GreetingIntentHandlerAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await this.handlerFactory.CreateIntentHandler(ZummerStrings.GreetingIntentName).Respond(activity, result);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent(ZummerStrings.SearchIntentName)]
        public async Task FindArticlesIntentHandlerAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await this.handlerFactory.CreateIntentHandler(ZummerStrings.SearchIntentName).Respond(activity, result);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task FallbackIntentHandlerAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(string.Format(Strings.FallbackIntentMessage));
            context.Wait(this.MessageReceived);
        }
    }
}
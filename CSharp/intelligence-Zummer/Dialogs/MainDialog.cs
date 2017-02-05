using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;
using Zummer.Handlers;

namespace Zummer.Dialogs
{
    /// <summary>
    /// The top-level natural language dialog for sample.
    /// </summary>
    [Serializable]
    [LuisModel("", "")]
    internal sealed class MainDialog : DispatchDialog
    {
        private readonly IHandlerFactory handlerFactory;
        private readonly ILuisService luis;
        
        public MainDialog(ILuisService luis, IHandlerFactory handlerFactory)
        {
            SetField.NotNull(out this.handlerFactory, nameof(handlerFactory), handlerFactory);
            SetField.NotNull(out this.luis, nameof(luis), luis);
        }

        /// <summary>
        /// This method process the summarization requests from the client by matching regex pattern on message text
        /// this request is sent when the user clicks on "Read Summary" button. A messages is submitted with value
        /// contains summary keyword at the beginning and url of article to summarize.
        /// Note: HeroCard is created in <see cref="Utilities.CardGenerators.ArticleCardGenerator"/>
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="activity">Message activity containing the message information such as text</param>
        /// <param name="result">The regex Match with the Regex pattern</param>
        /// <returns>Async Task</returns>
        [RegexPattern("(^summary )(.*)")]
        [ScorableGroup(0)]
        public async Task SummarizationRegexHandlerAsync(IDialogContext context, IMessageActivity activity, Match result)
        {
            await this.handlerFactory.CreateRegexHandler(ZummerStrings.SummarizeActionName).Respond(activity, result);
            context.Wait(this.ActivityReceivedAsync);
        }

        [LuisIntent(ZummerStrings.GreetingIntentName)]
        [ScorableGroup(1)]
        public async Task GreetingIntentHandlerAsync(IDialogContext context, IMessageActivity activity, LuisResult result)
        {
            await this.handlerFactory.CreateIntentHandler(ZummerStrings.GreetingIntentName).Respond(activity, result);
            context.Wait(this.ActivityReceivedAsync);
        }

        [LuisIntent(ZummerStrings.ArticlesIntentName)]
        [ScorableGroup(1)]
        public async Task FindArticlesIntentHandlerAsync(IDialogContext context, IMessageActivity activity, LuisResult result)
        {
            await this.handlerFactory.CreateIntentHandler(ZummerStrings.ArticlesIntentName).Respond(activity, result);
            context.Wait(this.ActivityReceivedAsync);
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        [ScorableGroup(1)]
        public async Task FallbackIntentHandlerAsync(IDialogContext context, IMessageActivity activity)
        {
            await context.PostAsync(string.Format(Strings.FallbackIntentMessage));
            context.Wait(this.ActivityReceivedAsync);
        }

        protected override ILuisService MakeService(ILuisModel model)
        {
            return this.luis;
        }

        protected override Regex MakeRegex(string pattern)
        {
            return new Regex(pattern, RegexOptions.IgnoreCase);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using NewsieBot.Utilities;

namespace NewsieBot.Handlers
{
    [Serializable]
    internal sealed class NewsCategoryScorable : ScorableBase<IActivity, LuisResult, double>
    {
        private readonly IHandlerFactory handlerFactory;

        public NewsCategoryScorable(IHandlerFactory handlerFactory)
        {
            SetField.NotNull(out this.handlerFactory, nameof(handlerFactory), handlerFactory);
        }

        protected override async Task<LuisResult> PrepareAsync(IActivity item, CancellationToken token)
        {
            var message = item as IMessageActivity;

            if (message != null && message.Text != null)
            {
                var text = message.Text;

                NewsCategory newsCategory;

                if (NewsCategoryParser.TryParse(text, out newsCategory))
                {
                    return new LuisResult(
                        text,
                        new List<EntityRecommendation>
                        {
                            new EntityRecommendation(type: NewsieStrings.NewsEntityCategory, entity: newsCategory.ToString())
                        },
                        new IntentRecommendation(NewsieStrings.NewsIntentName, 1));
                }
            }

            return null;
        }

        protected override async Task PostAsync(IActivity item, LuisResult state, CancellationToken token)
        {
            var message = item as IMessageActivity;

            if (message != null)
            {
                await this.handlerFactory.CreateIntentHandler(NewsieStrings.NewsIntentName).Respond(message, state);
            }
        }

        protected override Task DoneAsync(IActivity item, LuisResult state, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        protected override bool HasScore(IActivity item, LuisResult state)
        {
            var message = item as IMessageActivity;

            if (message != null && message.Text != null)
            {
                var text = message.Text;

                NewsCategory newsCategory;

                return NewsCategoryParser.TryParse(text, out newsCategory);
            }

            return false;
        }

        protected override double GetScore(IActivity item, LuisResult state)
        {
            return 1.0;
        }
    }
}
namespace JobListingBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Search.Dialogs;
    using Search.Models;
    using Search.Services;

    [Serializable]
    public class JobsDialog : SearchDialog
    {
        private static readonly string[] TopRefiners = { "business_title", "agency", "work_location" };

        public JobsDialog(ISearchClient searchClient, SearchQueryBuilder queryBuilder) : base(searchClient, queryBuilder, new JobStyler(), multipleSelection: true)
        {
        }

        protected override string[] GetTopRefiners()
        {
            return TopRefiners;
        }

        [Serializable]
        public class JobStyler : PromptStyler
        {
            public override void Apply<T>(ref IMessageActivity message, string prompt, IReadOnlyList<T> options, IReadOnlyList<string> descriptions = null, string speak = null)
            {
                var hits = (IList<SearchHit>)options;

                var cards = hits.Select(h => new HeroCard
                {
                    Title = h.Title,
                    Buttons = new[] { new CardAction(ActionTypes.ImBack, "Save", value: h.Key) },
                    Text = h.Description
                });

                message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                message.Attachments = cards.Select(c => c.ToAttachment()).ToList();
                message.Text = prompt;
                message.Speak = speak;
            }
        }
    }
}

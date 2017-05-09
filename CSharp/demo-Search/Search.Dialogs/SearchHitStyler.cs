namespace Search.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Search.Models;

    [Serializable]
    public class SearchHitStyler : PromptStyler
    {
        public override void Apply<T>(ref IMessageActivity message, string prompt, IReadOnlyList<T> options, IReadOnlyList<string> descriptions = null, string speak = null)
        {
            var hits = options as IList<SearchHit>;
            if (hits != null)
            {
                var cards = hits.Select(h => new ThumbnailCard
                {
                    Title = h.Title,
                    Images = new[] { new CardImage(h.PictureUrl) },
                    Buttons = new[] { new CardAction(ActionTypes.ImBack, "Pick this one", value: h.Key) },
                    Text = h.Description
                });

                message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                message.Attachments = cards.Select(c => c.ToAttachment()).ToList();
                message.Text = prompt;
                message.Speak = speak;
            }
            else
            {
                base.Apply<T>(ref message, prompt, options, descriptions, speak);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Connector;
using Search.Dialogs.UserInteraction;
using Search.Models;

namespace Search.Dialogs
{
    public interface ISearchHitStyler
    {
        void Show(ref IMessageActivity message, IReadOnlyList<SearchHit> hits, string prompt = null,
            params Button[] buttons);
    }

    [Serializable]
    public class SearchHitStyler : ISearchHitStyler
    {
        public void Show(ref IMessageActivity message, IReadOnlyList<SearchHit> hits, string prompt = null,
            params Button[] buttons)
        {
            if (hits != null)
            {
                var cards = hits.Select(h =>
                {
                    var actions = new List<CardAction>();
                    foreach (var button in buttons)
                    {
                        actions.Add(new CardAction(ActionTypes.PostBack, button.Label,
                            value: string.Format(button.Message, h.Key)));
                    }
                    return new ThumbnailCard
                    {
                        Title = h.Title,
                        Images = new[] {new CardImage(h.PictureUrl)},
                        Buttons = actions.ToArray(),
                        Text = h.Description
                    };
                });

                message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                message.Attachments = cards.Select(c => c.ToAttachment()).ToList();
                message.Text = prompt;
            }
        }
    }
}
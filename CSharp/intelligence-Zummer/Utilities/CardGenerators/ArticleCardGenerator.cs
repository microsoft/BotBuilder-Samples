using System;
using System.Collections.Generic;
using Microsoft.Bot.Connector;
using Zummer.Models.Search;

namespace Zummer.Utilities.CardGenerators
{
    internal class ArticleCardGenerator
    {
        public static Attachment GetArticleCard(ZummerSearchResult result, string channelId)
        {
            Attachment attachment;

            var titleParts = result.Name.Split(new []{'-'}, StringSplitOptions.RemoveEmptyEntries);
            var title = titleParts[0];
            var subTitle = string.Empty;

            if (titleParts.Length > 1)
            {
                subTitle = titleParts[1];
            }

            var tap = new CardAction(ActionTypes.OpenUrl, Strings.ViewOnWeb, value: result.Url);

            var actions = new List<CardAction>
            {
                new CardAction(ActionTypes.ImBack, Strings.ReadSummary, value: "summary " + result.Url)
            };

            switch (channelId)
            {
                case "slack":
                    attachment = CardGenerator.GetThumbNailCard(subTitle, title, result.Snippet, actions);
                    break;
                default:
                    attachment = CardGenerator.GetThumbNailCard(title, subTitle, result.Snippet, actions, tap);
                    break;
            }

            return attachment;
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Bot.Connector;
using AzureSearchBot.Model;

namespace AzureSearchBot.Util
{
    public static class CardUtil
    {
        public static async void ShowHeroCard(IMessageActivity message, SearchResult searchResult)
        {
            //Make reply activity and set layout
            Activity reply = ((Activity)message).CreateReply();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            //Make each Card for each musician
            foreach (Value musician in searchResult.value)
            {
                List<CardImage> cardImages = new List<CardImage>();
                cardImages.Add(new CardImage(url: musician.imageURL));
                HeroCard card = new HeroCard()
                {
                    Title = musician.Name,
                    Subtitle = $"Era: {musician.Era } | Search Score: {musician.searchscore}",
                    Text = musician.Description,
                    Images = cardImages
                };
                reply.Attachments.Add(card.ToAttachment());
            }

            //make connector and reply message
            ConnectorClient connector = new ConnectorClient(new Uri(reply.ServiceUrl));
            await connector.Conversations.SendToConversationAsync(reply);
        }
    }
}
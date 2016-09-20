namespace ContosoFlowers.BotAssets.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Bot.Connector;

    public static class HeroCardExtensions
    {
        public static void AddHeroCard<T>(this IMessageActivity message, string title, string subtitle, IEnumerable<T> options, IEnumerable<string> images = default(IEnumerable<string>))
        {
            var heroCard = GenerateHeroCard(title, subtitle, options, images);

            if (message.Attachments == null)
            {
                message.Attachments = new List<Attachment>();
            }

            message.Attachments.Add(heroCard.ToAttachment());
        }

        public static void AddHeroCard(this IMessageActivity message, string title, string subtitle, IList<KeyValuePair<string, string>> options, IEnumerable<string> images = default(IEnumerable<string>))
        {
            var heroCard = GenerateHeroCard(title, subtitle, options, images);

            if (message.Attachments == null)
            {
                message.Attachments = new List<Attachment>();
            }

            message.Attachments.Add(heroCard.ToAttachment());
        }

        private static HeroCard GenerateHeroCard(string title, string subtitle, IEnumerable<KeyValuePair<string, string>> options, IEnumerable<string> images)
        {
            var actions = new List<CardAction>();

            foreach (var option in options)
            {
                actions.Add(new CardAction
                {
                    Title = option.Key.ToString(),
                    Type = ActionTypes.ImBack,
                    Value = option.Value.ToString()
                });
            }

            var cardImages = new List<CardImage>();

            if (images != default(IEnumerable<string>))
            {
                foreach (var image in images)
                {
                    cardImages.Add(new CardImage
                    {
                        Url = image,
                    });
                }
            }

            return new HeroCard(title, subtitle, images: cardImages, buttons: actions);
        }

        private static HeroCard GenerateHeroCard<T>(string title, string subtitle, IEnumerable<T> options, IEnumerable<string> images)
        {
            return GenerateHeroCard(title, subtitle, options.Select(option => new KeyValuePair<string, string>(option.ToString(), option.ToString())), images);
        }
    }
}

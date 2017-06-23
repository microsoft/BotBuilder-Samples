namespace BotBuilder.Samples.AdaptiveCards
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::AdaptiveCards;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Connector;
    using Newtonsoft.Json.Linq;

    [Serializable]
    public class HotelsDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var message = context.Activity as IMessageActivity;
            var query = HotelsQuery.Parse(message.Value);

            await context.PostAsync($"Ok. Searching for Hotels in {query.Destination} from {query.Checkin.Value.ToString("MM/dd")} to {query.Checkin.Value.AddDays(query.Nights.Value).ToString("MM/dd")}...");

            try
            {
                await SearchHotels(context, query);
            }
            catch (FormCanceledException ex)
            {
                await context.PostAsync($"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}");
            }
        }

        private async Task SearchHotels(IDialogContext context, HotelsQuery searchQuery)
        {
            var hotels = this.GetHotels(searchQuery);

            // Result count
            var title = $"I found in total {hotels.Count()} hotels for your dates:";
            var intro = new List<CardElement>()
            {
                    new TextBlock()
                    {
                        Text = title,
                        Size = TextSize.ExtraLarge,
                        Speak = $"<s>{title}</s>"
                    }
            };

            // Hotels in rows of three
            var rows = Split(hotels, 3)
                .Select(group => new ColumnSet()
                {
                    Columns = new List<Column>(group.Select(AsHotelItem))
                });

            var card = new AdaptiveCard()
            {
                Body = intro.Union(rows).ToList()
            };

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);

            await context.PostAsync(reply);
        }

        private Column AsHotelItem(Hotel hotel)
        {
            var submitActionData = JObject.Parse("{ \"Type\": \"HotelSelection\" }");
            submitActionData.Merge(JObject.FromObject(hotel));

            return new Column()
            {
                Size = "20",
                Items = new List<CardElement>()
                {
                    new TextBlock()
                    {
                        Text = hotel.Name,
                        Speak = $"<s>{hotel.Name}</s>",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Wrap = false,
                        Weight = TextWeight.Bolder
                    },
                    new Image()
                    {
                        Size = ImageSize.Auto,
                        Url = hotel.Image
                    }
                },
                SelectAction = new SubmitAction()
                {
                    DataJson = submitActionData.ToString()
                }
            };
        }

        private IEnumerable<Hotel> GetHotels(HotelsQuery searchQuery)
        {
            var hotels = new List<Hotel>();

            // Filling the hotels results manually just for demo purposes
            for (int i = 1; i <= 6; i++)
            {
                var random = new Random(i);
                Hotel hotel = new Hotel()
                {
                    Name = $"Hotel {i}",
                    Location = searchQuery.Destination,
                    Rating = random.Next(1, 5),
                    NumberOfReviews = random.Next(0, 5000),
                    PriceStarting = random.Next(80, 450),
                    Image = $"https://placeholdit.imgix.net/~text?txtsize=35&txt=Hotel+{i}&w=500&h=260",
                    MoreImages = new List<string>()
                    {
                        "https://placeholdit.imgix.net/~text?txtsize=65&txt=Pic+1&w=450&h=300",
                        "https://placeholdit.imgix.net/~text?txtsize=65&txt=Pic+2&w=450&h=300",
                        "https://placeholdit.imgix.net/~text?txtsize=65&txt=Pic+3&w=450&h=300",
                        "https://placeholdit.imgix.net/~text?txtsize=65&txt=Pic+4&w=450&h=300"
                    }
                };

                hotels.Add(hotel);
            }

            hotels.Sort((h1, h2) => h1.PriceStarting.CompareTo(h2.PriceStarting));

            return hotels;
        }
        public static IEnumerable<IEnumerable<T>> Split<T>(IEnumerable<T> list, int parts)
        {
            return list.Select((item, ix) => new { ix, item })
                       .GroupBy(x => x.ix % parts)
                       .Select(x => x.Select(y => y.item));
        }
    }
}
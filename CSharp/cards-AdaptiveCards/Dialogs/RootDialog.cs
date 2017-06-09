namespace BotBuilder.Samples.AdaptiveCards
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using global::AdaptiveCards;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Newtonsoft.Json;

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string FlightsOption = "Flights";

        private const string HotelsOption = "Hotels";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Value != null)
            {
                // Got an Action Submit
                dynamic value = message.Value;
                string submitType = value.Type.ToString();
                switch (submitType)
                {
                    case "HotelSearch":
                        HotelsQuery query;
                        try
                        {
                            query = HotelsQuery.Parse(value);

                            // Trigger validation using Data Annotations attributes from the HotelsQuery model
                            List<ValidationResult> results = new List<ValidationResult>();
                            bool valid = Validator.TryValidateObject(query, new ValidationContext(query, null, null), results, true);
                            if (!valid)
                            {
                                // Some field in the Hotel Query are not valid
                                var errors = string.Join("\n", results.Select(o => " - " + o.ErrorMessage));
                                await context.PostAsync("Please complete all the search parameters:\n" + errors);
                                return;
                            }
                        }
                        catch (InvalidCastException)
                        {
                            // Hotel Query could not be parsed
                            await context.PostAsync("Please complete all the search parameters");
                            return;
                        }

                        // Proceed with hotels search
                        await context.Forward(new HotelsDialog(), this.ResumeAfterOptionDialog, message, CancellationToken.None);

                        return;

                    case "HotelSelection":
                        await SendHotelSelectionAsync(context, (Hotel)JsonConvert.DeserializeObject<Hotel>(value.ToString()));
                        context.Wait(MessageReceivedAsync);

                        return;
                }
            }

            if (message.Text != null && (message.Text.ToLower().Contains("help") || message.Text.ToLower().Contains("support") || message.Text.ToLower().Contains("problem")))
            {
                await context.Forward(new SupportDialog(), this.ResumeAfterSupportDialog, message, CancellationToken.None);
            }
            else
            {
                await ShowOptionsAsync(context);
            }
        }

        private async Task ShowOptionsAsync(IDialogContext context)
        {
            AdaptiveCard card = new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
                    new Container()
                    {
                        Speak = "<s>Hello!</s><s>Are you looking for a flight or a hotel?</s>",
                        Items = new List<CardElement>()
                        {
                            new ColumnSet()
                            {
                                Columns = new List<Column>()
                                {
                                    new Column()
                                    {
                                        Size = ColumnSize.Auto,
                                        Items = new List<CardElement>()
                                        {
                                            new Image()
                                            {
                                                Url = "https://placeholdit.imgix.net/~text?txtsize=65&txt=Adaptive+Cards&w=300&h=300",
                                                Size = ImageSize.Medium,
                                                Style = ImageStyle.Person
                                            }
                                        }
                                    },
                                    new Column()
                                    {
                                        Size = ColumnSize.Stretch,
                                        Items = new List<CardElement>()
                                        {
                                            new TextBlock()
                                            {
                                                Text =  "Hello!",
                                                Weight = TextWeight.Bolder,
                                                IsSubtle = true
                                            },
                                            new TextBlock()
                                            {
                                                Text = "Are you looking for a flight or a hotel?",
                                                Wrap = true
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                // Buttons
                Actions = new List<ActionBase>() {
                    new ShowCardAction()
                    {
                        Title = "Hotels",
                        Speak = "<s>Hotels</s>",
                        Card = GetHotelSearchCard()
                    },
                    new ShowCardAction()
                    {
                        Title = "Flights",
                        Speak = "<s>Flights</s>",
                        Card = new AdaptiveCard()
                        {
                            Body = new List<CardElement>()
                            {
                                new TextBlock()
                                {
                                    Text = "Flights is not implemented =(",
                                    Speak = "<s>Flights is not implemented</s>",
                                    Weight = TextWeight.Bolder
                                }
                            }
                        }
                    }
                }
            };

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);

            await context.PostAsync(reply, CancellationToken.None);

            context.Wait(MessageReceivedAsync);
        }
        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task ResumeAfterSupportDialog(IDialogContext context, IAwaitable<int> result)
        {
            var ticketNumber = await result;

            await context.PostAsync($"Thanks for contacting our support team. Your ticket number is {ticketNumber}.");
            context.Wait(this.MessageReceivedAsync);
        }

        private static AdaptiveCard GetHotelSearchCard()
        {
            return new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
                        // Hotels Search form
                        new TextBlock()
                        {
                            Text = "Welcome to the Hotels finder!",
                            Speak = "<s>Welcome to the Hotels finder!</s>",
                            Weight = TextWeight.Bolder,
                            Size = TextSize.Large
                        },
                        new TextBlock() { Text = "Please enter your destination:" },
                        new TextInput()
                        {
                            Id = "Destination",
                            Speak = "<s>Please enter your destination</s>",
                            Placeholder = "Miami, Florida",
                            Style = TextInputStyle.Text
                        },
                        new TextBlock() { Text = "When do you want to check in?" },
                        new DateInput()
                        {
                            Id = "Checkin",
                            Speak = "<s>When do you want to check in?</s>"
                        },
                        new TextBlock() { Text = "How many nights do you want to stay?" },
                        new NumberInput()
                        {
                            Id = "Nights",
                            Min = 1,
                            Max = 60,
                            Speak = "<s>How many nights do you want to stay?</s>"
                        }
                },
                Actions = new List<ActionBase>()
                {
                    new SubmitAction()
                    {
                        Title = "Search",
                        Speak = "<s>Search</s>",
                        DataJson = "{ \"Type\": \"HotelSearch\" }"
                    }
                }
            };
        }

        private static async Task SendHotelSelectionAsync(IDialogContext context, Hotel hotel)
        {
            var description = $"{hotel.Rating} start with {hotel.NumberOfReviews}. From ${hotel.PriceStarting} per night.";
            var card = new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
                    new Container()
                    {
                        Items = new List<CardElement>()
                        {
                            new TextBlock()
                            {
                                Text = $"{hotel.Name} in {hotel.Location}",
                                Weight = TextWeight.Bolder,
                                Speak = $"<s>{hotel.Name}</s>"
                            },
                            new TextBlock()
                            {
                                Text = description,
                                Speak = $"<s>{description}</s>"
                            },
                            new Image()
                            {
                                Size = ImageSize.Large,
                                Url = hotel.Image
                            },
                            new ImageSet()
                            {
                                ImageSize = ImageSize.Medium,
                                Separation = SeparationStyle.Strong,
                                Images = hotel.MoreImages.Select(img => new Image()
                                {
                                    Url = img
                                }).ToList()
                            }
                        },
                        SelectAction = new OpenUrlAction()
                        {
                             Url = "https://dev.botframework.com/"
                        }
                    }
                }
            };

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);

            await context.PostAsync(reply, CancellationToken.None);
        }
    }
}
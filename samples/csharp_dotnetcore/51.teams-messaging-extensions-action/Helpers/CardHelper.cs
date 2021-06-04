using AdaptiveCards;
using Microsoft.Bot.Schema.Teams;
using System.Collections.Generic;
using Microsoft.BotBuilderSamples.Models;

namespace Microsoft.BotBuilderSamples.Helpers
{
    public class CardHelper
    {
        public static List<MessagingExtensionAttachment> CreateAdaptiveCardAttachment(MessagingExtensionAction action, CardResponse createCardResponse)
        {
            AdaptiveCard adaptiveCard = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0));
            adaptiveCard.Body = new List<AdaptiveElement>()
            {
                new AdaptiveColumnSet()
                {
                    Columns = new List<AdaptiveColumn>()
                    {
                        new AdaptiveColumn()
                        {
                            Items=new List<AdaptiveElement>()
                            {
                                new AdaptiveTextBlock()
                                {
                                    Text= "Name :",
                                    Wrap=true,
                                    Size=AdaptiveTextSize.Medium,
                                    Weight=AdaptiveTextWeight.Bolder
                                }
                            },
                            Width = AdaptiveColumnWidth.Auto
                        },
                         new AdaptiveColumn()
                        {
                            Items=new List<AdaptiveElement>()
                            {
                                new AdaptiveTextBlock()
                                {
                                    Text= createCardResponse.Title,
                                    Wrap=true,
                                    Size=AdaptiveTextSize.Medium,
                                }
                            },
                            Width = AdaptiveColumnWidth.Auto
                        },
                    }
                },
                new AdaptiveColumnSet()
                {
                    Columns = new List<AdaptiveColumn>()
                    {
                        new AdaptiveColumn()
                        {
                            Items=new List<AdaptiveElement>()
                            {
                                new AdaptiveTextBlock()
                                {
                                    Text= "Designation :",
                                    Wrap=true,
                                    Size=AdaptiveTextSize.Medium,
                                    Weight=AdaptiveTextWeight.Bolder
                                }
                            },
                            Width = AdaptiveColumnWidth.Auto
                        },
                        new AdaptiveColumn()
                        {
                            Items=new List<AdaptiveElement>()
                            {
                                new AdaptiveTextBlock()
                                {
                                    Text= createCardResponse.Subtitle,
                                    Wrap=true,
                                    Size=AdaptiveTextSize.Medium,
                                }
                            },
                            Width = AdaptiveColumnWidth.Auto
                        },
                    }
                },
                new AdaptiveColumnSet()
                {
                    Columns = new List<AdaptiveColumn>()
                    {
                        new AdaptiveColumn()
                        {
                            Items=new List<AdaptiveElement>()
                            {
                                new AdaptiveTextBlock()
                                {
                                    Text= "Description :",
                                    Wrap=true,
                                    Size=AdaptiveTextSize.Medium,
                                    Weight=AdaptiveTextWeight.Bolder
                                }
                            },
                            Width = AdaptiveColumnWidth.Auto
                        },
                         new AdaptiveColumn()
                        {
                            Items=new List<AdaptiveElement>()
                            {
                                new AdaptiveTextBlock()
                                {
                                    Text= createCardResponse.Text,
                                    Wrap=true,
                                    Size=AdaptiveTextSize.Medium,
                                }
                            },
                            Width = AdaptiveColumnWidth.Auto
                        },
                    }
                },
            };

            var attachments = new List<MessagingExtensionAttachment>();
            attachments.Add(new MessagingExtensionAttachment
            {
                Content = adaptiveCard,
                ContentType = AdaptiveCard.ContentType
            });

            return attachments;
        }
    }
}

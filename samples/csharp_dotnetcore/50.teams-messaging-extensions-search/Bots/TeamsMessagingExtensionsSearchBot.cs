// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class TeamsMessagingExtensionsSearchBot : TeamsActivityHandler
    {

        protected override async Task<MessagingExtensionResponse> OnTeamsMessagingExtensionQueryAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionQuery query, CancellationToken cancellationToken)
        {
            var text = query?.Parameters?[0]?.Value as string ?? string.Empty;
            switch(text)
            {
                case "adaptive card":
                  MessagingExtensionResponse response =  GetAdaptiveCard();
                  return response;
                    
                case "connector card":
                  MessagingExtensionResponse connectorCard = GetConnectorCard();
                    return connectorCard;

                case "result grid":
                    MessagingExtensionResponse resultGrid = GetResultGrid();
                    return resultGrid;
            }

            var packages = await FindPackages(text);

            // We take every row of the results and wrap them in cards wrapped in MessagingExtensionAttachment objects.
            // The Preview is optional, if it includes a Tap, that will trigger the OnTeamsMessagingExtensionSelectItemAsync event back on this bot.
           
            var attachments = packages.Select(package =>
            {
                var previewCard = new ThumbnailCard { Title = package.Item1, Tap = new CardAction { Type = "invoke", Value = package } };
                if (!string.IsNullOrEmpty(package.Item5))
                {
                    previewCard.Images = new List<CardImage>() { new CardImage(package.Item5, "Icon") };
                }

                var attachment = new MessagingExtensionAttachment
                {
                    ContentType = HeroCard.ContentType,
                    Content = new HeroCard { Title = package.Item1 },
                    Preview = previewCard.ToAttachment()
                };

                return attachment;
            }).ToList();

            // The list of MessagingExtensionAttachments must we wrapped in a MessagingExtensionResult wrapped in a MessagingExtensionResponse.
            return new MessagingExtensionResponse
            {
                ComposeExtension = new MessagingExtensionResult
                {
                    Type = "result",
                    AttachmentLayout = "list",
                    Attachments = attachments
                }
            };
        }

        protected override Task<MessagingExtensionResponse> OnTeamsMessagingExtensionSelectItemAsync(ITurnContext<IInvokeActivity> turnContext, JObject query, CancellationToken cancellationToken)
        {
            // The Preview card's Tap should have a Value property assigned, this will be returned to the bot in this event. 
            var (packageId, version, description, projectUrl, iconUrl) = query.ToObject<(string, string, string, string, string)>();

            // We take every row of the results and wrap them in cards wrapped in in MessagingExtensionAttachment objects.
            // The Preview is optional, if it includes a Tap, that will trigger the OnTeamsMessagingExtensionSelectItemAsync event back on this bot.
        
            var card = new ThumbnailCard
            {
                Title = $"{packageId}, {version}",
                Subtitle = description,
                Buttons = new List<CardAction>
                    {
                        new CardAction { Type = ActionTypes.OpenUrl, Title = "Nuget Package", Value = $"https://www.nuget.org/packages/{packageId}" },
                        new CardAction { Type = ActionTypes.OpenUrl, Title = "Project", Value = projectUrl },
                    },
            };

            if (!string.IsNullOrEmpty(iconUrl))
            {
                card.Images = new List<CardImage>() { new CardImage(iconUrl, "Icon") };
            }

            var attachment = new MessagingExtensionAttachment
            {
                ContentType = ThumbnailCard.ContentType,
                Content = card,
            };

            return Task.FromResult(new MessagingExtensionResponse
            {
                ComposeExtension = new MessagingExtensionResult
                {
                    Type = "result",
                    AttachmentLayout = "list",
                    Attachments = new List<MessagingExtensionAttachment> { attachment }
                }
            });
        }

        // Generate a set of substrings to illustrate the idea of a set of results coming back from a query. 
        private async Task<IEnumerable<(string, string, string, string, string)>> FindPackages(string text)
        {
            var obj = JObject.Parse(await (new HttpClient()).GetStringAsync($"https://azuresearch-usnc.nuget.org/query?q=id:{text}&prerelease=true"));
            return obj["data"].Select(item => (item["id"].ToString(), item["version"].ToString(), item["description"].ToString(), item["projectUrl"]?.ToString(), item["iconUrl"]?.ToString()));
        }

        public MessagingExtensionResponse GetAdaptiveCard()
        {
            string filepath = "./Resources/RestaurantCard.json";
            var previewcard = new ThumbnailCard
            {
                Title = "Adaptive Card",
                Text = "Please select to get Adaptive card"
            };

            var adaptiveList = FetchAdaptive(filepath);
            var attachment = new MessagingExtensionAttachment
            {
                ContentType = AdaptiveCards.AdaptiveCard.ContentType,
                Content = adaptiveList.Content,
                Preview = previewcard.ToAttachment()
            };

            return new MessagingExtensionResponse
            {
                ComposeExtension = new MessagingExtensionResult
                {
                    Type = "result",
                    AttachmentLayout = "list",
                    Attachments = new List<MessagingExtensionAttachment> { attachment }
                }
            };
        }

        public MessagingExtensionResponse GetConnectorCard()
        {
            string filepath = "./Resources/connectorCard.json";
            var previewcard = new ThumbnailCard
            {
                Title = "O365 Connector Card",
                Text = "Please select to get Connector card"
            };

            var connector = FetchConnector(filepath);
            var attachment = new MessagingExtensionAttachment
            {
                ContentType = O365ConnectorCard.ContentType,
                Content = connector.Content,
                Preview = previewcard.ToAttachment()
            };

            return new MessagingExtensionResponse
            {
                ComposeExtension = new MessagingExtensionResult
                {
                    Type = "result",
                    AttachmentLayout = "list",
                    Attachments = new List<MessagingExtensionAttachment> { attachment }
                }
            };
        }

        public static Attachment FetchAdaptive(string filepath)
        {
            var adaptiveCardJson = File.ReadAllText(filepath);
            var adaptiveCardAttachment = new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = JsonConvert.DeserializeObject(adaptiveCardJson)               
            };

            return adaptiveCardAttachment;           
        }

        public Attachment FetchConnector(string filepath)
        {
            var connectorCardJson = File.ReadAllText(filepath);
            var connectorCardAttachment = new MessagingExtensionAttachment
            {
                ContentType = O365ConnectorCard.ContentType,
                Content = JsonConvert.DeserializeObject(connectorCardJson)
            };

            return connectorCardAttachment;
        }

        public MessagingExtensionResponse GetResultGrid()
        {
            List<string> images = new List<string>();
            images.Add("https://picsum.photos/300?image=882");
            images.Add("http://connectorsdemo.azurewebsites.net/images/WIN12_Scene_01.jpg");
            images.Add("https://image.flaticon.com/icons/png/512/732/732221.png");
            images.Add("https://cdn0.iconfinder.com/data/icons/flat-round-system/512/microsoft_windows-512.png");
            images.Add("http://connectorsdemo.azurewebsites.net/images/WIN12_Anthony_02.jpg");
            images.Add("https://icons-for-free.com/iconfiles/png/512/business+company+estate+office+work+icon-1320086520504455343.png");
            images.Add("https://cdn1.iconfinder.com/data/icons/application-file-formats/128/microsoft-excel-512.png");
            images.Add("http://connectorsdemo.azurewebsites.net/images/WIN14_Jan_04.jpg");
            images.Add("https://image.shutterstock.com/image-photo/image-150nw-148441982.jpg");
            images.Add("https://avatars.slack-edge.com/2019-05-15/636116561829_46b2865d45aa5e56cf5e_512.png");
            images.Add("https://cdn.iconscout.com/icon/free/png-512/microsoft-office-2-761689.png");

            List<MessagingExtensionAttachment> attachments = new List<MessagingExtensionAttachment>();

            foreach (string img in images)
            {
                var thumbnailCard = new ThumbnailCard();              
                thumbnailCard.Images = new List<CardImage>() { new CardImage(img) };
                var attachment = new MessagingExtensionAttachment
                {
                    ContentType = ThumbnailCard.ContentType,
                    Content = new ThumbnailCard { Images = thumbnailCard.Images },
                };
                attachments.Add(attachment);
            }

            return new MessagingExtensionResponse
            {
                ComposeExtension = new MessagingExtensionResult
                {
                    Type = "result",
                    AttachmentLayout = "grid",
                    Attachments = attachments
                }
            };
        }
    }
}

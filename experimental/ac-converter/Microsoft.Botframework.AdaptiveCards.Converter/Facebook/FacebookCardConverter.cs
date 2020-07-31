using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Botframework.AdaptiveCards.Converter.Facebook.Helpers;
using Microsoft.Botframework.AdaptiveCards.Converter.Facebook.Models;
using Microsoft.Botframework.AdaptiveCards.Converter.Facebook.Models.Outgoing;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Facebook
{
    public class FacebookCardConverter
    {
        public async Task<List<FacebookChannelData>> ToFacebookChannelData(List<AdaptiveCard> adaptiveCardList)
        {
            var facebookChannelDataList = new List<FacebookChannelData>();

            foreach (var adaptiveCard in adaptiveCardList)
            {
                foreach (var element in adaptiveCard.Body)
                {
                    facebookChannelDataList.AddRange(await ConvertAdaptiveElement(element));
                }

                if (adaptiveCard.Actions.Any())
                {
                    facebookChannelDataList.AddRange(ConvertAdaptiveActions(adaptiveCard.Actions));
                }
            }
            return facebookChannelDataList;
        }

        /// <summary>
        /// Convert general Adaptive card element to FacebookChannelData.
        /// </summary>
        /// <param name="adaptiveElement">The adaptive card element need to convert.</param>
        private async Task<List<FacebookChannelData>> ConvertAdaptiveElement(AdaptiveElement adaptiveElement)
        {
            if (adaptiveElement is AdaptiveTextBlock textBlock)
            {
                return await ConvertTextBlock(textBlock);
            }
            else if (adaptiveElement is AdaptiveImage image)
            {
                return await ConvertAdaptiveImage(image);
            }
            else if (adaptiveElement is AdaptiveMedia media)
            {
                return await ConvertAdaptiveMedia(media);
            }
            else if (adaptiveElement is AdaptiveRichTextBlock richTextBlock)
            {
                return await ConvertRichTextBlock(richTextBlock);
            }
            else if (adaptiveElement is AdaptiveActionSet actionSet)
            {
                return ConvertAdaptiveActionSet(actionSet);
            }
            else if (adaptiveElement is AdaptiveContainer container)
            {
                return await ConvertAdaptiveContainer(container);
            }
            else if (adaptiveElement is AdaptiveColumnSet columnSet)
            {
                return await ConvertColumnSet(columnSet);
            }
            else if (adaptiveElement is AdaptiveFactSet factSet)
            {
                return await ConvertFactSet(factSet);
            }
            else if (adaptiveElement is AdaptiveImageSet imageSet)
            {
                return await ConvertAdaptiveImageSet(imageSet);
            }
            else
            {
                throw new NotSupportedException($"Element type not support, type: {adaptiveElement.GetType().FullName}");
            }
        }

        #region Card elements
        public async Task<List<FacebookChannelData>> ConvertTextBlock(AdaptiveTextBlock textBlock)
        {
            var facebookChannelDataList = new List<FacebookChannelData>();
            var facebookChannelData = new FacebookChannelData
            {
                Text = textBlock.Text
            };

            if (textBlock.Weight == AdaptiveTextWeight.Bolder)
            {
                facebookChannelData.Text = string.Format("*{0}*", facebookChannelData.Text);
            }

            facebookChannelDataList.Add(facebookChannelData);
            return facebookChannelDataList;
        }

        public async Task<List<FacebookChannelData>> ConvertAdaptiveImage(AdaptiveImage image)
        {
            var facebookChannelDataList = new List<FacebookChannelData>();
            var facebookAttachment = new FacebookAttachment
            {
                Type = AttachmentType.Image,
                Payload = new FileAttachmentPayload
                {
                    Url = image.Url.AbsoluteUri,
                    IsReusable = false
                }
            };

            var facebookChannelData = new FacebookChannelData
            {
                Attachment = facebookAttachment
            };

            facebookChannelDataList.Add(facebookChannelData);
            return facebookChannelDataList;
        }

        /// <summary>
        /// Convert general adaptive media <see cref="AdaptiveMedia"/> to facebook messages.
        /// </summary>
        /// <param name="media">The media need to convert.</param>
        /// <returns></returns>
        private async Task<List<FacebookChannelData>> ConvertAdaptiveMedia(AdaptiveMedia media)
        {
            var facebookChannelDataList = new List<FacebookChannelData>();
            foreach (var mediaSource in media.Sources)
            {

                facebookChannelDataList.AddRange(await ConvertAdaptiveMediaSource(mediaSource));
            }

            return facebookChannelDataList;
        }

        private async Task<List<FacebookChannelData>> ConvertAdaptiveMediaSource(AdaptiveMediaSource mediaSource)
        {
            var facebookChannelDataList = new List<FacebookChannelData>();
            switch (mediaSource.MimeType)
            {
                case "audio/basic":
                case "audio/mid":
                case "audio/mpeg":
                case "audio/x-aiff":
                case "audio/x-mpegurl":
                case "audio/x-pn-realaudio":
                case "audio/x-wav":
                    {
                        var facebookAttachment = new FacebookAttachment
                        {
                            Type = AttachmentType.Audio,
                            Payload = new FileAttachmentPayload
                            {
                                Url = mediaSource.Url,
                                IsReusable = false
                            }
                        };

                        var facebookChannelData = new FacebookChannelData
                        {
                            Attachment = facebookAttachment
                        };

                        facebookChannelDataList.Add(facebookChannelData);
                        break;
                    }

                case "image/bmp":
                case "image/cis-cod":
                case "image/gif":
                case "image/ief":
                case "image/jpeg":
                case "image/pipeg":
                case "image/svg+xml":
                case "image/tiff":
                case "image/x-cmu-raster":
                case "image/x-cmx":
                case "image/x-icon":
                case "image/x-portable-anymap":
                case "image/x-portable-bitmap":
                case "image/x-portable-graymap":
                case "image/x-portable-pixmap":
                case "image/x-rgb":
                case "image/x-xbitmap":
                case "image/x-xpixmap":
                case "image/x-xwindowdump":
                    {
                        var facebookAttachment = new FacebookAttachment
                        {
                            Type = AttachmentType.Image,
                            Payload = new FileAttachmentPayload
                            {
                                Url = mediaSource.Url,
                                IsReusable = false
                            }
                        };

                        var facebookChannelData = new FacebookChannelData
                        {
                            Attachment = facebookAttachment
                        };

                        facebookChannelDataList.Add(facebookChannelData);
                        break;
                    }

                case "video/mp4":
                case "video/mpeg":
                case "video/quicktime":
                case "video/x-la-asf":
                case "video/x-ms-asf":
                case "video/x-msvideo":
                case "video/x-sgi-movie":
                    {
                        var facebookAttachment = new FacebookAttachment
                        {
                            Type = AttachmentType.Video,
                            Payload = new FileAttachmentPayload
                            {
                                Url = mediaSource.Url,
                                IsReusable = false
                            }
                        };

                        var facebookChannelData = new FacebookChannelData
                        {
                            Attachment = facebookAttachment
                        };

                        facebookChannelDataList.Add(facebookChannelData);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return facebookChannelDataList;
        }

        public async Task<List<FacebookChannelData>> ConvertRichTextBlock(AdaptiveRichTextBlock richTextBlock)
        {
            var facebookChannelDataList = new List<FacebookChannelData>();
            var richTextBlockContents = new StringBuilder();
            foreach (var inline in richTextBlock.Inlines)
            {
                var textRun = inline as AdaptiveTextRun;
                var text = textRun.Text;
                if (textRun.Italic)
                {
                    text = string.Format("_{0}_", text);
                }

                if (textRun.Strikethrough)
                {
                    text = string.Format("~{0}~", text);
                }

                if (textRun.Weight == AdaptiveTextWeight.Bolder)
                {
                    text = string.Format("*{0}*", text);

                }

                richTextBlockContents.Append(text + " ");
            }

            var facebookChannelData = new FacebookChannelData
            {
                Text = richTextBlockContents.ToString()
            };

            facebookChannelDataList.Add(facebookChannelData);
            return facebookChannelDataList;
        }

        #endregion

        #region Containers

        /// <summary>
        /// Convert action set <see cref="AdaptiveActionSet"/> to facebook button messages.
        /// </summary>
        /// <param name="adaptiveActionSet">The action set need to convert.</param>
        private List<FacebookChannelData> ConvertAdaptiveActionSet(AdaptiveActionSet adaptiveActionSet)
        {
            return ConvertAdaptiveActions(adaptiveActionSet.Actions);
        }

        /// <summary>
        /// </summary>
        /// <param name="adaptiveContainer">The Adaptive card "Container" Element.</param>
        /// <returns>The converted <see cref="BoxComponent"/></returns>
        private async Task<List<FacebookChannelData>> ConvertAdaptiveContainer(AdaptiveContainer adaptiveContainer)
        {
            var facebookChannelDataList = new List<FacebookChannelData>();
            foreach (var element in adaptiveContainer.Items)
            {
                facebookChannelDataList.AddRange(await ConvertAdaptiveElement(element));
            }

            return facebookChannelDataList;
        }

        /// <summary>
        /// Convert adaptive card column set <see cref="AdaptiveColumnSet"/> to Facebook messages.
        /// </summary>
        /// <param name="columnSet">The column set need to convert.</param>
        private async Task<List<FacebookChannelData>> ConvertColumnSet(AdaptiveColumnSet columnSet)
        {
            var facebookChannelDataList = new List<FacebookChannelData>();
            foreach (var column in columnSet.Columns)
            {
                facebookChannelDataList.AddRange(await ConvertColumn(column));
            }

            return facebookChannelDataList;
        }

        /// <summary>
        /// Convert adaptive card Column <see cref="AdaptiveColumn"/> to Facebook messages.
        /// </summary>
        /// <param name="column">The Column need to convert.</param>
        private async Task<List<FacebookChannelData>> ConvertColumn(AdaptiveColumn column)
        {
            var facebookChannelDataList = new List<FacebookChannelData>();
            foreach (var element in column.Items)
            {
                facebookChannelDataList.AddRange(await ConvertAdaptiveElement(element));
            }

            return facebookChannelDataList;
        }

        public async Task<List<FacebookChannelData>> ConvertFactSet(AdaptiveFactSet adaptiveFactSet)
        {
            var facebookChannelDataList = new List<FacebookChannelData>();
            var factSetContents = new StringBuilder();
            foreach (var fact in adaptiveFactSet.Facts)
            {
                var line = string.Format("*{0}* {1}", fact.Title, fact.Value);
                factSetContents.AppendLine(line);
            }

            var facebookChannelData = new FacebookChannelData
            {
                Text = factSetContents.ToString()
            };

            facebookChannelDataList.Add(facebookChannelData);
            return facebookChannelDataList;
        }

        /// <summary>
        /// Convert image set <see cref="AdaptiveImageSet"/> to Facebook messages.
        /// </summary>
        /// <param name="imageSet">The ImageSet need to convert.</param>
        private async Task<List<FacebookChannelData>> ConvertAdaptiveImageSet(AdaptiveImageSet imageSet)
        {
            var facebookChannelDataList = new List<FacebookChannelData>();
            for (var i = 0; i < imageSet.Images.Count; i++)
            {
                var image = imageSet.Images[i];
                facebookChannelDataList.AddRange(await ConvertAdaptiveImage(image));
            }

            return facebookChannelDataList;
        }

        #endregion

        /// <summary>
        /// Convert adaptive card action <see cref="AdaptiveAction"/> to Facebook button message.
        /// </summary>
        /// <param name="action">The select action of adaptive card element.</param>
        private Button CreateButton(AdaptiveAction action)
        {
            if (action is AdaptiveOpenUrlAction openUrlAction)
            {
                var button = new Button
                {
                    Type = ButtonType.web_url,
                    Url = openUrlAction.Url.AbsoluteUri,
                    Title = openUrlAction?.Title?.Ellipsize(Constants.ButtonTitleLimit)
                };
                return button;
            }
            return null;
        }

        // TODO: Button template only support 3 buttons, need take action to handle more than 3.
        private List<FacebookChannelData> ConvertAdaptiveActions(List<AdaptiveAction> actions)
        {
            var buttons = new List<Button>();
            foreach (var action in actions)
            {
                buttons.Add(CreateButton(action));
            }

            var buttonTemplate = new ButtonTemplate
            {
                Text = Constants.ButtonTemplateName,
                Buttons = buttons.ToArray()
            };
            var facebookChannelDataList = new List<FacebookChannelData>();
            var facebookAttachment = new FacebookAttachment
            {
                Type = AttachmentType.Template,
                Payload = buttonTemplate
            };
            var facebookChannelData = new FacebookChannelData
            {
                Attachment = facebookAttachment
            };
            facebookChannelDataList.Add(facebookChannelData);
            return facebookChannelDataList;
        }


        #region flight template.
        public Task<List<FacebookChannelData>> ToFacebookUpdateTemplate(List<AdaptiveCard> adaptiveCards)
        {
            var facebookChannelDataList = new List<FacebookChannelData>();
            foreach (var adaptiveCard in adaptiveCards)
            {
                var flightUpdateTemplate = new FlightUpdateTemplate()
                {
                    IntroMessage = adaptiveCard.Speak,
                    UpdateType = "delay",
                    PNRNumber = "test",
                    UpdateFilghtInfo = new FlightInfo
                    {
                        FlightNumber = ((adaptiveCard.Body[2] as AdaptiveColumnSet).Columns[0].Items[1] as AdaptiveTextBlock).Text,
                        DepartureAirport = new Airport
                        {
                            AirportCode = ((adaptiveCard.Body[3] as AdaptiveColumnSet).Columns[0].Items[1] as AdaptiveTextBlock).Text,
                            City = ((adaptiveCard.Body[3] as AdaptiveColumnSet).Columns[0].Items[0] as AdaptiveTextBlock).Text,
                        },
                        ArrivalAirport = new Airport
                        {
                            AirportCode = ((adaptiveCard.Body[3] as AdaptiveColumnSet).Columns[2].Items[1] as AdaptiveTextBlock).Text,
                            City = ((adaptiveCard.Body[3] as AdaptiveColumnSet).Columns[2].Items[0] as AdaptiveTextBlock).Text,
                        },
                        FlightSchedule = new Schedule
                        {
                            DepartureTime = GetFBDateFormat(((adaptiveCard.Body[2] as AdaptiveColumnSet).Columns[1].Items[1] as AdaptiveTextBlock).Text),
                            ArrivalTime = GetFBDateFormat(((adaptiveCard.Body[2] as AdaptiveColumnSet).Columns[2].Items[1] as AdaptiveTextBlock).Text),
                        }
                    }
                };

                var facebookAttachment = new FacebookAttachment
                {
                    Type = "template",
                    Payload = flightUpdateTemplate
                };

                var facebookChannelData = new FacebookChannelData
                {
                    Attachment = facebookAttachment
                };

                facebookChannelDataList.Add(facebookChannelData);
            }
            return Task.FromResult(facebookChannelDataList);
        }

        private string GetFBDateFormat(string time)
        {
            var dt = DateTime.Parse(time);
            return dt.ToString("s");
        }

        #endregion
    }
}

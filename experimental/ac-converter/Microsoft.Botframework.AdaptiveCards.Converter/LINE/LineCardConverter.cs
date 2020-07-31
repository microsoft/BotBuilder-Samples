using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AdaptiveCards;

using Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE
{
    public class LineCardConverter
    {
        public async Task<object> ToChannelData(List<AdaptiveCard> cards, string attachmentLayout = "list")
        {
            var message = new FlexMessage(nameof(AdaptiveCard));
            var containers = await ToFlexContainerList(cards).ConfigureAwait(false);
            var container = new CarouselContainer()
            {
                Contents = containers,
            };

            message.Contents = container;

            return message;
        }

        public async Task<object> ToChannelData(AdaptiveCard card)
        {
            var message = new FlexMessage(card.FallbackText ?? nameof(AdaptiveCard))
            {
                Contents = await ToFlexContainer(card).ConfigureAwait(false)
            };

            return message;
        }

        private async Task<IList<BubbleContainer>> ToFlexContainerList(List<AdaptiveCard> cards)
        {
            var containers = new List<BubbleContainer>();
            foreach (var card in cards)
            {
                containers.Add(await ToFlexContainer(card).ConfigureAwait(false));
            }

            return containers;
        }

        private async Task<BubbleContainer> ToFlexContainer(AdaptiveCard card)
        {
            var boxComponent = new BoxComponent();
            if (card.BackgroundImage != null)
            {
                boxComponent.AddContents(await ConvertBackGroundImage(card.BackgroundImage).ConfigureAwait(false));
            }

            foreach (var element in card.Body)
            {
                boxComponent.AddContents(await ConvertAdaptiveElement(element).ConfigureAwait(false));
            }

            if (card.SelectAction != null)
            {
                boxComponent.Action = ConvertSelectAction(card.SelectAction);
            }

            var bubbleContainer = new BubbleContainer();
            bubbleContainer.SetBody(boxComponent);
            // Set actions in flex message footer.
            if (card.Actions.Any())
            {
                var footer = new BoxComponent()
                {
                    Layout = BoxLayout.Vertical
                };

                foreach (var action in card.Actions)
                {
                    var button = new ButtonComponent(ConvertAdaptiveAction(action));
                    footer.AddContents(button);
                }

                if (footer.IsValid())
                {
                    bubbleContainer.SetFooter(footer);
                }
            }

            return bubbleContainer;
        }


        #region Card elements
        public async Task<IFlexComponent> ConvertTextBlock(AdaptiveTextBlock textBlock)
        {
            var textComponent = new TextComponent();
            textComponent.Text = textBlock.Text;
            if (textBlock.IsSubtle)
            {
                textComponent.Color = LineCardConverterHelper.ToSubtleColorHex(textBlock.Color);
            }
            else
            {
                textComponent.Color = LineCardConverterHelper.ToColorHex(textBlock.Color);
            }

            textComponent.Align = LineCardConverterHelper.ToAlign(textBlock.HorizontalAlignment);
            textComponent.MaxLines = textBlock.MaxLines;
            textComponent.Size = LineCardConverterHelper.ToTextSize(textBlock.Size);
            textComponent.Weight = LineCardConverterHelper.ToWeight(textBlock.Weight);
            textComponent.Wrap = textBlock.Wrap;
            var boxComponent = new BoxComponent();
            boxComponent.AddContents(textComponent);
            return boxComponent;
        }

        public async Task<IFlexComponent> ConvertAdaptiveImage(AdaptiveImage image)
        {
            var boxComponent = new BoxComponent()
            {
                Action = ConvertSelectAction(image.SelectAction)
            };

            var imageComponent = new ImageComponent();
            imageComponent.Url = image.Url.AbsoluteUri;
            imageComponent.BackgroundColor = image.BackgroundColor;
            imageComponent.Align = LineCardConverterHelper.ToAlign(image.HorizontalAlignment);
            imageComponent.Size = LineCardConverterHelper.ToImageSize(image.Size);

            if (image.PixelHeight > 0)
            {
                boxComponent.Height = string.Format(PixelString.px, image.PixelHeight.ToString());
            }

            if (image.PixelWidth > 0)
            {
                boxComponent.Width = string.Format(PixelString.px, image.PixelWidth.ToString());
            }

            if (image.PixelHeight > 0 || image.PixelWidth > 0)
            {

                // Set 'height' / 'weight' will override 'size'.
                imageComponent.Size = LineCardConverterHelper.ToImageSize(AdaptiveImageSize.Stretch);

                // Our approach to handle AdaptiveImage's height and weight:
                // Put ImageComponent in a BoxComponent, set this BoxComponent's height and weight.
                // For ImageComponent, set size to 'full'. Set aspectRatio. Set aspectMode to 'cover'.
                // This approach will render a same drawing area as AdaptiveImage. 
                // But parts of the image that do not fit in the drawing area are not displayed.
                if (image.PixelHeight > 0 && image.PixelWidth > 0)
                {
                    var height = Math.Min(image.PixelHeight, image.PixelWidth * 3);
                    imageComponent.AspectRatio = new AspectRatio((int)image.PixelWidth, (int)height);
                    imageComponent.AspectMode = AspectMode.Cover;
                }
                else
                {
                    // If one of weight and height is 0, keep the original aspectRatio and aspectMode.
                    imageComponent.AspectMode = AspectMode.Fit;
                }
            }

            boxComponent.AddContents(imageComponent);
            return boxComponent;
        }

        /// <summary>
        /// Convert general adaptive media <see cref="AdaptiveMedia"/> to flex component.
        /// </summary>
        /// <param name="media">The media need to convert.</param>
        /// <returns></returns>
        private IFlexComponent ConvertAdaptiveMedia(AdaptiveMedia media)
        {

            // We are not support general media in flex component right now.
            // We use ImageComponent to show images.
            // Otherwise downgrade to alt text if user provided.
            var boxComponent = new BoxComponent();
            bool isValid = false;
            foreach (var mediaSource in media.Sources)
            {
                if (MIMETypes.ImageTypes.Contains(mediaSource.MimeType))
                {
                    isValid = true;
                    var imageComponent = new ImageComponent();
                    imageComponent.Url = new Uri(mediaSource.Url).AbsoluteUri;
                    boxComponent.AddContents(imageComponent);
                }
            }

            // All media cannot be shown.
            if (!isValid)
            {
                if (!string.IsNullOrEmpty(media.AltText))
                {
                    var textComponent = new TextComponent(media.AltText);
                    boxComponent.AddContents(textComponent);
                }
                else
                {
                    throw new NotSupportedException($"Media is not support, type: {media.Type}");
                }
            }

            return boxComponent;
        }

        public async Task<IFlexComponent> ConvertRichTextBlock(AdaptiveRichTextBlock richTextBlock)
        {
            var textComponent = new TextComponent();
            var contents = new List<IFlexComponent>();
            foreach (var inline in richTextBlock.Inlines)
            {
                var textRun = inline as AdaptiveTextRun;
                var span = new SpanComponent();
                span.Text = textRun.Text;
                if (textRun.IsSubtle)
                {
                    span.Color = LineCardConverterHelper.ToSubtleColorHex(textRun.Color);
                }
                else
                {
                    span.Color = LineCardConverterHelper.ToColorHex(textRun.Color);
                }

                if (textRun.Italic)
                {
                    span.Style = Style.Italic;
                }
                else
                {
                    span.Style = Style.Normal;
                }

                span.Size = LineCardConverterHelper.ToTextSize(textRun.Size);
                if (textRun.Strikethrough)
                {
                    span.Decoration = Decoration.Linethrough;
                }
                else
                {
                    span.Decoration = Decoration.None;
                }

                span.Weight = LineCardConverterHelper.ToWeight(textRun.Weight);
                contents.Add(span);
            }

            textComponent.Contents = contents;
            textComponent.Align = LineCardConverterHelper.ToAlign(richTextBlock.HorizontalAlignment);
            var boxComponent = new BoxComponent();
            boxComponent.AddContents(textComponent);
            return boxComponent;
        }

        #endregion

        #region Containers
        /// <summary>
        /// </summary>
        /// <param name="adaptiveContainer">The Adaptive card "Container" Element.</param>
        /// <returns>The converted <see cref="BoxComponent"/></returns>
        private async Task<BoxComponent> ConvertAdaptiveContainer(AdaptiveContainer adaptiveContainer)
        {
            var boxComponent = new BoxComponent
            {
                Layout = ConvertVerticalContentAlignment(adaptiveContainer.VerticalContentAlignment),
                Action = ConvertSelectAction(adaptiveContainer.SelectAction)
            };

            // Add Contents will only add the valid component, the invalid one will be ignored.
            // Background iamge must be the first content otherwise it will cover other component.
            if (adaptiveContainer.BackgroundImage != null)
            {
                boxComponent.AddContents(await ConvertBackGroundImage(adaptiveContainer.BackgroundImage).ConfigureAwait(false));
            }

            foreach (var element in adaptiveContainer.Items)
            {
                boxComponent.AddContents(await ConvertAdaptiveElement(element).ConfigureAwait(false));
            }

            return boxComponent;
        }

        /// <summary>
        /// Convert general Adaptive card element to Flex component.
        /// </summary>
        /// <param name="adaptiveElement">The adaptive card element need to convert.</param>
        private async Task<IFlexComponent> ConvertAdaptiveElement(AdaptiveElement adaptiveElement)
        {
            if (adaptiveElement is AdaptiveTextBlock textBlock)
            {
                return await ConvertTextBlock(textBlock).ConfigureAwait(false);
            }
            else if (adaptiveElement is AdaptiveColumnSet columnSet)
            {
                return await ConvertColumnSet(columnSet).ConfigureAwait(false);
            }
            else if (adaptiveElement is AdaptiveImage image)
            {
                return await ConvertAdaptiveImage(image).ConfigureAwait(false);
            }
            else if (adaptiveElement is AdaptiveImageSet imageSet)
            {
                return await ConvertAdaptiveImageSet(imageSet).ConfigureAwait(false);
            }
            else if (adaptiveElement is AdaptiveMedia media)
            {
                return ConvertAdaptiveMedia(media);
            }
            else if (adaptiveElement is AdaptiveRichTextBlock richTextBlock)
            {
                return await ConvertRichTextBlock(richTextBlock).ConfigureAwait(false);
            }
            else if (adaptiveElement is AdaptiveActionSet actionSet)
            {
                return ConvertAdaptiveActionSet(actionSet);
            }
            else if (adaptiveElement is AdaptiveContainer container)
            {
                return await ConvertAdaptiveContainer(container).ConfigureAwait(false);
            }
            else if (adaptiveElement is AdaptiveFactSet factSet)
            {
                return await ConvertFactSet(factSet).ConfigureAwait(false);
            }
            else
            {
                // Should container the flowing types.
                // Input.ChoiceSet, Input.Date, Input.Number, Input.Text, Input.Time, Input.Toggle
                throw new NotSupportedException($"Element type not support, type: {adaptiveElement.GetType().FullName}");
            }
        }

        /// <summary>
        /// Convert adaptive card select action <see cref="AdaptiveAction"/> to LINE template action.
        /// </summary>
        /// <param name="selectAction">The select action of adaptive card element.</param>
        private ITemplateAction ConvertSelectAction(AdaptiveAction selectAction)
        {
            if (selectAction == null)
            {
                return null;
            }

            if (selectAction is AdaptiveOpenUrlAction openUrlAction)
            {
                return new UriAction(openUrlAction.Title, openUrlAction.Url.AbsoluteUri);
            }

            // throw new NotSupportedException($"SelectAction type not support, type: {selectAction.Type}");

            return null;
        }

        /// <summary>
        /// Convert adaptive card select action <see cref="AdaptiveAction"/> to LINE template action.
        /// </summary>
        /// <param name="action">The select action of adaptive card element.</param>
        private ITemplateAction ConvertAdaptiveAction(AdaptiveAction action)
        {
            if (action is AdaptiveOpenUrlAction openUrlAction)
            {
                return new UriAction(openUrlAction.Title, openUrlAction.Url.AbsoluteUri);
            }

            // throw new NotSupportedException($"SelectAction type not support, type: {action.Type}");

            return null;
        }

        /// <summary>
        /// Convert action set <see cref="AdaptiveActionSet"/> to flex component.
        /// </summary>
        /// <param name="adaptiveActionSet">The action set need to convert.</param>
        private IFlexComponent ConvertAdaptiveActionSet(AdaptiveActionSet adaptiveActionSet)
        {
            var boxComponent = new BoxComponent()
            {
                Layout = BoxLayout.Horizontal
            };

            foreach (var action in adaptiveActionSet.Actions)
            {
                var button = new ButtonComponent(ConvertAdaptiveAction(action));
                boxComponent.AddContents(button);
            }

            return boxComponent;
        }

        /// <summary>
        /// Convert image set <see cref="AdaptiveImageSet"/> to flex component.
        /// </summary>
        /// <param name="imageSet">The ImageSet need to convert.</param>
        private async Task<IFlexComponent> ConvertAdaptiveImageSet(AdaptiveImageSet imageSet)
        {
            var boxComponent = new BoxComponent()
            {
                Layout = BoxLayout.Vertical
            };

            var maxCountPerLine = 1;
            switch (imageSet.ImageSize)
            {
                case AdaptiveImageSize.Large:
                    maxCountPerLine = 2;
                    break;
                case AdaptiveImageSize.Medium:
                    maxCountPerLine = 4;
                    break;
                case AdaptiveImageSize.Small:
                    maxCountPerLine = 8;
                    break;
                case AdaptiveImageSize.Auto:
                case AdaptiveImageSize.Stretch:
                    break;
            }

            var imageBox = new BoxComponent(BoxLayout.Horizontal);
            for (var i = 0; i < imageSet.Images.Count; i++)
            {
                var image = imageSet.Images[i];
                imageBox.AddContents(await ConvertAdaptiveImage(image).ConfigureAwait(false));

                if (imageBox.Contents.Count == maxCountPerLine)
                {
                    boxComponent.AddContents(imageBox);
                    imageBox = new BoxComponent(BoxLayout.Horizontal);
                }

                if (i == imageSet.Images.Count - 1 && imageBox.Contents.Count != 0)
                {
                    boxComponent.AddContents(imageBox);
                }
            }


            return boxComponent;
        }

        /// <summary>
        /// Convert adaptive card column set <see cref="AdaptiveColumnSet"/> to flex component.
        /// </summary>
        /// <param name="columnSet">The column set need to convert.</param>
        private async Task<IFlexComponent> ConvertColumnSet(AdaptiveColumnSet columnSet)
        {
            var boxComponent = new BoxComponent
            {
                Layout = BoxLayout.Horizontal,
                Action = ConvertSelectAction(columnSet.SelectAction)
            };

            foreach (var column in columnSet.Columns)
            {
                boxComponent.AddContents(await ConvertColumn(column).ConfigureAwait(false));
            }

            return boxComponent;
        }

        /// <summary>
        /// Convert adaptive card Column <see cref="AdaptiveColumn"/> to flex component.
        /// </summary>
        /// <param name="column">The Column need to convert.</param>
        private async Task<IFlexComponent> ConvertColumn(AdaptiveColumn column)
        {
            int.TryParse(column.Width, out var flex);
            var boxComponent = new BoxComponent()
            {
                Flex = flex != 0 ? flex : 1,
                Height = column.Height.IsPixel() ? column.Height.ToString() : null
            };

            foreach (var element in column.Items)
            {
                boxComponent.AddContents(await ConvertAdaptiveElement(element).ConfigureAwait(false));
            }

            return boxComponent;
        }

        public async Task<IFlexComponent> ConvertFactSet(AdaptiveFactSet adaptiveFactSet)
        {
            var boxComponent = new BoxComponent();
            foreach (var fact in adaptiveFactSet.Facts)
            {
                var titleSpanComponent = new SpanComponent();

                // Leave a space here to separate title and value. 
                titleSpanComponent.Text = fact.Title + " ";
                titleSpanComponent.Size = LineCardConverterHelper.ToTextSize(AdaptiveTextSize.Default);
                titleSpanComponent.Weight = Weight.Bold;

                var valueSpanComponent = new SpanComponent();
                valueSpanComponent.Text = fact.Value;
                valueSpanComponent.Size = LineCardConverterHelper.ToTextSize(AdaptiveTextSize.Default);

                var textContents = new List<IFlexComponent>();
                textContents.Add(titleSpanComponent);
                textContents.Add(valueSpanComponent);

                var textComponent = new TextComponent();
                textComponent.Contents = textContents;
                boxComponent.AddContents(textComponent);
            }

            return boxComponent;
        }

        /// <summary>
        /// Convert <see cref="AdaptiveVerticalContentAlignment"/> to <see cref="BoxComponent.Layout"/>.
        /// </summary>
        /// <param name="verticalContentAlignment"></param>
        /// <param name="boxComponent"></param>
        /// <returns></returns>
        private string ConvertVerticalContentAlignment(AdaptiveVerticalContentAlignment verticalContentAlignment)
        {
            switch (verticalContentAlignment)
            {
                case AdaptiveVerticalContentAlignment.Center:
                    return BoxLayout.Baseline;
                case AdaptiveVerticalContentAlignment.Bottom:
                default:
                    return BoxLayout.Horizontal;
            }
        }

        #endregion

        /// <summary>
        /// Convert adaptive card BackgroundImage to flex message image component <see cref="ImageComponent">
        /// Note: This component should be the FIRST element of its parent container.
        /// Or other elements will be covered.
        /// </summary>
        /// <param name="adaptiveBackgroundImage"></param>
        /// <returns></returns>
        public async Task<IFlexComponent> ConvertBackGroundImage(AdaptiveBackgroundImage adaptiveBackgroundImage)
        {
            var imageComponent = new ImageComponent();
            imageComponent.Url = adaptiveBackgroundImage.Url.AbsoluteUri;
            imageComponent.Size = LineCardConverterHelper.ToImageSize(AdaptiveImageSize.Stretch);
            imageComponent.AspectMode = AspectMode.Cover;
            imageComponent.Position = Position.Absolute;
            imageComponent.OffsetTop = string.Format(PixelString.px, "0");
            imageComponent.OffsetStart = string.Format(PixelString.px, "0");
            imageComponent.OffsetEnd = string.Format(PixelString.px, "0");
            imageComponent.OffsetBottom = string.Format(PixelString.px, "0");
            return imageComponent;
        }
    }
}

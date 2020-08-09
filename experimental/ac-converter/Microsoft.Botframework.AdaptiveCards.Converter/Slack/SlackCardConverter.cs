using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Botframework.AdaptiveCards.Converter.Slack.Models;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Slack
{
    public class SlackCardConverter
    {
        private const int maxBlockNumbers = 50;

        public async Task<object> ToChannelData(List<AdaptiveCard> cards)
        {
            var slackBlocks = new List<ISlackBlock>();
            foreach (var card in cards)
            {
                var newBlocks = await ConvertAdaptiveCard(card).ConfigureAwait(false);
                if (slackBlocks.Count + newBlocks.Count <= maxBlockNumbers)
                {
                    slackBlocks.AddRange(newBlocks);
                }
                else
                {
                    break;
                }
            }

            var message = new SlackPostMessage()
            {
                text = "ChannelData",
                blocks = slackBlocks.ToArray()
            };

            return message;
        }

        public async Task<object> ToChannelData(AdaptiveCard card)
        {
            var slackBlocks = await ConvertAdaptiveCard(card).ConfigureAwait(false);

            var message = new SlackPostMessage()
            {
                text = "ChannelData",
                blocks = slackBlocks.ToArray()
            };

            return message;
        }

        public async Task<List<ISlackBlock>> ConvertAdaptiveCard(AdaptiveCard card)
        {
            var slackBlocks = new List<ISlackBlock>();
            foreach (var element in card.Body)
            {
                slackBlocks.AddRange(await ConvertAdaptiveElement(element).ConfigureAwait(false));
            }

            if (card.Actions.Any())
            {
                var elements = new List<IBlockElement>();
                foreach (var action in card.Actions)
                {
                    var actionElement = await ConvertAdaptiveAction(action).ConfigureAwait(false);
                    if (actionElement != null)
                    {
                        elements.Add(actionElement);
                    }
                }

                var actionBlock = new ActionsBlock()
                {
                    elements = elements.ToArray()
                };

                slackBlocks.Add(actionBlock);
            }

            return slackBlocks;
        }

        #region Card elements

        public async Task<List<ISlackBlock>> ConvertTextBlock(AdaptiveTextBlock textBlock)
        {
            var sectionBlock = new SectionBlock();
            if (textBlock.Weight == AdaptiveTextWeight.Bolder)
            {
                sectionBlock.text = new TextObject()
                {
                    text = string.Format("*{0}*", textBlock.Text),
                    type = TextType.Mrkdwn
                };
            }
            else
            {
                sectionBlock.text = new TextObject()
                {
                    text = textBlock.Text,
                    type = TextType.PlainText
                };
            }

            sectionBlock.block_id = textBlock.Id;
            var slackBlocks = new List<ISlackBlock>() { sectionBlock };
            if (textBlock.Separator)
            {
                slackBlocks = await AddDividerBlock(slackBlocks);
            }

            return slackBlocks;
        }

        public async Task<List<ISlackBlock>> ConvertAdaptiveImage(AdaptiveImage image)
        {
            var imageBlock = new ImageBlock();
            imageBlock.image_url = image.Url.AbsoluteUri;
            if (!string.IsNullOrEmpty(image.AltText))
            {
                imageBlock.alt_text = image.AltText;
            }

            imageBlock.block_id = image.Id;
            var slackBlocks = new List<ISlackBlock>() { imageBlock };
            if (image.Separator)
            {
                slackBlocks = await AddDividerBlock(slackBlocks);
            }

            return slackBlocks;
        }

        public async Task<List<ISlackBlock>> ConvertAdaptiveMedia(AdaptiveMedia media)
        {

            // We only support image right now.
            // We will show alt text if user provided.
            var slackBlocks = new List<ISlackBlock>();
            bool isValid = false;
            foreach (var mediaSource in media.Sources)
            {
                if (mediaSource.MimeType == "image/jpeg")
                {
                    isValid = true;
                    var imageBlock = new ImageBlock();
                    imageBlock.image_url = mediaSource.Url;
                    slackBlocks.Add(imageBlock);
                }
            }

            // All media cannot be shown. Try to show alt text.
            if (!isValid)
            {
                if (!string.IsNullOrEmpty(media.AltText))
                {
                    var sectionBlock = new SectionBlock()
                    {
                        text = new TextObject()
                        {
                            text = media.AltText,
                            type = TextType.PlainText
                        }
                    };

                    slackBlocks.Add(sectionBlock);
                }
            }

            if (media.Separator)
            {
                slackBlocks = await AddDividerBlock(slackBlocks);
            }

            return slackBlocks;
        }

        public async Task<List<ISlackBlock>> ConvertRichTextBlock(AdaptiveRichTextBlock richTextBlock)
        {
            var fields = new List<TextObject>();
            foreach (var inline in richTextBlock.Inlines)
            {
                var textRun = inline as AdaptiveTextRun;
                var textObject = new TextObject()
                {
                    text = textRun.Text,
                    type = TextType.PlainText
                };

                if (textRun.Italic)
                {
                    textObject.text = string.Format("_{0}_", textObject.text);
                    textObject.type = TextType.Mrkdwn;
                }

                if (textRun.Strikethrough)
                {
                    textObject.text = string.Format("~{0}~", textObject.text);
                    textObject.type = TextType.Mrkdwn;
                }

                if (textRun.Weight == AdaptiveTextWeight.Bolder)
                {
                    textObject.text = string.Format("*{0}*", textObject.text);
                    textObject.type = TextType.Mrkdwn;
                }

                fields.Add(textObject);
            }

            var sectionBlock = new SectionBlock()
            {
                fields = fields.ToArray()
            };

            sectionBlock.block_id = richTextBlock.Id;
            var slackBlocks = new List<ISlackBlock>() { sectionBlock };
            if (richTextBlock.Separator)
            {
                slackBlocks = await AddDividerBlock(slackBlocks);
            }

            return slackBlocks;
        }

        #endregion

        #region Containers

        /// <summary>
        /// Convert action set <see cref="AdaptiveActionSet"/> to Slack blocks.
        /// </summary>
        /// <param name="adaptiveActionSet">The action set need to convert.</param>
        private async Task<List<ISlackBlock>> ConvertAdaptiveActionSet(AdaptiveActionSet adaptiveActionSet)
        {
            var elements = new List<IBlockElement>();
            foreach (var action in adaptiveActionSet.Actions)
            {
                var actionElement = await ConvertAdaptiveAction(action).ConfigureAwait(false);
                if (actionElement != null)
                {
                    elements.Add(actionElement);
                }
            }

            var actionBlock = new ActionsBlock()
            {
                elements = elements.ToArray()
            };

            actionBlock.block_id = adaptiveActionSet.Id;
            var slackBlocks = new List<ISlackBlock>() { actionBlock };
            if (adaptiveActionSet.Separator)
            {
                slackBlocks = await AddDividerBlock(slackBlocks);
            }

            return slackBlocks;
        }

        /// <summary>
        /// </summary>
        /// <param name="adaptiveContainer">The Adaptive card "Container" Element.</param>
        /// <returns>The converted <see cref="BoxComponent"/></returns>
        private async Task<List<ISlackBlock>> ConvertAdaptiveContainer(AdaptiveContainer adaptiveContainer)
        {
            var slackBlocks = new List<ISlackBlock>();
            foreach (var element in adaptiveContainer.Items)
            {
                slackBlocks.AddRange(await ConvertAdaptiveElement(element).ConfigureAwait(false));
            }

            if (adaptiveContainer.Separator)
            {
                slackBlocks = await AddDividerBlock(slackBlocks);
            }

            return slackBlocks;
        }

        /// <summary>
        /// Convert adaptive card column set <see cref="AdaptiveColumnSet"/> to slack blocks.
        /// </summary>
        /// <param name="columnSet">The column set need to convert.</param>
        private async Task<List<ISlackBlock>> ConvertColumnSet(AdaptiveColumnSet columnSet)
        {
            var slackBlocks = new List<ISlackBlock>();
            foreach (var column in columnSet.Columns)
            {
                slackBlocks.AddRange(await ConvertColumn(column).ConfigureAwait(false));
            }

            if (columnSet.Separator)
            {
                slackBlocks = await AddDividerBlock(slackBlocks);
            }

            return slackBlocks;
        }

        /// <summary>
        /// Convert adaptive card Column <see cref="AdaptiveColumn"/> to slack blocks.
        /// </summary>
        /// <param name="column">The Column need to convert.</param>
        private async Task<List<ISlackBlock>> ConvertColumn(AdaptiveColumn column)
        {
            var slackBlocks = new List<ISlackBlock>();
            foreach (var element in column.Items)
            {
                slackBlocks.AddRange(await ConvertAdaptiveElement(element).ConfigureAwait(false));
            }

            if (column.Separator)
            {
                slackBlocks = await AddDividerBlock(slackBlocks);
            }

            return slackBlocks;
        }

        public async Task<List<ISlackBlock>> ConvertFactSet(AdaptiveFactSet adaptiveFactSet)
        {
            var fields = new List<TextObject>();
            foreach (var fact in adaptiveFactSet.Facts)
            {
                var titleText = new TextObject()
                {
                    text = string.Format("*{0} *", fact.Title),
                    type = TextType.Mrkdwn
                };

                var valueText = new TextObject()
                {
                    text = fact.Value,
                    type = TextType.PlainText
                };

                fields.Add(titleText);
                fields.Add(valueText);
            }

            var sectionBlock = new SectionBlock()
            {
                fields = fields.ToArray()
            };

            sectionBlock.block_id = adaptiveFactSet.Id;
            var slackBlocks = new List<ISlackBlock>() { sectionBlock };
            if (adaptiveFactSet.Separator)
            {
                slackBlocks = await AddDividerBlock(slackBlocks);
            }

            return slackBlocks;
        }

        /// <summary>
        /// Convert image set <see cref="AdaptiveImageSet"/> to flex component.
        /// </summary>
        /// <param name="imageSet">The ImageSet need to convert.</param>
        private async Task<List<ISlackBlock>> ConvertAdaptiveImageSet(AdaptiveImageSet imageSet)
        {
            var slackBlocks = new List<ISlackBlock>();
            for (var i = 0; i < imageSet.Images.Count; i++)
            {
                var image = imageSet.Images[i];
                slackBlocks.AddRange(await ConvertAdaptiveImage(image).ConfigureAwait(false));
            }

            if (imageSet.Separator)
            {
                slackBlocks = await AddDividerBlock(slackBlocks);
            }

            return slackBlocks;
        }

        /// <summary>
        /// Convert general Adaptive card element to Slack block.
        /// </summary>
        /// <param name="adaptiveElement">The adaptive card element need to convert.</param>
        private async Task<List<ISlackBlock>> ConvertAdaptiveElement(AdaptiveElement adaptiveElement)
        {
            if (adaptiveElement is AdaptiveTextBlock textBlock)
            {
                return await ConvertTextBlock(textBlock).ConfigureAwait(false);
            }
            else if (adaptiveElement is AdaptiveImage image)
            {
                return await ConvertAdaptiveImage(image).ConfigureAwait(false);
            }
            else if (adaptiveElement is AdaptiveMedia media)
            {
                return await ConvertAdaptiveMedia(media).ConfigureAwait(false);
            }
            else if (adaptiveElement is AdaptiveRichTextBlock richTextBlock)
            {
                return await ConvertRichTextBlock(richTextBlock).ConfigureAwait(false);
            }
            else if (adaptiveElement is AdaptiveActionSet actionSet)
            {
                return await ConvertAdaptiveActionSet(actionSet).ConfigureAwait(false);
            }
            else if (adaptiveElement is AdaptiveContainer container)
            {
                return await ConvertAdaptiveContainer(container).ConfigureAwait(false);
            }
            else if (adaptiveElement is AdaptiveColumnSet columnSet)
            {
                return await ConvertColumnSet(columnSet).ConfigureAwait(false);
            }
            else if (adaptiveElement is AdaptiveFactSet factSet)
            {
                return await ConvertFactSet(factSet).ConfigureAwait(false);
            }
            else if (adaptiveElement is AdaptiveImageSet imageSet)
            {
                return await ConvertAdaptiveImageSet(imageSet).ConfigureAwait(false);
            }
            else
            {

                // throw new NotSupportedException($"Element type not support, type: {adaptiveElement.GetType().FullName}");
                return new List<ISlackBlock>();
            }
        }

        #endregion

        #region Actions

        /// <summary>
        /// Convert adaptive card select action <see cref="AdaptiveAction"/> to Slack action.
        /// </summary>
        /// <param name="action">The select action of adaptive card element.</param>
        private async Task<IBlockElement> ConvertAdaptiveAction(AdaptiveAction action)
        {
            if (action is AdaptiveOpenUrlAction openUrlAction)
            {
                return new Button()
                {
                    text = new TextObject()
                    {
                        text = action.Title,
                        type = TextType.PlainText
                    },
                    url = openUrlAction.Url.AbsoluteUri
                };
            }

            return null;
        }

        #endregion

        #region Common property

        public async Task<List<ISlackBlock>> AddDividerBlock(List<ISlackBlock> oldBlocks)
        {
            var newBlocksWithDivider = new List<ISlackBlock>() { new DividerBlock() };
            newBlocksWithDivider.AddRange(oldBlocks);
            return newBlocksWithDivider;
        }

        #endregion

    }
}

// <copyright file="MessageActionsPayloadBodyExtensions.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Schema.Teams
{
    using System.Collections.Generic;
    using HtmlAgilityPack;

    /// <summary>
    /// MessageActionsPayloadBody extensions.
    /// </summary>
    public static class MessageActionsPayloadBodyExtensions
    {
        private static readonly HashSet<string> TextRestrictedHtmlTags = new HashSet<string> { "at", "attachment" };

        /// <summary>
        /// Strip HTML tags from MessageActionsPayloadBody content.
        /// </summary>
        /// <param name="body">The MessageActionsPayloadBody.</param>
        /// <returns>Plain text content.</returns>
        public static string GetPlainTextContent(this MessageActionsPayloadBody body)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(body.Content);
            return StripHtmlTags(doc.DocumentNode, TextRestrictedHtmlTags);
        }

        private static string StripHtmlTags(HtmlNode node, ISet<string> tags)
        {
            string result = string.Empty;
            if (tags.Contains(node.Name))
            {
                result += node.OuterHtml;
            }
            else
            {
                foreach (HtmlNode childNode in node.ChildNodes)
                {
                    if (childNode.NodeType == HtmlNodeType.Text)
                    {
                        result += childNode.InnerText;
                    }
                    else
                    {
                        result += StripHtmlTags(childNode, tags);
                    }
                }
            }

            return result;
        }
    }
}

// <copyright file="WikipediaPage.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.WikipediaMessagingExtension.Engine.Models
{
    /// <summary>
    /// Represents page data Wikipedia API returns.
    /// </summary>
    internal class WikipediaPage
    {
        /// <summary>
        /// Gets or sets the pageid of the wikipedia article.
        /// </summary>
        public string Pageid { get; set; }

        /// <summary>
        /// Gets or sets the image thumbnail.
        /// </summary>
        public WikipediaThumbnail Thumbnail { get; set; }
    }
}

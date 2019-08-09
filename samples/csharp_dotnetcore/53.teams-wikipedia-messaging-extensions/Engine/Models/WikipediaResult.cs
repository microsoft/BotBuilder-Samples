// <copyright file="WikipediaResult.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.WikipediaMessagingExtension.Engine.Models
{
    /// <summary>
    /// Represents data Wikipedia search API returns.
    /// </summary>
    internal class WikipediaResult
    {
        /// <summary>
        /// Gets or sets the pageid of the wikipedia article.
        /// </summary>
        public string Pageid { get; set; }

        /// <summary>
        /// Gets or sets the title of the wikipedia article.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the snippet of the wikipedia article.
        /// </summary>
        public string Snippet { get; set; }
    }
}

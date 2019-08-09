// <copyright file="WikipediaQuery.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.WikipediaMessagingExtension.Engine.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Return the query data that Wikipedia API returns.
    /// </summary>
    internal class WikipediaQuery
    {
        /// <summary>
        /// Gets or sets Wikipedia search result.
        /// </summary>
        [JsonProperty(PropertyName = "search")]
        public IList<WikipediaResult> Results { get; set; }

        /// <summary>
        /// Gets or sets pages from Wikipedia article.
        /// </summary>
        public IList<WikipediaPage> Pages { get; set; }
    }
}

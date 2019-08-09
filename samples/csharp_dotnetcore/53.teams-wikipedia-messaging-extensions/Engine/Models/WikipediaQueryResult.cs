// <copyright file="WikipediaQueryResult.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.WikipediaMessagingExtension.Engine.Models
{
    /// <summary>
    /// Defines the object that contains Wikipedia query results.
    /// </summary>
    internal class WikipediaQueryResult
    {
        /// <summary>
        /// Gets or sets query result from Wikipedia API.
        /// </summary>
        public WikipediaQuery Query { get; set; }
    }
}

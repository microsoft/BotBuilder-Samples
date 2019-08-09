// <copyright file="WikipediaSearchHandler.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.WikipediaMessagingExtension.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Bot.Builder.Teams.WikipediaMessagingExtension.Engine.Models;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Newtonsoft.Json;

    /// <summary>
    /// Wikipedia search handler.
    /// </summary>
    /// <seealso cref="ISearchHandler" />
    public class WikipediaSearchHandler : ISearchHandler
    {
        /// <summary>
        /// Gets the url of Wikipedia search API.
        /// </summary>
        /// <value>
        /// The url of Wikipedia search API.
        /// </value>
        private const string WikiSearchUrlFormat = "https://en.wikipedia.org/w/api.php?action=query&list=search&srsearch={0}&srlimit={1}&sroffset={2}&format=json&formatversion=2";

        /// <summary>
        /// Gets the url of image search.
        /// </summary>
        /// <value>
        /// The url of image search API.
        /// </value>
        private const string ImageSearchUrl = "https://en.wikipedia.org/w/api.php?action=query&formatversion=2&format=json&prop=pageimages&piprop=thumbnail&pithumbsize=400&pageids=";

        /// <summary>
        /// Gets the default url of image.
        /// </summary>
        /// <value>
        /// The default url of image.
        /// </value>
        private const string DefaultImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b3/Wikipedia-logo-v2-en.svg/1200px-Wikipedia-logo-v2-en.svg.png";

        /// <summary>
        /// The HTTP client used to call Wikipedia.
        /// </summary>
        private readonly HttpClient httpClient = new HttpClient();

        /// <summary>
        /// Gets the search result asynchronously.
        /// </summary>
        /// <param name="query">The invoke query object</param>
        /// <returns>Messaging extension result.</returns>
        public async Task<MessagingExtensionResult> GetSearchResultAsync(MessagingExtensionQuery query)
        {
            MessagingExtensionResult composeExtensionResult = new MessagingExtensionResult
            {
                Type = "result",
                AttachmentLayout = "list",
                Attachments = new List<MessagingExtensionAttachment>(),
            };
            IList<WikipediaResult> searchResults = new List<WikipediaResult>();

            // Search Wikipedia
            string apiUrl = GenerateSearchApiUrl(query);
            WikipediaQueryResult queryResult = await this.InvokeWikipediaApiAsync(apiUrl).ConfigureAwait(false);
            searchResults = queryResult.Query.Results;

            // Grab pageIds so that we can batch query to fetch image urls of the pages
            IList<string> pageIds = new List<string>();
            foreach (WikipediaResult searchResult in searchResults)
            {
                pageIds.Add(searchResult.Pageid);
            }

            IDictionary<string, string> imageResults = await this.GetImageUrlAsync(pageIds).ConfigureAwait(false);

            // Genereate results
            foreach (WikipediaResult searchResult in searchResults)
            {
                string imageUrl = DefaultImageUrl; // Always set a default image url in case of failure, or image doesn't exist
                if (imageResults.ContainsKey(searchResult.Pageid))
                {
                    imageUrl = imageResults[searchResult.Pageid];
                }

                ThumbnailCard previewCard = new ThumbnailCard
                {
                    Title = HttpUtility.HtmlEncode(searchResult.Title),
                    Text = searchResult.Snippet,
                };
                previewCard.Images = new CardImage[] { new CardImage(imageUrl, searchResult.Title) };

                // Generate cards with links in the titles - preview cards don't have links
                ThumbnailCard card = new ThumbnailCard
                {
                    Title = "<a href='" +
                        HttpUtility.HtmlAttributeEncode("https://en.wikipedia.org/wiki/" +
                        Uri.EscapeDataString(searchResult.Title)) +
                        "' target='_blank'>" +
                        HttpUtility.HtmlEncode(searchResult.Title) +
                        "</a>",
                    Text = searchResult.Snippet,
                    Images = previewCard.Images,
                };
                composeExtensionResult.Attachments.Add(card.ToAttachment().ToMessagingExtensionAttachment(previewCard.ToAttachment()));
            }

            return composeExtensionResult;
        }

        /// <summary>
        /// Generates the search API URL.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Search API url.</returns>
        private static string GenerateSearchApiUrl(MessagingExtensionQuery query)
        {
            return string.Format(
                WikiSearchUrlFormat,
                Uri.EscapeDataString(query.Parameters[0].Value.ToString()),
                query.QueryOptions.Count,
                query.QueryOptions.Skip);
        }

        /// <summary>
        /// Gets the image URL asynchronously.
        /// </summary>
        /// <param name="pageIds">The page ids.</param>
        /// <returns>Image url map.</returns>
        private async Task<IDictionary<string, string>> GetImageUrlAsync(IList<string> pageIds)
        {
            string pageIdQuery = string.Join("|", pageIds);
            IDictionary<string, string> result = new Dictionary<string, string>();
            WikipediaQueryResult queryResult = await this.InvokeWikipediaApiAsync(ImageSearchUrl + pageIdQuery).ConfigureAwait(false);
            if (queryResult != null && queryResult.Query != null)
            {
                foreach (WikipediaPage page in queryResult.Query.Pages)
                {
                    if (page.Thumbnail != null)
                    {
                        result.Add(page.Pageid, page.Thumbnail.Source);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Invokes the wikipedia API asynchronously.
        /// </summary>
        /// <param name="apiUrl">The API URL.</param>
        /// <returns>Wikipedia search result.</returns>
        private async Task<WikipediaQueryResult> InvokeWikipediaApiAsync(string apiUrl)
        {
            HttpResponseMessage response = await this.httpClient.GetAsync(apiUrl).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<WikipediaQueryResult>(responseBody);
        }
    }
}

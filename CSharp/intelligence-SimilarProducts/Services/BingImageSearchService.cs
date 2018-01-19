namespace SimilarProducts.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Configuration;
    using Newtonsoft.Json;

    /// <summary>
    /// A client that handles the interactions with the Bing Image Search API.
    /// <remarks>
    /// </remarks>
    /// </summary>
    public class BingImageSearchService : IImageSearchService
    {
        /// <summary>
        /// Microsoft Computer Vision API key.
        /// </summary>
        private static readonly string ApiKey = WebConfigurationManager.AppSettings["BingSearchApiKey"];

        /// <summary>
        /// The bing API URL.
        /// </summary>
        private static readonly string BingApiUrl = "https://api.cognitive.microsoft.com/bing/v7.0/images/details?modules=SimilarProducts&mkt=en-us";

        /// <summary>
        /// Gets a list of visually similar products from an image URL.
        /// </summary>
        /// <param name="url">The URL of an image.</param>
        /// <returns>List of visually similar products' images.</returns>
        public async Task<IList<ImageResult>> GetSimilarProductImagesAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);

                string apiUrl = BingApiUrl + $"&imgUrl={HttpUtility.UrlEncode(url)}";

                var text = await httpClient.GetStringAsync(apiUrl);
                var response = JsonConvert.DeserializeObject<BingImageResponse>(text);

                return response?.VisuallySimilarProducts?.Value.Select(i => new ImageResult
                    {
                        HostPageDisplayUrl = i.HostPageDisplayUrl,
                        HostPageUrl = i.HostPageUrl,
                        Name = i.Name,
                        ThumbnailUrl = i.ThumbnailUrl,
                        WebSearchUrl = i.WebSearchUrl
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// Gets a list of visually similar products from an image stream.
        /// </summary>
        /// <param name="stream">The stream to an image.</param>
        /// <returns>List of visually similar images.</returns>
        public async Task<IList<ImageResult>> GetSimilarProductImagesAsync(Stream stream)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);

                var strContent = new StreamContent(stream);
                strContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { FileName = "Any-Name-Works" };

                var content = new MultipartFormDataContent();
                content.Add(strContent);

                var postResponse = await httpClient.PostAsync(BingApiUrl, content);
                var text = await postResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<BingImageResponse>(text);

                return response?.VisuallySimilarProducts?.Value.Select(i => new ImageResult
                    {
                        HostPageDisplayUrl = i.HostPageDisplayUrl,
                        HostPageUrl = i.HostPageUrl,
                        Name = i.Name,
                        ThumbnailUrl = i.ThumbnailUrl,
                        WebSearchUrl = i.WebSearchUrl
                    })
                    .ToList();
            }
        }
    }
}
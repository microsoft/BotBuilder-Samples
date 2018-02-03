namespace ImageCaption.Services
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Configuration;
    using Microsoft.ProjectOxford.Vision;
    using Microsoft.ProjectOxford.Vision.Contract;

    /// <summary>
    /// A wrapper around the Microsoft Cognitive Computer Vision API Service.
    /// <remarks>
    /// This class makes use of the Microsoft Computer Vision SDK.
    /// SDK: https://github.com/Microsoft/ProjectOxford-ClientSDK/blob/master/Vision/Windows/ClientLibrary"
    /// </remarks>
    /// </summary>
    public class MicrosoftCognitiveCaptionService : ICaptionService
    {
        /// <summary>
        /// Microsoft Computer Vision API key.
        /// </summary>
        private static readonly string ApiKey = WebConfigurationManager.AppSettings["MicrosoftVisionApiKey"];

        /// <summary>
        /// Microsoft Computer Vision API Endpoint.
        /// </summary>
        private static readonly string ApiEndpoint = WebConfigurationManager.AppSettings["MicrosoftVisionApiEndpoint"];

        /// <summary>
        /// The set of visual features we want from the Vision API.
        /// </summary>
        private static readonly VisualFeature[] VisualFeatures = { VisualFeature.Description };

        /// <summary>
        /// Gets the caption of an image URL.
        /// <remarks>
        /// This method calls <see cref="IVisionServiceClient.AnalyzeImageAsync(string, string[])"/> and
        /// returns the first caption from the returned <see cref="AnalysisResult.Description"/>
        /// </remarks>
        /// </summary>
        /// <param name="url">The URL to an image.</param>
        /// <returns>Description if caption found, null otherwise.</returns>
        public async Task<string> GetCaptionAsync(string url)
        {
            var client = new VisionServiceClient(ApiKey, ApiEndpoint);
            var result = await client.AnalyzeImageAsync(url, VisualFeatures);
            return ProcessAnalysisResult(result);
        }

        /// <summary>
        /// Gets the caption of the image from an image stream.
        /// <remarks>
        /// This method calls <see cref="IVisionServiceClient.AnalyzeImageAsync(Stream, string[])"/> and
        /// returns the first caption from the returned <see cref="AnalysisResult.Description"/>
        /// </remarks>
        /// </summary>
        /// <param name="stream">The stream to an image.</param>
        /// <returns>Description if caption found, null otherwise.</returns>
        public async Task<string> GetCaptionAsync(Stream stream)
        {
            var client = new VisionServiceClient(ApiKey, ApiEndpoint);
            var result = await client.AnalyzeImageAsync(stream, VisualFeatures);
            return ProcessAnalysisResult(result);
        }

        /// <summary>
        /// Processes the analysis result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>The caption if found, error message otherwise.</returns>
        private static string ProcessAnalysisResult(AnalysisResult result)
        {
            string message = result?.Description?.Captions.FirstOrDefault()?.Text;

            return string.IsNullOrEmpty(message) ?
                        "Couldn't find a caption for this one" :
                        "I think it's " + message;
        }
    }
}
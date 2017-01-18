namespace SimilarProducts.Services
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class BingImageResponse
    {
        [JsonProperty("_type")]
        public string Type { get; set; }

        public BingImageInstrumentation Instrumentation { get; set; }

        public List<BingImageVisuallySimilarProduct> VisuallySimilarProducts { get; set; }

        public string ImageInsightsToken { get; set; }
    }
}
namespace SimilarProducts.Services
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class BingImageResponse
    {
        [JsonProperty("_type")]
        public string Type { get; set; }

        public BingImageInstrumentation Instrumentation { get; set; }

        public ValueList<BingImageVisuallySimilarProduct> VisuallySimilarProducts { get; set; }

        public string ImageInsightsToken { get; set; }
    }

    public class ValueList<T>
    {
        public List<T> Value { get; set; }
    }
}
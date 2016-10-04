namespace SimilarProducts.Services
{
    using System.Collections.Generic;

    public class Instrumentation
    {
        public string pageLoadPingUrl { get; set; }
    }

    public class Thumbnail
    {
        public int width { get; set; }
        public int height { get; set; }
    }

    public class AggregateRating
    {
        public double ratingValue { get; set; }
        public double bestRating { get; set; }
    }

    public class AggregateOffer
    {
        public string name { get; set; }
        public string priceCurrency { get; set; }
        public AggregateRating aggregateRating { get; set; }
        public double lowPrice { get; set; }
        public int offerCount { get; set; }
    }

    public class VisuallySimilarProduct
    {
        public string name { get; set; }
        public string webSearchUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string datePublished { get; set; }
        public string contentUrl { get; set; }
        public string hostPageUrl { get; set; }
        public string contentSize { get; set; }
        public string encodingFormat { get; set; }
        public string hostPageDisplayUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public Thumbnail thumbnail { get; set; }
        public string imageInsightsToken { get; set; }
        public string imageId { get; set; }
        public string accentColor { get; set; }
        public AggregateOffer aggregateOffer { get; set; }
    }

    public class BingImageResponse
    {
        public string _type { get; set; }
        public Instrumentation instrumentation { get; set; }
        public List<VisuallySimilarProduct> visuallySimilarProducts { get; set; }
        public string imageInsightsToken { get; set; }
    }
}
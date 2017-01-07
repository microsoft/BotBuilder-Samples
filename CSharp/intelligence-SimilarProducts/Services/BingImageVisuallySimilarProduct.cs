namespace SimilarProducts.Services
{
    public class BingImageVisuallySimilarProduct
    {
        public string Name { get; set; }

        public string WebSearchUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        public string DatePublished { get; set; }

        public string ContentUrl { get; set; }

        public string HostPageUrl { get; set; }

        public string ContentSize { get; set; }

        public string EncodingFormat { get; set; }

        public string HostPageDisplayUrl { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public BingImageThumbnail Thumbnail { get; set; }

        public string ImageInsightsToken { get; set; }

        public string ImageId { get; set; }

        public string AccentColor { get; set; }

        public BingImageAggregateOffer AggregateOffer { get; set; }
    }
}
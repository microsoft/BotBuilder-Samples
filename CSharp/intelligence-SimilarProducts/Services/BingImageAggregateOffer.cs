namespace SimilarProducts.Services
{
    public class BingImageAggregateOffer
    {
        public string Name { get; set; }

        public string PriceCurrency { get; set; }

        public BingImageAggregateRating AggregateRating { get; set; }

        public double LowPrice { get; set; }

        public int OfferCount { get; set; }
    }
}
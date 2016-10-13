namespace RealEstateBot.Dialogs
{
    using Search.Dialogs;
    using Search.Models;
    using Search.Services;
    using System;

    [Serializable]
    public class RealEstateSearchDialog : SearchDialog
    {
        private static readonly string[] TopRefiners = { "region", "city", "type", "beds", "baths", "price", "daysOnMarket", "sqft" };

        public RealEstateSearchDialog(ISearchClient searchClient) : base(searchClient, multipleSelection: true)
        {
            // TODO: This should really be driven by analyzing the schema
            // Preferred interaction model
            SearchClient.Schema["baths"].FilterPreference = PreferredFilter.MinValue;
            SearchClient.Schema["beds"].FilterPreference = PreferredFilter.MinValue;
            SearchClient.Schema["price"].FilterPreference = PreferredFilter.Range;
            SearchClient.Schema["daysOnMarket"].FilterPreference = PreferredFilter.RangeMax;
            SearchClient.Schema["sqft"].FilterPreference = PreferredFilter.RangeMax;
        }

        protected override string[] GetTopRefiners()
        {
            return TopRefiners;
        }
    }
}

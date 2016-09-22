namespace RealEstateBot.Dialogs
{
    using System;
    using Search.Dialogs;
    using Search.Services;

    [Serializable]
    public class RealEstateSearchDialog : SearchDialog
    {
        private static readonly string[] TopRefiners = { "region", "city", "type" };

        public RealEstateSearchDialog(ISearchClient searchClient) : base(searchClient, multipleSelection: true)
        {
        }

        protected override string[] GetTopRefiners()
        {
            return TopRefiners;
        }
    }
}

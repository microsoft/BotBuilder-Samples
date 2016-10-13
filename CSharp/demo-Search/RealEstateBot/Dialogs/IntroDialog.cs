namespace RealEstateBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Microsoft.Bot.Connector;
    using Search.Models;
    using Search.Services;

    [Serializable]
    public class IntroDialog : IDialog<object>
    {
        private ISearchClient searchClient;

        public IntroDialog(ISearchClient searchClient)
        {
            SetField.NotNull(out this.searchClient, nameof(searchClient), searchClient);
            var schema = searchClient.Schema;

            // This is not needed is you supply the web.config SearchDialogsServiceAdminKey because it will come from the service itself
            if (schema.Count == 0)
            {
                schema.Add("listingId", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = false, IsFilterable = false, IsKey = true, IsRetrievable = true, IsSearchable = false, IsSortable = false, Name = "listingId", Type = typeof(String) });
                schema.Add("beds", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = true, IsFilterable = true, IsKey = false, IsRetrievable = true, IsSearchable = false, IsSortable = true, Name = "beds", Type = typeof(Int32) });
                schema.Add("baths", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = true, IsFilterable = true, IsKey = false, IsRetrievable = true, IsSearchable = false, IsSortable = true, Name = "baths", Type = typeof(Int32) });
                schema.Add("description", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = false, IsFilterable = false, IsKey = false, IsRetrievable = true, IsSearchable = true, IsSortable = false, Name = "description", Type = typeof(String) });
                schema.Add("sqft", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = true, IsFilterable = true, IsKey = false, IsRetrievable = true, IsSearchable = false, IsSortable = true, Name = "sqft", Type = typeof(Int32) });
                schema.Add("daysOnMarket", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = true, IsFilterable = true, IsKey = false, IsRetrievable = true, IsSearchable = false, IsSortable = true, Name = "daysOnMarket", Type = typeof(Int32) });
                schema.Add("status", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = true, IsFilterable = true, IsKey = false, IsRetrievable = true, IsSearchable = true, IsSortable = false, Name = "status", Type = typeof(String) });
                schema.Add("source", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = true, IsFilterable = true, IsKey = false, IsRetrievable = true, IsSearchable = true, IsSortable = false, Name = "source", Type = typeof(String) });
                schema.Add("number", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = false, IsFilterable = false, IsKey = false, IsRetrievable = true, IsSearchable = false, IsSortable = false, Name = "number", Type = typeof(String) });
                schema.Add("street", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = true, IsFilterable = true, IsKey = false, IsRetrievable = true, IsSearchable = true, IsSortable = false, Name = "street", Type = typeof(String) });
                schema.Add("unit", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = false, IsFilterable = false, IsKey = false, IsRetrievable = true, IsSearchable = false, IsSortable = false, Name = "unit", Type = typeof(String) });
                schema.Add("type", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = true, IsFilterable = true, IsKey = false, IsRetrievable = true, IsSearchable = false, IsSortable = false, Name = "type", Type = typeof(String) });
                schema.Add("city", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = true, IsFilterable = true, IsKey = false, IsRetrievable = true, IsSearchable = true, IsSortable = true, Name = "city", Type = typeof(String) });
                schema.Add("cityPhonetic", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = true, IsFilterable = true, IsKey = false, IsRetrievable = true, IsSearchable = true, IsSortable = true, Name = "cityPhonetic", Type = typeof(String) });
                schema.Add("district", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = true, IsFilterable = true, IsKey = false, IsRetrievable = true, IsSearchable = false, IsSortable = true, Name = "district", Type = typeof(String) });
                schema.Add("region", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = true, IsFilterable = true, IsKey = false, IsRetrievable = true, IsSearchable = true, IsSortable = true, Name = "region", Type = typeof(String) });
                schema.Add("zipcode", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = true, IsFilterable = true, IsKey = false, IsRetrievable = true, IsSearchable = true, IsSortable = true, Name = "zipcode", Type = typeof(String) });
                schema.Add("countryCode", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = true, IsFilterable = true, IsKey = false, IsRetrievable = true, IsSearchable = true, IsSortable = true, Name = "countryCode", Type = typeof(String) });
                schema.Add("location", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = false, IsFilterable = true, IsKey = false, IsRetrievable = true, IsSearchable = false, IsSortable = true, Name = "location", Type = typeof(Microsoft.Spatial.GeographyPoint) });
                schema.Add("price", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = true, IsFilterable = true, IsKey = false, IsRetrievable = true, IsSearchable = false, IsSortable = true, Name = "price", Type = typeof(Int64) });
                schema.Add("thumbnail", new SearchField { FilterPreference = PreferredFilter.None, IsFacetable = false, IsFilterable = false, IsKey = false, IsRetrievable = true, IsSearchable = false, IsSortable = false, Name = "thumbnail", Type = typeof(String) });
            }
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.StartSearchDialog);
            return Task.CompletedTask;
        }

        public Task StartSearchDialog(IDialogContext context, IAwaitable<IMessageActivity> input)
        {
            context.Call(new RealEstateSearchDialog(this.searchClient), this.Done);
            return Task.CompletedTask;
        }

        public async Task Done(IDialogContext context, IAwaitable<IList<SearchHit>> input)
        {
            var selection = await input;

            if (selection != null && selection.Any())
            {
                string list = string.Join("\n\n", selection.Select(s => $"* {s.Title} ({s.Key})"));
                await context.PostAsync($"Done! For future reference, you selected these properties:\n\n{list}");
            }

            context.Done<object>(null);
        }
    }
}

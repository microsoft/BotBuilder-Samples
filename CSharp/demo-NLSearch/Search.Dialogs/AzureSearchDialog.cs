using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Search.Services;
using Microsoft.Bot.Builder.Luis;
using Search.Models;
using Search.Azure.Services;
using System.Collections.Concurrent;
using Microsoft.Azure.Search.Models;
using Search.Dialogs.UserInteraction;

namespace Search.Dialogs
{
    [Serializable]
    public abstract class AzureSearchDialog : SearchDialog
    {
        [NonSerialized]
        private static ConcurrentDictionary<AzureSearchConfiguration, AzureSearchClient> _clients = new ConcurrentDictionary<AzureSearchConfiguration, AzureSearchClient>();

        private AzureSearchConfiguration _configuration;

        public AzureSearchDialog(
            AzureSearchConfiguration configuration,
            LuisModelAttribute luis,
            SearchSpec query = null,
            bool multipleSelection = false,
            bool includeCount = true,
            IEnumerable<string> refiners = null
            )
            : base(luis, query, multipleSelection, includeCount, refiners)
        {
            _configuration = configuration;
            if (refiners == null)
            {
                var defaultRefiners = new List<string>();
                foreach (var field in SearchClient.Schema.Fields.Values.OrderBy(f => f.Name))
                {
                    if (field.IsFacetable && field.NameSynonyms.Alternatives.Any())
                    {
                        defaultRefiners.Add(field.Name);
                    }
                }
                defaultRefiners.Add(Resources.ButtonResource(ButtonType.Keyword).Label);
                refiners = defaultRefiners;
            }
            var buttons = new List<Button>();
            var keywordButton = Resources.ButtonResource(ButtonType.Keyword);
            foreach (var refiner in refiners)
            {
                if (refiner == keywordButton.Label)
                {
                    buttons.Add(keywordButton);
                }
                else
                {
                    var field = SearchClient.Schema.Field(refiner);
                    buttons.Add(new Button(field.Description()));
                }
            }
        }

        public override IEnumerable<string> DefaultRefiners
        {
            get
            {
                var defaultRefiners = new List<string>();
                foreach (var field in SearchClient.Schema.Fields.Values.OrderBy(f => f.Name))
                {
                    if (field.IsFacetable && field.NameSynonyms.Alternatives.Any())
                    {
                        defaultRefiners.Add(field.Name);
                    }
                }
                defaultRefiners.Add(Resources.ButtonResource(ButtonType.Keyword).Label);
                return defaultRefiners;
            }
        }

        public override ISearchClient SearchClient
        {
            get
            {
                AzureSearchClient client;
                if (!_clients.TryGetValue(_configuration, out client))
                {
                    client = new AzureSearchClient(_configuration, SearchResultMapper());
                    client = _clients.GetOrAdd(_configuration, client);
                }
                return client;
            }
        }

        public abstract IMapper<DocumentSearchResult, GenericSearchResult> SearchResultMapper();
    }
}

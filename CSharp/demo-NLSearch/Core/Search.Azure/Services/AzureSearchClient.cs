using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Search.Models;
using Search.Services;
using Newtonsoft.Json;

namespace Search.Azure.Services
{
#if !NETSTANDARD1_6
    [Serializable]
#else
    [JsonObject(MemberSerialization.Fields)]
#endif
    public class AzureSearchConfiguration
    {
        public string ServiceName { get; set; }
        public string IndexName { get; set; }
        public string ServiceKey { get; set; }
        public string SchemaPath { get; set; }
    }

    public class AzureSearchClient : ISearchClient
    {
        private readonly IMapper<DocumentSearchResult, GenericSearchResult> mapper;
        private readonly ISearchIndexClient searchClient;

        public AzureSearchClient(AzureSearchConfiguration configuration, IMapper<DocumentSearchResult, GenericSearchResult> mapper)
        {
            this.Schema = SearchSchema.Load(configuration.SchemaPath);
            this.mapper = mapper;
            var client = new SearchServiceClient(configuration.ServiceName, new SearchCredentials(configuration.ServiceKey));
            searchClient = client.Indexes.GetClient(configuration.IndexName);
        }

        public async Task<GenericSearchResult> SearchAsync(SearchSpec spec, string refiner)
        {
            var oldFilter = spec.Filter;
            if (refiner != null && oldFilter != null)
            {
                spec.Filter = spec.Filter.Remove(refiner);
            }
            string search;
            var parameters = BuildSearch(spec, refiner, out search);
            var documentSearchResult = await searchClient.Documents.SearchAsync(search, parameters);
            spec.Filter = oldFilter;
            return mapper.Map(documentSearchResult);
        }

        public SearchSchema Schema { get; } = new SearchSchema();

        // Azure search only supports full text in a seperate tree combined with AND of the filter.
        // We extract all of the FullText operators found in the tree along AND paths only.
        private FilterExpression ExtractFullText(FilterExpression expression, List<FilterExpression> searchExpression)
        {
            FilterExpression filter = null;
            if (expression != null)
            {
                switch (expression.Operator)
                {
                    case FilterOperator.And:
                    {
                        var left = ExtractFullText((FilterExpression) expression.Values[0], searchExpression);
                        var right = ExtractFullText((FilterExpression) expression.Values[1], searchExpression);
                        filter = FilterExpression.Combine(left, right);
                    }
                        break;
                    case FilterOperator.FullText:
                    {
                        searchExpression.Add(expression);
                        filter = null;
                    }
                        break;
                    default:
                        filter = expression;
                        break;
                }
            }
            return filter;
        }

        private string BuildFilter(FilterExpression expression)
        {
            string filter = null;
            if (expression != null)
            {
                string op = null;
                switch (expression.Operator)
                {
                    case FilterOperator.And:
                    {
                        var left = BuildFilter((FilterExpression) expression.Values[0]);
                        var right = BuildFilter((FilterExpression) expression.Values[1]);
                        filter = $"({left}) and ({right})";
                        break;
                    }
                    case FilterOperator.Or:
                    {
                        var left = BuildFilter((FilterExpression) expression.Values[0]);
                        var right = BuildFilter((FilterExpression) expression.Values[1]);
                        filter = $"({left}) or ({right})";
                        break;
                    }
                    case FilterOperator.Not:
                    {
                        var child = BuildFilter((FilterExpression) expression.Values[0]);
                        filter = $"not ({child})";
                        break;
                    }
                    case FilterOperator.FullText:
                        throw new ArgumentException("Cannot handle complex full text expressions.");

                    case FilterOperator.LessThan:
                        op = "lt";
                        break;
                    case FilterOperator.LessThanOrEqual:
                        op = "le";
                        break;
                    case FilterOperator.Equal:
                        op = "eq";
                        break;
                    case FilterOperator.GreaterThanOrEqual:
                        op = "ge";
                        break;
                    case FilterOperator.GreaterThan:
                        op = "gt";
                        break;
                    default:
                        break;
                }
                if (op != null)
                {
                    var field = Schema.Field((string)expression.Values[0]);
                    var value = SearchTools.Constant(expression.Values[1]);
                    if (field.Type == typeof(string[]))
                    {
                        if (expression.Operator != FilterOperator.Equal)
                        {
                            throw new NotSupportedException();
                        }
                        filter = $"{field.Name}/any(z: z eq {value})";
                    }
                    else
                    {
                        filter = $"{field.Name} {op} {value}";
                    }
                }
            }
            return filter;
        }

        private SearchParameters BuildSearch(SearchSpec spec, string facet, out string search)
        {
            var parameters = new SearchParameters
            {
                Top = spec.HitsPerPage,
                Skip = spec.Skip,
                SearchMode = SearchMode.Any,
                IncludeTotalResultCount = spec.GetTotalCount
            };

            if (facet != null)
            {
                parameters.Facets = new List<string> {$"{facet},count:{spec.MaxFacets}"};
            }

            if (Schema.KeywordFields != null)
            {
                parameters.SearchFields = Schema.KeywordFields;
            }

            var searchExpressions = new List<FilterExpression>();
            var filter = ExtractFullText(spec.Filter, searchExpressions);
            parameters.QueryType = QueryType.Full;
            parameters.Filter = BuildFilter(filter);
            search = BuildSearchFilter(spec.Phrases, searchExpressions);
            return parameters;
        }

        private string BuildSearchFilter(IEnumerable<string> phrases, IList<FilterExpression> expressions)
        {
            var builder = new StringBuilder();
            var prefix = "";
            foreach (var phrase in phrases)
            {
                builder.Append(prefix);
                builder.Append(SearchTools.Constant(phrase));
                prefix = " AND ";
            }
            if (expressions.Any())
            {
                builder.Append($"{prefix}(");
                prefix = "";
                foreach (var expression in expressions)
                {
                    var property = (SearchField) expression.Values[0];
                    var value = SearchTools.Constant(expression.Values[1]);
                    builder.Append($"{prefix}{property.Name}:{value}");
                    prefix = " AND ";
                }
                builder.Append(")");
            }
            return builder.ToString();
        }
    }
}
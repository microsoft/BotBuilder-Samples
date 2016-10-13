namespace Search.Azure.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.Search;
    using Microsoft.Azure.Search.Models;
    using Search.Models;
    using Search.Services;
    using Microsoft.Spatial;

    public class AzureSearchClient : ISearchClient
    {
        private readonly ISearchIndexClient searchClient;
        private readonly IMapper<DocumentSearchResult, GenericSearchResult> mapper;
        private IDictionary<string, SearchField> schema = new Dictionary<string, SearchField>();

        public AzureSearchClient(IMapper<DocumentSearchResult, GenericSearchResult> mapper)
        {
            this.mapper = mapper;
            var indexName = ConfigurationManager.AppSettings["SearchDialogsIndexName"];
            var adminKey = ConfigurationManager.AppSettings["SearchDialogsServiceAdminKey"];
            if (adminKey != null)
            {
                var adminClient = new SearchServiceClient(ConfigurationManager.AppSettings["SearchDialogsServiceName"],
                                                                      new SearchCredentials(adminKey));
                AddFields(adminClient.Indexes.Get(indexName).Fields);
            }
            var client = new SearchServiceClient(ConfigurationManager.AppSettings["SearchDialogsServiceName"],
                                                                 new SearchCredentials(ConfigurationManager.AppSettings["SearchDialogsServiceKey"]));
            searchClient = client.Indexes.GetClient(indexName);
        }

        public async Task<GenericSearchResult> SearchAsync(SearchQueryBuilder queryBuilder, string refiner)
        {
            var documentSearchResult = await this.searchClient.Documents.SearchAsync(queryBuilder.SearchText, BuildParameters(queryBuilder, refiner));

            return this.mapper.Map(documentSearchResult);
        }

        private SearchField ToSearchField(Field field)
        {
            Type type;
            if (field.Type == DataType.Boolean) type = typeof(Boolean);
            else if (field.Type == DataType.DateTimeOffset) type = typeof(DateTime);
            else if (field.Type == DataType.Double) type = typeof(double);
            else if (field.Type == DataType.Int32) type = typeof(Int32);
            else if (field.Type == DataType.Int64) type = typeof(Int64);
            else if (field.Type == DataType.String) type = typeof(string);
            else if (field.Type == DataType.GeographyPoint) type = typeof(GeographyPoint);
            else
            {
                throw new ArgumentException($"Cannot map {field.Type} to a C# type");
            }
            return new SearchField()
            {
                Name = field.Name,
                Type = type,
                IsFacetable = field.IsFacetable,
                IsFilterable = field.IsFilterable,
                IsKey = field.IsKey,
                IsRetrievable = field.IsRetrievable,
                IsSearchable = field.IsSearchable,
                IsSortable = field.IsSortable
            };
        }

        public void AddFields(IEnumerable<Field> fields)
        {
            foreach (var field in fields)
            {
                schema[field.Name] = ToSearchField(field);
            }
            TraceFields(schema);
        }

        public static void TraceFields(IDictionary<string, SearchField> schema)
        {
            Func<bool, string> toBool = (val) => val ? "true" : "false";
            foreach (var entry in schema)
            {
                var field = entry.Value;
                System.Diagnostics.Trace.WriteLine($"SearchClient.Schema.Add(\"{entry.Key}\", new SearchField {{FilterPreference=PreferredFilter.{field.FilterPreference}, IsFacetable={toBool(field.IsFacetable)}, IsFilterable={toBool(field.IsFilterable)}, IsKey={toBool(field.IsKey)}, IsRetrievable={toBool(field.IsRetrievable)}, IsSearchable={toBool(field.IsSearchable)}, IsSortable={toBool(field.IsSortable)}, Name=\"{field.Name}\", Type=typeof({field.Type.Name})}});");
            }
        }

        private static string ToFilter(SearchField field, FilterExpression expression)
        {
            string filter = "";
            if (expression.Values.Length > 0)
            {
                var constant = Constant(expression.Values[0]);
                string op = null;
                bool connective = false;
                switch (expression.Operator)
                {
                    case Operator.LessThan: op = "lt"; break;
                    case Operator.LessThanOrEqual: op = "le"; break;
                    case Operator.Equal: op = "eq"; break;
                    case Operator.GreaterThan: op = "gt"; break;
                    case Operator.GreaterThanOrEqual: op = "ge"; break;
                    case Operator.Or: op = "or"; connective = true; break;
                    case Operator.And: op = "and"; connective = true; break;
                }
                if (connective)
                {
                    var builder = new StringBuilder();
                    var seperator = string.Empty;
                    builder.Append('(');
                    foreach (var child in expression.Values)
                    {
                        builder.Append(seperator);
                        builder.Append(ToFilter(field, (FilterExpression)child));
                        seperator = $" {op} ";
                    }
                    builder.Append(')');
                    filter = builder.ToString();
                }
                else
                {
                    filter = $"{field.Name} {op} {constant}";
                }
            }
            return filter;
        }

        public static string Constant(object value)
        {
            string constant = null;
            if (value is string)
            {
                constant = $"'{EscapeFilterString(value as string)}'";
            }
            else
            {
                constant = value.ToString();
            }
            return constant;
        }

        private SearchParameters BuildParameters(SearchQueryBuilder queryBuilder, string facet)
        {
            SearchParameters parameters = new SearchParameters
            {
                Top = queryBuilder.HitsPerPage,
                Skip = queryBuilder.PageNumber * queryBuilder.HitsPerPage,
                SearchMode = SearchMode.All
            };

            if (facet != null)
            {
                parameters.Facets = new List<string> { facet };
            }

            if (queryBuilder.Refinements.Count > 0)
            {
                StringBuilder filter = new StringBuilder();
                string separator = string.Empty;

                foreach (var entry in queryBuilder.Refinements)
                {
                    SearchField field;
                    if (Schema.TryGetValue(entry.Key, out field))
                    {
                        filter.Append(separator);
                        filter.Append(FilterExpression.ToFilter(field, entry.Value));
                        separator = " and ";
                    }
                    else
                    {
                        throw new ArgumentException($"{entry.Key} is not in the schema");
                    }
                }

                parameters.Filter = filter.ToString();
            }

            return parameters;
        }

        private static string EscapeFilterString(string s)
        {
            return s.Replace("'", "''");
        }

        public IDictionary<string, SearchField> Schema
        {
            get
            { return schema; }
        }
    }
}

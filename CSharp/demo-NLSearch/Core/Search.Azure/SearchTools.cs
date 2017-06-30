using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Search.Utilities;

namespace Search.Azure
{
    public static partial class SearchTools
    {
        public static SearchSchema GetIndexSchema(string service, string adminKey, string indexName)
        {
            var schema = new SearchSchema();
            var adminClient = new SearchServiceClient(service, new SearchCredentials(adminKey));
            var fields = adminClient.Indexes.Get(indexName).Fields;
            foreach (var field in fields)
            {
                schema.AddField(ToSearchField(field));
            }
            return schema;
        }

        public static DataType GetDataType(this Type type)
        {
            DataType dt;
            if (type == typeof(Boolean)) dt = DataType.Boolean;
            else if (type == typeof(DateTime)) dt = DataType.DateTimeOffset;
            else if (type == typeof(double)) dt = DataType.Double;
            else if (type == typeof(Int32)) dt = DataType.Int32;
            else if (type == typeof(Int64)) dt = DataType.Int64;
            else if (type == typeof(string)) dt = DataType.String;
            else if (type == typeof(string[])) dt = DataType.Collection(DataType.String);
            else if (type == typeof(GeographyPoint)) dt = DataType.GeographyPoint;
            else
            {
                throw new ArgumentException($"Cannot map {type} to an Azure Search type");
            }
            return dt;
        }

        public static SearchField ToSearchField(Field field)
        {
            Type type;
            if (field.Type == DataType.Boolean) type = typeof(Boolean);
            else if (field.Type == DataType.DateTimeOffset) type = typeof(DateTime);
            else if (field.Type == DataType.Double) type = typeof(double);
            else if (field.Type == DataType.Int32) type = typeof(Int32);
            else if (field.Type == DataType.Int64) type = typeof(Int64);
            else if (field.Type == DataType.String) type = typeof(string);
            // == does not work here
            else if (field.Type.ToString() == DataType.Collection(DataType.String).ToString()) type = typeof(string[]);
            else if (field.Type == DataType.GeographyPoint) type = typeof(GeographyPoint);
            else
            {
                throw new ArgumentException($"Cannot map {field.Type} to a C# type");
            }

#if !NETSTANDARD1_6
            return new SearchField(field.Name, Microsoft.Bot.Builder.FormFlow.Advanced.Language.CamelCase(field.Name))
#else
            return new SearchField(field.Name, Casing.CamelCase(field.Name))
#endif
            {
                Type = type,
                IsFacetable = field.IsFacetable,
                IsFilterable = field.IsFilterable,
                IsKey = field.IsKey,
                IsRetrievable = field.IsRetrievable,
                IsSearchable = field.IsSearchable,
                IsSortable = field.IsSortable,
                FilterPreference = (field.IsFacetable ? PreferredFilter.Facet : PreferredFilter.None)
            };
        }

        public static bool IsNumeric(this Type type)
        {
            return type == typeof(Byte)
                || type == typeof(UInt16)
                || type == typeof(Int16)
                || type == typeof(UInt32)
                || type == typeof(Int32)
                || type == typeof(UInt64)
                || type == typeof(Int64)
                || type == typeof(float)
                || type == typeof(double);
        }

        public static string Constant(object value)
        {
            string constant = null;
            if (value is string)
            {
                var val = (value as string).Replace("'", "''");
                constant = $"'{val}'";
            }
            else if (value is DateTime)
            {
                var val = ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ssZ");
                constant = $"'datetime'{val}";
            }
            else if (value is DateTimeOffset)
            {
                constant = ((DateTimeOffset)value).ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            else
            {
                constant = value.ToString();
            }
            return constant;
        }
    }
}

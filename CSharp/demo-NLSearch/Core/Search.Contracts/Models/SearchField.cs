using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Search.Models
{
    public enum PreferredFilter { None, Facet, MinValue, MaxValue };

#if !NETSTANDARD1_6
    [Serializable]
#else
    [JsonObject(MemberSerialization.OptOut)]
#endif
    public class SearchField
    {
        private sealed class TypeConverter : JsonConverter

        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Type);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var fullTypeName = (string) reader.Value;
                // Use the raw type name to bridge between .net core and normal .net
                var typeName = fullTypeName.Substring(0, fullTypeName.IndexOf(","));
                var type = Type.GetType(typeName);
                if (type == null)
                {
                    type = Type.GetType(fullTypeName);
                }
                return type;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var type = (Type)value;
                writer.WriteValue(type.AssemblyQualifiedName);
            }
        }

        public SearchField(string name, params string[] alternatives)
        {
            Name = name;
            NameSynonyms = new Synonyms(name, alternatives);
        }

        public override string ToString()
        {
            return Name;
        }

        public string Description()
        {
            return NameSynonyms.Alternatives.First();
        }

        public string Name { get; set; }

        [JsonConverter(typeof(TypeConverter))]
        public Type Type { get; set; } = typeof(string);

        public bool IsFacetable { get; set; }
        public bool IsFilterable { get; set; }
        public bool IsKey { get; set; }
        public bool IsMoney { get; set; }
        public bool IsRetrievable { get; set; }
        public bool IsSearchable { get; set; }
        public bool IsSortable { get; set; }

        // Fields to control experience
        public PreferredFilter FilterPreference { get; set; }
        public Synonyms NameSynonyms { get; set; }
        public List<Synonyms> ValueSynonyms { get; set; } = new List<Synonyms>();
        public double Min { get; set; }
        public double Max { get; set; }
        public List<string> Examples { get; set; }
    }
}

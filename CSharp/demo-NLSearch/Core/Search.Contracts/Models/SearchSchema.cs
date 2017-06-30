using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using System.Linq;

namespace Search.Models
{
#if !NETSTANDARD1_6
    [Serializable]
#else
    [JsonObject(MemberSerialization.OptOut)]
#endif
    public class SearchSchema
    {
        private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
        };

        public Dictionary<string, SearchField> Fields { get; set; }

        // Default property to use with currency values
        public string DefaultCurrencyProperty { get; set; }

        // Default property to use with numeric values
        public string DefaultNumericProperty { get; set; }

        public string DefaultGeoProperty { get; set; }

        // Key field for looking up record
        public SearchField Key
        {
            get
            {
                return Fields.Values.First((f) => f.IsKey);
            }
        }

        // Actual keywords seperated by commas
        public string Keywords { get; set; }

        // Where to search for keywords
        public List<string> KeywordFields { get; set; }

        public List<SearchFragment> Fragments = new List<SearchFragment>();

        public SearchSchema()
        {
            Fields = new Dictionary<string, SearchField>();
        }

        public void AddField(SearchField field)
        {
            Fields.Add(field.Name, field);
        }

        public void RemoveField(string name)
        {
            Fields.Remove(name);
        }

        public SearchField Field(string name)
        {
            return Fields[name];
        }

        public void Save(string path)
        {
            using (var output = new StreamWriter(new FileStream(path, FileMode.Create)))
            {
                output.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
            }
        }

        public static SearchSchema Load(string path)
        {
            return JsonConvert.DeserializeObject<SearchSchema>(File.ReadAllText(path));
        }
    }
}
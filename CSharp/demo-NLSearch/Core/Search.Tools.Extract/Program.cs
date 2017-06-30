using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using Search.Azure;
using Search.Models;
using Search.Utilities;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;
using Microsoft.Spatial;

namespace Search.Tools.Extract
{
    internal class Program
    {
        private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
        };

        private static async Task<int> Apply(SearchIndexClient client, string valueField, string idField, string text,
            SearchParameters sp, Func<int, SearchResult, Task> function,
            int max = int.MaxValue,
            int page = 1000)
        {
            var originalFilter = sp.Filter;
            var originalOrder = sp.OrderBy;
            var originalTop = sp.Top;
            var originalSkip = sp.Skip;
            var total = 0;
            object lastValue = null;
            object lastID = null;
            sp.OrderBy = new string[] { valueField };
            sp.Top = page;
            var results = client.Documents.Search(text, sp).Results;
            while (total < max && results.Any())
            {
                var skipping = lastValue != null;
                var newValue = false;
                var row = 0;
                var firstRowWithValue = 0;
                foreach (var result in results)
                {
                    var id = result.Document[idField];
                    if (skipping)
                    {
                        // Skip until we find the last processed id
                        skipping = !id.Equals(lastID);
                    }
                    else
                    {
                        var value = result.Document[valueField];
                        await function(total, result);
                        lastID = id;
                        if (!value.Equals(lastValue))
                        {
                            firstRowWithValue = row;
                            lastValue = value;
                            newValue = true;
                        }
                        if (++total == max)
                        {
                            break;
                        }
                    }
                    ++row;
                }
                if (skipping)
                {
                    throw new Exception($"Could not find id {lastID} in {lastValue}");
                }
                if (row == 1 || total >= max)
                {
                    // Last row in the table
                    break;
                }
                var toSkip = row - firstRowWithValue - 1;
                if (newValue)
                {
                    sp.Skip = toSkip;
                }
                else
                {
                    sp.Skip += toSkip;
                }
                sp.Filter = (originalFilter == null ? "" : $"({originalFilter}) and ") +
                            $"{valueField} ge {SearchTools.Constant(lastValue)}";
                results = client.Documents.Search(text, sp).Results;
            }
            sp.Filter = originalFilter;
            sp.OrderBy = originalOrder;
            sp.Top = originalTop;
            sp.Skip = originalSkip;
            return total;
        }

        private static async Task<int> StreamApply(TextReader stream, Func<int, SearchResult, Task> function, int samples)
        {
            string line;
            int count = 0;
            while ((line = stream.ReadLine()) != null && count < samples)
            {
                var result = new SearchResult();
                result.Document = new Document();
                var doc = JsonConvert.DeserializeObject<Document>(line);
                foreach (var entry in doc)
                {
                    if (entry.Value is JArray)
                    {
                        result.Document[entry.Key] = (from val in (entry.Value as JArray) select (string)val).ToArray<string>();
                    }
                    else
                    {
                        result.Document[entry.Key] = entry.Value;
                    }
                }
                await function(count++, result);
            }
            return count;
        }

        private static async Task Process(int count,
            SearchResult result,
            Parameters parameters,
            Dictionary<string, Histogram<object>> histograms,
            KeywordExtractor extractor,
            TextWriter copyStream)
        {
            var doc = result.Document;
            if (copyStream != null)
            {
                var jsonDoc = JsonConvert.SerializeObject(doc);
                copyStream.WriteLine(jsonDoc);
            }
            if (parameters.AnalyzeFields != null)
            {
                try
                {
                    foreach (var field in parameters.AnalyzeFields)
                    {
                        var value = doc[field];
                        if (value is string[])
                        {
                            foreach (var val in value as string[])
                            {
                                await extractor.AddTextAsync(val);
                            }
                        }
                        else
                        {
                            await extractor.AddTextAsync(value as string);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"\nSuspended extracting analyze text keywords.\n{e.Message}");
                }
            }
            if (parameters.KeywordFields != null)
            {
                foreach (var field in parameters.KeywordFields)
                {
                    var value = doc[field];
                    if (value is string[])
                    {
                        foreach (var val in value as string[])
                        {
                            extractor.AddKeyword(val);
                        }
                    }
                    else
                    {
                        extractor.AddKeyword(value as string);
                    }
                }
            }
            foreach (var field in parameters.Facets)
            {
                var value = doc[field];
                if (value != null)
                {
                    Histogram<object> histogram;
                    if (!histograms.TryGetValue(field, out histogram))
                    {
                        histogram = histograms[field] = new Histogram<object>();
                    }
                    if (value is string[])
                    {
                        foreach (var val in value as string[])
                        {
                            histogram.Add(val);
                        }
                    }
                    else
                    {
                        histogram.Add(value);
                    }
                }
            }
            if (count % 100 == 0)
            {
                Console.Write($"\n{count}: ");
            }
            else
            {
                Console.Write(".");
            }
        }

        private static void Usage(string msg = null)
        {
            if (msg != null)
            {
                Console.WriteLine(msg);
            }
            Console.WriteLine(
                @"extract <Service name> <Index name> <Admin key> [-a <attributeList>] [-ad <domain>] [-af <fieldList>] [-ak <key>] [-al <language>] [-c <file>] [-dc <Field>] [-dk <FieldList>] [-dn <Field>] [-e <examples>] [-f <facetList>] [-g <path>] [-h <path>] [-j <jsonFile>] [-kf <fieldList>] [-km <max>] [-kt <threshold>] [-mo <min>] [-mt <max>] [-o <outputPath>] [-v <field>]");
            Console.WriteLine(
                @"Generate <parameters.IndexName>.json schema file.
If you generate a histogram using -g and -h, it will be used to determine attributes if less than -u unique values are found.
You can find keywords either through -kf for actual keywords or -af to generate keywords using the text analytics cognitive service.");
            Console.WriteLine(
                @"-a <attributeList> : Comma seperated list of field names to generate attributes from.  
    If not specified, all string or string[] will be considered for atttributes.");
            Console.WriteLine(
                @"-ad <domain>: Analyze text domain, westus.api.cognitive.microsoft.com by default.");
            Console.WriteLine(
                @"-af <fieldList> : Comma seperated fields to analyze for keywords.");
            Console.WriteLine(
                @"-ak <key> : Key for calling analyze text cognitive service.");
            Console.WriteLine(
                @"-al <language> : Language to use for keyword analysis, en by default.");
            Console.WriteLine(
                @"-c <file> : Copy search index to local JSON file that can be used via -j instead of talking to Azure Search service.");
            Console.WriteLine(
                @"-dc <Field> : Default currency field.");
            Console.WriteLine(
                @"-dn <Field> : Default numeric field.");
            Console.WriteLine(
                @"-dk <FieldList> : Comma seperated list of fields to search for user keywords.");
            Console.WriteLine(
                @"-e <examples> : Number of most frequent examples to keep, default is 3.");
            Console.WriteLine(
                @"-f <facetList>: Comma seperated list of facet names for histogram.  By default all fields in schema.");
            Console.WriteLine(
                @"-g <path>: Generate a file with histogram information from index.  This can take a long time.");
            Console.WriteLine(
                @"-h <path>: Use histogram to help generate schema.  This can be the just generated histogram.");
            Console.WriteLine(
                @"-j <file> : Apply analysis to JSON file rather than search index.");
            Console.WriteLine(
                @"-kf <fieldList> : Comma seperated fields that contain keywords.");
            Console.WriteLine(
                @"-km <max> : Maximum number of keywords to extract, default is 10,000.");
            Console.WriteLine(
                @"-kt <threshold> : Minimum number of docs required to be a keyword, default is 5.");
            Console.WriteLine(
                @"-mo <min> : Minimum number of occurrences for a value to be an attribute candidate, default is 3.");
            Console.WriteLine(
                @"-mt <max> : Maximum number of attributes to allow, default is 5000 and must be < 20,000.");
            Console.WriteLine(
                @"-o <path>: Where to put generated schema.");
            Console.WriteLine(
                @"-s <samples>: Maximum number of rows to sample from index when doing -g.  All by default.");
            Console.WriteLine(
                @"-v <field>: Field to order by when using -g.  There must be no more than 100,000 rows with the same value.  Will use key field if sortable and filterable.");
            Console.WriteLine(
                @"-w : Create a new index from -j JSON file.");
            Console.WriteLine(
                @"{} can be used to comment out arguments.");
            Environment.Exit(-1);
        }

        private static string NextArg(int i, string[] args)
        {
            string arg = null;
            if (i < args.Length)
            {
                arg = args[i];
            }
            else
            {
                Usage();
            }
            return arg;
        }

        private class Parameters
        {
            public string AdminKey;
            public string AnalyzeKey;
            public string AnalyzeDomain = "westus.api.cognitive.microsoft.com";
            public string[] AnalyzeFields = null;
            public string AnalyzeLanguage = "en";
            public string ApplyPath;
            public string[] Attributes = null;
            public bool CopyJSON = false;
            public string CopyPath;
            public string DefaultCurrencyField;
            public string DefaultNumericField;
            public string[] DefaultKeywordFields = null;
            public int Examples = 3;
            public string[] Facets;
            public string GeneratePath;
            public string HistogramPath;
            public string IndexName;
            public string[] KeywordFields;
            public int KeywordMax = 10000;
            public int KeywordThreshold = 5;
            public int MinAttributeOccurrences = 5;
            public int MaxAttributeValues = 5000;
            public int Samples = int.MaxValue;
            public string SchemaPath;
            public string ServiceName;
            public string Sortable;

            public void Display(TextWriter writer)
            {
                foreach (var field in typeof(Parameters).GetFields())
                {
                    var value = field.GetValue(this);
                    if (value != null)
                    {
                        writer.Write($"{field.Name}:");
                        if (value is IEnumerable && !(value is string))
                        {
                            foreach (var val in value as IEnumerable)
                            {
                                writer.Write($" {val}");
                            }
                            writer.WriteLine();
                        }
                        else
                        {
                            writer.WriteLine($" {value}");
                        }
                    }
                }
            }
        };

        private static async Task WriteDocs(ISearchIndexClient client, List<Document> docs, string key)
        {
            Dictionary<string, Document> keyToDoc = null;
            while (docs.Any())
            {
                try
                {
                    await client.Documents.IndexAsync(IndexBatch.MergeOrUpload<Document>(docs));
                    docs.Clear();
                }
                catch (IndexBatchException ex)
                {
                    if (keyToDoc == null)
                    {
                        foreach (var doc in docs)
                        {
                            keyToDoc[(string)doc[key]] = doc;
                        }
                    }
                    docs = (from result in ex.IndexingResults where !result.Succeeded select keyToDoc[result.Key]).ToList();
                }
            }
        }

        private static async Task CopyService(Parameters parameters)
        {
            if (parameters.ApplyPath == null)
            {
                Usage("-w requries -j to point to the source JSON file.");
            }
            Console.WriteLine($"Creating service {parameters.ServiceName} with index {parameters.IndexName}");
            var reader = new StreamReader(new FileStream(parameters.ApplyPath, FileMode.Open));
            var schema = JsonConvert.DeserializeObject<SearchSchema>(reader.ReadLine());
            reader.ReadLine();
            var client = new SearchServiceClient(parameters.ServiceName, new SearchCredentials(parameters.AdminKey));
            var index = new Index() { Name = parameters.IndexName };
            string keyName = null;
            var fields = new List<Field>();
            foreach (var field in schema.Fields.Values)
            {
                if (field.IsKey)
                {
                    // Ensure keys can be used to enumerate
                    field.IsFilterable = true;
                    field.IsSortable = true;
                }
                var indexField = new Field(field.Name, field.Type.GetDataType())
                {
                    IsFacetable = field.IsFacetable,
                    IsFilterable = field.IsFilterable,
                    IsKey = field.IsKey,
                    IsRetrievable = field.IsRetrievable,
                    IsSearchable = field.IsSearchable,
                    IsSortable = field.IsSortable,
                };
                fields.Add(indexField);
                if (field.IsKey)
                {
                    keyName = field.Name;
                }
            }
            index.Fields = fields.ToArray();
            var indexClient = client.Indexes.GetClient(parameters.IndexName);
            if (!client.Indexes.ListNames().Contains(parameters.IndexName))
            {
                await client.Indexes.CreateAsync(index);
            }

            Console.WriteLine("Copying data");
            int count = 0;
            string line;
            var docs = new List<Document>();
            while ((line = reader.ReadLine()) != null)
            {
                if (count % 100 == 0)
                {
                    Console.Write($"\n{count}: ");
                }
                ++count;
                Console.Write('.');
                var doc = JsonConvert.DeserializeObject<Document>(line);
                var newDoc = new Document();
                // Patch up GeoJSON objects
                foreach (var field in doc)
                {
                    var val = field.Value;
                    if (val != null && val is JObject)
                    {
                        dynamic obj = val as JObject;
                        var coords = CoordinateSystem.Geography(obj.EpsgId as int?);
                        var spatial = GeographyPoint.Create(coords, (double)obj.Latitude, (double)obj.Longitude, (double?)obj.Z, (double?)obj.M);
                        val = spatial;
                    }
                    newDoc[field.Key] = val;
                }
                docs.Add(newDoc);
                if (docs.Count() == 1000)
                {
                    Console.Write(".");
                    await WriteDocs(indexClient, docs, keyName);
                }
            }
            if (docs.Any())
            {
                await WriteDocs(indexClient, docs, keyName);
            }
        }

        private struct Attribute: IEqualityComparer<Attribute>
        {
            public readonly SearchField Field;
            public readonly int Count;
            public readonly string Value;
            public Attribute(SearchField field, int count, string value)
            {
                Field = field;
                Count = count;
                Value = value;
            }

            public bool Equals(Attribute x, Attribute y)
            {
                return x.Value.Equals(y.Value, StringComparison.CurrentCultureIgnoreCase);
            }

            public int GetHashCode(Attribute obj)
            {
                return obj.Value.ToLower().GetHashCode();
            }
        }

        private static async Task MainAsync(Parameters parameters)
        {
            if (parameters.AnalyzeFields == null ? parameters.AnalyzeKey != null : parameters.AnalyzeKey == null)
            {
                Console.WriteLine("In order to analyze keywords you need both -ak and -af parameters.");
            }
            if (parameters.CopyJSON)
            {
                await CopyService(parameters);
            }
            else
            {
                var applyStream = parameters.ApplyPath == null ? null : new StreamReader(new FileStream(parameters.ApplyPath, FileMode.Open));
                SearchSchema schema;
                if (applyStream == null)
                {
                    schema = SearchTools.GetIndexSchema(parameters.ServiceName, parameters.AdminKey, parameters.IndexName);
                }
                else
                {
                    schema = JsonConvert.DeserializeObject<SearchSchema>(applyStream.ReadLine());
                    applyStream.ReadLine();
                }
                if (parameters.DefaultCurrencyField != null)
                {
                    schema.DefaultCurrencyProperty = parameters.DefaultCurrencyField;
                    schema.Field(parameters.DefaultCurrencyField).IsMoney = true;
                }
                if (parameters.DefaultNumericField != null)
                {
                    schema.DefaultNumericProperty = parameters.DefaultNumericField;
                }
                if (parameters.DefaultKeywordFields != null)
                {
                    schema.KeywordFields = parameters.DefaultKeywordFields.ToList();
                }
                if (parameters.GeneratePath != null)
                {
                    if (parameters.Sortable == null)
                    {
                        foreach (var field in schema.Fields.Values)
                        {
                            if (field.IsKey && field.IsSortable && field.IsFilterable)
                            {
                                parameters.Sortable = field.Name;
                            }
                        }
                        if (parameters.Sortable == null)
                        {
                            Usage("You must specify a field with -v.");
                        }
                    }
                    var indexClient = new SearchIndexClient(parameters.ServiceName, parameters.IndexName, new SearchCredentials(parameters.AdminKey));
                    if (parameters.Facets == null)
                    {
                        parameters.Facets = (from field in schema.Fields.Values
                                             where (field.Type == typeof(string) || field.Type == typeof(string[]) || field.Type.IsNumeric()) && field.IsFilterable
                                             select field.Name).ToArray();
                    }
                    var id = schema.Fields.Values.First((f) => f.IsKey);
                    var histograms = new Dictionary<string, Histogram<object>>();
                    var sp = new SearchParameters();
                    var timer = Stopwatch.StartNew();
                    var copyStream = parameters.CopyPath == null ? null : new StreamWriter(new FileStream(parameters.CopyPath, FileMode.Create));
                    if (copyStream != null)
                    {
                        copyStream.WriteLine(JsonConvert.SerializeObject(schema));
                    }
                    var extractor = parameters.AnalyzeKey != null ? new KeywordExtractor(parameters.AnalyzeKey, parameters.AnalyzeLanguage, parameters.AnalyzeDomain) : null;
                    var results = await (applyStream == null
                        ? Apply(indexClient, parameters.Sortable, id.Name, null, sp,
                        (count, result) => Process(count, result, parameters, histograms, extractor, copyStream),
                        parameters.Samples)
                    : StreamApply(applyStream, (count, result) => Process(count, result, parameters, histograms, extractor, null), parameters.Samples));
                    Console.WriteLine($"\nFound {results} in {timer.Elapsed.TotalSeconds}s");
                    if (copyStream != null)
                    {
                        copyStream.Dispose();
                    }
                    if (parameters.AnalyzeFields != null || parameters.KeywordFields != null)
                    {
                        var counts = await extractor.KeywordsAsync();
                        var topN = (from count in counts where count.Value >= parameters.KeywordThreshold orderby count.Value descending select count.Key).Take(parameters.KeywordMax);
                        var sorted = (from keyword in topN orderby keyword ascending select keyword);
                        schema.Keywords = string.Join(",", sorted.ToArray());
                    }
                    using (var stream = new FileStream(parameters.GeneratePath, FileMode.Create))
                    {
#if !NETSTANDARD1_6
                        var serializer = new BinaryFormatter();
                        serializer.Serialize(stream, histograms);
#else
                        var jsonHistograms = JsonConvert.SerializeObject(histograms);
                        stream.Write(Encoding.UTF8.GetBytes(jsonHistograms), 0, Encoding.UTF8.GetByteCount(jsonHistograms));
#endif
                    }
                }
                if (parameters.HistogramPath != null)
                {
                    Dictionary<string, Histogram<object>> histograms;
                    using (var stream = new FileStream(parameters.HistogramPath, FileMode.Open))
                    {
#if !NETSTANDARD1_6
                        var deserializer = new BinaryFormatter();
                        histograms = (Dictionary<string, Histogram<object>>)deserializer.Deserialize(stream);
#else
                        using (TextReader reader = new StreamReader(stream))
                        {
                            var text = reader.ReadToEnd();
                            histograms = JsonConvert.DeserializeObject<Dictionary<string, Histogram<object>>>(text, jsonSettings);
                        }
#endif
                        var attributes = new List<Attribute>();
                        foreach (var histogram in histograms)
                        {
                            var field = schema.Field(histogram.Key);
                            var counts = histogram.Value;
                            if ((parameters.Attributes == null || parameters.Attributes.Contains(field.Name))
                                && counts.Values().FirstOrDefault() != null
                                && (field.Type == typeof(string) || field.Type == typeof(string[])))
                            {
                                foreach (var value in counts.Pairs())
                                {
                                    var canonical = Normalize(value.Key as string);
                                    if (value.Value >= parameters.MinAttributeOccurrences && !string.IsNullOrEmpty(canonical))
                                    {
                                        attributes.Add(new Attribute(field, value.Value, canonical));
                                    }
                                }
                            }
                            if (field.Type.IsNumeric())
                            {
                                double min = double.MaxValue;
                                double max = double.MinValue;
                                foreach (var val in counts.Values())
                                {
                                    var num = double.Parse((string)val);
                                    if (num < min) min = num;
                                    if (num > max) max = num;
                                }
                                field.Min = min;
                                field.Max = max;
                            }
                            if (parameters.Examples > 0)
                            {
                                field.Examples = (from count in counts.Pairs()
                                                  let key = Convert.ToString(count.Key).Trim()
                                                  where key.Length > 0
                                                  orderby count.Value descending
                                                  select key)
                                                  .Take(parameters.Examples)
                                                  .ToList();
                            }
                        }
                        var selected = (from attribute in attributes
                                        orderby attribute.Count descending
                                        select attribute)
                                        .Take(parameters.MaxAttributeValues);
                        var grouped = (from attribute in selected
                                       orderby attribute.Value ascending
                                       group attribute by attribute.Field into afield
                                       select afield);
                        var vals = new List<Synonyms>();
                        foreach(var afield in grouped)
                        {
                            var field = afield.Key;
                            field.ValueSynonyms = new List<Synonyms>();
                            foreach(var attribute in afield)
                            {
                                var synonyms = new Synonyms(attribute.Value, attribute.Value);
                                field.ValueSynonyms.Add(synonyms);
                            }
                        }
                    }
                }
                schema.Save(parameters.SchemaPath);
            }
        }

        private static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Usage();
            }
            var parameters = new Parameters();
            parameters.ServiceName = args[0];
            parameters.IndexName = args[1];
            parameters.AdminKey = args[2];
            parameters.SchemaPath = parameters.IndexName + ".json";
            for (var i = 3; i < args.Length; ++i)
            {
                var arg = args[i];
                if (arg.StartsWith("{"))
                {
                    while (!args[i].EndsWith("}") && ++i < args.Count())
                    {
                    }
                    if (i == args.Count())
                    {
                        break;
                    }
                }
                else
                {
                    switch (arg)
                    {
                        case "-a":
                            parameters.Attributes = NextArg(++i, args).Split(',');
                            break;
                        case "-ad":
                            parameters.AnalyzeDomain = NextArg(++i, args);
                            break;
                        case "-af":
                            parameters.AnalyzeFields = NextArg(++i, args).Split(',');
                            break;
                        case "-ak":
                            parameters.AnalyzeKey = NextArg(++i, args);
                            break;
                        case "-al":
                            parameters.AnalyzeLanguage = NextArg(++i, args);
                            break;
                        case "-am":
                            parameters.KeywordMax = int.Parse(NextArg(++i, args));
                            break;
                        case "-at":
                            parameters.KeywordThreshold = int.Parse(NextArg(++i, args));
                            break;
                        case "-c":
                            parameters.CopyPath = NextArg(++i, args);
                            break;
                        case "-dc":
                            parameters.DefaultCurrencyField = NextArg(++i, args);
                            break;
                        case "-dn":
                            parameters.DefaultNumericField = NextArg(++i, args);
                            break;
                        case "-dk":
                            parameters.DefaultKeywordFields = NextArg(++i, args).Split(',');
                            break;
                        case "-f":
                            parameters.Facets = NextArg(++i, args).Split(',');
                            break;
                        case "-g":
                            parameters.GeneratePath = NextArg(++i, args);
                            break;
                        case "-h":
                            parameters.HistogramPath = NextArg(++i, args);
                            break;
                        case "-j":
                            parameters.ApplyPath = NextArg(++i, args);
                            break;
                        case "-k":
                            parameters.KeywordFields = NextArg(++i, args).Split(',');
                            break;
                        case "-mo":
                            parameters.MinAttributeOccurrences = int.Parse(NextArg(++i, args));
                            break;
                        case "-mt":
                            parameters.MaxAttributeValues = int.Parse(NextArg(++i, args));
                            break;
                        case "-o":
                            parameters.SchemaPath = NextArg(++i, args);
                            break;
                        case "-s":
                            parameters.Samples = int.Parse(NextArg(++i, args));
                            break;
                        case "-v":
                            parameters.Sortable = NextArg(++i, args);
                            break;
                        case "-w":
                            parameters.CopyJSON = true;
                            break;
                        default:
                            Usage($"{arg} is not understood.");
                            break;
                    }
                }
            }
            parameters.Display(Console.Out);
            MainAsync(parameters).Wait();
        }

        public static string Normalize(string input)
        {
            if (input == null)
            {
                return "";
            }
            else
            {
                var start = 0;
                for (; start < input.Length; ++start)
                {
                    var ch = input[start];
                    if (!char.IsPunctuation(ch) && !char.IsWhiteSpace(ch))
                    {
                        break;
                    }
                }
                var end = input.Length;
                for (; end > 0; --end)
                {
                    var ch = input[end - 1];
                    if (!char.IsPunctuation(ch) && !char.IsWhiteSpace(ch))
                    {
                        break;
                    }
                }
                return end > start ? input.Substring(start, end - start) : "";
            }
        }
    }
}
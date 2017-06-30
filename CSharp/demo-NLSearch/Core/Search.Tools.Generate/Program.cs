namespace Search.Generate
{
    using Azure;
    using Microsoft.LUIS.API;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Search.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        private const string stringName = "stringname";
        private const string numberName = "numbername";
        private const string moneyName = "moneyname";
        private const string attributeName = "attributename";
        private const string keyword = "keywordname";
        private const string SubscriptionKey = "LUISSubscriptionKey";

        static void Clear(dynamic model)
        {
            dynamic properties = ClosedList(model, "Properties");
            properties.subLists = new JArray();
            dynamic attributes = ClosedList(model, "Attributes");
            attributes.subLists = new JArray();
        }

        static string Normalize(string word)
        {
            return word.Trim().ToLower();
        }

        static string AddSynonyms(string original, Synonyms synonyms)
        {
            var builder = new StringBuilder(original);
            var prefix = string.IsNullOrEmpty(original) ? "" : ",";
            foreach (var alt in synonyms.Alternatives)
            {
                builder.Append(prefix);
                builder.Append(Normalize(alt));
                prefix = ",";
            }
            return builder.ToString();
        }

        static void AddToClosedList(dynamic model, string closedList, Synonyms synonym)
        {
            var list = ClosedList(model, closedList);
            var sublists = (JArray)list.subLists;
            var sublist = new JObject();
            var words = new JArray();
            sublist.Add("canonicalForm", synonym.Canonical);
            foreach (var word in synonym.Alternatives)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    words.Add(Normalize(word));
                }
            }
            sublist.Add("list", words);
            sublists.Add(sublist);
        }

        public class SubListComparer : IEqualityComparer<JToken>
        {
            public bool Equals(JToken x, JToken y)
            {
                dynamic dx = x;
                dynamic dy = y;
                return ((string)dx.canonicalForm).Equals((string)dy.canonicalForm, StringComparison.CurrentCultureIgnoreCase);
            }

            public int GetHashCode(JToken obj)
            {
                dynamic d = obj;
                return ((string) d.canonicalForm).ToLower().GetHashCode();
            }
        }

        static void SortClosedList(dynamic model, string closedList)
        {
            var list = ClosedList(model, closedList);
            var sublists = (JArray)list.subLists;
            var newlists = new JArray();
            foreach (var sub in (from sub in sublists.Distinct(new SubListComparer()) let word = (string) sub["canonicalForm"] orderby word ascending select sub))
            {
                newlists.Add(sub);
            }
            list.subLists = newlists;
        }

        static void AddDescription(dynamic model, string appName, string[] args)
        {
            model.desc = $"LUIS model generated via the command generate {string.Join(" ", args)}";
            model.name = appName;
        }

        static dynamic Feature(dynamic model, string name)
        {
            dynamic match = null;
            foreach (var feature in model.model_features)
            {
                if (feature.name == name)
                {
                    match = feature;
                    break;
                }
            }
            return match;
        }

        static dynamic ClosedList(dynamic model, string name)
        {
            dynamic match = null;
            foreach (var list in model.closedLists)
            {
                if (list.name == name)
                {
                    match = list;
                    break;
                }
            }
            return match;
        }

        static void AddComparison(dynamic model, SearchField field)
        {
            AddToClosedList(model, "Properties", field.NameSynonyms);
        }

        static void AddNamed(dynamic model, string feature, dynamic entry)
        {
            bool add = true;
            foreach (dynamic val in model[feature])
            {
                if (val.name == entry.name)
                {
                    val.Replace(entry);
                    add = false;
                    break;
                }
            }
            if (add)
            {
                model[feature].Add(entry);
            }
        }

        static void AddUtterances(dynamic model, IEnumerable<string> choices, string intent, string entity)
        {
            var newUtterances = new Dictionary<string, dynamic>();
            var foundUtterance = new Dictionary<string, bool>();
            foreach (var choice in choices)
            {
                var newUtterance = CreateUtterance(choice, intent, entity);
                if (!newUtterances.ContainsKey(choice))
                {
                    // We can get duplicates because of normalization
                    newUtterances.Add(choice, newUtterance);
                    foundUtterance.Add(choice, false);
                }
            }
            var replacements = new List<dynamic>();
            foreach (var child in model.utterances)
            {
                dynamic newUtterance;
                if (newUtterances.TryGetValue((string)child.text, out newUtterance))
                {
                    foundUtterance[(string)newUtterance.text] = true;
                    replacements.Add(child);
                }
            }
            foreach (var replacement in replacements)
            {
                replacement.Replace(newUtterances[(string)replacement.text]);
            }
            foreach (var found in foundUtterance)
            {
                if (!found.Value)
                {
                    model.utterances.Add(newUtterances[found.Key]);
                }
            }
        }

        static dynamic CreateUtterance(string text, string intent, string entity)
        {
            dynamic newUtterance = new JObject();
            var words = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            newUtterance.text = string.Join(" ", text);
            newUtterance.intent = intent;
            var entities = new JArray();
            dynamic attribute = new JObject();
            attribute.entity = entity;
            attribute.startPos = 0;
            attribute.endPos = words.Count() - 1;
            entities.Add(attribute);
            newUtterance.entities = entities;
            return newUtterance;
        }

        static string RandomValueAlternative(SearchField field, Random rand)
        {
            var alt = field.ValueSynonyms[rand.Next(field.ValueSynonyms.Count())];
            return alt.Alternatives[rand.Next(alt.Alternatives.Count())];
        }

        static IEnumerable<string> ValueChoices(SearchField field)
        {
            foreach (var synonym in field.ValueSynonyms)
            {
                foreach (var alt in synonym.Alternatives)
                {
                    yield return alt;
                }
            }
        }

        static void AddAttribute(dynamic model, SearchField field)
        {
            AddToClosedList(model, "Properties", field.NameSynonyms);
            foreach (var synonym in field.ValueSynonyms)
            {
                AddToClosedList(model, "Attributes", synonym);
            }
        }

        static void ExpandFacetExamples(dynamic model)
        {
            var closedList = (JArray)ClosedList(model, "Properties").subLists;
            var facetNames = closedList.SelectMany((dynamic l) => ((JArray)l.list));
            foreach (var utterance in model.utterances)
            {
                if (utterance.intent == "Facet")
                {
                    model.utterances.Remove(utterance);
                    break;
                }
            }
            foreach (var facet in facetNames)
            {
                dynamic utterance = new JObject();
                utterance.text = facet;
                utterance.intent = "Facet";
                utterance.entities = new JArray();
                model.utterances.Add(utterance);
            }
        }

        static Dictionary<int, List<string>> SplitByWords(IEnumerable<string> phrases)
        {
            var result = new Dictionary<int, List<string>>();
            foreach (var phrase in phrases)
            {
                var words = phrase.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var count = words.Length;
                List<string> candidates;
                if (result.TryGetValue(words.Length, out candidates))
                {
                    candidates.Add(phrase);
                }
                else
                {
                    result[count] = new List<string> { phrase };
                }
            }
            return result;
        }

        static bool ReplaceNames(dynamic utterance, Func<string, string> tokenReplacement, out bool failed)
        {
            var text = (string)utterance.text;
            var offset = 0;
            var hasKeyword = false;
            var replacedAll = true;
            utterance.text = PropertyOrAttribute.Replace(text, (match) =>
            {
                var token = match.Value.ToLower();
                var replacement = tokenReplacement(token);
                if (replacement == null)
                {
                    replacedAll = false;
                }
                else
                {
                    if (token == keyword)
                    {
                        hasKeyword = true;
                    }
                    var index = match.Index + offset;
                    var difference = replacement.Length - match.Length;
                    foreach (var entity in utterance.entities)
                    {
                        if (entity.startPos > index)
                        {
                            entity.startPos += difference;
                        }
                        if (entity.endPos >= index)
                        {
                            entity.endPos += difference;
                        }
                    }
                    offset += difference;
                }
                return replacement;
            });
            failed = !replacedAll;
            return hasKeyword;
        }

        static Regex PropertyOrAttribute = new Regex($"{stringName}|{numberName}|{moneyName}|{attributeName}|{keyword}", RegexOptions.IgnoreCase);

        static void ReplaceGenericNames(dynamic model, IEnumerable<SearchField> fields, string keywordList)
        {
            // Use a fixed random sequence to minimize random churn
            var rand = new Random(0);
            var stringNames = (from field in fields where field.Type == typeof(string) || field.Type == typeof(string[]) select field).SelectMany((f) => f.NameSynonyms.Alternatives).ToArray();
            var numberNames = (from field in fields where field.Type.IsNumeric() && !field.IsMoney select field).SelectMany((f) => f.NameSynonyms.Alternatives).ToArray();
            var moneyNames = (from field in fields where field.IsMoney select field).SelectMany((f) => f.NameSynonyms.Alternatives).ToArray();
            var attributes = fields.SelectMany((f) => f.ValueSynonyms.SelectMany((v) => v.Alternatives)).ToArray();
            var allKeywords = SplitByWords(keywordList.Split(',').Select(w => w.Trim()));
            var keywords = allKeywords.First().Value.ToArray();
            var toRemove = new List<dynamic>();
            var toAdd = new List<dynamic>();
            foreach (var utterance in model.utterances)
            {
                var original = new JObject(utterance);
                Func<string, string> replaceToken = (token) =>
                {
                    string[] choices = null;
                    if (token == stringName)
                    {
                        choices = stringNames;
                    }
                    else if (token == numberName)
                    {
                        choices = numberNames;
                    }
                    else if (token == moneyName)
                    {
                        choices = moneyNames;
                    }
                    else if (token == attributeName)
                    {
                        choices = attributes;
                    }
                    else if (token == keyword)
                    {
                        choices = keywords;
                    }
                    return choices == null ? null : choices[rand.Next(choices.Length)];
                };
                bool failed;
                bool hasKeyword = ReplaceNames(utterance, replaceToken, out failed);
                if (failed)
                {
                    toRemove.Add(utterance);
                }
                if (hasKeyword)
                {
                    toAdd.Add(original);
                }
            }
            foreach (var failed in toRemove)
            {
                model.utterances.Remove(failed);
            }
            foreach (var vals in allKeywords.Values.Skip(1))
            {
                var keywordChoices = vals.ToArray();
                Func<string, string> replaceToken = (token) =>
                {
                    string[] choices = null;
                    if (token == stringName)
                    {
                        choices = stringNames;
                    }
                    else if (token == attributeName)
                    {
                        choices = attributes;
                    }
                    else if (token == keyword)
                    {
                        choices = keywordChoices;
                    }
                    return choices == null ? null : choices[rand.Next(choices.Length)];
                };
                foreach (var add in toAdd)
                {
                    bool failed = false;
                    var copy = new JObject(add);
                    ReplaceNames(copy, replaceToken, out failed);
                    if (!failed)
                    {
                        model.utterances.Add(copy);
                    }
                }
            }
        }

        static void AddKeywords(dynamic model, string keywords)
        {
            var feature = Feature(model, "Keywords");
            feature.words = keywords;
        }

        // This is a utility to allow replacing names in utterances programatically
        // Mapper goes from utterance to a token mapper
        static void ReplaceTokens(dynamic model, Func<string, Func<string, string>> mapper, string outputPath)
        {
            foreach (var utterance in model.utterances)
            {
                var str = (string)utterance.text;
                var tokenTransformer = mapper(str);
                bool failed = false;
                ReplaceNames(utterance, tokenTransformer, out failed);
                if (failed)
                {
                    throw new Exception($"Failed transforming {str}.");
                }
            }
            using (var stream = new StreamWriter(new FileStream(outputPath, FileMode.Create)))
            {
                stream.Write(JsonConvert.SerializeObject(model, Formatting.Indented));
            }
        }

        static async Task MainAsync(string[] args, Parameters p)
        {
            var cts = new CancellationTokenSource();
            System.Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            var subscription = new Subscription(p.Domain, p.SubscriptionKey, basicAuth: p.BasicAuth);
            var schema = SearchSchema.Load(p.SchemaPath);
            dynamic template;
            if (p.TemplatePath != null)
            {
                Console.WriteLine($"Reading template from {p.TemplatePath}");
                template = JObject.Parse(File.ReadAllText(p.TemplatePath));
                if (p.UploadTemplate)
                {
                    Console.WriteLine($"Uploading template {template.name} to LUIS");
                    await subscription.ReplaceApplicationAsync(template, cts.Token);
                }
            }
            else
            {
                Console.WriteLine($"Downloading {p.TemplateName} template from LUIS");
                var app = await subscription.GetApplicationByNameAsync(p.TemplateName, cts.Token);
                template = app != null ? await app.DownloadAsync(cts.Token) : null;
                if (template == null)
                {
                    Usage($"Could not download {p.TemplateName} from LUIS.");
                }
                if (p.OutputTemplate != null)
                {
                    Console.WriteLine($"Writing template to {p.OutputTemplate}");
                    using (var stream = new StreamWriter(new FileStream(p.OutputTemplate, FileMode.Create)))
                    {
                        stream.Write(JsonConvert.SerializeObject(template, Formatting.Indented));
                    }
                }
            }

            Console.WriteLine($"Generating {p.OutputName} from schema {p.SchemaPath}");
            /* This is an example of programatically changing the template with entity annotations being updated
            Func<string, Func<string, string>> mapper =
                (utterance) =>
                {
                    return (token) =>
                        {
                            var result = token;
                            if (token == "propertyname")
                            {
                                result = "stringname";
                            }
                            return result;
                        };
                };
            ReplaceTokens(template, mapper, "SearchTemplate-new.json");
            System.Environment.Exit(0);
            */
            Clear(template);
            AddDescription(template, p.OutputName, args);
            bool usesNumbers = false;
            foreach (var field in schema.Fields.Values)
            {
                if ((field.Type == typeof(string)
                    || field.Type == typeof(string[]))
                    && field.ValueSynonyms.Any())
                {
                    AddAttribute(template, field);
                }
                else if (field.Type.IsNumeric() && field.IsFilterable)
                {
                    usesNumbers = true;
                    AddComparison(template, field);
                }
                else if ((field.IsFacetable || field.IsFilterable)
                        && (field.Type == typeof(string)
                            || field.Type == typeof(string[])))
                {
                    AddComparison(template, field);
                }
            }
            var bing = (JArray)template.bing_entities;
            bing.Clear();
            if (usesNumbers)
            {
                bing.Add("number");
            }
            if (schema.DefaultCurrencyProperty != null)
            {
                bing.Add("money");
            }

            AddKeywords(template, schema.Keywords);
            ReplaceGenericNames(template, schema.Fields.Values, schema.Keywords);
            ExpandFacetExamples(template);
            SortClosedList(template, "Attributes");

            if (p.OutputPath != null)
            {
                Console.WriteLine($"Writing generated model to {p.OutputPath}");
                using (var stream = new StreamWriter(new FileStream(p.OutputPath, FileMode.Create)))
                {
                    stream.Write(JsonConvert.SerializeObject(template, Formatting.Indented));
                }
            }

            if (p.Upload)
            {
                Console.WriteLine($"Uploading {p.OutputName} to LUIS");
                var app = await subscription.ReplaceApplicationAsync(template, cts.Token, p.SpellingKey);
                if (app == null)
                {
                    Console.WriteLine($"Could not upload, train or publish {p.OutputName} to LUIS.");
                }
                else
                {
                    Console.WriteLine($"New LUIS app key is {app.ApplicationID}");
                }
            }
        }

        static void Usage(string msg = null)
        {
            if (msg != null)
            {
                Console.WriteLine(msg);
            }
            Console.WriteLine("generate <schemaFile> [-l <LUIS subscription key>] [-m <modelName>] [-o <outputFile>] [-ot <outputTemplate>] [-tf <templateFile>] [-tm <modelName>] [-u] [-ut]");
            Console.WriteLine("Take a JSON schema file and use it to generate a LUIS model from a template.");
            Console.WriteLine("The template can be the included SearchTemplate.json file or can be downloaded from LUIS.");
            Console.WriteLine("The resulting LUIS model can be saved as a file or automatically uploaded to LUIS.");
            Console.WriteLine("-d <LUIS Domain> : LUIS domain which defaults to westus.api.cognitive.microsoft.com.");
            Console.WriteLine($"-l <LUIS subscription key> : LUIS subscription key, default is environment variable from {SubscriptionKey}.");
            Console.WriteLine("-m <modelName> : Output LUIS model name.  By default will be <schemaFileName>Model.");
            Console.WriteLine("-o <outputFile> : Output LUIS JSON file to generate. By default this will be <schemaFileName>Model.json in the same directory as <schemaFile>.");
            Console.WriteLine("-ot <outputFile> : Output template to <outputFile>.");
            Console.WriteLine("-p <userName:password> : Use basic auth--only useful for LUIS internal.");
            Console.WriteLine("-s <spelling API Key> : Enable spelling using supplied API key.");
            Console.WriteLine("-tf <templateFile> : LUIS Template file to modify based on schema.  By default this is SearchTemplate.json.");
            Console.WriteLine("-tm <modelName> : LUIS model to use as template. Must also specify -l.");
            Console.WriteLine("-u: Upload resulting model to LUIS.  Must also specify -l.");
            Console.WriteLine("-ut: Upload template to LUIS.  Must also specify -l.");
            Console.WriteLine("Use {} to comment out arguments.");
            Console.WriteLine("Common usage:");
            Console.WriteLine("generate <schema> : Generate <schema>Model.json in the directory with <schema> from the SearchTemplate.json.");
            Console.WriteLine("generate <schema> -l <LUIS key> -u : Update the existing <schemaFileName>Model LUIS model and upload it to LUIS.");
            System.Environment.Exit(-1);
        }

        static string NextArg(int i, string[] args)
        {
            string arg = null;
            if (i < args.Length && !args[i].StartsWith("-"))
            {
                arg = args[i];
            }
            else
            {
                Usage();
            }
            return arg;
        }

        public class Parameters
        {
            public Parameters(string schemaPath)
            {
                SchemaPath = schemaPath;
                OutputName = Path.GetFileNameWithoutExtension(schemaPath) + "Model";
            }

            public string BasicAuth = null;
            public string Domain = "westus.api.cognitive.microsoft.com";
            public string OutputPath;
            public string OutputName;
            public string OutputTemplate;
            public string SchemaPath;
            public string SpellingKey;
            public string SubscriptionKey;
            public string TemplatePath;
            public string TemplateName;
            public bool Upload = false;
            public bool UploadTemplate = false;
        }

        static void Main(string[] args)
        {
            if (args.Count() < 1)
            {
                Usage();
            }
            var p = new Parameters(args[0]);
            // For local debugging of the sample without checking in your key
            p.SubscriptionKey = Environment.GetEnvironmentVariable(SubscriptionKey);
            for (var i = 1; i < args.Count(); ++i)
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
                    switch (arg.Trim().ToLower())
                    {
                        case "-d": p.Domain = NextArg(++i, args); break;
                        case "-l": p.SubscriptionKey = NextArg(++i, args); break;
                        case "-m": p.OutputName = NextArg(++i, args); break;
                        case "-o": p.OutputPath = NextArg(++i, args); break;
                        case "-ot": p.OutputTemplate = NextArg(++i, args); break;
                        case "-p": p.BasicAuth = NextArg(++i, args); break;
                        case "-s": p.SpellingKey = NextArg(++i, args); break;
                        case "-tf": p.TemplatePath = NextArg(++i, args); break;
                        case "-tm": p.TemplateName = NextArg(++i, args); break;
                        case "-u": p.Upload = true; break;
                        case "-ut": p.UploadTemplate = true; break;
                        default: Usage($"Unknown parameter {arg}"); break;
                    }
                }
            }
            if ((p.Upload || p.TemplateName != null || p.UploadTemplate) && p.SubscriptionKey == null)
            {
                Usage($"You must supply your LUIS subscription key with -l or through the environment variable {SubscriptionKey}.");
            }

            if (p.TemplateName == null && p.TemplatePath == null)
            {
                p.TemplatePath = "SearchTemplate.json";
            }
            else if (p.TemplateName != null && p.TemplatePath != null)
            {
                Usage("Can only specify either template file or LUIS model name.");
            }
            if (!p.Upload && p.OutputPath == null)
            {
                p.OutputPath = Path.Combine(Path.GetDirectoryName(p.SchemaPath), Path.GetFileNameWithoutExtension(p.SchemaPath) + "Model.json");
            }
            MainAsync(args, p).Wait();
        }
    }
}

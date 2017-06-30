using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Search.Tools.Extract
{
    public class KeywordExtractor
    {
        // NOTE: Could not use official SDK because it is not compatible with .NET core
        private const int MaxDoc = 10000;
        private const int MaxDocs = 1000;
        private const int MaxLength = 1024 * 1024;
        private string _key;
        private string _language;
        private string _domain;
        private CancellationToken _cancellation;
        private HttpClient _client;
        private bool _suspend = false;
        private int _sizeSoFar = 0;
        private JArray _documents = new JArray();
        public Dictionary<string, int> _counts = new Dictionary<string, int>();

        public KeywordExtractor(string key, string language="en", string domain= "westus.api.cognitive.microsoft.com", CancellationToken ct = default(CancellationToken))
        {
            _key = key;
            _language = language;
            _domain = domain;
            _cancellation = ct;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _key);
        }

        public async Task AddTextAsync(string text)
        {
            if (!_suspend)
            {
                if (text.Length > MaxDoc)
                {
                    // Truncate to maximum length on word boundary
                    text = text.Substring(0, MaxDoc);
                    text = text.Substring(0, text.LastIndexOf(' '));
                }
                await AddDocAsync(text);
            }
        }

        public async Task<IReadOnlyDictionary<string, int>> KeywordsAsync()
        {
            if (!_suspend)
            {
                await GetKeywordsAsync();
            }
            return _counts;
        }

        public void AddKeyword(string keyword)
        {
            int count;
            if (_counts.TryGetValue(keyword, out count))
            {
                ++_counts[keyword];
            }
            else
            {
                _counts[keyword] = 1;
            }
        }

        private async Task GetKeywordsAsync()
        {
            if (_documents.Count > 0)
            {
                var documents = new JObject();
                documents.Add("documents", _documents);
                _documents = new JArray();
                _sizeSoFar = 0;
                var uri = new Uri($"https://{_domain}/text/analytics/v2.0/keyPhrases");
                HttpResponseMessage response;
                var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(documents));
                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    response = await _client.PostAsync(uri, content, _cancellation);
                }
                if (response.IsSuccessStatusCode)
                {
                    dynamic keyphrases = JObject.Parse(await response.Content.ReadAsStringAsync());
                    if (((JArray)keyphrases.errors).Count > 0)
                    {
                        var builder = new StringBuilder();
                        builder.AppendLine("Errors while looking for key phrases");
                        foreach (dynamic error in (JArray)keyphrases.errors)
                        {
                            builder.Append(error.id);
                            builder.Append(": ");
                            builder.Append(error.message);
                            builder.AppendLine();
                        }
                        _suspend = true;
                        throw new Exception(builder.ToString());
                    }
                    else
                    {
                        foreach (dynamic doc in (JArray)keyphrases.documents)
                        {
                            var phrases = new HashSet<string>();
                            foreach (string phrase in doc.keyPhrases)
                            {
                                phrases.Add(phrase);
                            }
                            foreach(var phrase in phrases)
                            {
                                AddKeyword(phrase);
                            }
                        }
                    }
                }
                else
                {
                    _suspend = true;
                    throw new Exception($"Could not get key phrases: {response.ReasonPhrase}");
                }
            }
        }

        private async Task AddDocAsync(string text)
        {
            var body = $@"{{""language"":""{ _language}"", ""id"":""{_documents.Count}"", ""text"":""{text}""}}";
            var document = JObject.Parse(body);
            _sizeSoFar += body.Length;
            if (_sizeSoFar < MaxLength)
            {
                _documents.Add(document);
                if (_documents.Count == MaxDocs)
                {
                    await GetKeywordsAsync();
                }
            }
            else
            {
                await GetKeywordsAsync();
                _documents.Add(document);
                _sizeSoFar = body.Length;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Search.Utilities
{
    public class Translation
    {
        public string SourceLanguage;
        public string[] Translations;
    }

    [Serializable]
    public class Translator
    {
        const string BASE = "https://api.microsofttranslator.com/v2/Http.svc/";
        string _key;

        public Translator(string key)
        {
            _key = key;
        }

        public string Detect(string text)
        {
            string locale = null;
            var uri = $"{BASE}/Detect?text={HttpUtility.UrlEncode(text)}";
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _key);
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                var dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));
                locale = (string)dcs.ReadObject(stream);
            }
            return locale;
        }

        public async Task<Translation> Translate(string from, string to, params string[] texts)
        {
            var result = new Translation();
            if (_key == null || from == to)
            {
                result.SourceLanguage = from;
                result.Translations = texts;
            }
            else
            {
                var uri = $"{BASE}/TranslateArray";
                var strings = string.Join("\n", texts.Select((t) =>
                    {
                        var text = t
                            .Replace("<literal>", "<literal translate=\"no\">");
                        return $@"<string xmlns=""http://schemas.microsoft.com/2003/10/Serialization/Arrays"">{System.Web.HttpUtility.HtmlEncode(text)}</string>";
                    }));
                var body =
                    $@"<TranslateArrayRequest>
                     <AppId />
                     <From>{from}</From>
                     <Options>
                       <Category xmlns=""http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2"">generalnn</Category>
                       <ContentType>text/html</ContentType>
                     </Options>
                     <Texts>{strings}</Texts>
                     <To>{to}</To>
                   </TranslateArrayRequest>";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(uri);
                    request.Content = new StringContent(body, Encoding.UTF8, "text/xml");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", _key);
                    var response = await client.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            var doc = XDocument.Parse(responseBody);
                            var ns = XNamespace.Get("http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2");
                            var i = 0;
                            result.Translations = new string[texts.Length];
                            foreach (var xe in doc.Descendants(ns + "TranslateArrayResponse"))
                            {
                                var text = xe.Elements(ns + "TranslatedText").First().Value;
                                result.Translations[i++] = Regex.Replace(text, "<literal translate=\"no\">(.*)</literal>", "$1");
                            }
                            // TODO: What if there is more than one language?
                            result.SourceLanguage = doc.Descendants(ns + "From").First().Value;
                            break;

                        default:
                            throw new Exception($"Could not translate {body}: {responseBody}");
                    }
                }
            }
            return result;
        }
    }
}

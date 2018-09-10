// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MultiLingualBot.Translation.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MultiLingualBot.Translation
{
    public interface ITranslator
    {
        Task<string> TranslateAsync(string text, string targetLocale);
    }

    public class MicrosoftTranslator : ITranslator
    {
        private const string host = "https://api.cognitive.microsofttranslator.com";
        private const string path = "/translate?api-version=3.0";
        private const string uriParams = "&to=";

        private readonly string _key = "ENTER KEY HERE";

        public MicrosoftTranslator(string key)
        {
            _key = key;
        }

        public async Task<string> TranslateAsync(string text, string targetLocale)
        {
            // From Cognitive Services translation documentation:
            // https://docs.microsoft.com/en-us/azure/cognitive-services/translator/quickstart-csharp-translate
            var body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                var uri = host + path + uriParams + targetLocale;
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", _key);

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TranslatorResponse[]>(responseBody);

                return result?.FirstOrDefault()?.Translations?.FirstOrDefault()?.Text;
            }
        }
    }
}

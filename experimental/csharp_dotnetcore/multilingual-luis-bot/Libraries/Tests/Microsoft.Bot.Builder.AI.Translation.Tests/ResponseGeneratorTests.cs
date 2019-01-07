using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using Microsoft.Bot.Builder.AI.Translation.Model;
using Microsoft.Bot.Builder.AI.Translation.ResponseGenerator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Text;

namespace Microsoft.Bot.Builder.AI.Translation.Tests
{
    [TestClass]
    public class ResponseGeneratorTests
    {

        private const string _apiKey = "<dummy-key>";
        private const string DetectUrl = "https://api.cognitive.microsofttranslator.com/detect?api-version=3.0";
        private const string TranslateUrl = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&includeAlignment=true&includeSentenceLength=true";

        [TestMethod]
        [TestCategory("ResponseGenerator")]
        public void DetectRequest_DetectFrench()
        {
            IResponseGenerator responseGenerator = new TranslatorResponseGenerator();
            var requestModels = new List<TranslatorRequestModel>
            {
                new TranslatorRequestModel()
                {
                    Text = "C'est un exemple français."
                }
            };
            var detectRequest = GetDetectRequestMessage(requestModels);
            var response = responseGenerator.GenerateDetectResponseAsync(detectRequest).GetAwaiter().GetResult();
            var lang = response.FirstOrDefault().Language;
            Assert.AreEqual("fr", lang);
        }

        [TestMethod]
        [TestCategory("ResponseGenerator")]
        public void DetectRequest_DetectEnglish()
        {
            IResponseGenerator responseGenerator = new TranslatorResponseGenerator();
            var requestModels = new List<TranslatorRequestModel>
            {
                new TranslatorRequestModel()
                {
                    Text = "This is an English example."
                }
            };
            var detectRequest = GetDetectRequestMessage(requestModels);
            var response = responseGenerator.GenerateDetectResponseAsync(detectRequest).GetAwaiter().GetResult();
            var lang = response.FirstOrDefault().Language;
            Assert.AreEqual("en", lang);
        }

        [TestMethod]
        [TestCategory("ResponseGenerator")]
        public void TranslateRequest_TranslateFrenchToEnglish()
        {
            IResponseGenerator responseGenerator = new TranslatorResponseGenerator();
            var requestModels = new List<TranslatorRequestModel>
            {
                new TranslatorRequestModel()
                {
                    Text = "C'est un exemple français."
                }
            };
            string from = "fr", to = "en";
            var detectRequest = GetTranslateRequestMessage(from, to, requestModels);
            var response = responseGenerator.GenerateTranslateResponseAsync(detectRequest).GetAwaiter().GetResult();
            var lang = response.FirstOrDefault();
            Assert.AreEqual("It's a French example.", lang.Translations.FirstOrDefault().Text);
        }

        [TestMethod]
        [TestCategory("ResponseGenerator")]
        public void TranslateRequest_TranslateEnglishToFrench()
        {
            IResponseGenerator responseGenerator = new TranslatorResponseGenerator();
            List<TranslatorRequestModel> requestModels = new List<TranslatorRequestModel>
            {
                new TranslatorRequestModel()
                {
                    Text = "This is an English example."
                }
            };
            string from = "en", to = "fr";
            var detectRequest = GetTranslateRequestMessage(from, to, requestModels);
            var response = responseGenerator.GenerateTranslateResponseAsync(detectRequest).GetAwaiter().GetResult();
            var lang = response.FirstOrDefault();
            Assert.AreEqual("C'est un exemple anglais.", lang.Translations.FirstOrDefault().Text);
        }

        private HttpRequestMessage GetTranslateRequestMessage(string from, string to, IEnumerable<TranslatorRequestModel> translatorRequests)
        {
            if (translatorRequests == null || translatorRequests.ToList().Count < 1)
            {
                throw new ArgumentNullException(nameof(translatorRequests));
            }

            var query = $"&from={from}&to={to}";
            var requestUri = new Uri(TranslateUrl + query);
            return GetRequestMessage(requestUri, translatorRequests);
        }

        private HttpRequestMessage GetDetectRequestMessage(IEnumerable<TranslatorRequestModel> translatorRequests)
        {
            if (translatorRequests == null || translatorRequests.ToList().Count < 1)
            {
                throw new ArgumentNullException(nameof(translatorRequests));
            }
            var requestUri = new Uri(DetectUrl);
            return GetRequestMessage(requestUri, translatorRequests);
        }

        private HttpRequestMessage GetRequestMessage(Uri requestUri, IEnumerable<TranslatorRequestModel> translatorRequests)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _apiKey);
            request.Content = new StringContent(JsonConvert.SerializeObject(translatorRequests), Encoding.UTF8, "application/json");
            return request;
        }
    }
}

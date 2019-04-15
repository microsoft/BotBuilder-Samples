using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using Microsoft.Bot.Builder.AI.Translation.Model;
using Microsoft.Bot.Builder.AI.Translation.RequestBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Text;

namespace Microsoft.Bot.Builder.AI.Translation.Tests
{
    [TestClass]
    public class RequestBuilderTests
    {

        private const string _apiKey = "dummy-key";
        private const string DetectUrl = "https://api.cognitive.microsofttranslator.com/detect?api-version=3.0";
        private const string TranslateUrl = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&includeAlignment=true&includeSentenceLength=true";

        [TestMethod]
        [TestCategory("RequestBuilder")]
        public void InvalidArguments_NullApiKey()
        {
            string apiKey = null;
            Assert.ThrowsException<ArgumentNullException>(() => new TranslatorRequestBuilder(apiKey));
        }

        [TestMethod]
        [TestCategory("RequestBuilder")]
        public void InvalidArguments_EmptyApiKey()
        {
            var apiKey = "";
            Assert.ThrowsException<ArgumentNullException>(() => new TranslatorRequestBuilder(apiKey));
        }

        [TestMethod]
        [TestCategory("RequestBuilder")]
        public void DetectRequest_ValidRequest()
        {
            IRequestBuilder requestBuilder = new TranslatorRequestBuilder(_apiKey);
            var requestModels = new List<TranslatorRequestModel>();
            requestModels.Add(new TranslatorRequestModel()
            {
                Text = "C'est un exemple français."
            });
            var request = requestBuilder.BuildDetectRequest(requestModels);
            Assert.IsTrue(request.Headers.Contains("Ocp-Apim-Subscription-Key"));
            Assert.AreEqual(_apiKey, request.Headers.GetValues("Ocp-Apim-Subscription-Key").FirstOrDefault());

            Assert.AreEqual(DetectUrl, request.RequestUri.ToString());
            Assert.AreEqual(request.Content.ToString(), new StringContent(JsonConvert.SerializeObject(requestModels), Encoding.UTF8, "application/json").ToString());
        }

        [TestMethod]
        [TestCategory("RequestBuilder")]
        public void DetectRequest_NullTranslatorModel()
        {
            IRequestBuilder requestBuilder = new TranslatorRequestBuilder(_apiKey);
            List<TranslatorRequestModel> requestModels = null;
            Assert.ThrowsException<ArgumentNullException>(() => requestBuilder.BuildDetectRequest(requestModels));
        }

        [TestMethod]
        [TestCategory("RequestBuilder")]
        public void DetectRequest_EmptyTranslatorModel()
        {
            IRequestBuilder requestBuilder = new TranslatorRequestBuilder(_apiKey);
            var requestModels = new List<TranslatorRequestModel>();
            Assert.ThrowsException<ArgumentNullException>(() => requestBuilder.BuildDetectRequest(requestModels));
        }


        [TestMethod]
        [TestCategory("RequestBuilder")]
        public void TranslateRequest_ValidRequest()
        {
            IRequestBuilder requestBuilder = new TranslatorRequestBuilder(_apiKey);
            var requestModels = new List<TranslatorRequestModel>();
            requestModels.Add(new TranslatorRequestModel()
            {
                Text = "C'est un exemple français."
            });
            string from = "fr", to = "en";
            var request = requestBuilder.BuildTranslateRequest(from, to, requestModels);
            Assert.IsTrue(request.Headers.Contains("Ocp-Apim-Subscription-Key"));
            Assert.AreEqual(_apiKey, request.Headers.GetValues("Ocp-Apim-Subscription-Key").FirstOrDefault());

            Assert.AreEqual(TranslateUrl + $"&from={from}&to={to}", request.RequestUri.ToString());
            Assert.AreEqual(request.Content.ToString(), new StringContent(JsonConvert.SerializeObject(requestModels), Encoding.UTF8, "application/json").ToString());
        }

        [TestMethod]
        [TestCategory("RequestBuilder")]
        public void TranslateRequest_NullTranslatorModel()
        {
            IRequestBuilder requestBuilder = new TranslatorRequestBuilder(_apiKey);
            List<TranslatorRequestModel> requestModels = null;
            Assert.ThrowsException<ArgumentNullException>(() => requestBuilder.BuildTranslateRequest("en", "fr", requestModels));
        }

        [TestMethod]
        [TestCategory("RequestBuilder")]
        public void TranslateRequest_EmptyTranslatorModel()
        {
            IRequestBuilder requestBuilder = new TranslatorRequestBuilder(_apiKey);
            var requestModels = new List<TranslatorRequestModel>();
            Assert.ThrowsException<ArgumentNullException>(() => requestBuilder.BuildTranslateRequest("fr", "en", requestModels));
        }
    }
}

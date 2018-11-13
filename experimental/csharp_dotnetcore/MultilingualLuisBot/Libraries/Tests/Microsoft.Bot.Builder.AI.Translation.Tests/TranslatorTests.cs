// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.Translation.PostProcessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace Microsoft.Bot.Builder.AI.Translation.Tests
{
    [TestClass]
    public class TranslatorTests
    {
        private const string _translatorKey = "dummy-key";

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void Translator_InvalidArguments_NullTranslatorKey()
        {
            string translatorKey = null;
            Assert.ThrowsException<ArgumentNullException>(() => new Translator(translatorKey));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void Translator_InvalidArguments_EmptyTranslatorKey()
        {
            var translatorKey = "";
            Assert.ThrowsException<ArgumentNullException>(() => new Translator(translatorKey));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task Translator_DetectAndTranslateToEnglish()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, GetDetectUri())
                .Respond("application/json", GetResponse("Translator_DetectAndTranslateToEnglish_Detect.json"));
            mockHttp.When(HttpMethod.Post, GetTranslateUri("fr", "en"))
                .Respond("application/json", GetResponse("Translator_DetectAndTranslateToEnglish_Translate.json"));

            var translator = new Translator(_translatorKey, mockHttp.ToHttpClient());

            var sentence = "salut";
            var detectedLanguage = await translator.DetectAsync(sentence);
            Assert.IsNotNull(detectedLanguage);
            Assert.AreEqual("fr", detectedLanguage, "should detect french language");

            var translatedSentence = await translator.TranslateAsync(sentence, detectedLanguage, "en");
            Assert.IsNotNull(translatedSentence);
            Assert.AreEqual("Hello", translatedSentence.GetTranslatedMessage());
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task Translator_LiteralTagTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, GetTranslateUri("fr", "en"))
                .Respond("application/json", GetResponse("Translator_LiteralTagTest.json"));

            var translator = new Translator(_translatorKey, mockHttp.ToHttpClient());

            var sentence = "salut <literal>Jean Bouchier mon ami</literal>";

            var translatedSentence = await translator.TranslateArrayAsync(new string[] { sentence }, "fr", "en");
            var patterns = new Dictionary<string, List<string>>();
            patterns.Add("fr", new List<string>());
            var postProcessor = new PatternsPostProcessor(patterns);
            var postProcessedDocument = postProcessor.Process(translatedSentence[0], "fr");
            Assert.IsNotNull(translatedSentence);
            Assert.AreEqual("Hi Jean Bouchier mon ami", postProcessedDocument.PostProcessedMessage);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task Translator_TranslateFrenchToEnglish()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, GetTranslateUri("fr", "en"))
                .Respond("application/json", GetResponse("Translator_TranslateFrenchToEnglish.json"));

            var translator = new Translator(_translatorKey, mockHttp.ToHttpClient());

            var sentence = "salut 20-10";
            var translatedSentence = await translator.TranslateAsync(sentence, "fr", "en");
            Assert.IsNotNull(translatedSentence);
            Assert.AreEqual("Hi 20-10", translatedSentence.GetTranslatedMessage());
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task Translator_TranslateFrenchToEnglishArray()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, GetTranslateUri("fr", "en"))
                .Respond("application/json", GetResponse("Translator_TranslateFrenchToEnglishArray.json"));

            var translator = new Translator(_translatorKey, mockHttp.ToHttpClient());

            var sentences = new string[] { "mon nom est", "salut", "au revoir" };
            var translatedSentences = await translator.TranslateArrayAsync(sentences, "fr", "en");
            Assert.IsNotNull(translatedSentences);
            Assert.AreEqual(3, translatedSentences.Count, "should be 3 sentences");
            Assert.AreEqual("My name is", translatedSentences[0].GetTranslatedMessage());
            Assert.AreEqual("Hello", translatedSentences[1].GetTranslatedMessage());
            Assert.AreEqual("Good bye", translatedSentences[2].GetTranslatedMessage());
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task Translator_TranslateEnglishToFrench()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, GetTranslateUri("en", "fr"))
                .Respond("application/json", GetResponse("Translator_TranslateEnglishToFrench.json"));

            var translator = new Translator(_translatorKey, mockHttp.ToHttpClient());

            var sentence = "hello";
            var translatedSentence = await translator.TranslateAsync(sentence, "en", "fr");
            Assert.IsNotNull(translatedSentence);
            Assert.AreEqual("Salut", translatedSentence.GetTranslatedMessage());
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task Translator_TranslateEnglishToFrenchArray()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, GetTranslateUri("en", "fr"))
                .Respond("application/json", GetResponse("Translator_TranslateEnglishToFrenchArray.json"));

            var translator = new Translator(_translatorKey, mockHttp.ToHttpClient());

            var sentences = new string[] { "Hello", "Good bye" };
            var translatedSentences = await translator.TranslateArrayAsync(sentences, "en", "fr");
            Assert.IsNotNull(translatedSentences);
            Assert.AreEqual(2, translatedSentences.Count, "should be 2 sentences");
            Assert.AreEqual("Salut", translatedSentences[0].GetTranslatedMessage());
            Assert.AreEqual("Au revoir", translatedSentences[1].GetTranslatedMessage());
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task Translator_InvalidSourceLanguage()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, GetTranslateUri("na", "de"))
                .Respond(HttpStatusCode.BadRequest, "application/json", GetResponse("Translator_InvalidSourceLanguage.json"));

            var translator = new Translator(_translatorKey, mockHttp.ToHttpClient());

            var sentence = "Arrange an appointment for tomorrow";
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await translator.TranslateAsync(sentence, "na", "de"));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task Translator_InvalidTargetLanguage()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, GetTranslateUri("en", "na"))
                .Respond(HttpStatusCode.BadRequest, "application/json", GetResponse("Translator_InvalidTargetLanguage.json"));

            var translator = new Translator(_translatorKey, mockHttp.ToHttpClient());

            var sentence = "Arrange an appointment for tomorrow";
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await translator.TranslateAsync(sentence, "en", "na"));
        }

        private string GetDetectUri()
        {
            return $"https://api.cognitive.microsofttranslator.com/detect?api-version=3.0";
        }

        private string GetTranslateUri(string from, string to)
        {
            return $"https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&includeAlignment=true&includeSentenceLength=true&from={from}&to={to}";
        }
        
        private Stream GetResponse(string fileName)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "TestData", fileName);
            return File.OpenRead(path);
        }
    }
}

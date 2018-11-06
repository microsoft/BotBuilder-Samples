// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.Translation.PostProcessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace Microsoft.Bot.Builder.AI.Translation.Tests
{
    [TestClass]
    public class TranslatorPostProcessorsTests
    {
        private const string _translatorKey = "dummy-key";

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void Translator_PatternsTest_InvalidArguments()
        {
            Dictionary<string, List<string>> patterns = null;

            Assert.ThrowsException<ArgumentNullException>(() => new PatternsPostProcessor(patterns));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void Translator_PatternsTest_EmptyPatternsDictionary()
        {
            var patterns = new Dictionary<string, List<string>>();

            Assert.ThrowsException<ArgumentException>(() => new PatternsPostProcessor(patterns));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task Translator_PatternsTest_EmptyLanguagePatternsData()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, GetTranslateUri("es", "en"))
                .Respond("application/json", GetResponse("Translator_PatternsTest_EmptyLanguagePatternsData.json"));

            var translator = new Translator(_translatorKey, mockHttp.ToHttpClient());
            //using an empty language list won't throw an exception, but it won't affect the post processing for this language
            var patterns = new Dictionary<string, List<string>>();
            var spanishPatterns = new List<string>();
            patterns.Add("es", spanishPatterns);

            IPostProcessor patternsPostProcessor = new PatternsPostProcessor(patterns);
            var sentence = "mi perro se llama Enzo";

            var translatedDocuments = await translator.TranslateArrayAsync(new string[] { sentence }, "es", "en");
            Assert.IsNotNull(translatedDocuments);
            var postProcessedMessage = patternsPostProcessor.Process(translatedDocuments[0], "es").PostProcessedMessage;
            Assert.IsNotNull(postProcessedMessage);
            Assert.AreEqual("My dog's name is Enzo", postProcessedMessage);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task Translator_PatternsTest_FrenchPatterns()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, GetTranslateUri("fr", "en"))
                .Respond("application/json", GetResponse("Translator_PatternsTest_FrenchPatterns.json"));

            var translator = new Translator(_translatorKey, mockHttp.ToHttpClient());
            var patterns = new Dictionary<string, List<string>>();
            var frenchPatterns = new List<string> { "mon nom est (.+)" };
            patterns.Add("fr", frenchPatterns);
            
            IPostProcessor patternsPostProcessor = new PatternsPostProcessor(patterns);

            var sentence = "mon nom est l'etat";
            var translatedDocuments = await translator.TranslateArrayAsync(new string[] { sentence }, "fr", "en");
            Assert.IsNotNull(translatedDocuments);
            var postProcessedMessage = patternsPostProcessor.Process(translatedDocuments[0], "fr").PostProcessedMessage;
            Assert.IsNotNull(postProcessedMessage);
            Assert.AreEqual("My name is l'etat", postProcessedMessage);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task Translator_PatternsTest_FrenchPatternsWithMultipleSpaces()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, GetTranslateUri("fr", "en"))
                .Respond("application/json", GetResponse("Translator_PatternsTest_FrenchPatternsWithMultipleSpaces.json"));

            var translator = new Translator(_translatorKey, mockHttp.ToHttpClient());
            var patterns = new Dictionary<string, List<string>>();
            var frenchPatterns = new List<string> { "mon nom est (.+)" };
            patterns.Add("fr", frenchPatterns);
            
            IPostProcessor patternsPostProcessor = new PatternsPostProcessor(patterns);

            var sentence = "mon     nom     est    l'etat   ";
            var translatedDocuments = await translator.TranslateArrayAsync(new string[] { sentence }, "fr", "en");
            Assert.IsNotNull(translatedDocuments);
            var postProcessedMessage = patternsPostProcessor.Process(translatedDocuments[0], "fr").PostProcessedMessage;
            Assert.IsNotNull(postProcessedMessage);
            Assert.AreEqual("My name is l'etat", postProcessedMessage);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task Translator_PatternsTest_FrenchPatternsWithNumbers()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, GetTranslateUri("fr", "en"))
                .Respond("application/json", GetResponse("Translator_PatternsTest_FrenchPatternsWithNumbers.json"));

            var translator = new Translator(_translatorKey, mockHttp.ToHttpClient());
            var patterns = new Dictionary<string, List<string>>();
            var frenchPatterns = new List<string> { "mon nom est (.+)" };
            patterns.Add("fr", frenchPatterns);
            
            IPostProcessor patternsPostProcessor = new PatternsPostProcessor(patterns);

            var sentence = "J'ai 25 ans et mon nom est l'etat";
            var translatedDocuments = await translator.TranslateArrayAsync(new string[] { sentence }, "fr", "en");
            Assert.IsNotNull(translatedDocuments);
            var postProcessedMessage = patternsPostProcessor.Process(translatedDocuments[0], "fr").PostProcessedMessage;
            Assert.IsNotNull(postProcessedMessage);
            Assert.AreEqual("I am 25 years old and my name is l'etat", postProcessedMessage);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task Translator_PatternsTest_SpanishPatterns()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, GetTranslateUri("es", "en"))
                .Respond("application/json", GetResponse("Translator_PatternsTest_SpanishPatterns.json"));

            var translator = new Translator(_translatorKey, mockHttp.ToHttpClient());
            var patterns = new Dictionary<string, List<string>>();
            var spanishPatterns = new List<string> { "perr[oa]" };
            patterns.Add("es", spanishPatterns);

            IPostProcessor patternsPostProcessor = new PatternsPostProcessor(patterns);
            var sentence = "mi perro se llama Enzo";

            var translatedDocuments = await translator.TranslateArrayAsync(new string[] { sentence }, "es", "en");
            Assert.IsNotNull(translatedDocuments);
            var postProcessedMessage = patternsPostProcessor.Process(translatedDocuments[0], "es").PostProcessedMessage;
            Assert.IsNotNull(postProcessedMessage);
            Assert.AreEqual("My perro's name is Enzo", postProcessedMessage);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void Translator_DictionaryTest_InvalidArguments()
        {
            CustomDictionary userCustomDictonaries = null;
            Assert.ThrowsException<ArgumentNullException>(() => new CustomDictionaryPostProcessor(userCustomDictonaries));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task Translator_PatternsTest_EmptyCustomLanguageDictionaryData()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, GetTranslateUri("fr", "en"))
                .Respond("application/json", GetResponse("Translator_PatternsTest_EmptyCustomLanguageDictionaryData.json"));

            var translator = new Translator(_translatorKey, mockHttp.ToHttpClient());
            var userCustomDictonaries = new CustomDictionary();
            IPostProcessor customDictionaryPostProcessor = new CustomDictionaryPostProcessor(userCustomDictonaries);

            var frenchSentence = "Je veux voir éclair";

            var translatedDocuments = await translator.TranslateArrayAsync(new string[] { frenchSentence }, "fr", "en");
            Assert.IsNotNull(translatedDocuments);
            Assert.ThrowsException<ArgumentException>(() => customDictionaryPostProcessor.Process(translatedDocuments[0], "fr").PostProcessedMessage);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task Translator_DictionaryTest_FrenchDictionary()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, GetTranslateUri("fr", "en"))
                .Respond("application/json", GetResponse("Translator_DictionaryTest_FrenchDictionary.json"));

            var translator = new Translator(_translatorKey, mockHttp.ToHttpClient());

            var userCustomDictonary = new CustomDictionary();
            var frenctDictionary = new Dictionary<string, string>
            {
                { "éclair", "eclairs tart" }
            };

            userCustomDictonary.AddNewLanguageDictionary("fr", frenctDictionary);
            IPostProcessor customDictionaryPostProcessor = new CustomDictionaryPostProcessor(userCustomDictonary);

            var frenchSentence = "Je veux voir éclair";

            var translatedDocuments = await translator.TranslateArrayAsync(new string[] { frenchSentence }, "fr", "en");
            Assert.IsNotNull(translatedDocuments);
            var postProcessedMessage = customDictionaryPostProcessor.Process(translatedDocuments[0], "fr").PostProcessedMessage;
            Assert.IsNotNull(postProcessedMessage);
            Assert.AreEqual("I want to see eclairs tart", postProcessedMessage);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task Translator_DictionaryTest_ItalianDictionary()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, GetTranslateUri("it", "en"))
                .Respond("application/json", GetResponse("Translator_DictionaryTest_ItalianDictionary.json"));

            var translator = new Translator(_translatorKey, mockHttp.ToHttpClient());

            var userCustomDictonary = new CustomDictionary();

            var italianDictionary = new Dictionary<string, string>
            {
                { "camera", "bedroom" },
                { "foto", "personal photo" }
            };
            userCustomDictonary.AddNewLanguageDictionary("it", italianDictionary);

            IPostProcessor customDictionaryPostProcessor = new CustomDictionaryPostProcessor(userCustomDictonary);

            var italianSentence = "Voglio fare una foto nella camera";

            var translatedDocuments = await translator.TranslateArrayAsync(new string[] { italianSentence }, "it", "en");
            Assert.IsNotNull(translatedDocuments);
            var postProcessedMessage = customDictionaryPostProcessor.Process(translatedDocuments[0], "it").PostProcessedMessage;
            Assert.IsNotNull(postProcessedMessage);
            Assert.AreEqual("I want to take a personal photo in the bedroom", postProcessedMessage);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task Translator_PatternsAndDictionaryTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Post, GetTranslateUri("fr", "en"))
                .Respond("application/json", GetResponse("Translator_PatternsAndDictionaryTest.json"));

            var translator = new Translator(_translatorKey, mockHttp.ToHttpClient());
            
            //creating the patterns post processor
            var attachedPostProcessors = new List<IPostProcessor>();
            var patterns = new Dictionary<string, List<string>>();
            var frenchPatterns = new List<string> { "mon nom est (.+)" };

            patterns.Add("fr", frenchPatterns);
            IPostProcessor patternsPostProcessor = new PatternsPostProcessor(patterns);

            //attaching patterns post processor to the list of post processors
            attachedPostProcessors.Add(patternsPostProcessor);

            //creating user custom dictionary post processor
            var userCustomDictonaries = new CustomDictionary();
            var frenctDictionary = new Dictionary<string, string>
            {
                { "etat", "Eldad" }
            };

            userCustomDictonaries.AddNewLanguageDictionary("fr", frenctDictionary);
            IPostProcessor customDictionaryPostProcessor = new CustomDictionaryPostProcessor(userCustomDictonaries);

            //attaching user custom dictionary post processor to the list of post processors
            attachedPostProcessors.Add(customDictionaryPostProcessor);

            var sentence = "mon nom est etat";

            //translating the document
            var translatedDocuments = await translator.TranslateArrayAsync(new string[] { sentence }, "fr", "en");
            Assert.IsNotNull(translatedDocuments);
            string postProcessedMessage = null;

            //test the actual use case of the compined post processors together
            foreach (var postProcessor in attachedPostProcessors)
            {
                postProcessedMessage = postProcessor.Process(translatedDocuments[0], "fr").PostProcessedMessage;
            }

            Assert.IsNotNull(postProcessedMessage);
            Assert.AreEqual("My name is Eldad", postProcessedMessage);
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

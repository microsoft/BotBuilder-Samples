using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using Microsoft.Bot.Builder.AI.Translation.Model;
using Microsoft.Bot.Builder.AI.Translation.ResponseGenerator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Bot.Builder.AI.Translation.PreProcessor;

namespace Microsoft.Bot.Builder.AI.Translation.Tests
{
    [TestClass]
    public class PreProcessorTests
    {

        [TestMethod]
        [TestCategory("PreProcessor")]
        public void PreProcessor_DetectPreprocess()
        {
            IPreProcessor preProcesor = new TranslatorPreProcessor();
            var sentence = "salut <literal>Jean Bouchier mon ami</literal>";
            sentence = preProcesor.PreprocessMessage(sentence);
            Assert.AreEqual("salut Jean Bouchier mon ami ", sentence);
        }

        [TestMethod]
        [TestCategory("PreProcessor")]
        public void PreProcessor_TranslatePreprocess()
        {
            IPreProcessor preProcesor = new TranslatorPreProcessor();
            var sentence = "salut <literal>Jean Bouchier mon ami</literal>";
            preProcesor.PreprocessMessage(sentence, out var processedTextToTranslate, out var noTranslatePhrases);
            Assert.AreEqual("salut Jean Bouchier mon ami ", processedTextToTranslate);
            Assert.IsTrue(noTranslatePhrases.Contains("(Jean Bouchier mon ami)"));
            Assert.AreEqual(1, noTranslatePhrases.Count);
        }
    }
}

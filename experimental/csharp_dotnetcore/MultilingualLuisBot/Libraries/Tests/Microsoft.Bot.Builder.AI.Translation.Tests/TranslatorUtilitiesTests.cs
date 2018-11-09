// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.AI.Translation.PostProcessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Bot.Builder.AI.Translation.Tests
{
    [TestClass]
    public class TranslatorUtilitiesTests
    {
        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_JoinWordsIntoSentences_InvalidArguments()
        {
            string delimeter = null;
            string[] tokens = new string[] { };
            Assert.ThrowsException<ArgumentNullException>(() => PostProcessingUtilities.Join(delimeter, tokens));

            delimeter = " ";
            tokens = null;
            Assert.ThrowsException<ArgumentNullException>(() => PostProcessingUtilities.Join(delimeter, tokens));

            delimeter = null;
            tokens = null;
            Assert.ThrowsException<ArgumentNullException>(() => PostProcessingUtilities.Join(delimeter, tokens));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_JoinWordsIntoSentences_SimpleScenario()
        {
            string delimeter = " ";
            string[] tokens = new string[] { };
            var joinedSentence = PostProcessingUtilities.Join(delimeter, tokens);
            Assert.IsNotNull(joinedSentence);
            Assert.AreEqual(string.Empty, joinedSentence);

            //Check the direct joining case
            tokens = new string[] { "My", "name", "is", "Eldad" };
            joinedSentence = PostProcessingUtilities.Join(delimeter, tokens);
            Assert.IsNotNull(joinedSentence);
            Assert.AreEqual("My name is Eldad", joinedSentence);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_JoinWordsIntoSentences_ComplexScenario()
        {
            string delimeter = " ";
            string[] tokens = new string[] { "Mon", "nom", "est", "l'", "etat" };

            //Check special joining case when the tokens contains punctuation marks
            var joinedSentence = PostProcessingUtilities.Join(delimeter, tokens);
            Assert.IsNotNull(joinedSentence);
            Assert.AreEqual("Mon nom est l'etat", joinedSentence);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_SplitSentence_InvalidArguments()
        {
            string sentence = null;
            Assert.ThrowsException<ArgumentNullException>(() => PostProcessingUtilities.SplitSentence(sentence));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_SplitSentence_SourceSentence_SimpleScenario()
        {
            //testing splitting without using translation alignment information, in this case only split using white space delimiter
            string sourceSentence = "mon nom est l'etat";
            var splittedSentence = PostProcessingUtilities.SplitSentence(sourceSentence);
            AreEqualArrays(new string[] { "mon", "nom", "est", "l'etat" }, splittedSentence);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_SplitSentence_SourceSentence_ComplexScenario()
        {
            //testing splitting using translation alignment information, in this case split splitting takes care of punctuation characters
            string sourceSentence = "mon nom est l'etat";
            string rawAlignment = "0:2-0:1 4:6-3:6 8:10-8:9 12:13-11:13 14:17-15:19";
            string[] alignments = rawAlignment.Split(" ");
            var splittedSentence = PostProcessingUtilities.SplitSentence(sourceSentence, alignments);
            AreEqualArrays(new string[] { "mon", "nom", "est", "l'", "etat" }, splittedSentence);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_SplitSentence_SourceSentence_ComplexScenarioWithPunctuation()
        {
            //testing splitting using translation alignment information, in this case split splitting takes care of punctuation characters
            string sourceSentence = "mon nom est l'etat?.!";
            string rawAlignment = "0:2-0:1 4:6-3:6 8:10-8:9 12:13-11:13 14:17-15:19 18:20-20:22";
            string[] alignments = rawAlignment.Split(" ");
            var splittedSentence = PostProcessingUtilities.SplitSentence(sourceSentence, alignments);
            AreEqualArrays(new string[] { "mon", "nom", "est", "l'", "etat", "?.!" }, splittedSentence);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_SplitSentence_TargetSentence_SimpleScenario()
        {
            //testing splitting without using translation alignment information, in this case only split using white space delimiter
            string translatedSentence = "My dog's name is Enzo";
            var splittedSentence = PostProcessingUtilities.SplitSentence(translatedSentence, isSourceSentence: false);
            AreEqualArrays(new string[] { "My", "dog's", "name", "is", "Enzo" }, splittedSentence);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_SplitSentence_TargetSentence_ComplexScenario()
        {
            //testing splitting using translation alignment information, in this case split splitting takes care of punctuation characters
            string translatedSentence = "My dog's name is Enzo";
            string rawAlignment = "0:1-0:1 3:7-3:5 9:10-6:7 9:10-14:15 12:16-9:12 18:21-17:20";
            string[] alignments = rawAlignment.Split(" ");
            var splittedSentence = PostProcessingUtilities.SplitSentence(translatedSentence, alignments, false);
            AreEqualArrays(new string[] { "My", "dog", "'s", "name", "is", "Enzo" }, splittedSentence);
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_WordAlignmentParse_InvalidArguments_NullAlignments()
        {
            string[] alignments = null;
            string[] sourceTokens = new string[] { };
            string[] translatedTokens = new string[] { };
            Assert.ThrowsException<ArgumentNullException>(() => PostProcessingUtilities.WordAlignmentParse(alignments, sourceTokens, translatedTokens));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_WordAlignmentParse_InvalidArguments_NullSourceTokens()
        {
            string[] alignments = new string[] { };
            string[] sourceTokens = null;
            string[] translatedTokens = new string[] { };
            Assert.ThrowsException<ArgumentNullException>(() => PostProcessingUtilities.WordAlignmentParse(alignments, sourceTokens, translatedTokens));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_WordAlignmentParse_InvalidArguments_NullTranslatedTokens()
        {
            string[] alignments = new string[] { };
            string[] sourceTokens = new string[] { };
            string[] translatedTokens = null;
            Assert.ThrowsException<ArgumentNullException>(() => PostProcessingUtilities.WordAlignmentParse(alignments, sourceTokens, translatedTokens));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_WordAlignmentParse_InvalidArguments_InvalidAlignmentsFormat()
        {
            string[] alignments = new string[] { "test", "test" };
            string[] sourceTokens = new string[] { };
            string[] translatedTokens = new string[] { };
            Assert.ThrowsException<FormatException>(() => PostProcessingUtilities.WordAlignmentParse(alignments, sourceTokens, translatedTokens));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_WordAlignmentParse()
        {
            string rawAlignment = "0:2-0:1 4:6-3:6 8:10-8:9 12:13-11:13 14:17-15:19";
            string[] alignments = rawAlignment.Split(" ");
            string[] sourceTokens = new string[] { "mon", "nom", "est", "l'", "etat" };
            string[] translatedTokens = new string[] { "my", "name", "is", "the", "state" };
            Dictionary<int, int> alignmentMap = PostProcessingUtilities.WordAlignmentParse(alignments, sourceTokens, translatedTokens);
            Assert.IsNotNull(alignmentMap);
            foreach (KeyValuePair<int, int> alignmentElement in alignmentMap)
            {
                Assert.IsNotNull(alignmentElement);
                Assert.IsNotNull(alignmentElement.Key);
                Assert.IsNotNull(alignmentElement.Value);
                Assert.IsTrue(alignmentElement.Value >= 0);
            }
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_KeepSourceWordInTranslation_InvalidArguments_NullAlignments()
        {
            Dictionary<int, int> alignments = null;
            string[] sourceTokens = new string[] { };
            string[] translatedTokens = new string[] { };
            int sourceWordIndex = 0;
            Assert.ThrowsException<ArgumentNullException>(() => PostProcessingUtilities.KeepSourceWordInTranslation(alignments, sourceTokens, translatedTokens, sourceWordIndex));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_KeepSourceWordInTranslation_InvalidArguments_NegativeAlignments()
        {
            Dictionary<int, int> alignments = new Dictionary<int, int>
            {
                {0, -1 }
            };
            string[] sourceTokens = new string[] { };
            string[] translatedTokens = new string[] { };
            int sourceWordIndex = 0;
            Assert.ThrowsException<ArgumentException>(() => PostProcessingUtilities.KeepSourceWordInTranslation(alignments, sourceTokens, translatedTokens, sourceWordIndex));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_KeepSourceWordInTranslation_InvalidArguments_NullSourceTokens()
        {
            Dictionary<int, int> alignments = new Dictionary<int, int>
            {
                {0, 0 },
                {1, 1 },
            };
            string[] sourceTokens = null;
            string[] translatedTokens = new string[] { };
            int sourceWordIndex = 0;
            Assert.ThrowsException<ArgumentNullException>(() => PostProcessingUtilities.KeepSourceWordInTranslation(alignments, sourceTokens, translatedTokens, sourceWordIndex));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_KeepSourceWordInTranslation_InvalidArguments_NullTranslatedTokens()
        {
            Dictionary<int, int> alignments = new Dictionary<int, int>
            {
                {0, 0 },
                {1, 1 },
            };
            string[] sourceTokens = new string[] { };
            string[] translatedTokens = null;
            int sourceWordIndex = 0;
            Assert.ThrowsException<ArgumentNullException>(() => PostProcessingUtilities.KeepSourceWordInTranslation(alignments, sourceTokens, translatedTokens, sourceWordIndex));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_KeepSourceWordInTranslation_InvalidArguments_NegativeSourceWordIndex()
        {
            Dictionary<int, int> alignments = new Dictionary<int, int>
            {
                {0, 0 },
                {1, 1 },
            };
            string[] sourceTokens = new string[] { };
            string[] translatedTokens = new string[] { };
            int sourceWordIndex = -1;
            Assert.ThrowsException<ArgumentException>(() => PostProcessingUtilities.KeepSourceWordInTranslation(alignments, sourceTokens, translatedTokens, sourceWordIndex));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_KeepSourceWordInTranslation_InvalidArguments_AllInvalidParameters()
        {
            Dictionary<int, int> alignments = new Dictionary<int, int>
            {
                {0, 0 },
                {1, 1 },
            };
            string[] sourceTokens = new string[] { };
            string[] translatedTokens = new string[] { };
            int sourceWordIndex = -1;
            Assert.ThrowsException<ArgumentException>(() => PostProcessingUtilities.KeepSourceWordInTranslation(alignments, sourceTokens, translatedTokens, sourceWordIndex));
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public void TranslatorUtilities_KeepSourceWordInTranslation()
        {
            Dictionary<int, int> alignments = new Dictionary<int, int>
            {
                {0,  1},
                {1,  0},
                {2,  2},
            };
            string[] sourceTokens = new string[] { "ti", "amo", "contento" };
            string[] translatedTokens = new string[] { "love", "you", "happy" };
            int sourceWordIndex = 2;
            translatedTokens = PostProcessingUtilities.KeepSourceWordInTranslation(alignments, sourceTokens, translatedTokens, sourceWordIndex);
            Assert.IsNotNull(translatedTokens);
            Assert.IsTrue(translatedTokens.Length == 3);
            AreEqualArrays(new string[] { "love", "you", "contento" }, translatedTokens);
        }
        /// <summary>
        /// Custom arrays equality check.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedArray">expected array of T type.</param>
        /// <param name="actualArray">actual array of T type.</param>
        private void AreEqualArrays<T>(T[] expectedArray, T[] actualArray)
        {
            Assert.IsNotNull(expectedArray);
            Assert.IsNotNull(actualArray);
            Assert.AreEqual(expectedArray.Length, actualArray.Length);
            T expectedElement;
            T actualElement;
            for (int i = 0; i < expectedArray.Length; i++)
            {
                expectedElement = expectedArray[i];
                actualElement = actualArray[i];
                Assert.AreEqual(expectedElement, actualElement);
            }
        }
    }
}

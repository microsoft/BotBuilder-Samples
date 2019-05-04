// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.Translation.Model;
using Microsoft.Bot.Builder.AI.Translation.PostProcessor;
using Microsoft.Bot.Builder.AI.Translation.PreProcessor;
using Microsoft.Bot.Builder.AI.Translation.RequestBuilder;
using Microsoft.Bot.Builder.AI.Translation.ResponseGenerator;

namespace Microsoft.Bot.Builder.AI.Translation
{
    /// <summary>
    /// Provides access to the Microsoft Translator Text API.
    /// Uses API to detect text language and translate text from a source language to a target language.
    /// Apply translation customizations through patterns and custom dictionary.
    /// </summary>
    public class Translator : ITranslator
    {
        private IPreProcessor _preProcessor;
        private IRequestBuilder _requestBuilder;
        private IResponseGenerator _responseGenerator;
        private List<IPostProcessor> attachedPostProcessors;

        /// <summary>
        /// Initializes a new instance of the <see cref="Translator"/> class.
        /// </summary>
        /// <param name="apiKey">Your subscription key for the Microsoft Translator Text API.</param>
        /// <param name="httpClient">An alternate HTTP client to use.</param>
        public Translator(string apiKey, HttpClient httpClient = null)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            _preProcessor = new TranslatorPreProcessor();
            _requestBuilder = new TranslatorRequestBuilder(apiKey);
            _responseGenerator = new TranslatorResponseGenerator(httpClient);
            attachedPostProcessors = new List<IPostProcessor>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Translator"/> class.
        /// </summary>
        /// <param name="apiKey">Your subscription key for the Microsoft Translator Text API.</param>
        /// <param name="patterns">List of regex patterns, indexed by language identifier,
        /// that can be used to flag text that should not be translated.</param>
        /// /// <param name="userConfiguredLanguageDictionary">Custom languages dictionary object, used to store all the different languages dictionaries
        /// configured by the user to overwrite the translator output to certain vocab by the custom dictionary translation.</param>
        /// <param name="httpClient">An alternate HTTP client to use.</param>
        public Translator(string apiKey, Dictionary<string, List<string>> patterns, ConfiguredLanguageDictionary userConfiguredLanguageDictionary, HttpClient httpClient = null)
            : this(apiKey, httpClient)
        {
            InitializePostProcessors(patterns, userConfiguredLanguageDictionary);
        }

        /// <summary>
        /// Detects the language of the input text.
        /// </summary>
        /// <param name="textToDetect">The text to translate.</param>
        /// <returns>A task that represents the detection operation.
        /// The task result contains the id of the detected language.</returns>
        public async Task<string> DetectAsync(string textToDetect)
        {
            textToDetect = _preProcessor.PreprocessMessage(textToDetect);

            var payload = new TranslatorRequestModel[] { new TranslatorRequestModel { Text = textToDetect } };

            using (var request = _requestBuilder.BuildDetectRequest(payload))
            {
                var detectedLanguages = await _responseGenerator.GenerateDetectResponseAsync(request).ConfigureAwait(false);
                return detectedLanguages.First().Language;
            }
        }

        /// <summary>
        /// Translates a single message from a source language to a target language.
        /// </summary>
        /// <param name="textToTranslate">The text to translate.</param>
        /// <param name="sourceLanguage">The language code of the translation text. For example, "en" for English.</param>
        /// <param name="targetLanguage">The language code to translate the text into.</param>
        /// <returns>A task that represents the translation operation.
        /// The task result contains the translated document.</returns>
        public async Task<ITranslatedDocument> TranslateAsync(string textToTranslate, string sourceLanguage, string targetLanguage)
        {
            var results = await TranslateArrayAsync(new string[] { textToTranslate }, sourceLanguage, targetLanguage).ConfigureAwait(false);
            return results.First();
        }

        /// <summary>
        /// Translates an array of strings from a source language to a target language.
        /// </summary>
        /// <param name="translateArraySourceTexts">The strings to translate.</param>
        /// <param name="sourceLanguage">The language code of the translation text. For example, "en" for English.</param>
        /// <param name="targetLanguage">The language code to translate the text into.</param>
        /// <returns>A task that represents the translation operation.
        /// The task result contains a list of the translated documents.</returns>
        public async Task<List<ITranslatedDocument>> TranslateArrayAsync(string[] translateArraySourceTexts, string sourceLanguage, string targetLanguage)
        {
            var translatedDocuments = new List<ITranslatedDocument>();
            for (var srcTxtIndx = 0; srcTxtIndx < translateArraySourceTexts.Length; srcTxtIndx++)
            {
                var currentTranslatedDocument = new TranslatedDocument(translateArraySourceTexts[srcTxtIndx]);
                translatedDocuments.Add(currentTranslatedDocument);

                // Check for literal tag in input user message
                _preProcessor.PreprocessMessage(currentTranslatedDocument.GetSourceMessage(), out var processedText, out var literanlNoTranslateList);
                currentTranslatedDocument.SetSourceMessage(processedText);
                translateArraySourceTexts[srcTxtIndx] = processedText;
                currentTranslatedDocument.SetLiteranlNoTranslatePhrases(literanlNoTranslateList);
            }

            // list of translation request for the service
            var payload = translateArraySourceTexts.Select(s => new TranslatorRequestModel { Text = s });
            using (var request = _requestBuilder.BuildTranslateRequest(sourceLanguage, targetLanguage, payload))
            {
                var translatedResults = await _responseGenerator.GenerateTranslateResponseAsync(request).ConfigureAwait(false);
                var sentIndex = 0;
                foreach (var translatedValue in translatedResults)
                {
                    var translation = translatedValue.Translations.First();
                    var currentTranslatedDocument = translatedDocuments[sentIndex];
                    currentTranslatedDocument.SetRawAlignment(translation.Alignment?.Projection ?? null);
                    currentTranslatedDocument.SetTranslatedMessage(translation.Text);

                    if (!string.IsNullOrEmpty(currentTranslatedDocument.GetRawAlignment()))
                    {
                        var alignments = currentTranslatedDocument.GetRawAlignment().Trim().Split(' ');
                        currentTranslatedDocument.SetSourceTokens(PostProcessingUtilities.SplitSentence(currentTranslatedDocument.GetSourceMessage(), alignments));
                        currentTranslatedDocument.SetTranslatedTokens(PostProcessingUtilities.SplitSentence(translation.Text, alignments, false));
                        currentTranslatedDocument.SetIndexedAlignment(PostProcessingUtilities.WordAlignmentParse(alignments, currentTranslatedDocument.GetSourceTokens(), currentTranslatedDocument.GetTranslatedTokens()));
                        currentTranslatedDocument.SetTranslatedMessage(PostProcessingUtilities.Join(" ", currentTranslatedDocument.GetTranslatedTokens()));
                    }
                    else
                    {
                        var translatedText = translation.Text;
                        currentTranslatedDocument.SetTranslatedMessage(translatedText);
                        currentTranslatedDocument.SetSourceTokens(new string[] { currentTranslatedDocument.GetSourceMessage() });
                        currentTranslatedDocument.SetTranslatedTokens(new string[] { currentTranslatedDocument.GetTranslatedMessage() });
                        currentTranslatedDocument.SetIndexedAlignment(new Dictionary<int, int>());
                    }

                    sentIndex++;
                }

                // post process all translated documents
                PostProcesseDocuments(translatedDocuments, sourceLanguage);
                return translatedDocuments;
            }
        }

        /// <summary>
        /// Initialize attached post processors according to the availability of patterns and custom dictionary provided to the translator constructor.
        /// </summary>
        private void InitializePostProcessors(Dictionary<string, List<string>> patterns, ConfiguredLanguageDictionary userConfiguredLanguageDictionary)
        {
            attachedPostProcessors = new List<IPostProcessor>();
            if (patterns != null && patterns.Count > 0)
            {
                attachedPostProcessors.Add(new PatternsPostProcessor(patterns));
            }

            if (userConfiguredLanguageDictionary != null && !userConfiguredLanguageDictionary.IsEmpty())
            {
                attachedPostProcessors.Add(new ConfiguredLanguageDictionaryPostProcessor(userConfiguredLanguageDictionary));
            }
        }

        /// <summary>
        /// Applies all the attached post processors to the translated messages.
        /// </summary>
        /// <param name="translatedDocuments">List of <see cref="ITranslatedDocument"/> represent the output of the translator module.</param>
        /// <param name="languageId">Current language id.</param>
        private void PostProcesseDocuments(List<ITranslatedDocument> translatedDocuments, string languageId)
        {
            foreach (var translatedDocument in translatedDocuments)
            {
                foreach (var postProcessor in attachedPostProcessors)
                {
                    translatedDocument.SetTranslatedMessage(postProcessor.Process(translatedDocument, languageId).PostProcessedMessage);
                }
            }
        }
    }
}

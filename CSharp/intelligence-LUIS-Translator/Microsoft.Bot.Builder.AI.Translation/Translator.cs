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
    /// Uses api key and detect input language translate single sentence or array of sentences then apply translation post processing fix.
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
        /// <param name="preProcessor">The PreProcessor to use.</param>"
        /// <param name="requestBuilder">The RequestBuilder to use.</param>
        /// <param name="responseGenerator">The ResponseBuilder to use.</param>
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
        /// <param name="preProcessor">The PreProcessor to use.</param>"
        /// <param name="requestBuilder">The RequestBuilder to use.</param>
        /// <param name="responseGenerator">The ResponseBuilder to use.</param>
        /// <param name="httpClient">An alternate HTTP client to use.</param>
        /// <param name="patterns">List of regex patterns, indexed by language identifier,
        /// that can be used to flag text that should not be translated.</param>
        /// /// <param name="userCustomDictonaries">Custom languages dictionary object, used to store all the different languages dictionaries
        /// configured by the user to overwrite the translator output to certain vocab by the custom dictionary translation.</param>
        public Translator(string apiKey, Dictionary<string, List<string>> patterns, CustomDictionary userCustomDictonaries, HttpClient httpClient = null)
            : this(apiKey, httpClient)
        {
            InitializePostProcessors(patterns, userCustomDictonaries);
        }

        public async Task<string> DetectAsync(string textToDetect)
        {
            textToDetect = _preProcessor.PreprocessMessage(textToDetect);

            var payload = new TranslatorRequestModel[] { new TranslatorRequestModel { Text = textToDetect } };

            using (var request = _requestBuilder.GetDetectRequestMessage(payload))
            {
                var detectedLanguages = await _responseGenerator.GenerateDetectResponseAsync(request).ConfigureAwait(false);
                return detectedLanguages.First().Language;
            }
        }

        public async Task<ITranslatedDocument> TranslateAsync(string textToTranslate, string from, string to)
        {
            var results = await TranslateArrayAsync(new string[] { textToTranslate }, from, to).ConfigureAwait(false);
            return results.First();
        }

        public async Task<List<ITranslatedDocument>> TranslateArrayAsync(string[] translateArraySourceTexts, string from, string to)
        {
            var translatedDocuments = new List<ITranslatedDocument>();
            for (var srcTxtIndx = 0; srcTxtIndx < translateArraySourceTexts.Length; srcTxtIndx++)
            {
                // Check for literal tag in input user message
                var currentTranslatedDocument = new TranslatedDocument(translateArraySourceTexts[srcTxtIndx]);
                translatedDocuments.Add(currentTranslatedDocument);
                _preProcessor.PreprocessMessage(currentTranslatedDocument.GetSourceMessage(), out var processedText, out var literanlNoTranslateList);
                currentTranslatedDocument.SetSourceMessage(processedText);
                translateArraySourceTexts[srcTxtIndx] = processedText;
                currentTranslatedDocument.SetLiteranlNoTranslatePhrases(literanlNoTranslateList);
            }

            // list of translation request for the service
            var payload = translateArraySourceTexts.Select(s => new TranslatorRequestModel { Text = s });
            using (var request = _requestBuilder.GetTranslateRequestMessage(from, to, payload))
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
                PostProcesseDocuments(translatedDocuments, from);
                return translatedDocuments;
            }
        }

        /// <summary>
        /// Initialize attached post processors according to what the user sent in the middle ware constructor.
        /// </summary>
        private void InitializePostProcessors(Dictionary<string, List<string>> patterns, CustomDictionary userCustomDictonaries)
        {
            attachedPostProcessors = new List<IPostProcessor>();
            if (patterns != null && patterns.Count > 0)
            {
                attachedPostProcessors.Add(new PatternsPostProcessor(patterns));
            }

            if (userCustomDictonaries != null && !userCustomDictonaries.IsEmpty())
            {
                attachedPostProcessors.Add(new CustomDictionaryPostProcessor(userCustomDictonaries));
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

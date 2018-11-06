// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.Bot.Builder.AI.Translation.PostProcessor
{
    /// <summary>
    /// PatternsPostProcessor  is used to handle translation errors while translating numbers
    /// and to handle the words that need to be kept same as source language from provided template each line having a regex
    /// having first group matching the words that needs to be kept.
    /// </summary>
    public class PatternsPostProcessor : IPostProcessor
    {
        private readonly Dictionary<string, HashSet<string>> _processedPatterns;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternsPostProcessor"/> class that indexes input template for the source language.
        /// </summary>
        /// <param name="patterns">No translate patterns for different languages.</param>
        public PatternsPostProcessor(Dictionary<string, List<string>> patterns)
        {
            if (patterns == null)
            {
                throw new ArgumentNullException(nameof(patterns));
            }

            if (patterns.Count == 0)
            {
                throw new ArgumentException(MessagesProvider.EmptyPatternsErrorMessage);
            }

            _processedPatterns = new Dictionary<string, HashSet<string>>();
            foreach (var item in patterns)
            {
                _processedPatterns.Add(item.Key, new HashSet<string>());
                foreach (var pattern in item.Value)
                {
                    var processedLine = pattern.Trim();

                    // if (the pattern doesn't follow this format (pattern), add the braces around the pattern
                    if (!Regex.IsMatch(pattern, "(\\(.+\\))"))
                    {
                        processedLine = '(' + processedLine + ')';
                    }

                    _processedPatterns[item.Key].Add(processedLine);
                }
            }
        }

        /// <summary>
        /// Process the logic for patterns post processor used to handle numbers and no translate list.
        /// </summary>
        /// <param name="translatedDocument">Translated document.</param>
        /// <param name="languageId">Current source language id.</param>
        /// <returns>A <see cref="PostProcessedDocument"/> stores the original translated document state and the newly post processed message.</returns>
        public PostProcessedDocument Process(ITranslatedDocument translatedDocument, string languageId)
        {
            // validate function arguments for null and incorrect format
            ValidateParameters(translatedDocument);

            // flag to indicate if the source message contains number , will used for
            var containsNum = Regex.IsMatch(translatedDocument.GetSourceMessage(), @"\d");

            // output variable declaration
            string processedResult;

            // temporary pattern is used to contain two set of patterns :
            //  - the post processed patterns that was configured by the user ie : _processedPatterns and
            //  - the   liternal no translate pattern ie : translatedDocument.LiteranlNoTranslatePhrases , which takes the following regx "<literal>(.*)</literal>" , so the following code checks if this pattern exists in the translated document object to be added to the no translate list
            //  - ex : translatedDocument.SourceMessage = I like my friend <literal>happy</literal> , the literal tag here specifies that the word "happy" shouldn't be translated
            var temporaryPatterns = _processedPatterns[languageId];

            if (translatedDocument.GetLiteranlNoTranslatePhrases() != null && translatedDocument.GetLiteranlNoTranslatePhrases().Count > 0)
            {
                temporaryPatterns.UnionWith(translatedDocument.GetLiteranlNoTranslatePhrases());
            }

            if (temporaryPatterns.Count == 0 && !containsNum)
            {
                processedResult = translatedDocument.GetTranslatedMessage();
            }

            if (string.IsNullOrWhiteSpace(translatedDocument.GetRawAlignment()))
            {
                processedResult = translatedDocument.GetTranslatedMessage();
            }

            // loop for all the patterns and substitute each no translate pattern match with the original source words

            // ex : assuming the pattern = "mon nom est (.+)"
            // and the phrase = "mon nom est l'etat"
            // the original translator output for this phrase would be "My name is the state",
            // after applying the patterns post processor , the output would be : "My name is l'etat"
            foreach (var pattern in temporaryPatterns)
            {
                if (Regex.IsMatch(translatedDocument.GetSourceMessage(), pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase))
                {
                    SubstituteNoTranslatePattern(translatedDocument, pattern);
                }
            }

            SubstituteNumericPattern(translatedDocument);
            processedResult = PostProcessingUtilities.Join(" ", translatedDocument.GetTranslatedTokens());
            return new PostProcessedDocument(translatedDocument, processedResult);
        }

        /// <summary>
        /// Substitutes matched no translate pattern with the original token.
        /// </summary>
        /// <param name="translatedDocument">Translated document.</param>
        /// <param name="pattern">The no translate pattern.</param>
        private void SubstituteNoTranslatePattern(ITranslatedDocument translatedDocument, string pattern)
        {
            // get the matched no translate pattern
            var matchNoTranslate = Regex.Match(translatedDocument.GetSourceMessage(), pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // calculate the boundaries of the pattern match
            // ex : "mon nom est l'etat
            // start index = 12
            // length = 6
            var noTranslateStartChrIndex = matchNoTranslate.Groups[1].Index;

            // the length of the matched pattern without spaces , which will be used in determining the translated tokens that will be replaced by their original values
            var noTranslateMatchLength = matchNoTranslate.Groups[1].Value.Replace(" ", string.Empty).Length;
            var wrdIndx = 0;
            var chrIndx = 0;
            var newChrLengthFromMatch = 0;
            var srcIndex = -1;
            var newNoTranslateArrayLength = 1;
            var sourceMessageCharacters = translatedDocument.GetSourceMessage().ToCharArray();

            foreach (var wrd in translatedDocument.GetSourceTokens())
            {
                // if the beginning of the current word equals the beginning of the matched no trasnalate word, then assign the current word index to srcIndex
                if (chrIndx == noTranslateStartChrIndex)
                {
                    srcIndex = wrdIndx;
                }

                // the following code block does the folowing :
                // - checks if a match wsa found
                // - checks if this match length equals the starting matching token length, if yes then this is the only token to process,
                // otherwise continue the loop and add the next token to the list of tokens to be processed
                // ex : "mon nom est l'etat"
                // tokens = {"mon", "nom", "est", "l'", "etat"}
                // when the loop reaches the token "l'" then srcIndex will = 3, but we don't want to consider only the token "l'" as the no translate token,
                // instead we want to match the whole "l'etat" string regardless how many tokens it contains ie regardless that "l'etat" is actually composed of 2 tokens "l'" and "etat"
                // so what these condition is trying to do is make the necessary checks that we got all the matched pattern not just a part of it's tokens!

                // checks if match was found or not, because srcIndex value changes only in case a match was found !
                if (srcIndex != -1)
                {
                    // checks if we found all the tokens that matches the pattern
                    if (newChrLengthFromMatch + translatedDocument.GetSourceTokens()[wrdIndx].Length >= noTranslateMatchLength)
                    {
                        break;
                    }

                    // if the previous condition fails it means that the next token is also matched in the pattern, so we increase the size of the no translate words array by 1
                    newNoTranslateArrayLength += 1;

                    // increment newChrLengthFromMatch with the found word size
                    newChrLengthFromMatch += translatedDocument.GetSourceTokens()[wrdIndx].Length;
                }

                // the following block of code is used to calculate the next token starting index which could have two cases
                // the first case is that the current token is followed by a space in this case we increment the next chrIndx by 1 to get the next character after the space
                // the second case is that the token is followed by the next token without spaces , in this case we calculate chrIndx as chrIndx += wrd.Length without incrementing
                // assumption : The provided sourceMessage and sourceMessageCharacters doesn't contain any consecutive white spaces,
                // in our use case this handling is done using the translator output itself using the following line of code in PreprocessMessage function:
                // textToTranslate = Regex.Replace(textToTranslate, @"\s+", " ");//used to remove multiple spaces in input user message
                if (chrIndx + wrd.Length < sourceMessageCharacters.Length && sourceMessageCharacters[chrIndx + wrd.Length] == ' ')
                {
                    chrIndx += wrd.Length + 1;
                }
                else
                {
                    chrIndx += wrd.Length;
                }

                wrdIndx++;
            }

            // if the loop ends and srcIndex then no match was found
            if (srcIndex == -1)
            {
                return;
            }

            // add the no translate words to a new array
            var wrdNoTranslate = new string[newNoTranslateArrayLength];
            Array.Copy(translatedDocument.GetSourceTokens(), srcIndex, wrdNoTranslate, 0, newNoTranslateArrayLength);

            // loop for each of the no translate words and replace it's translation with it's origin
            foreach (var srcWrd in wrdNoTranslate)
            {
                translatedDocument.SetTranslatedTokens(PostProcessingUtilities.KeepSourceWordInTranslation(translatedDocument.GetIndexedAlignment(), translatedDocument.GetSourceTokens(), translatedDocument.GetTranslatedTokens(), srcIndex));
                srcIndex++;
            }
        }

        /// <summary>
        /// Substitute the numeric numbers in translated message with their orignal format in source message.
        /// </summary>
        /// <param name="translatedDocument">Translated document.</param>
        private void SubstituteNumericPattern(ITranslatedDocument translatedDocument)
        {
            var numericMatches = Regex.Matches(translatedDocument.GetSourceMessage(), @"\d+", RegexOptions.Singleline);
            foreach (Match numericMatch in numericMatches)
            {
                var srcIndex = Array.FindIndex(translatedDocument.GetSourceTokens(), row => row == numericMatch.Groups[0].Value);
                translatedDocument.SetTranslatedTokens(PostProcessingUtilities.KeepSourceWordInTranslation(translatedDocument.GetIndexedAlignment(), translatedDocument.GetSourceTokens(), translatedDocument.GetTranslatedTokens(), srcIndex));
            }
        }

        /// <summary>
        /// Validate <see cref="ITranslatedDocument"/> object main parameters for null values.
        /// </summary>
        /// <param name="translatedDocument">The document to validate.</param>
        private void ValidateParameters(ITranslatedDocument translatedDocument)
        {
            if (translatedDocument == null)
            {
                throw new ArgumentNullException(nameof(translatedDocument));
            }

            if (translatedDocument.GetSourceMessage() == null)
            {
                throw new ArgumentNullException(nameof(translatedDocument.GetSourceMessage));
            }

            if (translatedDocument.GetTranslatedMessage() == null)
            {
                throw new ArgumentNullException(nameof(translatedDocument.GetTranslatedMessage));
            }
        }
    }
}

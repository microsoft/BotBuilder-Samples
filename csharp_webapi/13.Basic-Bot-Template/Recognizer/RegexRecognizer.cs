// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace BasicBot
{
    public class RegexRecognizer : IRecognizer
    {
        private Dictionary<Regex, string> _map;

        public RegexRecognizer(Dictionary<Regex, string> map)
        {
            _map = map;
        }

        public async Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
           => await RecognizeInternalAsync(turnContext, cancellationToken);

        public async Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken)
           where T : IRecognizerConvert, new()
        {
            var result = new T();
            result.Convert(await RecognizeInternalAsync(turnContext, cancellationToken));
            return result;
        }

        private Task<RecognizerResult> RecognizeInternalAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var score = new IntentScore() { Score = 1.0 };
            var emptyResult = new RecognizerResult()
            {
                Intents = new Dictionary<string, IntentScore> { { "None", score } },
            };
            var text = turnContext.Activity.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                return Task.FromResult(emptyResult);
            }
            else
            {
                foreach (var regexMap in _map)
                {
                    var match = regexMap.Key.Match(text.Trim());
                    if (match.Success)
                    {
                        var intents = new Dictionary<string, IntentScore> { { regexMap.Value, score } };

                        return Task.FromResult(new RecognizerResult()
                        {
                            Text = text,
                            Intents = intents,
                        });
                    }
                }
            }

            return Task.FromResult(emptyResult);
        }
    }
}

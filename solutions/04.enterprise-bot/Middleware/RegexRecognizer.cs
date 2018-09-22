using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace EnterpriseBot
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
            var text = turnContext.Activity.Text;

            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }
            else
            {
                foreach (var regexMap in _map)
                {
                    var match = regexMap.Key.Match(text);

                    if (match.Success)
                    {
                        var score = new IntentScore() { Score = 1.0 };
                        var intents = new Dictionary<string, IntentScore> { { regexMap.Value, score } };

                        return Task.FromResult(new RecognizerResult()
                        {
                            Text = text,
                            Intents = intents,
                        });
                    }
                }

                return null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.TraceExtensions;

namespace Microsoft.Bot.Builder.AI.LuisVNext
{
    /// <summary>
    /// Class for a recognizer that utilizes the LUISVNext service.
    /// </summary>
    public class LuisVNextRecognizer : AdaptiveRecognizer, IRecognizer 
    {
        /// <summary>
        /// The Kind value for this recognizer.
        /// </summary>
        [JsonProperty("$kind")]
        public const string Kind = "Microsoft.LuisVNextRecognizer";

        /// <summary>
        /// The context label for a LUISVNext trace activity.
        /// </summary>
        private const string LuisVNextTraceLabel = "LuisVNext Trace";

        /// <summary>
        /// The LuisVNextClient instance that handles calls to the service.
        /// </summary>
        private LuisVNextClient LuisVNextClient;

        /// <summary>
        /// The LuisVNextRecognizer constructor.
        /// </summary>
        public LuisVNextRecognizer(LuisVNextOptions options, HttpClientHandler httpClientHandler = default)
        {
            LuisVNextClient = new LuisVNextClient(options, httpClientHandler);
        }

        /// <summary>
        /// The RecognizeAsync function used to recognize the intents and entities in the utterance present in the turn context. 
        /// The function uses the options provided in the constructor of the LUISVNextRecognizer object.
        /// </summary>
        public async Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return await RecognizeInternalAsync(turnContext?.Activity?.AsMessageActivity()?.Text, turnContext, cancellationToken);
        }

        /// <summary>
        /// The RecognizeAsync overload of template type T that allows the user to define their own implementation of the IRecognizerConvert class.
        /// </summary>
        public async Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken) 
            where T : IRecognizerConvert, new()
        {
            var result = new T();
            result.Convert(await RecognizeInternalAsync(turnContext?.Activity?.AsMessageActivity()?.Text, turnContext, cancellationToken));
            return result;
        }

        internal async Task<RecognizerResult> RecognizeInternalAsync(string utterance, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var luisResponse = await LuisVNextClient.Predict(utterance, cancellationToken);
            var recognizerResult = BuildRecognizerResultFromLuisResponse(luisResponse, utterance);

            var traceInfo = JObject.FromObject(
                new
                {
                    response = luisResponse,
                    luisResult = recognizerResult,
                });

            await turnContext.TraceActivityAsync("LuisVNext Recognizer", traceInfo, nameof(LuisVNextRecognizer), LuisVNextTraceLabel, cancellationToken).ConfigureAwait(false);

            return recognizerResult;
        }

        private RecognizerResult BuildRecognizerResultFromLuisResponse(JObject luisResponse, string utterance)
        {
            var prediction = (JObject)luisResponse["prediction"];
            var recognizerResult = new RecognizerResult
            {
                Text = utterance,
                AlteredText = prediction["alteredQuery"]?.Value<string>(),
                Intents = LuisUtil.GetIntents(prediction),
                Entities = LuisUtil.ExtractEntitiesAndMetadata(prediction)
            };

            LuisUtil.AddProperties(prediction, recognizerResult);

            return recognizerResult;
        }
    }
}

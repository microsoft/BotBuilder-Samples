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

namespace Microsoft.Bot.Builder.AI.CLU
{
    /// <summary>
    /// Class for a recognizer that utilizes the CLU service.
    /// </summary>
    public class CluRecognizer : AdaptiveRecognizer, IRecognizer 
    {
        /// <summary>
        /// The Kind value for this recognizer.
        /// </summary>
        [JsonProperty("$kind")]
        public const string Kind = "Microsoft.CluRecognizer";

        /// <summary>
        /// The context label for a CLU trace activity.
        /// </summary>
        private const string CluTraceLabel = "CLU Trace";

        /// <summary>
        /// The CluClient instance that handles calls to the service.
        /// </summary>
        private CluClient CluClient;

        /// <summary>
        /// The CluRecognizer constructor.
        /// </summary>
        public CluRecognizer(CluOptions options, HttpClientHandler httpClientHandler = default)
        {
            CluClient = new CluClient(options, httpClientHandler);
        }

        /// <summary>
        /// The RecognizeAsync function used to recognize the intents and entities in the utterance present in the turn context. 
        /// The function uses the options provided in the constructor of the CluRecognizer object.
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
            var cluResponse = await CluClient.Predict(utterance, cancellationToken);
            var recognizerResult = BuildRecognizerResultFromCluResponse(cluResponse, utterance);

            var traceInfo = JObject.FromObject(
                new
                {
                    response = cluResponse,
                    recognizerResult = recognizerResult,
                });

            await turnContext.TraceActivityAsync("CLU Recognizer", traceInfo, nameof(CluRecognizer), CluTraceLabel, cancellationToken).ConfigureAwait(false);

            return recognizerResult;
        }

        private RecognizerResult BuildRecognizerResultFromCluResponse(JObject cluResponse, string utterance)
        {
            var prediction = (JObject)cluResponse["prediction"];
            var recognizerResult = new RecognizerResult
            {
                Text = utterance,
                AlteredText = prediction["alteredQuery"]?.Value<string>(),
                Intents = CluUtil.GetIntents(prediction),
                Entities = CluUtil.ExtractEntitiesAndMetadata(prediction)
            };

            CluUtil.AddProperties(prediction, recognizerResult);

            return recognizerResult;
        }
    }
}

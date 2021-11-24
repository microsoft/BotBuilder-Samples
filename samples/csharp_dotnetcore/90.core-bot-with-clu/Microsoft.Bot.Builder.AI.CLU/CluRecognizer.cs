using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.TraceExtensions;
using Azure;
using Azure.AI.Language.Conversations;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;

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
        /// The Conversation Analysis Client instance that handles calls to the service.
        /// </summary>
        private ConversationAnalysisClient ConversationsClient;

        /// <summary>
        /// CLU Recognizer Options
        /// </summary>
        private CluOptions Options;

        /// <summary>
        /// Key used when adding Question Answering into to  <see cref="RecognizerResult"/> intents collection.
        /// </summary>
        public const string QuestionAnsweringMatchIntent = "QuestionAnsweringMatch";

        /// <summary>
        /// The CluRecognizer constructor.
        /// </summary>
        public CluRecognizer(CluOptions options, ConversationAnalysisClient conversationAnalysisClient = default)
        {
            // for mocking purposes
            ConversationsClient = conversationAnalysisClient != null ? conversationAnalysisClient
                : new ConversationAnalysisClient(
                    new Uri(options.Endpoint),
                    new AzureKeyCredential(options.EndpointKey),
                    new ConversationAnalysisClientOptions(options.ApiVersion)
                  );
            Options = options;
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
            var analyzeConversationOptions = BuildAnalyzeConversationOptionsFromCluOptions(Options, utterance);
            var cluResponse = await ConversationsClient.AnalyzeConversationAsync(analyzeConversationOptions, cancellationToken);
            var recognizerResult = BuildRecognizerResultFromCluResponse(cluResponse.Value, utterance);

            var traceInfo = JObject.FromObject(
                new
                {
                    response = cluResponse,
                    recognizerResult = recognizerResult,
                });

            await turnContext.TraceActivityAsync("CLU Recognizer", traceInfo, nameof(CluRecognizer), CluTraceLabel, cancellationToken).ConfigureAwait(false);

            return recognizerResult;
        }
        private AnalyzeConversationOptions BuildAnalyzeConversationOptionsFromCluOptions(CluOptions options, string utterance)
        {
            // this will need to be changed in the next release of the Conversations SDK
            return new AnalyzeConversationOptions(options.ProjectName, options.DeploymentName, utterance)
            {
                Verbose = options.Verbose,
                Language = options.Language,
                IsLoggingEnabled = options.IsLoggingEnabled,
                DirectTarget = options.DirectTarget
            };
        }
        private RecognizerResult BuildRecognizerResultFromCluResponse(AnalyzeConversationResult cluResult, string utterance)
        {
            var recognizerResult = new RecognizerResult
            {
                Text = utterance,
                AlteredText = cluResult.Query
            };

            // CLU Projects can be Conversation projects (LuisVNext) or Orchestration projects that
            // can retrieve responses from other types of projects (Question answering, LUIS or Conversations)
            var projectKind = cluResult.Prediction.ProjectKind;

            if (projectKind == ProjectKind.Conversation)
            {
                CluUtil.BuildRecognizerResultFromConversations(cluResult.Prediction, recognizerResult);
            }
            else
            {
                // workflow projects can return results from LUIS, Conversations or QuestionAnswering Projects
                var orchestrationPrediction = cluResult.Prediction as OrchestratorPrediction;

                // finding name of the target project, then finding the target project type
                var respondingProjectName = orchestrationPrediction.TopIntent;
                var targetIntentResult = orchestrationPrediction.Intents[respondingProjectName];

                // targetIntentResult.TargetKind is currently internal but will be changed in next version.
                // GetType() is used temporarily.

                // var targetKind = targetIntentResult.TargetKind;
                var targetKind = targetIntentResult.GetType().Name;

                switch (targetKind)
                {
                    case "ConversationTargetIntentResult":
                        var conversationTargetIntentResult = targetIntentResult as ConversationTargetIntentResult;
                        CluUtil.BuildRecognizerResultFromConversations(conversationTargetIntentResult.Result.Prediction, recognizerResult);
                        break;

                    case "LuisTargetIntentResult":
                        var luisTargetIntentResult = targetIntentResult as LuisTargetIntentResult;
                        CluUtil.BuildRecognizerResultFromLuis(luisTargetIntentResult, recognizerResult);
                        break;

                    case "QuestionAnsweringTargetIntentResult":
                        var questionAnsweringTargetIntentResult = targetIntentResult as QuestionAnsweringTargetIntentResult;
                        CluUtil.BuildRecognizerResultFromQuestionAnswering(questionAnsweringTargetIntentResult, recognizerResult);
                        break;

                    default:
                        //NoneType
                        break;
                }
            }

            CluUtil.AddProperties(cluResult, recognizerResult);

            return recognizerResult;
        }

    }
}

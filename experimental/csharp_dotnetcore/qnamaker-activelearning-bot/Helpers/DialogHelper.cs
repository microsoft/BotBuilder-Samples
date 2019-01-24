using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using QnAMakerActiveLearningBot.QnAMakerServices;

namespace QnAMakerActiveLearningBot.Helpers
{
    /// <summary>
    /// QnAMaker Active Learning Dialog helper class
    /// </summary>
    public class DialogHelper
    {
        /// <summary>
        /// QnA Maker Active Learning dialog name
        /// </summary>
        public string ActiveLearningDialogName = "active-learning-dialog";

        /// <summary>
        /// QnA Maker Active Learning Dialog
        /// </summary>
        public WaterfallDialog QnAMakerActiveLearningDialog;

        private readonly string QnAMakerKey;
        private QnAMakerOptions qnaMakerOptions;
        private readonly BotServices _services;

        // Define value names for values tracked inside the dialogs.
        private const string CurrentQuery = "value-current-query";
        private const string QnAData = "value-qnaData";

        // Dialog Options parameters
        private const float DefaultThreshold = 0.03F;
        private const int DefaultTopN = 3;

        /// <summary>
        /// Dialog helper to generate dialogs
        /// </summary>
        /// <param name="services">Bot Services</param>
        /// <param name="qnAMakerKey">QnA Maker Key</param>
        public DialogHelper(BotServices services, string qnAMakerKey)
        {
            QnAMakerActiveLearningDialog = new WaterfallDialog(ActiveLearningDialogName)
                .AddStep(CallGenerateAnswer)
                .AddStep(FilterLowVariationScoreList)
                .AddStep(CallTrain)
                .AddStep(DisplayQnAResult);
            _services = services;
            this.QnAMakerKey = qnAMakerKey;
        }

        private async Task<DialogTurnResult> CallGenerateAnswer(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var scoreThreshold = DefaultThreshold;
            var top = DefaultTopN;

            QnAMakerOptions qnaMakerOptions = null;

            // Getting options
            if (stepContext.ActiveDialog.State["options"] != null)
            {
                qnaMakerOptions = stepContext.ActiveDialog.State["options"] as QnAMakerOptions;
                scoreThreshold = qnaMakerOptions?.ScoreThreshold != null ? qnaMakerOptions.ScoreThreshold : DefaultThreshold;
                top = qnaMakerOptions?.Top != null ? qnaMakerOptions.Top : DefaultTopN;
            }

            var response = await _services.QnAServices[QnAMakerKey].GetAnswersAsync(stepContext.Context, qnaMakerOptions);

            var filteredResponse = response.Where(answer => answer.Score > scoreThreshold).ToList();

            stepContext.Values[QnAData] = new List<QueryResult>(filteredResponse);
            stepContext.Values[CurrentQuery] = stepContext.Context.Activity.Text;
            return await stepContext.NextAsync(cancellationToken);
        }

        private async Task<DialogTurnResult> FilterLowVariationScoreList(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var response = stepContext.Values[QnAData] as List<QueryResult>;

            var filteredResponse = ActiveLearningHelper.GetLowScoreVariation(response);

            stepContext.Values[QnAData] = filteredResponse;

            if (filteredResponse.Count > 1)
            {
                var suggestedQuestions = new List<string>();
                foreach (var qna in filteredResponse)
                {
                    suggestedQuestions.Add(qna.Questions[0]);
                }

                // Get hero card activity
                var message = CardHelper.GetHeroCard(suggestedQuestions);

                await stepContext.Context.SendActivityAsync(message);

                return new DialogTurnResult(DialogTurnStatus.Waiting);
            }
            else
            {
                return await stepContext.NextAsync(new List<QueryResult>(response), cancellationToken);
            }
        }

        private async Task<DialogTurnResult> CallTrain(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var trainResponses = stepContext.Values[QnAData] as List<QueryResult>;
            var currentQuery = stepContext.Values[CurrentQuery] as string;

            if (trainResponses.Count > 1)
            {
                var reply = stepContext.Context.Activity.Text;
                var qnaResult = trainResponses.Where(kvp => kvp.Questions[0] == reply).FirstOrDefault();

                if (qnaResult != null)
                {
                    stepContext.Values[QnAData] = new List<QueryResult>() { qnaResult };

                    var records = new FeedbackRecord[]
                    {
                        new FeedbackRecord
                        {
                            UserId = stepContext.Context.Activity.Id,
                            UserQuestion = currentQuery,
                            QnaId = qnaResult.Id,
                        }
                    };

                    var feedbackRecords = new FeedbackRecords { Records = records };

                    // Call Active Learning Train API
                    ActiveLearningHelper.CallTrain(_services.QnaEndpoint.Host, feedbackRecords, _services.QnaEndpoint.KnowledgeBaseId, _services.QnaEndpoint.EndpointKey, cancellationToken);

                    return await stepContext.NextAsync(new List<QueryResult>(){ qnaResult }, cancellationToken);
                }
                else
                {
                    return await stepContext.EndDialogAsync();
                }
            }

            return await stepContext.NextAsync(stepContext.Result, cancellationToken);
        }

        private async Task<DialogTurnResult> DisplayQnAResult(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is List<QueryResult> response && response.Count > 0)
            {
                await stepContext.Context.SendActivityAsync(response[0].Answer, cancellationToken: cancellationToken);
            }
            else
            {
                var msg = "No QnAMaker answers found.";
                await stepContext.Context.SendActivityAsync(msg, cancellationToken: cancellationToken);
            }

            return await stepContext.EndDialogAsync();
        }
    }
}

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
    /// Dialog helper class
    /// </summary>
    public class DialogHelper
    {
        /// <summary>
        /// QnA Maker active learning dialog name
        /// </summary>
        public string ActiveLearningDialogName = "active-learning-dialog";

        /// <summary>
        /// QnA Maker active learning Dialog
        /// </summary>
        public WaterfallDialog QnAMakerActiveLearningDialog;

        private readonly string QnAMakerKey;
        private QnAMakerDialogOptions qnaMakerDialogOptions;
        private readonly BotServices _services;

        // Define value names for values tracked inside the dialogs.
        private const string CurrentQuery = "value-current-query";
        private const string QnAData = "value-qnaData";

        // Dialog Options parameters
        private const double DefaultThreshold = 20.0;
        private const int DefaultTopN = 3;

        /// <summary>
        /// Dialog helper to generate dialogs
        /// </summary>
        /// <param name="services">Bot Services</param>
        /// <param name="QnAMakerKey">QnA Maker Key</param>
        public DialogHelper(BotServices services, string QnAMakerKey)
        {
            QnAMakerActiveLearningDialog = new WaterfallDialog(ActiveLearningDialogName)
                .AddStep(CallGenerateAnswer)
                .AddStep(FilterLowVariationScoreList)
                .AddStep(CallTrain)
                .AddStep(DisplayQnAResult);
            _services = services;
            this.QnAMakerKey = QnAMakerKey;
        }

        private async Task<DialogTurnResult> CallGenerateAnswer(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var threshold = DefaultThreshold;
            var topN = DefaultTopN;

            // Getting configurations
            if (stepContext.ActiveDialog.State["options"] != null)
            {
                var qnaMakerDialogOptions = stepContext.ActiveDialog.State["options"] as QnAMakerDialogOptions;

                threshold = qnaMakerDialogOptions?.Threshold != null ? qnaMakerDialogOptions.Threshold : DefaultThreshold;
                topN = qnaMakerDialogOptions?.Top != null ? qnaMakerDialogOptions.Top : DefaultTopN;
            }

            var qnaOptions = new QnAMakerOptions();
            qnaOptions.Top = topN;
            var response = await _services.QnAServices[QnAMakerKey].GetAnswersAsync(stepContext.Context, qnaOptions);
            var filteredResponse = response.Where(answer => answer.Score * 100 > threshold).ToList();

            stepContext.Values[QnAData] = new List<QueryResult>(filteredResponse);
            stepContext.Values[CurrentQuery] = stepContext.Context.Activity.Text;
            return await stepContext.NextAsync(new List<QueryResult>(response), cancellationToken);
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
                var title = "Did you mean:";
                var noMatchText = "None of the above";
                var message = CardHelper.GetHeroCard(title, noMatchText, suggestedQuestions);

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
            var responses = stepContext.Values[QnAData] as List<QueryResult>;
            var currentQuery = stepContext.Values[CurrentQuery] as string;

            if (responses.Count > 1)
            {
                var reply = stepContext.Context.Activity.Text;
                var qnaResult = responses.Where(kvp => kvp.Questions[0] == reply).FirstOrDefault();

                if (qnaResult != null)
                {
                    var feedbackRecords = new List<FeedbackRecordDTO>
                    {
                        new FeedbackRecordDTO
                        {
                            UserId = stepContext.Context.Activity.Id,
                            UserQuestion = currentQuery,
                            QnaId = qnaResult.Id,
                        },
                    };

                    var body = JsonConvert.SerializeObject(new FeedbackRecordsDTO { FeedbackRecords = feedbackRecords });

                    // call train
                    ActiveLearningHelper.CallTrain(_services.QnaEndpoint.Host, body, _services.QnaEndpoint.KnowledgeBaseId, _services.QnaEndpoint.EndpointKey);
                }
                else
                {
                    return await stepContext.EndDialogAsync();
                }
            }

            return await stepContext.NextAsync(new List<QueryResult>(responses), cancellationToken);
        }

        private async Task<DialogTurnResult> DisplayQnAResult(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Values[QnAData] is List<QueryResult> response && response.Count > 0)
            {
                await stepContext.Context.SendActivityAsync(response[0].Answer, cancellationToken: cancellationToken);
            }
            else
            {
                var msg = @"No QnA Maker answers were found.";

                await stepContext.Context.SendActivityAsync(msg, cancellationToken: cancellationToken);
            }

            return await stepContext.EndDialogAsync();
        }
    }
}

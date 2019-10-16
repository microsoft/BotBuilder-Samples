// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Dialogs;
using QnAMakerSample.Utils;

namespace QnAMakerSample.Dialog
{
    /// <summary>
    /// QnAMaker action builder class
    /// </summary>
    public class QnAMakerBaseDialog : ComponentDialog
    {
        // Dialog Options parameters
        public const float DefaultThreshold = 0.3F;
        public const int DefaultTopN = 3;
        public const string DefaultNoAnswer = "No QnAMaker answers found.";

        // Card parameters
        public const string DefaultCardTitle = "Did you mean:";
        public const string DefaultCardNoMatchText = "None of the above.";
        public const string DefaultCardNoMatchResponse = "Thanks for the feedback.";

        // Define value names for values tracked inside the dialogs.
        public const string QnAOptions = "qnaOptions";
        public const string QnADialogResponseOptions = "qnaDialogResponseOptions";
        private const string CurrentQuery = "currentQuery";
        private const string QnAData = "qnaData";
        private const string QnAContextData = "qnaContextData";
        private const string PreviousQnAId = "prevQnAId";

        /// <summary>
        /// QnA Maker action builder
        /// </summary>
        private const string QnAMakerDialogName = "qnamaker-dialog";
        private readonly IBotServices _services;
        private float maximumScoreForLowScoreVariation = 0.95F;

        /// <summary>
        /// Initializes a new instance of the <see cref="QnAMakerBaseDialog"/> class.
        /// Dialog helper to generate dialogs.
        /// </summary>
        /// <param name="_services">Bot Services.</param>
        public QnAMakerBaseDialog(IBotServices services): base(nameof(QnAMakerBaseDialog))
        {
            AddDialog(new WaterfallDialog(QnAMakerDialogName)
                .AddStep(CallGenerateAnswerAsync)
                .AddStep(CallTrain)
                .AddStep(CheckForMultiTurnPrompt)
                .AddStep(DisplayQnAResult));
            _services = services ?? throw new ArgumentNullException(nameof(services));

            // The initial child Dialog to run.
            InitialDialogId = QnAMakerDialogName;
        }

        private static Dictionary<string, object> GetDialogOptionsValue(DialogContext dialogContext)
        {
            var dialogOptions = new Dictionary<string, object>();

            if (dialogContext.ActiveDialog.State["options"] != null)
            {
                dialogOptions = dialogContext.ActiveDialog.State["options"] as Dictionary<string, object>;
            }

            return dialogOptions;
        }

        private async Task<DialogTurnResult> CallGenerateAnswerAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var qnaMakerOptions = new QnAMakerOptions
            {
                ScoreThreshold = DefaultThreshold,
                Top = DefaultTopN
            };

            var dialogOptions = GetDialogOptionsValue(stepContext);

            // Getting options
            if (dialogOptions.ContainsKey(QnAOptions))
            {
                qnaMakerOptions = dialogOptions[QnAOptions] as QnAMakerOptions;
                qnaMakerOptions.ScoreThreshold = qnaMakerOptions?.ScoreThreshold ?? DefaultThreshold;
                qnaMakerOptions.Top = DefaultTopN;
            }

            // Storing the context info
            stepContext.Values[CurrentQuery] = stepContext.Context.Activity.Text;

            // -Check if previous context is present, if yes then put it with the query
            // -Check for id if query is present in reverse index.
            if (!dialogOptions.ContainsKey(QnAContextData))
            {
                dialogOptions[QnAContextData] = new Dictionary<string, int>();
            }
            else
            {
                var previousContextData = dialogOptions[QnAContextData] as Dictionary<string, int>;
                if (dialogOptions[PreviousQnAId] != null)
                {
                    var previousQnAId = Convert.ToInt32(dialogOptions[PreviousQnAId]);

                    if (previousQnAId > 0)
                    {
                        qnaMakerOptions.Context = new QnARequestContext
                        {
                            PreviousQnAId = previousQnAId
                        };

                        if (previousContextData.TryGetValue(stepContext.Context.Activity.Text, out var currentQnAId))
                        {
                            qnaMakerOptions.QnAId = currentQnAId;
                        }
                    }
                }
            }

            // Calling QnAMaker to get response.
            var response = await _services.QnAMakerService.GetAnswersRawAsync(stepContext.Context, qnaMakerOptions).ConfigureAwait(false);

            // Resetting previous query.
            dialogOptions[PreviousQnAId] = -1;
            stepContext.ActiveDialog.State["options"] = dialogOptions;

            // Take this value from GetAnswerResponse 
            var isActiveLearningEnabled = response.ActiveLearningEnabled;

            stepContext.Values[QnAData] = new List<QueryResult>(response.Answers);
            
            // Check if active learning is enabled.
            if (isActiveLearningEnabled && response.Answers.Any() && response.Answers.First().Score <= 0.95)
            {
                // Get filtered list of the response that support low score variation criteria.
                response.Answers = _services.QnAMakerService.GetLowScoreVariation(response.Answers);

                if (response.Answers.Count() > 1)
                {
                    var suggestedQuestions = new List<string>();
                    foreach (var qna in response.Answers)
                    {
                        suggestedQuestions.Add(qna.Questions[0]);
                    }

                    // Get active learning suggestion card activity.
                    var qnaDialogResponseOptions = dialogOptions[QnADialogResponseOptions] as QnADialogResponseOptions;
                    var message = QnACardBuilder.GetSuggestionsCard(suggestedQuestions, qnaDialogResponseOptions.ActiveLearningCardTitle, qnaDialogResponseOptions.CardNoMatchText);
                    await stepContext.Context.SendActivityAsync(message).ConfigureAwait(false);

                    return new DialogTurnResult(DialogTurnStatus.Waiting);
                }
            }

            var result = new List<QueryResult>();
            if (response.Answers.Any())
            {
                result.Add(response.Answers.First());
            }

            stepContext.Values[QnAData] = result;

            // If card is not shown, move to next step with top qna response.
            return await stepContext.NextAsync(result, cancellationToken).ConfigureAwait(false);
        }

        private async Task<DialogTurnResult> CallTrain(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var trainResponses = stepContext.Values[QnAData] as List<QueryResult>;
            var currentQuery = stepContext.Values[CurrentQuery] as string;

            var reply = stepContext.Context.Activity.Text;

            var dialogOptions = GetDialogOptionsValue(stepContext);
            var qnaDialogResponseOptions = dialogOptions[QnADialogResponseOptions] as QnADialogResponseOptions;

            if (trainResponses.Count > 1)
            {
                var qnaResult = trainResponses.FirstOrDefault(kvp => kvp.Questions[0] == reply);

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
                    await _services.QnAMakerService.CallTrainAsync(feedbackRecords).ConfigureAwait(false);

                    return await stepContext.NextAsync(new List<QueryResult>() { qnaResult }, cancellationToken).ConfigureAwait(false);
                }
                else if (reply.Equals(qnaDialogResponseOptions.CardNoMatchText, StringComparison.OrdinalIgnoreCase))
                {
                    await stepContext.Context.SendActivityAsync(qnaDialogResponseOptions.CardNoMatchResponse, cancellationToken: cancellationToken).ConfigureAwait(false);
                    return await stepContext.EndDialogAsync().ConfigureAwait(false);
                }
                else
                {
                    return await stepContext.ReplaceDialogAsync(QnAMakerDialogName, stepContext.ActiveDialog.State["options"], cancellationToken).ConfigureAwait(false);
                }
            }

            return await stepContext.NextAsync(stepContext.Result, cancellationToken).ConfigureAwait(false);
        }

        private async Task<DialogTurnResult> CheckForMultiTurnPrompt(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result is List<QueryResult> response && response.Count > 0)
            {
                // -Check if context is present and prompt exists 
                // -If yes: Add reverse index of prompt display name and its corresponding qna id
                // -Set PreviousQnAId as answer.Id
                // -Display card for the prompt
                // -Wait for the reply
                // -If no: Skip to next step

                var answer = response.First();

                if (answer.Context != null && answer.Context.Prompts.Count() > 0)
                {
                    var dialogOptions = GetDialogOptionsValue(stepContext);
                    var qnaDialogResponseOptions = dialogOptions[QnADialogResponseOptions] as QnADialogResponseOptions;
                    var previousContextData = new Dictionary<string, int>();
                    if (dialogOptions.ContainsKey(QnAContextData))
                    {
                        previousContextData = dialogOptions[QnAContextData] as Dictionary<string, int>;
                    }

                    foreach (var prompt in answer.Context.Prompts)
                    {
                        previousContextData.Add(prompt.DisplayText, prompt.QnaId);
                    }

                    dialogOptions[QnAContextData] = previousContextData;
                    dialogOptions[PreviousQnAId] = answer.Id;
                    stepContext.ActiveDialog.State["options"] = dialogOptions;

                    // Get multi-turn prompts card activity.
                    var message = QnACardBuilder.GetQnAPromptsCard(answer);
                    await stepContext.Context.SendActivityAsync(message).ConfigureAwait(false);

                    return new DialogTurnResult(DialogTurnStatus.Waiting);
                }
            }

            return await stepContext.NextAsync(stepContext.Result, cancellationToken).ConfigureAwait(false);
        }

        private async Task<DialogTurnResult> DisplayQnAResult(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var dialogOptions = GetDialogOptionsValue(stepContext);
            var qnaDialogResponseOptions = dialogOptions[QnADialogResponseOptions] as QnADialogResponseOptions;
            var reply = stepContext.Context.Activity.Text;

            if (reply.Equals(qnaDialogResponseOptions.CardNoMatchText, StringComparison.OrdinalIgnoreCase))
            {
                await stepContext.Context.SendActivityAsync(qnaDialogResponseOptions.CardNoMatchResponse, cancellationToken: cancellationToken).ConfigureAwait(false);
                return await stepContext.EndDialogAsync().ConfigureAwait(false);
            }

            // If previous QnAId is present, replace the dialog
            var previousQnAId = Convert.ToInt32(dialogOptions[PreviousQnAId]);
            if (previousQnAId > 0)
            {
                return await stepContext.ReplaceDialogAsync(QnAMakerDialogName, dialogOptions, cancellationToken).ConfigureAwait(false);
            }

            // If response is present then show that response, else default answer.
            if (stepContext.Result is List<QueryResult> response && response.Count > 0)
            {
                await stepContext.Context.SendActivityAsync(response.First().Answer, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(qnaDialogResponseOptions.NoAnswer, cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            return await stepContext.EndDialogAsync().ConfigureAwait(false);
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Dialog
{
    /// <summary>
    /// QnAMaker action builder class
    /// </summary>
    public class QnAMakerMultiturnDialog : ComponentDialog
    {
        // Dialog Options parameters	        // Dialog Options parameters
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
        /// Initializes a new instance of the <see cref="QnAMakerBaseDialog"/> class.
        /// Dialog helper to generate dialogs.
        /// </summary>
        /// <param name="configuration">QnA Maker configuration.</param>
        public QnAMakerMultiturnDialog(IQnAMakerConfiguration configuration) : base(nameof(QnAMakerMultiturnDialog))
        {
            AddDialog(new WaterfallDialog(QnAMakerDialogName)
                .AddStep(CallGenerateAnswerAsync)
                .AddStep(CheckForMultiTurnPrompt)
                .AddStep(DisplayQnAResult));
            _qnaService = configuration?.QnAMakerService ?? throw new ArgumentNullException(nameof(configuration));

            // The initial child Dialog to run.
            InitialDialogId = QnAMakerDialogName;
        }

        private const string QnAMakerDialogName = "qnamaker-multiturn-dialog";
        private readonly QnAMaker _qnaService;

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
                Top = DefaultTopN,
                Context = new QnARequestContext(),
                QnAId = 0
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

                        qnaMakerOptions.QnAId = 0;
                        if (previousContextData.TryGetValue(stepContext.Context.Activity.Text.ToLower(), out var currentQnAId))
                        {
                            qnaMakerOptions.QnAId = currentQnAId;
                        }
                    }
                }
            }

            // Calling QnAMaker to get response.
            var response = await _qnaService.GetAnswersRawAsync(stepContext.Context, qnaMakerOptions).ConfigureAwait(false);

            // Resetting previous query.
            dialogOptions[PreviousQnAId] = -1;
            stepContext.ActiveDialog.State["options"] = dialogOptions;

            stepContext.Values[QnAData] = new List<QueryResult>(response.Answers);

            var result = new List<QueryResult>();
            if (response.Answers.Any())
            {
                result.Add(response.Answers.First());
            }

            stepContext.Values[QnAData] = result;

            // If card is not shown, move to next step with top qna response.
            return await stepContext.NextAsync(result, cancellationToken).ConfigureAwait(false);
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

                if (answer.Context != null && answer.Context.Prompts != null && answer.Context.Prompts.Count() > 0)
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
                        previousContextData[prompt.DisplayText.ToLower()] = prompt.QnaId;
                    }

                    dialogOptions[QnAContextData] = previousContextData;
                    dialogOptions[PreviousQnAId] = answer.Id;
                    stepContext.ActiveDialog.State["options"] = dialogOptions;

                    // Get multi-turn prompts card activity.
                    var message = GetQnAPromptsCardWithoutNoMatch(answer);
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

        /// <summary>
        /// Get multi-turn prompts card.
        /// </summary>
        /// <param name="result">Result to be dispalyed as prompts.</param>
        /// <returns>IMessageActivity.</returns>
        private static IMessageActivity GetQnAPromptsCardWithoutNoMatch(QueryResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var chatActivity = Activity.CreateMessageActivity();
            chatActivity.Text = result.Answer;
            var buttonList = new List<CardAction>();

            // Add all prompt
            foreach (var prompt in result.Context.Prompts)
            {
                buttonList.Add(
                    new CardAction()
                    {
                        Value = prompt.DisplayText,
                        Type = "imBack",
                        Title = prompt.DisplayText,
                    });
            }

            var plCard = new HeroCard()
            {
                Buttons = buttonList
            };

            // Create the attachment.
            var attachment = plCard.ToAttachment();

            chatActivity.Attachments.Add(attachment);

            return chatActivity;
        }
    }
}

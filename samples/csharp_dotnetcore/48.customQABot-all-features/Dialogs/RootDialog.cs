// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA.Dialogs;
using Microsoft.Bot.Builder.AI.QnA.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    /// <summary>
    /// This is an example root dialog. Replace this with your applications.
    /// </summary>
    public class RootDialog : ComponentDialog
    {
        private const string InitialDialog = "initial-dialog";
        private const string DefaultCardTitle = "Did you mean:"; // Accepts string to display Active Learning Card Title.
        private const string DefaultCardNoMatchText = "None of the above."; // Accepts string to display Active Learning Card no match text.
        private const string DefaultCardNoMatchResponse = "Thanks for the feedback."; // Accepts string to display Active Learning Card no match response.

        private const float ScoreThreshold = 0.3f; // Score threshold accepts values in the range of 0 to 1
        private const int Top = 3; // Accepts integer representing number of answers to return from knowledge base.
        private const string RankerType = "Default"; // Accepts "Default" or "QuestionOnly" ranker type.
        private const bool IsTest = false; // Accepts true/false to call test or production environment
        private const bool IncludeUnstructuredSources = true; // Accepts true/false to query unstructured sources.

        /// <summary>
        /// Initializes a new instance of the <see cref="RootDialog"/> class.
        /// </summary>
        /// <param name="services">Bot Services.</param>
        public RootDialog(IConfiguration configuration)
            : base("root")
        {
            AddDialog(CreateQnAMakerDialog(configuration));

            AddDialog(new WaterfallDialog(InitialDialog)
               .AddStep(InitialStepAsync));

            // The initial child Dialog to run.
            InitialDialogId = InitialDialog;
        }

        private QnAMakerDialog CreateQnAMakerDialog(IConfiguration configuration)
        {
            var hostname = configuration["QnAEndpointHostName"];
            if (string.IsNullOrEmpty(hostname))
            {
                throw new ArgumentException(nameof(hostname));
            }
            hostname = GetHostname(hostname);

            var endpointKey = configuration["QnAEndpointKey"];
            if (string.IsNullOrEmpty(endpointKey))
            {
                throw new ArgumentException(nameof(endpointKey));
            }

            var knowledgeBaseId = configuration["QnAKnowledgeBaseId"];
            if (string.IsNullOrEmpty(knowledgeBaseId))
            {
                throw new ArgumentException(nameof(knowledgeBaseId));
            }

            var enablePreciseAnswer = IsPreciseAnswerEnabled(configuration["EnablePreciseAnswer"]);
            var displayPreciseAnswerOnly = DisplayPreciseAnswerOnly(configuration["DisplayPreciseAnswerOnly"]);

            // Create a new instance of QnAMakerDialog with dialogOptions initialized.
            var qnamakerDialog = new QnAMakerDialog(nameof(QnAMakerDialog), knowledgeBaseId, endpointKey, hostname, noAnswer: MessageFactory.Text(configuration["DefaultAnswer"] ?? string.Empty), cardNoMatchResponse: MessageFactory.Text(DefaultCardNoMatchResponse))
            {
                Threshold = ScoreThreshold,
                ActiveLearningCardTitle = DefaultCardTitle,
                CardNoMatchText = DefaultCardNoMatchText,
                Top = Top,
                Filters = { },
                QnAServiceType = ServiceType.Language,
                EnablePreciseAnswer = enablePreciseAnswer,
                DisplayPreciseAnswerOnly = displayPreciseAnswerOnly,
                IncludeUnstructuredSources = IncludeUnstructuredSources,
                RankerType = RankerType,
                IsTest = IsTest
            };
            return qnamakerDialog;
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken) => await stepContext.BeginDialogAsync(nameof(QnAMakerDialog), null, cancellationToken);

        private static string GetHostname(string hostname)
        {
            if (!hostname.StartsWith("https://"))
            {
                hostname = string.Concat("https://", hostname);
            }
            return hostname;
        }

        private bool IsPreciseAnswerEnabled(string enablePreciseAnswer)
        {
            var rawEnablePreciseAnswer = enablePreciseAnswer;
            if (!string.IsNullOrWhiteSpace(rawEnablePreciseAnswer))
            {
                return bool.Parse(rawEnablePreciseAnswer);
            }
            return true;
        }

        private BoolExpression DisplayPreciseAnswerOnly(string displayPreciseAnswerOnly)
        {
            var rawDisplayPreciseAnswerOnly = displayPreciseAnswerOnly;
            if (!string.IsNullOrWhiteSpace(rawDisplayPreciseAnswerOnly))
            {
                return bool.Parse(rawDisplayPreciseAnswerOnly);
            }
            return false;
        }
    }
}

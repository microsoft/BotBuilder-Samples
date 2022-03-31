// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        private const string DialogId = "initial-dialog";
        private const string ActiveLearningCardTitle = "Did you mean:";
        private const string ActiveLearningCardNoMatchText = "None of the above.";
        private const string ActiveLearningCardNoMatchResponse = "Thanks for the feedback.";

        private const float ScoreThreshold = 0.3f;
        private const int TopAnswers = 3;
        private const string RankerType = "Default";
        private const bool IsTest = false;
        private const bool IncludeUnstructuredSources = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootDialog"/> class.
        /// </summary>
        /// <param name="configuration">An <see cref="IConfiguration"/> instance.</param>
        public RootDialog(IConfiguration configuration)
            : base("root")
        {
            AddDialog(CreateQnAMakerDialog(configuration));

            AddDialog(new WaterfallDialog(DialogId)
               .AddStep(InitialStepAsync));

            // The initial child Dialog to run.
            InitialDialogId = DialogId;
        }

        private QnAMakerDialog CreateQnAMakerDialog(IConfiguration configuration)
        {
            const string missingConfigError = "{0} is missing or empty in configuration.";

            var hostname = configuration["LanguageEndpointHostName"];
            if (string.IsNullOrEmpty(hostname))
            {
                throw new ArgumentException(string.Format(missingConfigError, "LanguageEndpointHostName"));
            }

            var endpointKey = configuration["LanguageEndpointKey"];
            if (string.IsNullOrEmpty(endpointKey))
            {
                throw new ArgumentException(string.Format(missingConfigError, "LanguageEndpointKey"));
            }

            var knowledgeBaseId = configuration["ProjectName"];
            if (string.IsNullOrEmpty(knowledgeBaseId))
            {
                throw new ArgumentException(string.Format(missingConfigError, "ProjectName"));
            }

            var enablePreciseAnswer = bool.Parse(configuration["EnablePreciseAnswer"]);
            var displayPreciseAnswerOnly = bool.Parse(configuration["DisplayPreciseAnswerOnly"]);

            // Create a new instance of QnAMakerDialog with dialogOptions initialized.
            var noAnswer = MessageFactory.Text(configuration["DefaultAnswer"] ?? string.Empty);
            var qnamakerDialog = new QnAMakerDialog(nameof(QnAMakerDialog), knowledgeBaseId, endpointKey, hostname, noAnswer: noAnswer, cardNoMatchResponse: MessageFactory.Text(ActiveLearningCardNoMatchResponse))
            {
                Threshold = ScoreThreshold,
                ActiveLearningCardTitle = ActiveLearningCardTitle,
                CardNoMatchText = ActiveLearningCardNoMatchText,
                Top = TopAnswers,
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

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(QnAMakerDialog), null, cancellationToken);
        }
    }
}

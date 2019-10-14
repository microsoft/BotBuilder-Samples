// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Recognizers.Text;
using Newtonsoft.Json.Linq;
using QnAMakerSample.Utils;

namespace QnAMakerSample.Dialog
{
    /// <summary>
    /// This is an example root dialog. Replace this with your applications.
    /// </summary>
    public class RootDialog : ComponentDialog
    {
        /// <summary>
        /// QnA Maker initial dialog
        /// </summary>
        private const string InitialDialog = "initial-dialog";

        /// <summary>
        /// Initializes a new instance of the <see cref="RootDialog"/> class.
        /// </summary>
        /// <param name="services">Bot Services.</param>
        public RootDialog(IBotServices services)
            : base("root")
        {
            AddDialog(new QnAMakerBaseDialog(services));

            AddDialog(new WaterfallDialog(InitialDialog)
               .AddStep(InitialStepAsync));

            // The initial child Dialog to run.
            InitialDialogId = InitialDialog;
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Set values for generate answer options.
            var qnamakerOptions = new QnAMakerOptions
            {
                ScoreThreshold = QnAMakerBaseDialog.DefaultThreshold,
                Top = QnAMakerBaseDialog.DefaultTopN
            };

            // Set values for dialog responses.
            var qnaDialogResponseOptions = new QnADialogResponseOptions
            {
                NoAnswer = QnAMakerBaseDialog.DefaultNoAnswer,
                ActiveLearningCardTitle = QnAMakerBaseDialog.DefaultCardTitle,
                CardNoMatchText = QnAMakerBaseDialog.DefaultCardNoMatchText,
                CardNoMatchResponse = QnAMakerBaseDialog.DefaultCardNoMatchResponse
            };

            var dialogOptions = new Dictionary<string, object>
            {
                [QnAMakerBaseDialog.QnAOptions] = qnamakerOptions,
                [QnAMakerBaseDialog.QnADialogResponseOptions] = qnaDialogResponseOptions
            };

            return await stepContext.BeginDialogAsync(nameof(QnAMakerBaseDialog), dialogOptions, cancellationToken);
        }
    }
}

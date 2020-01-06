// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.AI.QnA.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Dialog
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
            AddDialog(new QnAMakerMultiturnDialog(services));

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
                ScoreThreshold = QnAMakerMultiturnDialog.DefaultThreshold,
                Top = QnAMakerMultiturnDialog.DefaultTopN,
                Context = new QnARequestContext()
            };

            var noAnswer = (Activity)Activity.CreateMessageActivity();
            noAnswer.Text = QnAMakerMultiturnDialog.DefaultNoAnswer;

            var cardNoMatchResponse = new Activity(QnAMakerMultiturnDialog.DefaultCardNoMatchResponse);

            // Set values for dialog responses.	
            var qnaDialogResponseOptions = new QnADialogResponseOptions
            {
                NoAnswer = noAnswer,
                ActiveLearningCardTitle = QnAMakerMultiturnDialog.DefaultCardTitle,
                CardNoMatchText = QnAMakerMultiturnDialog.DefaultCardNoMatchText,
                CardNoMatchResponse = cardNoMatchResponse
            };

            var dialogOptions = new Dictionary<string, object>
            {
                [QnAMakerMultiturnDialog.QnAOptions] = qnamakerOptions,
                [QnAMakerMultiturnDialog.QnADialogResponseOptions] = qnaDialogResponseOptions
            };

            return await stepContext.BeginDialogAsync(nameof(QnAMakerMultiturnDialog), dialogOptions, cancellationToken);
        }
    }
}

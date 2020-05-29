/* eslint no-extra-boolean-cast: 0 */
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const {
    ComponentDialog,
    DialogTurnStatus,
    WaterfallDialog
} = require('botbuilder-dialogs');

const { QnACardBuilder } = require('../utils/qnaCardBuilder');

// Default parameters
const DefaultThreshold = 0.3;
const DefaultTopN = 3;
const DefaultNoAnswer = 'No QnAMaker answers found.';

// Card parameters
const DefaultCardTitle = 'Did you mean:';
const DefaultCardNoMatchText = 'None of the above.';
const DefaultCardNoMatchResponse = 'Thanks for the feedback.';

// Define value names for values tracked inside the dialogs.
const QnAOptions = 'qnaOptions';
const QnADialogResponseOptions = 'qnaDialogResponseOptions';
const CurrentQuery = 'currentQuery';
const QnAData = 'qnaData';
const QnAContextData = 'qnaContextData';
const PreviousQnAId = 'prevQnAId';

/// QnA Maker dialog.
const QNAMAKER_DIALOG = 'qnamaker-dialog';
const QNAMAKER_MULTITURN_DIALOG = 'qnamaker-multiturn-dialog';

class QnAMakerMultiturnDialog extends ComponentDialog {
    /**
     * Core logic of QnA Maker dialog.
     * @param {QnAMaker} qnaService A QnAMaker service object.
     */
    constructor(qnaService) {
        super(QNAMAKER_MULTITURN_DIALOG);

        this._qnaMakerService = qnaService;

        this.addDialog(new WaterfallDialog(QNAMAKER_DIALOG, [
            this.callGenerateAnswerAsync.bind(this),
            this.checkForMultiTurnPrompt.bind(this),
            this.displayQnAResult.bind(this)
        ]));

        this.initialDialogId = QNAMAKER_DIALOG;
    }

    /**
    * @param {WaterfallStepContext} stepContext contextual information for the current step being executed.
    */
    async callGenerateAnswerAsync(stepContext) {
        // Default QnAMakerOptions
        var qnaMakerOptions = {
            scoreThreshold: DefaultThreshold,
            top: DefaultTopN,
            context: {},
            qnaId: -1
        };

        var dialogOptions = getDialogOptionsValue(stepContext);

        if (dialogOptions[QnAOptions] != null) {
            qnaMakerOptions = dialogOptions[QnAOptions];
            qnaMakerOptions.scoreThreshold = qnaMakerOptions.scoreThreshold ? qnaMakerOptions.scoreThreshold : DefaultThreshold;
            qnaMakerOptions.top = qnaMakerOptions.top ? qnaMakerOptions.top : DefaultThreshold;
        }

        // Storing the context info
        stepContext.values[CurrentQuery] = stepContext.context.activity.text;

        var previousContextData = dialogOptions[QnAContextData];
        var prevQnAId = dialogOptions[PreviousQnAId];

        if (previousContextData != null && prevQnAId != null) {
            if (prevQnAId > 0) {
                qnaMakerOptions.context = {
                    previousQnAId: prevQnAId
                };

                qnaMakerOptions.qnaId = 0;
                if (previousContextData[stepContext.context.activity.text.toLowerCase()] !== null) {
                    qnaMakerOptions.qnaId = previousContextData[stepContext.context.activity.text.toLowerCase()];
                }
            }
        }

        // Calling QnAMaker to get response.
        var response = await this._qnaMakerService.getAnswersRaw(stepContext.context, qnaMakerOptions);

        // Resetting previous query.
        dialogOptions[PreviousQnAId] = -1;
        stepContext.activeDialog.state.options = dialogOptions;

        // Take this value from GetAnswerResponse.
        stepContext.values[QnAData] = response.answers;

        var result = [];
        if (response.answers.length > 0) {
            result.push(response.answers[0]);
        }

        stepContext.values[QnAData] = result;

        return await stepContext.next(result);
    }

    /**
    * @param {WaterfallStepContext} stepContext contextual information for the current step being executed.
    */
    async checkForMultiTurnPrompt(stepContext) {
        if (stepContext.result != null && stepContext.result.length > 0) {
            // -Check if context is present and prompt exists.
            // -If yes: Add reverse index of prompt display name and its corresponding qna id.
            // -Set PreviousQnAId as answer.Id.
            // -Display card for the prompt.
            // -Wait for the reply.
            // -If no: Skip to next step.

            var answer = stepContext.result[0];

            if (answer.context != null && answer.context.prompts != null && answer.context.prompts.length > 0) {
                var dialogOptions = getDialogOptionsValue(stepContext);

                var previousContextData = {};

                if (!!dialogOptions[QnAContextData]) {
                    previousContextData = dialogOptions[QnAContextData];
                }

                answer.context.prompts.forEach(prompt => {
                    previousContextData[prompt.displayText.toLowerCase()] = prompt.qnaId;
                });

                dialogOptions[QnAContextData] = previousContextData;
                dialogOptions[PreviousQnAId] = answer.id;
                stepContext.activeDialog.state.options = dialogOptions;

                // Get multi-turn prompts card activity.
                var message = QnACardBuilder.GetQnAPromptsCard(answer);
                await stepContext.context.sendActivity(message);

                return { status: DialogTurnStatus.waiting };
            }
        }

        return await stepContext.next(stepContext.result);
    }

    /**
    * @param {WaterfallStepContext} stepContext contextual information for the current step being executed.
    */
    async displayQnAResult(stepContext) {
        var dialogOptions = getDialogOptionsValue(stepContext);
        var qnaDialogResponseOptions = dialogOptions[QnADialogResponseOptions];

        var reply = stepContext.context.activity.text;

        if (reply === qnaDialogResponseOptions.cardNoMatchText) {
            await stepContext.context.sendActivity(qnaDialogResponseOptions.cardNoMatchResponse);
            return await stepContext.endDialog();
        }

        var previousQnAId = dialogOptions[PreviousQnAId];
        if (previousQnAId > 0) {
            return await stepContext.replaceDialog(QNAMAKER_DIALOG, dialogOptions);
        }

        var responses = stepContext.result;
        if (responses != null) {
            if (responses.length > 0) {
                await stepContext.context.sendActivity(responses[0].answer);
            } else {
                await stepContext.context.sendActivity(qnaDialogResponseOptions.noAnswer);
            }
        }

        return await stepContext.endDialog();
    }
}

function getDialogOptionsValue(dialogContext) {
    var dialogOptions = {};

    if (dialogContext.activeDialog.state.options !== null) {
        dialogOptions = dialogContext.activeDialog.state.options;
    }

    return dialogOptions;
}

module.exports.QnAMakerMultiturnDialog = QnAMakerMultiturnDialog;
module.exports.QNAMAKER_MULTITURN_DIALOG = QNAMAKER_MULTITURN_DIALOG;
module.exports.DefaultThreshold = DefaultThreshold;
module.exports.DefaultTopN = DefaultTopN;
module.exports.DefaultNoAnswer = DefaultNoAnswer;
module.exports.DefaultCardTitle = DefaultCardTitle;
module.exports.DefaultCardNoMatchText = DefaultCardNoMatchText;
module.exports.DefaultCardNoMatchResponse = DefaultCardNoMatchResponse;
module.exports.QnAOptions = QnAOptions;
module.exports.QnADialogResponseOptions = QnADialogResponseOptions;

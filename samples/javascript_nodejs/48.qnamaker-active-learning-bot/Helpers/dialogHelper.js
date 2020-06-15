/* eslint dot-notation: 0 */
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { CardFactory } = require('botbuilder');
const { DialogTurnStatus, WaterfallDialog } = require('botbuilder-dialogs');

// Card parameters
const cardTitle = 'Did you mean:';
const cardNoMatchText = 'None of the above.';
const cardNoMatchResponse = 'Thanks for the feedback.';

class DialogHelper {
    /**
     * QnAMaker Active Learning Dialog helper class.
     * @param {QnAMaker} qnamaker An instance of QnAMaker service.
     */
    constructor(qnamaker) {
        this.activeLearningDialogName = 'active-learning-dialog';
        this.qnaData = 'value-qnaData';
        this.currentQuery = 'value-current-query';

        this.qnaMaker = qnamaker;
        this.qnaMakerActiveLearningDialog = new WaterfallDialog(this.activeLearningDialogName);

        this.qnaMakerActiveLearningDialog
            .addStep(this.callGenerateAnswer.bind(this))
            .addStep(this.filterLowVariationScoreList.bind(this))
            .addStep(this.callTrain.bind(this))
            .addStep(this.displayQnAResult.bind(this));
    }

    /**
    * @param {WaterfallStepContext} stepContext contextual information for the current step being executed.
    */
    async callGenerateAnswer(stepContext) {
        // Default QnAMakerOptions
        var qnaMakerOptions = {
            ScoreThreshold: 0.03,
            Top: 3
        };

        if (stepContext.activeDialog.state['options'] != null) {
            qnaMakerOptions = stepContext.activeDialog.state['options'];
        }

        // Perform a call to the QnA Maker service to retrieve matching Question and Answer pairs.
        var qnaResults = await this.qnaMaker.getAnswers(stepContext.context, qnaMakerOptions);

        var filteredResponses = qnaResults.filter(r => r.score > qnaMakerOptions.ScoreThreshold);

        stepContext.values[this.qnaData] = filteredResponses;
        stepContext.values[this.currentQuery] = stepContext.context.activity.text;

        return await stepContext.next();
    }

    /**
    * @param {WaterfallStepContext} stepContext contextual information for the current step being executed.
    */
    async filterLowVariationScoreList(stepContext) {
        var responses = stepContext.values[this.qnaData];

        var filteredResponses = this.qnaMaker.getLowScoreVariation(responses);
        stepContext.values[this.qnaData] = filteredResponses;

        if (filteredResponses.length > 1) {
            var suggestedQuestions = [];
            filteredResponses.forEach(element => {
                suggestedQuestions.push(element.questions[0]);
            });

            var message = GetHeroCard(suggestedQuestions, cardTitle, cardNoMatchText);

            await stepContext.context.sendActivity(message);

            return { status: DialogTurnStatus.waiting };
        } else {
            return await stepContext.next(responses);
        }
    }

    /**
    * @param {WaterfallStepContext} stepContext contextual information for the current step being executed.
    */
    async callTrain(stepContext) {
        var trainResponses = stepContext.values[this.qnaData];
        var currentQuery = stepContext.values[this.currentQuery];
        var reply = stepContext.context.activity.text;

        if (trainResponses.length > 1) {
            var qnaResults = trainResponses.filter(r => r.questions[0] === reply);

            if (qnaResults.length > 0) {
                stepContext.values[this.qnaData] = qnaResults;

                var feedbackRecords = {
                    FeedbackRecords: [
                        {
                            UserId: stepContext.context.activity.id,
                            UserQuestion: currentQuery,
                            QnaId: qnaResults[0].id
                        }
                    ]
                };

                // Call Active Learning Train API
                this.qnaMaker.callTrainAsync(feedbackRecords);

                return await stepContext.next(qnaResults);
            } else if (reply === cardNoMatchText) {
                await stepContext.context.sendActivity(cardNoMatchResponse);
                return await stepContext.endDialog();
            } else {
                return await stepContext.replaceDialog(this.activeLearningDialogName, stepContext.activeDialog.state['options']);
            }
        }

        return await stepContext.next(stepContext.result);
    }

    /**
    * @param {WaterfallStepContext} stepContext contextual information for the current step being executed.
    */
    async displayQnAResult(stepContext) {
        var responses = stepContext.result;
        var message = 'No QnAMaker answers found.';

        if (responses != null) {
            if (responses.length > 0) {
                message = responses[0].answer;
            }
        }

        await stepContext.context.sendActivity(message);
        return await stepContext.endDialog();
    }
}

/**
* Get Hero card to get user feedback
* @param {Array} suggestionList A list of suggested questions strings.
* @param {string} cardTitle Title of the card.
* @param {string} cardNoMatchText No match text.
*/
function GetHeroCard(suggestionList, cardTitle = 'Did you mean:', cardNoMatchText = 'None of the above.') {
    var cardActions = [];
    suggestionList.forEach(element => {
        cardActions.push({
            value: element,
            type: 'imBack',
            title: element
        });
    });

    cardActions.push({
        value: cardNoMatchText,
        type: 'imBack',
        title: cardNoMatchText
    });

    var heroCard = CardFactory.heroCard(
        cardTitle,
        [],
        CardFactory.actions(cardActions));

    return { attachments: [heroCard] };
}

module.exports.DialogHelper = DialogHelper;

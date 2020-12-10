// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const {
    QnAMakerDialog
} = require('botbuilder-ai');

const {
    ActivityFactory,
    MessageFactory
} = require('botbuilder');

// Default parameters
const DefaultThreshold = 0.3;
const DefaultTopN = 3;
const DefaultAnswer = 'No QnAMaker answers found.';

// Card parameters
const DefaultCardTitle = 'Did you mean:';
const DefaultCardNoMatchText = 'None of the above.';
const DefaultCardNoMatchResponse = 'Thanks for the feedback.';

/// QnA Maker dialog.
const QNAMAKER_BASE_DIALOG = 'qnamaker-base-dailog';

class QnAMakerBaseDialog extends QnAMakerDialog {
    /**
     * Core logic of QnA Maker dialog.
     * @param {QnAMaker} qnaService A QnAMaker service object.
     */
    constructor(knowledgebaseId, authkey, host, defaultAnswer) {
        const defaultAnswerActivity = MessageFactory.text(!!defaultAnswer.trim() ? defaultAnswer : DefaultAnswer);
        let filters = [];
        super(knowledgebaseId, authkey, host, defaultAnswerActivity, DefaultThreshold, DefaultCardTitle, DefaultCardNoMatchText,
            DefaultTopN, ActivityFactory.cardNoMatchResponse, filters, QNAMAKER_BASE_DIALOG);
        this.id = QNAMAKER_BASE_DIALOG;
    }
}

module.exports.QnAMakerBaseDialog = QnAMakerBaseDialog;
module.exports.QNAMAKER_BASE_DIALOG = QNAMAKER_BASE_DIALOG;
module.exports.DefaultThreshold = DefaultThreshold;
module.exports.DefaultTopN = DefaultTopN;
module.exports.DefaultAnswer = DefaultAnswer;
module.exports.DefaultCardTitle = DefaultCardTitle;
module.exports.DefaultCardNoMatchText = DefaultCardNoMatchText;
module.exports.DefaultCardNoMatchResponse = DefaultCardNoMatchResponse;

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

const { QnAMakerDialog } = require('botbuilder-ai');
const { MessageFactory } = require('botbuilder');

/// QnA Maker dialog.
const QNAMAKER_BASE_DIALOG = 'qnamaker-base-dialog';

class QnAMakerBaseDialog extends QnAMakerDialog {
    /**
     * Core logic of QnA Maker dialog.
     * @param {string} knowledgeBaseId A QnAMaker service object.
     * @param {string} endpointKey A QnAMaker service object.
     * @param {string} endpointHostName A QnAMaker service object.
     * @param {string} defaultAnswer A QnAMaker service object.
     */
    constructor(
        knowledgeBaseId,
        endpointKey,
        endpointHostName,
        defaultAnswer,
    ) {
        let noAnswer;
        if (typeof defaultAnswer === 'string') {
            noAnswer = MessageFactory.text(defaultAnswer);
        }

        super(knowledgeBaseId, endpointKey, endpointHostName, noAnswer);
        this.id = QNAMAKER_BASE_DIALOG;
    }
}

module.exports.QnAMakerBaseDialog = QnAMakerBaseDialog;
module.exports.QNAMAKER_BASE_DIALOG = QNAMAKER_BASE_DIALOG;

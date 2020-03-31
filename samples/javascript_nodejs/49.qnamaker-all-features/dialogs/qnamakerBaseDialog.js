// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const {
    ComponentDialog,
    DialogTurnStatus,
    WaterfallDialog
} = require('botbuilder-dialogs');

const {
    QnAMakerDialog
} = require('botbuilder-ai');

const {
    ActivityFactory
} = require('botbuilder-core');


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
const QNAMAKER_BASE_DIALOG = 'qnamaker-base-dailog';

class QnAMakerBaseDialog extends QnAMakerDialog {
    /**
     * Core logic of QnA Maker dialog.
     * @param {QnAMaker} qnaService A QnAMaker service object.
     */
    constructor(knowledgebaseId, authkey, host) {
        console.log('qnamkaerbasedialog'+ knowledgebaseId+' '+authkey+' '+host)
        var noAnswer = ActivityFactory.DefaultNoAnswer;
        var strictFilters = null;
        super(knowledgebaseId, authkey, host, noAnswer, 0.3, 'Did you mean:','None of the above.',
        3, ActivityFactory.cardNoMatchResponse, strictFilters, QNAMAKER_BASE_DIALOG);
        this.id = QNAMAKER_BASE_DIALOG;
    }

    async GetQnAMakerOptions(dc)
    {
        console.log('GetQnAMakerOptions')
        return new QnAMakerOptions
        {
            ScoreThreshold = DefaultThreshold,
            Top = DefaultTopN,
            QnAId = 0,
            RankerType = "Default",
            IsTest = false
        };
    }

    async  GetQnAResponseOptions(dialogContext)
    {
        console.log('GetQnAResponseOptions');


        var cardNoMatchResponse = new Activity(DefaultCardNoMatchResponse);

        var responseOptions = new QnADialogResponseOptions
        {
            ActiveLearningCardTitle = DefaultCardTitle,
            CardNoMatchText = DefaultCardNoMatchText,
            NoAnswer = noAnswer,
            CardNoMatchResponse = cardNoMatchResponse

        };

        return responseOptions;
    }

}

module.exports.QnAMakerBaseDialog = QnAMakerBaseDialog;
module.exports.QNAMAKER_BASE_DIALOG = QNAMAKER_BASE_DIALOG;
module.exports.DefaultThreshold = DefaultThreshold;
module.exports.DefaultTopN = DefaultTopN;
module.exports.DefaultNoAnswer = DefaultNoAnswer;
module.exports.DefaultCardTitle = DefaultCardTitle;
module.exports.DefaultCardNoMatchText = DefaultCardNoMatchText;
module.exports.DefaultCardNoMatchResponse = DefaultCardNoMatchResponse;
module.exports.QnAOptions = QnAOptions;
module.exports.QnADialogResponseOptions = QnADialogResponseOptions;

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

const { QnAMakerDialog, RankerTypes } = require('botbuilder-ai');
const {
    ComponentDialog,
    DialogSet,
    DialogTurnStatus,
    WaterfallDialog
} = require('botbuilder-dialogs');
const { MessageFactory } = require('botbuilder');
const { ServiceType } = require('botbuilder-ai/lib/qnamaker-interfaces/serviceType');

const INITIAL_DIALOG = 'initial-dialog';
const ROOT_DIALOG = 'root-dialog';
const QNAMAKER_BASE_DIALOG = 'qnamaker-base-dialog';
const ACTIVE_LEARNING_CARD_TITLE = 'Did you mean:';
const ACTIVE_LEARNING_CARD_NO_MATCH_TEXT = 'None of the above.';
const ACTIVE_LEARNING_CARD_NO_MATCH_RESPONSE = 'Thanks for your feedback.';
const SCORE_THRESHOLD = 0.3;
const TOP_ANSWERS = 3;
const RANKER_TYPE = RankerTypes.default;
const ISTEST = false;
const INCLUDE_UNSTRUCTURED_SOURCES = true;

/**
 * Creates QnAMakerDialog instance with provided configuraton values.
 */
const createQnAMakerDialog = (knowledgeBaseId, endpointKey, endpointHostName, defaultAnswer, enablePreciseAnswerRaw, displayPreciseAnswerOnlyRaw) => {
    let noAnswerActivity;
    if (typeof defaultAnswer === 'string' && defaultAnswer !== '') {
        noAnswerActivity = MessageFactory.text(defaultAnswer);
    }

    const qnaMakerDialog = new QnAMakerDialog(
        knowledgeBaseId,
        endpointKey,
        endpointHostName,
        noAnswerActivity,
        SCORE_THRESHOLD,
        ACTIVE_LEARNING_CARD_TITLE,
        ACTIVE_LEARNING_CARD_NO_MATCH_TEXT,
        TOP_ANSWERS,
        ACTIVE_LEARNING_CARD_NO_MATCH_RESPONSE,
        RANKER_TYPE
    );

    qnaMakerDialog.enablePreciseAnswer = enablePreciseAnswerRaw === 'true';
    qnaMakerDialog.displayPreciseAnswerOnly = displayPreciseAnswerOnlyRaw === 'true';
    qnaMakerDialog.qnaServiceType = ServiceType.language;
    qnaMakerDialog.id = QNAMAKER_BASE_DIALOG;
    qnaMakerDialog.isTest = ISTEST;
    qnaMakerDialog.includeUnstructuredSources = INCLUDE_UNSTRUCTURED_SOURCES;

    return qnaMakerDialog;
};

class RootDialog extends ComponentDialog {
    /**
     * Root dialog for this bot. Creates a QnAMakerDialog.
     * @param {string} knowledgeBaseId Knowledge Base ID of the QnA Maker instance.
     * @param {string} endpointKey Endpoint key needed to query QnA Maker.
     * @param {string} endpointHostName Host name of the QnA Maker instance.
     * @param {string} defaultAnswer (optional) Text used to create a fallback response when QnA Maker doesn't have an answer for a question.
     * @param {string} enablePreciseAnswer
     * @param {string} displayPreciseAnswerOnly

    */
    constructor(knowledgeBaseId, endpointKey, endpointHostName, defaultAnswer, enablePreciseAnswer, displayPreciseAnswerOnly) {
        super(ROOT_DIALOG);
        // Initial waterfall dialog.
        this.addDialog(new WaterfallDialog(INITIAL_DIALOG, [
            this.startInitialDialog.bind(this)
        ]));

        this.addDialog(createQnAMakerDialog(knowledgeBaseId, endpointKey, endpointHostName, defaultAnswer, enablePreciseAnswer, displayPreciseAnswerOnly));
        this.initialDialogId = INITIAL_DIALOG;
    }

    /**
     * The run method handles the incoming activity (in the form of a TurnContext) and passes it through the dialog system.
     * If no dialog is active, it will start the default dialog.
     * @param {*} turnContext
     * @param {*} accessor
     */
    async run(context, accessor) {
        const dialogSet = new DialogSet(accessor);
        dialogSet.add(this);

        const dialogContext = await dialogSet.createContext(context);
        const results = await dialogContext.continueDialog();
        if (results.status === DialogTurnStatus.empty) {
            await dialogContext.beginDialog(this.id);
        }
    }

    // This is the first step of the WaterfallDialog.
    // It kicks off the dialog with the QnA Maker with provided options.
    async startInitialDialog(step) {
        return await step.beginDialog(QNAMAKER_BASE_DIALOG);
    }
}

module.exports.RootDialog = RootDialog;

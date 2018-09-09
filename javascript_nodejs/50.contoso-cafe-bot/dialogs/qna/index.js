// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { QnAMaker } = require('botbuilder-ai');

// QnA name from ../../mainDialog/resources/cafeDispatchModel.lu 
const QnA_DIALOG_NAME = 'QnA';
// Name of the QnA Maker service in the .bot file.
const QnA_CONFIGURATION = 'cafeFaqChitChat';
// CONSTS used in QnA Maker query. See [here](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-qna?view=azure-bot-service-4.0&tabs=cs) for additional info
const QnA_TOP_1 = 1;
const QnA_TOP_10 = 10;
const QnA_CONFIDENCE_THRESHOLD = 0.5;

const dialogTurnResult = require('../shared/turnResult');
const { DialogTurnStatus } = require('botbuilder-dialogs');
class QnADialog {
    /**
     * 
     * @param {Object} botConfig bot configuration from .bot file
     */
    constructor(botConfig) {
        if(!botConfig) throw ('Need bot config');

        // add recogizers
        const qnaConfig = botConfig.findServiceByNameOrId(QnA_CONFIGURATION);
        if(!qnaConfig || !qnaConfig.kbId) throw (`QnA Maker application information not found in .bot file. Please ensure you have all required QnA Maker applications created and available in the .bot file. See readme.md for additional information\n`);
        this.qnaRecognizer = new QnAMaker({
            knowledgeBaseId: qnaConfig.kbId,
            endpointKey: qnaConfig.endpointKey,
            host: qnaConfig.hostname
        });
    }
    /**
     * 
     * @param {Object} context context object
     * @param {Boolean} filterSearch if present and set to true, qna will get top 10 results 
     *                               (instead of top 1) and return to caller. This is used by
     *                               the dialogs to get contextual help via QnA metadata filters. 
     */
    async onTurn(context, filterSearch) {
        let topNResults = QnA_TOP_1;
        if(filterSearch !== undefined && filterSearch) topNResults = QnA_TOP_10;
        // Call QnA Maker and get results.
        const qnaResult = await this.qnaRecognizer.generateAnswer(context.activity.text, topNResults, QnA_CONFIDENCE_THRESHOLD);
        if(!qnaResult || qnaResult.length === 0 || !qnaResult[0].answer) {
            // No answer found. respond with dialogturnstatus.empty
            await context.sendActivity(`Sorry, I did not find an answer to what you are looking for in my Knowledge base...`);
            return new dialogTurnResult(DialogTurnStatus.empty);
        }
        if(filterSearch === undefined) {
            // respond with qna result
            await context.sendActivity(qnaResult[0].answer);
            return new dialogTurnResult(DialogTurnStatus.complete);
        } else {
            // just return the result to caller without responding to user. 
            return new dialogTurnResult(DialogTurnStatus.complete, qnaResult);
        }
    }
};

QnADialog.Name = QnA_DIALOG_NAME;

module.exports = QnADialog;
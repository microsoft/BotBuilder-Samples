// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { QnAMaker } = require('botbuilder-ai');

// Name of the QnA Maker service in the .bot file.
const QnA_CONFIGURATION = 'sample.qna';
// CONSTS used in QnA Maker query. See [here](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-qna?view=azure-bot-service-4.0&tabs=cs) for additional info
const QnA_TOP_N = 1;
const QnA_CONFIDENCE_THRESHOLD = 0.5;
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
     */
    async onTurn(context) {
        // Call QnA Maker and get results.
        const qnaResult = await this.qnaRecognizer.generateAnswer(context.activity.text, QnA_TOP_N, QnA_CONFIDENCE_THRESHOLD);
        if(!qnaResult || qnaResult.length === 0 || !qnaResult[0].answer) {
            await context.sendActivity(`No answer found in QnA Maker KB.`);
            return;
        }
        // respond with qna result
        await context.sendActivity(qnaResult[0].answer);
    }
};

module.exports = QnADialog;
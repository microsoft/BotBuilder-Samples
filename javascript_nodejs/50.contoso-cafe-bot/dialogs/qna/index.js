// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { QnAMaker } = require('botbuilder-ai');
const { DialogTurnStatus } = require('botbuilder-dialogs');
const { TurnResult } = require('../shared/helpers');

// QnA name from ../../mainDialog/resources/cafeDispatchModel.lu 
const QnA_DIALOG = 'QnADialog';

// Name of the QnA Maker service in the .bot file.
const QnA_CONFIGURATION = 'cafeFaqChitChat';

// CONSTS used in QnA Maker query. See [here](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-qna?view=azure-bot-service-4.0&tabs=cs) for additional info
const QnA_TOP_1 = 1;
const QnA_TOP_10 = 10;
const QnA_CONFIDENCE_THRESHOLD = 0.5;

class QnADialog {
    /**
     * 
     * @param {Object} botConfig bot configuration from .bot file
     */
    constructor(botConfig, userProfilePropertyAccessor) {
        if(!botConfig) throw ('Need bot config');
        if(!userProfilePropertyAccessor) throw ('Need user profile property accessor');
        this.userProfilePropertyAccessor = userProfilePropertyAccessor;
        
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
            await context.sendActivity(`I'm still learning.. Sorry, I do not know how to help you with that.`);
            await context.sendActivity(`Follow [this link](https://www.bing.com/search?q=${context.activity.text}) to search the web!`);
            return new TurnResult(DialogTurnStatus.empty);
        }
        if(filterSearch === undefined) {
            // respond with qna result
            await context.sendActivity(await this.userSalutation(context) + qnaResult[0].answer);
            return new TurnResult(DialogTurnStatus.complete);
        } else {
            // just return the result to caller without responding to user. 
            return new TurnResult(DialogTurnStatus.complete, qnaResult);
        }
    }
    /**
     * Async helper function to randomly include user salutation. Helps make bot's response feel more natural.
     * 
     * @param {Object} context 
     */
    async userSalutation (context) {
        let salutation = '';
        const userProfile = await this.userProfilePropertyAccessor.get(context);
        if(userProfile !== undefined && userProfile.userName !== '') {
            const userName = userProfile.userName;
            // see if we have user's name
            let userSalutationList = [``,
                                      ``,
                                      `Well... ${userName}, `,
                                      `${userName}, `];
            // Randomly include user's name in response so the reply in personalized.
            const randomNumberIdx = Math.floor(Math.random() * userSalutationList.length);
            if(userSalutationList[randomNumberIdx] !== undefined) salutation = userSalutationList[randomNumberIdx];
        } 
        return salutation;
    }
};

QnADialog.Name = QnA_DIALOG;

module.exports = QnADialog;

const userSalutation = function() {

}
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { QnAMaker } = require('botbuilder-ai');
const { Dialog } = require('botbuilder-dialogs');

// QnA name from ../../dispatcher/resources/cafeDispatchModel.lu
const QNA_DIALOG = 'QnA';

// Name of the QnA Maker service in the .bot file.
const QNA_CONFIGURATION = 'cafeFaqChitChat';

// CONST used in QnA Maker query.
// See (https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-qna) for additional information
const QNA_NUM_OF_RESULTS = 1;
const QNA_CONFIDENCE_THRESHOLD = 0.5;

module.exports = {
    QnADialog: class extends Dialog {
        static get Name() { return QNA_DIALOG; }
        /**
         * Constructor.
         *
         * @param {BotConfiguration} botConfig bot configuration from .bot file
         * @param {StatePropertyAccessor} user profile accessor
         * @param {String} optional: dialog id
         *
         */
        constructor(botConfig, userProfileAccessor, dialogId) {
            (dialogId === undefined) ? super(QNA_DIALOG) : super(dialogId);

            if (!botConfig) throw new Error('Missing parameter. Bot Configuration is required');
            if (!userProfileAccessor) throw new Error('Missing parameter. User profile property accessor is required');

            this.userProfileAccessor = userProfileAccessor;

            // add recognizer
            const qnaConfig = botConfig.findServiceByNameOrId(QNA_CONFIGURATION);
            if (!qnaConfig || !qnaConfig.kbId) throw new Error(`QnA Maker application information not found in .bot file. Please ensure you have all required QnA Maker applications created and available in the .bot file. See readme.md for additional information\n`);
            this.qnaRecognizer = new QnAMaker({
                knowledgeBaseId: qnaConfig.kbId,
                endpointKey: qnaConfig.endpointKey,
                host: qnaConfig.hostname
            });
        }
        /**
         * Override beginDialog.
         *
         * @param {Object} dc dialog context
         * @param {Object} options options
         */
        async beginDialog(dc, options) {
            // Call QnA Maker and get results.
            const qnaResult = await this.qnaRecognizer.generateAnswer(dc.context.activity.text, QNA_NUM_OF_RESULTS, QNA_CONFIDENCE_THRESHOLD);
            if (!qnaResult || qnaResult.length === 0 || !qnaResult[0].answer) {
                // No answer found.
                await dc.context.sendActivity(`I'm still learning.. Sorry, I do not know how to help you with that.`);
                await dc.context.sendActivity(`Follow [this link](https://www.bing.com/search?q=${ dc.context.activity.text }) to search the web!`);
            } else {
                // respond with qna result
                await dc.context.sendActivity(await this.userSalutation(dc.context) + qnaResult[0].answer);
            }
            return await dc.endDialog();
        }
        /**
         * Async helper function to randomly include user salutation. Helps make bot's response feel more natural.
         *
         * @param {Object} context
         */
        async userSalutation(context) {
            let salutation = '';
            const userProfile = await this.userProfileAccessor.get(context);
            if (userProfile !== undefined && userProfile.userName !== '') {
                const userName = userProfile.userName;
                // see if we have user's name
                let userSalutationList = [``,
                    ``,
                    `Well... ${ userName }, `,
                    `${ userName }, `];
                // Randomly include user's name in response so the reply in personalized.
                const randomNumberIdx = Math.floor(Math.random() * userSalutationList.length);
                if (userSalutationList[randomNumberIdx] !== undefined) salutation = userSalutationList[randomNumberIdx];
            }
            return salutation;
        }
    }
};

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { QnADialog } = require('../qna');

// Help intent name from ../../dispatcher/resources/cafeDispatchModel.lu
// This is used by MainDispatcher to dispatch to the appropriate child dialog.
// The name needs to match the intent name returned by LUIS.
const HELP_INTENT_NAME = 'Help';

// Help, ChitChat and QnA share the same QnA Maker model.
// Help and ChitChat dialogs derive from the QnADialog class.
module.exports = {
    HelpDialog: class extends QnADialog {
        static get Name() {
            return HELP_INTENT_NAME;
        }
        /**
         * Constructor.
         *
         * @param {BotConfiguration} botConfig bot configuration from .bot file
         * @param {StatePropertyAccessor} user profile accessor
         */
        constructor(botConfig, userProfileAccessor) {
            if (!botConfig) throw new Error('Missing parameter. Need bot configuration.');
            if (!userProfileAccessor) throw new Error('Missing parameter. Need user profile property accessor');

            super(botConfig, userProfileAccessor, HELP_INTENT_NAME);
        }
    }
};

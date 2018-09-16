// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Help intent name from ../../dispatcher/resources/cafeDispatchModel.lu 
const HELP_INTENT_NAME = 'Help';
const { QnADialog } = require('../qna');

// Help, ChitChat and QnA share the same QnA Maker model. So, just export the Help intent name here. 
// This is used by MainDispatcher to dispatch to the appropriate child dialog.
// The name needs to match the intent name returned by LUIS. 
module.exports = {
    HelpDialog: class extends QnADialog {
        static get Name () { 
            return HELP_INTENT_NAME; 
        }
        /**
         * Constructor. 
         * 
         * @param {Object} botConfig bot configuration
         * @param {Object} userProfileAccessor 
         */
        constructor(botConfig, userProfileAccessor) {
            if (!botConfig) throw ('Missing parameter. Need bot configuration.');
            if (!userProfileAccessor) throw ('Missing parameter. Need user profile property accessor');

            super(botConfig, userProfileAccessor, HELP_INTENT_NAME);
        }
    }
};
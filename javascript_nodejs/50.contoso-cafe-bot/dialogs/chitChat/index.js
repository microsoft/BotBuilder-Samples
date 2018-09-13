// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Help intent name from ../../mainDialog/resources/cafeDispatchModel.lu 
const CHITCHAT_INTENT_NAME = 'ChitChat';

// Help, ChitChat and QnA share the same QnA Maker model. So, just export the Help intent name here. 
// This is used by MainDialog to dispatch to the appropriate child dialog.
// The name needs to match the intent name returned by LUIS. 
module.exports = {
    Name : CHITCHAT_INTENT_NAME
};
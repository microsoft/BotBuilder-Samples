// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Possible LUIS entities. You can refer to dialogs\dispatcher\resources\entities.lu for list of entities
const LUIS_ENTITIES_LIST = ['confirmationList', 'number', 'datetime', 'cafeLocation', 'userName_patternAny', 'userName'];

const LUIS_ENTITIES = {
    user_name_simple: 'userName',
    user_name_patternAny: 'userName_patternAny',
    number: 'number',
    datetime: 'datetime',
    cafeLocation: 'cafeLocation',
    confirmationList: 'confirmationList'
};
// List of all intents this bot will recognize. THis includes intents from the following LUIS models:
//  1. Main dispatcher - see dialogs\dispatcher\resources\cafeDispatchModel.lu
//  2. getUserProfile model - see dialogs\whoAreYou\resources\getUserProfile.lu
//  3. cafeBookTableTurnN model - see dialogs\bookTable\resources\turn-N.lu
const LUIS_INTENTS = {
    Who_are_you: 'Who_are_you',
    What_can_you_do: 'What_can_you_do'
};
module.exports.LUIS_ENTITIES_LIST = LUIS_ENTITIES_LIST;
module.exports.LUIS_INTENTS = LUIS_INTENTS;
module.exports.LUIS_ENTITIES = LUIS_ENTITIES;

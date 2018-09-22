// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// This .json file was generated using ludown's luis batch testing option with ./resources/querySuggestions.lu as input.
// To generate list of utterances in the model, you can call ludown parse toluis --in <INPUT-FILE> -t
const dispatchLUISModel = require('./resources/querySuggestions.json');
const DEFAULT_NUMBER_OF_SUGGESTIONS = 3;
const PROMOTIONS_LIST = ['Book a table', 'Who are you?', 'Sing a song'];

/**
 * Helper method that returns an array of possible queries the user can issue.
 *
 * @param {Integer} numberOfSuggestions
 * @returns {String []} of query suggestions
 */
const generate = function(numberOfSuggestions) {
    let suggestedQueries = ['What can you do?'];
    if (PROMOTIONS_LIST.length !== 0) {
        let rndIdx = Math.floor(PROMOTIONS_LIST.length * Math.random());
        suggestedQueries.push(PROMOTIONS_LIST[rndIdx]);
    }
    let possibleUtterances = dispatchLUISModel;
    if (numberOfSuggestions === undefined) numberOfSuggestions = DEFAULT_NUMBER_OF_SUGGESTIONS;
    while (--numberOfSuggestions) {
        let rndIdx = Math.floor(possibleUtterances.length * Math.random());
        suggestedQueries.push(possibleUtterances[rndIdx].text);
    }
    return suggestedQueries;
};

module.exports = {
    GenSuggestedQueries: generate
};

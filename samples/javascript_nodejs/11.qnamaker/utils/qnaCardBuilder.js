// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { CardFactory, ActionTypes } = require('botbuilder');

class QnACardBuilder {
    /**
    * Get multi-turn prompts card.
    * @param {QnAMakerResult} result Result to be dispalyed as prompts.
    * @param {string} cardNoMatchText No match text.
    */
    static GetQnAPromptsCard(result) {
        var cardActions = [];
        result.context.prompts.forEach(prompt => {
            cardActions.push({
                value: prompt.displayText,
                type: ActionTypes.ImBack,
                title: prompt.displayText
            });
        });

        var heroCard = CardFactory.heroCard('', result.answer, [], CardFactory.actions(cardActions));
        return { attachments: [heroCard] };
    }

    /**
    * Get suggestion hero card to get user feedback.
    * @param {Array} suggestionList A list of suggested questions strings.
    * @param {string} cardTitle Title of the card.
    * @param {string} cardNoMatchText No match text.
    */
    static GetSuggestionCard(suggestionList, cardTitle, cardNoMatchText) {
        var cardActions = [];
        suggestionList.forEach(element => {
            cardActions.push({
                value: element,
                type: ActionTypes.ImBack,
                title: element
            });
        });

        cardActions.push({
            value: cardNoMatchText,
            type: ActionTypes.ImBack,
            title: cardNoMatchText
        });

        var heroCard = CardFactory.heroCard(
            '',
            cardTitle,
            [],
            CardFactory.actions(cardActions));

        return { attachments: [heroCard] };
    }
}

module.exports.QnACardBuilder = QnACardBuilder;

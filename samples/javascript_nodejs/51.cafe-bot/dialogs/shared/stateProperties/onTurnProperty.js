// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { LuisRecognizer } = require('botbuilder-ai');
const { LUIS_ENTITIES_LIST } = require('../helpers');
const { EntityProperty } = require('./entityProperty');
/**
 * On turn property class.
 *   On turn property captures intent and entities from card based input or
 *   NLP results from LUIS.ai
 */
class OnTurnProperty {
    /**
     * On Turn Property constructor.
     *
     * @param {String} intent intent name
     * @param {EntityProperty []} entities Array of Entities
     */
    constructor(intent, entities) {
        this.intent = intent || '';
        this.entities = entities || [];
    }
}
/**
 *
 * Static method to create an on turn property object from LUIS results
 *
 * @param {Object} LUISResults
 * @returns {OnTurnProperty}
 */
OnTurnProperty.fromLUISResults = function(LUISResults) {
    let onTurnProperties = new OnTurnProperty();
    onTurnProperties.intent = LuisRecognizer.topIntent(LUISResults);
    // Gather entity values if available. Uses a const list of LUIS entity names.
    LUIS_ENTITIES_LIST.forEach(luisEntity => {
        if (luisEntity in LUISResults.entities) {
            onTurnProperties.entities.push(new EntityProperty(luisEntity, LUISResults.entities[luisEntity]));
        }
    });
    return onTurnProperties;
};
/**
 *
 * Static method to create an on turn property object from card input
 *
 * @param {Object} cardValue context.activity.value from a card interaction
 * @returns {OnTurnProperty}
 */
OnTurnProperty.fromCardInput = function(cardValue) {
    // All cards used by this bot are adaptive cards with the card's 'data' property set to useful information.
    let onTurnProperties = new OnTurnProperty();
    for (var key in cardValue) {
        if (!cardValue.hasOwnProperty(key)) continue;
        // we do not need to keep the 'text' in on turn property
        if (key.toLowerCase().trim() === 'text') {
            continue;
        }
        if (key.toLowerCase().trim() === 'intent') {
            onTurnProperties.intent = cardValue[key];
        } else {
            onTurnProperties.entities.push(new EntityProperty(key, [cardValue[key]]));
        }
    }
    return onTurnProperties;
};
module.exports = OnTurnProperty;

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const ON_TURN_PROPERTY = 'onTurnProperty';
const DIALOG_STATE_PROPERTY = 'dialogState';
const onTurnProperty = require('./onTurnProperty');
const entityProperty = require('./entityProperty');

/**
 * Bot state class. Holds property accessors to on turn property and dialog property
 */
class BotState {
    /**
     * 
     * @param {Object} state state object. Can be user state, conversation state or anything that implements IPropertyManager
     */
    constructor(state) {
        if(!state) throw ('Need state');
        this.onTurnPropertyAccessor = state.createProperty(ON_TURN_PROPERTY);
        this.dialogPropertyAccessor = state.createProperty(DIALOG_STATE_PROPERTY);
    }
};
/**
 * Static method to create a new entity
 * @param {String} name entity name
 * @param {String} value entity value
 * @returns {Object} {name:<ENTITY-NAME>, value:<ENTITY-VALUE>}
 */
BotState.addNewEntity = function(name, value) {
    return new entityProperty(name, value);
};
/**
 * Static method to create a new turn object
 * @param {String} intent intent name
 * @param {Object []} entities array of entities. Entity is an object with {name:<ENTITY-NAME>, value:<ENTITY-VALUE>}
 * @returns {Object} {intent:<INTENT-NAME>, entities:[<ENTITY-OBJECT>]}
 */
BotState.newTurn = function(intent, entities) {
    return new onTurnProperty(intent, entities);
}

module.exports = BotState;
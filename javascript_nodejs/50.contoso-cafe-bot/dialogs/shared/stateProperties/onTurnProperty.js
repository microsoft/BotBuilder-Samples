// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { entityProperty } = require('./index');

/**
 * On turn property class.
 */
class OnTurnProperty {
    /**
     * On Turn Property constructor.
     * 
     * @param {String} intent intent name
     * @param {entityProperty []} entities Array of Entities
     */
    constructor(intent, entities) {
        this.intent = intent ? intent : '';
        this.entities = entities ? entities : [];
    }
}

module.exports = OnTurnProperty;
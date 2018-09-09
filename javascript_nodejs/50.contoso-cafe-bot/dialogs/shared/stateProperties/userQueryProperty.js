// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const queryConstraintProperty = require('./queryConstraintProperty');

/**
 * On turn property class.
 */
class UserQueryProperty {
    /**
     * Reservations property constructor.
     * 
     * @param {queryConstraintProperty []} constraints Array of query constraints
     */
    constructor(queryConstraints) {
        if(!queryConstraints) throw ('Need query constraints')
        this.queryConstraints = queryConstraints ? queryConstraints : [];
    }
}

module.exports = UserQueryProperty;
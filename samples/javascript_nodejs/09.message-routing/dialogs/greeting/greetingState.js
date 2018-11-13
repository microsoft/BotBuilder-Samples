// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/**
 * Simple object used by the user state property accessor.
 * Used to store the user state.
 */
class GreetingState {
    constructor(name, city) {
        this.name = name;
        this.city = city;
    }
}

module.exports.GreetingState = GreetingState;

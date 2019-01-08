// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

class MemoryStore {

    constructor() {
        this.state = {};
    }

    async load(key) {
        return { value: this.state[key], eTag: '' };
    }

    async save(key, newState, eTag) {

        // debug
        console.log(JSON.stringify(newState, null, 2));

        this.state[key] = newState;
        return true;
    }
}

module.exports.MemoryStore = MemoryStore;

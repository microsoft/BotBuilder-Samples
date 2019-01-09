// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

class MemoryStore {

    constructor() {
        this.state = {};
    }

    async load(key) {

        const oldState = this.state[key];

        // debug
        //console.log('-------------------OLD-----------------');
        //console.log(JSON.stringify(oldState, null, 2));

        return { value: oldState, eTag: '' };
    }

    async save(key, newState, eTag) {

        // debug
        //console.log('-------------------NEW-----------------');
        //console.log(JSON.stringify(newState, null, 2));

        if (newState !== undefined) {
            this.state[key] = JSON.parse(JSON.stringify(newState));
        }
        return true;
    }
}

module.exports.MemoryStore = MemoryStore;

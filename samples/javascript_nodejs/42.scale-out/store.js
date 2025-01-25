// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/**
 * An ETag aware store definition.
 * The interface is defined in terms of JObject to move serialization out of the storage layer
 * while still indicating it is JSON, a fact the store may choose to make use of.
 */
class Store {
    async loadAsync(key) {
        throw new Error('Not implemented');
    }

    async saveAsync(key, content, etag) {
        throw new Error('Not implemented');
    }
}

module.exports.Store = { Store };

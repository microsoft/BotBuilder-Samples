// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.scaleout;

import com.fasterxml.jackson.databind.JsonNode;
import com.microsoft.bot.schema.Pair;

import java.util.concurrent.CompletableFuture;

/**
 * An ETag aware store definition.
 * The interface is defined in terms of Object to move serialization out of the storage layer
 * while still indicating it is JSON, a fact the store may choose to make use of.
 */
public interface Store {

    /**
     * Loads a value from the Store.
     * @param key The key.
     * @return A pair object.
     */
    CompletableFuture<Pair<JsonNode, String>> load(String key);

    /**
     * Saves a values to the Store if the etag matches.
     * @param key The key.
     * @param content The content to save.
     * @param etag The string representing the etag.
     * @return True if the content was saved.
     */
    CompletableFuture<Boolean> save(String key, JsonNode content, String etag);
}

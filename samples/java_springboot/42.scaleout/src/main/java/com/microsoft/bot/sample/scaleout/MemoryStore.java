// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.scaleout;

import com.fasterxml.jackson.databind.JsonNode;
import com.microsoft.bot.schema.Pair;

import java.util.HashMap;
import java.util.Map;
import java.util.UUID;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.Semaphore;

/**
 * A thread safe implementation of the IStore abstraction intended for testing.
 */
public class MemoryStore implements Store {

    private Map<String, Pair<JsonNode, String>> store = new HashMap<>();

    // When setting up the database, calls are made to CosmosDB. If multiple calls are made, we'll end up setting the
    // collectionLink member variable more than once. The semaphore is for making sure the initialization of the
    // database is done only once.
    private static final Semaphore SEMAPHORE = new Semaphore(1);

    /**
     * {@inheritDoc}
     */
    @Override
    public CompletableFuture<Pair<JsonNode, String>> load(String key) {
        try {
            SEMAPHORE.acquire();

            Pair<JsonNode, String> value = store.get(key);
            if (value != null) {
                return CompletableFuture.completedFuture(value);
            }
            return CompletableFuture.completedFuture(new Pair<>(null, null));
        } catch (InterruptedException e) {
            e.printStackTrace();
            return CompletableFuture.completedFuture(new Pair<>(null, null));
        } finally {
            SEMAPHORE.release();
        }
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public CompletableFuture<Boolean> save(String key, JsonNode content, String eTag) {
        try {
            SEMAPHORE.acquire();
            Pair<JsonNode, String> value = store.get(key);
            if (eTag != null && value != null) {
                if (eTag != value.getRight()) {
                    return CompletableFuture.completedFuture(false);
                }
            }

            store.put(key, new Pair<>(content, UUID.randomUUID().toString()));
            return CompletableFuture.completedFuture(true);
        } catch (InterruptedException e) {
            e.printStackTrace();
            return CompletableFuture.completedFuture(false);
        } finally {
            SEMAPHORE.release();
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.messagereaction;

import com.microsoft.bot.builder.Storage;
import com.microsoft.bot.schema.Activity;
import java.util.HashMap;
import java.util.Map;
import java.util.concurrent.CompletableFuture;

/**
 * `Activity`'s class with a Storage provider.
 */
public class ActivityLog {

    private Storage storage;

    /**
     * Initializes a new instance of the [ActivityLog](xref:reactions-bot.ActivityLog) class.
     *
     * @param withStorage A storage provider that stores and retrieves plain old JSON objects.
     */
    public ActivityLog(Storage withStorage) {
        storage = withStorage;
    }

    /**
     * Saves an {@link Activity} with its associated id into the storage.
     *
     * @param activityId {@link Activity}'s Id.
     * @param activity The {@link Activity} object.
     * @return A CompletableFuture
     */
    public CompletableFuture<Void> append(String activityId, Activity activity) {
        if (activityId == null) {
            throw new IllegalArgumentException("activityId");
        }

        if (activity == null) {
            throw new IllegalArgumentException("activity");
        }

        Map<String, Object> dictionary = new HashMap<String, Object>();
        dictionary.put(activityId, activity);
        return storage.write((Map<String, Object>) dictionary);
    }

    /**
     * Retrieves an {@link Activity} from the storage by a given Id.
     *
     * @param activityId {@link Activity}'s Id.
     * @return The {@link Activity}'s object retrieved from storage.
     */
    public CompletableFuture<Activity> find(String activityId) {
        if (activityId == null) {
            throw new IllegalArgumentException("activityId");
        }

        return storage.read(new String[]{activityId})
            .thenApply(activitiesResult ->
                activitiesResult.size() >= 1 ? ((Activity) activitiesResult.get(activityId)) : null);
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.scaleout;

import com.microsoft.bot.builder.BotAdapter;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.ConversationReference;
import com.microsoft.bot.schema.ResourceResponse;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.CompletableFuture;

/**
 * This custom BotAdapter supports scenarios that only Send Activities. Update and Delete Activity
 * are not supported.
 * Rather than sending the outbound Activities directly as the BotFrameworkAdapter does this class
 * buffers them in a list. The list is exposed as a public property.
 */
public class DialogHostAdapter extends BotAdapter {

    private List<Activity> response = new ArrayList<Activity>();

    /**
     * Gets the response object.
     * @return The response object.
     */
    public List<Activity> getResponses() {
        return response;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public CompletableFuture<ResourceResponse[]> sendActivities(TurnContext turnContext, List<Activity> activities) {
        for (Activity activity: activities) {
            response.add(activity);
        }

        return CompletableFuture.completedFuture(new ResourceResponse[0]);
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public CompletableFuture<Void> deleteActivity(TurnContext context, ConversationReference reference) {
        throw new UnsupportedOperationException();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public CompletableFuture<ResourceResponse> updateActivity(TurnContext context, Activity activity) {
        throw new UnsupportedOperationException();
    }
}

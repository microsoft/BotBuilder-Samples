// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.scaleout;

import com.fasterxml.jackson.databind.JsonNode;
import com.microsoft.bot.builder.ActivityHandler;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.dialogs.Dialog;
import com.microsoft.bot.schema.Pair;

import java.util.concurrent.CompletableFuture;
import java.util.concurrent.CompletionException;

/**
 * Represents a bot that processes incoming Activities.
 * For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
 * @param <T> is a Dialog.
 */
public class ScaleoutBot<T extends Dialog> extends ActivityHandler {

    private final Store store;
    private final Dialog dialog;

    /**
     * Initializes a new instance of the {@link ScaleoutBot} class.
     * @param withStore The store we will be using.
     * @param withDialog The root dialog to run.
     */
    public ScaleoutBot(Store withStore, T withDialog) {
        if (withStore == null) {
            throw new IllegalArgumentException("withStore can't be null");
        }
        store = withStore;

        if (withDialog == null) {
            throw new IllegalArgumentException("withDialog can't be null");
        }
        dialog = withDialog;
    }

    /**
     * This bot runs Dialogs that send message Activities in a way that can be scaled out
     * with a multi-machine deployment.
     * The bot logic makes use of the standard HTTP ETag/If-Match mechanism for optimistic locking. This mechanism
     * is commonly supported on cloud storage technologies from multiple vendors including teh Azure Blob Storage
     * service. A full implementation against Azure Blob Storage is included in this sample.
     *
     * @param turnContext The ITurnContext object created by the integration layer.
     * @return A task.
     */
    @Override
    protected CompletableFuture<Void> onMessageActivity(TurnContext turnContext) {
        String key = null;
        if (turnContext.getActivity().getConversation() != null) {
            // Create the storage key for this conversation.
            key = String.format(
                "%s/conversations/%s",
                turnContext.getActivity().getChannelId(),
                turnContext.getActivity().getConversation().getId());
        }

        final Boolean[] shouldBreak = {false};
        String finalKey = key;
        // The execution sits in a loop because there might be a retry if the save operation fails.
        while (true) {
            // Load any existing state associated with this key
            CompletableFuture<Pair<JsonNode, String>> saveTask = store.load(finalKey).thenCompose(pairOldState -> {
                // Run the dialog system with the old state and inbound activity,
                // the result is a new state and outbound activities.
                return DialogHost.run(dialog, turnContext.getActivity(), pairOldState.getLeft())
                    .thenCompose(pairNewState -> {
                        // Save the updated state associated with this key.
                        return store.save(finalKey, pairNewState.getRight(), pairOldState.getRight())
                            .thenCompose(success -> {
                                // Following a successful save, send any outbound Activities,
                                // otherwise retry everything.
                                if (success) {
                                    if (pairNewState.getLeft().length > 0) {
                                        // This is an actual send on the TurnContext we were given
                                        // and so will actual do a send this time.
                                        turnContext.sendActivities(pairNewState.getLeft())
                                            .thenApply(result -> null);
                                    }
                                    shouldBreak[0] = true;
                                }
                                return CompletableFuture.completedFuture(null);
                            });
                    });
            });
            if (saveTask.isCompletedExceptionally()) {
                throw new CompletionException(new Exception());
            }

            saveTask.join();
            if (shouldBreak[0]) {
                break;
            }
        }
        return CompletableFuture.completedFuture(null);
    }
}

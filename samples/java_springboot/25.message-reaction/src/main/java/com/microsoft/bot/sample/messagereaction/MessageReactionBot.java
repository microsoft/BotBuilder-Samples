// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.messagereaction;

import com.microsoft.bot.builder.ActivityHandler;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.MessageReaction;
import java.util.List;
import java.util.concurrent.CompletableFuture;

/**
 * From the UI you need to @mention the bot, then add a message reaction to the message the bot sent.
 */
public class MessageReactionBot extends ActivityHandler {

    private final ActivityLog log;

    /**
     * Initializes a new instance of the {@link MessageReactionBot} class.
     *
     * @param withLog The {@link ActivityLog}.
     */
    public MessageReactionBot(ActivityLog withLog) {
        log = withLog;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    protected CompletableFuture<Void> onMessageActivity(TurnContext turnContext) {
        return sendMessageAndLogActivityId(turnContext, String.format("echo: %s", turnContext.getActivity().getText()));
    }

    /**
     * {@inheritDoc}
     */
    @Override
    protected CompletableFuture<Void> onReactionsAdded(
        List<MessageReaction> messageReactions,
        TurnContext turnContext
    ) {
        for (MessageReaction reaction : messageReactions) {
            // The ReplyToId property of the inbound MessageReaction Activity will correspond to a Message Activity
            // which had previously been sent from this bot.
            log.find(turnContext.getActivity().getReplyToId()).thenCompose(resultActivity -> {
                if (resultActivity == null) {
                    // If we had sent the message from the error handler
                    // we wouldn't have recorded the Activity Id and so we shouldn't expect to see it in the log.
                    return sendMessageAndLogActivityId(
                        turnContext,
                        String.format("Activity %s not found in the log.", turnContext.getActivity().getReplyToId()));
                }

                return sendMessageAndLogActivityId(
                    turnContext,
                    String.format("You added '%s' regarding '%s'", reaction.getType(), resultActivity.getText()));
            });
        }

        return CompletableFuture.completedFuture(null);
    }

    /**
     * {@inheritDoc}
     */
    @Override
    protected CompletableFuture<Void> onReactionsRemoved(
        List<MessageReaction> messageReactions,
        TurnContext turnContext
    ) {
        for (MessageReaction reaction : messageReactions) {
            // The ReplyToId property of the inbound MessageReaction Activity will correspond to a Message Activity
            // which was previously sent from this bot.
            log.find(turnContext.getActivity().getReplyToId()).thenCompose(resultActivity -> {
                if (resultActivity == null) {
                    // If we had sent the message from the error handler
                    // we wouldn't have recorded the Activity Id and so we shouldn't expect to see it in the log.
                    return sendMessageAndLogActivityId(
                        turnContext,
                        String.format("Activity %s not found in the log.", turnContext.getActivity().getReplyToId()));
                }

                return sendMessageAndLogActivityId(
                    turnContext,
                    String.format("You removed '%s' regarding '%s'", reaction.getType(), resultActivity.getText()));
            });
        }

        return CompletableFuture.completedFuture(null);
    }

    /**
     * Sends an activity reply.
     *
     * @param context The {@link TurnContext}.
     * @param text The {@link Activity}'s text.
     */
    private CompletableFuture<Void> sendMessageAndLogActivityId(TurnContext turnContext, String text) {
        // We need to record the Activity Id from the Activity just sent
        // in order to understand what the reaction is a reaction too.
        Activity replyActivity = MessageFactory.text(text);
        return turnContext.sendActivity(replyActivity)
            .thenCompose(resourceResponse -> log.append(resourceResponse.getId(), replyActivity)
            .thenApply(result -> null));
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.qnamaker.all.features;

import com.microsoft.bot.builder.ActivityHandler;
import com.microsoft.bot.builder.BotState;
import com.microsoft.bot.builder.ConversationState;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.UserState;
import com.microsoft.bot.dialogs.Dialog;
import com.microsoft.bot.schema.ChannelAccount;

import java.util.List;
import java.util.concurrent.CompletableFuture;

/**
 * A simple bot that responds to utterances with answers from QnA Maker.
 * If an answer is not found for an utterance, the bot responds with help.
 *
 * @param <T> A {@link Dialog}
 */
public class QnABot<T extends Dialog> extends ActivityHandler {

    private final BotState conversationState;
    private final Dialog dialog;
    private final BotState userState;

    /**
     * Initializes a new instance of the {@link QnABot} class.
     *
     * @param withConversationState A {@link ConversationState}
     * @param withUserState A {@link UserState}
     * @param withDialog A {@link Dialog}
     */
    public QnABot(ConversationState withConversationState, UserState withUserState, T withDialog) {
        this.conversationState = withConversationState;
        this.userState = withUserState;
        this.dialog = withDialog;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public CompletableFuture<Void> onTurn(TurnContext turnContext) {
        return super.onTurn(turnContext)
            // Save any state changes that might have occurred during the turn.
            .thenCompose(turnResult -> conversationState.saveChanges(turnContext, false))
            .thenCompose(saveResult -> userState.saveChanges(turnContext, false));
    }

    /**
     * {@inheritDoc}
     */
    @Override
    protected CompletableFuture<Void> onMessageActivity(TurnContext turnContext) {
        // Run the Dialog with the new message Activity.
        return Dialog.run(
            this.dialog,
            turnContext,
            this.conversationState.createProperty("DialogState"));
    }

    /**
     * {@inheritDoc}
     */
    @Override
    protected CompletableFuture<Void> onMembersAdded(List<ChannelAccount> membersAdded, TurnContext turnContext) {
        for (ChannelAccount member: membersAdded) {
            if (!member.getId().equals(turnContext.getActivity().getRecipient().getId())) {
                turnContext.sendActivity(MessageFactory.text("Hello and welcome!")).thenApply(result -> null);
            }
        }

        return CompletableFuture.completedFuture(null);
    }
}

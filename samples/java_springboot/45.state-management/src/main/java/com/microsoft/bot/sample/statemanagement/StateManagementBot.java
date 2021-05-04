// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.statemanagement;

import com.codepoetics.protonpack.collectors.CompletableFutures;
import com.microsoft.bot.builder.ActivityHandler;
import com.microsoft.bot.builder.ConversationState;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.StatePropertyAccessor;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.UserState;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.ChannelAccount;
import org.apache.commons.lang3.StringUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.time.LocalDateTime;
import java.time.OffsetDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.CompletableFuture;

/**
 * This class implements the functionality of the Bot.
 *
 * <p>
 * This is where application specific logic for interacting with the users would
 * be added. This class tracks the conversation state through POJO's saved in
 * {@link ConversationState} and {@link UserState}.
 * </p>
 *
 * @see ConversationData
 * @see UserProfile
 */
public class StateManagementBot extends ActivityHandler {
    private ConversationState conversationState;
    private UserState userState;

    @Autowired
    public StateManagementBot(ConversationState withConversationState, UserState withUserState) {
        conversationState = withConversationState;
        userState = withUserState;
    }

    /**
     * Normal onTurn processing, with saving of state after each turn.
     *
     * @param turnContext The context object for this turn. Provides information
     *                    about the incoming activity, and other data needed to
     *                    process the activity.
     * @return A future task.
     */
    @Override
    public CompletableFuture<Void> onTurn(TurnContext turnContext) {
        return super.onTurn(turnContext)
            .thenCompose(turnResult -> conversationState.saveChanges(turnContext))
            .thenCompose(saveResult -> userState.saveChanges(turnContext));
    }

    /**
     * Send a welcome message to new members.
     *
     * @param membersAdded A list of all the members added to the conversation, as
     *                     described by the conversation update activity.
     * @param turnContext  The context object for this turn.
     * @return A future task.
     */
    @Override
    protected CompletableFuture<Void> onMembersAdded(
        List<ChannelAccount> membersAdded,
        TurnContext turnContext
    ) {
        return membersAdded.stream()
            .filter(
                member -> !StringUtils
                    .equals(member.getId(), turnContext.getActivity().getRecipient().getId())
            )
            .map(
                channel -> turnContext
                    .sendActivity(
                        MessageFactory
                            .text("Welcome to State Bot Sample. Type anything to get started.")
                    )
            )
            .collect(CompletableFutures.toFutureList())
            .thenApply(resourceResponses -> null);
    }

    /**
     * This will prompt for a user name, after which it will send info about the
     * conversation. After sending information, the cycle restarts.
     *
     * @param turnContext The context object for this turn.
     * @return A future task.
     */
    @Override
    protected CompletableFuture<Void> onMessageActivity(TurnContext turnContext) {
        // Get state data from ConversationState.
        StatePropertyAccessor<ConversationData> dataAccessor =
            conversationState.createProperty("data");
        CompletableFuture<ConversationData> dataFuture =
            dataAccessor.get(turnContext, ConversationData::new);

        // Get profile from UserState.
        StatePropertyAccessor<UserProfile> profileAccessor = userState.createProperty("profile");
        CompletableFuture<UserProfile> profileFuture =
            profileAccessor.get(turnContext, UserProfile::new);

        return dataFuture.thenCombine(profileFuture, (conversationData, userProfile) -> {
            if (StringUtils.isEmpty(userProfile.getName())) {
                if (conversationData.getPromptedUserForName()) {
                    // Reset the flag to allow the bot to go though the cycle again.
                    conversationData.setPromptedUserForName(false);

                    // Set the name to what the user provided and reply.
                    userProfile.setName(turnContext.getActivity().getText());
                    return turnContext.sendActivity(
                        MessageFactory.text(
                            "Thanks " + userProfile.getName()
                                + ".  To see conversation data, type anything."
                        )
                    );
                } else {
                    conversationData.setPromptedUserForName(true);
                    return turnContext.sendActivity(MessageFactory.text("What is your name?"));
                }
            } else {
                // Set the flag to true, so we don't prompt in the next turn.
                conversationData.setPromptedUserForName(true);

                OffsetDateTime messageTimeOffset = turnContext.getActivity().getTimestamp();
                LocalDateTime localMessageTime = messageTimeOffset.toLocalDateTime();
                conversationData.setTimestamp(localMessageTime.toString());
                conversationData.setChannelId(turnContext.getActivity().getChannelId());

                List<Activity> sendToUser = new ArrayList<>();

                sendToUser.add(
                    MessageFactory.text(
                        userProfile.getName() + " sent: " + turnContext.getActivity().getText()
                    )
                );

                sendToUser.add(
                    MessageFactory.text(
                        userProfile.getName() + " message received at: "
                            + conversationData.getTimestamp()
                    )
                );

                sendToUser.add(
                    MessageFactory.text(
                        userProfile.getName() + " message received from: "
                            + conversationData.getChannelId()
                    )
                );

                return turnContext.sendActivities(sendToUser);
            }
        })
            // make the return value happy.
            .thenApply(resourceResponse -> null);
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.inspection;

import com.codepoetics.protonpack.collectors.CompletableFutures;
import com.microsoft.bot.builder.*;
import com.microsoft.bot.schema.ChannelAccount;
import org.apache.commons.lang3.StringUtils;
import org.springframework.stereotype.Component;

import java.util.List;
import java.util.concurrent.CompletableFuture;

/**
 * This class implements the functionality of the Bot.
 *
 * <p>
 * This is where application specific logic for interacting with the users would
 * be added. For this sample, the {@link #onMessageActivity(TurnContext)} echos
 * the text back to the user. The {@link #onMembersAdded(List, TurnContext)}
 * will send a greeting to new conversation participants.
 * </p>
 *
 * <p>
 * See README.md for details on using the InspectionMiddleware.
 * </p>
 */
public class EchoBot extends ActivityHandler {
    private ConversationState conversationState;
    private UserState userState;

    public EchoBot(ConversationState withConversationState, UserState withUserState) {
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

    @Override
    protected CompletableFuture<Void> onMessageActivity(TurnContext turnContext) {
        // Get state data from ConversationState.
        StatePropertyAccessor<CustomState> dataAccessor =
            conversationState.createProperty("customState");
        CompletableFuture<CustomState> convStateFuture =
            dataAccessor.get(turnContext, CustomState::new);

        // Get profile from UserState.
        StatePropertyAccessor<CustomState> profileAccessor =
            userState.createProperty("customState");
        CompletableFuture<CustomState> userStateFuture =
            profileAccessor.get(turnContext, CustomState::new);

        return convStateFuture.thenCombine(userStateFuture, (convProp, userProp) -> {
            convProp.setValue(convProp.getValue() + 1);
            userProp.setValue(userProp.getValue() + 1);

            return turnContext.sendActivity(
                MessageFactory.text(
                    String.format(
                        "Echo: %s conversation state %d user state %d",
                        turnContext.getActivity().getText(), convProp.getValue(),
                        userProp.getValue()
                    )
                )
            );
        }).thenApply(resourceResponse -> null);
    }

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
            .map(channel -> turnContext.sendActivity(MessageFactory.text("Hello and welcome!")))
            .collect(CompletableFutures.toFutureList())
            .thenApply(resourceResponses -> null);
    }
}

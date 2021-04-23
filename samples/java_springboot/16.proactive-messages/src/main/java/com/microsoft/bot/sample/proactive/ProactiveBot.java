// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.proactive;

import com.codepoetics.protonpack.collectors.CompletableFutures;
import com.microsoft.bot.builder.ActivityHandler;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.ChannelAccount;
import com.microsoft.bot.schema.ConversationReference;
import org.apache.commons.lang3.StringUtils;
import org.springframework.beans.factory.annotation.Value;

import java.util.List;
import java.util.concurrent.CompletableFuture;

/**
 * This class implements the functionality of the Bot.
 *
 * <p>
 * This is where application specific logic for interacting with the users would
 * be added. For this sample, the {@link #onMessageActivity(TurnContext)} echos
 * the text back to the user and updates the shared
 * {@link ConversationReferences}. The
 * {@link #onMembersAdded(List, TurnContext)} will send a greeting to new
 * conversation participants with instructions for sending a proactive message.
 * </p>
 */
public class ProactiveBot extends ActivityHandler {
    @Value("${server.port:3978}")
    private int port;

    // Message to send to users when the bot receives a Conversation Update event
    private final String WELCOMEMESSAGE =
        "Welcome to the Proactive Bot sample.  Navigate to http://localhost:%d/api/notify to proactively message everyone who has previously messaged this bot.";

    private ConversationReferences conversationReferences;

    public ProactiveBot(ConversationReferences withReferences) {
        conversationReferences = withReferences;
    }

    @Override
    protected CompletableFuture<Void> onMessageActivity(TurnContext turnContext) {
        addConversationReference(turnContext.getActivity());

        // Echo back what the user said
        return turnContext
            .sendActivity(MessageFactory.text(String.format("You sent '%s'", turnContext.getActivity().getText())))
            .thenApply(sendResult -> null);
    }

    @Override
    protected CompletableFuture<Void> onMembersAdded(
        List<ChannelAccount> membersAdded,
        TurnContext turnContext
    ) {
        return membersAdded.stream()
            .filter(
                // Greet anyone that was not the target (recipient) of this message.
                member -> !StringUtils
                    .equals(member.getId(), turnContext.getActivity().getRecipient().getId())
            )
            .map(
                channel -> turnContext
                    .sendActivity(MessageFactory.text(String.format(WELCOMEMESSAGE, port)))
            )
            .collect(CompletableFutures.toFutureList())
            .thenApply(resourceResponses -> null);
    }

    @Override
    protected CompletableFuture<Void> onConversationUpdateActivity(TurnContext turnContext) {
        addConversationReference(turnContext.getActivity());
        return super.onConversationUpdateActivity(turnContext);
    }

    // adds a ConversationReference to the shared Map.
    private void addConversationReference(Activity activity) {
        ConversationReference conversationReference = activity.getConversationReference();
        conversationReferences.put(conversationReference.getUser().getId(), conversationReference);
    }
}

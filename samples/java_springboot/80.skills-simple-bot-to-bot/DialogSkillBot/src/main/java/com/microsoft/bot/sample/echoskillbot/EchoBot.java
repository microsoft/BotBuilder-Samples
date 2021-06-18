// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.echoskillbot;

import com.microsoft.bot.builder.ActivityHandler;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.EndOfConversationCodes;
import com.microsoft.bot.schema.InputHints;

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
 */
public class EchoBot extends ActivityHandler {

    @Override
    protected CompletableFuture<Void> onMessageActivity(TurnContext turnContext) {
        if (
            turnContext.getActivity().getText().contains("end") || turnContext.getActivity().getText().contains("stop")
        ) {
            String messageText = "ending conversation from the skill...";
            return turnContext.sendActivity(MessageFactory.text(messageText, messageText, InputHints.IGNORING_INPUT))
                .thenApply(result -> {
                    Activity endOfConversation = Activity.createEndOfConversationActivity();
                    endOfConversation.setCode(EndOfConversationCodes.COMPLETED_SUCCESSFULLY);
                    return turnContext.sendActivity(endOfConversation);
                })
                .thenApply(finalResult -> null);
        } else {
            String messageText = String.format("Echo: %s", turnContext.getActivity().getText());
            return turnContext.sendActivity(MessageFactory.text(messageText, messageText, InputHints.IGNORING_INPUT))
                .thenApply(result -> {
                    String nextMessageText =
                        "Say \"end\" or \"stop\" and I'll end the conversation and back to the parent.";
                    return turnContext.sendActivity(
                        MessageFactory.text(nextMessageText, nextMessageText, InputHints.EXPECTING_INPUT)
                    );
                })
                .thenApply(result -> null);
        }
    }
}

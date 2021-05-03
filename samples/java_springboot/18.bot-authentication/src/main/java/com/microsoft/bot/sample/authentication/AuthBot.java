// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.authentication;

import java.util.concurrent.CompletableFuture;
import java.util.List;

import com.microsoft.bot.builder.ConversationState;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.UserState;
import com.microsoft.bot.dialogs.Dialog;
import com.microsoft.bot.schema.ChannelAccount;

import com.codepoetics.protonpack.collectors.CompletableFutures;
import com.microsoft.bot.schema.Activity;
import org.apache.commons.lang3.StringUtils;
import org.slf4j.LoggerFactory;

public class AuthBot extends DialogBot<MainDialog> {

    public AuthBot(ConversationState conversationState, UserState userState, MainDialog dialog) {
        super(conversationState, userState, dialog);
    }

    @Override
    protected CompletableFuture<Void> onMembersAdded(
        List<ChannelAccount> membersAdded, TurnContext turnContext
    ) {
        return turnContext.getActivity().getMembersAdded().stream()
            .filter(member -> !StringUtils
                .equals(member.getId(), turnContext.getActivity().getRecipient().getId()))
            .map(channel -> {
                Activity reply = MessageFactory.text("Welcome to AuthBot."
                    + " Type anything to get logged in. Type 'logout' to sign-out.");

                return turnContext.sendActivity(reply);
            })
            .collect(CompletableFutures.toFutureList())
            .thenApply(resourceResponse -> null);
    }

    @Override
    protected CompletableFuture<Void> onTokenResponseEvent(TurnContext turnContext) {
        LoggerFactory.getLogger(AuthBot.class).info("Running dialog with Token Response Event Activity.");

        // Run the Dialog with the new Token Response Event Activity.
        return Dialog.run(dialog, turnContext, conversationState.createProperty("DialogState"));
    }

}

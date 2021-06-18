// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.teamsauth;

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

// This bot is derived (view DialogBot<T>) from the TeamsActivityHandler class
// currently included as part of this sample.
public class TeamsBot<T extends Dialog> extends DialogBot {

    public TeamsBot(ConversationState conversationState, UserState userState, T dialog) {
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
                Activity reply = MessageFactory.text("Welcome to AuthenticationBot."
                    + " Type anything to get logged in. Type 'logout' to sign-out.");

                return turnContext.sendActivity(reply);
            })
            .collect(CompletableFutures.toFutureList())
            .thenApply(resourceResponse -> null);
    }

    @Override
    protected CompletableFuture<Void> onTeamsSigninVerifyState(TurnContext turnContext) {
        LoggerFactory.getLogger(TeamsBot.class).info("Running dialog with signin/verifystate from an Invoke Activity.");

        // The OAuth Prompt needs to see the Invoke Activity in order to complete the login process.

        // Run the Dialog with the new Token Response Event Activity.
         return Dialog.run(dialog, turnContext, conversationState.createProperty("DialogState"));
    }

}

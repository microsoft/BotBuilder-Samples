// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.usingcards;

import com.codepoetics.protonpack.collectors.CompletableFutures;
import com.microsoft.bot.builder.ConversationState;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.UserState;
import com.microsoft.bot.dialogs.Dialog;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.ChannelAccount;
import java.util.List;
import java.util.concurrent.CompletableFuture;
import org.apache.commons.lang3.StringUtils;

// RichCardsBot prompts a user to select a Rich Card and then returns the card
// that matches the user's selection.
public class RichCardsBot extends DialogBot<Dialog> {

    public RichCardsBot(
        ConversationState withConversationState,
        UserState withUserState,
        Dialog withDialog
    ) {
        super(withConversationState, withUserState, withDialog);
    }

    @Override
    protected CompletableFuture<Void> onMembersAdded(
        List<ChannelAccount> membersAdded, TurnContext turnContext
    ) {
        return turnContext.getActivity().getMembersAdded().stream()
            .filter(member -> !StringUtils
                .equals(member.getId(), turnContext.getActivity().getRecipient().getId()))
            .map(channel -> {
                Activity reply = MessageFactory.text("Welcome to CardBot."
                    + " This bot will show you different types of Rich Cards."
                    + " Please type anything to get started.");

                return turnContext.sendActivity(reply);
            })
            .collect(CompletableFutures.toFutureList())
            .thenApply(resourceResponse -> null);
    }
}

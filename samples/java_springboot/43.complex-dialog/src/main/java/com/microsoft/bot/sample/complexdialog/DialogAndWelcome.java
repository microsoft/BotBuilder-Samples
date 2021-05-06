// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.complexdialog;

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

public class DialogAndWelcome<T extends Dialog> extends DialogBot {

    public DialogAndWelcome(
        ConversationState withConversationState,
        UserState withUserState,
        T withDialog
    ) {
        super(withConversationState, withUserState, withDialog);
    }

    @Override
    protected CompletableFuture<Void> onMembersAdded(
        List<ChannelAccount> membersAdded, TurnContext turnContext
    ) {
        // Greet anyone that was not the target (recipient) of this message.
        // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
        return turnContext.getActivity().getMembersAdded().stream()
            .filter(member -> !StringUtils
                .equals(member.getId(), turnContext.getActivity().getRecipient().getId()))
            .map(channel -> {
                Activity reply = MessageFactory.text("Welcome to Complex Dialog Bot User. "
                    + "This bot provides a complex conversation, with multiple dialogs. "
                    + "Type anything to get started.");

                return turnContext.sendActivity(reply);
            })
            .collect(CompletableFutures.toFutureList())
            .thenApply(resourceResponse -> null);
    }
}

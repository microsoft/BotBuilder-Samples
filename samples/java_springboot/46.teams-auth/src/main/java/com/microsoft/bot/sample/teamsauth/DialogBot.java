// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.teamsauth;

import com.microsoft.bot.builder.BotState;
import com.microsoft.bot.builder.ConversationState;
import com.microsoft.bot.builder.teams.TeamsActivityHandler;
import com.microsoft.bot.dialogs.Dialog;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.UserState;
import java.util.concurrent.CompletableFuture;

/**
 * This Bot implementation can run any type of Dialog. The use of type parameterization is to
 * allows multiple different bots to be run at different endpoints within the same project. This
 * can be achieved by defining distinct Controller types each with dependency on distinct IBot
 * types, this way ASP Dependency Injection can glue everything together without ambiguity. The
 * ConversationState is used by the Dialog system. The UserState isn't, however, it might have
 * been used in a Dialog implementation, and the requirement is that all BotState objects are
 * saved at the end of a turn.
 */
public class DialogBot<T extends Dialog> extends TeamsActivityHandler {
    protected Dialog dialog;
    protected BotState conversationState;
    protected BotState userState;

    public DialogBot(
        ConversationState withConversationState,
        UserState withUserState,
        T withDialog
    ) {
        dialog = withDialog;
        conversationState = withConversationState;
        userState = withUserState;
    }

    @Override
    public CompletableFuture<Void> onTurn(
        TurnContext turnContext
    ) {
        return super.onTurn(turnContext)
            .thenCompose(result -> conversationState.saveChanges(turnContext))
            .thenCompose(result -> userState.saveChanges(turnContext));
    }

    @Override
    protected CompletableFuture<Void> onMessageActivity(
        TurnContext turnContext
    ) {
        return Dialog.run(dialog, turnContext, conversationState.createProperty("DialogState"));
    }
}

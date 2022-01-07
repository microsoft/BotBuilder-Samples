// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.dialogskillbot.bots;

import java.util.concurrent.CompletableFuture;

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import com.microsoft.bot.builder.ActivityHandler;
import com.microsoft.bot.builder.ConversationState;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.dialogs.Dialog;

public class SkillBot<T extends Dialog> extends ActivityHandler {

    private ConversationState conversationState;
    private Dialog dialog;

    public SkillBot(ConversationState conversationState, T mainDialog) {
        this.conversationState = conversationState;
        this.dialog = mainDialog;
    }

    @Override
    public CompletableFuture<Void> onTurn(TurnContext turnContext) {
        return Dialog.run(dialog, turnContext, conversationState.createProperty("DialogState"))
        .thenAccept(result -> {
            conversationState.saveChanges(turnContext, false);
        });
    }

}

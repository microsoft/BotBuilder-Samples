// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.corebot.app.insights;

import com.microsoft.bot.builder.ActivityHandler;
import com.microsoft.bot.builder.BotState;
import com.microsoft.bot.builder.ConversationState;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.UserState;
import com.microsoft.bot.dialogs.Dialog;
import org.slf4j.LoggerFactory;

import java.util.concurrent.CompletableFuture;

/**
 * This Bot implementation can run any type of Dialog. The use of type parameterization is to allow
 * multiple different bots to be run at different endpoints within the same project. This can be
 * achieved by defining distinct Controller types each with dependency on distinct Bot types. The
 * ConversationState is used by the Dialog system. The UserState isn't, however, it might have been
 * used in a Dialog implementation, and the requirement is that all BotState objects are saved at
 * the end of a turn.
 *
 * @param <T> parameter of a type inheriting from Dialog
 */
public class DialogBot<T extends Dialog> extends ActivityHandler {

    private final Dialog dialog;
    private final BotState conversationState;
    private final BotState userState;

    /**
     * Gets the dialog in use.
     *
     * @return instance of dialog
     */
    protected Dialog getDialog() {
        return dialog;
    }

    /**
     * Gets the conversation state.
     *
     * @return instance of conversationState
     */
    protected BotState getConversationState() {
        return conversationState;
    }

    /**
     * Gets the user state.
     *
     * @return instance of userState
     */
    protected BotState getUserState() {
        return userState;
    }

    /**
     * Creates a DialogBot.
     *
     * @param withConversationState ConversationState to use in the bot
     * @param withUserState         UserState to use
     * @param withDialog            Param inheriting from Dialog class
     */
    public DialogBot(
        ConversationState withConversationState, UserState withUserState, T withDialog
    ) {
        this.conversationState = withConversationState;
        this.userState = withUserState;
        this.dialog = withDialog;
    }

    /**
     * Saves the BotState objects at the end of each turn.
     *
     * @param turnContext
     * @return
     */
    @Override
    public CompletableFuture<Void> onTurn(TurnContext turnContext) {
        return super.onTurn(turnContext)
            .thenCompose(turnResult -> conversationState.saveChanges(turnContext, false))
            .thenCompose(saveResult -> userState.saveChanges(turnContext, false));
    }

    /**
     * This method is executed when the turnContext receives a message activity.
     *
     * @param turnContext
     * @return
     */
    @Override
    protected CompletableFuture<Void> onMessageActivity(TurnContext turnContext) {
        LoggerFactory.getLogger(DialogBot.class).info("Running dialog with Message Activity.");

        // Run the Dialog with the new message Activity.
        return Dialog.run(dialog, turnContext, conversationState.createProperty("DialogState"));
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.teamsauth;

import java.util.concurrent.CompletableFuture;

import com.microsoft.bot.builder.BotFrameworkAdapter;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.dialogs.ComponentDialog;
import com.microsoft.bot.dialogs.DialogContext;
import com.microsoft.bot.dialogs.DialogTurnResult;
import com.microsoft.bot.schema.ActivityTypes;

public class LogoutDialog extends ComponentDialog {

    private final String connectionName;

    public LogoutDialog(String id, String connectionName) {
        super(id);
        this.connectionName = connectionName;
    }


    @Override
    protected CompletableFuture<DialogTurnResult> onBeginDialog(DialogContext innerDc, Object options) {
        return interrupt(innerDc).thenCompose(result -> {
            if (result != null) {
                return CompletableFuture.completedFuture(result);
            }

            return super.onBeginDialog(innerDc, options);
        });
    }

    @Override
    protected CompletableFuture<DialogTurnResult> onContinueDialog(DialogContext innerDc) {
        return interrupt(innerDc).thenCompose(result -> {
            if (result != null) {
                return CompletableFuture.completedFuture(result);
            }

            return super.onContinueDialog(innerDc);
        });
    }

    private CompletableFuture<DialogTurnResult> interrupt(DialogContext innerDc) {
        if (innerDc.getContext().getActivity().getType().equals(ActivityTypes.MESSAGE)) {
            String text = innerDc.getContext().getActivity().getText().toLowerCase();

            // Allow logout anywhere in the command
            if (text.contains("logout")) {
                // The bot adapter encapsulates the authentication processes.
                BotFrameworkAdapter botAdapter = (BotFrameworkAdapter) innerDc.getContext().getAdapter();
                botAdapter.signOutUser(
                        innerDc.getContext(),
                        getConnectionName(),
                        innerDc.getContext().getActivity().getFrom().getId())
                        .thenApply(result -> innerDc.getContext().sendActivity(MessageFactory.text("You have been signed out."))
                                .thenApply(sendResult -> null));
                return innerDc.cancelAllDialogs();
            }
        }

        return CompletableFuture.completedFuture(null);
    }

    /**
     * @return the ConnectionName value as a String.
     */
    protected String getConnectionName() {
        return this.connectionName;
    }

}


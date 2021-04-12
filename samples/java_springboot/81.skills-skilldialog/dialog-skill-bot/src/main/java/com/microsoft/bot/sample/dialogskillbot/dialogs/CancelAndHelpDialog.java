// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.dialogskillbot.dialogs;

import java.util.concurrent.CompletableFuture;

import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.dialogs.ComponentDialog;
import com.microsoft.bot.dialogs.DialogContext;
import com.microsoft.bot.dialogs.DialogTurnResult;
import com.microsoft.bot.dialogs.DialogTurnStatus;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.ActivityTypes;
import com.microsoft.bot.schema.InputHints;

public class CancelAndHelpDialog extends ComponentDialog {

    private final String HelpMsgText = "Show help here";
    private final String CancelMsgText = "Canceling...";

    public CancelAndHelpDialog(String id) {
        super(id);
    }

    @Override
    protected CompletableFuture<DialogTurnResult> onContinueDialog(DialogContext innerDc) {
        return interrupt(innerDc).thenCompose(result -> {
            if (result != null && result instanceof DialogTurnResult) {
                return CompletableFuture.completedFuture((DialogTurnResult) result);
            }
            return super.onContinueDialog(innerDc);
        });
    }

    private CompletableFuture<DialogTurnResult> interrupt(DialogContext innerDc) {
        if (innerDc.getContext().getActivity().getType().equals(ActivityTypes.MESSAGE)) {
            String text = innerDc.getContext().getActivity().getText().toLowerCase();

            switch (text) {
                case "help":
                case "?":
                    Activity helpMessage = MessageFactory.text(HelpMsgText, HelpMsgText, InputHints.EXPECTING_INPUT);
                    return innerDc.getContext()
                        .sendActivity(helpMessage)
                        .thenCompose(
                            result -> CompletableFuture.completedFuture(new DialogTurnResult(DialogTurnStatus.WAITING))
                        );

                case "cancel":
                case "quit":
                    Activity cancelMessage =
                        MessageFactory.text(CancelMsgText, CancelMsgText, InputHints.IGNORING_INPUT);
                    return innerDc.getContext().sendActivity(cancelMessage).thenCompose(result -> {
                        return innerDc.cancelAllDialogs();
                    });

            }
        }
        return CompletableFuture.completedFuture(null);
    }
}

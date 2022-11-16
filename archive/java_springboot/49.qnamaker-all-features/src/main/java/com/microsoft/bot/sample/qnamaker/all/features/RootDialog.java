// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.qnamaker.all.features;

import com.microsoft.bot.dialogs.ComponentDialog;
import com.microsoft.bot.dialogs.DialogTurnResult;
import com.microsoft.bot.dialogs.WaterfallDialog;
import com.microsoft.bot.dialogs.WaterfallStep;
import com.microsoft.bot.dialogs.WaterfallStepContext;
import com.microsoft.bot.integration.Configuration;

import java.util.Arrays;
import java.util.concurrent.CompletableFuture;

/**
 * This is an example root dialog. Replace this with your applications.
 */
public class RootDialog extends ComponentDialog {

    /**
     * QnA Maker initial dialog.
     */
    private final String initialDialog = "initial-dialog";

    /**
     * Root dialog for this bot. Creates a QnAMakerDialog.
     *
     * @param services A {@link BotServices} which contains the applications
     * @param configuration A {@link Configuration} populated with the application.properties file
     */
    public RootDialog(BotServices services, Configuration configuration) {
        super("root");

        this.addDialog(new QnAMakerBaseDialog(services, configuration));

        WaterfallStep[] waterfallSteps = {
            this::initialStep
        };
        WaterfallDialog waterfallDialog = new WaterfallDialog(initialDialog, Arrays.asList(waterfallSteps));
        this.addDialog(waterfallDialog);

        // The initial child Dialog to run.
        this.setInitialDialogId(initialDialog);
    }

    /**
     * This is the first step of the WaterfallDialog.
     * It kicks off the dialog with the QnA Maker with provided options.
     *
     * @param stepContext A {@link WaterfallStepContext}
     * @return A {@link DialogTurnResult}
     */
    private CompletableFuture<DialogTurnResult> initialStep(WaterfallStepContext stepContext) {
        return stepContext.beginDialog("QnAMakerDialog", null);
    }
}

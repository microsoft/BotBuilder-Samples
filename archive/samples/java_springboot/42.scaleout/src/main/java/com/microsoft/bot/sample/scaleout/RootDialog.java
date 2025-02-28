// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.scaleout;

import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.dialogs.ComponentDialog;
import com.microsoft.bot.dialogs.DialogTurnResult;
import com.microsoft.bot.dialogs.WaterfallDialog;
import com.microsoft.bot.dialogs.WaterfallStep;
import com.microsoft.bot.dialogs.WaterfallStepContext;
import com.microsoft.bot.dialogs.prompts.NumberPrompt;
import com.microsoft.bot.dialogs.prompts.PromptOptions;

import java.util.Arrays;
import java.util.concurrent.CompletableFuture;

/**
 * This is an example root dialog. Replace this with your applications.
 */
public class RootDialog extends ComponentDialog {

    /**
     * The constructor of the {@link RootDialog} class, which creates the main dialogs.
     */
    public RootDialog() {
        super("root");
        addDialog(createWaterfall());
        addDialog(new NumberPrompt<Long>("number", Long.class));

        setInitialDialogId("waterfall");
    }

    private static WaterfallDialog createWaterfall() {
        WaterfallStep[] waterfallSteps = {
            RootDialog::step1,
            RootDialog::step2,
            RootDialog::step3
        };
        return new WaterfallDialog("waterfall", Arrays.asList(waterfallSteps));
    }

    private static CompletableFuture<DialogTurnResult> step1(WaterfallStepContext stepContext) {
        PromptOptions promptOptions = new PromptOptions();
        promptOptions.setPrompt(MessageFactory.text("Enter a number."));
        return stepContext.prompt("number", promptOptions);
    }

    private static CompletableFuture<DialogTurnResult> step2(WaterfallStepContext stepContext) {
        long first = (long) stepContext.getResult();
        stepContext.getValues().put("first", first);
        String text = String.format("I have %d now enter another number", first);
        PromptOptions promptOptions = new PromptOptions();
        promptOptions.setPrompt(MessageFactory.text(text));
        return stepContext.prompt("number", promptOptions);
    }

    private static CompletableFuture<DialogTurnResult> step3(WaterfallStepContext stepContext) {
        long first = (long) stepContext.getValues().get("first");
        long second = (long) stepContext.getResult();
        String text = String.format("The result of the first minus the second is %d.", first - second);
        stepContext.getContext().sendActivity(text)
            .thenApply(result -> CompletableFuture.completedFuture(null));
        return stepContext.endDialog(null);
    }
}

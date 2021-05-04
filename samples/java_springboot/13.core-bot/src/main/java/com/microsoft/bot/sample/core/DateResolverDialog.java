// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.core;

import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.dialogs.DialogTurnResult;
import com.microsoft.bot.dialogs.WaterfallDialog;
import com.microsoft.bot.dialogs.WaterfallStep;
import com.microsoft.bot.dialogs.WaterfallStepContext;
import com.microsoft.bot.dialogs.prompts.DateTimePrompt;
import com.microsoft.bot.dialogs.prompts.DateTimeResolution;
import com.microsoft.bot.dialogs.prompts.PromptOptions;
import com.microsoft.bot.dialogs.prompts.PromptValidatorContext;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.InputHints;
import com.microsoft.recognizers.datatypes.timex.expression.Constants;
import com.microsoft.recognizers.datatypes.timex.expression.TimexProperty;

import javax.annotation.Nullable;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.concurrent.CompletableFuture;

/**
 * The class containing the date resolver dialogs.
 */
public class DateResolverDialog extends CancelAndHelpDialog {
    private final String promptMsgText = "When would you like to travel?";
    private final String repromptMsgText =
        "I'm sorry, to make your booking please enter a full travel date including Day Month and Year.";


    /**
     * The constructor of the DateResolverDialog class.
     * @param id The dialog's id.
     */
    public DateResolverDialog(@Nullable String id) {
        super(id != null ? id : "DateResolverDialog");


        addDialog(new DateTimePrompt("DateTimePrompt",
            DateResolverDialog::dateTimePromptValidator, null));
        WaterfallStep[] waterfallSteps = {
            this::initialStep,
            this::finalStep
        };
        addDialog(new WaterfallDialog("WaterfallDialog", Arrays.asList(waterfallSteps)));

        // The initial child Dialog to run.
        setInitialDialogId("WaterfallDialog");
    }

    private CompletableFuture<DialogTurnResult> initialStep(WaterfallStepContext stepContext) {
        String timex = (String) stepContext.getOptions();

        Activity promptMessage = MessageFactory.text(promptMsgText, promptMsgText, InputHints.EXPECTING_INPUT);
        Activity repromptMessage = MessageFactory.text(repromptMsgText, repromptMsgText, InputHints.EXPECTING_INPUT);

        if (timex == null) {
            // We were not given any date at all so prompt the user.
            PromptOptions promptOptions = new PromptOptions();
            promptOptions.setPrompt(promptMessage);
            promptOptions.setRetryPrompt(repromptMessage);
            return stepContext.prompt("DateTimePrompt", promptOptions);
        }

        // We have a Date we just need to check it is unambiguous.
        TimexProperty timexProperty = new TimexProperty(timex);
        if (!timexProperty.getTypes().contains(Constants.TimexTypes.DEFINITE)) {
            // This is essentially a "reprompt" of the data we were given up front.
            PromptOptions promptOptions = new PromptOptions();
            promptOptions.setPrompt(repromptMessage);
            return stepContext.prompt("DateTimePrompt", promptOptions);
        }

        DateTimeResolution dateTimeResolution = new DateTimeResolution();
        dateTimeResolution.setTimex(timex);
        List<DateTimeResolution> dateTimeResolutions = new ArrayList<DateTimeResolution>();
        dateTimeResolutions.add(dateTimeResolution);
        return stepContext.next(dateTimeResolutions);
    }

    private CompletableFuture<DialogTurnResult> finalStep(WaterfallStepContext stepContext) {
        String timex = ((ArrayList<DateTimeResolution>) stepContext.getResult()).get(0).getTimex();
        return stepContext.endDialog(timex);
    }

    private static CompletableFuture<Boolean> dateTimePromptValidator(
        PromptValidatorContext<List<DateTimeResolution>> promptContext
    ) {
        if (promptContext.getRecognized().getSucceeded()) {
            // This value will be a TIMEX. And we are only interested in a Date so grab the first result and drop the
            // Time part. TIMEX is a format that represents DateTime expressions that include some ambiguity.
            // e.g. missing a Year.
            String timex = ((List<DateTimeResolution>) promptContext.getRecognized().getValue())
                .get(0).getTimex().split("T")[0];

            // If this is a definite Date including year, month and day we are good otherwise reprompt.
            // A better solution might be to let the user know what part is actually missing.
            Boolean isDefinite = new TimexProperty(timex).getTypes().contains(Constants.TimexTypes.DEFINITE);

            return CompletableFuture.completedFuture(isDefinite);
        }

        return CompletableFuture.completedFuture(false);
    }
}

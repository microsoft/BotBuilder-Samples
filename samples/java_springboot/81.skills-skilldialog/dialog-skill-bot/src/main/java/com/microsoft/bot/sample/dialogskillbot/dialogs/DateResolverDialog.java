// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.dialogskillbot.dialogs;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.concurrent.CompletableFuture;

import com.microsoft.applicationinsights.core.dependencies.apachecommons.lang3.StringUtils;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.dialogs.DialogTurnResult;
import com.microsoft.bot.dialogs.WaterfallDialog;
import com.microsoft.bot.dialogs.WaterfallStep;
import com.microsoft.bot.dialogs.WaterfallStepContext;
import com.microsoft.bot.dialogs.prompts.DateTimePrompt;
import com.microsoft.bot.dialogs.prompts.DateTimeResolution;
import com.microsoft.bot.dialogs.prompts.PromptOptions;
import com.microsoft.bot.dialogs.prompts.PromptValidator;
import com.microsoft.bot.dialogs.prompts.PromptValidatorContext;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.InputHints;
import com.microsoft.recognizers.datatypes.timex.expression.Constants;
import com.microsoft.recognizers.datatypes.timex.expression.TimexProperty;

public class DateResolverDialog extends CancelAndHelpDialog {

    private final String PromptMsgText = "When would you like to travel?";
    private final String RepromptMsgText = "I'm sorry, to make your booking please enter a full travel date, including Day, Month, and Year.";

    public DateResolverDialog(String id) {
        super(!StringUtils.isAllBlank(id) ? id : "DateResolverDialog");
        addDialog(new DateTimePrompt("DateTimePrompt", new dateTimePromptValidator(), null));
        WaterfallStep[] waterfallSteps = { this::initialStep, this::finalStep };
        addDialog(new WaterfallDialog("WaterfallDialog", Arrays.asList(waterfallSteps)));

        // The initial child Dialog to run.
        setInitialDialogId("WaterfallDialog");
    }

    class dateTimePromptValidator implements PromptValidator<List<DateTimeResolution>> {

        @Override
        public CompletableFuture<Boolean> promptValidator(
                PromptValidatorContext<List<DateTimeResolution>> promptContext) {
            if (promptContext.getRecognized().getSucceeded()) {
                // This value will be a TMEX. We are only interested in the Date part, so grab
                // the first result and drop the Time part.
                // TMEX instanceof a format that represents DateTime expressions that include
                // some ambiguity, such as a missing Year.
                String timex = promptContext.getRecognized().getValue().get(0).getTimex().split("T")[0];

                // If this instanceof a definite Date that includes year, month and day we are
                // good; otherwise, reprompt.
                // A better solution might be to let the user know what part instanceof actually
                // missing.
                Boolean isDefinite = new TimexProperty(timex).getTypes().contains(Constants.TimexTypes.DEFINITE);

                return CompletableFuture.completedFuture(isDefinite);
            }
            return CompletableFuture.completedFuture(false);
        }

    }

    public CompletableFuture<DialogTurnResult> initialStep(WaterfallStepContext stepContext) {
        String timex = (String) stepContext.getOptions();

        Activity promptMessage = MessageFactory.text(PromptMsgText, PromptMsgText, InputHints.EXPECTING_INPUT);
        Activity repromptMessage = MessageFactory.text(RepromptMsgText, RepromptMsgText, InputHints.EXPECTING_INPUT);

        if (timex == null) {
            // We were not given any date at all so prompt the user.
            PromptOptions options = new PromptOptions();
            options.setPrompt(promptMessage);
            options.setRetryPrompt(repromptMessage);
            return stepContext.prompt("DateTimePrompt", options);
        }

        // We have a Date we just need to check it instanceof unambiguous.
        TimexProperty timexProperty = new TimexProperty(timex);
        if (!timexProperty.getTypes().contains(Constants.TimexTypes.DEFINITE)) {
            // This instanceof essentially a "reprompt" of the data we were given up front.
            PromptOptions options = new PromptOptions();
            options.setPrompt(repromptMessage);
            return stepContext.prompt("DateTimePrompt", options);
        }
        List<DateTimeResolution> resolutionList = new ArrayList<DateTimeResolution>();
        DateTimeResolution resolution = new DateTimeResolution();
        resolution.setTimex(timex);
        resolutionList.add(resolution);
        return stepContext.next(resolutionList);
    }

    private CompletableFuture<DialogTurnResult> finalStep(WaterfallStepContext stepContext) {
        String timex = ((List<DateTimeResolution>) stepContext.getResult()).get(0).getTimex();
        return stepContext.endDialog(timex);
    }
}

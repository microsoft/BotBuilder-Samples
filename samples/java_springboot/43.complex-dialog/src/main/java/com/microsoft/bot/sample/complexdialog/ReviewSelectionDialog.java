// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.complexdialog;

import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.dialogs.ComponentDialog;
import com.microsoft.bot.dialogs.DialogTurnResult;
import com.microsoft.bot.dialogs.WaterfallDialog;
import com.microsoft.bot.dialogs.WaterfallStep;
import com.microsoft.bot.dialogs.WaterfallStepContext;
import com.microsoft.bot.dialogs.choices.ChoiceFactory;
import com.microsoft.bot.dialogs.choices.FoundChoice;
import com.microsoft.bot.dialogs.prompts.ChoicePrompt;
import com.microsoft.bot.dialogs.prompts.PromptOptions;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.concurrent.CompletableFuture;
import org.apache.commons.lang3.StringUtils;

public class ReviewSelectionDialog extends ComponentDialog {
    // Define a "done" response for the company selection prompt.
    private static final String DONE_OPTION = "done";

    // Define value names for values tracked inside the dialogs.
    private static final String COMPANIES_SELECTED = "value-companiesSelected";

    private static List<String> companiesOptions = Arrays.asList(
        "Adatum Corporation", "Contoso Suites", "Graphic Design Institute", "Wide World Importers"
    );

    public ReviewSelectionDialog(String withId) {
        super(withId);

        addDialog(new ChoicePrompt("ChoicePrompt"));

        WaterfallStep[] waterfallSteps = {
            this::selectionStep,
            this::loopStep
        };
        addDialog(new WaterfallDialog("WaterfallDialog", Arrays.asList(waterfallSteps)));

        // The initial child Dialog to run.
        setInitialDialogId("WaterfallDialog");
    }

    private CompletableFuture<DialogTurnResult> selectionStep(WaterfallStepContext stepContext) {
        // Continue using the same selection list, if any, from the previous iteration of this dialog.
        List<String> list = stepContext.getOptions() instanceof List
            ? (List<String>) stepContext.getOptions()
            : new ArrayList<>();
        stepContext.getValues().put(COMPANIES_SELECTED, list);

        String message;
        if (list.size() == 0) {
            message = String.format("Please choose a company to review, or `%s` to finish.", DONE_OPTION);
        } else {
            message = String.format("You have selected **%s**. You can review an additional company, or choose `%s` to finish.",
                list.get(0),
                DONE_OPTION);
        }

        // Create the list of options to choose from.
        List<String> options = new ArrayList<>(companiesOptions);
        options.add(DONE_OPTION);
        if (list.size() > 0) {
            options.remove(list.get(0));
        }

        PromptOptions promptOptions = new PromptOptions();
        promptOptions.setPrompt(MessageFactory.text(message));
        promptOptions.setRetryPrompt(MessageFactory.text("Please choose an option from the list."));
        promptOptions.setChoices(ChoiceFactory.toChoices(options));

        // Prompt the user for a choice.
        return stepContext.prompt("ChoicePrompt", promptOptions);
    }

    private CompletableFuture<DialogTurnResult> loopStep(WaterfallStepContext stepContext) {
        // Retrieve their selection list, the choice they made, and whether they chose to finish.
        List<String> list = (List<String>) stepContext.getValues().get(COMPANIES_SELECTED);
        FoundChoice choice = (FoundChoice) stepContext.getResult();
        boolean done = StringUtils.equals(choice.getValue(), DONE_OPTION);

        // If they chose a company, add it to the list.
        if (!done) {
            list.add(choice.getValue());
        }

        // If they're done, exit and return their list.
        if (done || list.size() >= 2) {
            return stepContext.endDialog(list);
        }

        // Otherwise, repeat this dialog, passing in the list from this iteration.
        return stepContext.replaceDialog(getId(), list);
    }
}

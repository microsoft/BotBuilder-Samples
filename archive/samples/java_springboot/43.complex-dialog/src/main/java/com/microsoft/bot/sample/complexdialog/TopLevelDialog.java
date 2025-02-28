// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.complexdialog;

import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.dialogs.ComponentDialog;
import com.microsoft.bot.dialogs.DialogTurnResult;
import com.microsoft.bot.dialogs.WaterfallDialog;
import com.microsoft.bot.dialogs.WaterfallStep;
import com.microsoft.bot.dialogs.WaterfallStepContext;
import com.microsoft.bot.dialogs.prompts.NumberPrompt;
import com.microsoft.bot.dialogs.prompts.PromptOptions;
import com.microsoft.bot.dialogs.prompts.TextPrompt;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.concurrent.CompletableFuture;

public class TopLevelDialog extends ComponentDialog {
    // Define a "done" response for the company selection prompt.
    private static final String DONE_OPTION = "done";

    // Define value names for values tracked inside the dialogs.
    private static final String USER_INFO = "value-userInfo";

    public TopLevelDialog() {
        super("TopLevelDialog");

        addDialog(new TextPrompt("TextPrompt"));
        addDialog(new NumberPrompt<Integer>("NumberPrompt", Integer.class));

        addDialog(new ReviewSelectionDialog());

        WaterfallStep[] waterfallSteps = {
            this::nameStep,
            this::ageStep,
            this::startSelectionStep,
            this::acknowledgementStep
        };
        addDialog(new WaterfallDialog("WaterfallDialog", Arrays.asList(waterfallSteps)));

        // The initial child Dialog to run.
        setInitialDialogId("WaterfallDialog");
    }

    private CompletableFuture<DialogTurnResult> nameStep(WaterfallStepContext stepContext) {
        // Create an object in which to collect the user's information within the dialog.
        stepContext.getValues().put(USER_INFO, new UserProfile());

        // Ask the user to enter their name.
        PromptOptions promptOptions = new PromptOptions();
        promptOptions.setPrompt(MessageFactory.text("Please enter your name."));
        return stepContext.prompt("TextPrompt", promptOptions);
    }

    private CompletableFuture<DialogTurnResult> ageStep(WaterfallStepContext stepContext) {
        // Set the user's name to what they entered in response to the name prompt.
        UserProfile userProfile = (UserProfile) stepContext.getValues().get(USER_INFO);
        userProfile.setName((String) stepContext.getResult());

        // Ask the user to enter their age.
        PromptOptions promptOptions = new PromptOptions();
        promptOptions.setPrompt(MessageFactory.text("Please enter your age."));
        return stepContext.prompt("NumberPrompt", promptOptions);
    }

    private CompletableFuture<DialogTurnResult> startSelectionStep(WaterfallStepContext stepContext) {
        // Set the user's age to what they entered in response to the age prompt.
        UserProfile userProfile = (UserProfile) stepContext.getValues().get(USER_INFO);
        userProfile.setAge((Integer) stepContext.getResult());

        // If they are too young, skip the review selection dialog, and pass an empty list to the next step.
        if (userProfile.getAge() < 25) {
            return stepContext.getContext().sendActivity(MessageFactory.text("You must be 25 or older to participate."))
                .thenCompose(resourceResponse -> stepContext.next(new ArrayList<String>()));
        }

        // Otherwise, start the review selection dialog.
        return stepContext.beginDialog("ReviewSelectionDialog");
    }

    private CompletableFuture<DialogTurnResult> acknowledgementStep(WaterfallStepContext stepContext) {
        // Set the user's company selection to what they entered in the review-selection dialog.
        UserProfile userProfile = (UserProfile) stepContext.getValues().get(USER_INFO);
        userProfile.setCompaniesToReview(stepContext.getResult() instanceof List
            ? (List<String>) stepContext.getResult()
            : new ArrayList<>());

        // Thank them for participating.
        return stepContext.getContext()
            .sendActivity(MessageFactory.text(String.format("Thanks for participating, %s.", userProfile.getName())))
            .thenCompose(resourceResponse -> stepContext.endDialog(stepContext.getValues().get(USER_INFO)));
    }
}

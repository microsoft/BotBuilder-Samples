// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.multiturnprompt;

import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.StatePropertyAccessor;
import com.microsoft.bot.builder.UserState;
import com.microsoft.bot.connector.Channels;
import com.microsoft.bot.dialogs.ComponentDialog;
import com.microsoft.bot.dialogs.DialogTurnResult;
import com.microsoft.bot.dialogs.WaterfallDialog;
import com.microsoft.bot.dialogs.WaterfallStep;
import com.microsoft.bot.dialogs.WaterfallStepContext;
import com.microsoft.bot.dialogs.choices.ChoiceFactory;
import com.microsoft.bot.dialogs.choices.FoundChoice;
import com.microsoft.bot.dialogs.prompts.AttachmentPrompt;
import com.microsoft.bot.dialogs.prompts.ChoicePrompt;
import com.microsoft.bot.dialogs.prompts.ConfirmPrompt;
import com.microsoft.bot.dialogs.prompts.NumberPrompt;
import com.microsoft.bot.dialogs.prompts.PromptOptions;
import com.microsoft.bot.dialogs.prompts.PromptValidatorContext;
import com.microsoft.bot.dialogs.prompts.TextPrompt;
import com.microsoft.bot.schema.Attachment;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.concurrent.CompletableFuture;
import org.apache.commons.lang3.StringUtils;

public class UserProfileDialog extends ComponentDialog {
    private StatePropertyAccessor<UserProfile> userProfileAccessor;

    public UserProfileDialog(UserState withUserState) {
        super("UserProfileDialog");

        userProfileAccessor = withUserState.createProperty("UserProfile");

        WaterfallStep[] waterfallSteps = {
            UserProfileDialog::transportStep,
            UserProfileDialog::nameStep,
            UserProfileDialog::nameConfirmStep,
            UserProfileDialog::ageStep,
            UserProfileDialog::pictureStep,
            UserProfileDialog::confirmStep,
            this::summaryStep
        };

        // Add named dialogs to the DialogSet. These names are saved in the dialog state.
        addDialog(new WaterfallDialog("WaterfallDialog", Arrays.asList(waterfallSteps)));
        addDialog(new TextPrompt("TextPrompt"));
        addDialog(new NumberPrompt<Integer>("NumberPrompt", UserProfileDialog::agePromptValidator, Integer.class));
        addDialog(new ChoicePrompt("ChoicePrompt"));
        addDialog(new ConfirmPrompt("ConfirmPrompt"));
        addDialog(new AttachmentPrompt("AttachmentPrompt", UserProfileDialog::picturePromptValidator));

        // The initial child Dialog to run.
        setInitialDialogId("WaterfallDialog");
    }

    private static CompletableFuture<DialogTurnResult> transportStep(WaterfallStepContext stepContext) {
        // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
        // Running a prompt here means the next WaterfallStep will be run when the user's response is received.
        PromptOptions promptOptions = new PromptOptions();
        promptOptions.setPrompt(MessageFactory.text("Please enter your mode of transport."));
        promptOptions.setChoices(ChoiceFactory.toChoices("Car", "Bus", "Bicycle"));

        return stepContext.prompt("ChoicePrompt", promptOptions);
    }

    private static CompletableFuture<DialogTurnResult> nameStep(WaterfallStepContext stepContext) {
        stepContext.getValues().put("transport", ((FoundChoice) stepContext.getResult()).getValue());

        PromptOptions promptOptions = new PromptOptions();
        promptOptions.setPrompt(MessageFactory.text("Please enter your name."));
        return stepContext.prompt("TextPrompt", promptOptions);
    }

    private static CompletableFuture<DialogTurnResult> nameConfirmStep(WaterfallStepContext stepContext) {
        stepContext.getValues().put("name", stepContext.getResult());

        // We can send messages to the user at any point in the WaterfallStep.
        return stepContext.getContext().sendActivity(MessageFactory.text(String.format("Thanks %s", stepContext.getResult())))
            .thenCompose(resourceResponse -> {
                // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
                PromptOptions promptOptions = new PromptOptions();
                promptOptions.setPrompt(MessageFactory.text("Would you like to give your age?"));
                return stepContext.prompt("ConfirmPrompt", promptOptions);
            });
    }

    private static CompletableFuture<DialogTurnResult> ageStep(WaterfallStepContext stepContext) {
        if ((Boolean)stepContext.getResult()) {
            // User said "yes" so we will be prompting for the age.
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            PromptOptions promptOptions = new PromptOptions();
            promptOptions.setPrompt(MessageFactory.text("Please enter your age."));
            promptOptions.setRetryPrompt(MessageFactory.text("The value entered must be greater than 0 and less than 150."));

            return stepContext.prompt("NumberPrompt", promptOptions);
        }

        // User said "no" so we will skip the next step. Give -1 as the age.
        return stepContext.next(-1);
    }

    private static CompletableFuture<DialogTurnResult> pictureStep(WaterfallStepContext stepContext) {
        stepContext.getValues().put("age", (Integer) stepContext.getResult());

        String msg = (Integer)stepContext.getValues().get("age") == -1
            ? "No age given."
            : String.format("I have your age as %d.", (Integer)stepContext.getValues().get("age"));

        // We can send messages to the user at any point in the WaterfallStep.
        return stepContext.getContext().sendActivity(MessageFactory.text(msg))
            .thenCompose(resourceResponse -> {
                if (StringUtils.equals(stepContext.getContext().getActivity().getChannelId(), Channels.MSTEAMS)) {
                    // This attachment prompt example is not designed to work for Teams attachments, so skip it in this case
                    return stepContext.getContext().sendActivity(MessageFactory.text("Skipping attachment prompt in Teams channel..."))
                        .thenCompose(resourceResponse1 -> stepContext.next(null));
                }

                // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
                PromptOptions promptOptions = new PromptOptions();
                promptOptions.setPrompt(MessageFactory.text("Please attach a profile picture (or type any message to skip)."));
                promptOptions.setRetryPrompt(MessageFactory.text("The attachment must be a jpeg/png image file."));

                return stepContext.prompt("AttachmentPrompt", promptOptions);
            });
    }

    private static CompletableFuture<DialogTurnResult> confirmStep(WaterfallStepContext stepContext) {
        List<Attachment> attachments = (List<Attachment>)stepContext.getResult();
        stepContext.getValues().put("picture", attachments == null ? null : attachments.get(0));

        // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
        PromptOptions promptOptions = new PromptOptions();
        promptOptions.setPrompt(MessageFactory.text("Is this ok?"));
        return stepContext.prompt("ConfirmPrompt", promptOptions);
    }

    private CompletableFuture<DialogTurnResult> summaryStep(WaterfallStepContext stepContext) {
        if ((Boolean)stepContext.getResult()) {
            // Get the current profile object from user state.
            return userProfileAccessor.get(stepContext.getContext(), () -> new UserProfile())
                .thenCompose(userProfile -> {
                    userProfile.transport = (String) stepContext.getValues().get("transport");
                    userProfile.name = (String) stepContext.getValues().get("name");
                    userProfile.age = (Integer) stepContext.getValues().get("age");
                    userProfile.picture = (Attachment) stepContext.getValues().get("picture");

                    String msg = String.format(
                        "I have your mode of transport as %s and your name as %s",
                        userProfile.transport, userProfile.name
                    );

                    if (userProfile.age != -1) {
                        msg += String.format(" and your age as %s", userProfile.age);
                    }

                    msg += ".";

                    return stepContext.getContext().sendActivity(MessageFactory.text(msg))
                        .thenApply(resourceResponse -> userProfile);
                })
                .thenCompose(userProfile -> {
                    if (userProfile.picture != null) {
                        return stepContext.getContext().sendActivity(
                            MessageFactory.attachment(userProfile.picture,
                                "This is your profile picture."
                            ));
                    }

                    return stepContext.getContext().sendActivity(
                        MessageFactory.text("A profile picture wasn't attached.")
                    );
                })
                .thenCompose(resourceResponse -> stepContext.endDialog());
        }

        // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is the end.
        return stepContext.getContext().sendActivity(MessageFactory.text("Thanks. Your profile will not be kept."))
            .thenCompose(resourceResponse -> stepContext.endDialog());
    }

    private static CompletableFuture<Boolean> picturePromptValidator(
        PromptValidatorContext<List<Attachment>> promptContext
    ) {
        if (promptContext.getRecognized().getSucceeded()) {
            List<Attachment> attachments = promptContext.getRecognized().getValue();
            List<Attachment> validImages = new ArrayList<>();

            for (Attachment attachment : attachments) {
                if (StringUtils.equals(
                    attachment.getContentType(), "image/jpeg") || StringUtils.equals(attachment.getContentType(), "image/png")
                ) {
                    validImages.add(attachment);
                }
            }

            promptContext.getRecognized().setValue(validImages);

            // If none of the attachments are valid images, the retry prompt should be sent.
            return CompletableFuture.completedFuture(!validImages.isEmpty());
        }
        else {
            // We can return true from a validator function even if Recognized.Succeeded is false.
            return promptContext.getContext().sendActivity("No attachments received. Proceeding without a profile picture...")
                .thenApply(resourceResponse -> true);
        }
    }

    private static CompletableFuture<Boolean> agePromptValidator(
        PromptValidatorContext<Integer> promptContext
    ) {
        // This condition is our validation rule. You can also change the value at this point.
        return CompletableFuture.completedFuture(
            promptContext.getRecognized().getSucceeded()
                && promptContext.getRecognized().getValue() > 0
                && promptContext.getRecognized().getValue() < 150);
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.usingcards;

import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.dialogs.ComponentDialog;
import com.microsoft.bot.dialogs.DialogTurnResult;
import com.microsoft.bot.dialogs.WaterfallDialog;
import com.microsoft.bot.dialogs.WaterfallStep;
import com.microsoft.bot.dialogs.WaterfallStepContext;
import com.microsoft.bot.dialogs.choices.Choice;
import com.microsoft.bot.dialogs.choices.FoundChoice;
import com.microsoft.bot.dialogs.prompts.ChoicePrompt;
import com.microsoft.bot.dialogs.prompts.PromptOptions;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.Attachment;
import com.microsoft.bot.schema.AttachmentLayoutTypes;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.concurrent.CompletableFuture;
import org.slf4j.LoggerFactory;

public class MainDialog extends ComponentDialog {
    public MainDialog() {
        super("MainDialog");

        WaterfallStep[] waterfallSteps = {
            this::choiceCardStep,
            this::showCardStep
        };

        // Define the main dialog and its related components.
        addDialog(new ChoicePrompt("ChoicePrompt"));
        addDialog(new WaterfallDialog("WaterfallDialog", Arrays.asList(waterfallSteps)));

        // The initial child Dialog to run.
        setInitialDialogId("WaterfallDialog");
    }

    // 1. Prompts the user if the user is not in the middle of a dialog.
    // 2. Re-prompts the user when an invalid input is received.
    private CompletableFuture<DialogTurnResult> choiceCardStep(WaterfallStepContext stepContext) {
        LoggerFactory.getLogger(MainDialog.class).info("MainDialog.choiceCardStep");
        
        // Create the PromptOptions which contain the prompt and re-prompt messages.
        // PromptOptions also contains the list of choices available to the user.
        PromptOptions promptOptions = new PromptOptions();
        promptOptions.setPrompt(MessageFactory.text("What card would you like to see? You can click or type the card name"));
        promptOptions.setRetryPrompt(MessageFactory.text("That was not a valid choice, please select a card or number from 1 to 9."));
        promptOptions.setChoices(getChoices());

        // Prompt the user with the configured PromptOptions.
        return stepContext.prompt("ChoicePrompt", promptOptions);
    }

    // Send a Rich Card response to the user based on their choice.
    // This method is only called when a valid prompt response is parsed from the user's response to the ChoicePrompt.
    private CompletableFuture<DialogTurnResult> showCardStep(WaterfallStepContext stepContext) {
        LoggerFactory.getLogger(MainDialog.class).info("MainDialog.showCardStep");
        
        // Cards are sent as Attachments in the Bot Framework.
        // So we need to create a list of attachments for the reply activity.
        List<Attachment> attachments = new ArrayList<>();
        
        // Reply to the activity we received with an activity.
        Activity reply = MessageFactory.attachment(attachments);

        // Decide which type of card(s) we are going to show the user
        switch (((FoundChoice) stepContext.getResult()).getValue()) {
            case "Adaptive Card":
                // Display an Adaptive Card
                reply.getAttachments().add(Cards.createAdaptiveCardAttachment());
                break;
            case "Animation Card":
                // Display an AnimationCard.
                reply.getAttachments().add(Cards.getAnimationCard().toAttachment());
                break;
            case "Audio Card":
                // Display an AudioCard
                reply.getAttachments().add(Cards.getAudioCard().toAttachment());
                break;
            case "Hero Card":
                // Display a HeroCard.
                reply.getAttachments().add(Cards.getHeroCard().toAttachment());
                break;
            case "OAuth Card":
                // Display an OAuthCard
                reply.getAttachments().add(Cards.getOAuthCard().toAttachment());
                break;
            case "Receipt Card":
                // Display a ReceiptCard.
                reply.getAttachments().add(Cards.getReceiptCard().toAttachment());
                break;
            case "Signin Card":
                // Display a SignInCard.
                reply.getAttachments().add(Cards.getSigninCard().toAttachment());
                break;
            case "Thumbnail Card":
                // Display a ThumbnailCard.
                reply.getAttachments().add(Cards.getThumbnailCard().toAttachment());
                break;
            case "Video Card":
                // Display a VideoCard
                reply.getAttachments().add(Cards.getVideoCard().toAttachment());
                break;
            default:
                // Display a carousel of all the rich card types.
                reply.setAttachmentLayout(AttachmentLayoutTypes.CAROUSEL);
                reply.getAttachments().add(Cards.createAdaptiveCardAttachment());
                reply.getAttachments().add(Cards.getAnimationCard().toAttachment());
                reply.getAttachments().add(Cards.getAudioCard().toAttachment());
                reply.getAttachments().add(Cards.getHeroCard().toAttachment());
                reply.getAttachments().add(Cards.getOAuthCard().toAttachment());
                reply.getAttachments().add(Cards.getReceiptCard().toAttachment());
                reply.getAttachments().add(Cards.getSigninCard().toAttachment());
                reply.getAttachments().add(Cards.getThumbnailCard().toAttachment());
                reply.getAttachments().add(Cards.getVideoCard().toAttachment());
                break;
        }

        // Send the card(s) to the user as an attachment to the activity
        return stepContext.getContext().sendActivity(reply)
            .thenCompose(resourceResponse -> stepContext.getContext().sendActivity(
                // Give the user instructions about what to do next
                MessageFactory.text("Type anything to see another card.")
            ))
            .thenCompose(resourceResponse -> stepContext.endDialog());
    }

    private List<Choice> getChoices() {
        return Arrays.asList(
            new Choice("Adaptive Card", "adaptive"),
            new Choice("Animation Card", "animation"),
            new Choice("Audio Card", "audio"),
            new Choice("Hero Card", "hero"),
            new Choice("OAuth Card", "oauth"),
            new Choice("Receipt Card", "receipt"),
            new Choice("Signin Card", "signin"),
            new Choice("Thumbnail Card", "thumbnail", "thumb"),
            new Choice("Video Card", "video"),
            new Choice("All cards", "all")
        );
    }
}

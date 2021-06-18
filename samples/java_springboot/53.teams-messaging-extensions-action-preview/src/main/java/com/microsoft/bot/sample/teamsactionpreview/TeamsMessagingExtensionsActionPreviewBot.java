// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.teamsactionpreview;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.teams.TeamsActivityHandler;
import com.microsoft.bot.sample.teamsactionpreview.models.AdaptiveCard;
import com.microsoft.bot.sample.teamsactionpreview.models.Body;
import com.microsoft.bot.sample.teamsactionpreview.models.Choice;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.Attachment;
import com.microsoft.bot.schema.teams.*;
import org.apache.commons.io.IOUtils;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.io.InputStream;
import java.nio.charset.StandardCharsets;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.concurrent.CompletableFuture;
import java.util.stream.Collectors;

/**
 * This class implements the functionality of the Bot.
 *
 * <p>This is where application specific logic for interacting with the users would be
 * added.  This sample shows how to create a simple card based on
 * parameters entered by the user from a Task Module.
 * </p>
 */
public class TeamsMessagingExtensionsActionPreviewBot extends TeamsActivityHandler {

    @Override
    protected CompletableFuture<Void> onMessageActivity(
        TurnContext turnContext) {
        if (turnContext.getActivity().getValue() != null) {
            // This was a message from the card.
            LinkedHashMap obj = (LinkedHashMap) turnContext.getActivity().getValue();
            String answer = "";
            if (obj.get("Answer") != null) {
                answer = (String) obj.get("Answer");
            }
            String choices = "";
            if (obj.get("Choices") != null) {
                choices = (String) obj.get("Choices");
            }
         return turnContext.sendActivity(
             MessageFactory.text(
                 String.format("%1$s answered '%2$s' and chose '%3$s'",
                     turnContext.getActivity().getFrom().getName(),
                     answer,
                     choices)))
             .thenApply(resourceResponse -> null);
        }

        // This is a regular text message.
        return turnContext.sendActivity(MessageFactory.text("Hello from the TeamsMessagingExtensionsActionPreviewBot."))
            .thenApply(resourceResponse -> null);
    }

    @Override
    protected CompletableFuture<MessagingExtensionActionResponse> onTeamsMessagingExtensionFetchTask(
        TurnContext turnContext,
        MessagingExtensionAction action) {

        Attachment adaptiveCardEditor = getAdaptiveCardAttachment("adaptiveCardEditor.json");

        TaskModuleTaskInfo taskInfo = new TaskModuleTaskInfo();
        taskInfo.setCard(adaptiveCardEditor);
        taskInfo.setWidth(500);
        taskInfo.setHeight(450);
        taskInfo.setTitle("Task Module Fetch Example");

        TaskModuleContinueResponse continueResponse = new TaskModuleContinueResponse();
        continueResponse.setValue(taskInfo);
        continueResponse.setType("continue");

        MessagingExtensionActionResponse actionResponse = new MessagingExtensionActionResponse();
        actionResponse.setTask(continueResponse);

        return CompletableFuture.completedFuture(actionResponse);
    }

    @Override
    protected CompletableFuture<MessagingExtensionActionResponse> onTeamsMessagingExtensionSubmitAction(
        TurnContext turnContext,
        MessagingExtensionAction action) {

        Attachment adaptiveCard = getAdaptiveCardAttachment("submitCard.json");

        updateAttachmentAdaptiveCard(adaptiveCard, action);

        MessagingExtensionResult result = new MessagingExtensionResult();
        result.setType("botMessagePreview");
        result.setActivityPreview(MessageFactory.attachment(adaptiveCard));

        MessagingExtensionActionResponse response = new MessagingExtensionActionResponse();
        response.setComposeExtension(result);

        return CompletableFuture.completedFuture(response);
    }

    @Override
    protected CompletableFuture<MessagingExtensionActionResponse> onTeamsMessagingExtensionBotMessagePreviewEdit(
        TurnContext turnContext,
        MessagingExtensionAction action) {

        // This is a preview edit call and so this time we want to re-create the adaptive card editor.
        Attachment adaptiveCard = getAdaptiveCardAttachment("adaptiveCardEditor.json");

        Activity preview = action.getBotActivityPreview().get(0);
        Attachment previewCard = preview.getAttachments().get(0);
        updateAttachmentAdaptiveCardEdit(adaptiveCard, previewCard);

        TaskModuleTaskInfo taskInfo = new TaskModuleTaskInfo();
        taskInfo.setCard(adaptiveCard);
        taskInfo.setHeight(450);
        taskInfo.setWidth(500);
        taskInfo.setTitle("Task Module Fetch Example");

        TaskModuleContinueResponse continueResponse = new TaskModuleContinueResponse();
        continueResponse.setValue(taskInfo);
        continueResponse.setType("continue");

        MessagingExtensionActionResponse actionResponse = new MessagingExtensionActionResponse();
        actionResponse.setTask(continueResponse);

        return CompletableFuture.completedFuture(actionResponse);
    }

    @Override
    protected CompletableFuture<MessagingExtensionActionResponse> onTeamsMessagingExtensionBotMessagePreviewSend(
        TurnContext turnContext,
        MessagingExtensionAction action) {
        // The data has been returned to the bot in the action structure.

        Activity preview = action.getBotActivityPreview().get(0);
        Attachment previewCard = preview.getAttachments().get(0);

        Activity message = MessageFactory.attachment(previewCard);

        return turnContext.sendActivity(message)
            .thenApply(resourceResponse -> null);
    }

    @Override
    protected CompletableFuture<Void> onTeamsMessagingExtensionCardButtonClicked(
        TurnContext turnContext,
        Object cardData) {
        // If the adaptive card was added to the compose window (by either the OnTeamsMessagingExtensionSubmitActionAsync or
        // OnTeamsMessagingExtensionBotMessagePreviewSendAsync handler's return values) the submit values will come in here.
        Activity reply = MessageFactory.text("OnTeamsMessagingExtensionCardButtonClickedAsync");
        return turnContext.sendActivity(reply)
            .thenApply(resourceResponse -> null);
    }

    private Attachment getAdaptiveCardAttachment(String fileName) {
        try {
            InputStream input = getClass()
                .getClassLoader()
                .getResourceAsStream(fileName);
            String content = IOUtils.toString(input, StandardCharsets.UTF_8);

            Attachment attachment = new Attachment();
            attachment.setContentType("application/vnd.microsoft.card.adaptive");
            attachment.setContent(new ObjectMapper().readValue(content, AdaptiveCard.class));

            return attachment;
        } catch (IOException e) {
            LoggerFactory.getLogger(TeamsMessagingExtensionsActionPreviewBot.class)
                .error("getAdaptiveCardAttachment", e);
        }
        return new Attachment();
    }

    private void updateAttachmentAdaptiveCard(
        Attachment attachment,
        MessagingExtensionAction action
    ){
        LinkedHashMap data = (LinkedHashMap) action.getData();
        AdaptiveCard card = (AdaptiveCard) attachment.getContent();
        for (Body item : card.getBody() ) {
            if (item.getChoices() != null) {
                for (int index = 0 ; index < 3 ; index++) {
                    item.getChoices().get(index).setTitle((String) data.get("Option" + (index + 1)));
                    item.getChoices().get(index).setValue((String) data.get("Option" + (index + 1)));
                }
            }

            if (item.getId() != null && item.getId().equals("Question")) {
                item.setText((String) data.get("Question"));
            }
        }
    }

    private void updateAttachmentAdaptiveCardEdit(
        Attachment attachment,
        Attachment preview
    ) {
        AdaptiveCard prv = null;
        try {
            String cardAsString = new ObjectMapper().writeValueAsString(preview.getContent());
            prv = new ObjectMapper().readValue(cardAsString, AdaptiveCard.class);
        } catch (JsonProcessingException e) {
            LoggerFactory.getLogger(TeamsMessagingExtensionsActionPreviewBot.class)
                .error("updateAttachmentAdaptiveCardEdit", e);
        } catch (IOException e) {
            LoggerFactory.getLogger(TeamsMessagingExtensionsActionPreviewBot.class)
                .error("updateAttachmentAdaptiveCardEdit", e);
        }

        AdaptiveCard atc = (AdaptiveCard) attachment.getContent();

        Body question = atc.getBody().stream()
            .filter(i -> "Question".equals(i.getId()))
            .findAny()
            .orElse(null);

        question.setValue(prv.getBody().stream()
            .filter(i -> "Question".equals(i.getId()))
            .findAny()
            .orElse(null).getText());

        List<Body> options = atc.getBody().stream()
            .filter(i -> i.getId()!= null && i.getId().startsWith("Option"))
            .collect(Collectors.toList());

        for (Body item: options) {
            int responseIndex = Integer.parseInt(item.getId().charAt(6) + "");
            Choice choice = prv.getBody().stream()
                .filter(i -> i.getId() != null && i.getId().equals("Choices"))
                .findFirst()
                .orElse(null)
                .getChoices()
                .get(responseIndex - 1);

            item.setValue(choice.getValue());
        }
    }
}

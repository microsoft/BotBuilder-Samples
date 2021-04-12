// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.teamsaction;

import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.teams.TeamsActivityHandler;
import com.microsoft.bot.schema.CardImage;
import com.microsoft.bot.schema.HeroCard;
import com.microsoft.bot.schema.teams.*;

import java.util.*;
import java.util.concurrent.CompletableFuture;

/**
 * This class implements the functionality of the Bot.
 *
 * <p>This is where application specific logic for interacting with the users would be
 * added.  There are two basic types of Messaging Extension in Teams: Search-based and Action-based.
 * This sample illustrates how to build an Action-based Messaging Extension.</p>
 */
public class TeamsMessagingExtensionsActionBot extends TeamsActivityHandler {
    @Override
    protected CompletableFuture<MessagingExtensionActionResponse> onTeamsMessagingExtensionSubmitAction(
        TurnContext turnContext,
        MessagingExtensionAction action
    ) {
        switch (action.getCommandId()) {
            // These commandIds are defined in the Teams App Manifest.
            case "createCard":
                return createCardCommand(turnContext, action);

            case "shareMessage":
                return shareMessageCommand(turnContext, action);
            default:
                return notImplemented(
                    String.format("Invalid CommandId: %s", action.getCommandId()));
        }
    }

    private CompletableFuture<MessagingExtensionActionResponse> createCardCommand(
        TurnContext turnContext,
        MessagingExtensionAction action
    ) {
        Map<String, String> actionData = (Map<String, String>) action.getData();

        HeroCard card = new HeroCard();
        card.setTitle(actionData.get("title"));
        card.setSubtitle(actionData.get("subTitle"));
        card.setText(actionData.get("text"));

        MessagingExtensionAttachment attachment = new MessagingExtensionAttachment();
        attachment.setContent(card);
        attachment.setContentType(HeroCard.CONTENTTYPE);
        attachment.setPreview(card.toAttachment());
        List<MessagingExtensionAttachment> attachments = Arrays.asList(attachment);

        MessagingExtensionResult result = new MessagingExtensionResult();
        result.setAttachments(attachments);
        result.setAttachmentLayout("list");
        result.setType("result");
        MessagingExtensionActionResponse response = new MessagingExtensionActionResponse();
        response.setComposeExtension(result);

        return CompletableFuture.completedFuture(response);
    }

    private CompletableFuture<MessagingExtensionActionResponse> shareMessageCommand(
        TurnContext turnContext,
        MessagingExtensionAction action
    ) {
        Map<String, String> actionData = (Map<String, String>) action.getData();

        HeroCard card = new HeroCard();
        card.setTitle(
            action.getMessagePayload().getFrom().getUser() != null ? action.getMessagePayload()
                .getFrom().getUser().getDisplayName() : "");
        card.setText(action.getMessagePayload().getBody().getContent());

        if (action.getMessagePayload().getAttachments() != null && !action.getMessagePayload()
            .getAttachments().isEmpty()) {
            card.setSubtitle("Attachments not included)");
        }

        boolean includeImage = actionData.get("includeImage") != null ? (
            Boolean.valueOf(actionData.get("includeImage"))
        ) : false;
        if (includeImage) {
            CardImage cardImage = new CardImage();
            cardImage.setUrl("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQtB3AwMUeNoq4gUBGe6Ocj8kyh3bXa9ZbV7u1fVKQoyKFHdkqU");
            card.setImages(Arrays.asList(cardImage));
        }

        MessagingExtensionAttachment attachment = new MessagingExtensionAttachment();
        attachment.setContent(card);
        attachment.setContentType(HeroCard.CONTENTTYPE);
        attachment.setPreview(card.toAttachment());

        MessagingExtensionResult result = new MessagingExtensionResult();
        result.setAttachmentLayout("list");
        result.setType("result");
        result.setAttachments(Arrays.asList(attachment));

        MessagingExtensionActionResponse response = new MessagingExtensionActionResponse();
        response.setComposeExtension(result);
        return CompletableFuture.completedFuture(response);
    }
}

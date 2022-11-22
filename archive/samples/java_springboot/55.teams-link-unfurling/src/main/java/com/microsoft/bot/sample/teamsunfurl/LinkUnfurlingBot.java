// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.teamsunfurl;

import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.teams.TeamsActivityHandler;
import com.microsoft.bot.schema.CardImage;
import com.microsoft.bot.schema.HeroCard;
import com.microsoft.bot.schema.ThumbnailCard;
import com.microsoft.bot.schema.teams.AppBasedLinkQuery;
import com.microsoft.bot.schema.teams.MessagingExtensionAttachment;
import com.microsoft.bot.schema.teams.MessagingExtensionQuery;
import com.microsoft.bot.schema.teams.MessagingExtensionResponse;
import com.microsoft.bot.schema.teams.MessagingExtensionResult;
import org.apache.commons.lang3.StringUtils;

import java.util.Collections;
import java.util.concurrent.CompletableFuture;

/**
 * This class implements the functionality of the Bot.
 *
 * <p>This is where application specific logic for interacting with the users would be
 * added.</p>
 */
public class LinkUnfurlingBot extends TeamsActivityHandler {

    @Override
    protected CompletableFuture<MessagingExtensionResponse> onTeamsAppBasedLinkQuery(
        TurnContext turnContext,
        AppBasedLinkQuery query
    ) {
        ThumbnailCard card = new ThumbnailCard();
        card.setTitle("Thumbnail Card");
        card.setText(query.getUrl());
        card.setImages(Collections.singletonList(
            new CardImage("https://raw.githubusercontent.com/microsoft/botframework-sdk/master/icon.png")
        ));


        MessagingExtensionAttachment attachments = new MessagingExtensionAttachment();
        attachments.setContentType(HeroCard.CONTENTTYPE);
        attachments.setContent(card);

        MessagingExtensionResult result = new MessagingExtensionResult();
        result.setAttachmentLayout("list");
        result.setType("result");
        result.setAttachments(Collections.singletonList(attachments));

        return CompletableFuture.completedFuture(new MessagingExtensionResponse(result));
    }

    protected CompletableFuture<MessagingExtensionResponse> onTeamsMessagingExtensionQuery(
        TurnContext turnContext,
        MessagingExtensionQuery query
    ) {
        // Note: The Teams manifest.json for this sample also includes a Search Query, in
        // order to enable installing from App Studio.

        // These commandIds are defined in the Teams App Manifest.
        if (StringUtils.equalsIgnoreCase("searchQuery", query.getCommandId())) {
            HeroCard card = new HeroCard();
            card.setTitle("This is a Link Unfurling Sample");
            card.setSubtitle("It will unfurl links from *.BotFramework.com");
            card.setText("This sample demonstrates how to handle link unfurling in Teams.  "
                + "Please review the readme for more information.");

            MessagingExtensionAttachment attachment = new MessagingExtensionAttachment();
            attachment.setContent(card);
            attachment.setContentType(HeroCard.CONTENTTYPE);
            attachment.setPreview(card.toAttachment());

            MessagingExtensionResult result = new MessagingExtensionResult();
            result.setAttachmentLayout("list");
            result.setType("result");
            result.setAttachment(attachment);

            return CompletableFuture.completedFuture(new MessagingExtensionResponse(result));
        }

        return notImplemented(String.format("Invalid CommandId: %s", query.getCommandId()));
    }
}

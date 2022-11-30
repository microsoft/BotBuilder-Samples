// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.teamssearch;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.node.ArrayNode;
import com.fasterxml.jackson.databind.node.ObjectNode;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.teams.TeamsActivityHandler;
import com.microsoft.bot.schema.*;
import com.microsoft.bot.schema.teams.*;
import java.util.concurrent.CompletionException;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;
import org.apache.commons.lang3.StringUtils;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.util.*;
import java.util.concurrent.CompletableFuture;

/**
 * This class implements the functionality of the Bot.
 *
 * <p>This is where application specific logic for interacting with the users would be
 * added.  This sample illustrates how to build a Search-based Messaging Extension.</p>
 */

public class TeamsMessagingExtensionsSearchBot extends TeamsActivityHandler {

    @Override
    protected CompletableFuture<MessagingExtensionResponse> onTeamsMessagingExtensionQuery(
        TurnContext turnContext,
        MessagingExtensionQuery query
    ) {
        String text = "";
        if (query != null && query.getParameters() != null) {
            List<MessagingExtensionParameter> queryParams = query.getParameters();
            if (!queryParams.isEmpty()) {
                text = (String) queryParams.get(0).getValue();
            }
        }

        return findPackages(text)
            .thenApply(packages -> {
                // We take every row of the results
                // and wrap them in cards wrapped in MessagingExtensionAttachment objects.
                // The Preview is optional, if it includes a Tap
                // that will trigger the onTeamsMessagingExtensionSelectItem event back on this bot.
                List<MessagingExtensionAttachment> attachments = new ArrayList<>();
                for (String[] item : packages) {
                    ObjectNode data = Serialization.createObjectNode();
                    data.set("data", Serialization.objectToTree(item));

                    CardAction cardAction = new CardAction();
                    cardAction.setType(ActionTypes.INVOKE);
                    cardAction.setValue(Serialization.toStringSilent(data));
                    ThumbnailCard previewCard = new ThumbnailCard();
                    previewCard.setTitle(item[0]);
                    previewCard.setTap(cardAction);

                    if (!StringUtils.isEmpty(item[4])) {
                        CardImage cardImage = new CardImage();
                        cardImage.setUrl(item[4]);
                        cardImage.setAlt("Icon");
                        previewCard.setImages(Collections.singletonList(cardImage));
                    }

                    HeroCard heroCard = new HeroCard();
                    heroCard.setTitle(item[0]);

                    MessagingExtensionAttachment attachment = new MessagingExtensionAttachment();
                    attachment.setContentType(HeroCard.CONTENTTYPE);
                    attachment.setContent(heroCard);
                    attachment.setPreview(previewCard.toAttachment());

                    attachments.add(attachment);
                }

                MessagingExtensionResult composeExtension = new MessagingExtensionResult();
                composeExtension.setType("result");
                composeExtension.setAttachmentLayout("list");
                composeExtension.setAttachments(attachments);

                // The list of MessagingExtensionAttachments must we wrapped in a MessagingExtensionResult
                // wrapped in a MessagingExtensionResponse.
                return new MessagingExtensionResponse(composeExtension);
            });
    }

    @Override
    protected CompletableFuture<MessagingExtensionResponse> onTeamsMessagingExtensionSelectItem(
        TurnContext turnContext,
        Object query
    ) {

        // The Preview card's Tap should have a Value property assigned, this will be returned to the bot in this event.
        Map cardValue = (Map) query;
        List<String> data = (ArrayList) cardValue.get("data");
        CardAction cardAction = new CardAction();
        cardAction.setType(ActionTypes.OPEN_URL);
        cardAction.setTitle("Project");
        cardAction.setValue(data.get(3));

        // We take every row of the results
        // and wrap them in cards wrapped in MessagingExtensionAttachment objects.
        // The Preview is optional, if it includes a Tap
        // that will trigger the onTeamsMessagingExtensionSelectItem event back on this bot.
        ThumbnailCard card = new ThumbnailCard();
        card.setTitle(data.get(0));
        card.setSubtitle(data.get(2));
        card.setButtons(Arrays.asList(cardAction));

        if (StringUtils.isNotBlank(data.get(4))) {
            CardImage cardImage = new CardImage();
            cardImage.setUrl(data.get(4));
            cardImage.setAlt("Icon");
            card.setImages(Collections.singletonList(cardImage));
        }

        MessagingExtensionAttachment attachment = new MessagingExtensionAttachment();
        attachment.setContentType(ThumbnailCard.CONTENTTYPE);
        attachment.setContent(card);

        MessagingExtensionResult composeExtension = new MessagingExtensionResult();
        composeExtension.setType("result");
        composeExtension.setAttachmentLayout("list");
        composeExtension.setAttachments(Collections.singletonList(attachment));
        return CompletableFuture.completedFuture(new MessagingExtensionResponse(composeExtension));
    }

    // Generate a set of substrings to illustrate the idea of a set of results coming back from a query.
    private CompletableFuture<List<String[]>> findPackages(String text) {
        return CompletableFuture.supplyAsync(() -> {
            OkHttpClient client = new OkHttpClient();
            Request request = new Request.Builder()
                .url(String
                    .format(
                        "https://azuresearch-usnc.nuget.org/query?q=id:%s&prerelease=true",
                        text
                    ))
                .build();

            List<String[]> filteredItems = new ArrayList<>();
            try {
                Response response = client.newCall(request).execute();
                JsonNode obj = Serialization.jsonToTree(response.body().string());
                ArrayNode dataArray = (ArrayNode) obj.get("data");

                for (int i = 0; i < dataArray.size(); i++) {
                    JsonNode item = dataArray.get(i);
                    filteredItems.add(new String[] {
                        item.get("id").asText(),
                        item.get("version").asText(),
                        item.get("description").asText(),
                        item.has("projectUrl") ? item.get("projectUrl").asText() : "",
                        item.has("iconUrl") ? item.get("iconUrl").asText() : ""
                    });
                }
            } catch (IOException e) {
                LoggerFactory.getLogger(TeamsMessagingExtensionsSearchBot.class)
                    .error("findPackages", e);
                throw new CompletionException(e);
            }
            return filteredItems;
        });
    }
}

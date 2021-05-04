// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.teamssearchauth;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.node.ArrayNode;
import com.fasterxml.jackson.databind.node.ObjectNode;
import com.microsoft.bot.builder.StatePropertyAccessor;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.UserState;
import com.microsoft.bot.builder.UserTokenProvider;
import com.microsoft.bot.builder.teams.TeamsActivityHandler;
import com.microsoft.bot.connector.ExecutorFactory;
import com.microsoft.bot.integration.Configuration;
import com.microsoft.bot.schema.*;
import com.microsoft.bot.schema.teams.*;
import com.microsoft.graph.models.extensions.Message;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;
import org.apache.commons.io.IOUtils;
import org.apache.commons.lang3.StringUtils;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.io.InputStream;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.util.*;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.CompletionException;
import java.util.concurrent.atomic.AtomicReference;

/**
 * This class implements the functionality of the Bot.
 *
 * <p>
 * This is where application specific logic for interacting with the users would be added. For this
 * sample, the {@link #onMessageActivity(TurnContext)} echos the text back to the user. The {@link
 * #onMembersAdded(List, TurnContext)} will send a greeting to new conversation participants.
 * </p>
 */
public class TeamsMessagingExtensionsSearchAuthConfigBot extends TeamsActivityHandler {

    private String siteUrl;
    private String connectionName;
    private UserState userState;
    private StatePropertyAccessor<String> userConfigProperty;

    public TeamsMessagingExtensionsSearchAuthConfigBot(
        Configuration configuration,
        UserState userState
    ) {
        connectionName = configuration.getProperty("ConnectionName");
        siteUrl = configuration.getProperty("SiteUrl");
        userConfigProperty = userState.createProperty("UserConfiguration");
        this.userState = userState;
    }

    @Override
    public CompletableFuture<Void> onTurn(TurnContext turnContext) {
        return super.onTurn(turnContext)
            .thenCompose(saveResult -> userState.saveChanges(turnContext));
    }

    @Override
    protected CompletableFuture<MessagingExtensionResponse> onTeamsMessagingExtensionConfigurationQuerySettingUrl(
        TurnContext turnContext,
        MessagingExtensionQuery query
    ) {
        return userConfigProperty.get(turnContext, () -> "").thenApply(userConfigSettings -> {
            AtomicReference<String> escapedSettings = new AtomicReference<>("");
            if (!StringUtils.isEmpty(userConfigSettings)) {
                try {
                    escapedSettings.set(
                        URLEncoder.encode(userConfigSettings, StandardCharsets.UTF_8.toString()));
                } catch (UnsupportedEncodingException e) {
                    escapedSettings.set(userConfigSettings);
                }
            }

            CardAction cardAction = new CardAction();
            cardAction.setType(ActionTypes.OPEN_URL);
            cardAction.setValue(String.format("%s/searchSettings.html?settings=%s", siteUrl, escapedSettings.get()));

            MessagingExtensionSuggestedAction suggestedAction = new MessagingExtensionSuggestedAction();
            suggestedAction.setAction(cardAction);

            MessagingExtensionResult result = new MessagingExtensionResult();
            result.setType("config");
            result.setSuggestedActions(suggestedAction);
            return new MessagingExtensionResponse(result);
        });
    }

    @Override
    protected CompletableFuture<Void> onTeamsMessagingExtensionConfigurationSetting(
        TurnContext turnContext,
        Object settings
    ) {
        if (settings != null) {
            Map<String, String> settingsData = (Map<String, String>) settings;
            String state = settingsData.get("state");
            return userConfigProperty.set(turnContext, state);
        }

        return CompletableFuture.completedFuture(null);
    }

    @Override
    protected CompletableFuture<MessagingExtensionResponse> onTeamsMessagingExtensionQuery(
        TurnContext turnContext,
        MessagingExtensionQuery query
    ) {
        return userConfigProperty.get(turnContext)
            .thenCompose(settings -> {
                if (settings != null && settings.toLowerCase().contains("email")) {
                    return emailExtensionQuery(turnContext, query);
                }

                return packageExtensionQuery(query);
            });
    }

    private CompletableFuture<MessagingExtensionResponse> packageExtensionQuery(
        MessagingExtensionQuery query
    ) {
        String search = "";
        if (query.getParameters() != null && !query.getParameters().isEmpty()) {
            search = (String) query.getParameters().get(0).getValue();
        }

        return findPackages(search).thenApply(packages -> {
            List<MessagingExtensionAttachment> attachments = new ArrayList<>();
            for (String[] item : packages) {
                ObjectNode data = Serialization.createObjectNode();
                data.set("data", Serialization.objectToTree(item));

                CardAction cardAction = new CardAction();
                cardAction.setType(ActionTypes.INVOKE);
                cardAction.setValue(data);
                ThumbnailCard previewCard = new ThumbnailCard();
                previewCard.setTitle(item[0]);
                previewCard.setTap(cardAction);

                if (!StringUtils.isEmpty(item[4])) {
                    CardImage cardImage = new CardImage();
                    cardImage.setUrl(item[4]);
                    cardImage.setAlt("Icon");
                    previewCard.setImage(cardImage);
                }

                HeroCard heroCard = new HeroCard();
                heroCard.setTitle(item[0]);
                MessagingExtensionAttachment attachment = new MessagingExtensionAttachment();
                attachment.setContentType(HeroCard.CONTENTTYPE);
                attachment.setContent(heroCard);
                attachment.setPreview(previewCard.toAttachment());

                attachments.add(attachment);
            }

            MessagingExtensionResult result = new MessagingExtensionResult();
            result.setType("result");
            result.setAttachmentLayout("list");
            result.setAttachments(attachments);
            return new MessagingExtensionResponse(result);
        });
    }

    private CompletableFuture<MessagingExtensionResponse> emailExtensionQuery(
        TurnContext turnContext,
        MessagingExtensionQuery query
    ) {
        // When the Bot Service Auth flow completes, the action.State will contain a
        // magic code used for verification.
        String magicCode = "";
        String state = query.getState();
        if (state != null && !state.isEmpty()) {
            magicCode = state;
        }

        UserTokenProvider tokenProvider = (UserTokenProvider) turnContext.getAdapter();
        return tokenProvider.getUserToken(turnContext, connectionName, magicCode)
            .thenCompose(response -> {
                if (response == null || StringUtils.isEmpty(response.getToken())) {
                    // There is no token, so the user has not signed in yet.

                    return tokenProvider.getOAuthSignInLink(turnContext, connectionName)
                        .thenApply(link -> {
                            MessagingExtensionResponse extensionResponse = new MessagingExtensionResponse();
                            CardAction cardAction = new CardAction();
                            cardAction.setType(ActionTypes.OPEN_URL);
                            cardAction.setValue(extensionResponse);
                            cardAction.setTitle("Bot Service OAuth");

                            MessagingExtensionSuggestedAction suggestedAction = new MessagingExtensionSuggestedAction();
                            suggestedAction.setAction(cardAction);

                            MessagingExtensionResult result = new MessagingExtensionResult();
                            result.setType("auth");
                            result.setSuggestedActions(suggestedAction);

                            extensionResponse.setComposeExtension(result);

                            return extensionResponse;
                        });
                }

                String search = "";
                if (query.getParameters() != null && !query.getParameters().isEmpty()) {
                    search = (String) query.getParameters().get(0).getValue();
                }

                return CompletableFuture.completedFuture(new MessagingExtensionResponse(searchMail(search, response.getToken())));
            });
    }

    @Override
    protected CompletableFuture<MessagingExtensionResponse> onTeamsMessagingExtensionSelectItem(
        TurnContext turnContext,
        Object query
    ) {
        Map cardValue = (Map) query;
        List<String> data = (ArrayList) cardValue.get("data");
        CardAction cardAction = new CardAction();
        cardAction.setType(ActionTypes.OPEN_URL);
        cardAction.setTitle("Project");
        cardAction.setValue(data.get(3));
        ThumbnailCard card = new ThumbnailCard();
        card.setTitle(data.get(0));
        card.setSubtitle(data.get(2));
        card.setButtons(Arrays.asList(cardAction));
        
        if (!StringUtils.isEmpty(data.get(4))) {
            CardImage cardImage = new CardImage();
            cardImage.setUrl(data.get(4));
            cardImage.setAlt("Icon");
            card.setImage(cardImage);
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

    @Override
    protected CompletableFuture<MessagingExtensionActionResponse> onTeamsMessagingExtensionSubmitAction(
        TurnContext turnContext,
        MessagingExtensionAction action
    ) {
        return CompletableFuture.completedFuture(new MessagingExtensionActionResponse());
    }

    @Override
    protected CompletableFuture<MessagingExtensionActionResponse> onTeamsMessagingExtensionFetchTask(
        TurnContext turnContext,
        MessagingExtensionAction action
    ) {
        if (action.getCommandId().toUpperCase().equals("SIGNOUTCOMMAND")) {
            UserTokenProvider tokenProvider = (UserTokenProvider) turnContext.getAdapter();

            return tokenProvider.signOutUser(
                turnContext,
                connectionName,
                turnContext.getActivity().getFrom().getId()
            ).thenApply(response -> {
                TaskModuleTaskInfo taskInfo = new TaskModuleTaskInfo();
                taskInfo.setCard(createAdaptiveCardAttachment());
                taskInfo.setHeight(200);
                taskInfo.setWidth(400);
                taskInfo.setTitle("Adaptive Card: Inputs");

                TaskModuleContinueResponse continueResponse = new TaskModuleContinueResponse();
                continueResponse.setValue(taskInfo);

                MessagingExtensionActionResponse actionResponse = new MessagingExtensionActionResponse();
                actionResponse.setTask(continueResponse);
                return actionResponse;
            });
        }

        return notImplemented();
    }

    private Attachment createAdaptiveCardAttachment() {
        try (
            InputStream input = Thread.currentThread().getContextClassLoader()
                .getResourceAsStream("adaptiveCard.json")
        ) {
            String adaptiveCardJson = IOUtils.toString(input, StandardCharsets.UTF_8.toString());

            Attachment attachment = new Attachment();
            attachment.setContentType("application/vnd.microsoft.card.adaptive");
            attachment.setContent(Serialization.jsonToTree(adaptiveCardJson));
            return attachment;
        } catch (IOException e) {
            (LoggerFactory.getLogger(TeamsMessagingExtensionsSearchAuthConfigBot.class))
                .error("Unable to createAdaptiveCardAttachment", e);
            return new Attachment();
        }
    }

    private MessagingExtensionResult searchMail(String text, String token) {
        SimpleGraphClient graph = new SimpleGraphClient(token);
        List<Message> messages = graph.searchMailInbox(text);

        List<MessagingExtensionAttachment> attachments = new ArrayList<>();
        for (Message msg : messages) {

            CardImage cardImage = new CardImage();
            cardImage.setUrl("https://raw.githubusercontent.com/microsoft/botbuilder-samples/master/docs/media/OutlookLogo.jpg");
            cardImage.setAlt("Outlook logo");

            HeroCard heroCard = new HeroCard();
            heroCard.setTitle(msg.from.emailAddress.address);
            heroCard.setSubtitle(msg.subject);
            heroCard.setText(msg.body.content);

            ThumbnailCard thumbnailCard = new ThumbnailCard();
            thumbnailCard.setTitle(msg.from.emailAddress.address);
            thumbnailCard.setText(String.format("%s<br />%s", msg.subject, msg.bodyPreview));
            thumbnailCard.setImage(cardImage);

            MessagingExtensionAttachment attachment = new MessagingExtensionAttachment();
            attachment.setContentType(HeroCard.CONTENTTYPE);
            attachment.setContent(heroCard);
            attachment.setPreview(thumbnailCard.toAttachment());

            attachments.add(attachment);
        }

        MessagingExtensionResult result = new MessagingExtensionResult();
        result.setType("result");
        result.setAttachmentLayout("list");
        result.setAttachments(attachments);

        return result;
    }

    private CompletableFuture<List<String[]>> findPackages(String text) {
        return CompletableFuture.supplyAsync(() -> {
            OkHttpClient client = new OkHttpClient();
            Request request = new Request.Builder().url(
                String.format(
                    "https://azuresearch-usnc.nuget.org/query?q=id:%s&prerelease=true",
                    text
                )
            ).build();

            List<String[]> filteredItems = new ArrayList<String[]>();
            try {
                Response response = client.newCall(request).execute();
                JsonNode obj = Serialization.jsonToTree(response.body().string());
                ArrayNode dataArray = (ArrayNode) obj.get("data");

                for (int i = 0; i < dataArray.size(); i++) {
                    JsonNode item = dataArray.get(i);
                    filteredItems.add(
                        new String[] {
                            item.get("id").asText(),
                            item.get("version").asText(),
                            item.get("description").asText(),
                            item.has("projectUrl") ? item.get("projectUrl").asText() : "",
                            item.has("iconUrl") ? item.get("iconUrl").asText() : ""
                        }
                    );
                }
            } catch (IOException e) {
                LoggerFactory.getLogger(TeamsMessagingExtensionsSearchAuthConfigBot.class)
                    .error("findPackages", e);
                throw new CompletionException(e);
            }
            return filteredItems;
        }, ExecutorFactory.getExecutor());
    }
}

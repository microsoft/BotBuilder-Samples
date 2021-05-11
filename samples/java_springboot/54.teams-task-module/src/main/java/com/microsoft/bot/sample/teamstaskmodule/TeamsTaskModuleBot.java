// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.teamstaskmodule;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.teams.TeamsActivityHandler;
import com.microsoft.bot.connector.Async;
import com.microsoft.bot.integration.Configuration;
import com.microsoft.bot.sample.teamstaskmodule.models.AdaptiveCardTaskFetchValue;
import com.microsoft.bot.sample.teamstaskmodule.models.CardTaskFetchValue;
import com.microsoft.bot.sample.teamstaskmodule.models.TaskModuleIds;
import com.microsoft.bot.sample.teamstaskmodule.models.TaskModuleResponseFactory;
import com.microsoft.bot.sample.teamstaskmodule.models.TaskModuleUIConstants;
import com.microsoft.bot.sample.teamstaskmodule.models.UISettings;
import com.microsoft.bot.schema.Attachment;
import com.microsoft.bot.schema.CardAction;
import com.microsoft.bot.schema.HeroCard;
import com.microsoft.bot.schema.Serialization;
import com.microsoft.bot.schema.teams.*;
import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;
import java.util.concurrent.CompletionException;
import java.util.stream.Collectors;
import org.apache.commons.io.IOUtils;

import java.io.InputStream;
import java.nio.charset.StandardCharsets;
import java.util.Arrays;
import java.util.List;
import java.util.concurrent.CompletableFuture;

/**
 * This class implements the functionality of the Bot.
 *
 * <p>This is where application specific logic for interacting with the users would be
 * added.  For this sample, the {@link #onMessageActivity(TurnContext)} echos the text back to the
 * user.  The {@link #onMembersAdded(List, TurnContext)} will send a greeting to new conversation
 * participants.</p>
 */
public class TeamsTaskModuleBot extends TeamsActivityHandler {
    private final String baseUrl;
    private final List<UISettings> actions = Arrays.asList(
        TaskModuleUIConstants.ADAPTIVECARD,
        TaskModuleUIConstants.CUSTOMFORM,
        TaskModuleUIConstants.YOUTUBE
    );

    public TeamsTaskModuleBot(Configuration configuration) {
        baseUrl = configuration.getProperty("BaseUrl");
    }

    @Override
    protected CompletableFuture<Void> onMessageActivity(
        TurnContext turnContext
    ) {
        // This displays two cards: A HeroCard and an AdaptiveCard.  Both have the same
        // options.  When any of the options are selected, `onTeamsTaskModuleFetch`
        // is called.
        return turnContext.sendActivity(MessageFactory.attachment(Arrays.asList(
                getTaskModuleHeroCardOptions(),
                getTaskModuleAdaptiveCardOptions()
            ))
        )
            .thenApply(resourceResponse -> null);
    }

    @Override
    protected CompletableFuture<TaskModuleResponse> onTeamsTaskModuleFetch(
        TurnContext turnContext,
        TaskModuleRequest taskModuleRequest
    ) {
        // Called when the user selects an options from the displayed HeroCard or
        // AdaptiveCard.  The result is the action to perform.
        return Async.wrapBlock(() -> {
            CardTaskFetchValue<String> value = Serialization
                .safeGetAs(taskModuleRequest.getData(), CardTaskFetchValue.class);

            TaskModuleTaskInfo taskInfo = new TaskModuleTaskInfo();
            switch (value.getData()) {
                // Display the YouTube.html page
                case TaskModuleIds.YOUTUBE: {
                    String url = baseUrl + "/" + TaskModuleIds.YOUTUBE + ".html";
                    taskInfo.setUrl(url);
                    taskInfo.setFallbackUrl(url);
                    setTaskInfo(taskInfo, TaskModuleUIConstants.YOUTUBE);
                    break;
                }

                // Display the CustomForm.html page, and post the form data back via
                // onTeamsTaskModuleSubmit.
                case TaskModuleIds.CUSTOMFORM: {
                    String url = baseUrl + "/" + TaskModuleIds.CUSTOMFORM + ".html";
                    taskInfo.setUrl(url);
                    taskInfo.setFallbackUrl(url);
                    setTaskInfo(taskInfo, TaskModuleUIConstants.CUSTOMFORM);
                    break;
                }

                // Display an AdaptiveCard to prompt user for text, and post it back via
                // onTeamsTaskModuleSubmit.
                case TaskModuleIds.ADAPTIVECARD:
                    taskInfo.setCard(createAdaptiveCardAttachment());
                    setTaskInfo(taskInfo, TaskModuleUIConstants.ADAPTIVECARD);
                    break;

                default:
                    break;
            }

            return taskInfo;
        })
            .thenApply(TaskModuleResponseFactory::toTaskModuleResponse);
    }

    @Override
    protected CompletableFuture<TaskModuleResponse> onTeamsTaskModuleSubmit(
        TurnContext turnContext,
        TaskModuleRequest taskModuleRequest
    ) {
        // Called when data is being returned from the selected option (see `onTeamsTaskModuleFetch').
        return Async.wrapBlock(() ->
            // Echo the users input back.  In a production bot, this is where you'd add behavior in
            // response to the input.
            MessageFactory.text(
                "onTeamsTaskModuleSubmit TaskModuleRequest: "
                    + new ObjectMapper().writeValueAsString(taskModuleRequest)
            )
        )
            .thenCompose(turnContext::sendActivity)
            .thenApply(resourceResponse -> TaskModuleResponseFactory.createResponse("Thanks!"));
    }

    private void setTaskInfo(TaskModuleTaskInfo taskInfo, UISettings uiSettings) {
        taskInfo.setHeight(uiSettings.getHeight());
        taskInfo.setWidth(uiSettings.getWidth());
        taskInfo.setTitle(uiSettings.getTitle());
    }

    private Attachment getTaskModuleHeroCardOptions() {
        List<CardAction> buttons = actions.stream().map(cardType -> {
            CardTaskFetchValue fetchValue = new CardTaskFetchValue<String>();
            fetchValue.setData(cardType.getId());
            TaskModuleAction moduleAction = new TaskModuleAction(cardType.getButtonTitle(), fetchValue);
            return moduleAction;
            }).collect(Collectors.toCollection(ArrayList::new));

        HeroCard card = new HeroCard();
        card.setTitle("Task Module Invocation from Hero Card");
        card.setButtons(buttons);
        return card.toAttachment();
    }

    private Attachment getTaskModuleAdaptiveCardOptions() {
        try (InputStream inputStream = getClass().getClassLoader()
            .getResourceAsStream("adaptiveTemplate.json")
        ) {
            String cardTemplate = IOUtils.toString(inputStream, StandardCharsets.UTF_8);

            List<Map<String,Object>> cardActions = actions.stream().map(cardType -> {
                AdaptiveCardTaskFetchValue fetchValue = new AdaptiveCardTaskFetchValue<String>();
                fetchValue.setData(cardType.getId());
                Map<String, Object> a = new HashMap<>();
                a.put("type", "Action.Submit");
                a.put("title", cardType.getButtonTitle());
                a.put("data", fetchValue);
                return a;
            }).collect(Collectors.toCollection(ArrayList::new));

            String adaptiveCardJson = String.format(cardTemplate, Serialization.toString(cardActions));

            return adaptiveCardAttachmentFromJson(adaptiveCardJson);
        } catch (Throwable t) {
            throw new CompletionException(t);
        }
    }

    private Attachment createAdaptiveCardAttachment() {
        try (InputStream inputStream = getClass().getClassLoader()
            .getResourceAsStream("adaptivecard.json")
        ) {
            String result = IOUtils.toString(inputStream, StandardCharsets.UTF_8);
            return adaptiveCardAttachmentFromJson(result);
        } catch (Throwable t) {
            throw new CompletionException(t);
        }
    }

    private Attachment adaptiveCardAttachmentFromJson(String json) throws IOException {
        Attachment attachment = new Attachment();
        attachment.setContentType("application/vnd.microsoft.card.adaptive");
        attachment.setContent(new ObjectMapper().readValue(json, ObjectNode.class));
        return attachment;
    }
}

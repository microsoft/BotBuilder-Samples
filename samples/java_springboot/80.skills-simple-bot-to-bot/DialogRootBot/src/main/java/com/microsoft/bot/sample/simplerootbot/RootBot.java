// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.simplerootbot;

import com.codepoetics.protonpack.collectors.CompletableFutures;
import com.microsoft.bot.builder.ActivityHandler;
import com.microsoft.bot.builder.ConversationState;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.StatePropertyAccessor;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.TypedInvokeResponse;
import com.microsoft.bot.builder.skills.BotFrameworkSkill;
import com.microsoft.bot.connector.authentication.MicrosoftAppCredentials;
import com.microsoft.bot.integration.Configuration;
import com.microsoft.bot.integration.SkillHttpClient;
import com.microsoft.bot.schema.ActivityTypes;
import com.microsoft.bot.schema.ChannelAccount;

import org.apache.commons.lang3.StringUtils;

import java.util.List;
import java.util.concurrent.CompletableFuture;

/**
 * This class implements the functionality of the Bot.
 *
 * <p>
 * This is where application specific logic for interacting with the users would be added. For this
 * sample, the {@link #onMessageActivity(TurnContext)} echos the text back to the user. The {@link
 * #onMembersAdded(List, TurnContext)} will send a greeting to new conversation participants.
 * </p>
 */
public class RootBot extends ActivityHandler {

    public static final String ActiveSkillPropertyName = "com.microsoft.bot.sample.simplerootbot.ActiveSkillProperty";
    private StatePropertyAccessor<BotFrameworkSkill> activeSkillProperty;
    private String botId;
    private ConversationState conversationState;
    private SkillHttpClient skillClient;
    private SkillsConfiguration skillsConfig;
    private BotFrameworkSkill targetSkill;

    public RootBot(
        ConversationState conversationState,
        SkillsConfiguration skillsConfig,
        SkillHttpClient skillClient,
        Configuration configuration
    ) {
        if (conversationState == null) {
            throw new  IllegalArgumentException("conversationState cannot be null.");
        }

        if (skillsConfig == null) {
            throw new  IllegalArgumentException("skillsConfig cannot be null.");
        }

        if (skillClient == null) {
            throw new  IllegalArgumentException("skillsClient cannot be null.");
        }

        if (configuration == null) {
            throw new  IllegalArgumentException("configuration cannot be null.");
        }


        this.conversationState = conversationState;
        this.skillsConfig = skillsConfig;
        this.skillClient = skillClient;

        botId = configuration.getProperty(MicrosoftAppCredentials.MICROSOFTAPPID);

        if (StringUtils.isEmpty(botId)) {
            throw new IllegalArgumentException(String.format("%s instanceof not set in configuration",
                                                             MicrosoftAppCredentials.MICROSOFTAPPID));
        }

        // We use a single skill in this example.
        String targetSkillId = "EchoSkillBot";
        if (!skillsConfig.getSkills().containsKey(targetSkillId)) {
            throw new IllegalArgumentException(
                String.format("Skill with ID \"%s\" not found in configuration", targetSkillId)
            );
        } else {
            targetSkill = (BotFrameworkSkill) skillsConfig.getSkills().get(targetSkillId);
        }

        // Create state property to track the active skill
        activeSkillProperty = conversationState.createProperty(ActiveSkillPropertyName);
    }

    @Override
    public CompletableFuture<Void> onTurn(TurnContext turnContext) {
        // Forward all activities except EndOfConversation to the skill.
        if (!turnContext.getActivity().getType().equals(ActivityTypes.END_OF_CONVERSATION)) {
            // Try to get the active skill
            BotFrameworkSkill activeSkill = activeSkillProperty.get(turnContext).join();
            if (activeSkill != null) {
                // Send the activity to the skill
                sendToSkill(turnContext, activeSkill).join();
                return CompletableFuture.completedFuture(null);
            }
        }

         super.onTurn(turnContext);

        // Save any state changes that might have occured during the turn.
        return conversationState.saveChanges(turnContext, false);
    }

    @Override
    protected CompletableFuture<Void> onMessageActivity(TurnContext turnContext) {
        if (turnContext.getActivity().getText().contains("skill")) {
            return turnContext.sendActivity(MessageFactory.text("Got it, connecting you to the skill..."))
            .thenCompose(result -> {
                activeSkillProperty.set(turnContext, targetSkill);
                // Send the activity to the skill
                return sendToSkill(turnContext, targetSkill);
            });
        }

        // just respond
         return turnContext.sendActivity(
             MessageFactory.text("Me no nothin'. Say \"skill\" and I'll patch you through"))
         .thenCompose(result -> conversationState.saveChanges(turnContext, true));
    }

    @Override
    protected CompletableFuture<Void> onEndOfConversationActivity(TurnContext turnContext) {
        // forget skill invocation
         return activeSkillProperty.delete(turnContext).thenAccept(result -> {
            // Show status message, text and value returned by the skill
            String eocActivityMessage = String.format("Received %s.\n\nCode: %s",
                                                       ActivityTypes.END_OF_CONVERSATION,
                                                       turnContext.getActivity().getCode());

            if (!StringUtils.isEmpty(turnContext.getActivity().getText())) {
                eocActivityMessage += String.format("\n\nText: %s", turnContext.getActivity().getText());
            }

            if (turnContext.getActivity() != null && turnContext.getActivity().getValue() != null) {
                eocActivityMessage += String.format("\n\nValue: %s", turnContext.getActivity().getValue());
            }

            turnContext.sendActivity(MessageFactory.text(eocActivityMessage)).thenCompose(sendResult ->{
                // We are back at the root
                return turnContext.sendActivity(
                MessageFactory.text("Back in the root bot. Say \"skill\" and I'll patch you through"))
                .thenCompose(secondSendResult-> conversationState.saveChanges(turnContext));
            });
         });
    }


    @Override
    protected CompletableFuture<Void> onMembersAdded(List<ChannelAccount> membersAdded, TurnContext turnContext) {
        return membersAdded.stream()
        .filter(
            member -> !StringUtils
                .equals(member.getId(), turnContext.getActivity().getRecipient().getId())
        ).map(channel -> turnContext.sendActivity(MessageFactory.text("Hello and welcome!")))
        .collect(CompletableFutures.toFutureList()).thenApply(resourceResponses -> null);
    }

    private CompletableFuture<Void> sendToSkill(TurnContext turnContext, BotFrameworkSkill targetSkill) {
        // NOTE: Always SaveChanges() before calling a skill so that any activity generated by the skill
        // will have access to current accurate state.
         return conversationState.saveChanges(turnContext, true)
         .thenAccept(result -> {
            // route the activity to the skill
            skillClient.postActivity(botId,
                                    targetSkill,
                                    skillsConfig.getSkillHostEndpoint(),
                                    turnContext.getActivity(),
                                    Object.class)
            .thenApply(response -> {
            // Check response status
            if (!(response.getStatus() >= 200 && response.getStatus() <= 299)) {
                    throw new RuntimeException(
                        String.format(
                            "Error invoking the skill id: \"%s\" at \"%s\" (status instanceof %s). \r\n %s",
                            targetSkill.getId(),
                            targetSkill.getSkillEndpoint(),
                            response.getStatus(),
                            response.getBody()));
                }
                return CompletableFuture.completedFuture(null);
            });
         });
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.teamsconversation;

import com.codepoetics.protonpack.collectors.CompletableFutures;
import com.microsoft.bot.builder.BotFrameworkAdapter;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.teams.TeamsActivityHandler;
import com.microsoft.bot.builder.teams.TeamsInfo;
import com.microsoft.bot.connector.authentication.MicrosoftAppCredentials;
import com.microsoft.bot.integration.Configuration;
import com.microsoft.bot.schema.ActionTypes;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.CardAction;
import com.microsoft.bot.schema.ConversationParameters;
import com.microsoft.bot.schema.ConversationReference;
import com.microsoft.bot.schema.HeroCard;
import com.microsoft.bot.schema.Mention;
import com.microsoft.bot.schema.teams.TeamInfo;
import com.microsoft.bot.schema.teams.TeamsChannelAccount;
import org.apache.commons.lang3.StringUtils;

import java.net.URLEncoder;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.List;
import java.util.Map;
import java.util.concurrent.CompletableFuture;

/**
 * This class implements the functionality of the Bot.
 *
 * <p>
 * This is where application specific logic for interacting with the users would
 * be added. For this sample, the {@link #onMessageActivity(TurnContext)} echos
 * the text back to the user. The {@link #onMembersAdded(List, TurnContext)}
 * will send a greeting to new conversation participants.
 * </p>
 */
public class TeamsConversationBot extends TeamsActivityHandler {
    private String appId;
    private String appPassword;

    public TeamsConversationBot(Configuration configuration) {
        appId = configuration.getProperty("MicrosoftAppId");
        appPassword = configuration.getProperty("MicrosoftAppPassword");
    }

    @Override
    protected CompletableFuture<Void> onMessageActivity(TurnContext turnContext) {
        turnContext.getActivity().removeRecipientMention();

        switch (turnContext.getActivity().getText().trim()) {
            case "MentionMe":
                return mentionActivity(turnContext);

            case "UpdateCardAction":
                return updateCardActivity(turnContext);

            case "Delete":
                return deleteCardActivity(turnContext);

            case "MessageAllMembers":
                return messageAllMembers(turnContext);

            default:
                // This will come back deserialized as a Map.
                Object value = new Object() {
                    int count = 0;
                };

                HeroCard card = new HeroCard();
                card.setTitle("Welcome Card");
                card.setText("Click the buttons below to update this card");
                card.setButtons(getHeroCardButtons(value));

                return turnContext.sendActivity(MessageFactory.attachment(card.toAttachment()))
                    .thenApply(resourceResponse -> null);
        }
    }

    @Override
    protected CompletableFuture<Void> onTeamsMembersAdded(
        List<TeamsChannelAccount> membersAdded,
        TeamInfo teamInfo,
        TurnContext turnContext
    ) {
        return membersAdded.stream()
            .filter(
                member -> !StringUtils
                    .equals(member.getId(), turnContext.getActivity().getRecipient().getId())
            )
            .map(
                channel -> turnContext.sendActivity(
                    MessageFactory.text(
                        "Welcome to the team " + channel.getGivenName() + " " + channel.getSurname()
                            + "."
                    )
                )
            )
            .collect(CompletableFutures.toFutureList())
            .thenApply(resourceResponses -> null);
    }

    private CompletableFuture<Void> deleteCardActivity(TurnContext turnContext) {
        return turnContext.deleteActivity(turnContext.getActivity().getReplyToId());
    }

    // If you encounter permission-related errors when sending this message, see
    // https://aka.ms/BotTrustServiceUrl
    private CompletableFuture<Void> messageAllMembers(TurnContext turnContext) {
        String teamsChannelId = turnContext.getActivity().teamsGetChannelId();
        String serviceUrl = turnContext.getActivity().getServiceUrl();
        MicrosoftAppCredentials credentials = new MicrosoftAppCredentials(appId, appPassword);

        return TeamsInfo.getMembers(turnContext).thenCompose(members -> {
            List<CompletableFuture<Void>> conversations = new ArrayList<>();

            // Send a message to each member. These will all go out
            // at the same time.
            for (TeamsChannelAccount member : members) {
                Activity proactiveMessage = MessageFactory.text(
                    "Hello " + member.getGivenName() + " " + member.getSurname()
                        + ". I'm a Teams conversation bot."
                );

                ConversationParameters conversationParameters = new ConversationParameters();
                conversationParameters.setIsGroup(false);
                conversationParameters.setBot(turnContext.getActivity().getRecipient());
                conversationParameters.setMembers(Collections.singletonList(member));
                conversationParameters.setTenantId(turnContext.getActivity().getConversation().getTenantId());

                conversations.add(
                    ((BotFrameworkAdapter) turnContext.getAdapter()).createConversation(
                        teamsChannelId, serviceUrl, credentials, conversationParameters,
                        (context) -> {
                            ConversationReference reference =
                                context.getActivity().getConversationReference();
                            return context.getAdapter()
                                .continueConversation(
                                    appId, reference, (inner_context) -> inner_context
                                        .sendActivity(proactiveMessage)
                                        .thenApply(resourceResponse -> null)
                                );
                        }
                    )
                );
            }

            return CompletableFuture.allOf(conversations.toArray(new CompletableFuture[0]));
        })
            // After all member messages are sent, send confirmation to the user.
            .thenApply(
                conversations -> turnContext
                    .sendActivity(MessageFactory.text("All messages have been sent."))
            )
            .thenApply(allSent -> null);
    }

    private CompletableFuture<Void> updateCardActivity(TurnContext turnContext) {
        Map data = (Map) turnContext.getActivity().getValue();
        data.put("count", (int) data.get("count") + 1);

        HeroCard card = new HeroCard();
        card.setTitle("Welcome Card");
        card.setText("Update count - " + data.get("count"));
        card.setButtons(getHeroCardButtons(data));

        Activity updatedActivity = MessageFactory.attachment(card.toAttachment());
        updatedActivity.setId(turnContext.getActivity().getReplyToId());

        return turnContext.updateActivity(updatedActivity).thenApply(resourceResponse -> null);
    }

    private List<CardAction> getHeroCardButtons(Object value) {
        CardAction updateAction = new CardAction();
        updateAction.setType(ActionTypes.MESSAGE_BACK);
        updateAction.setTitle("Update Card");
        updateAction.setText("UpdateCardAction");
        updateAction.setValue(value);

        CardAction allMembersAction = new CardAction();
        allMembersAction.setType(ActionTypes.MESSAGE_BACK);
        allMembersAction.setTitle("Message All Members");
        allMembersAction.setText("MessageAllMembers");

        CardAction deleteAction = new CardAction();
        deleteAction.setType(ActionTypes.MESSAGE_BACK);
        deleteAction.setTitle("Delete card");
        deleteAction.setText("Delete");

        CardAction mentionAction = new CardAction();
        mentionAction.setType(ActionTypes.MESSAGE_BACK);
        mentionAction.setTitle("Who am I?");
        mentionAction.setText("MentionMe");

        return Arrays.asList(updateAction, allMembersAction, deleteAction, mentionAction);
    }

    private CompletableFuture<Void> mentionActivity(TurnContext turnContext) {
        Mention mention = new Mention();
        mention.setMentioned(turnContext.getActivity().getFrom());
        mention.setText(
            "<at>" + URLEncoder.encode(turnContext.getActivity().getFrom().getName()) + "</at>"
        );

        Activity replyActivity = MessageFactory.text("Hello " + mention.getText() + ".");
        replyActivity.setMentions(Collections.singletonList(mention));

        return turnContext.sendActivity(replyActivity).thenApply(resourceResponse -> null);
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.teamsstartnewthread;

import com.fasterxml.jackson.databind.node.JsonNodeFactory;
import com.fasterxml.jackson.databind.node.ObjectNode;
import com.microsoft.bot.builder.BotFrameworkAdapter;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.builder.teams.TeamsActivityHandler;
import com.microsoft.bot.connector.authentication.MicrosoftAppCredentials;
import com.microsoft.bot.integration.Configuration;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.ConversationParameters;
import com.microsoft.bot.schema.ConversationReference;

import java.util.concurrent.CompletableFuture;

/**
 * This class implements the functionality of the Bot.
 *
 * <p>This is where application specific logic for interacting with the users would be
 * added.  For this sample, the {@link #onMessageActivity(TurnContext)} creates a thread in a Teams channel.
 * </p>
 */
public class TeamsStartNewThreadBot extends TeamsActivityHandler {

    private String appId;
    private String appPassword;

    public TeamsStartNewThreadBot(Configuration configuration) {
        appId = configuration.getProperty("MicrosoftAppId");
        appPassword = configuration.getProperty("MicrosoftAppPassword");
    }

    @Override
    protected CompletableFuture<Void> onMessageActivity(
        TurnContext turnContext
    ) {

        String teamsChannelId = turnContext.getActivity().teamsGetChannelId();
        Activity message = MessageFactory.text("This will start a new thread in a channel");
        String serviceUrl = turnContext.getActivity().getServiceUrl();
        MicrosoftAppCredentials credentials = new MicrosoftAppCredentials(appId, appPassword);

        ObjectNode channelData = JsonNodeFactory.instance.objectNode();
        channelData.set(
            "channel",
            JsonNodeFactory.instance.objectNode()
                .set("id", JsonNodeFactory.instance.textNode(teamsChannelId))
        );

        ConversationParameters conversationParameters = new ConversationParameters();
        conversationParameters.setIsGroup(true);
        conversationParameters.setActivity(message);
        conversationParameters.setChannelData(channelData);

        BotFrameworkAdapter adapter = (BotFrameworkAdapter)turnContext.getAdapter();

        return adapter.createConversation(teamsChannelId,
            serviceUrl,
            credentials,
            conversationParameters,
            (tc) -> {
                ConversationReference reference = tc.getActivity().getConversationReference();
                return tc.getAdapter().continueConversation(
                    appId,
                    reference,
                    (continue_tc) -> continue_tc.sendActivity(
                        MessageFactory.text(
                            "This will be the first response to the new thread"
                        )
                    ).thenApply(resourceResponse -> null)
                );
            }
        ).thenApply(started -> null);
    }

}

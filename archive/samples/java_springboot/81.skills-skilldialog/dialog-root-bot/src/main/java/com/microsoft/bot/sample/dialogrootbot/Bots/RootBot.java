// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.dialogrootbot.bots;

import java.io.IOException;
import java.io.InputStream;
import java.io.StringWriter;
import java.nio.charset.StandardCharsets;
import java.util.List;
import java.util.concurrent.CompletableFuture;

import com.codepoetics.protonpack.collectors.CompletableFutures;
import com.microsoft.bot.builder.ActivityHandler;
import com.microsoft.bot.builder.ConversationState;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.dialogs.Dialog;
import com.microsoft.bot.restclient.serializer.JacksonAdapter;
import com.microsoft.bot.sample.dialogrootbot.AdaptiveCard;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.ActivityTypes;
import com.microsoft.bot.schema.Attachment;
import com.microsoft.bot.schema.ChannelAccount;

import org.apache.commons.io.IOUtils;
import org.apache.commons.io.input.BOMInputStream;
import org.apache.commons.lang3.StringUtils;
import org.slf4j.LoggerFactory;

public class RootBot<T extends Dialog> extends ActivityHandler {
    private final ConversationState conversationState;
    private final Dialog mainDialog;

    public RootBot(ConversationState conversationState, T mainDialog) {
        this.conversationState = conversationState;
        this.mainDialog = mainDialog;
    }

    @Override
    public CompletableFuture<Void> onTurn(TurnContext turnContext) {
        return handleTurn(turnContext).thenCompose(result -> conversationState.saveChanges(turnContext, false));
    }

    private CompletableFuture<Void> handleTurn(TurnContext turnContext) {
        if (!turnContext.getActivity().getType().equals(ActivityTypes.CONVERSATION_UPDATE)) {
            // Run the Dialog with the Activity.
            return Dialog.run(mainDialog, turnContext, conversationState.createProperty("DialogState"));
        } else {
            // Let the super.class handle the activity.
            return super.onTurn(turnContext);
        }
    }

    @Override
    protected CompletableFuture<Void> onMembersAdded(List<ChannelAccount> membersAdded, TurnContext turnContext) {

        Attachment welcomeCard = createAdaptiveCardAttachment("welcomeCard.json");
        Activity activity = MessageFactory.attachment(welcomeCard);
        activity.setSpeak("Welcome to the Dialog Skill Prototype!");

        return membersAdded.stream()
            .filter(member -> !StringUtils.equals(member.getId(), turnContext.getActivity().getRecipient().getId()))
            .map(channel -> runWelcome(turnContext, activity))
            .collect(CompletableFutures.toFutureList())
            .thenApply(resourceResponses -> null);
    }

    private CompletableFuture<Void> runWelcome(TurnContext turnContext, Activity activity) {
        return turnContext.sendActivity(activity).thenAccept(resourceResponses -> {
            Dialog.run(mainDialog, turnContext, conversationState.createProperty("DialogState"));
        });
    }

    // Load attachment from embedded resource.
    private Attachment createAdaptiveCardAttachment(String fileName) {
        try {
            InputStream input = getClass().getClassLoader().getResourceAsStream(fileName);
            BOMInputStream bomIn = new BOMInputStream(input);
            String content;
            StringWriter writer = new StringWriter();
            IOUtils.copy(bomIn, writer, StandardCharsets.UTF_8);
            content = writer.toString();
            bomIn.close();
            Attachment attachment = new Attachment();
            attachment.setContentType("application/vnd.microsoft.card.adaptive");
            attachment.setContent(new JacksonAdapter().serializer().readValue(content, AdaptiveCard.class));

            return attachment;
        } catch (IOException e) {
            LoggerFactory.getLogger(RootBot.class).error("createAdaptiveCardAttachment", e);
        }
        return new Attachment();
    }

}

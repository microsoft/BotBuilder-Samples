// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.attachments;

import com.codepoetics.protonpack.collectors.CompletableFutures;
import com.microsoft.bot.builder.ActivityHandler;
import com.microsoft.bot.builder.BotFrameworkAdapter;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.connector.Async;
import com.microsoft.bot.connector.Attachments;
import com.microsoft.bot.connector.ConnectorClient;
import com.microsoft.bot.schema.ActionTypes;
import com.microsoft.bot.schema.Activity;
import com.microsoft.bot.schema.Attachment;
import com.microsoft.bot.schema.AttachmentData;
import com.microsoft.bot.schema.CardAction;
import com.microsoft.bot.schema.ChannelAccount;
import com.microsoft.bot.schema.HeroCard;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.net.URL;
import java.nio.channels.Channels;
import java.nio.channels.ReadableByteChannel;
import java.util.Base64;
import org.apache.commons.io.IOUtils;
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
public class AttachmentsBot extends ActivityHandler {

    @Override
    protected CompletableFuture<Void> onMessageActivity(TurnContext turnContext) {
        return processInput(turnContext)
            .thenCompose(turnContext::sendActivity)
            .thenCompose(resourceResponse -> displayOptions(turnContext));
    }

    @Override
    protected CompletableFuture<Void> onMembersAdded(
        List<ChannelAccount> membersAdded,
        TurnContext turnContext
    ) {
        return sendWelcomeMessage(turnContext);
    }

    // Greet the user and give them instructions on how to interact with the bot.
    private CompletableFuture<Void> sendWelcomeMessage(TurnContext turnContext) {
        return turnContext.getActivity().getMembersAdded().stream()
            .filter(
                member -> !StringUtils
                    .equals(member.getId(), turnContext.getActivity().getRecipient().getId())
            ).map(member -> {
                String msg = String.format(
                    "Welcome to AttachmentsBot %s. This bot will "
                        + "introduce you to Attachments. Please select an option",
                    member.getName()
                );
                return turnContext.sendActivity(MessageFactory.text(msg))
                    .thenCompose(resourceResponse -> displayOptions(turnContext));
            })
            .collect(CompletableFutures.toFutureList()).thenApply(resourceResponses -> null);
    }

    private CompletableFuture<Void> displayOptions(TurnContext turnContext) {
        // Create a HeroCard with options for the user to interact with the bot.
        HeroCard card = new HeroCard();
        card.setText("You can upload an image or select one of the following choices");

        // Note that some channels require different values to be used in order to get buttons to display text.
        // In this code the emulator is accounted for with the 'title' parameter, but in other channels you may
        // need to provide a value for other parameters like 'text' or 'displayText'.
        card.setButtons(
            new CardAction(ActionTypes.IM_BACK, "1. Inline Attachment", "1"),
            new CardAction(ActionTypes.IM_BACK, "2. Internet Attachment", "2"),
            new CardAction(ActionTypes.IM_BACK, "3. Uploaded Attachment", "3")
        );

        Activity reply = MessageFactory.attachment(card.toAttachment());
        return turnContext.sendActivity(reply).thenApply(resourceResponse -> null);
    }

    // Given the input from the message, create the response.
    private CompletableFuture<Activity> processInput(TurnContext turnContext) {
        Activity activity = turnContext.getActivity();

        if (activity.getAttachments() != null && !activity.getAttachments().isEmpty()) {
            // We know the user is sending an attachment as there is at least one item
            // in the Attachments list.
            return CompletableFuture.completedFuture(handleIncomingAttachment(activity));
        }

        return handleOutgoingAttachment(turnContext, activity);
    }

    private CompletableFuture<Activity> handleOutgoingAttachment(TurnContext turnContext, Activity activity) {
        CompletableFuture<Activity> result;

        if (activity.getText().startsWith("1")) {
            result = getInlineAttachment()
                .thenApply(attachment -> {
                    Activity reply = MessageFactory.text("This is an inline attachment.");
                    reply.setAttachment(attachment);
                    return reply;
                });
        } else if (activity.getText().startsWith("2")) {
            result = getInternetAttachment()
                .thenApply(attachment -> {
                    Activity reply = MessageFactory.text("This is an attachment from a HTTP URL.");
                    reply.setAttachment(attachment);
                    return reply;
                });
        } else if (activity.getText().startsWith("3")) {
            // Get the uploaded attachment.
            result = getUploadedAttachment(
                turnContext, activity.getServiceUrl(), activity.getConversation().getId())
                .thenApply(attachment -> {
                    Activity reply = MessageFactory.text("This is an uploaded attachment.");
                    reply.setAttachment(attachment);
                    return reply;
                });
        } else {
            result = CompletableFuture.completedFuture(
                MessageFactory.text("Your input was not recognized please try again.")
            );
        }

        return result
            .exceptionally(ex -> MessageFactory.text(
                "There was an error handling the attachment: " + ex.getMessage())
            );
    }

    // Handle attachments uploaded by users. The bot receives an Attachment in an Activity.
    // The activity has a "List<T>" of attachments.
    // Not all channels allow users to upload files. Some channels have restrictions
    // on file type, size, and other attributes. Consult the documentation for the channel for
    // more information. For example Skype's limits are here
    // <see ref="https://support.skype.com/en/faq/FA34644/skype-file-sharing-file-types-size-and-time-limits"/>.
    private Activity handleIncomingAttachment(Activity activity) {
        String replyText = "";
        for (Attachment file : activity.getAttachments()) {
            ReadableByteChannel remoteChannel = null;
            FileOutputStream fos = null;

            try {
                // Determine where the file is hosted.
                URL remoteFileUrl = new URL(file.getContentUrl());

                // Save the attachment to the local system
                String localFileName = file.getName();

                // Download the actual attachment
                remoteChannel = Channels.newChannel(remoteFileUrl.openStream());
                fos = new FileOutputStream(localFileName);
                fos.getChannel().transferFrom(remoteChannel, 0, Long.MAX_VALUE);
            } catch (Throwable t) {
                replyText += "Attachment \"" + file.getName() + "\" failed to download.\r\n";
            } finally {
                if (remoteChannel != null) {
                    try {remoteChannel.close(); } catch (Throwable ignored) {};
                }
                if (fos != null) {
                    try {fos.close(); } catch (Throwable ignored) {};
                }
            }
        }

        return MessageFactory.text(replyText);
    }

    // Creates an inline attachment sent from the bot to the user using a base64 string.
    // Using a base64 string to send an attachment will not work on all channels.
    // Additionally, some channels will only allow certain file types to be sent this way.
    // For example a .png file may work but a .pdf file may not on some channels.
    // Please consult the channel documentation for specifics.
    private CompletableFuture<Attachment> getInlineAttachment() {
        return getEncodedFileData("architecture-resize.png")
            .thenApply(encodedFileData -> {
                Attachment attachment = new Attachment();
                attachment.setName("architecture-resize.png");
                attachment.setContentType("image/png");
                attachment.setContentUrl("data:image/png;base64," + encodedFileData);
                return attachment;
            });
    }

    // Creates an Attachment to be sent from the bot to the user from a HTTP URL.
    private CompletableFuture<Attachment> getInternetAttachment() {
        Attachment attachment = new Attachment();
        attachment.setName("architecture-resize.png");
        attachment.setContentType("image/png");
        attachment.setContentUrl("https://docs.microsoft.com/en-us/bot-framework/media/how-it-works/architecture-resize.png");
        return CompletableFuture.completedFuture(attachment);
    }

    private CompletableFuture<Attachment> getUploadedAttachment(TurnContext turnContext, String serviceUrl, String conversationId) {
        if (StringUtils.isEmpty(serviceUrl)) {
            return Async.completeExceptionally(new IllegalArgumentException("serviceUrl"));
        }
        if (StringUtils.isEmpty(conversationId)) {
            return Async.completeExceptionally(new IllegalArgumentException("conversationId"));
        }

        ConnectorClient connector = turnContext.getTurnState()
            .get(BotFrameworkAdapter.CONNECTOR_CLIENT_KEY);
        Attachments attachments = connector.getAttachments();

        return getFileData("architecture-resize.png")
            .thenCompose(fileData -> {
                AttachmentData attachmentData = new AttachmentData();
                attachmentData.setName("architecture-resize.png");
                attachmentData.setType("image/png");
                attachmentData.setOriginalBase64(fileData);

                return connector.getConversations().uploadAttachment(conversationId, attachmentData)
                    .thenApply(response -> {
                        String attachmentUri = attachments.getAttachmentUri(response.getId());

                        Attachment attachment = new Attachment();
                        attachment.setName("architecture-resize.png");
                        attachment.setContentType("image/png");
                        attachment.setContentUrl(attachmentUri);

                        return attachment;
                    });
            });
    }

    private CompletableFuture<String> getEncodedFileData(String filename) {
        return getFileData(filename)
            .thenApply(fileData -> Base64.getEncoder().encodeToString(fileData));
    }

    private CompletableFuture<byte[]> getFileData(String filename) {
        try (InputStream inputStream = Thread.currentThread().getContextClassLoader().getResourceAsStream(filename)) {
            return CompletableFuture.completedFuture(IOUtils.toByteArray(inputStream));
        } catch (Throwable t) {
            return Async.completeExceptionally(t);
        }
    }
}

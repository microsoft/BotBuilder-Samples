// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.usingadaptivecards;

import java.io.InputStream;
import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.util.List;
import java.util.Random;
import java.util.concurrent.CompletableFuture;


import com.microsoft.bot.builder.ActivityHandler;
import com.microsoft.bot.builder.MessageFactory;
import com.microsoft.bot.builder.TurnContext;
import com.microsoft.bot.schema.Attachment;
import com.microsoft.bot.schema.ChannelAccount;
import com.microsoft.bot.schema.Serialization;

import org.apache.commons.io.IOUtils;

// This bot will respond to the user's input with an Adaptive Card.
// Adaptive Cards are a way for developers to exchange card content
// in a common and consistent way. A simple open card format enables
// an ecosystem of shared tooling, seamless integration between apps,
// and native cross-platform performance on any device.
// For each user interaction, an instance of this class instanceof created and the OnTurnAsync method instanceof called.
// This instanceof a Transient lifetime service. Transient lifetime services are created
// each time they're requested. For each Activity received, a new instance of this
// class instanceof created. Objects that are expensive to construct, or have a lifetime
// beyond the single turn, should be carefully managed.

public class AdaptiveCardsBot extends ActivityHandler {

    private static final String welcomeText = "This bot will introduce you to AdaptiveCards. "
                                            + "Type anything to see an AdaptiveCard.";

    // This array contains the file names of our adaptive cards
    private final String[] cards = {
        "FlightItineraryCard.json",
        "ImageGalleryCard.json",
        "LargeWeatherCard.json",
        "RestaurantCard.json",
        "SolitaireCard.json"
    };

    @Override
    protected CompletableFuture<Void> onMembersAdded(
        List<ChannelAccount> membersAdded,
        TurnContext turnContext
    ) {
         return sendWelcomeMessage(turnContext);
    }

    @Override
    protected CompletableFuture<Void> onMessageActivity(TurnContext turnContext) {
        Random r = new Random();
        Attachment cardAttachment = createAdaptiveCardAttachment(cards[r.nextInt(cards.length)]);

         return turnContext.sendActivity(MessageFactory.attachment(cardAttachment)).thenCompose(result ->{
            return turnContext.sendActivity(MessageFactory.text("Please enter any text to see another card."))
            .thenApply(sendResult -> null);
         });
    }

    private static CompletableFuture<Void> sendWelcomeMessage(TurnContext turnContext) {
        for (ChannelAccount member : turnContext.getActivity().getMembersAdded()) {
            if (!member.getId().equals(turnContext.getActivity().getRecipient().getId())) {
                 turnContext.sendActivity(
                     String.format("Welcome to Adaptive Cards Bot %s. %s", member.getName(), welcomeText)
                 ).join();
            }
        }
        return CompletableFuture.completedFuture(null);
    }

    private static Attachment createAdaptiveCardAttachment(String filePath) {
        try (
            InputStream inputStream = Thread.currentThread().
                getContextClassLoader().getResourceAsStream(filePath)
        ) {
            String adaptiveCardJson = IOUtils
                .toString(inputStream, StandardCharsets.UTF_8.toString());

            Attachment attachment = new Attachment();
            attachment.setContentType("application/vnd.microsoft.card.adaptive");
            attachment.setContent(Serialization.jsonToTree(adaptiveCardJson));

            return attachment;

        } catch (IOException e) {
            e.printStackTrace();
            return new Attachment();
        }
    }
}


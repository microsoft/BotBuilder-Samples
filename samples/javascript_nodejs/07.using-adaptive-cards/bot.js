// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { CardFactory } = require('botbuilder');

// Import AdaptiveCard content.
const FlightItineraryCard = require('./resources/FlightItineraryCard.json');
const ImageGalleryCard = require('./resources/ImageGalleryCard.json');
const LargeWeatherCard = require('./resources/LargeWeatherCard.json');
const RestaurantCard = require('./resources/RestaurantCard.json');
const SolitaireCard = require('./resources/SolitaireCard.json');

// Create array of AdaptiveCard content, this will be used to send a random card to the user.
const CARDS = [
    FlightItineraryCard,
    ImageGalleryCard,
    LargeWeatherCard,
    RestaurantCard,
    SolitaireCard
];

/**
 * A bot that sends AdaptiveCards to the user when it receives a message.
 */
class AdaptiveCardsBot {
    /**
     * Every conversation turn for our AdaptiveCardsBot will call this method.
     * There are no dialogs used, since it's "single turn" processing, meaning a single
     * request and response, with no stateful conversation.
     * @param turnContext A TurnContext instance containing all the data needed for processing this conversation turn.
     */
    async onTurn(context) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (context.activity.type === 'message') {
            const randomlySelectedCard = CARDS[Math.floor((Math.random() * CARDS.length - 1) + 1)];
            await context.sendActivity({
                text: 'Here is an Adaptive Card:',
                attachments: [CardFactory.adaptiveCard(randomlySelectedCard)]
            });
        } else {
            await context.sendActivity(`[${ context.activity.type } event detected]`);
        }
    }
}

exports.AdaptiveCardsBot = AdaptiveCardsBot;

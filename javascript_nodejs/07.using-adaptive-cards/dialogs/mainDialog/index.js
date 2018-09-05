// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { CardFactory } = require('botbuilder');

// Adaptive Card content
const FlightItineraryCard = require('./Resources/FlightItineraryCard.json');
const ImageGalleryCard = require('./Resources/ImageGalleryCard.json');
const LargeWeatherCard = require('./Resources/LargeWeatherCard.json');
const RestaurantCard = require('./Resources/RestaurantCard.json');
const SolitaireCard = require('./Resources/SolitaireCard.json');

const CARDS = [
    FlightItineraryCard,
    ImageGalleryCard,
    LargeWeatherCard,
    RestaurantCard,
    SolitaireCard
];

class MainDialog {
    /**
     * 
     * @param {Object} context on turn context object.
     */
    async onTurn(context) {
        // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
        if (context.activity.type === 'message') {
            const randomlySelectedCard = CARDS[Math.floor((Math.random() * CARDS.length-1) + 1)];
            await context.sendActivity({
                text: 'Here is an Adaptive Card:',
                attachments: [CardFactory.adaptiveCard(randomlySelectedCard)]
            });
        }
        else {
            await context.sendActivity(`[${context.activity.type} event detected]`);
        }
    }
}

module.exports = MainDialog;
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    ActivityTypes,
    ConversationState,
    MemoryStorage
} from 'botbuilder-core';
import './css/app.css';
import { WebChatAdapter } from './webChatAdapter';
import { renderWebChat } from 'botframework-webchat';

// Create the custom WebChatAdapter.
const webChatAdapter = new WebChatAdapter();

// Connect our BotFramework-WebChat instance with the DOM.

renderWebChat({
    directLine: webChatAdapter.botConnection
}, document.getElementById('webchat')
);
// Instantiate MemoryStorage for use with the ConversationState class.
const memory = new MemoryStorage();

// Add the instantiated storage into ConversationState.
const conversationState = new ConversationState(memory);

// Create a property to keep track of how many messages are received from the user.
const countProperty = conversationState.createProperty('turnCounter');

// Register the business logic of the bot through the WebChatAdapter's processActivity implementation.
webChatAdapter.processActivity(async turnContext => {
    // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
    if (turnContext.activity.type === ActivityTypes.Message) {
        // Read from state.
        let count = await countProperty.get(turnContext);
        count = count === undefined ? 1 : count;
        await turnContext.sendActivity(
            `${ count }: You said "${ turnContext.activity.text }"`
        );
        // Increment and set turn counter.
        await countProperty.set(turnContext, ++count);
    } else {
        await turnContext.sendActivity(
            `[${ turnContext.activity.type } event detected]`
        );
    }
    await conversationState.saveChanges(turnContext);
});

// Create user and bot profiles.
export const USER_PROFILE = { id: 'Me!', name: 'Me!', role: 'user' };
export const BOT_PROFILE = { id: 'bot', name: 'bot', role: 'bot' };

// Prevent Flash of Unstyled Content (FOUC): https://en.wikipedia.org/wiki/Flash_of_unstyled_content
document.addEventListener('DOMContentLoaded', () => {
    window.requestAnimationFrame(() => {
        document.body.style.visibility = 'visible';
        // After the content has finished loading, send the bot a "conversationUpdate" Activity with the user's information.
        // When the bot receives a "conversationUpdate" Activity, the developer can opt to send a welcome message to the user.
        webChatAdapter.botConnection.postActivity({
            recipient: BOT_PROFILE,
            membersAdded: [USER_PROFILE],
            type: ActivityTypes.ConversationUpdate
        });
    });
});

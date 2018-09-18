// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { BotStateSet, ConversationState, MemoryStorage, UserState, ActivityTypes } from 'botbuilder-core';
import 'botframework-webchat/botchat.css';
import { App } from 'botframework-webchat/built/App';
import './css/app.css';
import { WebChatAdapter } from './webChatAdapter';

// Instantiate MemoryStorage for use with the ConversationState middleware.
const memory = new MemoryStorage();

// Create the custom WebChatAdapter and add the ConversationState middleware.
const webChatAdapter = new WebChatAdapter();

// Connect our BotFramework-WebChat App instance with the DOM.
App({
    user: { id: "Me!" },
    bot: { id: "bot" },
    botConnection: webChatAdapter.botConnection,
}, document.getElementById('bot'));

// Add the instantiated storage into state middleware.
const conversationState = new ConversationState(memory);
webChatAdapter.use(conversationState);

// Create a property to keep track of how many messages are received from the user.
const countProperty = conversationState.createProperty('turnCounter');

// Register the business logic of the bot through the WebChatAdapter's processActivity implementation.
webChatAdapter.processActivity(async (turnContext) => {
    // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
    if (turnContext.activity.type === ActivityTypes.Message) {
        // Read from state.
        let count = await countProperty.get(turnContext);
        count = count === undefined ? 1 : count;
        await turnContext.sendActivity(`${count}: You said "${turnContext.activity.text}"`);
        // Increment and set turn counter.
        await countProperty.set(turnContext, ++count);
    } else {
        await turnContext.sendActivity(`[${turnContext.activity.type} event detected]`);
    }
});

// Prevent Flash of Unstyled Content (FOUC): https://en.wikipedia.org/wiki/Flash_of_unstyled_content
document.addEventListener('DOMContentLoaded', function () {
    requestAnimationFrame(() => document.body.style.visibility = 'visible');
});

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { ActivityTypes, ConversationState, MemoryStorage } from 'botbuilder-core';
import 'botframework-webchat/botchat.css';
import { App } from 'botframework-webchat/built/App';
import './css/app.css';
import { WebChatAdapter } from './webChatAdapter';

// Create the custom WebChatAdapter.
const webChatAdapter = new WebChatAdapter();

// Connect our BotFramework-WebChat App instance with the DOM.
App({
    user: { id: 'Me!' },
    bot: { id: 'bot' },
    botConnection: webChatAdapter.botConnection
}, document.getElementById('bot'));

// Instantiate MemoryStorage for use with the ConversationState class.
const memory = new MemoryStorage();

// Add the instantiated storage into ConversationState.
const conversationState = new ConversationState(memory);

// Create a property to keep track of how many messages are received from the user.
const countProperty = conversationState.createProperty('turnCounter');

// Register the business logic of the bot through the WebChatAdapter's processActivity implementation.
webChatAdapter.processActivity(async (turnContext) => {
    // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
    if (turnContext.activity.type === ActivityTypes.Message) {
        // Read from state.
        let count = await countProperty.get(turnContext);
        count = count === undefined ? 1 : count;
        await turnContext.sendActivity(`${ count }: You said "${ turnContext.activity.text }"`);
        // Increment and set turn counter.
        await countProperty.set(turnContext, ++count);
    } else {
        await turnContext.sendActivity(`[${ turnContext.activity.type } event detected]`);
    }
    await conversationState.saveChanges(turnContext);
});

// Prevent Flash of Unstyled Content (FOUC): https://en.wikipedia.org/wiki/Flash_of_unstyled_content
document.addEventListener('DOMContentLoaded', () => {
    window.requestAnimationFrame(() => { document.body.style.visibility = 'visible'; });
});

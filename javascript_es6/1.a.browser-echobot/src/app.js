// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { BlobStorage, CosmosDbStorage } from 'botbuilder-azure';
import { BotStateSet, ConversationState, MemoryStorage, UserState, ActivityTypes } from 'botbuilder-core';
import 'botframework-webchat/botchat.css';
import { App } from 'botframework-webchat/built/App';
import './css/app.css';
import { WebChatAdapter } from './webChatAdapter';

// Instantiate MemoryStorage for use with the ConversationState middleware.
// Below are examples of different memory storage offerings that use Azure Blob and Cosmos DB storage.
const memory = new MemoryStorage();

// To use Azure Blob Storage to store memory, you can the BlobStorage class from `botbuilder-azure`.
// When using BlobStorage either a host string or a `Host` interface must be provided for the host parameter.
// const blobStorageHost = {
//     primaryHost: '',
//     secondaryHost: ''
// }
// const memory = new BlobStorage({
//     storageAccountOrConnectionString: '',
//     storageAccessKey: '',
//     host: '' || blobStorageHost,
//     containerName: ''
// });

// To use Azure Cosmos DB Storage to store memory, you can the CosmosDbStorage class from `botbuilder-azure`.
// const memory = new CosmosDbStorage({
//     serviceEndpoint: '',
//     authKey: '',
//     databaseId: '',
//     collectionId: ''
// });

// Create the custom WebChatAdapter and add the ConversationState middleware.
const webChatAdapter = new WebChatAdapter();

// Connect our BotFramework-WebChat App instance with the DOM.
App({
    user: { id: "Me!" },
    bot: { id: "bot" },
    botConnection: webChatAdapter.botConnection,
}, document.getElementById('bot'));

// Add the instatiated storage into state middleware.
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

// FOUC
document.addEventListener('DOMContentLoaded', function () {
    requestAnimationFrame(() => document.body.style.visibility = 'visible');
});

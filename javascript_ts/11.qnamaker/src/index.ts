// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { BotFrameworkAdapter, ConversationState, MemoryStorage } from 'botbuilder';
import { QnAMakerEndpoint } from 'botbuilder-ai';
import { BotConfiguration, IEndpointService, IQnAService, QnaMakerService } from 'botframework-config';
import { config } from 'dotenv';
import * as path from 'path';
import * as restify from 'restify';
import { QnAMakerBot } from './bot';

const CONFIG_ERROR = 1;

// See https://aka.ms/about-bot-file to learn more about .bot files.
const BOT_CONFIGURATION = 'qnamaker';
const QNA_CONFIGURATION = 'qnamakerService';

// Read botFilePath and botFileSecret from .env file.
// Note: Ensure you have a .env file and include botFilePath and botFileSecret.
const ENV_FILE = path.join(__dirname, '../.env');
const env = require('dotenv').config({path: ENV_FILE});

// Create HTTP server
let server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log(`\n${server.name} listening to ${server.url}`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator`);
    console.log(`\nTo talk to your bot, open qnamaker.bot file in the emulator.`);
});

// .bot file path
const BOT_FILE = path.join(__dirname, (process.env.botFilePath || ''));

let botConfig: BotConfiguration;
try {
    // Read the configuration from .bot file. 
    botConfig = BotConfiguration.loadSync(BOT_FILE, process.env.botFileSecret);
} catch (err) {
    console.log(`Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.`);
    process.exit(CONFIG_ERROR);
}

// Get bot endpoint configuration by service name.
const endpointConfig = <IEndpointService>botConfig.findServiceByNameOrId(BOT_CONFIGURATION);
const qnaConfig = <IQnAService>botConfig.findServiceByNameOrId(QNA_CONFIGURATION);

// Map the contents to the required format for QnAMaker.
const qnaEndpointSettings: QnAMakerEndpoint = {
    knowledgeBaseId: qnaConfig.kbId,
    endpointKey: qnaConfig.endpointKey,
    host: qnaConfig.hostname
}

// Create adapter. See https://aka.ms/about-bot-adapter to learn more about adapters.
const adapter = new BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.MicrosoftAppId,
    appPassword: endpointConfig.appPassword || process.env.MicrosoftAppPassword
});

// Create the bot that will handle incoming messages.
const qnaMakerBot = new QnAMakerBot(qnaEndpointSettings, {});

// Listen for incoming requests.
server.post('/api/messages', async (req, res) => {
    await adapter.processActivity(req, res, async (context) => {
        await qnaMakerBot.onTurn(context);        
    });
});

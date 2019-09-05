// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const dotenv = require('dotenv');
const fetch = require('node-fetch');
const path = require('path');
const restify = require('restify');

// Import required bot services.
// See https://aka.ms/bot-services to learn more about the different parts of a bot.
const { BotFrameworkAdapter } = require('botbuilder');

// This bot's main dialog.
const { EchoBot } = require('./bot');

// Import required bot configuration.
const ENV_FILE = path.join(__dirname, '.env');
dotenv.config({ path: ENV_FILE });

// Create HTTP server
const server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, () => {
    console.log(`\n${ server.name } listening to ${ server.url }`);
    console.log(`\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator`);
    console.log(`\nTo talk to your bot, open the emulator select "Open Bot"`);
    console.log(`\nor navigate to ${`http://localhost:${process.env.port || process.env.PORT || 3978}/WebChat`} to talk to your bot`);
});

// Create adapter.
// See https://aka.ms/about-bot-adapter to learn more about how bots work.
const adapter = new BotFrameworkAdapter({
    appId: process.env.MicrosoftAppId,
    appPassword: process.env.MicrosoftAppPassword
});

// Catch-all for errors.
adapter.onTurnError = async (context, error) => {
    // This check writes out errors to console log .vs. app insights.
    console.error(`\n [onTurnError]: ${ error }`);
    // Send a message to the user
    await context.sendActivity(`Oops. Something went wrong!`);
};

// Create the main dialog.
const bot = new EchoBot();

// Listen for incoming requests.
server.post('/api/messages', (req, res) => {
    adapter.processActivity(req, res, async (context) => {
        // Route to main dialog.
        await bot.run(context);
    });
});
 // This code performs a token request from direct line using your Web Chat secret to generate a DirectLine Token.   
    // Your client code must provide either a secret or a token to talk to your bot.
    // Tokens are more secure, and this is the approach being demonstrated in this sample app. 
    // To learn about the differences between secrets and tokens and to understand the risks associated with using secrets, visit 
    // https://docs.microsoft.com/en-us/azure/bot-service/rest-api/bot-framework-rest-direct-line-3-0-authentication?view=azure-bot-service-4.0
    server.post('/directline/token', async (req, res) => {

        // The userId must always have ‘dl_’ pre-pended to it, which is a requirement for all Direct Line IDs.
        // All user IDs should be unique, if not multiple users could potentially share the same conversation state.
        const id = `dl_${Date.now() + Math.random(36)}`;

        const options = {
        method: 'POST',
        headers: { 
            'Authorization': `Bearer ${process.env.DirectLineSecret}`,
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            user: { id }
        })
        };
    
        try {
            // This is the URL to the Direct Line token endpoint (where the token is requested from)
            const response = await fetch('https://directline.botframework.com/v3/directline/tokens/generate', options);
            const token = await response.json();
            token.userId = id;
            res.send(token);
        } catch (error) {
            res.send(403);
            console.log(error);
        }
    });
    
    server.get('/*', restify.plugins.serveStatic({
        directory: path.join(__dirname, 'public'),
        default: 'index.html'
    }));
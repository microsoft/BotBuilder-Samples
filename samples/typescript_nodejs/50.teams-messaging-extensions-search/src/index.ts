// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Import required packages
import { config } from 'dotenv';
import * as path from 'path';
import * as restify from 'restify';

// Import required bot services.
// See https://aka.ms/bot-services to learn more about the different parts of a bot.
import {
  BotFrameworkAdapter,
  TurnContext,
  WebRequest,
  WebResponse
} from 'botbuilder';

// This bot's main dialog.
import { TeamsMessagingExtensionSearchBot } from './teamsMessagingExtensionSearchBot';

// Read botFilePath and botFileSecret from .env file.
const ENV_FILE = path.join( __dirname, '..', '.env' );
config( { path: ENV_FILE } );

// Create adapter.
// See https://aka.ms/about-bot-adapter to learn more about adapters.
const adapter = new BotFrameworkAdapter( {
  appId: process.env.MicrosoftAppId,
  appPassword: process.env.MicrosoftAppPassword
} );

// Catch-all for errors.
const onTurnErrorHandler = async ( context, error ) => {
  // This check writes out errors to console log .vs. app insights.
  // NOTE: In production environment, you should consider logging this to Azure
  //       application insights.
  console.error( `\n [onTurnError] unhandled error: ${ error }` );

  // Send a trace activity, which will be displayed in Bot Framework Emulator
  await context.sendTraceActivity(
    'OnTurnError Trace',
    `${ error }`,
    'https://www.botframework.com/schemas/error',
    'TurnError'
  );

  // Send a message to the user
  await context.sendActivity( 'The bot encountered an error or bug.' );
  await context.sendActivity( 'To continue to run this bot, please fix the bot source code.' );
};

// Set the onTurnError for the singleton BotFrameworkAdapter.
adapter.onTurnError = onTurnErrorHandler;

// Create the bot that will handle incoming messages.
const bot = new TeamsMessagingExtensionSearchBot();

// Create HTTP server.
const server = restify.createServer();
server.listen( process.env.port || process.env.PORT || 3978, () => {
  console.log( `\n${ server.name } listening to ${ server.url }` );
  console.log( '\nGet Bot Framework Emulator: https://aka.ms/botframework-emulator' );
  console.log( '\nTo talk to your bot, open the emulator select "Open Bot"' );
} );

// Listen for incoming requests.
server.post( '/api/messages', ( req: WebRequest, res: WebResponse ) => {
  adapter.processActivity( req, res, async ( context: TurnContext ): Promise<void> => {
    await bot.run( context );
  } );
} );

// Listen for Upgrade requests for Streaming.
server.on( 'upgrade', ( req, socket, head ) => {
  // Create an adapter scoped to this WebSocket connection to allow storing session data.
  const streamingAdapter = new BotFrameworkAdapter( {
    appId: process.env.MicrosoftAppId,
    appPassword: process.env.MicrosoftAppPassword
  } );
  // Set onTurnError for the BotFrameworkAdapter created for each connection.
  streamingAdapter.onTurnError = onTurnErrorHandler;

  streamingAdapter.useWebSocket( req, socket, head, async ( context ) => {
    // After connecting via WebSocket, run this logic for every request sent over
    // the WebSocket connection.
    await bot.run( context );
  } );
} );
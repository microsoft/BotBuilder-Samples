// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { AutoSaveStateMiddleware, BotFrameworkAdapter, ConversationState, TranscriptLoggerMiddleware, TurnContext, UserState } from "botbuilder";
import { BlobStorageService, BotConfiguration, IAppInsightsService, IBlobStorageService, ICosmosDBService, IEndpointService, IGenericService } from "botframework-config";
import * as path from "path";
import * as restify from "restify";
import { EnterpriseBot } from "./enterpriseBot";

// Read variables from .env file.
import { config } from "dotenv";
import { BotServices } from "./botServices";
const envName: string = process.env.NODE_ENV || "development";
config({ path: path.join(__dirname, "..", `.env.${envName}`) });

const BOT_CONFIGURATION = (process.env.ENDPOINT || "development");
const BOT_CONFIGURATION_ERROR = 1;

const configurationPath = path.join(__dirname, "..", process.env.BOT_FILE_NAME || ".bot");
const botSecret = process.env.BOT_FILE_SECRET || "";

try {
    require.resolve(configurationPath);
} catch (err) {
    console.log(`Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.`);
    process.exit(BOT_CONFIGURATION_ERROR);
}

// Get bot configuration for services
const botConfig: BotConfiguration = BotConfiguration.loadSync(configurationPath, botSecret);

// Get bot endpoint configuration by service name
const endpointConfig = botConfig.findServiceByNameOrId(BOT_CONFIGURATION) as IEndpointService;

// Create the adapter
const adapter = new BotFrameworkAdapter({
    appId: endpointConfig.appId || process.env.microsoftAppID,
    appPassword: endpointConfig.appPassword || process.env.microsoftAppPassword,
});

// Setup our global error handler
// For production bots use AppInsights, or a production-grade telemetry service to
// log errors and other bot telemetry.
import { TelemetryClient } from "applicationinsights";
// Get AppInsights configuration by service name
const APPINSIGHTS_CONFIGURATION = process.env.APPINSIGHTS_NAME || "";
const appInsightsConfig: IAppInsightsService = botConfig.findServiceByNameOrId(APPINSIGHTS_CONFIGURATION) as IAppInsightsService;
if (!appInsightsConfig) {
    console.log("Please configure your AppInsights connection in your .bot file.");
    process.exit(BOT_CONFIGURATION_ERROR);
}
const telemetryClient = new TelemetryClient(appInsightsConfig.instrumentationKey);

adapter.onTurnError = async (turnContext, error) => {
    // CAUTION:  The sample simply logs the error to the console.
    console.error(error);
    // For production bots, use AppInsights or similar telemetry system.
    // tell the user something happen
    telemetryClient.trackException({ exception: error });
    // for multi-turn dialog interactions,
    // make sure we clear the conversation state
    await turnContext.sendActivity("Sorry, it looks like something went wrong.");
};

// CAUTION: The Memory Storage used here is for local bot debugging only. When the bot
// is restarted, anything stored in memory will be gone.
// const storage = new MemoryStorage();

// For production bots use the Azure CosmosDB storage, Azure Blob, or Azure Table storage provides.
const CosmosDbStorage = require("botbuilder-azure").CosmosDbStorage;
const STORAGE_CONFIGURATION = process.env.STORAGE_NAME || ""; // this is the name of the cosmos DB configuration in your .bot file
const cosmosConfig: ICosmosDBService = botConfig.findServiceByNameOrId(STORAGE_CONFIGURATION) as ICosmosDBService;
if (!cosmosConfig) {
    console.log("Please configure your CosmosDB connection in your .bot file.");
    process.exit(BOT_CONFIGURATION_ERROR);
}
const storage = new CosmosDbStorage({
    authKey: cosmosConfig.key,
    collectionId: cosmosConfig.collection,
    databaseId: cosmosConfig.database,
    serviceEndpoint: cosmosConfig.endpoint,
});

// create conversation and user state with in-memory storage provider.
const conversationState = new ConversationState(storage);
const userState = new UserState(storage);

// Use the AutoSaveStateMiddleware middleware to automatically read and write conversation and user state.
// CONSIDER:  if only using userState, then switch to adapter.use(userState);
adapter.use(new AutoSaveStateMiddleware(conversationState, userState));

// Transcript Middleware (saves conversation history in a standard format)
const AzureBlobTranscriptStore = require("botbuilder-azure").AzureBlobTranscriptStore;
const BLOB_CONFIGURATION = process.env.BLOB_NAME || ""; // this is the name of the BlobStorage configuration in your .bot file
const blobStorageConfig: IBlobStorageService = botConfig.findServiceByNameOrId(BLOB_CONFIGURATION) as IBlobStorageService;
if (!blobStorageConfig) {
    console.log("Please configure your Blob storage connection in your .bot file.");
    process.exit(BOT_CONFIGURATION_ERROR);
}
const blobStorage = new BlobStorageService(blobStorageConfig);
const transcriptStore = new AzureBlobTranscriptStore({
    containerName: blobStorage.container,
    storageAccountOrConnectionString: blobStorage.connectionString,
});
adapter.use(new TranscriptLoggerMiddleware(transcriptStore));

// Typing Middleware (automatically shows typing when the bot is responding/working) (not implemented https://github.com/Microsoft/botbuilder-js/issues/470)
// adapter.use(new ShowTypingMiddleware());

// Content Moderation Middleware (analyzes incoming messages for inappropriate content including PII, profanity, etc.)
import { ContentModeratorMiddleware } from "./middleware/contentModeratorMiddleware";
const CM_CONFIGURATION = process.env.CONTENT_MODERATOR_NAME || ""; // this is the name of the Content Moderator configuration in your .bot file
const cmConfig: IGenericService = botConfig.findServiceByNameOrId(CM_CONFIGURATION) as IGenericService;
if (cmConfig && cmConfig.configuration.key && cmConfig.configuration.region) {
    const contentModerator = new ContentModeratorMiddleware(cmConfig.configuration.key, cmConfig.configuration.region);
    adapter.use(contentModerator);
} else {
    console.warn("Content Moderator not configured in .bot file.");
}

let bot: EnterpriseBot;
try {
    const services: BotServices = new BotServices(botConfig);
    bot = new EnterpriseBot(services, conversationState, userState);
} catch (err) {
    console.log(`Error: ${err}`);
    process.exit(BOT_CONFIGURATION_ERROR);
}

// Create server
const server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function() {
    console.log(`${server.name} listening to ${server.url}`);
    console.log(`Get the Emulator: https://aka.ms/botframework-emulator`);
    console.log(`To talk to your bot, open your '.bot' file in the Emulator`);
});

// Listen for incoming requests
server.post("/api/messages", (req, res) => {
    // Route received a request to adapter for processing
    adapter.processActivity(req, res, async (turnContext: TurnContext) => {
        // route to bot activity handler.
        await bot.onTurn(turnContext);
    });
});

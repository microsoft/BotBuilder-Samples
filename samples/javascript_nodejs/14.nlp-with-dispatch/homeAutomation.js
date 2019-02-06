// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { LuisRecognizer } = require('botbuilder-ai');

// LUIS intent names. you can get this from the .lu file.
const HOME_AUTOMATION_INTENT = 'HomeAutomation';
const NONE_INTENT = 'None';

// LUIS entity names.
const DEVICE_PROPERTY_ENTITY = 'deviceProperty';
const NUMBER_ENTITY = 'number';
const ROOM_ENTITY = 'Room';
const OPERATION_ENTITY = 'Operation';
const DEVICE_ENTITY = 'Device';
const DEVICE_PATTERNANY_ENTITY = 'Device_PatternAny';
const ROOM_PATTERNANY_ENTITY = 'Room_PatternAny';

// State
const { HomeAutomationState } = require('./home-automation-state');

// this is the LUIS service type entry in the .bot file.
const LUIS_CONFIGURATION = 'Home Automation';

class HomeAutomation {
    /**
     *
     * @param {ConversationState} convoState conversation state
     * @param {UserState} userState user state
     * @param {BotConfiguration} botConfig bot configuration from .bot file
     */
    constructor(convoState, userState, botConfig) {
        if (!convoState) throw new Error('Need conversation state');
        if (!userState) throw new Error('Need user state');
        if (!botConfig) throw new Error('Need bot config');

        // home automation state
        this.state = new HomeAutomationState(convoState, userState);

        // add recognizers
        const luisServiceName = botConfig.name + '_' + LUIS_CONFIGURATION;
        const luisConfig = botConfig.findServiceByNameOrId(luisServiceName);
        if (!luisConfig || !luisConfig.appId) throw new Error(`Home automation LUIS model not found in .bot file. Please ensure you have all required LUIS models created and available in the .bot file. See readme.md for additional information\n`);
        this.luisRecognizer = new LuisRecognizer({
            applicationId: luisConfig.appId,
            azureRegion: luisConfig.region,
            // CAUTION: Authoring key is used in this example as it is appropriate for prototyping.
            // When implimenting for deployment/production, assign and use a subscription key instead of an authoring key.
            endpointKey: luisConfig.authoringKey
        });
    }
    /**
     *
     * @param {TurnContext} turn context object
     */
    async onTurn(turnContext) {
        // make call to LUIS recognizer to get home automation intent + entities
        const homeAutoResults = await this.luisRecognizer.recognize(turnContext);
        const topHomeAutoIntent = LuisRecognizer.topIntent(homeAutoResults);

        // depending on intent, call turn on or turn off or return unknown
        switch (topHomeAutoIntent) {
        case HOME_AUTOMATION_INTENT:
            await this.handleDeviceUpdate(homeAutoResults, turnContext);
            break;
        case NONE_INTENT:
        default:
            await turnContext.sendActivity(`HomeAutomation dialog cannot fulfill this request.`);
        }
    }
    /**
     *
     * @param {RecognizerResults} homeAutoResults results from LUIS recognizer
     * @param {TurnContext} context context object
     */
    async handleDeviceUpdate(homeAutoResults, context) {
        // Find entities from the LUIS model
        // The LUIS language understanding model is set up with both machine learned simple entities as well as pattern.any entities.
        // Check to see if either one has value. In cases where both have value, take the machine learned simple entities value.
        const devices = findEntities(DEVICE_ENTITY, homeAutoResults.entities);
        const devicesPatternAny = findEntities(DEVICE_PATTERNANY_ENTITY, homeAutoResults.entities);
        // Find any rooms specified in the user utterance. Look for both the machine learned simple entity as well as pattern.any entity
        const rooms = findEntities(ROOM_ENTITY, homeAutoResults.entities);
        const roomsPatternAny = findEntities(ROOM_PATTERNANY_ENTITY, homeAutoResults.entities);
        // Find any entity that indicates the requested operation. E.g. Turn on/ Turn off.
        const operations = findEntities(OPERATION_ENTITY, homeAutoResults.entities);
        // Find any additional device properties specified.
        const deviceProperties = findEntities(DEVICE_PROPERTY_ENTITY, homeAutoResults.entities);
        const numberProperties = findEntities(NUMBER_ENTITY, homeAutoResults.entities);

        if (operations === undefined) {
            await context.sendActivity(`You reached the "HomeAutomation" dialog. However you need to specific an operation, for example turn on bedroom light`);
        } else {
            // Update device state.
            await this.state.setDevice((devices || devicesPatternAny), (rooms || roomsPatternAny), operations[0], (deviceProperties || numberProperties), context);
            await context.sendActivity(`You reached the "HomeAutomation" dialog.`);
            await context.sendActivity(`Here's the current snapshot of your prior operations`);
            // read out operations list
            await context.sendActivity(await this.state.getDevices(context));
        }
    }
};

module.exports.HomeAutomation = HomeAutomation;

// Helper function to retrieve specific entities from LUIS results
function findEntities(entityName, entityResults) {
    let entities = [];
    if (entityName in entityResults) {
        entityResults[entityName].forEach(entity => {
            entities.push(entity);
        });
    }
    return entities.length > 0 ? entities : undefined;
}

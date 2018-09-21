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

// STATE
const HomeAutomationState = require('./home-automation-state');

// this is the LUIS service type entry in the .bot file.
const LUIS_CONFIGURATION = 'homeautomation.luis';

class homeAutomation {
    /**
     * 
     * @param {Object} convoState conversation state
     * @param {Object} userState user state
     * @param {Object} botConfig bot configuration from .bot file
     */
    constructor(convoState, userState, botConfig) {
        if(!convoState) throw ('Need converstaion state');
        if(!userState) throw ('Need user state');
        if(!botConfig) throw ('Need bot config');

        // home automation state
        this.state = new HomeAutomationState(convoState, userState);
        
        // add recogizers
        const luisConfig = botConfig.findServiceByNameOrId(LUIS_CONFIGURATION);
        if(!luisConfig || !luisConfig.appId) throw (`Home automation LUIS model not found in .bot file. Please ensure you have all required LUIS models created and available in the .bot file. See readme.md for additional information\n`);
        this.luisRecognizer = new LuisRecognizer({
            applicationId: luisConfig.appId,
            azureRegion: luisConfig.region,
            // CAUTION: Its better to assign and use a subscription key instead of authoring key here.
            endpointKey: luisConfig.authoringKey
        });
    }
    /**
     * 
     * @param {Object} context context object
     */
    async onTurn(context) {
        // make call to LUIS recognizer to get home automation intent + entities
        const homeAutoResults = await this.luisRecognizer.recognize(context);
        const topHomeAutoIntent = LuisRecognizer.topIntent(homeAutoResults);
        
        // depending on intent, call turn on or turn off or return unknown
        switch(topHomeAutoIntent) {
            case HOME_AUTOMATION_INTENT: 
                await this.handleDeviceUpdate(homeAutoResults, context);
                break;
            case NONE_INTENT:
            default:
                await context.sendActivity(`HomeAutomation dialog cannot fulfill this request.`);
        }
    }
    /**
     * 
     * @param {Object} homeAutoResults results from LUIS recognizer
     * @param {Object} context context object
     */
    async handleDeviceUpdate(homeAutoResults, context) {
        // Find entities from the LUIS model
        // The LUIS language understanding model is set up with both machine learned simple entities as well as pattern.any entities. 
        // Check to see if either one has value. In cases where both have value, take the machine learned simple entities value.
        const devices = findEntities(DEVICE_ENTITY, homeAutoResults.entities);
        const devices_patternAny = findEntities(DEVICE_PATTERNANY_ENTITY, homeAutoResults.entities);
        // Find any rooms specified in the user utterance. Look for both the machine learned simple entity as well as pattern.any entity
        const rooms = findEntities(ROOM_ENTITY, homeAutoResults.entities);
        const rooms_patternAny = findEntities(ROOM_PATTERNANY_ENTITY, homeAutoResults.entities);
        // Find any entity that indicates the requested operation. E.g. Turn on/ Turn off.
        const operations = findEntities(OPERATION_ENTITY, homeAutoResults.entities);
        // Find any additional device properties specified. 
        const deviceProperties = findEntities(DEVICE_PROPERTY_ENTITY, homeAutoResults.entities);
        const numberProperties = findEntities(NUMBER_ENTITY, homeAutoResults.entities);
        // Update device state.
        await this.state.setDevice((devices || devices_patternAny), (rooms || rooms_patternAny), operations[0], (deviceProperties || numberProperties), context);
        await context.sendActivity(`You reached the "HomeAutomation" dialog.`);
        await context.sendActivity(`Here's the current snapshot of your prior operations`);
        // read out operations list
        await context.sendActivity(await this.state.getDevices(context));
    }
};

module.exports = homeAutomation;

// Helper function to retrieve specific entities from LUIS results
function findEntities(entityName, entityResults) {
    let entities = []
    if (entityName in entityResults) {
        entityResults[entityName].forEach(entity => {
            entities.push(entity);
        });
    }
    return entities.length > 0 ? entities : undefined;
}
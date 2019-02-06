// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { LuisRecognizer } = require('botbuilder-ai');

// LUIS intent names. you can get this from the .lu file.
const GET_CONDITION_INTENT = 'Get_Weather_Condition';
const GET_FORECAST_INTENT = 'Get_Weather_Forecast';
const NONE_INTENT = 'None';

// LUIS entity names.
const LOCATION_ENTITY = 'Location';
const LOCATION_PATTERNANY_ENTITY = 'Location_PatternAny';

// this is the LUIS service type entry in the .bot file.
const WEATHER_LUIS_CONFIGURATION = 'Weather';

class Weather {
    /**
     *
     * @param {BotConfiguration} bot configuration from .bot file
     */
    constructor(botConfig) {
        if (!botConfig) throw new Error('Need bot config');

        // add recognizers
        const luisWeatherServiceName = botConfig.name + '_' + WEATHER_LUIS_CONFIGURATION;
        const luisConfig = botConfig.findServiceByNameOrId(luisWeatherServiceName);
        if (!luisConfig || !luisConfig.appId) throw new Error(`Weather LUIS model not found in .bot file. Please ensure you have all required LUIS models created and available in the .bot file. See readme.md for additional information\n`);
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
        // Call weather LUIS model.
        const weatherResults = await this.luisRecognizer.recognize(turnContext);
        const topWeatherIntent = LuisRecognizer.topIntent(weatherResults);
        // Get location entity if available.
        const locationEntity = (LOCATION_ENTITY in weatherResults.entities) ? weatherResults.entities[LOCATION_ENTITY][0] : undefined;
        const locationPatternAnyEntity = (LOCATION_PATTERNANY_ENTITY in weatherResults.entities) ? weatherResults.entities[LOCATION_PATTERNANY_ENTITY][0] : undefined;
        // Depending on intent, call "Turn On" or "Turn Off" or return unknown.
        switch (topWeatherIntent) {
        case GET_CONDITION_INTENT:
            await turnContext.sendActivity(`You asked for current weather condition in Location = ` + (locationEntity || locationPatternAnyEntity));
            break;
        case GET_FORECAST_INTENT:
            await turnContext.sendActivity(`You asked for weather forecast in Location = ` + (locationEntity || locationPatternAnyEntity));
            break;
        case NONE_INTENT:
        default:
            await turnContext.sendActivity(`Weather dialog cannot fulfill this request.`);
        }
    }
};

module.exports.Weather = Weather;

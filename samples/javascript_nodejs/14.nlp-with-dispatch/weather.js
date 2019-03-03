// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { LuisRecognizer } = require('botbuilder-ai');
const _ = require('lodash');
// LUIS intent names. you can get this from the .lu file.
const GET_CONDITION_INTENT = 'Get Weather Condition';
const GET_FORECAST_INTENT = 'Get Weather Forecast';
const NONE_INTENT = 'None';

// LUIS entity names.
const LOCATION_ENTITY = 'Location';
const LOCATION_PATTERNANY_ENTITY = 'Location_PatternAny';

// this is the LUIS service type entry in the .bot file.
const WEATHER_LUIS_CONFIGURATION = 'Weather';

class Weather {
    constructor() {
    }

    /**
     *
     * @param {TurnContext} turn context object
     * @param {LuisResult} dispatchResults Initial recognizer result from Dispatch
     */
    async onTurn(turnContext, dispatchResults) {
        const weatherResults = dispatchResults.luisResult.connectedServiceResult
        const topWeatherIntent = weatherResults.topScoringIntent;
        
        // Get location entity if available.
        const location = _.filter(weatherResults.entities, x => x.type === LOCATION_ENTITY);
        const locationEntity = (location == null) ? null : location[0].entity;
        const locationPatternAny = _.filter(weatherResults.entities, x => x.type === LOCATION_PATTERNANY_ENTITY);        
        const locationPatternAnyEntity = (locationPatternAny == null) ? null : locationPatternAny[0].entity;

        // Depending on intent, call "Get Forecast" or "Get Conditions" or return unknown.
        switch (topWeatherIntent.intent) {
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

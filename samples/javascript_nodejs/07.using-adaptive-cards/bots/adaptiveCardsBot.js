// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler, CardFactory } = require('botbuilder');

const fetch = require('node-fetch')

// location of python weather script flask server
const pythonWeatherScript = 'http://127.0.0.1:5000'

const WELCOME_TEXT = 'Type "Weather {city_name}" to get weather information for that city!';

class AdaptiveCardsBot extends ActivityHandler {
    constructor() {
        super();
        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            for (let cnt = 0; cnt < membersAdded.length; cnt++) {
                if (membersAdded[cnt].id !== context.activity.recipient.id) {
                    await context.sendActivity(`${ WELCOME_TEXT }`);
                }
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        this.onMessage(async (context, next) => {
            const userInput = context.activity.text.toLowerCase();

            if (userInput.startsWith('weather ')) {
                const city = userInput.substring('weather '.length).trim();
                
                // add city variable to python url
                let pythonURLWithCity = `${pythonWeatherScript}/new_weather_card?city=${city}`

                // send fetch request to python
                let response = await fetch(pythonURLWithCity);
                
                if (response.ok) {
                    const WeatherJson = await response.json();
                    
                    await context.sendActivity({
                        text:`The weather in ${city} is:`,
                        attachments: [CardFactory.adaptiveCard(WeatherJson)]
                    })
                }
                // if error response in requesting api data
                else{
                    await context.sendActivity('An error occured while retrieving weather data');
                }
            }
            // if user does not enter weather as a command
            else {
                await context.sendActivity('Sorry, that is not a valid command. Please say "weather" followed by the name of a city.');
            } 
            // ensure that the next BotHandler is run.
            await next();
        });
    }
}

module.exports.AdaptiveCardsBot = AdaptiveCardsBot;

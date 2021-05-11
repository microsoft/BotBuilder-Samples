// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const axios = require('axios');
const querystring = require('querystring');
const { TeamsActivityHandler, CardFactory,MessageFactory, ConsoleTranscriptLogger } = require('botbuilder');
const fs = require('fs');
const AdaptiveCard = require('../Resources/RestaurantCard.json');
const ConnectorCard = require('../Resources/ConnectorCard.json');
const configuration = require('dotenv').config();
const env = configuration.parsed;
const baseurl = env.BaseUrl;
const publicDir = require('path').join(__dirname,'../public/Images'); 

class TeamsMessagingExtensionsSearchBot extends TeamsActivityHandler {
    
    async handleTeamsMessagingExtensionQuery(context, query) {
        const searchQuery = query.parameters[0].value;     
        const attachments = [];
        
        switch(searchQuery){
            case 'adaptive card':           
               return this.GetAdaptiveCard();
               
            case 'connector card':              
                return this.GetConnectorCard();

            case 'result grid': 
                return this.GetResultGrid();

            default: 
                const response = await axios.get(`http://registry.npmjs.com/-/v1/search?${ querystring.stringify({ text: searchQuery, size: 8 }) }`);
                
                response.data.objects.forEach(obj => {
                        const heroCard = CardFactory.heroCard(obj.package.name);
                        const preview = CardFactory.heroCard(obj.package.name);
                        preview.content.tap = { type: 'invoke', value: { description: obj.package.description } };
                        const attachment = { ...heroCard, preview };
                        attachments.push(attachment);
                });
    
                return {
                    composeExtension:  {
                           type: 'result',
                           attachmentLayout: 'list',
                           attachments: attachments
                    }
                };
            }       
        }

     GetAdaptiveCard() {
        const preview = CardFactory.thumbnailCard(
                'Adaptive Card',
                'Please select to get the card'
        );

        const adaptive = CardFactory.adaptiveCard(AdaptiveCard);
        
        const attachment = { ...adaptive, preview };

        return {
            composeExtension: {
                   type: 'result',
                   attachmentLayout: 'list',
                   attachments: [attachment]
            }
        };
    }
    
    GetConnectorCard() {    
        const preview = CardFactory.thumbnailCard(
                'Connector Card',
                'Please select to get the card'
        );

        const connector = CardFactory.o365ConnectorCard(ConnectorCard);
        const attachment = {...connector, preview };

        return {
            composeExtension: {
                   type: 'result',
                   attachmentLayout: 'list',
                   attachments: [attachment]
            }
        };
    }
   
    GetResultGrid() {
        const attachments = [];    
        const files = fs.readdirSync(publicDir, (err, result) => {
            if(err) {    
               console.error('error', err);
            }
        });

        files.forEach((file) => {
            const grid = CardFactory.thumbnailCard(
                '',
                [{ url: `${baseurl}/Images/${file}` }]
            );

            attachments.push(grid);
       });
       
        return {
            composeExtension: {
                   type: 'result',
                   attachmentLayout: 'grid',
                   attachments: attachments
                }
            };     
        }

    async handleTeamsMessagingExtensionSelectItem(context, obj) {
        
        return {
            composeExtension: {
                  type: 'result',
                  attachmentLayout: 'list',
                  attachments: [CardFactory.thumbnailCard(obj.description)]
            }
        };

    } 
}

module.exports.TeamsMessagingExtensionsSearchBot = TeamsMessagingExtensionsSearchBot;

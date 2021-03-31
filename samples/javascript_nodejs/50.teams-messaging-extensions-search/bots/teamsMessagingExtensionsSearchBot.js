// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const axios = require('axios');
const querystring = require('querystring');
const { TeamsActivityHandler, CardFactory,MessageFactory, ConsoleTranscriptLogger } = require('botbuilder');
var fs = require('fs');
const AdaptiveCard = require('../Resources/RestaurantCard.json');
const ConnectorCard = require('../Resources/ConnectorCard.json');
var configuration = require('dotenv').config();
var env = configuration.parsed;
var baseurl = env.BaseUrl;

debugger;
var publicDir = require('path').join(__dirname,'../public/Images'); 

class TeamsMessagingExtensionsSearchBot extends TeamsActivityHandler {
    
    async handleTeamsMessagingExtensionQuery(context, query) {
        const searchQuery = query.parameters[0].value;
      
        var attachments = [];
        switch(searchQuery){
            case 'adaptive card':           
               var adadptivecardresponse =   this.GetAdaptiveCard();
               return adadptivecardresponse;
               
            case 'connector card':              
                var connectorcardresponse = this.GetConnectorCard();
                return connectorcardresponse;

            case `result grid`:              
                var resultgridresponse = this.GetResultGrid();
                return resultgridresponse;
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
                composeExtension: {
                    type: 'result',
                    attachmentLayout: 'list',
                    attachments: attachments
                }
            };
        }       
    }

     GetAdaptiveCard() {
       
        var filepath = "./Resources/RestaurantCard.json";
        const attachments = [];
        const preview = CardFactory.thumbnailCard(
            'Adaptive Card',
            'Please select to get the card'
        );

        const adaptive = CardFactory.adaptiveCard(AdaptiveCard);
        
        const attachment = {...adaptive,preview}
        attachments.push(attachment);
        return {
            composeExtension: {
                type: 'result',
                attachmentLayout: 'list',
                attachments: attachments
            }
        };
    }
    
    GetConnectorCard() {    
        const attachments = [];
        const preview = CardFactory.thumbnailCard(
        'Connector Card',
        'Please select to get the card');

    const connector = CardFactory.o365ConnectorCard(ConnectorCard);
    const attachment = {...connector,preview}
    attachments.push(attachment);
    return {
        composeExtension: {
            type: 'result',
            attachmentLayout: 'list',
            attachments: attachments
        }
     };
    }

    GetResultGrid() {
       const attachments = [];   
       var files = fs.readdirSync(publicDir,function(err, result) {
           if(err) 
           console.log('error', err);
             });
       var grid="";
       files.forEach(file=>{
       grid = CardFactory.thumbnailCard(
            '',
            [{ url: baseurl+"/Images/"+ file }]
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

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes } = require('botbuilder-core');
const { DialogSet, DialogTurnStatus, NumberPrompt, TextPrompt, WaterfallDialog } = require('botbuilder-dialogs');

const SlotFillingDialog = require('./SlotFillingDialog');
const SlotDetails = require('./SlotDetails');

const DIALOG_STATE_PROPERTY = 'dialogState';

class MainDialog {
    /**
     * 
     * @param {ConversationState} conversationState A ConversationState object used to store dialog state.
     */
    constructor (conversationState) {

        this.conversationState = conversationState;

        // Create a property used to store dialog state.
        // See https://aka.ms/about-bot-state-accessors to learn more about bot state and state accessors.
        this.dialogState = this.conversationState.createProperty(DIALOG_STATE_PROPERTY);

        // Create a dialog set to include the dialogs used by this bot.
        this.dialogs = new DialogSet(this.dialogState);

        const fullname_slots = [
            new SlotDetails('first', 'text', 'Please enter your first name.'),
            new SlotDetails('last', 'text', 'Please enter your last name.')
        ];

        const address_slots = [
            new SlotDetails('street', 'text', 'Please enter your street address.'),
            new SlotDetails('city', 'text', 'Please enter the city.'),
            new SlotDetails('zip', 'text', 'Please enter your zipcode.')
        ];

        const slots = [
            new SlotDetails('fullname', 'fullname'),
            new SlotDetails('age', 'number', 'Please enter your age.'),
            new SlotDetails('shoesize', 'shoesize', 'Please enter your show size.', 'You must enter a size between 0 and 16. Half sizes are acceptable.'),
            new SlotDetails('address', 'address')
        ];


        this.dialogs.add(new SlotFillingDialog('address', address_slots));
        this.dialogs.add(new SlotFillingDialog('fullname', fullname_slots));
        this.dialogs.add(new TextPrompt('text'));
        this.dialogs.add(new NumberPrompt('number'));
        this.dialogs.add(new NumberPrompt('shoesize', this.showSizeValidator));
        this.dialogs.add(new SlotFillingDialog('slot-dialog', slots));

        this.dialogs.add(new WaterfallDialog('root', [
            this.startDialog,
            this.processResults
        ]));
    }

    // Kick off the dialog with the main slot dialog.
    async startDialog(dc, step) {
        return await dc.begin('slot-dialog');
    }   

    async processResults(dc, step) {
        const values = step.result.values;

        const fullname = values['fullname'].values;
        await dc.context.sendActivity(`Your name is ${ fullname['first'] } ${ fullname['last'] }.`);

        await dc.context.sendActivity(`You wear a size ${ values['shoesize'] } shoe.`);

        const address = values['address'].values;
        await dc.context.sendActivity(`Your address is: ${ address['street'] }, ${ address['city'] } ${ address['zip'] }`);

        return await dc.end();
    }

    async showSizeValidator(turnContext, step) {

        const shoesize = step.recognized.value;
        
        // show sizes can range from 0 to 16
        if (shoesize >= 0 && shoesize <= 16)
        {
            // we only accept round numbers or half sizes
            if (Math.floor(shoesize) == shoesize || Math.floor(shoesize * 2) == shoesize * 2)
            {
                // indicate success by returning the value
                step.end(shoesize);
            }
        }
    }

    /**
     * 
     * @param {TurnContext} turnContext A TurnContext object representing an incoming message to be handled by the bot.
     */
    async onTurn(turnContext) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (turnContext.activity.type === ActivityTypes.Message) {
            // Create dialog context.
            const dc = await this.dialogs.createContext(turnContext);

            const utterance = (turnContext.activity.text || '').trim().toLowerCase();
            if (utterance === 'cancel') { 
                if (dc.activeDialog) {
                    await dc.cancelAll();
                    await dc.context.sendActivity(`Ok... canceled.`);
                } else {
                    await dc.context.sendActivity(`Nothing to cancel.`);
                }
            }
            
            // Continue the current dialog if one is pending.
            const results = await dc.continue();

            // If no response has been sent, start the onboarding dialog.
            if (results.status === DialogTurnStatus.empty) {
                await dc.begin('root');
            } 
        } else if (
             turnContext.activity.type === ActivityTypes.ConversationUpdate &&
             turnContext.activity.membersAdded[0].name !== 'Bot'
        ) {
            // Send a "this is what the bot does" message.
            const description = [
                'This is a bot that demonstrates an alternate dialog system',
                'which uses a slot filling technique to collect multiple responses from a user.',
                'Say anything to continue.'
            ];
            await turnContext.sendActivity(description.join(' '));
        }
    }
}

module.exports = MainDialog;
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

<<<<<<< HEAD
const { ActivityTypes } = require('botbuilder-core');
=======
const { ActivityTypes } = require('botbuilder');
>>>>>>> 73ef1b18b2ae8785930ca5b993a955d1a99beadf
const { DialogSet, DialogTurnStatus, NumberPrompt, TextPrompt, WaterfallDialog } = require('botbuilder-dialogs');

const SlotFillingDialog = require('./SlotFillingDialog');
const SlotDetails = require('./SlotDetails');

const DIALOG_STATE_PROPERTY = 'dialogState';

class MainDialog {
    /**
<<<<<<< HEAD
     * MainDialog defines the core business logic of this bot.
=======
     * 
>>>>>>> 73ef1b18b2ae8785930ca5b993a955d1a99beadf
     * @param {ConversationState} conversationState A ConversationState object used to store dialog state.
     */
    constructor (conversationState) {

        this.conversationState = conversationState;

        // Create a property used to store dialog state.
        // See https://aka.ms/about-bot-state-accessors to learn more about bot state and state accessors.
        this.dialogState = this.conversationState.createProperty(DIALOG_STATE_PROPERTY);

        // Create a dialog set to include the dialogs used by this bot.
        this.dialogs = new DialogSet(this.dialogState);

<<<<<<< HEAD
        // Set up a series of questions for collecting the user's name.
=======
>>>>>>> 73ef1b18b2ae8785930ca5b993a955d1a99beadf
        const fullname_slots = [
            new SlotDetails('first', 'text', 'Please enter your first name.'),
            new SlotDetails('last', 'text', 'Please enter your last name.')
        ];

<<<<<<< HEAD
        // Set up a series of questions to collect a street address.
=======
>>>>>>> 73ef1b18b2ae8785930ca5b993a955d1a99beadf
        const address_slots = [
            new SlotDetails('street', 'text', 'Please enter your street address.'),
            new SlotDetails('city', 'text', 'Please enter the city.'),
            new SlotDetails('zip', 'text', 'Please enter your zipcode.')
        ];

<<<<<<< HEAD
        // Link the questions together into a parent group that contains references
        // to both the fullname and address questions defined above.
=======
>>>>>>> 73ef1b18b2ae8785930ca5b993a955d1a99beadf
        const slots = [
            new SlotDetails('fullname', 'fullname'),
            new SlotDetails('age', 'number', 'Please enter your age.'),
            new SlotDetails('shoesize', 'shoesize', 'Please enter your show size.', 'You must enter a size between 0 and 16. Half sizes are acceptable.'),
            new SlotDetails('address', 'address')
        ];

<<<<<<< HEAD
        // Add the individual child dialogs and prompts used.
        // Note that the built-in prompts work hand-in-hand with our custom SlotFillingDialog class
        // because they are both based on the provided Dialog class.
=======

>>>>>>> 73ef1b18b2ae8785930ca5b993a955d1a99beadf
        this.dialogs.add(new SlotFillingDialog('address', address_slots));
        this.dialogs.add(new SlotFillingDialog('fullname', fullname_slots));
        this.dialogs.add(new TextPrompt('text'));
        this.dialogs.add(new NumberPrompt('number'));
        this.dialogs.add(new NumberPrompt('shoesize', this.showSizeValidator));
        this.dialogs.add(new SlotFillingDialog('slot-dialog', slots));

<<<<<<< HEAD
        // Finally, add a 2-step WaterfallDialog that will initiate the SlotFillingDialog,
        // and then collect and display the results.
=======
>>>>>>> 73ef1b18b2ae8785930ca5b993a955d1a99beadf
        this.dialogs.add(new WaterfallDialog('root', [
            this.startDialog,
            this.processResults
        ]));
    }

<<<<<<< HEAD
    // This is the first step of the WaterfallDialog.
    // It kicks off the dialog with the multi-question SlotFillingDialog,
    // then passes the aggregated results on to the next step.
=======
    // Kick off the dialog with the main slot dialog.
>>>>>>> 73ef1b18b2ae8785930ca5b993a955d1a99beadf
    async startDialog(dc, step) {
        return await dc.begin('slot-dialog');
    }   

<<<<<<< HEAD
    // This is the second step of the WaterfallDialog.
    // It receives the results of the SlotFillingDialog and displays them.
    async processResults(dc, step) {

        // Each "slot" in the SlotFillingDialog is represented by a field in step.result.values.
        // The complex that contain subfields have their own .values field containing the sub-values.
=======
    async processResults(dc, step) {
>>>>>>> 73ef1b18b2ae8785930ca5b993a955d1a99beadf
        const values = step.result.values;

        const fullname = values['fullname'].values;
        await dc.context.sendActivity(`Your name is ${ fullname['first'] } ${ fullname['last'] }.`);

        await dc.context.sendActivity(`You wear a size ${ values['shoesize'] } shoe.`);

        const address = values['address'].values;
        await dc.context.sendActivity(`Your address is: ${ address['street'] }, ${ address['city'] } ${ address['zip'] }`);

        return await dc.end();
    }

<<<<<<< HEAD
    // Validate that the provided shoe size is between 0 and 16, and allow half steps.
    // This is used to instantiate a specialized NumberPrompt.
=======
>>>>>>> 73ef1b18b2ae8785930ca5b993a955d1a99beadf
    async showSizeValidator(turnContext, step) {

        const shoesize = step.recognized.value;
        
<<<<<<< HEAD
        // Show sizes can range from 0 to 16.
        if (shoesize >= 0 && shoesize <= 16)
        {
            // We only accept round numbers or half sizes.
            if (Math.floor(shoesize) == shoesize || Math.floor(shoesize * 2) == shoesize * 2)
            {
                // Indicate success by returning the value.
=======
        // show sizes can range from 0 to 16
        if (shoesize >= 0 && shoesize <= 16)
        {
            // we only accept round numbers or half sizes
            if (Math.floor(shoesize) == shoesize || Math.floor(shoesize * 2) == shoesize * 2)
            {
                // indicate success by returning the value
>>>>>>> 73ef1b18b2ae8785930ca5b993a955d1a99beadf
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
            
<<<<<<< HEAD
            if (!dc.context.responded) {
                // Continue the current dialog if one is pending.
                const results = await dc.continue();
            }

            if (!dc.context.responded) {
                // If no response has been sent, start the onboarding dialog.
                await dc.begin('root');
            }
=======
            // Continue the current dialog if one is pending.
            const results = await dc.continue();

            // If no response has been sent, start the onboarding dialog.
            if (results.status === DialogTurnStatus.empty) {
                await dc.begin('root');
            } 
>>>>>>> 73ef1b18b2ae8785930ca5b993a955d1a99beadf
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
        
        await this.conversationState.write(turnContext);
    }    
}

module.exports = MainDialog;
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ComponentDialog, DialogTurnStatus, DialogSet } = require('botbuilder-dialogs');
const { MessageFactory } = require('botbuilder');
const { TurnResult } = require('../shared/helpers');
const { GenSuggestedQueries } = require('../shared/helpers/genSuggestedQueries');
const { userProfileProperty } = require('../shared/stateProperties');

const MAIN_DIALOG = 'MainDialog';
const WhoAreYouDialog = require('../whoAreYou');
const QnADialog = require('../qna');
const ChitChatDialog = require('../chitChat');
const HelpDialog = require('../help');
const CancelDialog = require('../cancel');
const FindCafeLocationsDialog = require('../findCafeLocations');
const WhatCanYouDoDialog = require('../whatCanYouDo');

// User name entity from ../whoAreYou/resources/whoAreYou.lu
const USER_NAME_ENTITY = 'userName_patternAny';

// Query property from ../whatCanYouDo/resources/whatCanYHouDoCard.json
// When user responds to what can you do card, a query property is set in response.
const QUERY_PROPERTY = 'query';
const USER_PROFILE_PROPERTY = 'userProfileProperty';
const USER_RESERVATIONS_PROPERTY = 'userReservationsProperty';
const MAIN_DIALOG_STATE_PROPERTY = 'mainDialogState';
const TURN_COUNTER_PROPERTY = 'turnCounterProperty';

class MainDialog extends ComponentDialog {
    /**
     * Constructor.
     * @param {Object} botConfig bot configuration
     * @param {Object} onTurnPropertyAccessor 
     * @param {Object} conversationState 
     * @param {Object} userState 
     */
    constructor(botConfig, onTurnPropertyAccessor, conversationState, userState) {
        super(MAIN_DIALOG)
        if(!botConfig) throw ('Missing parameter. botConfig is required');
        if(!onTurnPropertyAccessor) throw ('Missing parameter. onTurnPropertyAccessor is required');
        if(!conversationState) throw ('Missing parameter. conversationState is required');
        if(!userState) throw ('Missing parameter. userState is required');

        // Create state objects for user, conversation and dialog states.   
        this.userProfilePropertyAccessor = userState.createProperty(USER_PROFILE_PROPERTY);
        this.reservationsPropertyAccessor = userState.createProperty(USER_RESERVATIONS_PROPERTY);
        this.mainDialogPropertyAccessor = conversationState.createProperty(MAIN_DIALOG_STATE_PROPERTY);
        this.turnCounterPropertyAccessor = conversationState.createProperty(TURN_COUNTER_PROPERTY);

        // keep on turn accessor and bot configuration
        this.onTurnPropertyAccessor = onTurnPropertyAccessor;
        this.botConfig = botConfig;

        // add dialogs
        this.dialogs = new DialogSet(this.mainDialogPropertyAccessor);
        this.dialogs.add(new CancelDialog());
        this.qnaDialog = new QnADialog(botConfig, this.userProfilePropertyAccessor);
        this.findCafeLocationsDialog = new FindCafeLocationsDialog();
        this.whatCanYouDoDialog = new WhatCanYouDoDialog();
        this.dialogs.add(new WhoAreYouDialog(botConfig, 
                                             this.userProfilePropertyAccessor, 
                                             this.turnCounterPropertyAccessor));
    }
    /**
     * Override onDialogBegin 
     * 
     * @param {Object} dc dialog context
     * @param {Object} options dialog turn options
     */
    async onDialogBegin(dc, options) {
        // Override default begin() logic with bot orchestration logic
        return await this.onDialogContinue(dc);
    }

    /**
     * Override onDialogContinue
     * 
     * @param {Object} dc dialog context
     */
    async onDialogContinue(dc) {
        let dialogTurnResult = new TurnResult(DialogTurnStatus.empty);
        // get on turn property through the property accessor
        const onTurnProperty = await this.onTurnPropertyAccessor.get(dc.context);
        // Main Dialog examines the incoming turn property to determine  
        //     1. If the requested operation is permissible - e.g. if user is in middle of a dialog, 
        //        then an out of order reply should not be allowed.
        //     2. Calls any oustanding dialogs to continue
        //     3. If results is no-match from outstanding dialog .OR. if there are no outstanding dialogs,
        //         Decide which child dialog should begin and start it
        //         
        const reqOpStatus = await this.isRequestedOperationPossible(dc, onTurnProperty.intent);

        if(!reqOpStatus.allowed) {
            await dc.context.sendActivity(reqOpStatus.reason);
            return dialogTurnResult; 
        }

        // continue outstanding dialogs
        dialogTurnResult = await dc.continue();

        // This will only be empty if there is no active dialog in the stack.
        if(dialogTurnResult.status === DialogTurnStatus.empty) {
            dialogTurnResult = await this.beginChildDialog(dc, onTurnProperty);
        }

        // Examine result from continue or beginChildDialog.
        switch(dialogTurnResult.status) {
            case DialogTurnStatus.complete: {
                if(dialogTurnResult.result) {
                    switch(dialogTurnResult.result.reason) {
                        case 'Interruption': {
                            // Interruption. Begin child dialog
                            dialogTurnResult = await this.beginChildDialog(dc, onTurnProperty, dialogTurnResult.result.payload);
                            break;
                        } 
                        case 'Abandon': {
                            // Re-hydrate old dialog
                            dialogTurnResult = await this.beginChildDialog(dc, dialogTurnResult.result.payload.onTurnProperty);
                            break;
                        }
                    }
                } else {
                    // The active dialog's stack ended with a complete status
                    await dc.context.sendActivity(MessageFactory.suggestedActions(GenSuggestedQueries(), `Is there anything else I can help you with?`));
                    // End active dialog
                    await dc.end();
                    break;
                }
            }
            case DialogTurnStatus.waiting: {
                // The active dialog is waiting for a response from the user, so do nothing
                break;
            }
            case DialogTurnStatus.cancelled: {
                // The active dialog's stack has been cancelled
                await dc.context.sendActivity(MessageFactory.suggestedActions(GenSuggestedQueries(), `Is there anything else I can help you with?`));
                // End active dialog
                await dc.cancelAll();
                break;
            }
        }
        dialogTurnResult = (dialogTurnResult === undefined) ? new TurnResult(DialogTurnStatus.empty) : dialogTurnResult;
        return dialogTurnResult;
    }

    /**
     * Method to begin appropriate child dialog based on user input
     * 
     * @param {Object} dc 
     * @param {Object} onTurnProperty 
     * @param {Object} childDialogPayload 
     */
    async beginChildDialog(dc, onTurnProperty, childDialogPayload) {
        switch(onTurnProperty.intent) {
            // Help, ChitChat and QnA share the same QnA Maker model. So just call the QnA Dialog.
            case QnADialog.Name: 
            case ChitChatDialog.Name: 
            case HelpDialog.Name: {
                return await this.qnaDialog.onTurn(dc.context);
            }
            case CancelDialog.Name: {
                await this.resetTurnCounter(dc.context);
                return await dc.begin(CancelDialog.Name, childDialogPayload);
            } case WhoAreYouDialog.Name: {
                // Get user profile.
                let userProfile = await this.userProfilePropertyAccessor.get(dc.context);
                // Handle case where user is re-introducing themselves. 
                // These utterances are defined in ../whoAreYou/resources/whoAreYou.lu 
                let userNameInOnTurnProperty = (onTurnProperty.entities || []).filter(item => item.entityName == USER_NAME_ENTITY);
                if(userNameInOnTurnProperty.length !== 0) {
                    let userName = userNameInOnTurnProperty[0].entityValue[0];
                    // capitalize user name   
                    userName = userName.charAt(0).toUpperCase() + userName.slice(1);
                    this.userProfilePropertyAccessor.set(dc.context, new userProfileProperty(userName));
                    return await dc.context.sendActivity(`Hello ${userName}, Nice to meet you again! I'm the Contoso Cafe Bot.`);
                }
                // Begin the who are you dialog if we have an invalid or empty user name or if the user name was previously set to 'Human'
                if(userProfile === undefined || userProfile.userName === '' || userProfile.userName === 'Human') {
                    await dc.context.sendActivity(`Hello, I'm the Contoso Cafe Bot.`);
                    await this.resetTurnCounter(dc.context);
                    // Begin user Profile dialog to ask user their name
                    return await dc.begin(WhoAreYouDialog.Name);
                } else {
                    // Already have the user name. So just greet them.
                    return await dc.context.sendActivity(`Hello ${userProfile.userName}, Nice to meet you again! I'm the Contoso Cafe Bot.`);
                }
            } case FindCafeLocationsDialog.Name: {
                return await this.findCafeLocationsDialog.onTurn(dc.context);
            } case WhatCanYouDoDialog.Name: {
                // Handle case when user interacted with the what can you do card.
                // What can you do card sends a custom data property with intent name, text value and possible entities.
                // See ../whatCanYouDo/resources/whatCanYouDoCard.json for card definition.
                let queryProperty = (onTurnProperty.entities || []).filter(item => item.entityName == QUERY_PROPERTY);
                if(queryProperty.length !== 0) {
                    let parsedJSON;
                    try {
                        parsedJSON = JSON.parse(queryProperty[0].entityValue);
                    } catch (err) {
                        return await dc.context.sendActivity(`Try and choose a query from the card before you click the 'Let's talk!' button.`);
                    }
                    if(parsedJSON.text !== undefined) {
                        dc.context.activity.text = parsedJSON.text;
                        await dc.context.sendActivity(`You said: '${dc.context.activity.text}'`);
                    }
                    return await this.beginChildDialog(dc, parsedJSON);
                }
                return await this.whatCanYouDoDialog.onTurn(dc.context);
            }
        }
    }
    /** 
     * Method to reset turn counter property
     * @param {Object} context
     * 
     */
    async resetTurnCounter(context) {
        this.turnCounterPropertyAccessor.set(context, 0);
    }
    /**
     * Method to evaluate if the requested user operation is possible.
     * User could be in the middle of a multi-turn dialog where intteruption might not be possible/ allowed
     * 
     * @param {Object} dc 
     * @param {String} requestedOperation 
     * @returns {Object} outcome object
     */
    async isRequestedOperationPossible(dc, requestedOperation) {
        let outcome = {allowed: true, reason: ''};
        let activeDialog = '';
        if(dc.activeDialog !== undefined) activeDialog = dc.activeDialog.id;

        // E.g. Book table submit and Book Table cancel requests through book table card are not allowed when Book Table is not the active dialog
        if(requestedOperation === 'Book_Table_Submit' || requestedOperation === 'Book_Table_Cancel') {
            if(activeDialog !== BookTableDialog.Name) {
                outcome.allowed = false;
                outcome.reason = `Sorry! I'm unable to process that. To start a new table reservation, try 'Book a table'`;
            }
        } else if(requestedOperation === CancelDialog.Name) {
            // Cancel dialog (with confirmation) is only possible for multi-turn dialogs - Book Table, Who are you
            if(activeDialog !== BookTableDialog.Name && activeDialog !== WhoAreYouDialog.Name) {
                outcome.allowed = false;
                outcome.reason = `Nothing to cancel.`;
            }
        }
        return outcome;
    }
};

MainDialog.Name = MAIN_DIALOG;

module.exports = MainDialog;
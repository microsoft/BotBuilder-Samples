// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ComponentDialog, DialogTurnStatus, DialogSet } = require('botbuilder-dialogs');
const { MessageFactory } = require('botbuilder');
const MAIN_DIALOG = 'MainDialog';
const DialogTurnResult = require('../shared/turnResult');
const BookTableDialog = require('../bookTable');
const WhoAreYouDialog = require('../whoAreYou');
const QnADialog = require('../qna');
const ChitChatDialog = require('../chitChat');
const HelpDialog = require('../help');
const CancelDialog = require('../cancel');
const FindCafeLocationsDialog = require('../findCafeLocations');
const WhatCanYouDoDialog = require('../whatCanYouDo');

const getQuerySuggestions = require('../shared/genSuggestedQueries');

// User name entity from ../whoAreYou/resources/whoAreYou.lu
const USER_NAME = 'userName_patternAny';

// Query property from ../whatCanYouDo/resources/whatCanYHouDoCard.json
// When user responds to what can you do card, a query property is set in response.
const QUERY_PROPERTY = 'query';
const { userProfileProperty } = require('../shared/stateProperties');


const USER_PROFILE_PROPERTY = 'userProfile';
const USER_RESERVATIONS_PROPERTY = 'userReservations';
const USER_QUERY_PROPERTY = 'userQuery';
const BOOK_TABLE_DIALOG_PROPERTY = 'bookTableDialog';
const MAIN_DIALOG_STATE_PROPERTY = 'mainDialogState';
const TURN_COUNTER_PROPERTY = 'turnCounter';

class MainDialog extends ComponentDialog {

    constructor(botConfig, onTurnPropertyAccessor, conversationState, userState) {
        super(MAIN_DIALOG)
        if(!botConfig) throw ('Missing parameter. botConfig is required');
        if(!onTurnPropertyAccessor) throw ('Missing parameter. onTurnPropertyAccessor is required');
        if(!conversationState) throw ('Missing parameter. conversationState is required');
        if(!userState) throw ('Missing parameter. userState is required');

        // Create state objects for user, conversation and dialog states.   
        this.userProfilePropertyAccessor = userState.createProperty(USER_PROFILE_PROPERTY);
        this.reservationsPropertyAccessor = userState.createProperty(USER_RESERVATIONS_PROPERTY);
        this.userQueryPropertyAccessor = userState.createProperty(USER_QUERY_PROPERTY);
        this.mainDialogPropertyAccessor = conversationState.createProperty(MAIN_DIALOG_STATE_PROPERTY);
        this.turnCounterPropertyAccessor = conversationState.createProperty(TURN_COUNTER_PROPERTY);
        this.bookTableDialogPropertyAccessor = conversationState.createProperty(BOOK_TABLE_DIALOG_PROPERTY);

        // keep on turn accessor and bot configuration
        this.onTurnPropertyAccessor = onTurnPropertyAccessor;
        this.botConfig = botConfig;
        // add dialogs
        this.dialogs = new DialogSet(this.mainDialogPropertyAccessor);
        // add book table dialog
        this.dialogs.add(new BookTableDialog(botConfig, 
                                             this.reservationsPropertyAccessor, 
                                             this.turnCounterPropertyAccessor, 
                                             onTurnPropertyAccessor, 
                                             this.bookTableDialogPropertyAccessor, 
                                             conversationState));
        
        // add cancel dialog
        this.dialogs.add(new CancelDialog());
        // add QnA dialog. This serves help, qna and chit chat.
        this.qnaDialog = new QnADialog(botConfig, this.userProfilePropertyAccessor);
        // add find cafe locations dialog.
        this.findCafeLocationsDialog = new FindCafeLocationsDialog();
        // add what can you dialog.
        this.whatCanYouDoDialog = new WhatCanYouDoDialog();
        // add who are you dialog
        this.dialogs.add(new WhoAreYouDialog(botConfig, 
                                             this.userProfilePropertyAccessor, 
                                             this.turnCounterPropertyAccessor));
    }

    async onDialogBegin(dc, options) {
        // Override default begin() logic with bot orchestration logic
        return await this.onDialogContinue(dc);
    }

    async onDialogContinue(dc) {
        let dialogTurnResult = new DialogTurnResult(DialogTurnStatus.empty);
        // get on turn property through the property accessor
        const onTurnProperty = await this.onTurnPropertyAccessor.get(dc.context);

        // Main Dialog examines the incoming turn property to determine  
        //     1. If the requested operation is permissible - e.g. if user is in middle of a dialog, then an out of order reply should not be allowed.
        //     2. Calls any oustanding dialogs to continue
        //     3. If results is no-match from outstanding dialog .OR. if there are no outstanding dialogs,
        //         Decide which child dialog should begin and start it
        //         
        const reqOpStatus = await this.isRequestedOperationPossible(dc, onTurnProperty.intent);

        if(!reqOpStatus.allowed) {
            await dc.context.sendActivity(reqOpStatus.reason);
            return dialogTurnResult; 
        }

        dialogTurnResult = await dc.continue();

        // This will only be empty if there is no active dialog in the stack.
        if(dialogTurnResult.status === DialogTurnStatus.empty) {
            dialogTurnResult = await this.beginChildDialog(dc, onTurnProperty);
        }

        switch(dialogTurnResult.status) {
            // case DialogTurnStatus.empty: {
            //     // begin right child dialog
            //     dialogTurnResult = await this.beginChildDialog(dc, onTurnProperty);
            //     break;
            // }
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
                    await dc.context.sendActivity(MessageFactory.suggestedActions(getQuerySuggestions(), `Is there anything else I can help you with?`));
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
                await dc.context.sendActivity(MessageFactory.suggestedActions(getQuerySuggestions(), `Is there anything else I can help you with?`));
                // End active dialog
                await dc.cancelAll();
                break;
            }
        }
        dialogTurnResult = (dialogTurnResult === undefined) ? new DialogTurnResult(DialogTurnStatus.empty) : dialogTurnResult;
        return dialogTurnResult;
    }

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
            } case BookTableDialog.Name: {
                await this.resetTurnCounter(dc.context);
                return await dc.begin(BookTableDialog.Name, childDialogPayload);
            } case WhoAreYouDialog.Name: {
                // Get user profile.
                let userProfile = await this.userProfilePropertyAccessor.get(dc.context);
                // Handle case where user is re-introducing themselves. 
                // These utterances are defined in ../whoAreYou/resources/whoAreYou.lu 
                let userNameInOnTurnProperty = (onTurnProperty.entities || []).filter(item => item.entityName == USER_NAME);
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
                    if(JSON.parse(queryProperty[0].entityValue).text !== undefined) {
                        dc.context.activity.text = JSON.parse(queryProperty[0].entityValue).text;
                        await dc.context.sendActivity(`You said: '${dc.context.activity.text}'`);
                    }
                    return await this.beginChildDialog(dc, JSON.parse(queryProperty[0].entityValue));
                }
                return await this.whatCanYouDoDialog.onTurn(dc.context);
            }
        }
    }
    
    async resetTurnCounter(context) {
        this.turnCounterPropertyAccessor.set(context, 0);
    }

    async isRequestedOperationPossible(dc, requestedOperation) {
        let outcome = {allowed: true, reason: ''};
        
        // get active dialog from property accessor
        // TODO: Evaluate if this can be achieved via dc.activeDialog instead of a separate property
        let activeDialog = '';
        if(dc.activeDialog !== undefined) activeDialog = dc.activeDialog.id;

        // Book table submit and Book Table cancel requests through book table card are not allowed when Book Table is not the active dialog
        if(requestedOperation === 'Book_Table_Submit' || requestedOperation === 'Book_Table_Cancel') {
            if(activeDialog !== BookTableDialog.Name) {
                outcome.allowed = false;
                outcome.reason = `Sorry! I'm unable to process that. To start a new table reservation, try 'Book a table'`;
            }
        } 
        else if(requestedOperation === CancelDialog.Name) {
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
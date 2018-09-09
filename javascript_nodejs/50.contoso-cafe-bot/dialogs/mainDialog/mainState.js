// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const USER_PROFILE_PROPERTY = 'userProfile';
const USER_RESERVATIONS_PROPERTY = 'userReservations';
const USER_QUERY_PROPERTY = 'userQuery';
const ACTIVE_DIALOG_PROPERTY = 'activeDialog';
const MAIN_DIALOG_STATE_PROPERTY = 'mainDialogState';
/**
 * Bot state class. Holds property accessors to on turn property and dialog property
 */
class MainState {
    /**
     * Main State constructor.
     * 
     * @param {Object} conversationState conversation state object
     * @param {Object} userState user state object
     */
    constructor(conversationState, userState) {
        // holds property accessor to the following properties
        //      userState - userProfile, reservations, userQuery
        //      conversationState - activeDialog, mainDialog
        if(!conversationState) throw ('Need conversation state');
        if(!userState) throw ('Need user state');

        this.userProfilePropertyAccessor = userState.createProperty(USER_PROFILE_PROPERTY);
        this.reservationsPropertyAccessor = userState.createProperty(USER_RESERVATIONS_PROPERTY);
        this.userQueryPropertyAccessor = userState.createProperty(USER_QUERY_PROPERTY);
        this.activeDialogPropertyAccessor = conversationState.createProperty(ACTIVE_DIALOG_PROPERTY);
        this.mainDialogPropertyAccessor = conversationState.createProperty(MAIN_DIALOG_STATE_PROPERTY);
    }
};

module.exports = MainState;

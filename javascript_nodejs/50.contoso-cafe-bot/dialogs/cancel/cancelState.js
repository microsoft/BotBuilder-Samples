// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const CANCEL_STATE_PROPERTY = 'cancelStateProperty';
const CANCEL_DIALOG_PROPERTY = 'cancelDialogProperty';

/**
 * Bot state class. Holds property accessors to on turn property and dialog property
 */
class CancelState {
    /**
     * Cancel State constructor.
     * 
     * @param {Object} state
     */
    constructor(state) {
        if(!state) throw ('Need state');
        
        this.cancelStatePropertyAccessor = state.createProperty(CANCEL_STATE_PROPERTY);
        this.cancelDialogPropertyAccessor = state.createProperty(CANCEL_DIALOG_PROPERTY);
    }
};

module.exports = CancelState;

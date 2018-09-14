// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const INTERRUPTION_DISPATCHER_DIALOG = 'interruptionDispatcherDialog';
const INTERRUPTION_DISPATCHER_STATE_PROPERTY = 'interruptionDispatcherStateProperty';

const { ComponentDialog, DialogTurnStatus, DialogSet } = require('botbuilder-dialogs');

const { CancelDialog } = require('../cancel');

module.exports = {
    InterruptionDispatcher: class extends ComponentDialog {
        static get Name() { return INTERRUPTION_DISPATCHER_DIALOG; }
        constructor(onTurnPropertyAccessor, conversationState, userProfilePropertyAccessor) {
            super (INTERRUPTION_DISPATCHER_DIALOG);
            
            if (!onTurnPropertyAccessor) throw ('Missing parameter. On turn property accessor is required.');
            if (!conversationState) throw ('Missing parameter. Conversation state is required.');

            this.interruptionDispatcherPropertyAccessor = conversationState.createProperty(INTERRUPTION_DISPATCHER_STATE_PROPERTY);

            // keep on turn accessor
            this.onTurnPropertyAccessor = onTurnPropertyAccessor;

            // add dialogs
            this.dialogs = new DialogSet(this.mainDispatcherPropertyAccessor);

            this.addDialog(new CancelDialog());
        }
        /**
         * Override onDialogBegin 
         * 
         * @param {Object} dc dialog context
         * @param {Object} options dialog turn options
         */
        async onDialogBegin(dc, options) {
            // Override default begin() logic with interruption orchestration logic
            return await this.interruptionDispatch(dc, options);
        }
        /**
         * Override onDialogContinue
         * 
         * @param {Object} dc dialog context
         */
        async onDialogContinue(dc) {
            // Override default continue() logic with interruption orchestration logic
            return await this.interruptionDispatch(dc);
        }

        async interruptionDispatch(dc, options) {
            // See if interruption is allowed
            const x = dc.activeDialog;
            const y = options;
        }
    }
};
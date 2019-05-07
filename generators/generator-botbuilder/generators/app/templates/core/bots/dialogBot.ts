// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { ActivityHandler, BotState, ConversationState, StatePropertyAccessor, UserState } from 'botbuilder';
import { Dialog, DialogState } from 'botbuilder-dialogs';
import { MainDialog } from '../dialogs/mainDialog';
import { Logger } from '../logger';

export class DialogBot extends ActivityHandler {
    private conversationState: BotState;
    private userState: BotState;
    private logger: Logger;
    private dialog: Dialog;
    private dialogState: StatePropertyAccessor<DialogState>;

    /**
     *
     * @param {BotState} conversationState
     * @param {BotState} userState
     * @param {Dialog} dialog
     * @param {Logger} logger object for logging events, defaults to console if none is provided
     */
    constructor(conversationState: BotState, userState: BotState, dialog: Dialog, logger: Logger) {
        super();
        if (!conversationState) { throw new Error('[DialogBot]: Missing parameter. conversationState is required'); }
        if (!userState) { throw new Error('[DialogBot]: Missing parameter. userState is required'); }
        if (!dialog) { throw new Error('[DialogBot]: Missing parameter. dialog is required'); }
        if (!logger) {
            logger = console as Logger;
            logger.log('[DialogBot]: logger not passed in, defaulting to console');
        }

        this.conversationState = conversationState as ConversationState;
        this.userState = userState as UserState;
        this.dialog = dialog;
        this.logger = logger;
        this.dialogState = this.conversationState.createProperty<DialogState>('DialogState');

        this.onMessage(async (context, next) => {
            this.logger.log('Running dialog with Message Activity.');

            // Run the Dialog with the new message Activity.
            await (this.dialog as MainDialog).run(context, this.dialogState);

            // Save any state changes. The load happened during the execution of the Dialog.
            await this.conversationState.saveChanges(context, false);
            await this.userState.saveChanges(context, false);

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        this.onDialog(async (context, next) => {
            // Save any state changes. The load happened during the execution of the Dialog.
            await this.conversationState.saveChanges(context, false);
            await this.userState.saveChanges(context, false);

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }

}

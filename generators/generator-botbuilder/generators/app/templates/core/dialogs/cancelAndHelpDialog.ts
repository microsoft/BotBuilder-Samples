// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { ComponentDialog, DialogContext, DialogTurnResult, DialogTurnStatus } from 'botbuilder-dialogs';

/**
 * This base class watches for common phrases like "help" and "cancel" and takes action on them
 * BEFORE they reach the normal bot logic.
 */
export class CancelAndHelpDialog extends ComponentDialog {
    constructor(id: string) {
        super(id);
    }

    public async onBeginDialog(innerDc: DialogContext, options: any): Promise<DialogTurnResult> {
        const result = await this.interrupt(innerDc);
        if (result) {
            return result;
        }
        return await super.onBeginDialog(innerDc, options);
    }

    public async onContinueDialog(innerDc: DialogContext): Promise<DialogTurnResult> {
        const result = await this.interrupt(innerDc);
        if (result) {
            return result;
        }
        return await super.onContinueDialog(innerDc);
    }

    public async interrupt(innerDc: DialogContext): Promise<DialogTurnResult|undefined> {
        const text = innerDc.context.activity.text.toLowerCase();

        switch (text) {
            case 'help':
            case '?':
                await innerDc.context.sendActivity('[ This is where to send sample help to the user... ]');
                return { status: DialogTurnStatus.waiting };
            case 'cancel':
            case 'quit':
                await innerDc.context.sendActivity('Cancelling');
                return await innerDc.cancelAllDialogs();
        }

        return;
    }
}

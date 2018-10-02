// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

import { Activity, ActivityTypes } from "botbuilder";
import { ComponentDialog, Dialog, DialogContext, DialogTurnResult, DialogTurnStatus } from "botbuilder-dialogs";

export abstract class RouterDialog extends ComponentDialog {
    constructor(dialogId: string) { super(dialogId); }

    protected onBeginDialog(innerDC: DialogContext): Promise<DialogTurnResult> {
        return this.onContinueDialog(innerDC);
    }

    protected async onContinueDialog(innerDC: DialogContext): Promise<DialogTurnResult> {
        const activity: Activity = innerDC.context.activity;

        switch (activity.type) {
            case ActivityTypes.Message: {
                const result = await innerDC.continueDialog();
                switch (result.status) {
                    case DialogTurnStatus.empty: {
                        await this.route(innerDC);
                        break;
                    }
                    case DialogTurnStatus.complete: {
                        await this.complete(innerDC);

                        // End active dialog.
                        await innerDC.endDialog();
                        break;
                    }
                }
                break;
            }
            case ActivityTypes.Event: {
                await this.onEvent(innerDC);
                break;
            }
            case ActivityTypes.ConversationUpdate: {
                await this.onStart(innerDC);
                break;
            }
            default: {
                await this.onSystemMessage(innerDC);
                break;
            }
        }

        return Dialog.EndOfTurn;
    }

    protected abstract route(innerDC: DialogContext): Promise<void>;

    protected complete(innerDC: DialogContext): Promise<void> {
        return Promise.resolve();
    }

    protected onEvent(innerDC: DialogContext): Promise<void> {
        return Promise.resolve();
    }

    protected onStart(innerDC: DialogContext): Promise<void> {
        return Promise.resolve();
    }

    protected onSystemMessage(innerDC: DialogContext): Promise<void> {
        return Promise.resolve();
    }
}

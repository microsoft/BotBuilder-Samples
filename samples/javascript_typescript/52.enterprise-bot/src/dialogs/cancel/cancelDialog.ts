// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

import { CancelResponses } from './cancelResponses'
import { ComponentDialog, DialogContext, WaterfallStepContext, DialogTurnResult, PromptOptions, WaterfallDialog, ConfirmPrompt } from 'botbuilder-dialogs';

export class CancelDialog extends ComponentDialog {
    // Constants
    public static readonly CancelPrompt: string = 'cancelPrompt';

    // Fields
    private static _responder: CancelResponses;

    constructor() {
        super(CancelDialog.name);
        this.initialDialogId = CancelDialog.name;

        let cancel = [
            CancelDialog.AskToCancel,
            CancelDialog.FinishCancelDialog];

            this.addDialog(new WaterfallDialog(this.initialDialogId,cancel));
            this.addDialog(new ConfirmPrompt(CancelDialog.CancelPrompt));
    }

    public static async AskToCancel(sc: WaterfallStepContext): Promise<DialogTurnResult> {
        return await sc.prompt(
            CancelDialog.CancelPrompt,
            <PromptOptions> { prompt : await CancelDialog._responder.RenderTemplate(sc.context, 'en', CancelResponses._confirmPrompt)});
    }

    public static async FinishCancelDialog(sc: WaterfallStepContext): Promise<DialogTurnResult> {
        return await sc.endDialog(<boolean>sc.result);
    }

    protected async endComponent(outerDC: DialogContext, result: any)
    {
        let doCancel: boolean=result;

        if (doCancel)
        {
            // If user chose to cancel
            await CancelDialog._responder.ReplyWith(outerDC.context, CancelResponses._cancelConfirmed);

            // Cancel all in outer stack of component i.e. the stack the component belongs to
            return await outerDC.cancelAllDialogs();
        }
        else
        {
            // else if user chose not to cancel
            await CancelDialog._responder.ReplyWith(outerDC.context, CancelResponses._cancelDenied);

            // End this component. Will trigger reprompt/resume on outer stack
            return await outerDC.endDialog();
        }
    }
}
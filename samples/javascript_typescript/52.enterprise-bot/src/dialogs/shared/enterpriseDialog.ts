import { RecognizerResult } from 'botbuilder';
import { LuisRecognizer } from "botbuilder-ai";
import { DialogContext } from "botbuilder-dialogs";
import { BotServices } from "../../botServices";
import { CancelDialog } from "../cancel/cancelDialog";
import { CancelResponses } from "../cancel/cancelResponses";
import { InterruptableDialog } from "./interruptableDialog";
import { MainResponses } from "../main/mainResponses";
import { TelemetryLuisRecognizer } from "../../middleware/telemetry/telemetryLuisRecognizer";

export class EnterpriseDialog extends InterruptableDialog {

    // Fields
    private readonly _services: BotServices;
    private readonly _cancelResponder: CancelResponses = new CancelResponses();
    private readonly _mainResponder: MainResponses = new MainResponses();

    constructor(botServices: BotServices, dialogId: string) { 
        super(dialogId); 
        
        this._services = botServices;
        this.addDialog(new CancelDialog());
    }
    
    protected async onDialogInterruption(dc: DialogContext): Promise<InterruptionStatus> {
        // Check dispatch intent.
        const luisService: TelemetryLuisRecognizer | undefined = this._services.luisServices.get('<YOUR MSBOT NAME>_General');
        if (!luisService) return Promise.reject(new Error('Luis service not presented'));
        
        const luisResult: RecognizerResult = await luisService.recognize(dc.context);
        const intent: string = LuisRecognizer.topIntent(luisResult);
        
        switch (intent) {
            case 'Cancel':
            return await this.onCancel(dc);
            case 'Help':
            return await this.onHelp(dc);
        }

        return InterruptionStatus.NoAction;
    }

    protected async onCancel(dc: DialogContext): Promise<InterruptionStatus> {
        if (dc.activeDialog && dc.activeDialog.id != 'CancelDialog') {
            // Don't start restart cancel dialog.
            await dc.beginDialog('CancelDialog');

            // Signal that the dialog is waiting on user response.
            return InterruptionStatus.Waiting;
        }

        // Else, continue
        return InterruptionStatus.NoAction;
    }

    protected async onHelp(dc: DialogContext): Promise<InterruptionStatus> {
        this._mainResponder.ReplyWith(dc.context, MainResponses.Help);

        // Signal the conversation was interrupted and should immediately continue.
        return InterruptionStatus.Interrupted;
    }
}

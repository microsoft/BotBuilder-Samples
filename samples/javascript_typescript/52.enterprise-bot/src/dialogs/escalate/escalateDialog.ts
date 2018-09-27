// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { EnterpriseDialog } from '../shared/enterpriseDialog';
import { EscalateResponses } from './escalateResponses';
import { BotServices } from "../../botServices";
import { WaterfallStepContext, DialogTurnResult, WaterfallDialog } from 'botbuilder-dialogs';

export class EscalateDialog extends EnterpriseDialog {
    // Fields
    public static readonly _responder: EscalateResponses = new EscalateResponses();

    constructor(botServices: BotServices) {
        super(botServices, EscalateDialog.name);
        this.initialDialogId = EscalateDialog.name;

        let escalate = [EscalateDialog.SendPhone];

        this.addDialog(new WaterfallDialog(this.initialDialogId, escalate));
    }

    private static async SendPhone(sc: WaterfallStepContext): Promise<DialogTurnResult> {
        await EscalateDialog._responder.ReplyWith(sc.context, EscalateResponses.SendPhone);
        return await sc.endDialog();
    }
}
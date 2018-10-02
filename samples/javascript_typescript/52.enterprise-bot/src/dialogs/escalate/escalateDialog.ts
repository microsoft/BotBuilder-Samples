// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { DialogTurnResult, WaterfallDialog, WaterfallStepContext } from "botbuilder-dialogs";
import { BotServices } from "../../botServices";
import { EnterpriseDialog } from "../shared/enterpriseDialog";
import { EscalateResponses } from "./escalateResponses";

export class EscalateDialog extends EnterpriseDialog {
    // Fields
    public static readonly _responder: EscalateResponses = new EscalateResponses();

    private static async SendPhone(sc: WaterfallStepContext): Promise<DialogTurnResult> {
        await EscalateDialog._responder.replyWith(sc.context, EscalateResponses.SendPhone);
        return await sc.endDialog();
    }

    constructor(botServices: BotServices) {
        super(botServices, EscalateDialog.name);
        this.initialDialogId = EscalateDialog.name;

        const escalate = [
            EscalateDialog.SendPhone.bind(this),
        ];

        this.addDialog(new WaterfallDialog(this.initialDialogId, escalate));
    }
}

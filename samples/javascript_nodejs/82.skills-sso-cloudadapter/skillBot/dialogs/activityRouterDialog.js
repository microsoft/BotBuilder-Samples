// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, InputHints, MessageFactory } = require('botbuilder');
const { ComponentDialog, DialogTurnStatus, WaterfallDialog } = require('botbuilder-dialogs');
const { SsoSkillDialog } = require('./ssoSkillDialog');

const ACTIVITY_ROUTER_DIALOG = 'ActivityRouterDialog';
const WATERFALL_DIALOG = 'WaterfallDialog';
const SSO_SKILL_DIALOG = 'SsoSkillDialog';

/**
 * A root dialog that can route activities sent to the skill to different sub-dialogs.
 */
class ActivityRouterDialog extends ComponentDialog {
    constructor() {
        super(ACTIVITY_ROUTER_DIALOG);

        this.addDialog(new SsoSkillDialog(process.env.ConnectionName))
            .addDialog(new WaterfallDialog(WATERFALL_DIALOG, [
                this.processActivity.bind(this)
            ]));

        // The initial child Dialog to run.
        this.initialDialogId = WATERFALL_DIALOG;
    }

    async processActivity(stepContext) {
        // A skill can send trace activities, if needed.
        const traceActivity = {
            type: ActivityTypes.Trace,
            timestamp: new Date(),
            text: 'ActivityRouterDialog.processActivity()',
            label: `Got activityType: ${ stepContext.context.activity.type }`
        };
        await stepContext.context.sendActivity(traceActivity);

        // In this simple skill, we only handle SSO events
        if (stepContext.context.activity.type === ActivityTypes.Event && stepContext.context.activity.name === 'SSO') {
            return await stepContext.beginDialog(SSO_SKILL_DIALOG);
        }

        // We didn't get an activity type we can handle.
        await stepContext.context.sendActivity(MessageFactory.text(`Unrecognized ActivityType: ${ stepContext.context.activity.type }.`, undefined, InputHints.IgnoringInput));
        return { status: DialogTurnStatus.complete };
    }
}

module.exports.ActivityRouterDialog = ActivityRouterDialog;

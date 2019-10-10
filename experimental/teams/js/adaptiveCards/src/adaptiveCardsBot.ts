// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    CardFactory,
    InvokeResponse,
    MessageFactory,
    TaskModuleMessageResponse,
    TaskModuleRequest,
    TaskModuleResponseBase,
    TaskModuleTaskInfo,
    TeamsActivityHandler,
    TurnContext
} from 'botbuilder';

/**
 * You can @mention the bot the text "1", "2", or "3". "1" will send back adaptive cards. "2" will send back a
 * task module that contains an adpative card. "3" will return an adpative card that contains BF card actions.
 */
export class AdaptiveCardsBot extends TeamsActivityHandler {
    constructor() {
        super();

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {
            TurnContext.removeRecipientMention(context.activity);
            let text = context.activity.text;
            if (text && text.length > 0) {
                text = text.trim();
                switch (text) {
                    case '1':
                        await this.sendAdaptiveCard1(context);
                        break;

                    case '2':
                        await this.sendAdaptiveCard2(context);
                        break;

                    case '3':
                        await this.sendAdaptiveCard3(context);
                        break;

                    default:
                        await context.sendActivity(`You said: ${text}`);
                }
            } else {
                await context.sendActivity('App sent a message with empty text');
                const activityValue = context.activity.value;
                if (activityValue) {
                    await context.sendActivity(`but with value ${JSON.stringify(activityValue)}`);
                }
            }
            await next();
        });

        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            for (const member of membersAdded) {
                if (member.id !== context.activity.recipient.id) {
                    await context.sendActivity('Hello and welcome!');
                }
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }

    protected async onTeamsTaskModuleFetch(context: TurnContext, taskModuleRequest: TaskModuleRequest): Promise<TaskModuleTaskInfo> {
        await context.sendActivity(MessageFactory.text(`OnTeamsTaskModuleFetchAsync TaskModuleRequest: ${JSON.stringify(taskModuleRequest)}`));

        /**
         * The following line disables the lint rules for the Adaptive Card so that users can
         * easily copy-paste the object into the Adaptive Cards Designer to modify their cards
         * The Designer can be found here: https://adaptivecards.io/designer/
         */
        /* tslint:disable:quotemark object-literal-key-quotes */
        const card = CardFactory.adaptiveCard({
            "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
            "actions": [
                {
                    "data": {
                        "submitLocation": "taskModule"
                    },
                    "title": "Action.Submit",
                    "type": "Action.Submit"
                }
            ],
            "body": [
                {
                    "text": "This is an Adaptive Card within a Task Module",
                    "type": "TextBlock"
                }
            ],
            "type": "AdaptiveCard",
            "version": "1.0"
        });
        /* tslint:enable:quotemark object-literal-key-quotes */
        return {
            card,
            height: 200,
            title: 'Task Module Example',
            width: 400
        } as TaskModuleTaskInfo;
    }

    protected async onTeamsTaskModuleSubmit(context: TurnContext, taskModuleRequest: TaskModuleRequest): Promise<TaskModuleResponseBase> {
        await context.sendActivity(MessageFactory.text(`OnTeamsTaskModuleSubmit value: ${JSON.stringify(taskModuleRequest)}`));
        return { type: 'message', value: 'Thanks!' } as TaskModuleMessageResponse;
    }

    protected async onTeamsCardActionInvoke(context: TurnContext): Promise<InvokeResponse> {
        await context.sendActivity(MessageFactory.text(`OnTeamsCardActionInvoke value: ${JSON.stringify(context.activity.value)}`));
        return { status: 200 } as InvokeResponse;
    }

    private async sendAdaptiveCard1(context: TurnContext): Promise<void> {
        /* tslint:disable:quotemark object-literal-key-quotes */
        const card = CardFactory.adaptiveCard({
            "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
            "actions": [
                {
                    "data": {
                        "msteams": {
                            "type": "imBack",
                            "value": "text"
                        }
                    },
                    "title": "imBack",
                    "type": "Action.Submit"
                },
                {
                    "data": {
                        "msteams": {
                            "type": "messageBack",
                            "value": { "key": "value" }
                        }
                    },
                    "title": "message back",
                    "type": "Action.Submit"
                },
                {
                    "data": {
                        "msteams": {
                            "displayText": "display text message back",
                            "text": "text received by bots",
                            "type": "messageBack",
                            "value": { "key": "value" }
                        }
                    },
                    "title": "message back local echo",
                    "type": "Action.Submit"
                },
                {
                    "data": {
                        "msteams": {
                            "type": "invoke",
                            "value": { "key": "value" }
                        }
                    },
                    "title": "invoke",
                    "type": "Action.Submit"
                }
            ],
            "body": [
                {
                    "text": "Bot Builder actions",
                    "type": "TextBlock"
                }
            ],
            "type": "AdaptiveCard",
            "version": "1.0"
        });
        /* tslint:enable:quotemark object-literal-key-quotes */
        await context.sendActivity(MessageFactory.attachment(card));
    }

    private async sendAdaptiveCard2(context: TurnContext): Promise<void> {
        /* tslint:disable:quotemark object-literal-key-quotes */
        const card = CardFactory.adaptiveCard({
            "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
            "actions": [
                {
                    "data": {
                        "msteams": {
                            "type": "invoke",
                            "value": {
                                "hiddenKey": "hidden value from task module launcher",
                                "type": "task/fetch"
                            }
                        }
                    },
                    "title": "Launch Task Module",
                    "type": "Action.Submit"
                }
            ],
            "body": [
                {
                    "text": "Task Module Adaptive Card",
                    "type": "TextBlock"
                }
            ],
            "type": "AdaptiveCard",
            "version": "1.0"
        });
        /* tslint:enable:quotemark object-literal-key-quotes */
        await context.sendActivity(MessageFactory.attachment(card));
    }

    private async sendAdaptiveCard3(context: TurnContext): Promise<void> {
        /* tslint:disable:quotemark object-literal-key-quotes */
        const card = CardFactory.adaptiveCard({
            "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
            "actions": [
                {
                    "data": {
                        "key": "value"
                    },
                    "title": "Action.Submit",
                    "type": "Action.Submit"
                }
            ],
            "body": [
                {
                    "text": "Bot Builder actions",
                    "type": "TextBlock"
                },
                {
                    "id": "x",
                    "type": "Input.Text"
                }
            ],
            "type": "AdaptiveCard",
            "version": "1.0"
        });
        /* tslint:enable:quotemark object-literal-key-quotes */
        await context.sendActivity(MessageFactory.attachment(card));
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    MessagingExtensionActionResponse,
    MessagingExtensionAction,
    TaskModuleContinueResponse,
    TaskModuleMessageResponse,
    TaskModuleResponseBase,
    TeamsActivityHandler,
} from 'botbuilder-teams';

import {
    Activity,
    Attachment,
    CardFactory
} from 'botbuilder';

export class TeamsActionExtensionBot extends TeamsActivityHandler {
    constructor() {
        super();

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {
            await context.sendActivity(`You said '${context.activity.text}'`);
            // By calling next() you ensure that the next BotHandler is run.
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

        // This method fires when an user uses an Action-based Messaging Extension from the Teams Client.
        // It should send back the tab or task module for the user to interact with.
        this.onMessagingExtensionFetchTask(async (context, value, next) => {
            return {
                status: 200,
                body: {
                    task: this.taskModuleResponse(value, false)
                }
            };
        });

        this.onBotMessagePreviewEdit(async (context, value: MessagingExtensionAction, next) => {
            const card = this.getCardFromPreviewMessage(value);
            let body: MessagingExtensionActionResponse;
            if (!card) {
                body = {
                    task: <TaskModuleMessageResponse>{
                        type: 'message',
                        value: 'Missing user edit card. Something wrong on Teams client.'
                    }
                }
            } else {
                body = {
                    task: <TaskModuleContinueResponse>{
                        type: 'continue',
                        value: { card }
                    }
                }
            }

            return { status: 200, body };
        });

        this.onBotMessagePreviewSend(async (context, value: MessagingExtensionAction, next) => {
            let body: MessagingExtensionActionResponse;
            const card = this.getCardFromPreviewMessage(value);
            if (!card) {
                body = {
                    task: <TaskModuleMessageResponse>{
                        type: 'message',
                        value: 'Missing user edit card. Something wrong on Teams client.'
                    }
                }
            } else {
                body = undefined;
                await context.sendActivity({ attachments: [card] });
            }

            return { status: 200, body };
        });

        this.onMessagingExtensionSubmit(async (context, value: MessagingExtensionAction, next) => {
            const data = value.data;
            let body: MessagingExtensionActionResponse;
            if (data && data.done) {
                // The commandContext check doesn't need to be used in this scenario as the manifest specifies the shareMessage command only works in the "message" context.
                let sharedMessage = (value.commandId === 'shareMessage' && value.commandContext === 'message')
                    ? `Shared message: <div style="background:#F0F0F0">${JSON.stringify(value.messagePayload)}</div><br/>`
                    : '';
                let preview = CardFactory.thumbnailCard('Created Card', `Your input: ${data.userText}`);
                let heroCard = CardFactory.heroCard('Created Card', `${sharedMessage}Your input: <pre>${data.userText}</pre>`);
                body = {
                    composeExtension: {
                        type: 'result',
                        attachmentLayout: 'list',
                        attachments: [
                            { ...heroCard, preview }
                        ]
                    }
                }
            } else if (value.commandId === 'createWithPreview') {
                const activityPreview = {
                    attachments: [
                        this.taskModuleResponseCard(value)
                    ]
                } as Activity;

                body = {
                    composeExtension: {
                        type: 'botMessagePreview',
                        activityPreview
                    }
                };
            } else {
                body = {
                    task: this.taskModuleResponse(value, false)
                }
            }

            return { status: 200, body };
        });
    }

    private getCardFromPreviewMessage(query: MessagingExtensionAction): Attachment {
        const userEditActivities = query.botActivityPreview;
        return userEditActivities
            && userEditActivities[0]
            && userEditActivities[0].attachments
            && userEditActivities[0].attachments[0];
    }

    private taskModuleResponse(query: any, done: boolean): TaskModuleResponseBase {
        if (done) {
            return <TaskModuleMessageResponse>{
                type: 'message',
                value: 'Thanks for your inputs!'
            }
        } else {
            return <TaskModuleContinueResponse>{
                type: 'continue',
                value: {
                    title: 'More Page',
                    card: this.taskModuleResponseCard(query, (query.data && query.data.userText) || undefined)
                }
            };
        }
    }

    private taskModuleResponseCard(data: any, textValue?: string) {
        return CardFactory.adaptiveCard({
            version: '1.0.0',
            type: 'AdaptiveCard',
            body: [
                {
                    type: 'TextBlock',
                    text: `Your request:`,
                    size: 'large',
                    weight: 'bolder'
                },
                {
                    type: 'Container',
                    style: 'emphasis',
                    items: [
                        {
                            type: 'TextBlock',
                            text: JSON.stringify(data),
                            wrap: true
                        }
                    ]
                },
                {
                    type: 'Input.Text',
                    id: 'userText',
                    placeholder: 'Type text here...',
                    value: textValue
                }
            ],
            actions: [
                {
                    type: 'Action.Submit',
                    title: 'Next',
                    data: {
                        done: false
                    }
                },
                {
                    type: 'Action.Submit',
                    title: 'Submit',
                    data: {
                        done: true
                    }
                }
            ]
        })
    }
}

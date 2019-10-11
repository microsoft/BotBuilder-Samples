// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    MessagingExtensionActionResponse,
    MessagingExtensionResult,
    MessagingExtensionSuggestedAction,
    MessagingExtensionQuery,
    TeamsActivityHandler,
    TurnContext,
} from 'botbuilder';

import {
    ActionTypes,
} from 'botframework-schema'

export class MessagingExtensionConfigBot  extends TeamsActivityHandler {
    constructor() {
        super();

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {
            await context.sendActivity(`echo: '${context.activity.text}'`);
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
    }

    protected async onTeamsMessagingExtensionConfigurationQuerySettingUrl(context: TurnContext, query: MessagingExtensionQuery){
        return <MessagingExtensionActionResponse>
        {
            composeExtension: <MessagingExtensionResult> {
                type: 'config',
                suggestedActions: <MessagingExtensionSuggestedAction> { 
                    actions: [
                        {
                            type: ActionTypes.OpenUrl,
                            value: 'https://teamssettingspagescenario.azurewebsites.net',
                        },
                    ]
                    }
            }
        }
    }

    protected async onTeamsMessagingExtensionConfigurationSetting(context: TurnContext, settings){
        // This event is fired when the settings page is submitted
        await context.sendActivity(`onTeamsMessagingExtensionSettings event fired with ${ JSON.stringify(settings) }`);
    }
}

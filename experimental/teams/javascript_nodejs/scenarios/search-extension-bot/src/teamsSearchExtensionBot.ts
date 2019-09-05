// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    MessagingExtensionResponse,
    MessagingExtensionQuery,
    TeamsActivityHandler,
    InvokeResponseTyped,
    MessagingExtensionSuggestedAction
} from 'botbuilder-teams';

import {
    ActionTypes,
    CardAction,
    CardFactory,
    BotState
} from 'botbuilder';

const RICH_CARD_PROPERTY = 'richCardConfig';

export class TeamsSearchExtensionBot extends TeamsActivityHandler {    

    // For this example, we're using UserState to store the user's preferences for the type of Rich Card they receive.
    // Users can specify the type of card they receive by configuring the Messaging Extension.
    // To store their configuration, we will use the userState passed in via the constructor.

    /**
     * We need to change the key for the user state because the bot might not be in the conversation, which means they get a 403 error.
     * @param userState 
     */
    constructor(public userState: BotState) {
        super();

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {
            await context.sendActivity(`You said '${ context.activity.text }'`);
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

        this.onMessagingExtensionQuery(async (context, query, next) => {
            const accessor = this.userState.createProperty<{useHeroCard: boolean}>(RICH_CARD_PROPERTY);
            const config = await accessor.get(context, { useHeroCard: true });
            
            const searchQuery = query.parameters[0].value;
            const cardText = `You said "${ searchQuery }"`;
            let composeExtensionResponse;

            const bfLogo = 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQtB3AwMUeNoq4gUBGe6Ocj8kyh3bXa9ZbV7u1fVKQoyKFHdkqU'
            const button = { type: 'openUrl', title: 'Click for more Information', value: "https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bots-overview" };

            if (config.useHeroCard) {
                const heroCard = CardFactory.heroCard('You searched for:', cardText, [bfLogo]);
    
                const secondPreview = CardFactory.heroCard('Learn more about Teams:', "https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bots-overview", [bfLogo]);
                const secondHeroCard = CardFactory.heroCard('Learn more about Teams:', "https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bots-overview", [bfLogo], [button]);
                composeExtensionResponse = {
                    composeExtension: {
                        type: 'result',
                        attachmentLayout: 'list',
                        attachments: [
                            { ...heroCard, preview: heroCard },
                            { ... secondHeroCard, preview: secondPreview }
                        ]
                    }
                }
            } else {
                const thumbnailCard = CardFactory.thumbnailCard('You searched for:', cardText, [bfLogo]);
    
                const secondPreview = CardFactory.thumbnailCard('Learn more about Teams:', "https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bots-overview", [bfLogo]);
                const secondThumbnailCard = CardFactory.thumbnailCard('Learn more about Teams:', "https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bots-overview", [bfLogo], [button]);
                composeExtensionResponse = {
                    composeExtension: {
                        type: 'result',
                        attachmentLayout: 'list',
                        attachments: [
                            { ...thumbnailCard, preview: thumbnailCard },
                            { ...secondThumbnailCard, preview: secondPreview }
                        ]
                    }
                }
            }

            const response: InvokeResponseTyped<MessagingExtensionResponse> = {
                status: 200,
                body: composeExtensionResponse as MessagingExtensionResponse
            };

            // For Invoke activities from Teams, we're currently not continuing the chain of handlers.
            // await next();

            return Promise.resolve(response);
        });

        this.onMessagingExtensionQuerySetting(async (context, value, next) => {
            const accessor = this.userState.createProperty<{useHeroCard: boolean}>(RICH_CARD_PROPERTY);
            const config = await accessor.get(context, { useHeroCard: true });

            if ((value as MessagingExtensionQuery).state === 'hero') {
                config.useHeroCard = true;
            } else if ((value as MessagingExtensionQuery).state === 'thumbnail') {
                config.useHeroCard = false;
            }

            const response: InvokeResponseTyped<MessagingExtensionResponse> = {
                status: 200,
                body: {}
            };
            // For Invoke activities from Teams, we're currently not continuing the chain of handlers.
            // await next();

            // We should save it after we send the message back to Teams.
            await this.userState.saveChanges(context);

            return Promise.resolve(response);
        });

        this.onMessagingExtensionQuerySettingUrl(async (context, value, next) => {
            const configExp: CardAction = {
                type: ActionTypes.OpenUrl,
                title: 'Config',
                value: process.env.host + '/composeExtensionSettings.html'
            }

            const messagingSuggestedActions: MessagingExtensionSuggestedAction = {
                actions: [configExp]
            }

            const composeExtensionResponse: MessagingExtensionResponse = {
                composeExtension: {
                    suggestedActions: messagingSuggestedActions,
                    type: 'config',
                }
            }

            const response: InvokeResponseTyped<MessagingExtensionResponse> = {
                status: 200,
                body: composeExtensionResponse
            };

            // For Invoke activities from Teams, we're currently not continuing the chain of handlers.
            // await next();
            return Promise.resolve(response);
        });
    }
}

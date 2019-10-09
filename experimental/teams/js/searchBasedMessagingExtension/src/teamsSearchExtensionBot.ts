// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    ActionTypes,
    CardFactory,
    BotState,
    MessagingExtensionResponse,
    MessagingExtensionResult,
    MessagingExtensionQuery,
    TeamsActivityHandler,
    TurnContext,
    MessagingExtensionSuggestedAction,
    MessagingExtensionActionResponse,
    AppBasedLinkQuery
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
    }

    protected async onTeamsMessagingExtensionQuery(context: TurnContext, query: MessagingExtensionQuery): Promise<MessagingExtensionResponse>{
        const accessor = this.userState.createProperty<{ useHeroCard: boolean }>(RICH_CARD_PROPERTY);
        const config = await accessor.get(context, { useHeroCard: true });

        const searchQuery = query.parameters[0].value;
        const cardText = `You said "${searchQuery}"`;
        let composeExtensionResponse: MessagingExtensionResponse;

        const bfLogo = 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQtB3AwMUeNoq4gUBGe6Ocj8kyh3bXa9ZbV7u1fVKQoyKFHdkqU';
        const button = { type: 'openUrl', title: 'Click for more Information', value: "https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bots-overview" };

        if (config.useHeroCard) {
            const heroCard = CardFactory.heroCard('You searched for:', cardText, [bfLogo], [button]);
            const preview = CardFactory.heroCard('You searched for:', cardText, [bfLogo]);

            const secondPreview = CardFactory.heroCard('Learn more about Teams:', "https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bots-overview", [bfLogo]);
            const secondHeroCard = CardFactory.heroCard('Learn more about Teams:', "https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bots-overview", [bfLogo], [button]);
            composeExtensionResponse = {
                composeExtension: {
                    type: 'result',
                    attachmentLayout: 'list',
                    attachments: [
                        { ...heroCard, preview },
                        { ...secondHeroCard, preview: secondPreview }
                    ]
                }
            }
        } else {
            const thumbnailCard = CardFactory.thumbnailCard('You searched for:', cardText, [bfLogo], [button]);
            const preview = CardFactory.thumbnailCard('You searched for:', cardText, [bfLogo]);

            const secondPreview = CardFactory.thumbnailCard('Learn more about Teams:', "https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bots-overview", [bfLogo]);
            const secondThumbnailCard = CardFactory.thumbnailCard('Learn more about Teams:', "https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bots-overview", [bfLogo], [button]);
            composeExtensionResponse = {
                composeExtension: {
                    type: 'result',
                    attachmentLayout: 'list',
                    attachments: [
                        { ...thumbnailCard, preview },
                        { ...secondThumbnailCard, preview: secondPreview }
                    ]
                }
            }
        }

        return composeExtensionResponse;
    }
    
    protected async onTeamsAppBasedLinkQuery(context: TurnContext, query: AppBasedLinkQuery): Promise<MessagingExtensionResponse>{
        const accessor = this.userState.createProperty<{ useHeroCard: boolean }>(RICH_CARD_PROPERTY);
        const config = await accessor.get(context, { useHeroCard: true });

        const url = query.url;
        const cardText = `You entered "${url}"`;
        let composeExtensionResponse: MessagingExtensionResponse;

        const bfLogo = 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQtB3AwMUeNoq4gUBGe6Ocj8kyh3bXa9ZbV7u1fVKQoyKFHdkqU';
        const button = { type: 'openUrl', title: 'Click for more Information', value: "https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bots-overview" };

        if (config.useHeroCard) {
            const heroCard = CardFactory.heroCard('HeroCard for Link Unfurling:', cardText, [bfLogo], [button]);
            const preview = CardFactory.heroCard('HeroCard for Link Unfurling:', cardText, [bfLogo]);

            composeExtensionResponse = {
                composeExtension: {
                    type: 'result',
                    attachmentLayout: 'list',
                    attachments: [
                        { ...heroCard, preview }
                    ]
                }
            }
        } else {
            const thumbnailCard = CardFactory.thumbnailCard('ThumbnailCard for Link Unfurling:', cardText, [bfLogo], [button]);
            const preview = CardFactory.thumbnailCard('ThumbnailCard for Link Unfurling:', cardText, [bfLogo]);

            composeExtensionResponse = {
                composeExtension: {
                    type: 'result',
                    attachmentLayout: 'list',
                    attachments: [
                        { ...thumbnailCard, preview }
                    ]
                }
            }
        }

        return composeExtensionResponse;
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
                            title: 'Config',
                            value: process.env.host + '/composeExtensionSettings.html',
                        },
                    ]
                }
            }
        }
    }

    protected async onTeamsMessagingExtensionConfigurationSetting(context: TurnContext, settings: MessagingExtensionQuery){
        // This event is fired when the settings page is submitted
        const accessor = this.userState.createProperty<{ useHeroCard: boolean }>(RICH_CARD_PROPERTY);
        const config = await accessor.get(context, { useHeroCard: true });

        if (settings.state === 'hero') {
            config.useHeroCard = true;
        } else if (settings.state === 'thumbnail') {
            config.useHeroCard = false;
        }

        // We should save it after we send the message back to Teams.
        await this.userState.saveChanges(context);
    }
}

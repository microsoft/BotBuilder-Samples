// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { TeamsActivityHandler, CardFactory, ActionTypes } = require('botbuilder');
const axios = require('axios');
const querystring = require('querystring');
const { SimpleGraphClient } = require('..\\simpleGraphClient.js');

// User Configuration property name
const USER_CONFIGURATION = 'userConfigurationProperty';

class TeamsMessagingExtensionsSearchAuthConfigBot extends TeamsActivityHandler {
    /**
     *
     * @param {UserState} User state to persist configuration settings
     */
    constructor(userState) {
        super();
        // Creates a new user property accessor.
        // See https://aka.ms/about-bot-state-accessors to learn more about the bot state and state accessors.
        this.userConfigurationProperty = userState.createProperty(USER_CONFIGURATION);
        this.connectionName = process.env.ConnectionName;
        this.userState = userState;
    }

    async handleTeamsMessagingExtensionConfigurationQuerySettingUrl(context, query) {
        // The user has requested the Messaging Extension Configuration page settings url.
        const userSettings = await this.userConfigurationProperty.get(context, '');
        let escapedSettings = '';
        if (userSettings) {
            escapedSettings = querystring.escape(userSettings);
        }

        return {
            composeExtension: {
                type: 'config',
                suggestedActions: {
                    actions: [
                        {
                            type: ActionTypes.OpenUrl,
                            value: process.env.SiteUrl + '/public/searchSettings.html?settings=' + escapedSettings
                        }
                    ]
                }
            }
        };
    }

    async handleTeamsMessagingExtensionConfigurationSetting(context, settings) {
        // When the user submits the settings page, this event is fired.
        if (settings.state != null) {
            await this.userConfigurationProperty.set(context, settings.state);

            await this.userState.saveChanges(context, false);
        }
    }

    async handleTeamsMessagingExtensionQuery(context, query) {
        const searchQuery = query.parameters[0].value;
        const attachments = [];

        const userSettings = await this.userConfigurationProperty.get(context, '');
        if (userSettings && userSettings.includes('email')) {
            // When the Bot Service Auth flow completes, the query.State will contain a magic code used for verification.
            let magicCode = '';
            if (query.state && Number.isInteger(Number(query.state))) {
                magicCode = query.state;
            }

            var tokenResponse = await context.adapter.getUserToken(context, this.connectionName, magicCode);
            if (!tokenResponse || !tokenResponse.token) {
                // There is no token, so the user has not signed in yet.

                // Retrieve the OAuth Sign in Link to use in the MessagingExtensionResult Suggested Actions
                const signInLink = await context.adapter.getSignInLink(context, this.connectionName);

                return {
                    composeExtension: {
                        type: 'auth',
                        suggestedActions: {
                            actions: [{
                                type: 'openUrl',
                                value: signInLink,
                                title: 'Bot Service OAuth'
                            }]
                        }
                    }
                };
            }

            // The user is signed in, so use the token to create a Graph Clilent and search their email
            const graphClient = new SimpleGraphClient(tokenResponse.token);
            var messages = await graphClient.searchMailInbox(searchQuery);

            // Here we construct a ThumbnailCard for every attachment, and provide a HeroCard which will be
            // displayed if the selects that item.
            messages.value.forEach(msg => {
                const heroCard = CardFactory.heroCard(msg.from.emailAddress.address);
                heroCard.content.subtitle = msg.subject;
                heroCard.content.text = msg.body.content;
                const heroCardContent = heroCard.content;

                const preview = CardFactory.thumbnailCard(msg.from.emailAddress.address,
                    msg.subject + '<br />' + msg.bodyPreview,
                    ['https://raw.githubusercontent.com/microsoft/botbuilder-samples/master/docs/media/OutlookLogo.jpg']);
                const attachment = { contentType: heroCard.contentType, content: heroCardContent, preview: preview };
                attachments.push(attachment);
            });
        } else {
            const response = await axios.get(`http://registry.npmjs.com/-/v1/search?${ querystring.stringify({ text: searchQuery, size: 8 }) }`);

            response.data.objects.forEach(obj => {
                const heroCard = CardFactory.heroCard(obj.package.name);
                const preview = CardFactory.heroCard(obj.package.name);
                preview.content.tap = { type: 'invoke', value: { description: obj.package.description } };
                const attachment = { ...heroCard, preview };
                attachments.push(attachment);
            });
        }

        return {
            composeExtension: {
                type: 'result',
                attachmentLayout: 'list',
                attachments: attachments
            }
        };
    }

    async handleTeamsMessagingExtensionSelectItem(context, obj) {
        return {
            composeExtension: {
                type: 'result',
                attachmentLayout: 'list',
                attachments: [CardFactory.thumbnailCard(obj.description)]
            }
        };
    }

    async handleTeamsMessagingExtensionFetchTask(context, action) {
        if (action.commandId === 'SignOutCommand') {
            const adapter = context.adapter;
            await adapter.signOutUser(context, this.connectionName);

            const card = CardFactory.adaptiveCard({
                version: '1.0.0',
                type: 'AdaptiveCard',
                body: [
                    {
                        type: 'TextBlock',
                        text: `You have been signed out.`
                    }
                ],
                actions: [
                    {
                        type: 'Action.Submit',
                        title: 'Close',
                        data: {
                            key: 'close'
                        }
                    }
                ]
            });

            return {
                task: {
                    type: 'continue',
                    value: {
                        card: card,
                        heigth: 200,
                        width: 400,
                        title: 'Adaptive Card: Inputs'
                    }
                }
            };
        }
    }

    async handleTeamsMessagingExtensionSubmitAction(context, action) {
        return {};
    }
}

module.exports.TeamsMessagingExtensionsSearchAuthConfigBot = TeamsMessagingExtensionsSearchAuthConfigBot;

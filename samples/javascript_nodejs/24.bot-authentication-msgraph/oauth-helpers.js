// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes } = require('botbuilder');
const { OAuthPrompt } = require('botbuilder-dialogs');
const { SimpleGraphClient } = require('./simple-graph-client');

// DialogId for the OAuthPrompt.
const LOGIN_PROMPT = 'loginPrompt';

/**
 * These methods call the Microsoft Graph API. The following OAuth scopes are used:
 * 'OpenId' 'email' 'Mail.Send.Shared' 'Mail.Read' 'profile' 'User.Read' 'User.ReadBasic.All'
 * for more information about scopes see:
 * https://developer.microsoft.com/en-us/graph/docs/concepts/permissions_reference
 */
class OAuthHelpers {
    /**
     * Enable the user to send an email via the bot.
     * @param {TurnContext} turnContext A TurnContext instance containing all the data needed for processing this conversation turn.
     * @param {TokenResponse} tokenResponse A response that includes a user token.
     * @param {string} emailAddress The email address of the recipient.
     */
    static async sendMail(turnContext, tokenResponse, emailAddress) {
        if (!turnContext) {
            throw new Error('OAuthHelpers.sendMail(): `turnContext` cannot be undefined.');
        }
        if (!tokenResponse) {
            throw new Error('OAuthHelpers.sendMail(): `tokenResponse` cannot be undefined.');
        }

        const client = new SimpleGraphClient(tokenResponse.token);
        const me = await client.getMe();

        await client.sendMail(
            emailAddress,
            `Message from a bot!`,
            `Hi there! I had this message sent from a bot. - Your friend, ${ me.displayName }`
        );
        await turnContext.sendActivity(`I sent a message to ${ emailAddress } from your account.`);
    }

    /**
     * Displays information about the user in the bot.
     * @param {TurnContext} turnContext A TurnContext instance containing all the data needed for processing this conversation turn.
     * @param {TokenResponse} tokenResponse A response that includes a user token.
     */
    static async listMe(turnContext, tokenResponse) {
        if (!turnContext) {
            throw new Error('OAuthHelpers.listMe(): `turnContext` cannot be undefined.');
        }
        if (!tokenResponse) {
            throw new Error('OAuthHelpers.listMe(): `tokenResponse` cannot be undefined.');
        }

        try {
            // Pull in the data from Microsoft Graph.
            const client = new SimpleGraphClient(tokenResponse.token);
            const me = await client.getMe();
            const manager = await client.getManager();

            // Create the reply activity.
            let reply = { type: ActivityTypes.Message };
            reply.text = `You are ${ me.displayName } and you report to ${ manager.displayName }.`;
            await turnContext.sendActivity(reply);
        } catch (error) {
            throw error;
        }
    }

    /**
     * Lists the user's collected email.
     * @param {TurnContext} turnContext A TurnContext instance containing all the data needed for processing this conversation turn.
     * @param {TokenResponse} tokenResponse A response that includes a user token.
     */
    static async listRecentMail(turnContext, tokenResponse) {
        if (turnContext === undefined) {
            throw new Error('OAuthHelpers.listRecentMail(): `turnContext` cannot be undefined.');
        }
        if (tokenResponse === undefined) {
            throw new Error('OAuthHelpers.listRecentMail(): `tokenResponse` cannot be undefined.');
        }

        var client = new SimpleGraphClient(tokenResponse.token);
        var messages = await client.getRecentMail();

        // Constructs and sends activities with information about the received `message`.
        async function sendMessageInfo(message) {
            const from = message.from.emailAddress.name;
            const address = message.from.emailAddress.address;
            const subject = message.subject;
            const messagePreview = message.bodyPreview;
            const email = {
                type: ActivityTypes.Message,
                text: `From: ${ from }\n` +
                    `Email: ${ address }\n` +
                    `Subject: ${ subject }\n` +
                    `Message: ${ messagePreview }`
            };
            await this.sendActivity(email);
        }

        const preparedMessages = messages.value.map(sendMessageInfo.bind(turnContext));
        await Promise.all(preparedMessages);
    }

    /**
     * Prompts the user to log in using the OAuth provider specified by the connection name.
     * @param {string} connectionName The connectionName from Azure when the OAuth provider is created.
     */
    static prompt(connectionName) {
        const loginPrompt = new OAuthPrompt(LOGIN_PROMPT,
            {
                connectionName: connectionName,
                text: 'Please login',
                title: 'Login',
                timeout: 30000 // User has 5 minutes to login.
            });
        return loginPrompt;
    }
}

exports.OAuthHelpers = OAuthHelpers;
exports.LOGIN_PROMPT = LOGIN_PROMPT;

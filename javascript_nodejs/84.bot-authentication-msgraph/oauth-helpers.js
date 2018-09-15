// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes } = require('botbuilder');
const { OAuthPrompt } = require('botbuilder-dialogs');
const { SimpleGraphClient } = require('./simple-graph-client');

/**
* This method calls the Microsoft Graph API. The following OAuth scopes are used:
* 'OpenId' 'email' 'Mail.Send.Shared' 'Mail.Read' 'profile' 'User.Read' 'User.ReadBasic.All'
* for more information about scopes see:
* https://developer.microsoft.com/en-us/graph/docs/concepts/permissions_reference
*/
class OAuthHelpers {

    // Enable the user to send an email via the bot
    async sendMail(turnContext, tokenResponse, recipient) {

        if (turnContext === undefined) {
            throw new Error(turnContext.name);
        }

        if (tokenResponse === undefined) {
            throw new Error(tokenResponse.name);
        }

        const client = new SimpleGraphClient(tokenResponse.token);
        const me = await client.getMe();

        await client.sendMail(
            recipient,
            `Message from a bot!`,
            `Hi there! I had this message sent from a bot. - Your friend, ${me.displayName}`
        );
        await step.context.sendActivity(`I sent a message to ${recipient} from your account.`);
        return await turnContext.end();
    }

    // Displays information about the user in the bot.
    async listMe(turnContext, tokenResponse) {

        if (turnContext === undefined) {
            throw new Error(turnContext.name);
        }

        if (tokenResponse === undefined) {
            throw new Error(tokenResponse.name);
        }

        try {
            // Pull in the data from Microsoft Graph.
            const client = new SimpleGraphClient(tokenResponse.token);
            const me = await client.getMe();
            const manager = await client.getManager();
            const photo = await client.getPhoto();

            // Generate the reply activity.
            let reply = { type: ActivityTypes.Message };
            let photoText = '';
            if (photo !== undefined) {
                const replyAttachment = { "contentType": photo.contentType, "contentUrl": photo.base64string }
                reply.attachments = [replyAttachment];
            }
            else {
                photoText = `Consider adding an image to your Outlook profile.`;
            }

            reply.text = `You are ${me.displayName} and you report to ${manager.displayName}. ${photoText}`;
            await turnContext.context.sendActivity(reply);
            // await turnContext.context.sendActivity(reply.text + { attachments: [{ reply }] });
        }
        catch (error) {
            throw error;
        }
    }

    // Lists the user's collected email.
    async listRecentMail(turnContext, tokenResponse) {

        if (turnContext === undefined) {
            throw new Error(turnContext.name);
        }

        if (tokenResponse == undefined) {
            throw new Error(tokenResponse.name);
        }

        var client = new SimpleGraphClient(tokenResponse.Token);
        var messages = await client.GetRecentMail();

        if (messages) {
            let count = messages.length();
            if (count > 5) {
                count = 1
            }

            for (var i = 0; i < count; i++) {
                from = messages[i].from.emailAddress.name;
                address = messages[i].from.emailAddress.address;
                subject = messages[i].subject;
                message = messages[i].bodyPreview;
                email = [
                    {
                        "type": "message",
                        "text":
                            `From: ${from} /\r\n/` +
                            `Email: ${address} /\r\n` +
                            `Subject: ${subject} /\r\n/` +
                            `Message: ${message}`
                    }
                ];
                email = email[0].text.replace(/\//g, "");
                await step.context.sendActivity(email);
            }
        }
        return await turnContext.end();
    }

    // Prompts the user to log in using the OAuth provider specified by the connection name.
    prompt(connectionName) {
        const loginPrompt = new OAuthPrompt("loginPrompt", {
            connectionName: connectionName,
            text: 'Please login',
            title: "Login",
            timeout: 30000 // User has 5 minutes to login.
        });
        return loginPrompt;
    }
}

exports.OAuthHelpers = OAuthHelpers;
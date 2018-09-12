// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { ActivityTypes, CardFactory } = require('botbuilder');
const { DialogSet, OAuthPrompt, WaterfallDialog } = require('botbuilder-dialogs');
const request = require('request');

// This bot uses OAuth to log the user in. The OAuth provider being demonstrated
// here is Azure Active Directory v2.0 (AADv2). Once logged in,the bot uses the
// Microsoft Graph API to demonstrate making calls to authenticated services.
class MainDialog {
    /**
         * Constructs the three pieces necessary by this bot to operate:
         * 1. StatePropertyAccessor
         * 2. DialogSet
         * 3. ChoicePrompt
         * 
         * The only argument taken by this constructor is a ConversationState instance, although any BotState instance would suffice for this bot. `conversationState` is used to create a StatePropertyAccessor, which is needed to create a DialogSet. All botbuilder-dialogs `Prompts` need a DialogSet to operate.
         * @param {ConversationState} conversationState 
         */
    constructor(conversationState) {
        this.data = conversationState;
        var that = this;

        // DialogState property accessor. Used to keep persist DialogState when using DialogSet.
        this.dialogState = conversationState.createProperty("dialogState");

        // Create a DialogSet that will be contain the ChoicePrompt.
        this.dialogs = new DialogSet(this.dialogState);

        var graphData = {};
        var photoResponse = {};
        var emailResponse = {};
        var helpMessage = `You can type "send <recipient_email>" to send an email, "recent" to view recent unread mail,` +
            ` "me" to see information about your, or "help" to view the commands` +
            ` again. Any other text will display your token.`;

        // Prompts the user to log in using the OAuth provider specified by the connection name.
        const authPrompt = new OAuthPrompt("loginPrompt", {
            connectionName: "AADv4Node",
            title: "Please Sign In"
        });
        this.dialogs.add(authPrompt);

        // Creates a Hero Card that is sent as a welcome message to the user.
        this.heroCard = function () {
            const heroCard = CardFactory.heroCard(
                "Welcome",
                CardFactory.images(["https://botframeworksamples.blob.core.windows.net/samples/aadlogo.png"]),
                CardFactory.actions([
                    {
                        "type": "imBack",
                        "title": "Me",
                        "value": "Me"
                    },
                    {
                        "type": "imBack",
                        "title": "Recent",
                        "value": "Recent"
                    },
                    {
                        "type": "imBack",
                        "title": "View Token",
                        "value": "View Token"
                    },
                    {
                        "type": "imBack",
                        "title": "Help",
                        "value": "Help"
                    },
                    {
                        "type": "imBack",
                        "title": "Signout",
                        "value": "Signout"
                    }
                ])
            );
            return heroCard;
        };

        // Logs in the user and calls proceeding dialogs, if login is successful.
        this.dialogs.add(new WaterfallDialog("graphDialog", [
            async function (dc, step) {
                return await dc.prompt("loginPrompt");
            },
            async function (dc, step) {
                var token = step.result;
                await that.getGraphData(token);
                await that.getUserInput(dc, token);
                return await dc.end();
            }
        ]));

        // Sends a welcome hero card.
        this.dialogs.add(new WaterfallDialog("sendWelcomeMessage", [
            async function (dc, step) {
                await dc.context.sendActivity({ attachments: [that.heroCard()] });
                return await dc.end();
            }
        ]));

        // Displays the user's collected Graph data.
        this.dialogs.add(new WaterfallDialog("listMe", [
            async function (dc) {
                var message = `You are ${graphData.displayName} and you report to ${graphData.manager}.`
                await dc.context.sendActivity(message);
                await dc.context.sendActivity({ "attachments": [{ "contentType": photoResponse.ContentType, "contentUrl": photoResponse.Base64string }] });
                return await dc.end();
            }
        ]));

        // Lists the user's collected email.
        this.dialogs.add(new WaterfallDialog("listRecentMail", [
            async function (dc) {
                var emails = emailResponse.value,
                    from = {},
                    address = {},
                    subject = {},
                    message = {},
                    email = [];
                for (var i = 0; i < 1; i++) {
                    from = emails[i].from.emailAddress.name;
                    address = emails[i].from.emailAddress.address;
                    subject = emails[i].subject;
                    message = emails[i].bodyPreview;
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
                    await dc.context.sendActivity(email);
                }
                return await dc.end();
            }
        ]));

        // Displays the user's token.
        this.dialogs.add(new WaterfallDialog("viewToken", [
            async function (dc, step) {
                var result = step.options;
                // Continue with task needing access token
                await dc.context.sendActivity(`You are logged in using token: ${result}`);
                return await dc.end();
            }
        ]));

        // Displays help text to the user.
        this.dialogs.add(new WaterfallDialog("help", [
            async function (dc) {
                await dc.context.sendActivity(helpMessage);
                return await dc.end();
            }
        ]));

        // Process for sending an email thru the bot.
        this.dialogs.add(new WaterfallDialog("sendMail", [
            async function (dc, step) {
                var email = step.options.email;
                await that.sendMail(step, email);
                await dc.context.sendActivity(`I sent a message to ${email} from your account.`);
                return await dc.end();
            }
        ]));

        // This method calls the Microsoft Graph API. The following OAuth scopes are used:
        // 'OpenId' 'email' 'Mail.Send.Shared' 'Mail.Read' 'profile' 'User.Read' 'User.ReadBasic.All'
        // for more information about scopes see:
        // https://developer.microsoft.com/en-us/graph/docs/concepts/permissions_reference
        this.getGraphData = async function (step) {
            var token = step.token

            // Collects information about the user in the bot.
            var optionsMe = {
                "url": "https://graph.microsoft.com/v1.0/me/",
                "headers": {
                    "Authorization": "Bearer " + token
                }
            };

            await request(optionsMe, function callback(error, response, body) {
                if (!error && response.statusCode == 200) {
                    var responseBody = JSON.parse(body);
                    graphData.displayName = responseBody.displayName;
                    graphData.email = responseBody.mail;
                } else {
                    console.log(error);
                }
            });

            // Collects the user's manager in the bot.
            var optionsManager = {
                "url": "https://graph.microsoft.com/v1.0/me/manager",
                "headers": {
                    "Authorization": "Bearer " + token
                }
            };

            await request(optionsManager, function callback(error, response, body) {
                if (!error && response.statusCode == 200) {
                    var responseBody = JSON.parse(body);
                    graphData.manager = responseBody.displayName;
                } else {
                    console.log(error);
                }
            });

            // Collects the user's photo in the bot.
            var optionsPhoto = {
                "url": "https://graph.microsoft.com/v1.0/me/photo/$value",
                "encoding": null, // Tells request this is a binary response
                "method": "GET",
                "headers": {
                    "Authorization": "Bearer " + token
                }
            };

            await request(optionsPhoto, function callback(error, response, body) {
                if (!error && response.statusCode == 200) {
                    // Grab the content-type header for the data URI
                    const contentType = response.headers["content-type"];

                    // Encode the raw body as a base64 string
                    const base64Body = body.toString("base64");

                    // Construct a Data URI for the image
                    const base64DataUri = "data:" + contentType + ";base64," + base64Body;

                    // Assign your values to the photoResponse object
                    photoResponse.data = [
                        {
                            "@odata.type": "#microsoft.graph.fileAttachment",
                            contentBytes: base64Body,
                            contentLocation: optionsPhoto.url,
                            isinline: true,
                            Name: "mypic.jpg"
                        }
                    ];
                    photoResponse.ContentType = contentType;
                    photoResponse.Base64string = base64DataUri;
                } else {
                    console.log(error);
                }
            });

            // Gets recent mail the user has received within the last hour and displays up
            // to 5 of the emails in the bot.
            var optionsEmail = {
                "url": "https://graph.microsoft.com/v1.0/me/messages",
                "headers": {
                    "Authorization": "Bearer " + token
                }
            };

            await request(optionsEmail, function callback(error, response, body) {
                if (!error && response.statusCode == 200) {
                    emailResponse = JSON.parse(body);
                } else {
                    console.log(error);
                }
                return (emailResponse.value);
            });
        };

        // Enable the user to send an email via the bot.
        this.sendMail = async function (step, email) {
            var result = step.options.token;
            var email = step.options.email;
            let dataEmail = JSON.stringify({
                "message": {
                    "subject": `Message from a bot!`,
                    "body": {
                        "contentType": "Text",
                        "content": `Hi there! I had this message sent from a bot. - Your friend, ${graphData.displayName}!`
                    },
                    "toRecipients": [
                        {
                            "emailAddress": {
                                "address": email
                            }
                        }
                    ]
                }
            });

            let optionsEmail = {
                "uri": "https://graph.microsoft.com/v1.0/me/sendMail",
                "method": "POST",
                "headers": {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + result
                },
                "body": dataEmail
            };

            await request(
                optionsEmail,
                function (error, response, body) {
                    console.log(body);
                    if (!error && response.statusCode == 200) {
                        console.log(`Send email successful: ${body}`);
                    }
                })
                .on("error", function (error) {
                    throw (`Server error: ${error}`);
                })
                .on("complete", function () {
                    console.log(`Send email request completed successfully`);
                });
        };

        // Waterfall dialog step to process the command sent by the user.
        this.getUserInput = async function (dc, step) {
            // We do not need to store the token in the bot. When we need the token we can
            // send another prompt. If the token is valid the user will not need to log back in.
            // The token will be available in the Result property of the task.
            var tokenResponse = step.token;
            var activity = await dc.context.activity.type;
            var text = await dc.context.activity.text;

            // If we have the token use the user is authenticated so we may use it to make API calls.
            if (tokenResponse != null) {
                if (activity === ActivityTypes.Message && !(/\d{6}/).test(text)) {
                    var parts = (dc.context.activity.text).split(" ");
                    var command = parts[0].toLowerCase();
                    if (command == "me") {
                        await dc.begin("listMe");
                        return await dc.end();
                    } else if (command == "send") {
                        step.email = parts[1].toLowerCase();
                        await dc.begin("sendMail", step);
                        return await dc.end();
                    } else if (command == "recent") {
                        await dc.begin("listRecentMail");
                        return await dc.end();
                    } else {
                        await dc.begin("viewToken", step.token);
                        return await dc.end();
                    }
                }
            } else {
                dc.context.sendActivity(`We couldn't log you in. Please try again later.`);
            }
        };

        // Processes input and route to the appropriate step.
        this.processInput = async function (turnContext) {
            var dc = await this.dialogs.createContext(turnContext);
            await dc.continue();
            switch (dc.context.activity.text.toLowerCase()) {
                case 'signout':
                case 'logout':
                case 'signoff':
                case 'logoff':
                    // The bot adapter encapsulates the authentication processes and sends
                    // activities to from the Bot Connector Service.
                    await authPrompt.signOutUser(dc.context);
                    // Let the user know they are signed out.
                    await dc.context.sendActivity('You are now signed out.');
                    await dc.cancelAll();
                    break;
                case 'help':
                    await dc.begin('help');
                    break;
                default:
                    // The user has input a command that has not been handled yet,
                    // begin the waterfall dialog to handle the input.
                    await dc.begin('graphDialog');
            }
            return dc;
        }
    };

    /**
     * This controls what happens when an activity get sent to the bot.
     * @param {Object} turnContext Provides the turnContext for the turn of the bot.
     */
    async onTurn(turnContext) {
        let dc = null;
        // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types

        switch (turnContext.activity.type) {
            case ActivityTypes.Message:
                await this.processInput(turnContext);
            case ActivityTypes.Event:
            case ActivityTypes.Invoke:
                // This handles the Microsoft Teams Invoke Activity sent when magic code is not used.
                // See: https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/authentication/auth-oauth-card#getting-started-with-oauthcard-in-teams
                // Manifest Schema Here: https://docs.microsoft.com/en-us/microsoftteams/platform/resources/schema/manifest-schema
                // It also handles the Event Activity sent from The Emulator when the magic code is not used.
                // See: https://blog.botframework.com/2018/08/28/testing-authentication-to-your-bot-using-the-bot-framework-emulator/

                // Sanity check the activity type and channel Id.
                if (turnContext.activity.type == ActivityTypes.Invoke && turnContext.activity.channelId != "msteams") {
                    throw ("The Invoke type is only valid on the MS Teams channel.");
                };

                dc = await this.dialogs.createContext(turnContext);
                await dc.continue();

                if (!turnContext.responded) {
                    await dc.begin("graphDialog");
                };
            case ActivityTypes.ConversationUpdate:
                dc = await this.dialogs.createContext(turnContext);
                await dc.continue();
                var member = turnContext.activity.membersAdded;
                for (var m in member) {
                    if (member[0].name != "Bot") {
                        // Send a HeroCard as a welcome message when a new user joins the conversation.
                        await dc.begin("sendWelcomeMessage");
                        await dc.begin("graphDialog");
                    }
                };
        }
    };
};

module.exports = MainDialog;
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// index.js is used to setup and configure your bot

const { FacebookAdapter } = require('botbuilder-adapter-facebook');

const OAUTH_ENDPOINT = 'https://api.botframework.com';
const US_GOV_OAUTH_ENDPOINT = 'https://api.botframework.azure.us';
const botframework_connector_1 = require("botframework-connector");
const botbuilder_1 = require("botbuilder");

class OAuthAdapter extends FacebookAdapter{
    constructor({appId: appId, password: password, fb_verify_token: fb_verify_token, fb_password: fb_password, fb_access_token: fb_access_token}){
        super({
            verify_token: fb_verify_token,
            app_secret: fb_password, 
            access_token: fb_access_token 
       });

       this.credentials = new botframework_connector_1.MicrosoftAppCredentials(appId, password);
       this.credentialsProvider = new botframework_connector_1.SimpleCredentialProvider(appId, password);
    }

    getUserToken(context, connectionName, magicCode) {
        return __awaiter(this, void 0, void 0, function* () {
            if (!context.activity.from || !context.activity.from.id) {
                throw new Error(`BotFrameworkAdapter.getUserToken(): missing from or from.id`);
            }
            if (!connectionName) {
                throw new Error('getUserToken() requires a connectionName but none was provided.');
            }

            const userId = context.activity.from.id;
            const client = new botframework_connector_1.TokenApiClient(this.credentials, { baseUri: OAUTH_ENDPOINT, userAgent: exports.USER_AGENT });
            const result = yield client.userToken.getToken(userId, connectionName, { code: magicCode, channelId: context.activity.channelId });
            return (!result || !result.token || result._response.status == 404) ? undefined : result;
        });
    }

    activityToFacebook(activity) {
        const message = super.activityToFacebook(activity);
        const signInUrl = this.getSignInUrl(activity);
        if (signInUrl){
            message.message.attachment = this.getSignInCard(signInUrl);
        }

        return message;
    }

    getSignInUrl(activity) {
        if (activity.attachments) {
            const attachments = activity.attachments;
            if (attachments.length > 0) {
                for(var i=0; i < attachments.length; i++){
                    const attachment = attachments[i];
                    const buttons = attachment.content.buttons;
                    if (buttons) {
                        for (var j=0; j<buttons.length;j++){
                            const button = buttons[j];
                            if (button.type === 'signin') return button.value;
                        }    
                    }
                }
            }
        }   
    }

    getSignInCard(signInUrl){
        return {
            "type": "template",
            "payload": {
              "template_type": "generic",
              "image_aspect_ratio": "horizontal",
              "elements": [
                {
                  "title": "Sign in to your account",
                  "buttons": [
                    {
                      "type": "web_url",
                      "url": signInUrl,
                      "title": "Sign in",
                      "webview_height_ratio": "full"
                    }
                  ]
                }
              ]
            }
          };
    }

    signOutUser(context, connectionName, userId) {
        return __awaiter(this, void 0, void 0, function* () {
            if (!context.activity.from || !context.activity.from.id) {
                throw new Error(`BotFrameworkAdapter.signOutUser(): missing from or from.id`);
            }
            if (!userId) {
                userId = context.activity.from.id;
            }

            const client = new botframework_connector_1.TokenApiClient(this.credentials, { baseUri: OAUTH_ENDPOINT, userAgent: exports.USER_AGENT });
            yield client.userToken.signOut(userId, { connectionName: connectionName, channelId: context.activity.channelId });
        });
    }

    getSignInResource(context, connectionName, userId, finalRedirect, appCredentials) {
        return __awaiter(this, void 0, void 0, function* () {
            if (!connectionName) {
                throw new Error('getUserToken() requires a connectionName but none was provided.');
            }
            if (!context.activity.from || !context.activity.from.id) {
                throw new Error(`BotFrameworkAdapter.getSignInResource(): missing from or from.id`);
            }
            // The provided userId doesn't match the from.id on the activity. (same for finalRedirect)
            if (userId && context.activity.from.id !== userId) {
                throw new Error('BotFrameworkAdapter.getSiginInResource(): cannot get signin resource for a user that is different from the conversation');
            }

            const client = new botframework_connector_1.TokenApiClient(this.credentials, { baseUri: OAUTH_ENDPOINT, userAgent: exports.USER_AGENT });
            const conversation = botbuilder_1.TurnContext.getConversationReference(context.activity);
            const state = {
                ConnectionName: connectionName,
                Conversation: conversation,
                relatesTo: context.activity.relatesTo,
                MSAppId: client.credentials.appId
            };
            const finalState = Buffer.from(JSON.stringify(state)).toString('base64');
            const options = { finalRedirect: finalRedirect };
            return yield (client.botSignIn.getSignInResource(finalState, options));
        });
    }
}

module.exports.OAuthAdapter = OAuthAdapter;
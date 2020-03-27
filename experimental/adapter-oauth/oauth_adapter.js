
const { FacebookAdapter } = require('botbuilder-adapter-facebook');

const OAUTH_ENDPOINT = 'https://api.botframework.com';
const US_GOV_OAUTH_ENDPOINT = 'https://api.botframework.azure.us';
const botframework_connector_1 = require("botframework-connector");
const botbuilder_1 = require("botbuilder");

class OAuthAdapter {
    constructor({onTurnError: ote, appId: appId, password: password, fb_verify_token: fb_verify_token, fb_password: fb_password, fb_access_token: fb_access_token}){
        this.adapter = new FacebookAdapter({
            verify_token: fb_verify_token,
            app_secret: fb_password, 
            access_token: fb_access_token 
       });

       this.adapter.onTurnError = ote;

       this.adapter.getUserToken = this.getUserToken.bind(this.adapter);
       this.adapter.getSignInResource = this.getSignInResource.bind(this.adapter);
       this.adapter.signOutUser = this.signOutUser.bind(this.adapter);
       this.adapter.activityToFacebook = this.activityToFacebook.bind(this.adapter);


       this.adapter.credentials = new botframework_connector_1.MicrosoftAppCredentials(appId, password);
       this.adapter.credentialsProvider = new botframework_connector_1.SimpleCredentialProvider(appId, password);

       console.log(this.adapter.credentials);
    }

    
    processActivity(req, res, logic) {
        return this.adapter.processActivity(req, res, logic);
    }


    getUserToken(context, connectionName, magicCode) {
        const oauth_adapter = this;
        return __awaiter(this, void 0, void 0, function* () {
            console.log("***********************************************");
            console.log("**************GET USER TOKEN12*******************");
            console.log("connectionName = " + connectionName);
            console.log("magicCode = " + magicCode);
            console.log(oauth_adapter.ConnectionName);
            console.log("***********************************************");
            if (!context.activity.from || !context.activity.from.id) {
                throw new Error(`BotFrameworkAdapter.getUserToken(): missing from or from.id`);
            }
            if (!connectionName) {
                throw new Error('getUserToken() requires a connectionName but none was provided.');
            }
            //oauth_adapter.checkEmulatingOAuthCards(context);
            const userId = context.activity.from.id;
            const client = new botframework_connector_1.TokenApiClient(this.credentials, { baseUri: OAUTH_ENDPOINT, userAgent: exports.USER_AGENT });
            const result = yield client.userToken.getToken(userId, connectionName, { code: magicCode, channelId: context.activity.channelId });
            if (!result || !result.token || result._response.status == 404) {
                console.log("DID NOT GET A TOKEN");
                return undefined;
            }
            else {
                return result;
            }
        });
    }


    activityToFacebook(activity) {


        var signInUrl = null;
        console.log("-----------------------------------------------456");
        console.log(activity);
        if (activity.attachments) {
            var attachments = activity.attachments;
            if (attachments.length > 0) {
                console.log("ABCD");
                for(var i=0; i < attachments.length; i++){
                    console.log("ABCD1");
                    var attachment = attachments[i];
                    console.log(">>>>>>>>>>>>>>>>>>");
                    console.log(attachment);
                    console.log("<<<<<<<<<<<<<<<<<");
                    var buttons = attachment.content.buttons;
                    if (buttons) {
                        console.log("ABCD2");
                        for (var j=0; j<buttons.length;j++){
                            var button = buttons[j];
                            console.log("------------------>1 " + button.type);
                            console.log("------------------>2 " + button.value);
                            signInUrl = button.value;
                        }
                        
                    }
                    //console.log(attachment);
                }
            }
            console.log(activity.attachments[0].content);

        }
        console.log("-----------------------------------------------");
        var card = {
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
                      //"url": "https://login.live.com/login.srf?wa=wsignin1.0&ct=1464753247&rver=6.6.6556.0&wp=MBI_SSL&wreply=https:%2F%2Foutlook.live.com%2Fowa%2F&id=292841&CBCXT=out",
                      "url": signInUrl,
                      "title": "Sign in",
                      "webview_height_ratio": "full"
                    }
                  ]
                }
              ]
            }
          };

        const message = {
            recipient: {
                id: activity.conversation.id
            },
            message: {
                text: activity.text,
                sticker_id: undefined,
                attachment: undefined,
                quick_replies: undefined
            },
            messaging_type: 'RESPONSE',
            tag: undefined,
            notification_type: undefined,
            persona_id: undefined,
            sender_action: undefined
        };

        if (signInUrl){
            message.message.attachment = card;
        }
        // map these fields to their appropriate place
        if (activity.channelData) {
            if (activity.channelData.messaging_type) {
                message.messaging_type = activity.channelData.messaging_type;
            }
            if (activity.channelData.tag) {
                message.tag = activity.channelData.tag;
            }
            if (activity.channelData.sticker_id) {
                message.message.sticker_id = activity.channelData.sticker_id;
            }
            if (activity.channelData.attachment) {
                message.message.attachment = activity.channelData.attachment;
            }
            if (activity.channelData.persona_id) {
                message.persona_id = activity.channelData.persona_id;
            }
            if (activity.channelData.notification_type) {
                message.notification_type = activity.channelData.notification_type;
            }
            if (activity.channelData.sender_action) {
                message.sender_action = activity.channelData.sender_action;
                // from docs: https://developers.facebook.com/docs/messenger-platform/reference/send-api/
                // Cannot be sent with message. Must be sent as a separate request.
                // When using sender_action, recipient should be the only other property set in the request.
                delete (message.message);
            }
            // make sure the quick reply has a type
            if (activity.channelData.quick_replies) {
                message.message.quick_replies = activity.channelData.quick_replies.map(function (item) {
                    const quick_reply = Object.assign({}, item);
                    if (!item.content_type)
                        quick_reply.content_type = 'text';
                    return quick_reply;
                });
            }
        }else{
            console.error("NO CHANNEL DATA");
        }
        
        //debug('OUT TO FACEBOOK > ', message);
        return message;
    }
    
    // checkEmulatingOAuthCards(context) {
    //     if (!this.isEmulatingOAuthCards &&
    //         context.activity.channelId === 'emulator' &&
    //         (!this.credentials.appId)) {
    //         this.isEmulatingOAuthCards = true;
    //     }
    //     console.log("isEmulatingOAuthCard " + this.isEmulatingOAuthCards );
    // }


    signOutUser(context, connectionName, userId) {
        return __awaiter(this, void 0, void 0, function* () {
            if (!context.activity.from || !context.activity.from.id) {
                throw new Error(`BotFrameworkAdapter.signOutUser(): missing from or from.id`);
            }
            if (!userId) {
                userId = context.activity.from.id;
            }
            //this.checkEmulatingOAuthCards(context);
            //const url = this.oauthApiUrl(context);
            //const client = this.createTokenApiClient(url);
            const client = new botframework_connector_1.TokenApiClient(this.credentials, { baseUri: OAUTH_ENDPOINT, userAgent: exports.USER_AGENT });
            yield client.userToken.signOut(userId, { connectionName: connectionName, channelId: context.activity.channelId });
        });
    }

    getSignInLink(context, connectionName) {
        console.log("GET SIGNIN LINK");
        return __awaiter(this, void 0, void 0, function* () {
            console.log("GET SIGNIN LINK 1234");
            let botbuilder_core_1 = botbuilder_1;
            //this.checkEmulatingOAuthCards(context);
            const conversation = botbuilder_core_1.TurnContext.getConversationReference(context.activity);
            //const url = this.oauthApiUrl(context);
            //const client = this.createTokenApiClient(url);
            const client = new botframework_connector_1.TokenApiClient(this.credentials, { baseUri: OAUTH_ENDPOINT, userAgent: exports.USER_AGENT });
            const state = {
                ConnectionName: connectionName,
                Conversation: conversation,
                MsAppId: client.credentials.appId
            };
            console.log("##################################################################");
            console.log(state);
            const finalState = Buffer.from(JSON.stringify(state)).toString('base64');
            console.log(finalState);
            return (yield client.botSignIn.getSignInUrl(finalState, { channelId: context.activity.channelId }))._response.bodyAsText;
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
            let botbuilder_core_1 = botbuilder_1;
            // const url = this.oauthApiUrl(context);
            // const credentials = appCredentials;
            // const client = this.createTokenApiClient(url, credentials);
            const client = new botframework_connector_1.TokenApiClient(this.credentials, { baseUri: OAUTH_ENDPOINT, userAgent: exports.USER_AGENT });
            const conversation = botbuilder_core_1.TurnContext.getConversationReference(context.activity);
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
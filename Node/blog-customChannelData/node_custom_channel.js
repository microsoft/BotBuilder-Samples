//loading modules
var express = require("express");
var restify = require("restify");
var botbuilder = require("botbuilder");
var request = require("request-promise");

//load environment variables here
const MSFT_APP_ID = process.env.MSFT_APP_ID;
const MSFT_APP_PW = process.env.MSFT_APP_PW;

//create an express server
var app = express();
app.listen( process.env.PORT || 3000, function(){
	console.log("Express app listening on port: " + process.env.PORT || 3000);
});

//create a chat connector for the bot
var connector = new botbuilder.ChatConnector({
    appId: MSFT_APP_ID,
    appPassword: MSFT_APP_PW
});

//load the botbuilder classes and build a unversal bot using the chat connector
var bot = new botbuilder.UniversalBot(connector);

//hook up bot endpoint
app.post("/messages/receive", connector.listen());

//root dialog
bot.dialog("/", function(session){

    console.log("-------------------------------------------------");
    console.log("Bot Received Message at '/' dialogue endpoint: ");

    //detect Facebook Messenger message here
    if(session.message.address.channelId == "facebook"){
        session.send("Facebook message recognized!");
	session.beginDialog("/send_share_button");
    } else session.send("Channel other than Facebook recognized.");

});

//where we create a facebook share button using sourceEvent
bot.dialog("/send_share_button", function(session){
    //construct a new message with the current session context
    var msg = new botbuilder.Message(session).sourceEvent({
        //specify the channel
        facebook: {
             //format according to channel's requirements
             //(in our case, the above JSON required by Facebook)
             attachment: {
                type: "template",
                payload: {
                    template_type: "generic",
                    elements: [
                        {
                            title: "Microsoft Bot Framework",
                            subtitle: "Check it out!",
                            buttons: [
                                {
                                    type: "web_url",
                                    url: "https://dev.botframework.com",
                                    title: "Go to Dev Portal"
                                },
                                {
                                    //this is our share button
                                    type: "element_share"
                                }
                            ]
                        }
                    ]
                }
            } //end of attachment
        }
    });

    //send message
    session.send(msg);
    session.endDialog("Show your friends!");
});

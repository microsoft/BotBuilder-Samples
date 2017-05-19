require('dotenv-extended').load({ path: '../.env' });

var builder = require('botbuilder');
var restify = require('restify');

var LuisActions = require('../../core');
var SampleActions = require('../all');
var LuisModelUrl = process.env.LUIS_MODEL_URL;

var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log('%s listening to %s', server.name, server.url);
});

var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});
server.post('/api/messages', connector.listen());

var bot = new builder.UniversalBot(connector);
var recognizer = new builder.LuisRecognizer(LuisModelUrl);
var intentDialog = bot.dialog('/', new builder.IntentDialog({ recognizers: [recognizer] })
    .onDefault(DefaultReplyHandler));

LuisActions.bindToBotDialog(bot, intentDialog, LuisModelUrl, SampleActions, {
    defaultReply: DefaultReplyHandler,
    fulfillReply: FulfillReplyHandler,
    onContextCreation: onContextCreationHandler
});

function DefaultReplyHandler(session) {
    session.endDialog(
        'Sorry, I did not understand "%s". Use sentences like "What is the time in Miami?", "Search for 5 stars hotels in Barcelona", "Tell me the weather in Buenos Aires", "Location of SFO airport")',
        session.message.text);
}

function FulfillReplyHandler(session, actionModel) {
    console.log('Action Binding "' + actionModel.intentName + '" completed:', actionModel);
    session.endDialog(actionModel.result.toString());
}

function onContextCreationHandler(action, actionModel, next, session) {

    // Here you can implement a callback to hydrate the actionModel as per request

    // For example:
    // If your action is related with a 'Booking' intent, then you could do something like:
    // BookingSystem.Hydrate(action) - hydrate action context already stored within some repository
    // (ex. using a booking ref that you can get from the context somehow)

    // To simply showcase the idea, here we are setting the checkin/checkout dates for 1 night
    // when the user starts a contextual intent related with the 'FindHotelsAction'

    // So if you simply write 'Change location to Madrid' the main action will have required parameters already set up
    // and you can get the user information for any purpose

    // The session object is available to read from conversationData or
    // you could identify the user if the session.message.user.id is somehow mapped to a userId in your domain

    // NOTE: Remember to call next() to continue executing the action binding's logic

    if (action.intentName === 'FindHotels') {
        if (!actionModel.parameters.Checkin) {
            actionModel.parameters.Checkin = new Date();
        }

        if (!actionModel.parameters.Checkout) {
            actionModel.parameters.Checkout = new Date();
            actionModel.parameters.Checkout.setDate(actionModel.parameters.Checkout.getDate() + 1);
        }
    }

    next();
}
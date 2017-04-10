'use strict';

var restify = require('restify');
var builder = require('botbuilder');
var server = restify.createServer();

server.listen(process.env.port || process.env.PORT || 3978, function () {
  console.log('%s listening to %s', server.name, server.url);
});

// setup bot credentials
var connector = new builder.ChatConnector({
  appId: process.env.MICROSOFT_APP_ID,
  appPassword: process.env.MICROSOFT_APP_PASSWORD
});

var bot = new builder.UniversalBot(connector);

// handle the proactive initiated dialog
bot.dialog('/survey', function (session, args, next) {
  if (session.message.text === "done") {
    session.send("Great, back to the original conversation");
    session.endDialog();
  } else {
    session.send('Hello, I\'m the survey dialog. I\'m interrupting your conversation to ask you a question. Type "done" to resume');
  }
});

// initiate a dialog proactively 
function startProactiveDialog(address) {
  bot.beginDialog(address, "*:/survey");
}

var savedAddress;
server.post('/api/messages', connector.listen());

// Do GET this endpoint to start a dialog proactively
server.get('/api/CustomWebApi', function (req, res, next) {
  startProactiveDialog(savedAddress);
  res.send('triggered');
  next();
});

// root dialog
bot.dialog('/', function (session, args) {

  savedAddress = session.message.address;

  var message = 'Hey there, I\'m going to interrupt our conversation and start a survey in a few seconds.';
  session.send(message);

  message = 'You can also make me send a message by accessing: ';
  message += 'http://localhost:' + server.address().port + '/api/CustomWebApi';
  session.send(message);

  setTimeout(() => {
    startProactiveDialog(savedAddress);
  }, 5000);
});

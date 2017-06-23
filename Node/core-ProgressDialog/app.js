/*-----------------------------------------------------------------------------
This demo shows how to create a progress dialog that will periodically notify
users of the status of a long running task.
-----------------------------------------------------------------------------*/

var restify = require('restify');
var builder = require('botbuilder');

// Setup Restify Server
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
   console.log('%s listening to %s', server.name, server.url); 
});
  
// Create chat connector for communicating with the Bot Framework Service
var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});

// Listen for messages from users 
server.post('/api/messages', connector.listen());

// Create your bot with a function to receive messages from the user.
var bot = new builder.UniversalBot(connector, [
    function (session) {
        // We can wrap a potentially long running operation with a progress dialog. This
        // will periodically notify the user that the bot is still working on their 
        // request. 
        var options = {
            initialText: 'Please wait... This may take a few moments.',
            text: 'Please wait... This is taking a little longer then expected.',
            speak: '<speak>Please wait.<break time="2s"/></speak>'
        };
        progress(session, options, function (callback) {
            // Make our async call here. If the call completes quickly then no progress
            // message will be sent.
            setTimeout(function () {
                callback('You said: ' + session.message.text);
            }, 20000);
        });
    },
    function (session, results) {
        session.send(results.response);
    }
]);

/**
 * Wrapper function to simplify calling our progress dialog.
 * @param {*} session Current session object.  
 * @param {*} options Options to control messages sent by the progress dialog. 
 *      Values:
 *          * text:         (Required) progress text to send.
 *          * speak:        (Optional) ssml to send with each progress message.
 *          * delay:        (Optional) delay (in ms) before progress is sent.
 *          * initialText:  (Optional) initial progress text.
 *          * initialSpeak: (Optional) initial progress ssml.
 *          * initialDelay: (Optional) delay before initial progress is sent.
 * @param {*} asyncFn Async function to call. Will be passed a callback with a 
 *      signature of (response: any) => void.  
 */
function progress(session, options, asyncFn) {
    session.beginDialog('progressDialog', {
        asyncFn: asyncFn,
        options: options
    })
}

bot.dialog('progressDialog', function (session, args) {
    if(!args) return;
    
    var asyncFn = args.asyncFn;
    var options = args.options;

    var count = 0;
    function sendProgress() {
        if (count++ > 0) {
            session.say(options.text, options.speak, { inputHint: builder.InputHint.ignoringInput });
        } else {
            var text = options.initialText || options.text;
            var speak = options.initialSpeak || options.speak;
            session.say(text, speak, { inputHint: builder.InputHint.ignoringInput });
        }
        hTimer = setTimeout(sendProgress, options.delay || 9000);
    }

    // Start progress timer
    var hTimer = setTimeout(sendProgress, options.initialDelay || 1000);

    // Call async function
    try {
        asyncFn(function (response) {
            // Stop timer and return response
            clearTimeout(hTimer);
            session.endDialogWithResult({ response: response });
        });
    } catch (err) {
        session.error(err);
    }
});

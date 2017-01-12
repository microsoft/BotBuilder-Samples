# Multi-Dialog Bot Sample

A sample bot showing different kind of dialogs.

[![Deploy to Azure][Deploy Button]][Deploy MultiDialogs/Node]
[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy MultiDialogs/Node]: https://azuredeploy.net

### Prerequisites

The minimum prerequisites to run this sample are:
* Latest Node.js with NPM. Download it from [here](https://nodejs.org/en/download/).
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.
* **[Recommended]** Visual Studio Code for IntelliSense and debugging, download it from [here](https://code.visualstudio.com/) for free.

### Code Highlights

With the Bot Builder SDK you'll use [dialogs](https://docs.botframework.com/en-us/node/builder/chat/dialogs/) to organize your bots conversations with the user. The bot tracks where it is in the conversation with a user using a stack that's persisted to the bots storage system. When the bot receives the first message from a user it will push the bots default dialog (`/`) onto the stack and pass that dialog the users message. The dialog can either process the incoming message and send a reply directly to the user or it can start other dialogs which will guide the user through a series of questions that collect input from the user needed to complete some task.

The session includes several methods for managing the bots dialog stack and therefore manipulate where the bot is conversationally with the user. Once you get the hang of working with the dialog stack you can use a combination of dialogs and the sessions stack manipulation methods to achieve just about any conversational flow you can dream of.

A bots dialogs can be expressed using a variety of forms:

* **Waterfall**

  Waterfalls let you collect input from a user using a sequence of steps. A bot is always in a state of providing a user with information or asking a question and then waiting for input. In the Node version of Bot Builder it's waterfalls that drive this back-n-forth flow.
* **Closure**

  You can also pass a single function for your dialog handler which simply results in creating a 1 step waterfall
* **Dialog object**

  For more specialized dialogs you can add an instance of a class that derives from the Dialog base class. This gives maximum flexibility for how your bot behaves as the built-in prompts and even waterfalls are implemented internally as dialogs.
* **SimpleDialog**

  Allows for the creation of custom dialogs that are based on a simple closure. This is useful for cases where you want a dynamic conversation flow or you have a situation that just doesn't map very well to using a waterfall.

#### Routing dialogs

Bot Builder uses dialogs to manage a bot's conversations with a user. To understand dialogs it's easiest to think of them as the equivalent of routes for a website. All bots will have at least one root ‘/' dialog just like all websites typically have at least one root ‘/' route. When the framework receives a message from the user it will be routed to this root ‘/' dialog for processing. For many bots this single root ‘/' dialog is all that's needed but just like websites often have multiple routes, bots will often have multiple dialogs.

Check out how app.js uses `bot.dialog()` to register each of the dialogs: [`flights, hotels, support` and `root`](app.js#L23-L29).
Each dialog has its own route path. We also placed each dialog handler in a separate file for maintainability.

````JavaScript
// hotels.js
module.exports = {
    Label: 'Hotels',
    Dialog: [
        // Destination
        function (session) {
            session.send('Welcome to the Hotels finder!');
            builder.Prompts.text(session, 'Please enter your destination');
        },
        function (session, results, next) {
            session.dialogData.destination = results.response;
            session.send('Looking for hotels in %s', results.response); 
            next();
        },
        ...
    ]
};

// app.js
var Hotels = require('./hotels');
bot.dialog('hotels', Hotels.Dialog);

// Root dialog
bot.dialog('/', ...);
````

#### Waterfall dialogs
Our [Hotels dialog](hotels.js) shows how to collect user information using a sequence of steps. Some steps starts with a `prompt` to ask the user for information. The following step handles the response by saving it using dialogData. Then invokes the `next` function argument to pass control to the next step.
The last step ends performing an async call to a simulated store, printing the results and ending the dialog with `session.endDialog()`. This also returns control to the parent dialog (our root dialog).

````JavaScript
[
    // Destination
    function (session) {
        session.send('Welcome to the Hotels finder!');
        builder.Prompts.text(session, 'Please enter your destination');
    },
    function (session, results, next) {
        session.dialogData.destination = results.response;
        session.send('Looking for hotels in %s', results.response); 
        next();
    },
    ...
    function (session) {
        var destination = session.dialogData.destination;
        var checkIn = new Date(session.dialogData.checkIn);
        var checkOut = checkIn.addDays(session.dialogData.nights);

        session.send(
            'Ok. Searching for Hotels in %s from %d/%d to %d/%d...',
            destination,
            checkIn.getMonth() + 1, checkIn.getDate(),
            checkOut.getMonth() + 1, checkOut.getDate());

        // Async search
        Store
            .searchHotels(destination, checkIn, checkOut)
            .then((hotels) => {
                // Results
                session.send('I found in total %d hotels for your dates:', hotels.length);

                var message = new builder.Message()
                    .attachmentLayout(builder.AttachmentLayout.carousel)
                    .attachments(hotels.map(hotelAsAttachment));

                session.send(message);

                // End
                session.endDialog();
            });
    }
]
````

#### Calling new dialogs

New dialogs can be triggered using [`session.beginDialog(id)`](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.session.html#begindialog). The new dialog is added to the top of the stack and control is transfered to it. In [app.js](app.js#L65-L72) we show how to trigger a dialog based on a user selection.
Control is returned from the child dialog back to the caller (popping of the stack) using [`session.endDialog`](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.session.html#enddialog) or [`session.endDialogWithResult`](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.session.html#enddialogwithresult).

#### IntentDialog for listening to specific keywords

Our root dialog handler is created using an [IntentDialog](https://docs.botframework.com/en-us/node/builder/chat/IntentDialog/) to handle any message including specific keywords. When a match is found the Support dialog is initiated using `session.beginDialog('support')`. Check out [app.js root dialog handler](app.js#L30-L33) to see its usage.
The IntentDialog also has an [onDefault](https://docs.botframework.com/en-us/node/builder/chat/IntentDialog/#onbegin--ondefault-handlers) handler that will be notified if it fails to match one of the registered keywords. [In our sample](app.js#L40-L51) we are using it to request a `Prompts.choice` so the user can decide how to continue.

````JavaScript
bot.dialog('/', new builder.IntentDialog()
    .matchesAny([/help/i, /support/i, /problem/i], [
        function (session) {
            session.beginDialog('support');
        },
        function (session, result) { ... }
    ])
    .onDefault([
        function (session) {
            // prompt for search option
            builder.Prompts.choice(
                session,
                'Are you looking for a flight or a hotel?',
                [Flights.Label, Hotels.Label],
                {
                    maxRetries: 3,
                    retryPrompt: 'Not a valid option'
                });
        }
    ...]);
````

#### Returning a result to the parent dialog

In our [Support dialog](support.js#L3-L12) we are generating a support ticket and the returning control to the root dialog using `session.endDialogWithResult(result)` and passing the ticket number as a `result` object.
Control is returned to the [second step](app.js#L34-38) of the waterfall defined when creating our `IntentDialog`. In it we print the ticket number and call `session.endDialog()` to end the dialog.

#### Handling dialog errors

Errors can be signaled from child dialogs using [session.error](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.session.html#error). This will signal the error via an on('error', err) event, for both the bot object and the session object. In [flights.js](flights.js#L4) we are signalling an error which the parent dialog handles using session.on('error'), as shown in [app.js](app.js#L59-L63):

````JavaScript
session.on('error', function (err) {
    session.send('Failed with message: %s', err.message);
    session.endDialog();
});
````

#### Asynchronous operations

Inevitably you're going to want to make some asynchronous network call to retrieve data and then send those results to the user using the session object. This is completely fine but there are a few best practices you'll want to follow.

* Use session.send within your callback or event handler.
* Do not call session.endDialog() immediately after starting the asynchronous call;
* Instead call session.endDialog() from within your callback or event handler. 

Check out [hotels.js](hotels.js#L48-L63) where we are calling an asynchronous method that returns a Promise. Our fulfill method calls `session.send()` and is also responsible for calling `session.endDialog()`.

````JavaScript
// Async search
Store
    .searchHotels(destination, checkIn, checkOut)
    .then((hotels) => {
        // Results
        session.send('I found in total %d hotels for your dates:', hotels.length);

        var message = new builder.Message()
            .attachmentLayout(builder.AttachmentLayout.carousel)
            .attachments(hotels.map(hotelAsAttachment));

        session.send(message);

        // End
        session.endDialog();
    });
````

### Outcome

You will see the following result in the Bot Framework Emulator when opening and running the sample solution.

![Sample Outcome](images/outcome-emulator.png)

You will see the following in your Facebook Messenger.

![Sample Outcome](images/outcome-facebook.png)

On the other hand, you will see the following in Skype.

![Sample Outcome](images/outcome-skype.png)

### More Information

To get more information about how to get started in Bot Builder for Node and Dialogs please review the following resources:
* [Bot Builder for Node.js Reference](https://docs.botframework.com/en-us/node/builder/overview/#navtitle)
* [Dialogs](https://docs.botframework.com/en-us/node/builder/chat/dialogs/)
* [Dialog Stack](https://docs.botframework.com/en-us/node/builder/chat/session/#dialog-stack)
* [Prompts](https://docs.botframework.com/en-us/node/builder/chat/prompts/)
* [IntentDialog](https://docs.botframework.com/en-us/node/builder/chat/IntentDialog/)
* [session.beginDialog](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.session.html#begindialog)
* [session.endDialog](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.session.html#enddialog)
* [session.endDialogWithResult](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.session.html#enddialogwithresult)
* [Using Session in Callbacks](https://docs.botframework.com/en-us/node/builder/chat/session/#using-session-in-callbacks)

> **Limitations**  
> The functionality provided by the Bot Framework Activity can be used across many channels. Moreover, some special channel features can be unleashed using the [Message.sourceEvent](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.message.html#sourceevent) method.
> 
> The Bot Framework does its best to support the reuse of your Bot in as many channels as you want. However, due to the very nature of some of these channels, some features are not fully portable.
> 
> The features used in this sample are fully supported in the following channels:
> - Skype
> - Facebook
> - Microsoft Teams
> - Slack
> - DirectLine
> - WebChat
> - Email
> - GroupMe
> - Kik
> - Telegram
> 
> On the other hand, they are not supported and the sample won't work as expected in the following channel:
> - SMS
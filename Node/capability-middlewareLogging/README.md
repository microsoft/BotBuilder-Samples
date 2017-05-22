## Middleware and Logging with BotBuilder Node SDK

This is a basic bot that logs incoming and outgoing messages to the console. If you are looking to add logging to your bot, replace `console.log` with a function that logs messages to your location of choice. There is also a regex match for the word ‘secret’ on incoming messages. This model is how you can build middleware that never causes the bot code to execute.

[![Deploy to Azure][Deploy Button]][Deploy Node/MiddlewareLogging]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy Node/MiddlewareLogging]: https://azuredeploy.net

### Adding Middleware Using the BotBuilder Node SDK 
In the Node SDK, middleware is installed using [UniversalBot.use](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.universalbot.html#use). There are three possible hooks: receive, botbuilder and send. The team that build this SDK created the receive hook to be used for middleware that works across multiple toolkits (i.e., BotBuilder & BotKit). If this is your specific use case, be aware that if you include middleware in botbuilder, receive will execute first, followed by botbuilder. Because this example (and most bots) do not fit this category, we will be using the botbuilder hook for incoming messages.

#### Simplest Code Example Incoming messages (user -> bot)
Capturing User Input:
 ```javascript
     botbuilder: function (session, next) {
        console.log(session.message.text);
        next();
    }
```

The botbuilder hook on the middleware is an example of [ISessionMiddleware](https://docs.botframework.com/en-us/node/builder/chat-reference/interfaces/_botbuilder_d_.isessionmiddleware.html). The main advantage of using this hook instead of receive is the access we gain to the [session](https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.session.html#sessionstate). This will execute once a message is bound to a particular session and gives the option of looking at a message and the state of the session (where user is in available dialogs, etc) then making a decision of how to proceed. In this example, the default bot behavior is suppressed a message is sent to the user, all in middleware based on user input.

The second parameter `next` is a function called to advance the middleware. Calling `next` in the last step of middleware causes the bot code to execute. If you want to suppress the functionality of the bot, return instead of calling `next`.

#### Outgoing messages (bot -> user)
Capturing bot output to a user:
```javascript
    send: function (event, next) {
        console.log(event.text);
        next();
    }
```

Both the send and receive hooks use [IEventMiddleware](https://docs.botframework.com/en-us/node/builder/chat-reference/interfaces/_botbuilder_d_.ieventmiddleware.html). 
The first argument is the event itself. To see whether an event is a message, check to see if event.type is 'message'.

The second parameter `next` is the same function called in the botbuilder hook.


Misc:
- Uncaught errors in middleware code will often cause a silent failure and execution of code falls through to the bot, even without calling `next`.

### More Information

To get more information about how to get started in Bot Builder for Node and Middleware please review the following resources:
* [Bot Builder for Node.js Reference](https://docs.microsoft.com/en-us/bot-framework/nodejs/)
* [Intercept messages](https://docs.microsoft.com/en-us/bot-framework/nodejs/bot-builder-nodejs-intercept-messages)
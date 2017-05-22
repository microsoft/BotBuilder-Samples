# Managing conversations and global dialogs

Communication with a user via a bot built with [Microsoft Bot Framework](https://dev.botframework.com) is managed via conversations, dialogs, waterfalls, and steps. As the user interacts with the bot, the bot will start, stop, and switch between various dialogs in response to the messages the user sends. Knowing how to manage dialogs in Bot Framework is one of the keys to successfully designing and creating a bot.

[![Deploy to Azure][Deploy Button]][Deploy Node/GlobalMessageHandlers]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy Node/GlobalMessageHandlers]: https://azuredeploy.net

## Dialogs and conversations, defined

At its most basic level, a **dialog** is a reusable module, a collection of methods, which performs an operation, such as completing an action on the user's behalf, or collecting information from the user. By creating dialogs you can add reuse to your bot, enable better communication with the user, and simplify what would otherwise be complex logic. Dialogs also contain state specific to the dialog in **dialogData**.

A **conversation** is a parent to dialogs, and contains the dialog stack. It also maintains two types of state, **conversationData**, shared between all users in the conversation, and **privateConversationData**, which is state data specific to that user.

### Waterfalls

Every dialog you create will have a collection of one or more methods that will be executed in a waterfall pattern. As each method completes, the next one in the waterfall will be executed.

### Dialog Stack

Your bot will maintain a stack of dialogs. The stack works just like a normal [LIFO stack](https://en.wikipedia.org/wiki/Stack_(abstract_data_type)), meaning the last dialog added will be the first one completed, and when a dialog completes control will then return to the previous dialog.

## Managing dialogs

Bots come in many shapes, sizes, and forms. Some bots are simply front ends to existing APIs, and respond to simple commands. Others are more complex, with back and forth messages between the user and bot, branching based on information collected from the user and the current state of the application. Depending on the requirements for the bot you're building, you'll need various tools at your disposal to start and stop dialogs.

### Starting dialogs

Dialogs can be started in a few ways. Every bot has a *default*, sometimes called a *root* dialog, which is executed when no other dialog has been started, and no other ones have been triggered via other means. You can create a dialog that responds globally to certain commands by using `triggerAction` or `beginDialogAction`. `triggerAction` is registered globally to the bot, while `beginDialogAction` registers the command to just that dialog. Finally, you can programmatically start a dialog by calling either `beginDialog` or `replaceDialog`, which will allow you to add a dialog to the stack or replace the current dialog, respectively.

### Ending dialogs and conversations

When a bot reaches the end of a waterfall, the next message will look for the next step in the waterfall. If there is no step, the bot simply doesn't respond, naturally ending the conversation or dialog. This can provide a bit of a confusing experience for the user, as they may need to retype their message to get a response from the bot. It can also be confusing for the developer, as there may be many ways a dialog might end depending on the logic.

As a result, when a conversation or dialog has come to an end, it's a best practice to explicitly call `endConversation`, `endDialog`, or `endDialogWithResult`. `endConversation` both clears the current dialog stack and resets all data stored in the session, **except** `userData`. Both `endDialog` and `endDialogWithResult` end the dialog, clear out `dialogData`, and control to previous dialog in the stack. Unlike `endDialog`, `endDialogWithResult` allows you to pass arguments into the previous dialog, which will be available in the second parameter of the first method in the waterfall (typically named `results`).

#### State management

Ending a conversation or dialog will also remove the associated state data. This is important to remember when deciding where to store state data. The best practices of minimizing scope of state data apply to bots, just as they do to any other application.

The place where state lifespan becomes trickiest is `dialogData`. If you start a new dialog, the dialog doesn't receive the data from the calling dialog. In addition, when a dialog completes, the previous dialog doesn't receive the data from the calling dialog. You can overcome this by using arguments. `endDialogWithResult` allows you to pass arguments to the prior dialog, while both `beginDialog` and `replaceDialog` allow you to pass arguments into the new dialog.

## The sample application

The sample application we will be building through the next set of examples is a simple calculator bot. Our calculator bot will allow the user to enter numbers, and once they say **total** we'll display the total and allow them to start all over again. We'll also want to allow the user to get help at any time, and to cancel as needed. The [sample code](https://github.com/GeekTrainer/dialog-sample) is provided on GitHub.

## Default dialog

Starting with version 3.5 of Microsoft Bot Framework, the *default* or *root* dialog is registered as the second parameter in the constructor for `UniversalBot`. In prior versions, this was done by adding a dialog named `/`, which led to naming similar to that of URLs, which really isn't appropriate when naming dialogs.

The default dialog is executed whenever the dialog stack is empty, and no other dialog is *triggered* via LUIS or another recognizer. (We'll see how to register dialogs using `triggerAction` a little later.) As a result, the default dialog should provide some contextual information to the user, such as a list of available commands and an overview of what the bot can perform.

From a design perspective, don't be afraid to send buttons to the user to help guide them through the experience; bots don't need to be text only. Buttons are a wonderful interface, as they can make it very clear what options the user can choose from, and limit the possibility of the user making a mistake.

To get started, we'll set up our default dialog to present the user with two buttons, **add** and **help**. For our first pass, we'll simply echo the user's selection; we'll add additional dialogs in the next section. We'll do this by setting up a two step waterfall, where the first step will prompt the user, and the second will end the conversation.

### Default dialog sample code

``` javascript
const builder = require('botbuilder');

const connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});

const bot = new builder.UniversalBot(connector, [
    (session, args, next) => {
        const card = new builder.ThumbnailCard(session);
        card.buttons([
            new builder.CardAction(session).title('Add a number').value('Add').type('imBack'),
            new builder.CardAction(session).title('Get help').value('Help').type('imBack'),
        ]).text(`What would you like to do?`);

        const message = new builder.Message(session);
        message.addAttachment(card);

        session.send(`Hi there! I'm the calculator bot! I can add numbers for you.`);
        const choices = ['Add', 'Help'];
        builder.Prompts.choice(session, message, choices);
    },
    (session, results, next) => {
        session.endConversation(`You chose ${results.response.entity}`);
    },
]);
```

[Sample code](https://github.com/GeekTrainer/dialog-sample/blob/master/sample-defaultDialog.js)

## Working with dialogs

One of the biggest challenges when creating a bot is dealing with the fact users can be random. Imagine the following exchange:

``` console
User: I'd like to make a reservation
Bot: Sure! How many people?
User: Do you have a vegan menu?
Bot: ???
```

This is a common scenario. The user sends a message to the bot. The bot responds. The user gets a new piece of information, in this case their friend is a vegan, and thus asks about a vegan menu. The bot is now stuck, because it wasn't expecting that response. `triggerAction` allows you to register a global command of sorts with the bot, and ensure the appropriate dialog is executed for every request.

### Naming dialogs

In prior versions of Bot Framework, developers typically started every dialog name with **/**. This was because when registering the default dialog in earlier versions you named it **/**. As you've already seen, that's not the case starting with version 3.5. As a result, you give your dialog a name that appropriately describes the operation the dialog is built to perform.

### Registering a dialog

`bot.dialog` is used to register a dialog. The two parameters you'll provide are the name of the dialog, and the array of methods you wish to execute when the user enters the dialog. Let's create the starter for **add** dialog. For now, we'll leave it with the simple echo, and introduce new functionality as we go forward.

### Dialog sample code

``` javascript
bot.dialog('AddNumber', [
    (session, args, next) => {
        session.endConversation(`This is the AddNumber dialog`);
    },
]);
```

## Using triggerAction to start a dialog

We want to register our **AddNumber** dialog with the bot so whenever the user types *add* this dialog will be executed. This is done through the use of `triggerAction`, which is a method available on `Dialog`. `triggerAction` accepts a parameter of type `ITriggerActionOptions`.

`ITriggerActionOptions` has a few properties, the most important of which is `matches`. Matches will either be a regular expression to match a string typed in by the user, such as *add* in our case, or a string literal if the match will be done through the use of a recognizer, such as one from LUIS.

Let's update our bot to register **AddNumber** to be started when the user types **add**. We'll remove the second step from the default dialog and take advantage of the behavior of our buttons, which will send the text of the button to the bot, much in the same way as if the user typed it themselves.

### triggerAction sample code

``` javascript
// just the updated code
const bot = new builder.UniversalBot(connector, [
    (session, args, next) => {
        const card = new builder.ThumbnailCard(session);
        card.buttons([
            new builder.CardAction(session).title('Add a number').value('Add').type('imBack'),
            new builder.CardAction(session).title('Get help').value('Help').type('imBack'),
        ]).text(`What would you like to do?`);

        const message = new builder.Message(session);
        message.addAttachment(card);

        session.send(`Hi there! I'm the calculator bot! I can add numbers for you.`);
        // we can end the conversation here
        // the buttons will provide the appropriate message
        session.endConversation(message);
    },
]);

bot.dialog('AddNumber', [
    (session, args, next) => {
        session.endConversation(`This is the AddNumber dialog`);
    },
]).triggerAction({matches: /^add$/i});
```

[Sample code](https://github.com/GeekTrainer/dialog-sample/blob/master/sample-triggerAction.js)

### triggerAction notes

`triggerAction` is a global registration of the command for the bot. If you wish to limit that to an individual dialog, use `beginDialogAction`, which we'll discuss later.

Also, `triggerAction` **replaces** the entire current dialog stack with the new dialog. While that can be good for **AddNumber**, that wouldn't be good for a dialog to provide help. We'll see a little later how `onSelectAction` can be used to manage this behavior.

If you execute the bot at this point you'll notice clicking **Add** on the buttons, or simply typing it, will cause the bot to send the message **This is the AddNumber dialog**. You'll also notice that **help**, at present, does nothing. We'll handle that in a bit.

## Using replaceDialog to replace the current dialog

Let's talk a little bit about our logic for **AddNumber**. We want to prompt the user for a number, add it to our running total, and then ask the user for the next number. Basically, we just need to restart the same dialog over and over again. We can use `replaceDialog` to perform this action.

In the first step of our waterfall, we'll check to see if there is a running total available in `privateConversationData`, and create one if it doesn't exist. We'll then prompt the user for the number they want to add.

In the second step, we'll retrieve the number, add it to our running total, and then start the dialog over again by calling `replaceDialog`.

### replaceDialog sample code

``` javascript
bot.dialog('AddNumber', [
    (session, args, next) => {
        let message = null;
        if(!session.privateConversationData.runningTotal) {
            message = `Give me the first number.`;
            session.privateConversationData.runningTotal = 0;
        } else {
            message = `Give me the next number, or say **total** to display the total.`;
        }
        builder.Prompts.number(session, message, {maxRetries: 3});
    },
    (session, results, next) => {
        if(results.response) {
            session.privateConversationData.runningTotal += results.response;
            session.replaceDialog('AddNumber');
        } else {
            session.endConversation(`Sorry, I don't understand. Let's start over.`);
        }
    },
]).triggerAction({matches: /^add$/i});
```

[Sample code](https://github.com/GeekTrainer/dialog-sample/blob/master/sample-replaceDialog.js)

### replaceDialog notes

`replaceDialog` takes two parameters, the first being the name of the dialog with which you wish to replace the current dialog, and the second being the arguments for the new dialog. The object you provide as the second parameter will be available in the first function in  the new dialog's waterfall in the second parameter (typically named `args`).

## Using beginDialogAction to localize commands

It doesn't make a lot of sense for our bot to have a global **total** command. After all, it's only valid if we're currently adding numbers. Using `beginDialogAction` allows you to register commands specific to that dialog, rather than global to the bot. By using `beginDialogAction`, we can ensure **total** is only executed when we're in the process of running a total.

The syntax for `beginDialogAction` is similar to `triggerAction`. You provide the name of the `DialogAction` you're creating, the name of the `Dialog` you wish to start, and the parameters for controlling when the dialog will be started.

### beginDialogAction sample code

``` javascript
bot.dialog('AddNumber', [
    // existing waterfall code
])
.triggerAction({matches: /^add$/i})
.beginDialogAction('Total', 'Total', { matches: /^total$/});

bot.dialog('Total', [
    (session, results, next) => {
        session.endConversation(`The total is ${session.privateConversationData.runningTotal}`);
    },
]);
```

[Sample code](https://github.com/GeekTrainer/dialog-sample/blob/master/sample-beginDialogAction.js)

### beginDialogAction notes

By using `endConversation`, we reset the entire conversation back to its starting state. This will automatically clear out any `privateConversationData`, as the conversation has ended.

## Using onSelectAction to control triggerAction behavior

By default, `triggerAction` will reset the current dialog stack with the new dialog. In the case of **AddNumber** that's just fine; the logic on the dialog is designed for the dialog to continually restart. But this is problematic when it comes to **Help**. Needless to say, we don't want to reset the entire set of dialogs when the user types **help**; we want to allow the user to pick up right where they left off.

Bot Framework provides `beginDialog` for adding a dialog to the stack. When that dialog completes, it returns to the control to the active step in the prior dialog. Or, in terms of the case of our **Help** example, it will allow the user to pick up where they left off.

The `onSelectAction` property on `ITriggerActionOptions` executes when the bot is about to start the dialog being triggered. By using this event, we can change the way the dialog is started, using `beginDialog`, which will add the dialog to the stack instead of replacing stack. The first parameter is the name of the dialog we wish to start, which is provided in `args.action`, and the second is the `args` parameter we want to pass into the the dialog when it starts. The code sample below will ensure we return control to the prior dialog when this one completes.

### onSelectAction sample code

``` javascript
bot.dialog('Help', [
    (session, args, next) => {
        session.endDialog(`You can type **add** to add numbers.`);
    }
]).triggerAction({
    matches: /^help/i,
    onSelectAction: (session, args) => {
        session.beginDialog(args.action, args);
    }
});
```

[Sample code](https://github.com/GeekTrainer/dialog-sample/blob/master/sample-onSelectAction.js)

### onSelectAction notes

When using `beginDialog`, don't hard code the name of the dialog you're about to start, but rather use `args.action`. Otherwise, you'll notice the dialog won't actually start.

## Using beginDialogAction to centralize help messaging

One of the challenges with the help solution we created earlier is it can only provide generic help; whenever the user types **help** the exact same message is sent to the user. By using **beginDialogAction** you can parameters to the triggered dialog, allowing you to centralize messaging for help. In our case, we'll use the name of the current action as the key to the message we want to send.

### beginDialogAction to centralize help sample code

``` javascript
bot.dialog('AddNumber', [
    // existing waterfall code snipped
])
.triggerAction({matches: /^add$/i})
.beginDialogAction('Total', 'Total', { matches: /^total$/})
.beginDialogAction('HelpAddNumber', 'Help', { matches: /^help$/, dialogArgs: {action: 'AddNumber'} });

bot.dialog('Total', [
    (session, results, next) => {
        session.endConversation(`The total is ${session.privateConversationData.runningTotal}`);
    },
]);

bot.dialog('Help', [
    (session, args, next) => {
        let message = '';
        switch(args.action) {
            case 'AddNumber':
                message = 'You can either type the next number, or use **total** to get the total.';
                break;
            default:
                message = 'You can type **add** to add numbers.';
                break;
        }
        session.endDialog(message);
    }
]).triggerAction({
    matches: /^help/i,
    onSelectAction: (session, args) => {
        session.beginDialog(args.action, args);
    }
});
```

[Sample code](https://github.com/GeekTrainer/dialog-sample/blob/master/sample-beginDialogAction-help.js)

## Using cancelAction to add a local cancel operation

If you've made it to this point in the article, you already have the skills necessary to create a global cancel operation - you'd add a new dialog, register it with `triggerAction`, and add a string match for the word **cancel**. The dialog would then call `endConversation` with a friendly message, and the user would be able to restart he operation.

However, let's say you wanted to provide granular support for cancel operations, changing the behavior on different dialogs, or maybe not allowing a cancel on a dialog at all. This is where `cancelAction` comes into place. `cancelAction` allows you to register a cancel command for a specific dialog. In addition, `cancelAction` only cancels the current dialog, **not** the entire conversation. This gives you a bit more control over how the cancel operation will behave.

The second parameter you'll pass into cancelAction is `ICancelActionOptions`, which includes the `matches` and `onSelectAction` properties we've seen before. It also adds `confirmPrompt`, which, if set, will prompt the user if they actually want to cancel.

### cancelAction sample code

``` javascript
bot.dialog('AddNumber', [
    // prior code for AddNumber snipped for clarity
])
.cancelAction('CancelAddNumber', 'Operation cancelled', {
    matches: /^cancel$/,
    onSelectAction: (session, args) => {
        session.endConversation(`Operation cancelled.`);
    },
    confirmPrompt: `Are you sure you wish to cancel?`
})
```

[Sample code](https://github.com/GeekTrainer/dialog-sample/blob/master/sample-cancelAction.js)

### cancelAction notes

By using `onSelectAction` you're able to end the entire conversation, resetting everything; the default behavior is to just cancel the current dialog.

## Conclusion

Bot Framework offers many options and methods for managing dialogs and responding to user requests. Harnessing the the power provided by dialogs allows you to create bots that can have conversations with your users that feel more natural.
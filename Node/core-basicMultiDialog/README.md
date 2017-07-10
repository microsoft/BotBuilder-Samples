# Bot Framework Dialogs

A conversation inside of a bot can contain one or more dialogs. A dialog is designed to be a self-contained set of interactions to collect information from a user, or perform an action on her behalf. The bot maintains a stack of dialogs, meaning that as each one completes, it's popped off the stack, and control returns to the one prior. You can add dialogs to the stack by calling `session.beginDialog` or `session.replaceDialog`, and ended by using `session.endDialog` or `session.endDialogWithResult`.

[![Deploy to Azure][Deploy Button]][Deploy Node/BasicMultiDialog]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy Node/BasicMultiDialog]: https://azuredeploy.net

## Why create dialogs?

Dialogs allow you to make your bot modular. Having multiple dialogs allows you to create smaller black boxes that can perform individual actions, much in the same way you'd create methods. In addition, because of the waterfall nature of bot logic, having dialogs allows you to perform validation, and restart an operation if needed. Finally, having multiple dialogs affords you the opportunity for reuse, which, of course, is the goal of all software development.

## Our sample bot

The sample bot contains three dialogs:

- Root dialog, which acts as a "controller"
- getName, which is designed to collect a name from the user
- getAge, which is designed to collect the age of the user

## Adding a dialog to a bot

You add a dialog to a bot, and thus make it available for calling, by using `bot.dialog`, and provide a name.

``` javascript
// add a dialog named getName
bot.dialog('getName', [
    (session, args, next) => {
        // normal bot code here
    }
]);
```

## Calling the dialog

Bot Framework offers two ways to call a dialog, `beginDialog` and `replaceDialog`. The difference between the two methods is if you need control to pass to the calling dialog. If you use `beginDialog`, control will be passed to the next step in the prior or calling dialog. If you use `replaceDialog`, the current dialog is replaced on the stack by the new dialog, and control will return to the one prior.

### The dialog stack explained in an example

Imagine the following code:

``` javascript

const bot = new builder.UniversalBot(connector, [
    // sets the default or root dialog
    (session, args, next) => {
        session.beginDialog('first');
    },
    (session, results, next) => {
        // this will be executed when the new dialog on the stack completes
    }
]);

bot.dialog('first', [
    (session, args, next) => {
        session.replaceDialog('second');
    },
    (session, results, next) => {
        // this will never get called
    }
]);

bot.dialog('second', [
    (session, args, next) => {
        session.endDialog('Control will return to the root');
    }
]);
```

#### Walkthrough

When the bot starts, it calls dialog `first`. Our stack now contains two dialogs, `root` and `first`.

When `first` runs, it calls `replaceDialog`, which will replace `first` on our stack with `second`. Our stack now contains `root` and `second`.

When `second` runs, it calls `endDialog`, which removes it from the stack. The only dialog left on our stack is `root`. The next block of code that would be run is the second step in the root dialog.

### Passing arguments

Dialogs are designed to be independent black boxes. As a result, you should avoid using `privateConversationData` state for storing or passing values. You should, instead, pass arguments by using the second parameter of `beginDialog` or `replaceDialog`. The values you provide will be passed into the dialog.

In the [bot.js](./bot.js) example, we call `session.beginDialog('getAge', { name: name })`. The `args` parameter will contain the object we pass in.

## Ending dialogs

You can end a dialog with either `endDialog` or `endDialogWithResult`. Both methods will end the dialog, and in each case execution will resume with the next step in the prior dialog on the stack.

If you merely need to pass a message to the user, call `endDialog`, which accepts a single parameter of a message to pass to the user.

If you need to pass a value back to the calling dialog, use `endDialogWithResult`, which will allow you to pass an object to the next step in the prior dialog. Typically, this will be an object with a single property of `response`, which is the same property used by `Prompts` in Bot Framework. This is the behavior demonstrated by both `getName` and `getAge`, as they are dialogs designed to retrieve a piece of information from the user.

## In review

Dialogs are a powerful unit of work in Bot Framework. They offer you as a developer the ability to create black boxes that can be reused as needed, or abstract away complex code. You will find that even simple bots will contain multiple dialogs.
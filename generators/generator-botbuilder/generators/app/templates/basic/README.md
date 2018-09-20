# <%= botName %> Bot

This bot has been created using [Microsoft Bot Framework](https://dev.botframework.com),

This bot is designed to do the following:

<%= description %>

## About the generator

The goal of the BotBuilder Yeoman generator is to both scaffold out a bot according to general best practices, and to provide some templates you can use when implementing commonly requested features and dialogs in your bot. As a result, you will notice that all dialogs are located in a folder called `dialogs`, and the one you chose when running the wizard becomes the default (or root) dialog. You are free to use the additional dialogs provided (or delete them) as you see fit.

One thing to note is it's not possible to completely generate a bot or dialog, as the questions you need to ask of your user will vary wildly depending on your scenario. As such, we hope we've given you a good starting point for building your bots with Bot Framework.

### Dialogs

This generator provides the following dialogs:
- Echo Dialog, for simple bots

Each class has three properties to help simplify addition to an existing bot:
- id: Used for the id
- waterfall: The logic (or waterfall) for the dialog
- name: The intent name for the dialog for triggering

You can add a dialog to a bot with the following code:

``` javascript
// declare your dialog

bot.dialog(dialog.id, dialog.waterfall).triggerAction({ matches: dialog.name });
```

By using this structure, it would be possible to dynamically load all of the dialogs in the `dialogs` folder, and then add them to the bot.

## Getting Started

### Dependencies

- **[Restify](http://restify.com)** Used to host the web service for the bot, and for making REST calls
- **[dotenv](https://github.com/motdotla/dotenv)** Used to manage environmental variables

### Structure

`index.<%= extension %>` references the bot and starts a Restify server. `bot.<%= extension %>` loads the dialog type you selected when running the generator and adds it as the default dialog. `dialogs.<%= extension %>` contains the list of sample dialogs.

### Configuring the bot

Update `.env` with the appropriate keys:

- App ID and Key for registered bots.

### Running the bot

```
<%= runCmd %>
```
### Developing the bot

```
<%= watchCmd %>
```

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator)

### Connect to bot using Bot Framework Emulator v4
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to `javascript_nodejs/13.basic-bot` folder
- Select `basic-bot.bot` file


## Further Reading
- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot basics](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [LUIS](https://luis.ai)
- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [QnA Maker](https://qnamaker.ai)


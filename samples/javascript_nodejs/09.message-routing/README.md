# message-routing bot

This bot has been created using [Microsoft Bot Framework](https://dev.botframework.com),

This samples shows how to:
- Handle user interruptions for such things as Help or Cancel
- Prompt for and validate requests for information from the user

## To try this sample
- Clone the repository
  ```bash
  git clone https://github.com/microsoft/botbuilder-samples.git
  ```
- In a terminal, navigate to `javascript_nodejs/09.message-routing`
  ```bash
  cd javascript_nodejs/09.message-routing
  ```
  - [Optional] Update the `.env` file under `samples/javascript_nodejs/09.message-routing` with your `botFileSecret`
    For Azure Bot Service bots, you can find the `botFileSecret` under application settings.
- Install modules
  ```bash
  npm install
  ```
- Create [required services](./deploymentScripts/DEPLOYMENT.MD)
- Run the sample
  ```bash
  npm start
  ```

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator)

### Connect to bot using Bot Framework Emulator v4
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to `javascript_nodejs/09.message-routing` folder
- Select `message-routing.bot` file

## Further Reading
- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot basics](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [LUIS](https://luis.ai)
- [Prompt Types](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0&tabs=javascript)
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [QnA Maker](https://qnamaker.ai)


## Additional Resources

### Dependencies

- **[Restify](http://restify.com)** Used to host the web service for the bot, and for making REST calls
- **[dotenv](https://github.com/motdotla/dotenv)** Used to manage environmental variables

### Project Structure

`index.js` references the bot and starts a Restify server. `bot.js` loads the dialog type you selected when running the generator and adds it as the default dialog. `dialogs.js` contains the list of sample dialogs.

### Configuring the bot

Update `.env` with the appropriate keys:

- App ID and Key for registered bots.

### Running the bot

```
node ./index.js
```
### Developing the bot

```
nodemon ./index.js
```




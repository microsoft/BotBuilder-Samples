# basic-bot

This bot has been created using [Microsoft Bot Framework](https://dev.botframework.com),

This sample uses the bot authentication capabilities of Azure Bot Service. In this sample we are assuming the OAuth 2 provider
is Azure Active Directory v2 (AADv2) and are utilizing the Microsoft Graph API to retrieve data about the
user. [Check here](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-authentication?view=azure-bot-service-4.0) for information about getting an AADv2
application setup for use in Azure Bot Service.
The [scopes](https://developer.microsoft.com/en-us/graph/docs/concepts/permissions_reference) used in this sample are
'OpenId' 'email' 'Mail.Send.Shared' 'Mail.Read' 'profile' 'User.Read' 'User.ReadBasic.All'.


## To try this sample
- Clone the repository
  ```bash
  git clone https://github.com/microsoft/botbuilder-samples.git
  ```
- In a terminal, navigate to javascript_nodejs/84.bot-authentication-msgraph
  ```bash
  cd javascript_nodejs/84.bot-authentication-msgraph
  ```
- Install modules
  ```bash
  npm install
  ```
- Update `basic-bot.bot` with required configuration settings
  - App ID and Key for registered bots
- Train LUIS to use the `greeting.lu` training set
  ```bash
  npm run train-luis
  ```
- Run the sample
  ```bash
  npm start
  ```


## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator)

### Connect to bot using Bot Framework Emulator v4
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to `javascript_nodejs/84.bot-authentication-msgraph` folder
- Select `authBot.bot` file

## Further Reading
- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot basics](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [LUIS](https://luis.ai)
- [Prompt Types](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0&tabs=javascript)
- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [QnA Maker](https://qnamaker.ai)


## Additional Resources

### Dependencies

- **[Restify](http://restify.com)** Used to host the web service for the bot, and for making REST calls
- **[dotenv](https://github.com/motdotla/dotenv)** Used to manage environmental variables

### Project Structure

The root `index.js` references the bot and starts a Restify server. The dialogs/mainDialog `index.js` loads the dialog type you selected when running the generator and adds it as the default dialog.

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



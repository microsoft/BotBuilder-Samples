# Enterprise Bot Template

This bot has been created using [Microsoft Bot Framework](https://dev.botframework.com).

## Concepts introduced in this sample
- Dialogs
- Template Manager
- Dispatch
- Middleware

This samples shows how to:
- Use [LUIS](https://luis.ai) to implement core AI capabilities
- Implement a multi-turn conversation using Dialogs
- Handle user interruptions for such things as Help or Cancel
- Prompt for and validate requests for information from the user


## To try this sample
- Clone the repository
  ```bash
  git clone https://github.com/microsoft/botbuilder-samples.git
  ```
- In a terminal, navigate to samples/javascript_typescript/52.enterprise-bot
  ```bash
  cd samples/javascript_typescript/52.enterprise-bot
  ```
- Install modules
  ```bash
  npm install
  ```
- Setup [Azure Powershell](https://docs.microsoft.com/en-us/powershell/azure/install-azurerm-ps?view=azurermps-6.9.0&viewFallbackFrom=azurermps-6.8.1)

  - Login to Azure

    ```bash
    Connect-AzureRmAccount
    ```
    **NOTE**: You will be prompted for login with your Microsoft Account

  - Select your Azure subscription

    ```bash
    Select-AzureRmSubscription -Subscription "subscription-name"
    ```
  

- Create [required services](./deploymentScripts/DEPLOYMENT.MD)
- Run the sample
  ```bash
  npm start
  ```
## Enabling more scenarios

### Authentication

To enable authentication follow these steps:

Register the SignInDialog in the MainDialog constructor
    
  ```typescript
  this.addDialog(new SignInDialog(this._services.authConnectionName);
  ```

Add the following in your code at your desired location to test a simple login flow:
  ```typescript
  const signInResult = await dc.beginDialog('SignInDialog');
  ```

### Content Moderation
Content moderation can be used to identify PII and adult content in the messages sent to the bot. To enable this functionality, go to the azure portal
and create a new content moderator service. Collect your subscription key and region to configure your .bot file. 

Uncomment the section of `Content Moderation Middleware` in the `index.ts` file to enable content moderation on every turn. 
The result of content moderation can be accessed via your bot state 


## Prerequisites
- NodeJS & Node Package Manager

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator)

### Connect to bot using Bot Framework Emulator v4
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to `javascript_typescript/52.enterprise-bot` folder
- Select your `.bot` file

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




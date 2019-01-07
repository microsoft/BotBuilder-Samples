<<<<<<< HEAD
# <%= botName %>
<%= description %>

This bot has been created using [Microsoft Bot Framework][1].

This samples shows how to:
- Use [LUIS][2] to implement core AI capabilities
- Implement a multi-turn conversation using Dialogs
- Handle user interruptions for such things as Help or Cancel
- Prompt for and validate requests for information from the user
- Demonstrate how to handle any unexptected errors


## To run this bot
- In a terminal,
  ```bash
  cd <%= botName %>
  ```
- Install modules
  ```bash
  npm install
  ```
- Create [required services][3]
- Run the bot
  ```bash
  <%= runCmd %>
  ```

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator][4] is a desktop application that allows bot developers to test and debug their bots on running locally or  or running remotely in Microsoft Azure.

- Install the Bot Framework Emulator from [here][5]

### Connect to the bot using Bot Framework Emulator v4
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to `<%= botName %>` folder
- Select `<%= botName %>.bot` file

## Deploy this bot to Azure
See [DEPLOYMENT.md][3] to learn more about deploying this bot to Azure and using the CLI tools to build the LUIS models this bot depends on.

## Further Reading
- [Bot Framework Documentation][6]
- [Bot basics][7]
- [Activity processing][8]
- [LUIS][2]
- [Prompt Types][9]
- [Azure Bot Service Introduction][10]
- [Channels and Bot Connector Service][11]
- [QnA Maker][12]

## Additional Resources

### Dependencies

- **[Restify][13]** Used to host the web service for the bot, and for making REST calls
- **[dotenv][14]** Used to manage environmental variables

### Project Structure
`index.<%= extension %>` references the bot and starts a Restify server. `bot.<%= extension %>` loads the main dialog router and determines how activities are processed.

### Configuring the bot

The generator created a `.env` file with the two necessary keys `botFilePath` and `botFileSecret`.  The `botFilePath` key is set to `<%= botName %>.bot`.  All of the services and their respective configuration settings are stored in the .bot file.
  - For Azure Bot Service bots, you can find the `botFileSecret` under application settings.
  - It is recommended that you encrypt your bot file before you commit it to your souce control system and/or before you deploy your bot to Azure or similar hosting service.  There are two ways to encrypt your `<%= botName %>.bot` file.  You can use [MSBot CLI][15] to encrypt your bot file or you can use [Bot Framework Emulator **V4**][16] to encrypt your bot file.  Both options will product a `botFileSecret` for you.  You will need to remember this in order to decrypt your .bot file.

### Running the bot

```
<%= runCmd %>
```
### Developing the bot

```
<%= watchCmd %>
```

[1]: https://dev.botframework.com
[2]: https://luis.ai
[3]: ./deploymentScripts/DEPLOYMENT.md
[4]: https://github.com/microsoft/botframework-emulator
[5]: https://aka.ms/botframework-emulator
[6]: https://docs.botframework.com
[7]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[8]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0
[9]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0&tabs=javascript
[10]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[11]: https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0
[12]: https://qnamaker.ai
[13]: http://restify.com
[14]: https://github.com/motdotla/dotenv
[15]: https://github.com/microsoft/botbuilder-tools
[16]: https://github.com/microsoft/botframework-emulator
=======
# <%= botname %>
<%= description %>

This bot has been created using [Bot Framework][1].

This samples shows how to:
- Use [LUIS][11] to implement core AI capabilities
- Implement a multi-turn conversation using Dialogs
- Handle user interruptions for such things as Help or Cancel
- Prompt for and validate requests for information from the user
- Demonstrate how to handle any unexpected errors


## Prerequisites
This samples requires prerequisites in order to run.
- [Required Prerequisites][41]

# To run the bot
- Install modules
    ```bash
    npm install
    ```
- Setup LUIS

    Assuming prerequisites have been installed:
    ```bash
    # log into Azure
    az login
    ```
    ```bash
    # set you Azure subscription
    az account set --subscription "<azure-subscription>"
    ```
    ```bash
    # Create LUIS service application
    msbot clone services --name "<%= botname %>" --luisAuthoringKey <LUIS-authoring-key> --code-dir "." --location <azure region like eastus, westus, westus2 etc.> --sdkLanguage "Node" --folder deploymentScripts/msbotClone --verbose
    ```
- Start the bot
    ```bash
    npm start
    ```

# Testing the bot using Bot Framework Emulator **v4**
[Bot Framework Emulator][5] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.2.0 or greater from [here][6]

## Connect to the bot using Bot Framework Emulator **v4**
- Launch Bot Framework Emulator
- File -> Open Bot Configuration
- Navigate to `<%= botname %>` folder
- Select `<%= botname %>.bot` file

# Deploy the bot to Azure
After creating the bot and testing it locally, you can deploy it to Azure to make it accessible from anywhere.
To learn how, see [Deploy your bot to Azure][40] for a complete set of deployment instructions.


# Further reading
- [Bot Framework Documentation][20]
- [Bot Basics][32]
- [Azure Bot Service Introduction][21]
- [Azure Bot Service Documentation][22]
- [Deploying Your Bot to Azure][40]
- [Azure CLI][7]
- [msbot CLI][9]
- [Azure Portal][10]
- [Language Understanding using LUIS][11]
- [Restify][30]
- [dotenv][31]

[1]: https://dev.botframework.com
[4]: https://nodejs.org
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://github.com/Microsoft/BotFramework-Emulator/releases
[7]: https://docs.microsoft.com/cli/azure/?view=azure-cli-latest
[8]: https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest
[9]: https://github.com/Microsoft/botbuilder-tools/tree/master/packages/MSBot
[10]: https://portal.azure.com
[11]: https://www.luis.ai
[20]: https://docs.botframework.com
[21]: https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[22]: https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0
[30]: https://www.npmjs.com/package/restify
[31]: https://www.npmjs.com/package/dotenv
[32]: https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[40]: https://aka.ms/azuredeployment
[41]: ./PREREQUISITES.md
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

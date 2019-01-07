<<<<<<< HEAD
# basic-bot

This bot has been created using [Microsoft Bot Framework](https://dev.botframework.com),

This samples shows how to:
- Use [LUIS](https://luis.ai) to implement core AI capabilities
- Implement a multi-turn conversation using Dialogs
- Handle user interruptions for such things as 'Help' or 'Cancel'
- Prompt for and validate requests for information from the user


## To try this sample
- Clone the repository
  ```bash
  git clone https://github.com/microsoft/botbuilder-samples.git
  ```
- In a terminal,
  ```bash
  cd samples/javascript_typescript/13.basic-bot
  ```
- Install modules
  ```bash
  npm install
  ```
- Create [required services](./deploymentScripts/DEPLOY.MD)
=======
# basic-bot sample
Bot Framework v4 basic bot sample

This bot has been created using [Microsoft Bot Framework][1], it shows how to:
- Use [LUIS][11] to implement core AI capabilities
- Implement a multi-turn conversation using Dialogs
- Handle user interruptions for such things as 'Help' or 'Cancel'
- Prompt for and validate requests for information from the user
- Demonstrate how to handle any unexpected errors

## Prerequisite
This sample requires prerequisites in order to run.
- [Required Prerequisites][41]

## To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to `samples/javascript_typescript/13.basic-bot`
    ```bash
    cd samples/javascript_typescript/13.basic-bot
    ```
- Install modules
    ```bash
    npm install
    ```
- Build the bot source code
    ```bash
    npm run build
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
    msbot clone services --name "<your_bot_name>" --luisAuthoringKey <LUIS-authoring-key> --code-dir "." --location westus --sdkLanguage "Node" --folder deploymentScripts/msbotClone --verbose
    ```
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
- Run the sample
  ```bash
  npm start
  ```

<<<<<<< HEAD
## Prerequisite
### Install TypeScript
In order to run this sample, you must have TypeScript installed.  To install TypeScript:
- Navigate to the [TypeScript portal](https://www.typescriptlang.org).
- Click the [Download](https://www.typescriptlang.org/#download-links) button.
- Follow the installation instructions for your development environment.

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator)

### Connect to bot using Bot Framework Emulator v4
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to `samples/javascript_typescript/13.basic-bot` folder
- Select `basic-bot.bot` file

## Deploy this bot to Azure
See [here](./deploymentScripts/DEPLOY.md) to learn more about deploying this bot to Azure and using the CLI tools to build the LUIS models this bot depends on.

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

`index.ts` references the bot and starts a Restify server. `bot.ts` loads the dialog type you selected when running the generator and adds it as the default dialog.

### Configuring the bot

Update `.env` with the appropriate keys botFilePath and botFileSecret.
  - For Azure Bot Service bots, you can find the botFileSecret under application settings.
  - If you use [MSBot CLI](https://github.com/microsoft/botbuilder-tools) to encrypt your bot file, the botFileSecret will be written out to the console window.
  - If you used [Bot Framework Emulator **V4**](https://github.com/microsoft/botframework-emulator) to encrypt your bot file, the secret key will be available in bot settings.

### Running the bot

```
node ./index.js
```
### Developing the bot

```
nodemon ./index.js
```


=======

## Testing the bot using Bot Framework Emulator **v4**
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to bot using Bot Framework Emulator v4
- Launch Bot Framework Emulator
- File -> Open Bot Configuration
- Navigate to `samples/javascript_typescript/13.basic-bot` folder
- Select `basic-bot.bot` file

# Deploy the bot to Azure
After creating the bot and testing it locally, you can deploy it to Azure to make it accessible from anywhere.  To deploy your bot to Azure:

```bash
# login to Azure
az login
```

### Publishing Changes to Azure Bot Service
As you make changes to your locally running bot, you can deploy those changes to Azure Bot Service using a _publish_ helper.  See `publish.cmd` if you are on Windows or `./publish` if you are on a non-Windows platform.  The following is an example of publishing local changes to Azure:

```bash
# build the TypeScript bot before you publish
npm run build
```

```bash
# run the publish helper (non-Windows) to update Azure Bot Service.  Use publish.cmd if running on Windows
./publish
```

### Getting Additional Help with Deploying to Azure
To learn more about deploying a bot to Azure, see [Deploy your bot to Azure][40] for a complete list of deployment instructions.


# Further reading
- [Bot Framework Documentation][20]
- [Bot Basics][32]
- [Prompt types][23]
- [Waterfall dialogs][24]
- [Ask the user questions][26]
- [Activity processing][25]
- [Azure Bot Service Introduction][21]
- [Azure Bot Service Documentation][22]
- [Azure CLI][7]
- [msbot CLI][9]
- [Azure Portal][10]
- [Language Understanding using LUIS][11]
- [Channels and Bot Connector Service][27]
- [Restify][30]
- [dotenv][31]

[7]: https://docs.microsoft.com/cli/azure/?view=azure-cli-latest
[8]: https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest
[9]: https://github.com/Microsoft/botbuilder-tools/tree/master/packages/MSBot
[10]: https://portal.azure.com
[11]: https://www.luis.ai
[20]: https://docs.botframework.com
[21]: https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[22]: https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0
[23]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0&tabs=javascript
[24]: https://docs.microsoft.com/en-us/javascript/api/botbuilder-dialogs/waterfall
[25]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0
[26]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-waterfall?view=azure-bot-service-4.0&tabs=jstab
[27]: https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0
[30]: https://www.npmjs.com/package/restify
[31]: https://www.npmjs.com/package/dotenv
[32]: https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145


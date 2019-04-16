# core-bot sample
Bot Framework v4 core bot sample.

This bot has been created using [Bot Framework][1], it shows how to:
- Use [LUIS][11] to implement core AI capabilities
- Implement a multi-turn conversation using Dialogs
- Handle user interruptions for such things as `Help` or `Cancel`
- Prompt for and validate requests for information from the user

## Prerequisites
This sample **requires** prerequisites in order to run.

### Overview
This bot uses [LUIS][11], an AI based cognitive service, to implement language understanding.

- Node.js version 10.14.1 or higher.
    ```bash
    # determine node version
    node --version
    ```

### Create a LUIS Application to enable language understanding
LUIS language model setup, training, and application configuration steps can be found [here][7].

If you wish to create a LUIS application via the CLI, these steps can be found in the [README-LUIS.md][41].

## To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to `samples/javascript_nodejs/13.core-bot`
    ```bash
    cd samples/javascript_nodejs/13.core-bot
    ```
- Install modules
    ```bash
    npm install
    ```
- Setup LUIS

    The prerequisite outlined above contain the steps necessary to provision a language understanding model on www.luis.ai.  Refer to _Create a LUIS Application to enable language understanding_ above for directions to setup and configure LUIS.

- Run the sample
    ```bash
    npm start
    ```
# Testing the bot using Bot Framework Emulator
[Bot Framework Emulator][5] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here][6]

## Connect to the bot using Bot Framework Emulator
- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

# Deploy the bot to Azure
To learn more about deploying a bot to Azure, see [Deploy your bot to Azure][40] for a complete list of deployment instructions.


# Further reading
- [Bot Framework Documentation][20]
- [Bot Basics][32]
- [Azure Bot Service Introduction][21]
- [Azure Bot Service Documentation][22]
- [Deploying Your Bot to Azure][40]
- [Azure CLI][7]
- [Azure Portal][10]
- [Language Understanding using LUIS][11]
- [Add Natural Language Understanding to Your Bot][12]
- [TypeScript][2]
- [Restify][30]
- [dotenv][31]

[1]: https://dev.botframework.com
[2]: https://www.typescriptlang.org
[3]: https://www.typescriptlang.org/#download-links
[4]: https://nodejs.org
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://github.com/Microsoft/BotFramework-Emulator/releases
[7]: https://docs.microsoft.com/cli/azure/?view=azure-cli-latest
[8]: https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest
[10]: https://portal.azure.com
[11]: https://www.luis.ai
[12]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&tabs=js#configure-your-bot-to-use-your-luis-app
[20]: https://docs.botframework.com
[21]: https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[22]: https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0
[30]: https://www.npmjs.com/package/restify
[31]: https://www.npmjs.com/package/dotenv
[32]: https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[40]: https://aka.ms/azuredeployment
[41]: README-LUIS.md

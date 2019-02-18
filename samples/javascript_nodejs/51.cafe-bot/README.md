# cafe bot
Bot Framework v4 cafe bot sample

Contoso cafe bot is a complete and fairly sophisticated sample that demonstrates various parts of the [BotBuilder V4 SDK](https://github.com/microsoft/botbuilder-js) and [BotBuilder CLI tools](https://github.com/microsoft/botbuilder-tools) in action.

This sample relies on prior knowledge/ familiarity with the following tools and services
- [LUIS](https://www.luis.ai)
- [QnA Maker](https://qnamaker.ai)
- [Ludown CLI tool](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Ludown)
- [LUIS CLI tool](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/LUIS)
- [QnA Maker CLI tool](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker)
- [MSBOT CLI tool](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/MSBot)
- [Chatdown CLI tool](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown)
- [Bot Framework Emulator](https://github.com/Microsoft/BotFramework-Emulator)

# Concepts covered in this sample
Contoso cafe bot is a fairly sophisticated bot sample that uses the following concepts in the [BotBuilder V4 SDK](https://github.com/microsoft/botbuilder-js) and [BotBuilder CLI tools](https://github.com/microsoft/botbuilder-tools) -

## Scenarios demonstrated
- Welcoming users
- Using cards to interact with users
- Using suggested actions to solicit user input
- Single and multi-turn conversations with users
- Interruptable, multi-turn conversations
- Support for help and cancel
- FAQ
- Chit-chat conversations
- Routing user input to appropriate dialog
- Handling no-match
- Prompting users for information
- Implementing a custom prompt
- Multi-turn conversations using dialogs
- Implementing custom dialog solution
- Managing user and conversation state

## Services and tools demonstrated
- Using [LUIS](https://www.luis.ai) for Natural Language Processing
- Using [QnA Maker](https://qnamaker.ai) for FAQ, chit-chat, getting help and other single-turn conversations
- Using [BotBuilder CLI tools](https://github.com/microsoft/botbuilder-tools) to create, configure and manage all required services.

## Scenarios covered via other samples
- [Proactive messages](../../../samples/javascript_nodejs/16.proactive-messages)
- [Multi-lingual conversations](../../../samples/javascript_nodejs/17.multi-lingual-conversations)
- [Bot authentication](../../../samples/javascript_nodejs/18.bot-authentication)

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to `samples/javascript_nodejs/51.cafe-bot`
    ```bash
    cd samples/javascript_nodejs/51.cafe-bot
    ```
- [Install required CLI tools](#Install-CLI-tools)
- Configure required services
    - [Using MSBOT CLI](#Configure-required-services-using-msbot)
    - [Manually import models using Ludown, LUIS and QnA Maker CLI](#building-and-creating-services)

- Install modules
    ```bash
    npm install
    ```
- Start the bot
    ```bash
    npm start
    ```

# Testing the bot using Bot Framework Emulator **v4**
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/microsoft/botframework-emulator/releases)

## Connect to bot using Bot Framework Emulator **v4**
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to `samples/javascript_nodejs/51.cafe-bot/`
- Select `contoso-cafe-bot.bot`

# Prerequisites
## Install CLI tools

Ensure you have [Node.js](https://nodejs.org/) version 10.14 or higher

- In a terminal
    ```bash
    npm i -g msbot chatdown ludown luis-apis qnamaker
    ```

## Configure required services using MSBOT
1. Follow instructions [here](https://portal.azure.com) to create an Azure account. If you already have an account, sign in. Click on all services -> search for 'subscriptions' -> copy the subscription ID you would like to use from the Home > Subscriptions page.
2. Follow instructions [here](https://www.luis.ai/home) to create a LUIS.ai account. If you already have an account, sign in. Click on your name on top right corner of the screen -> settings and grab your authoring key.
3. To create and configure required LUIS and QnA Maker services,
    - In a terminal,
        ```bash
        cd samples/javascript_nodejs/51.cafe-bot
        ```
    - Run MSbot Clone and pass in your LUIS authoring key and Azure subscription ID. This command will create required services for your bot and update the .bot file.
        ```bash
        msbot clone services -n <YOUR-BOT-NAME> -f deploymentScripts/msbotClone -l <Bot service location> --luisAuthoringKey <Key from step-2 above> --subscriptionId <Key from step-1 above> --appId <YOUR APP ID> --appSecret <YOUR APP SECRET PASSWORD>
        ```

**NOTE**: You can obtain your `appId` and `appSecret` at the Microsoft's [Application Registration Portal](https://apps.dev.microsoft.com/)

**Important:** Ensure that the name property of your LUIS endpoint in your `.bot` file matches the `luisConfig` in the following files:

* `/dialogs/dispatches/interruptionDispatcher.js`
* `/dialogs/dispatches/mainDispatcher.js`
* `/dialogs/shared/prompts/getLocDateTimePartySize.js`
* `/dialogs/shared/prompts/getUserNamePrompt.js`

Optionally, you can use the LUIS, QnA Maker portals to manually import the models found under **cognitiveModels** folder of this sample.

# Relevant commands for CLI tools
## Building and creating services
- Parse `.lu` files into LUIS models
    ```bash
    > ludown parse toluis --in dialogs/dispatcher/resources/cafeDispatchModel.lu -o cognitiveModels -n cafeDispatchModel.luis --verbose
    > ludown parse toluis --in dialogs/bookTable/resources/turn-N.lu -o cognitiveModels -n cafeBotBookTableTurnN.luis --verbose
    > ludown parse toluis --in dialogs/whoAreYou/resources/getUserProfile.lu -o cognitiveModels -n getUserProfile.luis --verbose
    ```
- Parse `.lu` files into QnA Maker KB and QnA Maker alterations file
    ```bash
    > ludown parse toqna --in dialogs/dispatcher/resources/cafeFAQ_ChitChat.lu -o cognitiveModels -n cafeFaqChitChat.qna -a --verbose
    ```
- Import LUIS applications and update the `.bot` file with LUIS service references:
    - Follow instructions [here](https://www.luis.ai/home) to create a LUIS.ai account. If you already have an account, sign in. Click on your name on top right corner of the screen -> settings and grab your authoring key.
    - LUIS-AUTHORING-REGION can be one of [westus|westeurope|australiaeast]. See [here](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-reference-regions) for more information.
    ```bash
    > luis import application --in cognitiveModels/cafeDispatchModel.luis --authoringKey <Your LUIS authoring key> --region <LUIS-AUTHORING-REGION> --msbot | msbot connect luis --stdin
    > luis import application --in cognitiveModels/cafeBotBookTableTurnN.luis --authoringKey <Your LUIS authoring key> --region <LUIS-AUTHORING-REGION> --msbot | msbot connect luis --stdin
    > luis import application --in cognitiveModels/getUserProfile.luis --authoringKey <Your LUIS authoring key> --region <LUIS-AUTHORING-REGION> --msbot | msbot connect luis --stdin
    ```
- Import QnA Maker KBs (Note: You don't need this if you have already run MSBOT clone)
    - Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/quickstarts/create-new-kb-nodejs#prerequisites) to create a QnA Maker subscription and copy your QnA Maker subscription key.
    ```bash
    > qnamaker create kb --in cognitiveModels/cafeFaqChitChat.qna --subscriptionKey <Your QnA Maker subscription key> --msbot | msbot connect qna --stdin
    ```
- Train LUIS model
    ```bash
    > msbot get cafeDispatchModel | luis train version --wait --stdin
    > msbot get cafeBotBookTableTurnN | luis train version --wait --stdin
    > msbot get getUserProfile | luis train version --wait --stdin
    ```
- Publish LUIS models
    ```bash
    > msbot get cafeDispatchModel | luis publish version --stdin
    > msbot get cafeBotBookTableTurnN | luis publish version --stdin
    > msbot get getUserProfile | luis publish version --stdin
    ```
- Train and publish QnA Maker KB
    ```bash
    > msbot get cafeFaqChitChat | qnamaker publish kb --stdin
    ```
- Replace QnA Maker alterations
    ```bash
    > msbot get cafeFaqChitChat | qnamaker replace alterations --in cognitiveModels/cafeFaqChitChat.qna_Alterations.json --stdin
    ```

## Making updates to the models

Any time you change `.lu` files, you can update and publish the LUIS and QnA Maker models using the following commands. You can also script and automate the CLI tools to match your development workflow. See [here](./deploymentScripts/updateModels.bat) for an example.

- Parse `.lu` files into LUIS models
    ```bash
    > ludown parse toluis --in dialogs/dispatcher/resources/cafeDispatchModel.lu -o cognitiveModels -n cafeDispatchModel.luis --verbose
    > ludown parse toluis --in dialogs/bookTable/resources/turn-N.lu -o cognitiveModels -n cafeBotBookTableTurnN.luis --verbose
    > ludown parse toluis --in dialogs/whoAreYou/resources/getUserProfile.lu -o cognitiveModels -n getUserProfile.luis --verbose
    ```
- Parse `.lu` files into QnA Maker KB and QnA Maker alterations file
    ```bash
    > ludown parse toqna --in dialogs/dispatcher/resources/cafeFAQ_ChitChat.lu -o cognitiveModels -n cafeFaqChitChat.qna -a --verbose
    ```
- Rename current LUIS version
    ```bash
    > msbot get cafeDispatchModel --secret <Key if your bot file is encrypted> | luis rename version --newVersionId 0.1_old --stdin
    ```
- Import a LUIS new application version
    ```bash
    > msbot get cafeDispatchModel --secret <Key if your bot file is encrypted> | luis import version --stdin --in cognitiveModels/cafeDispatchModel.luis
    ```
- Deleting old LUIS application version
    ```bash
    > msbot get cafeDispatchModel --secret <Key if your bot file is encrypted> | luis delete version --stdin --versionId 0.1_old
    ```
- Training a LUIS model
    ```bash
    > msbot get cafeDispatchModel --secret <Key if your bot file is encrypted> | luis train version --wait --stdin
    ```
- Publishing a LUIS model
    ```bash
    > msbot get cafeDispatchModel --secret <Key if your bot file is encrypted> | luis publish version --stdin
    ```

## Updating and publishing QnA Maker KB

- Replace KB contents
    ```bash
    > msbot get cafeFaqChitChat --secret <Key if your bot file is encrypted> | qnamaker replace kb --in cognitiveModels/cafeFaqChitChat.qna --stdin
    ```
- Train and publish KB
    ```bash
    > msbot get cafeFaqChitChat --secret <Key if your bot file is encrypted> | qnamaker publish kb --stdin
    ```
- Replace QnA Maker alterations
    ```bash
    > msbot get cafeFaqChitChat --secret <Key if your bot file is encrypted> | qnamaker replace alterations --in cognitiveModels/cafeFaqChitChat.qna_Alterations.json --stdin
    ```

Contoso cafe bot is a complete and fairly sophisticated sample that demonstrates various parts of the [BotBuilder V4 SDK](https://github.com/microsoft/botbuilder-dotnet) and [BotBuilder CLI tools](https://github.com/microsoft/botbuilder-tools) in action. 

This sample relies on prior knowledge / familiarity with the following tools and services 
- [LUIS](https://www.luis.ai)
- [QnA Maker](https://qnamaker.ai)
- [Ludown CLI tool](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Ludown)
- [LUIS CLI tool](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/LUIS)
- [QnA Maker CLI tool](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker)
- [MSBOT CLI tool](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/MSBot)
- [Chatdown CLI tool](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown)
- [Bot Framework Emulator](https://github.com/Microsoft/BotFramework-Emulator)

# Concepts covered in this sample
Contoso cafe bot is a fairly sophisticated bot sample that uses the following concepts in the [BotBuilder V4 SDK](https://github.com/microsoft/botbuilder-dotnet) and [BotBuilder CLI tools](https://github.com/microsoft/botbuilder-tools) - 

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
- [Proactive messages](../../../samples/csharp_dotnetcore/16.proactive-messages)
- [Multi-lingual conversations](../../../samples/csharp_dotnetcore/17.multi-lingual-conversations)
- [Bot authentication](../../../samples/csharp_dotnetcore/18.bot-authentication)

# To try this sample
- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
# Running Locally

## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/51.cafe-bot`) and open `CafeBot.csproj` in Visual Studio.
- Run the project (press `F5` key)

## .NET Core CLI
- Install the [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x). 
- Using the command line, navigate to `botbuilder-samples/samples/csharp_dotnetcore/51.cafe-bot` folder.
- Type `dotnet run`.

# Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://aka.ms/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

## Connect to bot using Bot Framework Emulator V4
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to `botbuilder-samples/samples/csharp_dotnetcore/51.cafe-bot`
- Select `cafe-bot.bot`.

# Prerequisites
## Install CLI tools

- Install [Node.js](https://nodejs.org/)
- In a terminal
- Install required tools
    ```bash
    npm i -g msbot chatdown ludown luis-apis qnamaker 
    ```

## Configure required services
1. Follow instructions [here](https://portal.azure.com) to create an Azure account. If you already have an account, sign in. Click on all services -> search for 'subscriptions' -> copy the subscription ID you would like to use from the Home > Subscriptions page.
2. Follow instructions [here](https://www.luis.ai/home) to create a LUIS.ai account. If you already have an account, sign in. Click on your name on top right corner of the screen -> settings and grab your authoring key.
3. To create and configure required LUIS and QnA Maker services, 
    - In a Powershell prompt,
```bash
cd BotBuilder-Samples\samples\csharp_dotnetcore\51.cafe-bot
```
 - - To login, run:	

```bash
az login
```
- - To select your Azure subscription, run:

```bash
az account set --subscription "<YOUR SUBSCRIPTION>"
```

- - Run MSbot Clone and pass in your LUIS authoring key and Azure subscription ID. This command will create required services for your bot and update the .bot file.

```bash
msbot clone services --name <YOUR-BOT-NAME> --folder DeploymentScripts/MsbotClone --location <Bot service location, ie "westus"> --luisAuthoringKey <YOUR LUIS AUTHORING KEY> --appId <YOUR APP ID> --appSecret <YOUR APP SECRET PASSWORD>
```

**NOTE**: You can obtain your `appId` and `appSecret` at the Microsoft's [Application Registration Portal](https://apps.dev.microsoft.com/)

**Important:** Ensure that LuisConfiguration in CafeBot.cs matches the name property of your LUIS endpoint in your `.bot` file.


Optionally, you can use the LUIS, QnA Maker portals to manually import the models found under **CognitiveModels** folder of this sample. 

# Relevant commands for CLI tools
## Building and creating services
- Parse .lu files into LUIS models
    ```bash
    > ludown parse toluis --in Dialogs/Dispatcher/Resources/cafeDispatchModel.lu -o cognitiveModels -n cafeDispatchModel --out cafeDispatchModel.luis
    > ludown parse toluis --in Dialogs/BookTable/Resources/turn-N.lu -o cognitiveModels -n cafeBotBookTableTurnNModel --out cafeBotBookTableTurnN.luis
    > ludown parse toluis --in Dialogs/WhoAreYou/Resources/getUserProfile.lu -o cognitiveModels -n getUserProfile --out getUserProfile.luis
    ```
- Parse .lu files into QnA Maker KB and QnA Maker alterations file
    ```bash
    > ludown parse toqna --in Dialogs/Dispatcher/Resources/cafeFAQ_ChitChat.lu -o cognitiveModels -n cafeFaqChitChat --out cafeFaqChitChat.qna -a
    ```
- Import LUIS applications (Note: You don't need this if you have already run MSBOT clone)
    ```bash
    > luis import application --in CognitiveModels/cafeDispatchModel.luis --authoringKey <Your LUIS authoring key> --region <LUIS-Authoring-Region> --msbot | msbot connect luis --stdin
    > luis import application --in CognitiveModels/cafeBotBookTableTurnN.luis --authoringKey <Your LUIS authoring key> --region <LUIS-Authoring-Region> --msbot | msbot connect luis --stdin
    > luis import application --in CognitiveModels/getUserProfile.luis --authoringKey <Your LUIS authoring key> --region <LUIS-Authoring-Region> --msbot | msbot connect luis --stdin
    ```
    **Note**: LUIS authoring region can be one of westus or westeurope or australiaeast

- Import QnA Maker KBs (Note: You don't need this if you have already run MSBOT clone)
    ```bash
    > qnamaker create kb --in CognitiveModels/cafeFaqChitChat.qna --subscriptionKey <YOUR QnA Maker Subscription Key> --msbot | msbot connect qna --stdin
    ```
    **Note**: See [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/quickstarts/csharp) for instructions on setting up a Cognitive Service API account and fetching your QnA Maker subscription Key. 

## Train and publish LUIS models
- Training a LUIS model
    ```bash
    > msbot get cafeDispatchModel | luis train version --wait --stdin
    > msbot get cafeBotBookTableTurnNModel | luis train version --wait --stdin
    > msbot get getUserProfile | luis train version --wait --stdin
    ```
- Publishing a LUIS model
    ```bash
    > msbot get cafeDispatchModel | luis publish version --stdin
    > msbot get cafeBotBookTableTurnNModel | luis publish version --stdin
    > msbot get getUserProfile | luis publish version --stdin
    ```

## Train and publish QnA Maker applications
- Train and publish KB
    ```bash
    > msbot get cafeFaqChitChat | qnamaker publish kb --stdin
    ```
- Replace QnA Maker alterations
    ```bash
    > msbot get cafeFaqChitChat | qnamaker replace alterations --in CognitiveModels/cafeFaqChitChat.qna_Alterations.json --stdin
    ```

## Updating and publishing LUIS models
- Rename current LUIS version
    ```bash
    > msbot get cafeDispatchModel | luis rename version --newVersionId 0.1_old --stdin
    ```
- Import a LUIS application version
    ```bash
    > msbot get cafeDispatchModel | luis import version --stdin --in CognitiveModels/cafeDispatchModel.luis
    ```
- Deleting old LUIS application version
    ```bash
    > msbot get cafeDispatchModel | luis delete version --stdin --versionId 0.1_old
    ```

## Updating and publishing QnA Maker KB

- Replace KB contents
    ```bash
    > msbot get cafeFaqChitChat | qnamaker replace kb --in CognitiveModels/cafeFaqChitChat.qna --stdin
    ```

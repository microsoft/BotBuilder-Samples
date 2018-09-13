This sample shows how to create complex conversation flows such as interruptions and cancellations, to build user-friendly, effective bots.

# Concepts introduced in this sample

Interruptions: We create an InterruptableDialog that allows us to complete a task such as gathering user information, but also be interrupted to, for example, ask for help and then pick up where we left off in the form filling.
This is a very natural pattern un human conversations. 

The [Language Understanding Intelligent Service (LUIS)](https://luis.ai), is a machine learning-based service to build natural language into apps, bots, and IoT devices. 
LUIS allows to Quickly create enterprise-ready, custom models that continuously improve. 

In this sample, we create a bot that can greet, collect user information in order to escalate to a human, and supports cancellation of the escalation process as well as interruption for help. Once help is provided in a card, the bot returns to the form filling to gather user information. 

## Overvies

This is achieved in this bot by creating a subclass of ComponentDialog, which we name [InterruptableDialog](Dialogs\Shared\InterruptableDialog.cs). We then have a main dialog, which is the entry point to the conversation.
When users ask for human escalation by saying, for example, ```talk to a human```, the [OnboardingDialog](Dialogs\Onboarding\OnboardingDialog.cs) triggers and starts the process of retrieving user information.
If the user asks for help, this will triger the Help interruption and then come back to form filling. 
Alternatively, asking for cancellation will cancel gathering user data, and trigger the [CancelDialog](Dialoges\Cancel\CancelDialog.cs).

# To try this sample

- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
## Install BotBuilder tools

- In a terminal, navigate to the samples folder (`BotBuilder-Samples\csharp_dotnetcore\14.NLP-With-Dispatch`) 

    ```bash
    cd BotBuilder-Samples\csharp_dotnetcore\14.NLP-With-Dispatch
    ```

- Install required tools - to successfully setup and configure all services this bot depend on, you need to install the MSBOT, LUIS, QnAMaker, Ludown, Dispatch CLI tools. 
    ```bash
    npm i -g msbot luis-apis ludown
    ```
- Configure required LUIS, QnA Maker and Dispatch services. See [here](#configure-services)

## Configure Services

This sample relies on [LUIS](https://luis.ai), [QnA Maker](https://qnamaker.ai) and [Dispatch](https://github.com/microsoft/botbuilder-tools//tree/master/packages/Dispatch) services. 

### Configure the LUIS service

To create required LUIS applications for this sample bot, 
- Create an account with [LUIS](https://luis.ai). If you already have an account, login to your account.
- Click on your name on top right corner of the screen -> settings and grab your authoring key.

To create the LUIS application this bot needs and update the .bot file configuration, in a terminal, 
- Clone this repository
- Navigate to BotBuilder-Samples\csharp_dotnetcore\9.Message-Routing
- Run the following commands
```bash 
> ludown parse toluis --in CognitiveModels/LUIS/General.lu -o CognitiveModels/LUIS -n General.luis

> luis import application --in CognitiveModels\LUIS\General.luis --authoringKey <YOUR-LUIS-AUTHORING-KEY>--endpointBasePath https://westus.api.cognitive.microsoft.com/luis/api/v2.0 --msbot | msbot connect luis --stdin --name General.luis

```

Note: You can create the LUIS applications in one of the [LUIS authoring regions](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/luis-reference-regions). 
You can use a different region, such as westus, westeurope or australiaeast via https://**LUIS-Authoring-Region**.api.cognitive.microsoft.com/luis/api/v2.0 in the commands above.

### Train and publish the LUIS models 
You need to train and publish the LUIS models that were created for this sample to work. You can do so using the following CLI commands

```bash
> msbot get General.luis | luis train version --wait --stdin  --versionId "0.1"
> msbot get General.luis | luis publish version --wait --stdin  --versionId "0.1"
```

### Securing keys in your .bot file
Since your .bot file contains service Ids, subscription and authoring keys, its best to encrypt them. To encrypt the .bot file, run

```bash
msbot secret -n
```

This will generate a strong key, encrypt the bot file and print the key. Please keep this key securely.
Any time the bot file is encrypted, make sure to set the botFileSecret environment variable this sample relies on (either through the .env file or other means).

## Visual Studio
- Navigate to the samples folder (`BotBuilder-Samples\csharp_dotnetcore\9.Message-Routing`) and open MessageRoutingBot.csproj in Visual Studio 
- Hit F5

## Visual Studio Code
- Open `BotBuilder-Samples\csharp_dotnetcore\9.Message-Routing` sample folder.
- Bring up a terminal, navigate to BotBuilder-Samples\9.Message-Routing folder
- type 'dotnet run'

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframeworkemulator).

### Connect to bot using Bot Framework Emulator **V4**
- Launch Bot Framework Emulator
- File -> Open bot and navigate to `BotBuilder-Samples\csharp_dotnetcore\9.Message-Routing` folder
- Select MessageRouting.bot file

# Further reading

- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Application Insights](https://azure.microsoft.com/en-us/services/application-insights/)
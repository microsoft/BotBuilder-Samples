This sample shows how to integrate QnA Maker in a simple bot with ASP.Net Core 2 and Application Insights. 

# Concepts introduced in this sample

[Dispatch](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Dispatch) is a tool to create and evaluate LUIS models used for NLP (Natural Language Processing). 
Dispatch works across multiple bot modules such as LUIS applications, QnA knowledge bases and other NLP sources (added to dispatch as a file type). 
Use the Dispatch model in cases when:
* Your bot consists of multiple modules and you need assistance in routing user's utterances to these modules and evaluate the bot integration.
* Evaluate quality of intents classification of a single LUIS model.
* Create a text classification model from text files.

The [Language Understanding Intelligent Service (LUIS)](https://luis.ai), is a machine learning-based service to build natural language into apps, bots, and IoT devices. 
LUIS allows to Quickly create enterprise-ready, custom models that continuously improve. 

The [QnA maker Service](https://www.qnamaker.ai) enables you to build, train and publish a simple question and answer bot based on FAQ URLs, structured documents or editorial content in minutes.

The [Application Insights](https://azure.microsoft.com/en-us/services/application-insights/) enables you to discover actionable insights through application performance management and instant analytics.

In this sample, we demonstrate how to use the Dispatch service to route utterances when there are multiple LUIS models and QnA maker services for different scenarios supported by a bot. 
In this case, we confiure dispatch with multiple LUIS models for conversations around home automation and weather information, plus QnA maker service to answer questions based on a FAQ text file as input.

# To try this sample

- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
## Install BotBuilder tools

- In a terminal, navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/14.nlp-with-dispatch`) 

    ```bash
    cd botbuilder-samples/samples/csharp_dotnetcore/14.nlp-with-dispatch
    ```

- Install required tools - to successfully setup and configure all services this bot depend on, you need to install the MSBOT, LUIS, QnAMaker, Ludown, Dispatch CLI tools. 

    Ensure you have [Node.js](https://nodejs.org/) version 8.5 or higher

    ```bash
    npm i -g msbot luis-apis qnamaker ludown botdispatch
    ```
- Configure required LUIS, QnA Maker and Dispatch services. See [here](#configure-services)

# Configure Services

This sample relies on [LUIS](https://luis.ai), [QnA Maker](https://qnamaker.ai) and [Dispatch](https://github.com/microsoft/botbuilder-tools//tree/master/packages/Dispatch) services. 

## Deploy this bot to Azure and configure services

You can use the MSBot Bot Builder CLI tool to clone and configure any services this sample depends on.

To install all Bot Builder tools:

Ensure you have [Node.js](https://nodejs.org/) version 8.5 or higher

```
npm i -g msbot chatdown ludown qnamaker luis-apis botdispatch luisgen
```
To clone this bot, run:
- Run MSbot Clone and pass in your LUIS authoring key and Azure subscription ID. This command will create required services for your bot and update the .bot file.
- Setup Azure Powershell (If you have never done so before).
- To login, run:
    ```bash
    Connect-AzureRmAccount
    ```
- To select your Azure subscription, run:
    ```bash
    Select-AzureRmSubscription -Subscription "YOUR SUBSRIPTION"
    ```
- Set up your bot:
    ```bash
    msbot clone services --name <YOUR-BOT-NAME> --folder "DeploymentScripts/MsbotClone" --location <Bot service location - ie, "westus"> --luisAuthoringKey <LUIS_AUTHORING> --subscriptionId <AZURE_SUBSCRIPTION>
    ```
Note: You can also manually import the LUIS and QnA Maker applications via the [LUIS.ai](https://luis.ai) and [QnAMaker.ai](https://qnamaker.ai) portals. See instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/what-is-luis) to import the LUIS models and [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/quickstarts/create-publish-knowledge-base) to import the QnA Maker knowledge base. All models this sample relies on is [here](./cognitiveModels).

**Alternately** you can configure the required services by following the steps below. 

## Manually configure required services

### Configure the LUIS service

To create required LUIS applications for this sample bot, 
- Create an account with [LUIS](https://luis.ai). If you already have an account, login to your account.
- Click on your name on top right corner of the screen -> settings and grab your authoring key.

To create the LUIS application this bot needs and update the .bot file configuration, in a terminal, 
- Clone this repository
- Navigate to botbuilder-samples/csharp_dotnetcore/samples/14.nlp-with-dispatch
- Run the following commands
```bash 
> ludown parse toluis --in Resources/homeautomation.lu -o CognitiveModels --out homeAutomation.luis -n "Home Automation" -d "Home Automation LUIS application - Bot Builder Samples" --verbose

> ludown parse toluis --in Resources/weather.lu -o CognitiveModels --out weather.luis -n Weather -d "Weather LUIS application - Bot Builder Samples" --verbose

> luis import application --in CognitiveModels/homeAutomation.luis --authoringKey <LUIS-AUTHORING-KEY> --region <LUIS-AUTHORING-REGION> --msbot | msbot connect luis --stdin

> luis import application --in CognitiveModels/weather.luis --authoringKey <LUIS-AUTHORING-KEY> --region <LUIS-AUTHORING-REGION> --msbot | msbot connect luis --stdin
```

If you decide to change the names passed to msbot such as weather.luis, then you need to update the constants in [NlpDispatchBot.cs](NlpDispatch/NlpDispatchBot.cs). For example, if you change homeautomation.luis to just home, you would update the HomeAutomationLuisKey variable to "home" and the homeAutomationDispatchKey to the intent name assigned by dispatcher, which in this case will be "l_home".

Note: You can create the LUIS applications in one of the [LUIS authoring regions](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/luis-reference-regions). 
You can use a different region, such as westus, westeurope or australiaeast via https://**LUIS-Authoring-Region**.api.cognitive.microsoft.com/luis/api/v2.0 in the commands above.

### Train and publish the LUIS models 
You need to train and publish the LUIS models that were created for this sample to work. You can do so using the following CLI commands

```bash
> msbot get "Home Automation" | luis train version --wait --stdin
> msbot get "Weather" | luis train version --stdin --wait
> msbot get "Home Automation" | luis publish version --stdin
> msbot get "Weather" | luis publish version --stdin
```

### Configure QnA Maker service
To create a new QnA Maker application for the bot, 
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/set-up-qnamaker-service-azure) to create a new QnA Maker Azure resource.
- Navigate to your QnA Maker resource -> keys and copy the subscription key

To create the QnA Maker application and update the .bot file with the QnA Maker configuration,  
- Open a terminal
- Navigate to samples/csharp_dotnetcore/14.nlp-with-dispatch
- Run the following commands
```bash
> ludown parse toqna --in resources/sample-qna.lu -o cognitiveModels --out dispatch.qna --verbose

> qnamaker create kb --in cognitiveModels/dispatch.qna --subscriptionKey <QNA-MAKER-SUBSCRIPTION-KEY> --msbot | msbot connect qna --stdin
```
### Train and publish the QnA Maker KB
You need to train and publish the QnA Maker Knowledge Bases that were created for this sample to work. You can do so using the following CLI commands

```bash
> msbot get "sample-qna" | qnamaker publish kb --stdin
```

### Configure the Dispatch application
[Dispatch](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Dispatch) is a CLI tool that enables you to create a dispatch NLP model across the different LUIS applications and / or QnA Maker Knowledge Bases you have for your bot. For this sample, you would have already created 2 LUIS applications (Home Automation and Weather) and one QnA Maker Knowledge base. 

To create a new dispatch model for these services and update the .bot file configuration, in a terminal:
- Navigate to samples/csharp_dotnetcore/14.nlp-with-dispatch
- Run the following commands
```bash
> dispatch create -b BotConfiguration.bot | msbot connect dispatch --stdin
```
### Securing keys in your .bot file
Since your .bot file contains service Ids, subscription and authoring keys, its best to encrypt them. To encrypt the .bot file, run

```bash
msbot secret -n
```

This will generate a strong key, encrypt the bot file and print the key. Please keep this key securely.
Any time the bot file is encrypted, make sure to set the botFileSecret environment variable this sample relies on (either through the .env file or other means).

## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/14.nlp-with-dispatch`) and open NLP-With-Dispatch-Bot.csproj in Visual Studio 
- Hit F5

## Visual Studio Code
- Open `botbuilder-samples/samples/csharp_dotnetcore/14.nlp-with-dispatch` sample folder.
- Bring up a terminal, navigate to botbuilder-samples/14.NLP-With-Dispatch folder
- type 'dotnet run'

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframeworkemulator).

### Connect to bot using Bot Framework Emulator **V4**
- Launch Bot Framework Emulator
- File -> Open bot and navigate to `botbuilder-samples/samples/csharp_dotnetcore/14.nlp-with-dispatch` folder
- Select BotConfiguration.bot file

# Further reading

- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [QnA Maker Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/overview/overview)
- [QnA Maker Command Line Tool](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker)
- [Application Insights](https://azure.microsoft.com/en-us/services/application-insights/)
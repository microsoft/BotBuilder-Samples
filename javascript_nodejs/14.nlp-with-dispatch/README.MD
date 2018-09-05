[![Deploy to Azure](https://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)

This sample shows how to create a bot that relies on multiple [LUIS.ai](https://luis.ai) and [QnAMaker.ai](https://qnamaker.ai) models for natural language processing (NLP). 

This bot example uses [`restify`](https://www.npmjs.com/package/restify) and [`dotenv`](https://www.npmjs.com/package/dotenv).

[Dispatch](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Dispatch) is a tool to create and evaluate LUIS models used for NLP (Natural Language Processing). Dispatch works across multiple bot modules such as LUIS applications, QnA knowledge bases and other NLP sources (added to dispatch as a file type).

Use the Dispatch model in cases when:
- Your bot consists of multiple modules and you need assistance in routing user's utterances to these modules and evaluate the bot integration.
- Evaluate quality of intents classification of a single LUIS model.
- Create a text classification model from text files.

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/Microsoft/botbuilder-js.git
    ```
- In a terminal, navigate to javascript_nodejs/14.nlp-with-dispatch
    ```bash
    cd javascript_nodejs/14.nlp-with-dispatch
    ```
- Point to the MyGet feed 
    ```bash
    npm config set registry https://botbuilder.myget.org/F/botbuilder-v4-js-daily/npm/
    ```
- Install modules 
    ```bash
    npm i 
    ```
- Install required tools - to successfully setup and configure all services this bot depend on, you need to install the MSBOT, LUIS, QnAMaker, Ludown, Dispatch CLI tools. 
    ```bash
    npm i -g msbot luis-apis qnamaker ludown botdispatch
    ```
- Configure required LUIS, QnA Maker and Dispatch services. See [here](#configure-services)
- Once services are configured, run the sample 
    ```bash
    npm start
    ```
- To reset registry, you can do
    ```bash
    npm config set registry https://registry.npmjs.org/
    ```

# Configure services
This sample relies on [LUIS](https://luis.ai), [QnA Maker](https://qnamaker.ai) and [Dispatch](https://github.com/microsoft/botbuilder-tools//tree/master/packages/Dispatch) services. 

## Configure the LUIS service
To create required LUIS applications for this sample bot, 
- Create an account with [LUIS](https://luis.ai). If you already have an account, login to your account.
- Click on your name on top right corner of the screen -> settings and grab your authroing key.

To create the LUIS application this bot needs and update the .bot file configuration, in a terminal, 
- Clone this repository
- Navigate to javascript_nodejs\14.nlp-with-dispatch
- Run the following command
```bash 
> ludown parse toluis --in dialogs/homeAutomation/resources/homeautomation.lu -o cognitiveModels -n homeautomation.luis

> ludown parse toluis --in dialogs/weather/resources/weather.lu -o cognitiveModels -n weather.luis

> luis import application --in cognitiveModels\homeautomation.luis --authoringKey <YOUR-LUIS-AUTHORING-KEY> --endpointBasePath https://westus.api.cognitive.microsoft.com/luis/api/v2.0 --msbot | msbot connect luis --stdin --name homeautomation.luis

> luis import application --in cognitiveModels\weather.luis --authoringKey <YOUR-LUIS-AUTHORING-KEY> --endpointBasePath https://westus.api.cognitive.microsoft.com/luis/api/v2.0 --msbot | msbot connect LUIS --stdin --name weather.luis
```

Note: You can create the LUIS applications in one of the [LUIS authoring regions](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/luis-reference-regions). 
You can use a different region (westus or westeurope or australiaeast) via https://**LUIS-Authoring-Region**.api.cognitive.microsoft.com/luis/api/v2.0 in the commands above.

## Train and publish the LUIS models 
You need to train and publish the LUIS models that were created for this sample to work. You can do so using the following CLI commands

```bash
> msbot get service --name homeautomation.luis | luis train version --wait --stdin
> msbot get service --name homeautomation.luis | luis publish version --stdin
> msbot get service --name weather.luis | luis train version --wait --stdin
> msbot get service --name weather.luis | luis publish version --stdin
```

## Configure QnA Maker service
To create a new QnA Maker application for the bot, 
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/set-up-qnamaker-service-azure) to create a new QnA Maker Azure resource.
- Navigate to your QnA Maker resource -> keys and copy the subscription key

To create the QnA Maker application and update the .bot file with the QnA Maker configuration,  
- Open a terminal
- Navigate to samples\14.nlp-with-dispatch
- Run the following command
```bash
> ludown parse toqna --in dialogs/qna/resources/sample-qna.lu -o cognitiveModels -n sample.qna

> qnamaker create kb --in cognitiveModels\sample.qna --subscriptionKey <YOUR-QNA-SUBSCRIPTION-KEY> --msbot | msbot connect qna --stdin --name sample.qna
```
## Train and publish the QnA Maker KB
You need to train and publish the QnA Maker Knowledge Bases that were created for this sample to work. You can do so using the following CLI commands

```bash
> msbot get service --name sample.qna | qnamaker publish kb --stdin
```

## Configure the Dispatch application
[Dispatch](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Dispatch) is a CLI tool that enables you to create a dispatch NLP model across the different LUIS applications and/ or QnA Maker Knowledge Bases you have for your bot. For this sample, you would have already created 2 LUIS applications (Home Automation and Weather) and one QnA Maker Knowledge base. 

To create a new dispatch model for these services and update the .bot file configuration, in a terminal,
- Navigate to samples\14.nlp-with-dispatch
- Run the following command
```bash
> dispatch create -b nlp-with-dispatch.bot | msbot connect dispatch --stdin --name bot-dispatch
```
## Securing keys in your .bot file
Since your .bot file contains service IDs, subscription and authoring keys, its best to encrypt them. To encrypt the .bot file, you can run

```bash
msbot secret -n
```

This will generate a strong key, encrypt the bot file and print the key. Please keep this key securely.

Any time the bot file is encrypted, make sure to set the botFileSecret environment variable this sample relies on (either through the .env file or other means).

# Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://aka.ms/botframework-emulator)

## Connect to bot using Bot Framework Emulator **V4**
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to javascript_nodejs\14.nlp-with-dispatch folder
- Select nlp-with-dispatch.bot file
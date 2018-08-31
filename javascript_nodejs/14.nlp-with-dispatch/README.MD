[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)

This sample shows how to create a bot that relies on multiple [LUIS.ai](http://luis.ai) and [QnAMaker.ai](http://qnamaker.ai) models for natural language processing (NLP). 

This bot example uses [`restify`](https://www.npmjs.com/package/restify) and [`dotenv`](https://www.npmjs.com/package/dotenv)

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/Microsoft/botbuilder-js.git
    ```
- In a terminal, navigate to samples\2.echobot-with-counter
    ```bash
    cd samples\2.echobot-with-counter
    ```
- Point to the myget feed 
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
- Configure the required LUIS applications. To create a new LUIS application for the bot, 
- Create an account with [LUIS](https://luis.ai). If you already have an account, login to your account
- Click on your name on top right corner of the screen -> settings and grab your authroing key.

To create the LUIS application this bot needs and update the .bot file configuration, in a terminal, 
- Clone this repository
- Navigate to samples\14.nlp-with-dispatch
- Run the following command
```bash 
> luis import application --in cognitiveModels\homeautomation.luis --authoringKey <YOUR-LUIS-AUTHORING-KEY> --endpointBasePath https://westus.api.cognitive.microsoft.com/luis/api/v2.0 --msbot | msbot connect luis --stdin --name homeautomation-LUIS

> luis import application --in cognitiveModels\weather.luis --authoringKey <YOUR-LUIS-AUTHORING-KEY> --endpointBasePath https://westus.api.cognitive.microsoft.com/luis/api/v2.0 --msbot | msbot connect LUIS --stdin --name weather-LUIS
```

Note: Should you so desire, you can create the LUIS application in a region of your choice. See [here](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/luis-reference-regions) to learn more about available LUIS authoring regions. You can specify a different region (westus or westeurope or australiaeast) via https://**LUIS-Authoring-Region**.api.cognitive.microsoft.com/luis/api/v2.0 in the above command.

- To reset registry, you can do
    ```bash
    npm config set registry https://registry.npmjs.org/
    ```

# Configure the LUIS service


## Train and publish the LUIS models 
You need to train and publish the LUIS models that were created for this sample to work. You can do so using the following CLI commands

```bash
> msbot get service --name "homeautomation-LUIS" | luis train version --wait --stdin
> msbot get service --name "homeautomation-LUIS" | luis publish version --stdin
> msbot get service --name "weather-LUIS" | luis train version --wait --stdin
> msbot get service --name "weather-LUIS" | luis publish version --stdin
```

# Configure QnA Maker service
To create a new QnA Maker application for the bot, 
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/set-up-qnamaker-service-azure) to create a new QnA Maker Azure resource.
- Navigate to your QnA Maker resource -> keys and copy the subscription Key

To create the QnA Maker application this bot needs and update the .bot file configuration, in a terminal, 
- Navigate to samples\14.nlp-with-dispatch
- Run the following command
```bash
> qnamaker create kb --in cognitiveModels\sample.qna --subscriptionKey <YOUR-QNA-SUBSCRIPTION-KEY> --msbot | msbot connect qna --stdin --name simple-QnA
```
## Train and publish the QnA Maker KB
You need to train and publish the QnA Maker Knowledge Bases that were created for this sample to work. You can do so using the following CLI commands

```bash
> msbot get service --name "simple-QnA" | qnamaker publish kb --stdin
```

# Configure the Dispatch application
[Dispatch](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Dispatch) is a CLI tool that enables you to create a dispatch NLP model across the different LUIS applications and/ or QnA Maker Knowledge Bases you have for your bot. For this sample, you would have already created 2 LUIS applications (Home Automation and Weather) and one QnA Maker Knowledge base. 

To create a new dispatch model for these services and update the .bot file configuration, in a terminal,
- Navigate to samples\14.nlp-with-dispatch
- Run the following command
```bash
> dispatch create -b nlp-with-dispatch.bot | msbot connect dispatch --stdin --name bot-dispatch
```
# Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://aka.ms/botframework-emulator)

## Connect to bot using Bot Framework Emulator **V4**
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to samples\2.echobot-with-state folder
- Select echobot-with-state.bot file

## Prerequisites
-	<REQUIRED TOOLS, VERSIONS>


-	<STEPS TO GET SET UP WITH THE SAMPLE. E.g. RUN AN INCLUDED SCRIPT OR MANUALLY DO SOMETHING ETC>

NOTE: <ANY NOTES ABOUT THE PREREQUISITES OR ALTERNATE THINGS TO CONSIDER TO GET SET UP>

## Visual studio
-	<STEPS TO RUN THIS SAMPLE FROM VISUAL STUDIO>

## Visual studio code
-	<STEPS TO RUN THIS SAMPLE FROM VISUAL STUDIO CODE>

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

## Connect to bot using Bot Framework Emulator **V4**
- Launch Bot Framework Emulator
- File -> Open bot and navigate to samples\8.AspNetCore-LUIS-Bot folder
- Select AspNetCore-LUIS-Bot.bot file

# Further reading
-	<LINKS TO ADDITIONAL READING>
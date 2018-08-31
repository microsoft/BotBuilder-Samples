# Deploy connected services
This bot relies on [LUIS](https://luis.ai) and [QnAMaker](https://qnamaker.ai) cognitive services to function. You can use simple command line tools to get set up with all required services.

# Install required tools
To successfully setup and configure all services this bot depend on, you need to install the MSBOT, LUIS, QnAMaker, Ludown, Dispatch CLI tools. 

```bash
> npm i -g msbot luis-apis qnamaker ludown botdispatch
```

# Keeping track of service references using .bot file
You can keep track of all services that your bot depend on in the .bot file. You can use either the MSBOT CLI tool or user Bot Framework Emulator to manage your .bot file. 

# Configure the LUIS service
To create a new LUIS application for the bot, 
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
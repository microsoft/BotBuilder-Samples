# NLP with LUIS
Bot Framework v4 NLP with LUIS bot sample

This sample shows how to create a bot that uses Language Understanding (LUIS).


# Concepts introduced in this sample
[Language Understanding (LUIS)](https://www.luis.ai) is a cloud-based API service that applies custom machine-learning intelligence to a user's conversational, natural language text to predict overall meaning, and pull out relevant, detailed information.

In this sample, we demonstrate how to call LUIS to extract the intents from a user's message.

## Prerequisites
This samples requires prerequisites in order to run.
- [Required Prerequisites][41]


# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```
- In a console, navigate to s`amples/javascript_nodejs/12.nlp-with-luis`
    ```bash
    cd samples/javascript_nodejs/12.nlp-with-luis
    ```
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
    msbot clone services --name "nlp-with-luis" --luisAuthoringKey <LUIS-authoring-key> --code-dir "." --location westus --sdkLanguage "Node" --folder deploymentScripts/msbotClone --verbose
    ```
- Start the bot
    ```bash
    npm start
    ```

# Testing the bot using Bot Framework Emulator **v4**
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.1.0 or greater from [here](https://github.com/microsoft/botframework-emulator/releases)

## Connect to bot using Bot Framework Emulator **v4**
- Launch Bot Framework Emulator
- File -> Open Bot Configuration
- Navigate to `samples/javascript_nodejs/12.nlp-with-luis` folder
- Select `nlp-with-luis.bot` file


# LUIS
Language Understanding service (LUIS) allows your application to understand what a person wants in their own words. LUIS uses machine learning to allow developers to build applications that can receive user input in natural language and extract meaning from it.

# Optional Command-line Tools for LUIS
- Install the `ludown` CLI tool [here](https://aka.ms/using-ludown) to help describe language understanding components for your bot.
- Install the `luis` CLI tool [here](https://aka.ms/using-luis-cli) to create and manage your LUIS applications.



# Deploy the bot to Azure
After creating the bot and testing it locally, you can deploy it to Azure to make it accessible from anywhere.
To learn how, see [Deploy your bot to Azure][40] for a complete set of deployment instructions.


# Further reading
- [Using LUIS for Language Understanding](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&tabs=js)
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [LUIS documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/)

[40]: https://aka.ms/azuredeployment
[41]: ./PREREQUISITES.md

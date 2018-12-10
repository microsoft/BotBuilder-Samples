This sample shows how to create a bot that uses Language Understanding (LUIS) to extract the intents from a user's message.

# Concepts introduced in this sample
[Language Understanding (LUIS)](https://www.luis.ai) is a cloud-based API service that applies custom machine-learning intelligence to a user's conversational, natural language text to predict overall meaning, and pull out relevant, detailed information.

## Prerequisite
This sample requires prerequisites in order to run.
- [Required Prerequisites][41]


# To try this sample◊
- Clone the repository
    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```
- In a console, navigate to `samples/javascript_typescript/12.nlp-with-luis`
    ```bash
    cd samples/javascript_typescript/12.nlp-with-luis
    ```
- Install modules
    ```bash
    npm install
    ```
- Install modules
    ```bash
    npm install
    ```
- Start the bot
    ```bash
    npm start
    ```

- [Optional] Update the .env file under `samples/javascript_typescript/12.nlp-with-luis` with your `botFileSecret`
    For Azure Bot Service bots, you can find the `botFileSecret` under application settings.

# LUIS
Language Understanding service (LUIS) allows your application to understand what a person wants in their own words. LUIS uses machine learning to allow developers to build applications that can receive user input in natural language and extract meaning from it.


# Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator)

## Connect to bot using Bot Framework Emulator V4
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to `samples/javascript_typescript/12.nlp-with-luis` folder
- Select `nlp-with-luis.bot` file


# Deploy this bot to Azure
To clone this bot, perform the following:
- Collect your Luis Authoring Key from the the [LUIS portal](https://www.luis.ai) by selecting your name in the top right corner. Save this key for the next step.

- Run the following command from the project directory:
```bash
msbot clone services --name "<NAME>" --luisAuthoringKey "<YOUR AUTHORING KEY>" --folder deploymentScripts/msbotClone --location "e.g, westus" --appId <YOUR APP ID> --appSecret <YOUR APP SECRET PASSWORD>
```




### Train and publish the LUIS models
You need to train and publish the LUIS models that were created for this sample to work. You can do so using the following CLI commands

### Install `ludown` and `luis` CLI tools
- Install the `ludown` CLI tool [here](https://aka.ms/using-ludown) to help describe language understanding components for your bot.
- Install the `luis` CLI tool [here](https://aka.ms/using-luis-cli) to create and manage your LUIS applications.


```bash
msbot get "Reminders" | luis train version --wait --stdin
msbot get "Reminders" | luis publish version --stdin
```


# Further reading
- [Using LUIS for Language Understanding](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&tabs=js)
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [LUIS documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/)

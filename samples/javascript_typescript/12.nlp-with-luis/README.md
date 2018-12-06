This sample shows how to create a bot that uses Language Understanding (LUIS) to extract the intents from a user's message.

# Concepts introduced in this sample
[Language Understanding (LUIS)](https://www.luis.ai) is a cloud-based API service that applies custom machine-learning intelligence to a user's conversational, natural language text to predict overall meaning, and pull out relevant, detailed information.

## Prerequisites
### Set up LUIS
- Navigate to [LUIS portal](https://www.luis.ai).
- Click the `Sign in` button.
- Click on `My Apps`.
- Click on the `Import new app` button.
- Click on the `Choose File` and select [reminders.json](cognitiveModels/reminders.json) from the `BotBuilder-Samples/javascript_nodejs/12.nlp-with-luis/cognitiveModels` folder.
- Provide a name for the LUIS app
- Click on the `Train` button. To train your language model.
- Click on the `Publish` button.  To publish your trained language model.
- Update [nlp-with-luis.bot](nlp-with-luis.bot) file with your AppId, SubscriptionKey, Region and Version.
    You can find this information under "Keys and Endpoint" tab for your LUIS application at [LUIS portal](https://www.luis.ai).  For example, for https://westus.settingsapi.cognitive.microsoft.com/luis/v2.0/apps/{LuisAppID}?subscription-key={LuisSubscriptionKey}&verbose=true&timezoneOffset=0&q=

    The Version is listed on the page.
    Note: Enter either the "authoringKey" OR "subscriptionKey", not both.
    [See an example .bot service configuration for using LUIS here](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&tabs=js#configure-your-bot-to-use-your-luis-app).

- Update [index.js](index.js) and set the `LUIS_CONFIGURATION` value to match the `name` field in your service declaration.

- Update [nlp-with-luis.bot](nlp-with-luis.bot) file with your Authoring Key.
    You can find this under your user settings at [luis.ai](https://www.luis.ai).  Click on your name in the upper right hand corner of the portal, and click on the "Settings" menu option.

    NOTE: Once you publish your app on LUIS portal for the first time, it may take some time to go live.

- Your .bot file should now include an item in the services array that looks like this:

```javascript
{
    "type":"luis",
    "name":"<some name>",
    "appId":"<an app id>",
    "version":"<a version number>",
    "authoringKey":"<your authoring key>",
    "subscriptionKey":"<your subscription key>",
    "region":"<region>",
    "id":"<some number>"
}
```

### (Optional) Install `ludown` and `luis`
- Install the `ludown` CLI tool [here](https://aka.ms/using-ludown) to help describe language understanding components for your bot.
- Install the `luis` CLI tool [here](https://aka.ms/using-luis-cli) to create and manage your LUIS applications.


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

**NOTE**: You can obtain your `appId` and `appSecret` at the Microsoft's [Application Registration Portal](https://apps.dev.microsoft.com/)

**NOTE**: By default your Luis Applications will be deployed to your free starter endpoint. An Azure LUIS service will be deployed along with your bot but you must manually add and publish to it from the luis.ai portal and update your key in the .bot file.

-  Note the generated secret generated by MSBot.
- The secret key is used later in for the emulator and configuration.
```bash
The secret used to decrypt <NAME>.bot is:
XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX=
NOTE: This secret is not recoverable and you should store this secret in a secure place according to best security practices.
Your project may be configured to rely on this secret and you should update it as appropriate.
```
- Inspect Bot configuration file.
- The `msbot clone` command above generates a bot configuration file.
- The name of the bot configuration file is `<NAME>.bot`, where `<NAME>` is the name of your bot used in the `msbot clone` step.
- The configuration file can be loaded by the [Microsoft Bot Framework Emulator](https://aka.ms/botframeworkemulator).

- Update `src/index.ts`
Update the following line to add a prefix with the name of your bot (plus underscore '_').
```typescript
const LUIS_CONFIGURATION = '<NAME>_nlp-with-luis-LUIS';
```

**Alternately** you can configure the required services by following the steps below.

## Manually configure required services
### Configure the LUIS service
To create required LUIS applications for this sample bot,
- Create an account with [LUIS](https://www.luis.ai). If you already have an account, login to your account.
- Click on your name on top right corner of the screen -> settings and grab your authoring key.

To create the LUIS application this bot needs and update the `.bot` file configuration, in a terminal,
- Clone this repository
- Navigate to `samples/javascript_nodejs/12.nlp-with-luis`
- Run the following command

```bash
> ludown parse toluis --in cognitiveModels/reminders.lu -o cognitiveModels --out reminders.luis -n "Reminders" -d "Reminder LUIS application - Bot Builder Samples" --verbose

> luis import application --in cognitiveModels/reminders.luis --authoringKey <LUIS-AUTHORING-KEY> --region <LUIS-AUTHORING-REGION> --msbot | msbot connect luis --stdin
```

Note: You can create the LUIS applications in one of the [LUIS authoring regions](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/luis-reference-regions).
You can use a different region (`westus` or `westeurope` or `australiaeast`) by specifying them as `--region` value in the commands above.

### Train and publish the LUIS models
You need to train and publish the LUIS models that were created for this sample to work. You can do so using the following CLI commands

```bash
> msbot get "Reminders" | luis train version --wait --stdin
> msbot get "Reminders" | luis publish version --stdin
```

## Securing keys in your .bot file
Since your `.bot` file contains service IDs, subscription and authoring keys, its best to encrypt them. To encrypt the `.bot` file, you can run

```bash
msbot secret -n
```

This will generate a strong key, encrypt the bot file and print the key. Please keep this key securely.

Any time the bot file is encrypted, make sure to set the botFileSecret environment variable this sample relies on (either through the `.env` file or other means).

# Further reading
- [Using LUIS for Language Understanding](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&tabs=js)
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [LUIS documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/)

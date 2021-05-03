# Multilingual Bot

Bot Framework v4 multilingual bot sample

This sample will present the user with a set of cards to pick their choice of language. The user can either change language by invoking the option cards, or by entering the language code (_en_/_es_). The bot will then acknowledge the selection.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to translate incoming and outgoing text using a custom middleware and the [Microsoft Translator Text API](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/).

## Concepts introduced in this sample

Translation Middleware: We create a translation middleware that can translate text from bot to user and from user to bot, allowing the creation of multi-lingual bots.

The middleware is driven by user state. This means that users can specify their language preference, and the middleware automatically will intercept messages back and forth and present them to the user in their preferred language.

Users can change their language preference anytime, and since this gets written to the user state, the middleware will read this state and instantly modify its behavior to honor the newly selected preferred language.

The [Microsoft Translator Text API](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/), Microsoft Translator Text API is a cloud-based machine translation service. With this API you can translate text in near real-time from any app or service through a simple REST API call.
The API uses the most modern neural machine translation technology, as well as offering statistical machine translation technology.

## Prerequisites

- Java 1.8+
- Install [Maven](https://maven.apache.org/)
- An account on [Azure](https://azure.microsoft.com) if you want to deploy to Azure.
- [Microsoft Translator Text API key](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/translator-text-how-to-signup)

    To consume the Microsoft Translator Text API, first obtain a key following the instructions in the [Microsoft Translator Text API documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/translator-text-how-to-signup).

    Paste the key in the `TranslatorKey` setting in the `application.properties` file.

## To try this sample

- From the root of this project folder:
  - Build the sample using `mvn package`
  - Run it by using `java -jar .\target\bot-multilingual-sample.jar`

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the latest Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

### Creating a custom middleware

Translation Middleware: We create a translation middleware than can translate text from bot to user and from user to bot, allowing the creation of multilingual bots.
Users can specify their language preference, which is stored in the user state. The translation middleware translates to and from the user's preferred language.

### Microsoft Translator Text API

The [Microsoft Translator Text API](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/), Microsoft Translator Text API is a cloud-based machine translation service. With this API you can translate text in near real-time from any app or service through a simple REST API call.
The API uses the most modern neural machine translation technology, as well as offering statistical machine translation technology.

## Deploy this bot to Azure

As described on [Deploy your bot](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-deploy-az-cli), you will perform the first 4 steps to setup the Azure app, then deploy the code using the azure-webapp Maven plugin.

### 1. Login to Azure
From a command (or PowerShell) prompt in the root of the bot folder, execute:  
`az login`

### 2. Set the subscription
`az account set --subscription "<azure-subscription>"`

If you aren't sure which subscription to use for deploying the bot, you can view the list of subscriptions for your account by using `az account list` command.

### 3. Create an App registration
`az ad app create --display-name "<botname>" --password "<appsecret>" --available-to-other-tenants`

Replace `<botname>` and `<appsecret>` with your own values.

`<botname>` is the unique name of your bot.  
`<appsecret>` is a minimum 16 character password for your bot.

Record the `appid` from the returned JSON

### 4. Create the Azure resources
Replace the values for `<appid>`, `<appsecret>`, `<botname>`, and `<groupname>` in the following commands:

#### To a new Resource Group
`az deployment sub create --name "multilingualBotDeploy" --location "westus" --template-file ".\deploymentTemplates\template-with-new-rg.json" --parameters appId="<appid>" appSecret="<appsecret>" botId="<botname>" botSku=S1 newAppServicePlanName="multilingualBotPlan" newWebAppName="multilingualBot" groupLocation="westus" newAppServicePlanLocation="westus"`

#### To an existing Resource Group
`az deployment group create --resource-group "<groupname>" --template-file ".\deploymentTemplates\template-with-preexisting-rg.json" --parameters appId="<appid>" appSecret="<appsecret>" botId="<botname>" newWebAppName="multilingualBot" newAppServicePlanName="multilingualBotPlan" appServicePlanLocation="westus" --name "multilingualBot"`

### 5. Update app id and password
In src/main/resources/application.properties update
- `MicrosoftAppPassword` with the botsecret value
- `MicrosoftAppId` with the appid from the first step

### 6. Deploy the code
- Execute `mvn clean package`
- Execute `mvn azure-webapp:deploy -Dgroupname="<groupname>" -Dbotname="<bot-app-service-name>"`

If the deployment is successful, you will be able to test it via "Test in Web Chat" from the Azure Portal using the "Bot Channel Registration" for the bot.

After the bot is deployed, you only need to execute #6 if you make changes to the bot.

### Add `TranslatorKey` to Application Settings

If you used the `application.properties` file to store your `TranslatorKey` then you'll need to add this key and its value to the Application Settings for your deployed bot.

- Log into the [Azure portal](https://portal.azure.com)
- In the left nav, click on `Bot Services`
- Click the `<your_bot_name>` Name to display the bots Web App Settings
- Click the `Application Settings`
- Scroll to the `Application settings` section
- Click `+ Add new setting`
- Add the key `TranslatorKey` with a value of the Translator Text API `Authentication key` created from the steps above

## Further reading

- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Bot State](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-storage-concept?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Spring Boot](https://spring.io/projects/spring-boot)

# Table of Contents
[Concepts introduced in this sample](#concepts-introduced-in-this-sample)
[Prerequisites](#prerequisites)
[Configure Services](#configure-services)
[Run the Sample](#run-this-sample)
[Testing the bot using Bot Framework Emulator](#testing-the-bot-using-bot-framework-emulator)
[Deploy to Azure](#deploy-to-azure)
[Further Reading](#further-reading)

# Concepts introduced in this sample
<DESCRIPTION OF THE CONCEPTS>

- Services used in this sample
- [LUIS](https://luis.ai) for Natural Language Processing
- [QnA Maker](https://qnamaker.ai) for FAQ, chit-chat, getting help and other single-turn conversations
- [Dispatch](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-dispatch?view=azure-bot-service-4.0&tabs=csharp) For multiple QnA and/or LUIS applications
- [AppInsights](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-manage-analytics?view=azure-bot-service-4.0) For Telemetry in your Bot

# Prerequisites

###	Required Tools
- [.NET Core SDK](https://www.microsoft.com/net/download/dotnet-core/2.1) version 2.1.403 or higher
- [Node.js](https://www.microsoft.com/net/download/dotnet-core/2.1) version 8.5 or higher 
- [Azure CLI tools](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) (Optional)
- [Bot Builder Tools](https://github.com/Microsoft/BotBuilder-tools) (Optional)

### Clone the repo
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
## Configure Services

### Using CLI Tools
- If you have not already, you can install all of the Bot Builder Tools with this command:
```bash
npm install -g chatdown msbot ludown luis-apis qnamaker botdispatch luisgen
```
#### Create a .bot File
To create a .bot file, use the `init` command of MS bot.
```bash
msbot init --name {NAME OF YOUR BOT} --endpoint {ENDPOINT FOR YOUR BOT}
```
**NOTES:** 
- This sample is preconfigured to use http://localhost:3978/api/messages as the endpoint for local development.
- For additional options when generating a .bot file, see [here](https://github.com/Microsoft/botbuilder-tools/blob/master/packages/MSBot/docs/create-bot.md).
#### Add Services
**NOTE** If you want to add these services and provision the resources all in one step, please see the [Deploy to Azure](#deploy-to-azure) section.

##### [AppInsights](https://github.com/Microsoft/botbuilder-tools/blob/master/packages/MSBot/docs/bot-file-encryption.md)
```bash
msbot connect appinsights 
```
**NOTE** If you already have an app insights application, you will need to add options to the command which can be found [here](https://github.com/Microsoft/botbuilder-tools/blob/master/packages/MSBot/docs/add-services.md#connecting-to-azure-appinsights-service).
In this case the command might look something like this:
```bash
msbot connect appinsights --name {YOUR APP NAME} --tenantId {YOUR TENANT ID} --subscriptionId {YOUYR SUBSCRIPTION ID} --resourceGroup {YOUR RESOURCE GROUP NAME} --serviceName {YOUR SERVICE NAME} --instrumentationKey {YOUR INSTRUMENTATION KEY} --applicationId {YOUR APP INSIGHT APPLICATION ID}
```
##### [Dispatch](https://github.com/Microsoft/botbuilder-tools/blob/master/packages/MSBot/docs/add-services.md#connecting-to-bot-dispatch)
```bash
msbot connect dispatch --name {DISPATCH NAME} --appId {YOUR LUIS APP ID FOR DISPATCH} --version {VERSION NUMBER (ex: 0.1)} --subscriptionKey {YOUR SUBSCRIPTION KEY} ----authoringKey {YOUR AUTHORING KEY}
```

##### [QnA Maker](https://github.com/Microsoft/botbuilder-tools/blob/master/packages/MSBot/docs/add-services.md#connecting-to-qna-maker-knowledge-base)
```bash
msbot connect qna --secret EncryptItPlease --name "{QnA APP NAME}" --kbId {KB-ID} --subscriptionKey {KEY} --endpointKey {ENDPOINT-KEY} --hostname "https://{YOUR SITE}.azurewebsites.net"
```

##### [LUIS](https://github.com/Microsoft/botbuilder-tools/blob/master/packages/MSBot/docs/add-services.md#connecting-to-luis-application)
```bash
msbot connect luis --name "My Luis Model" --appId {APP-ID} --version v0.1 --authoringKey {AUTHORING-KEY}
```


#### Encrypt Keys in Your .bot file (Optional)
```bash
msbot secret --new
```
**NOTES:** 
- More information about and options for encrypting your .bot file can be found [here](https://github.com/Microsoft/botbuilder-tools/blob/master/packages/MSBot/docs/bot-file-encryption.md).
### Manual Setup Using Portal(s)
If you would not like to use the CLI tools to create your .bot file, you can manually create one and copy and paste your ID's and keys in from the portals for the services used.

Once you are finished, your .bot file should look similar to this:
```javascript
{
    "name": "",
    "description": "",
    "services": [
        {
            "type": "abs",
            "id": "100",
            "name": "",
            "tenantId": "7",
            "subscriptionId": "",
            "resourceGroup": "",
            "serviceName": "",
            "appId": ""
        },
      {
        "type": "endpoint",
        "id": "1",
        "name": "development",
        "appId": "",
        "appPassword": "",
        "endpoint": "http://localhost:3978/api/messages"
      },
        {
            "type": "blob",
            "id": "2",
            "name": "",
            "serviceName": "",
            "tenantId": "",
            "subscriptionId": "",
            "resourceGroup": "ent67",
            "connectionString": "",
            "container": ""
        },
        {
            "type": "appInsights",
            "tenantId": "",
            "subscriptionId": "",
            "resourceGroup": "",
            "name": "",
            "serviceName": "",
            "instrumentationKey": "",
            "applicationId": "",
            "apiKeys": {},
            "id": "3"
        },
        {
            "type": "cosmosdb",
            "id": "8",
            "name": "",
            "serviceName": "",
            "tenantId": "",
            "subscriptionId": "",
            "resourceGroup": "",
            "endpoint": "https://ent67.documents.azure.com:443/",
            "key": "",
            "database": "",
            "collection": ""
        },
        {
            "type": "generic",
            "id": "5",
            "name": "ContentModerator",
            "url": "",
            "configuration": {
                "key": "",
                "region": ""
            }
        },
        {
            "type": "generic",
            "id": "364",
            "name": "Authentication",
            "url": "",
            "configuration": {
                "Azure Active Directory v2": ""
            }
        },
        {
            "type": "luis",
            "name": "",
            "appId": "",
            "authoringKey": "",
            "subscriptionKey": "",
            "version": "0.1",
            "region": "westus",
            "id": "120"
        },
        {
            "type": "qna",
            "name": "",
            "id": "85",
            "kbId": "",
            "subscriptionKey": "",
            "endpointKey": "",
            "hostname": "https://{APP NAME}.azurewebsites.net/qnamaker"
        },
        {
            "type": "dispatch",
            "serviceIds": [
                "120",
                "85"
            ],
            "name": "",
            "appId": "",
            "authoringKey": "",
            "subscriptionKey": "",
            "version": "Dispatch",
            "region": "westus",
            "id": "152"
        }
    ],
    "padlock": "",
    "version": "2.0"
}
```

## Run this Sample

### Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/{SAMPLE NAME}`), and open {SAMPLE NAME}.csproj in Visual Studio.
- Run the project (press `F5` key).

### Visual Studio Code
- Open `botbuilder-samples/samples/csharp_dotnetcore/{SAMPLE NAME}` sample folder.
- Bring up a terminal, navigate to `botbuilder-samples/samples/csharp_dotnetcore/{SAMPLE NAME}` folder.
- Type `dotnet run`.

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to bot using Bot Framework Emulator **V4**
- Launch Bot Framework Emulator
- From the *File* menu select *Open Bot Configuration*
- Navigate to your `.bot` file

## Deploy to Azure

### Using CLI Tools
```bash
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id>
```
### Deploy from Visual Studio
- Right click on the project in viasual studio
- Select publish
- Follow the steps in the prompt
### Deprovision your bot
- To deprovision your bot using the CLI tools :
```bash
az group delete --name {RESOURCE GROUP NAME}
```
- To deprovision your bot using the Azure Portal:
    - Search for your resource group in the Azure portal.
    - Click on the "..." to the left side of the resource group.
    - Select delete and follow the prompt

# Further Reading
- <LINKS TO ADDITIONAL READING>

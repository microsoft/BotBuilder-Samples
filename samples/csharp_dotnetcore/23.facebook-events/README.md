

# Concepts introduced in this sample

This sample shows how to integrate and consume Facebook specific payloads, such as postbacks, quick replies and optin events. 
Since Bot Framework supports multiple Facebook pages for a single bot, we also show how to know the page to which the message was sent, so developers can have custom behavior per page.

# To try this sample

- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- [Optional] Update the `appsettings.json` file under `botbuilder-samples/samples/csharp_dotnetcore/23.facebook-events` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.
<<<<<<< HEAD
# Prerequisites
## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/23.facebook-events`) and open Facebook-Events-Bot.csproj in Visual Studio 
- Hit F5

## Visual Studio Code
- Open `botbuilder-samples/samples/csharp_dotnetcore/23.facebook-events` sample folder.
- Bring up a terminal, navigate to botbuilder-samples/samples/csharp_dotnetcore/23.Facebook-Events folder
- type 'dotnet run'
=======

# Running Locally

## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/23.facebook-events`) and open `Facebook-Events-Bot.csproj` in Visual Studio 
- Run the project (press `F5` key)

## .NET Core CLI
- Install the [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x). 
- Using the command line, navigate to `botbuilder-samples/samples/csharp_dotnetcore/23.Facebook-Events` folder
- type `dotnet run`
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframeworkemulator).

### Connect to bot using Bot Framework Emulator V4
- Launch Bot Framework Emulator
- File -> Open bot and navigate to `botbuilder-samples/samples/csharp_dotnetcore/23.facebook-events` folder
<<<<<<< HEAD
- Select BotConfiguration.bot file
=======
- Select `facebook-events.bot` file
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

### Enable Facebook Channel

The final step to test Facebook-specific features is to publish your bot for the Facebook channel. The Bot Framework makes this very easy,
and the detailed steps are explained in the [Bot Framework Channel Documentation](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-facebook?view=azure-bot-service-3.0).

# Deploy this bot to Azure
<<<<<<< HEAD
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. 

To install all Bot Builder tools - 

Ensure you have [Node.js](https://nodejs.org/) version 8.5 or higher

```bash
npm i -g msbot chatdown ludown qnamaker luis-apis botdispatch luisgen
```
To clone this bot, run
```
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id>
```
=======
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. In order to install this and other tools, you can read [Installing CLI Tools](../../../Installing_CLI_tools.md).

To clone this bot, run
```bash
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id> --appId <YOUR APP ID> --appSecret <YOUR APP SECRET PASSWORD>
```

**NOTE**: You can obtain your `appId` and `appSecret` at the Microsoft's [Application Registration Portal](https://apps.dev.microsoft.com/)

>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
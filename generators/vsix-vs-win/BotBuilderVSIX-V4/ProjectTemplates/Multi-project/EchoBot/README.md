# $safeprojectname$
DESCRIPTION

This bot has been created using [Microsoft Bot Framework][1], it shows how to create a simple bot that accepts input from the user and echoes it back.

# To run the bot

## Visual Studio
* Navigate to the folder containing the `.csproj` file and open it in Visual Studio.
* Run the project (press `F5` key)

## .NET Core CLI
* Install the [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x).
* Using the command line, navigate to your project's root folder.
* Type `dotnet run`.

## Interacting With Your Bot Using the Emulator
Launch the [Microsoft Bot Framework Emulator v4][4] and open the generated project's `.bot` file.

# Testing the bot using Bot Framework Emulator **v4**
[Microsoft Bot Framework Emulator][4] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

* Install the Bot Framework Emulator version 4.1.0 or greater from [here][6].

## Connect to the bot using Bot Framework Emulator **v4**
* Launch Bot Framework Emulator
* File -> Open Bot Configuration
* Navigate to $safeprojectname$ folder
* Select `$safeprojectname$.bot` file

# Deploy the bot to Azure
See [Deploy your C# bot to Azure][555] for instructions.

The deployment process assumes you have an account on Microsoft Azure and are able to log into the [Microsoft Azure Portal][577].

If you are new to Microsoft Azure, please refer to [Getting started with Azure][6] for guidance on how to get started on Azure.

# Further reading
* [Bot Framework Documentation][20]
* [Bot Basics][32]
* [Azure Bot Service Introduction][21]
* [Azure Bot Service Documentation][22]
* [Azure CLI][7]
* [msbot CLI][9]
* [Azure Portal][10]
* [Language Understanding using LUIS][11]
* [Restify][30]
* [dotenv][31]

[1]: https://dev.botframework.com
[2]: https://www.typescriptlang.org
[3]: https://www.typescriptlang.org/#download-links
[4]: https://github.com/microsoft/botframework-emulator
[6]: https://github.com/Microsoft/BotFramework-Emulator/releases
[7]: https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest
[8]: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest
[9]: https://github.com/Microsoft/botbuilder-tools/tree/master/packages/MSBot
[10]: https://portal.azure.com
[11]: https://luis.ai
[20]: https://docs.botframework.com
[21]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[22]: https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0
[30]: https://www.npmjs.com/package/restify
[31]: https://www.npmjs.com/package/dotenv
[32]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0

[50]: https://visualstudio.microsoft.com/downloads/
[51]: https://azure.microsoft.com/en-us/free/
[52]: https://docs.microsoft.com/en-us/powershell/azure/overview?

[66]: https://azure.microsoft.com/get-started/
[555]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-deploy-azure?view=azure-bot-service-4.0
[577]: https://portal.azure.com
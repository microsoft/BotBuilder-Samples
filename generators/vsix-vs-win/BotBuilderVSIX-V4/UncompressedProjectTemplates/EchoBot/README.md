# $safeprojectname$

This bot has been created using [Microsoft Bot Framework][1], it shows how to create a simple bot that accepts input from the user and echoes it back.

# To run the bot

## Visual Studio
* Navigate to the folder containing the `.csproj` file and open it in Visual Studio.
* Run the project (press `F5` key)

## .NET Core CLI
* Install the [.NET Core CLI tools][2].
* Using the command line, navigate to your project's root folder.
* Type `dotnet run`.

## Interacting With Your Bot Using the Emulator
Launch the [Microsoft Bot Framework Emulator v4][3] and open the `$safeprojectname$.bot` file.

# Testing the bot using Bot Framework Emulator **v4**
[Microsoft Bot Framework Emulator][3] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

* Install the Bot Framework Emulator version 4.1.0 or greater from [here][4].

## Connect to the bot using Bot Framework Emulator **v4**
* Launch Bot Framework Emulator
* File -> Open Bot Configuration
* Navigate to $safeprojectname$ folder
* Select `$safeprojectname$.bot` file

# Deploy the bot to Azure
See [Deploy your C# bot to Azure][5] for instructions.

The deployment process assumes you have an account on Microsoft Azure and are able to log into the [Microsoft Azure Portal][6].

If you are new to Microsoft Azure, please refer to [Getting started with Azure][7] for guidance on how to get started on Azure.

# Further reading
* [Bot Framework Documentation][8]
* [Bot Basics][9]
* [Azure Bot Service Introduction][10]
* [Azure Bot Service Documentation][11]
* [Azure CLI][12]
* [msbot CLI][13]
* [Azure Portal][14]
* [Language Understanding using LUIS][15]

[1]: https://dev.botframework.com
[2]: https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x
[3]: https://github.com/microsoft/botframework-emulator
[4]: https://github.com/Microsoft/BotFramework-Emulator/releases
[5]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-deploy-azure?view=azure-bot-service-4.0
[6]: https://portal.azure.com
[7]: https://azure.microsoft.com/get-started/
[8]: https://docs.botframework.com
[9]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[10]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[11]: https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0
[12]: https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest
[13]: https://github.com/Microsoft/botbuilder-tools/tree/master/packages/MSBot
[14]: https://portal.azure.com
[15]: https://luis.ai
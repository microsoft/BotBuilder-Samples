# __PROJECT_NAME__
Bot Framework v4 empty bot sample.

This bot has been created using [Bot Framework][1], it shows the minimum code required to build a bot.

## Prerequisites
- [.NET Core SDK][4] version __NETCORE_VERSION__
	```bash
	# determine dotnet version
	dotnet --version
	```

# To try this sample
- In a terminal, navigate to `__PROJECT_NAME__`
    ```bash
    # change into project folder
	cd __PROJECT_NAME__
    ```
- Run the bot from a terminal or from Visual Studio, choose option A or B.

	A) From a terminal
	```bash
	# run the bot
	dotnet run
	```

	B) Or from Visual Studio
	- Launch Visual Studio
	- File -> Open -> Project/Solution
	- Navigate to `__PROJECT_NAME__` folder
	- Select `__PROJECT_NAME__.csproj` file
	- Press `F5` to run the project

# Testing the bot using Bot Framework Emulator **v4**
[Bot Framework Emulator][5] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here][6]

## Connect to the bot using Bot Framework Emulator **v4**
- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

# Deploy the bot to Azure
To learn more about deploying a bot to Azure, see [Deploy your bot to Azure][40] for a complete list of deployment instructions.

# Further reading
- [Bot Framework Documentation][20]
- [Bot Basics][32]
- [Azure Bot Service Introduction][21]
- [Azure Bot Service Documentation][22]
- [.NET Core CLI tools][23]
- [Azure CLI][7]
- [msbot CLI][9]
- [Azure Portal][10]
- [Language Understanding using LUIS][11]

#### Generated with `dotnet new emptybot` vX.X.X

[1]: https://dev.botframework.com
[4]: https://dotnet.microsoft.com/download
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://github.com/Microsoft/BotFramework-Emulator/releases
[7]: https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest
[8]: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest
[9]: https://github.com/Microsoft/botbuilder-tools/tree/master/packages/MSBot
[10]: https://portal.azure.com
[11]: https://www.luis.ai
[20]: https://docs.botframework.com
[21]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[22]: https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0
[23]: https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x
[32]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[40]: https://aka.ms/azuredeployment

# Concepts introduced in this sample

This sample shows how to integrate and consume Facebook specific payloads, such as postbacks, quick replies, echos and optin events. 

Since Bot Framework supports multiple Facebook pages for a single bot, we also show how to know the page to which the message was sent, so developers can have custom behavior per page.

### Install .NET Core and CLI Tooling
- [.NET Core SDK][https://dotnet.microsoft.com/download] version 2.1
	```bash
	# determine dotnet version
	dotnet --version
	```
- If you don't have an Azure subscription, create a [free account][https://azure.microsoft.com/free/].
- Install the latest version of the [Azure CLI][https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest] tool. Version 2.0.54 or higher.

# To try this sample
- In a terminal, navigate to `23.facebook-events`
    ```bash
    # change into project folder
	cd 23.facebook-events
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
	- Navigate to `23.facebook-events` folder
	- Select `Facebook-Events-Bot.csproj` file
	- Press `F5` to run the project

## Testing the bot using Bot Framework Emulator
[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframeworkemulator).

### Connect to the bot using Bot Framework Emulator
- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

### Enable Facebook Channel

The final step to test Facebook-specific features is to publish your bot for the Facebook channel. The Bot Framework makes this very easy,
and the detailed steps are explained in the [Bot Framework Channel Documentation](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-facebook?view=azure-bot-service-3.0).

## Deploy the bot to Azure
To learn more about deploying a bot to Azure, see [Deploy your bot to Azure][https://aka.ms/azuredeployment] for a complete list of deployment instructions.

# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
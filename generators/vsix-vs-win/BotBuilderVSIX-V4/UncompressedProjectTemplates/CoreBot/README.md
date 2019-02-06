# $safeprojectname$

This bot has been created using [Bot Framework][1], it shows how to:
- Use [LUIS][11] to implement core AI capabilities
- Implement a multi-turn conversation using Dialogs
- Handle user interruptions for such things as `Help` or `Cancel`
- Prompt for and validate requests for information from the user

This sample requires prerequisites in order to run.
- [Required Prerequisites][41]

# To try this sample
- In a terminal, navigate to `<your_project_folder>/$safeprojectname$`
    ```bash
    # change into project folder
	cd <your_project_folder>/$safeprojectname$
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
    Before creating the Luis service application, is recommendable following the next steps:

    - Delete the auto-generated .bot file, as the `msbot clone services` command can't update an existing file.
    - As the command `msbot clone services` uses secret-manager to safely store the key generated for encrypting the bot file, it's necessary adding the next code in your $safeprojectname$.csproj file:

    ```
        <PropertyGroup>
            <UserSecretsId>GUID</UserSecretsId>
        </PropertyGroup>
    ```
    **Note**: You can add any arbitrary `GUID` value for the `UserSecretsId` property, but you can not repeat `GUID` values among the projects, as they must be unique.

    - Install Microsoft.Extensions.Configuration.UserSecrets NuGet package

    ```bash
    # Create LUIS service application
    msbot clone services --name "$safeprojectname$" --luisAuthoringKey <LUIS-authoring-key> --code-dir "." --location westus --sdkLanguage "CSharp" --folder deploymentScripts/msbotClone --verbose
    ```

    **Note**: Once the Luis service application is created, change the value of `LuisConfiguration` variable in $safeprojectname$Bot.cs with the Luis service name, which can be found in $safeprojectname$.bot file (it should be `$safeprojectname$_core-bot-LUIS`).

- Run the bot from a terminal or from Visual Studio, choose option A or B.

	A) From a terminal
	```bash
	# run the bot
	dotnet run
	```

	B) Or from Visual Studio
	- Launch Visual Studio
	- File -> Open -> Project/Solution
	- Navigate to `<your_project_folder>/$safeprojectname$` folder
	- Select `$safeprojectname$.csproj` file
	- Press `F5` to run the project

# Testing the bot using Bot Framework Emulator **v4**
[Bot Framework Emulator][5] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.2.0 or greater from [here][6]

## Connect to the bot using Bot Framework Emulator **v4**
- Launch Bot Framework Emulator
- File -> Open Bot Configuration
- Navigate to `<your_project_folder>/$safeprojectname$` folder
- Select `$safeprojectname$.bot` file

# Deploy the bot to Azure
## Prerequisites
- [Azure Deployment Prerequisites][41]

## Provision a Bot with Azure Bot Service
After creating the bot and testing it locally, you can deploy it to Azure to make it accessible from anywhere.  To deploy your bot to Azure:

```bash
# login to Azure
az login
```

```bash
# set you Azure subscription
az account set --subscription "<azure-subscription>"
```

```bash
# provision Azure Bot Services resources to host your bot
msbot clone services --name "$safeprojectname$" --code-dir "." --location westus --sdkLanguage "Csharp" --folder deploymentScripts/msbotClone --verbose
```

### Publishing Changes to Azure Bot Service
As you make changes to your bot running locally, and want to deploy those change to Azure Bot Service, you can _publish_ those change using either `publish.cmd` if you are on Windows or `./publish` if you are on a non-Windows platform.  The following is an example of publishing

```bash
# run the publish helper (non-Windows) to update Azure Bot Service.  Use publish.cmd if running on Windows
./publish
```

### Getting Additional Help Deploying to Azure
To learn more about deploying a bot to Azure, see [Deploy your bot to Azure][40] for a complete list of deployment instructions.

# Further reading
- [Bot Framework Documentation][20]
- [Bot Basics][32]
- [Prompt types][23]
- [Waterfall dialogs][24]
- [Ask the user questions][26]
- [Activity processing][25]
- [Azure Bot Service Introduction][21]
- [Azure Bot Service Documentation][22]
- [.NET Core CLI tools][23]
- [Azure CLI][7]
- [msbot CLI][9]
- [Azure Portal][10]
- [Language Understanding using LUIS][11]
- [Channels and Bot Connector Service][27]

#### Generated with `dotnet new corebot` vX.X.X

[1]: https://dev.botframework.com
[4]: https://dotnet.microsoft.com/download
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://github.com/Microsoft/BotFramework-Emulator/releases
[7]: https://docs.microsoft.com/cli/azure/?view=azure-cli-latest
[8]: https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest
[9]: https://github.com/Microsoft/botbuilder-tools/tree/master/packages/MSBot
[10]: https://portal.azure.com
[11]: https://www.luis.ai
[20]: https://docs.botframework.com
[21]: https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[22]: https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0
[23]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0
[24]: https://docs.microsoft.com/en-us/javascript/api/botbuilder-dialogs/waterfall
[25]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0
[26]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-waterfall?view=azure-bot-service-4.0
[27]: https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0
[30]: https://www.npmjs.com/package/restify
[31]: https://www.npmjs.com/package/dotenv
[32]: https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[40]: https://aka.ms/azuredeployment
[41]: ./PREREQUISITES.md

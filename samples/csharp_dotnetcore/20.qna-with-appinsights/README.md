<<<<<<< HEAD
﻿This sample shows how to integrate QnA Maker in a simple bot with ASP.Net Core 2 and Application Insights. 

# Concepts introduced in this sample
The [QnA Maker Service](https://www.qnamaker.ai) enables you to build, train and publish a simple question
and answer bot based on FAQ URLs, structured documents or editorial content in minutes.

The [Application Insights](https://azure.microsoft.com/en-us/services/application-insights/) enables you to discover actionable insights through application performance management and instant analytics.

In this sample, we demonstrate how to use the QnA Maker service to answer questions based on a FAQ text file as input.

# To try this sample

- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- [Optional] Update the `appsettings.json` file under `botbuilder-samples/samples/csharp_dotnetcore/20.qna-with-appinsights` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.

## Prerequisites
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/set-up-qnamaker-service-azure)
to create a QnA Maker service.
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/how-to/migrate-knowledge-base) to
import the [sample.qna](sample.qna) to your newly created QnA Maker service.
- Update [BotConfiguration.bot](BotConfiguration.bot) with your kbid (KnowledgeBase Id) and endpointKey in the "qna" services section. You can find this
information under "Settings" tab for your QnA Maker Knowledge Base at [QnAMaker.ai](https://www.qnamaker.ai).
- Name of your QnA Bot should be "QnABot".
- Follow instructions [here](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-asp-net-core) to set up your Application Insights service.
  - Note: The Application Insights will automatically update the [appsettings.json](appsettings.json) file.
- (Optional) Follow instructions [here](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker) to set up the
Qna Maker CLI to deploy the model.

## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/20.qna-with-appinsights`) and open QnABot.csproj in Visual Studio.
- In Visual Studio right click on the solution and select "Restore NuGet Packages".
- Run the project (press `F5` key)

## Visual Studio Code
- Open `botbuilder-samples/samples/csharp_dotnetcore/20.qna-with-appinsights` sample folder
- Bring up a terminal, navigate to `botbuilder-samples/samples/csharp_dotnetcore/20.qna-with-appinsights` folder.
- In the Visual Studio Code terminal type `dotnet restore`.
- Type `dotnet run`.

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframeworkemulator).

## Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator
- File -> Open bot and navigate to `botbuilder-samples/samples/csharp_dotnetcore/20.qna-with-appinsights` folder.
- Select the BotConfiguration.bot file.

# Deploy this bot to Azure
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. In order to install this and other tools, you can read [Installing CLI Tools](../../../Installing_CLI_tools.md).

To clone this bot, run
```bash
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id>
```

# Further reading

- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [QnA Maker documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/overview/overview)
- [QnA Maker command line tool](https://github.com/Microsoft/botbuilder-tools/tree/master/packages/QnAMaker)
- [Application Insights](https://azure.microsoft.com/en-us/services/application-insights/)
=======
﻿# QnA with Application Insights
This sample demonstrates how to integrate QnA Maker to a bot with ASP.Net Core 2 and
Application Insights.

This bot has been created using [Microsoft Bot Framework][1].

This samples shows how to:
- Use [QnA Maker][11] to implement core AI capabilities
- How to use Middleware to log messages to Application Insights.
- How to log QnA Maker results to Application Insights.
- View metrics using a PowerBI report, Azure Monitor queries or Visual Studio


## Prerequisites
This samples requires prerequisites in order to run.
- [Required Prerequisites][41]

# To run the bot
- Setup QnA Maker
    Assuming prerequisites have been installed:
    ```bash
    # log into Azure
    az login
    ```
    ```bash
    # set you Azure subscription
    az account set --subscription "<azure-subscription>"
    ```
    ```bash
    # Create QnA Maker service application
    msbot clone services --name "<your-bot-name>" --location <azure region like eastus, westus, westus2 etc.> --folder "DeploymentScripts/MsbotClone" --verbose
    ```

- Start the bot
   - If using Visual Studio:
      - Navigate to the samples folder (`botBuilder-samples\samples\csharp_dotnetcore\20.qna-with-appinsights`) and open `QnAABotAppInsights.csproj` in Visual Studio.
      - Run the project (press `F5` key)

   - If using .NET Core CLI:
      - Using the command line, navigate to `botBuilder-samples\samples\csharp_dotnetcore\20.qna-with-appinsights` folder.
      - Type `dotnet run`.

# Testing the bot using Bot Framework Emulator **v4**
[Microsoft Bot Framework Emulator][5] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.1.0 or greater from [here][6]

## Connect to the bot using Bot Framework Emulator **v4**
- Launch Bot Framework Emulator
- File -> Open Bot Configuration
- Navigate to `botBuilder-samples\samples\csharp_dotnetcore\20.qna-with-appinsights` folder
- Select `<your-bot-name>.bot` file

# Deploy the bot to Azure
After creating the bot and testing it locally, you can deploy it to Azure to make it accessible from anywhere.
To learn how, see [Deploy your bot to Azure][40] for a complete set of deployment instructions.

# View metrics
- Learn how to use [PowerBI, Azure Monitor queries and Visual Studio][42] to view Application Insights data

# Further reading
- [Bot Framework Documentation][20]
- [Bot Basics][32]
- [Azure Bot Service Introduction][21]
- [Azure Bot Service Documentation][22]
- [Deploying Your Bot to Azure][40]
- [Azure CLI][7]
- [msbot CLI][9]
- [Azure Portal][10]
- [QnA Maker][11]


[1]: https://dev.botframework.com
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://github.com/Microsoft/BotFramework-Emulator/releases
[7]: https://docs.microsoft.com/cli/azure/?view=azure-cli-latest
[8]: https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest
[9]: https://github.com/Microsoft/botbuilder-tools/tree/master/packages/MSBot
[10]: https://portal.azure.com
[11]: https://www.qnamaker.ai
[20]: https://docs.botframework.com
[21]: https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[22]: https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0
[32]: https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[40]: https://aka.ms/azuredeployment
[41]: ./PREREQUISITES.md
[42]: https://aka.ms/botPowerBiTemplate
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

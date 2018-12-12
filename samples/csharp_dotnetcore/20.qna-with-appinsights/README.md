# QnA with Application Insights
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
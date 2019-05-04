# Multi-turn dialog for QnA Maker
This sample demonstrates a multi-turn dialog for QnA Maker bot with ASP.Net Core 2.

#Concepts introduced in this sample
The [QnA Maker Service][7] enables you to build, train and publish a multi-turn question and answer bot based on FAQ URLs, structured documents or editorial content in minutes.
In this sample, we demonstrate how to a create a multi-turn dialog to interact with your knowledge base.

# Prerequisites
- Follow the documentation [here](https://aka.ms/qnamakermultiturn) to understand the QnA Maker multi-turn scenario and create a QnA Maker Service.


#To try this sample
- Clone the samples repository
- Update [.env](.env) in the sample with your QnA Maker endpoint details (QnAEndpointHostName, QnAAuthKey and QnAKnowledgebaseId). You can find this
information under "Settings" tab of your QnA Maker Knowledge Base at [QnAMaker.ai](https://www.qnamaker.ai)

# Running Locally
- Clone the repository
    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- Navigate to the sample directory
    
- Install modules
    ```bash
    npm install
    ```
- Start the bot
    ```bash
    npm start
    ```

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator][5] is a desktop application that allows bot 
developers to test and debug their bots on localhost or running remotely through a tunnel.
- Install the [Bot Framework emulator][6].

## Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator.
- File -> New Bot Configuration (Provide your local bot endpoint details).

# Deploy the bot to Azure
See [Deploy your C# bot to Azure][50] for instructions.

If you are new to Microsoft Azure, please refer to [Getting started with Azure][70] for guidance on how to get started on Azure.

# Further reading
* [Bot Framework Documentation][80]
* [Bot Basics][90]
* [Azure Bot Service Introduction][100]
* [Azure Bot Service Documentation][110]
* [Azure Portal][140]

[1]: https://dev.botframework.com
[2]: https://docs.microsoft.com/en-us/visualstudio/releasenotes/vs2017-relnotes
[3]: https://dotnet.microsoft.com/download/dotnet-core/2.1
[4]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[5]: https://github.com/microsoft/botframework-emulator
[6]: https://aka.ms/botframeworkemulator
[7]: https://www.qnamaker.ai

[50]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-deploy-azure?view=azure-bot-service-4.0
[60]: https://portal.azure.com
[70]: https://azure.microsoft.com/get-started/
[80]: https://docs.botframework.com
[90]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0
[100]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[110]: https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0
[120]: https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest
[130]: https://github.com/Microsoft/botbuilder-tools/tree/master/packages/MSBot
[140]: https://portal.azure.com
[150]: https://www.luis.ai
# Declarative bot

This sample demonstrates using [Adaptive dialog][1],  [Language Generation][2] features with Orchestrator to demonstrate an end-to-end Alarm bot in action.

This sample demonstrates use of Orchestrator as a recognizer with [Adaptive Dialogs](https://aka.ms/adaptive-dialogs)

## Prerequisites

This sample **requires** prerequisites in order to run.
- Bot project must target x64 platform
- Install latest supported version of [Visual C++](https://support.microsoft.com/en-gb/help/2977003/the-latest-supported-visual-c-downloads)
- Install latest [Bot Framework Emulator](https://github.com/microsoft/BotFramework-Emulator/releases)
- [.NET Core SDK](https://aka.ms/dotnet-core-applaunch?framework=Microsoft.AspNetCore.App&framework_version=3.1.0&arch=x64&rid=win10-x64) version 3.1
  ```bash
  > dotnet --version
  ```
- Install BF CLI with Orchestrator plugin
    - Install bf cli 
    ```bash
    > npm i -g @microsoft/botframework-cli
    ```
    - Install bf orchestrator
    ```bash
    > bf plugins:install @microsoft/bf-orchestrator-cli@beta
    ```
    - Make sure bf orchestrator command is working and shows all available orchestrator commands
    ```bash
    > bf orchestrator
    ```
    
## To try this bot sample

- Clone the repository
    ```bash
    > git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- CD experimental/orchestrator/csharp_dotnetcore/03.declarative-bot
    ```bash
    > cd experimental/orchestrator/csharp_dotnetcore/03.declarative-bot
    ```
- Configure Orchestrator: Download NLR model
    - You can view list of available models using this command.  Copy Version Id value from latest model and use it for --versionId parameter of the orchestrator:nlr:get command below.
    ```bash
    > bf orchestrator:nlr:list
    ```
    - Download the NLR model
    ```bash
    > mkdir model
    > bf orchestrator:nlr:get --versionId <version id> --out ./model --verbose
    ```
    - Build the Orchestrator snapshot
    ```bash
    > mkdir generated
    > bf orchestrator:build --dialog --in ./Dialogs --out ./generated --model ./model
    ```
- Run the bot from a terminal or from Visual Studio, choose option A or B.
    A) From a terminal

    ```bash
    > cd experimental/orchestrator/csharp_dotnetcore/03.declarative-bot
    > dotnet run
    ```
    B) Or from Visual Studio

    - Launch Visual Studio
    - File -> Open -> Project/Solution
    - Navigate to `Orchestrator` folder
    - Select `OrchestratorSamples.sln` file
    - Right click on `03.declarative-bot` project in the solution and 'Set as Startup Project'
    - Press `F5` to run the project

## Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Further reading
- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)

[1]:https://aka.ms/adaptive-dialogs
[2]:https://aka.ms/language-generation
[3]:../../../../samples/csharp_dotnetcore/06.using-cards
[4]:https://botbuilder.myget.org/gallery/botbuilder-declarative
[5]:https://luis.ai
[6]:#LUIS-Setup
[7]:https://github.com/Microsoft/botbuilder-tools
[8]:https://nodejs.org/en/
[9]:https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-how-to-account-settings#authoring-key
[10]:https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-concept-keys
[extension]:https://marketplace.visualstudio.com/items?itemName=tomlm.vscode-dialog-debugger
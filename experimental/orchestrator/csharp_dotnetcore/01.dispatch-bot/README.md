# NLP with Orchestrator

Bot Framework v4 NLP with Orchestrator bot sample

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a bot that relies on multiple [LUIS.ai](https://www.luis.ai) and [QnAMaker.ai](https://www.qnamaker.ai) models for natural language processing (NLP).

Use the Orchestrator Dispatch model in cases when:

- Your bot consists of multiple language modules (LUIS + QnA) and you need assistance in routing user's utterances to these modules in order to integrate the different modules into your bot.
- Create a text classification model from text files.

## Prerequisites

This sample **requires** prerequisites in order to run.

### Overview

This bot uses Orchestrator to route user utterances to multiple LUIS models and QnA maker services to support multiple conversational scenarios.

### Install .NET Core CLI
- [AzCopy](https://docs.microsoft.com/azure/storage/common/storage-use-azcopy-v10) (Place azcopy somewhere on the system path)
- [.NET Core SDK](https://aka.ms/dotnet-core-applaunch?framework=Microsoft.AspNetCore.App&framework_version=3.1.0&arch=x64&rid=win10-x64) version 3.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

### Install BF CLI tool with Orchestrator plugin
- Set npm registry to private orchestrator npm registry
   ```bash
   npm config set registry https://botbuilder.myget.org/F/orchestrator/auth/cabd1414-7df4-4995-a945-6a8fc73b1c60/npm/
   ```
- Install bf cli 
   ```bash
   npm i -g @microsoft/botframework-cli
   ```

- Install bf orchestrator
   ```bash
   bf plugins:install @microsoft/bf-orchestrator
   ```

- Make sure bf orchestrator command is working and shows all available orchestrator commands
   ```bash
   bf orchestrator
   ```

- Reset npm registry back to npmjs
   ```bash
   npm config set registry https://registry.npmjs.org/
   ```


### To try this bot sample

- Clone the repository at https://fuselabs.visualstudio.com/_git/Orchestrator

    ```bash
    git clone https://fuselabs.visualstudio.com/DefaultCollection/Orchestrator/_git/Orchestrator
    ```

- To configure LUIS models and QnA Maker service used in this sample, refer to the steps found [here](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-dispatch?view=azure-bot-service-4.0). 

    - Only complete sections [Create LUIS apps](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-dispatch?view=azure-bot-service-4.0&tabs=cs#create-luis-apps) and [Create QnA Maker knowledge base](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-dispatch?view=azure-bot-service-4.0&tabs=cs#create-qna-maker-knowledge-base).
    - For convenience, use the `DispatchBot/CognitiveModels/HomeAutomation.json` and `DispatchBot/CognitiveModels/Weather.json` files in this sample for creating LUIS applications.
    - For convenience, use the `DispatchBot/CognitiveModels/QnAMaker.tsv` file in this sample for QnA Maker application.
    - Update settings in `DispatchBot/appsettings.json`


- Run the bot from a terminal or from Visual Studio, choose option A or B.

    A) From a terminal

    ```bash
    cd Orchestrator\Samples\DispatchBot
    # run the bot
    dotnet run
    ```

    B) Or from Visual Studio

    - Launch Visual Studio
    - File -> Open -> Project/Solution
    - Navigate to `Orchestrator` folder
    - Select `OrchestratorSamples.sln` file
    - Right click on `DispatchBot` project in the solution and 'Set as Startup Project'
    - Press `F5` to run the project

#### Working with BF Orchestrator CLI
##### Create Orchestrator example file to be used for bot run time
- Navigate to Orchestrator repo folder
    ```bash
    cd <repo_root>\Orchestrator
    ```
    
- Create Orchestrator example file (all on a single line, where <repo_root> is the root of your clone).

    ```bash
    bf orchestrator:create \
    -i .\Samples\DispatchBot\CognitiveModels \
    -o .\Samples\DispatchBot\CognitiveModels \
    -m .\nlr --hierarchical 
    ```

    The output should resemble:

    ```bash
    "Processing: filepath:d:\\src\\Orchestrator\\Samples\\DispatchBot\\CognitiveModels""Processing d:\\src\\Orchestrator\\Samples\\DispatchBot\\CognitiveModels\\HomeAutomation.json...\n""Processing d:\\src\\Orchestrator\\Samples\\DispatchBot\\CognitiveModels\\Weather.json...\n"2020-04-07 09:42:39.1020753 [W:onnxruntime:, graph.cc:2412 onnxruntime::Graph::CleanUnusedInitializers] Removing initializer 'model._dumpy'. It is not used by any node and should be removed from the model.
    2020-04-07 09:42:39.1024190 [W:onnxruntime:, graph.cc:2412 onnxruntime::Graph::CleanUnusedInitializers] Removing initializer 'model.encoder.pooler.dense.weight'. It is not used by any node and should be removed from the model.
    2020-04-07 09:42:39.1101893 [W:onnxruntime:, graph.cc:2412 onnxruntime::Graph::CleanUnusedInitializers] Removing initializer 'model.encoder.pooler.dense.bias'. It is not used by any node and should be removed from the model.
    Model:examplesAdded = 74
    Model:lineNumber = 75
    LabelResolverExtensions:examplesAdded = 74
    ```


- Once create commands is completed, the example file *orchestrator.blu* should be created in the output folder specified in the above command.  (ie, `DispatchBot/CognitiveModels/orchestrator.blu`).

##### Evaluate Orchestrator train data
  Execute following, on a single line, where <repo_root> is the root of your clone and <output_dir> is a location to store the evaluation report.
```bash
bf orchestrator:evaluate \
-i .\Samples\DispatchBot\CognitiveModels\orchestrator.blu \
-o <output_folder> \
-m .\nlr 
```

##### Evaluate Orchestrator test data (optional, if test set available)
   
    bf orchestrator:test -i <path_to_blu_or_tsv_file> -t <path_to_test_tsv_file> -o <output_folder> -m <repo_root>\Orchestrator\nlr 
    
##### Predict Orchestrator test data (optional, interactive)

    bf orchestrator:predict -i <path_to_blu_or_tsv_file>  -m <repo_root>\Orchestrator\nlr

##### Finetune Orchestrator model (optional, if needed)

    bf orchestrator:finetune -i <test files folder> -o <output folder>

#### Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

##### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`


## Where to file bugs

Please file bugs here: https://fuselabs.visualstudio.com/Orchestrator/_workitems/create/Bug

## Deploy the bot to Azure 

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

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


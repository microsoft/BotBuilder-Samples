# SimpleBotToBot Echo Skill

Bot Framework v4 skills echo sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple RootBot that sends message activities to a SkillBot that echoes it back.

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## Key concepts in this sample

The solution includes a parent bot (`SimpleRootBot`) and a skill bot (`EchoSkillBot`) and shows how the parent bot can post activities to the skill bot.

- `SimpleRootBot`: this project shows how to consume an echo skill and includes:
  - A [RootBot](SimpleRootBot/Bots/RootBot.cs) that calls the echo skill and keeps the conversation active until the user says "end" or "stop". [RootBot](SimpleRootBot/Bots/RootBot.cs) also keeps track of the conversation with the skill and handles the `EndOfConversation` activity received from the skill to terminate the conversation
  - A simple [SkillConversationIdFactory](SimpleRootBot/SkillConversationIdFactory.cs) based on an in memory `ConcurrentDictionary` that creates and maintains conversation IDs used to interact with a skill
  - A [SkillsConfiguration](SimpleRootBot/SkillsConfiguration.cs) class that can load skill definitions from `appsettings`
  - A [SkillController](SimpleRootBot/Controllers/SkillController.cs) that handles skill responses
  - An [AllowedSkillsClaimsValidator](SimpleRootBot/Authentication/AllowedSkillsClaimsValidator.cs) class that is used to authenticate that responses sent to the bot are coming from the configured skills
  - A [Startup](SimpleRootBot/Startup.cs) class that shows how to register the different skill components for dependency injection
- `EchoSkillBot`: this project shows a simple echo skill that receives message activities from the parent bot and echoes what the user said. This project includes:
  - A sample [EchoBot](EchoSkillBot/Bots/EchoBot.cs) that shows how to send EndOfConversation based on the message sent to the skill and yield control back to the parent bot
  - A sample [AllowedCallersClaimsValidator](EchoSkillBot/Authentication/AllowedCallersClaimsValidator.cs) that shows how validate that the skill is only invoked from a list of allowed callers
  - A [sample skill manifest](EchoSkillBot/wwwroot/manifest/echoskillbot-manifest-1.0.json) that describes what the skill can do

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- Create a bot registration in the azure portal for the `EchoSkillBot` and update [EchoSkillBot/appsettings.json](EchoSkillBot/appsettings.json) with the `MicrosoftAppId` and `MicrosoftAppPassword` of the new bot registration
- Create a bot registration in the azure portal for the `SimpleRootBot` and update [SimpleRootBot/appsettings.json](SimpleRootBot/appsettings.json) with the `MicrosoftAppId` and `MicrosoftAppPassword` of the new bot registration
- Update the `BotFrameworkSkills` section in [SimpleRootBot/appsettings.json](SimpleRootBot/appsettings.json) with the `AppId` for the skill you created in the previous step
- (Optionally) Add the `SimpleRootBot` `MicrosoftAppId` to the `AllowedCallers` list in [EchoSkillBot/appsettings.json](EchoSkillBot/appsettings.json) 
- Open the `SimpleBotToBot.sln` solution and configure it to [start debugging with multiple processes](https://docs.microsoft.com/en-us/visualstudio/debugger/debug-multiple-processes?view=vs-2019#start-debugging-with-multiple-processes)


## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.7.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`, the `MicrosoftAppId` and `MicrosoftAppPassword` for the `SimpleRootBot`

## Deploy the bots to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

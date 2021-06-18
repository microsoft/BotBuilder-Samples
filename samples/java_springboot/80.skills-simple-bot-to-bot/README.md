# SimpleBotToBot Echo Skill

Bot Framework v4 skills echo sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple skill consumer (SimpleRootBot) that sends message activities to a skill (EchoSkillBot) that echoes it back.

This sample is a Spring Boot app and uses the Azure CLI and azure-webapp Maven plugin to deploy to Azure.

## Prerequisites

- Java 1.8+
- Install [Maven](https://maven.apache.org/)
- An account on [Azure](https://azure.microsoft.com) if you want to deploy to Azure.

## Key concepts in this sample

The solution includes a parent bot (`SimpleRootBot`) and a skill bot (`EchoSkillBot`) and shows how the parent bot can post activities to the skill bot and returns the skill responses to the user.

- `SimpleRootBot`: this project shows how to consume an echo skill and includes:
  - A [RootBot](SimpleRootBot/src/main/java/com/microsoft/bot/sample/simplerootbot/RootBot.java) that calls the echo skill and keeps the conversation active until the user says "end" or "stop". [RootBot](SimpleRootBot/src/main/java/com/microsoft/bot/sample/simplerootbot/RootBot.java) also keeps track of the conversation with the skill and handles the `EndOfConversation` activity received from the skill to terminate the conversation
  - A simple [SkillConversationIdFactory](SimpleRootBot/src/main/java/com/microsoft/bot/sample/simplerootbot/SkillConversationIdFactory.java) based on an in memory `Map` that creates and maintains conversation IDs used to interact with a skill
  - A [SkillsConfiguration](SimpleRootBot/src/main/java/com/microsoft/bot/sample/simplerootbot/SkillsConfiguration.java) class that can load skill definitions from `appsettings`
  - A [SkillController](SimpleRootBot/src/main/java/com/microsoft/bot/sample/simplerootbot/controller/SkillController.java) that handles skill responses
  - An [AllowedSkillsClaimsValidator](SimpleRootBot/src/main/java/com/microsoft/bot/sample/simplerootbot/Authentication/AllowedSkillsClaimsValidator.java) class that is used to authenticate that responses sent to the bot are coming from the configured skills
  - A [Application](SimpleRootBot/src/main/java/com/microsoft/bot/sample/simplerootbot/Application.java) class that shows how to register the different skill components for dependency injection
- `EchoSkillBot`: this project shows a simple echo skill that receives message activities from the parent bot and echoes what the user said. This project includes:
  - A sample [EchoBot](EchoSkillBot/src/main/java/com/microsoft/echoskillbot/EchoBot.java) that shows how to send EndOfConversation based on the message sent to the skill and yield control back to the parent bot
  - A sample [AllowedCallersClaimsValidator](EchoSkillBot/src/main/java/com/microsoft/echoskillbot/authentication/AllowedCallersClaimsValidator.java) that shows how validate that the skill is only invoked from a list of allowed callers
  - A [sample skill manifest](EchoSkillBot/src/main/webapp/manifest/echoskillbot-manifest-1.0.json) that describes what the skill can do

## To try this sample
- Create a bot registration in the azure portal for the `EchoSkillBot` and update [EchoSkillBot/application.properties](EchoSkillBot/src/main/resources/application.properties) with the `MicrosoftAppId` and `MicrosoftAppPassword` of the new bot registration
- Create a bot registration in the azure portal for the `SimpleRootBot` and update [SimpleRootBot/application.properties](SimpleRootBot/src/main/resources/application.properties) with the `MicrosoftAppId` and `MicrosoftAppPassword` of the new bot registration
- Update the `BotFrameworkSkills` section in [SimpleRootBot/application.properties](SimpleRootBot/src/main/resources/application.properties) with the app ID for the skill you created in the previous step
- (Optionally) Add the `SimpleRootBot` `MicrosoftAppId` to the `AllowedCallers` list in [EchoSkillBot/application.properties](EchoSkillBot/src/main/resources/application.properties) 
- Open the `SimpleBotToBot` project and start it for debugging
- Open the `EchoSkillsBot` project and start it for debugging

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.7.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`, the `MicrosoftAppId` and `MicrosoftAppPassword` for the `SimpleRootBot`

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

# SimpleBotToBot (**DRAFT**)

Bot Framework v4 Skills Echo sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to use skills from a rootbot.

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## Key concepts

- A [SimpleRootBot](SimpleRootBot/README.md) that can consume an echo skill.
- A [EchoSkillBot](EchoSkillBot/README.md) that handle echoes message activities from the parent bot.

## To try this sample

- Create a bot registration in the azure portal for the EchoSkillBot and update [EchoSkillBot/appsettings.json](EchoSkillBot/appsettings.json) with the AppId and password.
- Create a bot registration in the azure portal for the SimpleRootBot and update [SimpleRootBot/appsettings.json](SimpleRootBot/appsettings.json) with the AppId and password. 
- Update the BotFrameworkSkills section in [SimpleRootBot/appsettings.json](SimpleRootBot/appsettings.json) with the AppId for the skill you created in the previou step.
- Configure Visual Studio to run both applications at the same time.
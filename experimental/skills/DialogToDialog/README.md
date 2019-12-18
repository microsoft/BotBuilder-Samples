# DialogToDialog (**DRAFT**)

Bot Framework v4 Skills with Dialogs sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to use skills from a rootbot.

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## Key concepts

- A [DialogRootBot](DialogRootBot/README.md) that can consume skills.
- A [DialogSkillBot](DialogSkillBot/README.md) that handle requests from a parent bot.

## To try this sample

- Create a bot registration in the azure portal for the DialogSkillBot and update [DialogSkillBot/appsettings.json](DialogSkillBot/appsettings.json) with the AppId and password.
- Create a bot registration in the azure portal for the DialogRootBot and update [DialogRootBot/appsettings.json](DialogRootBot/appsettings.json) with the AppId and password. 
- Update the BotFrameworkSkills section in [DialogRootBot/appsettings.json](DialogRootBot/appsettings.json) with the AppId for the skill you created in the previou step.
- Configure Visual Studio to run both applications at the same time.
- (Optional) Configure the bot registration for [DialogSkillBot](DialogSkillBot) with an OAuth connection if you want to test acquiring OAuth tokens from the skill.
- (Optional) Configure the LuisAppId, LuisAPIKey and LuisAPIHostName section in the [DialogSkillBot configuration](DialogSkillBot/appsettings.json) if you want to run message activities through LUIS.
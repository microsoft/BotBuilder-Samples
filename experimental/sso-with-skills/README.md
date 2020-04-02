# SSO with Simple Skill Consumer and Skill

Bot Framework v4 skills echo sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple skill consumer (SimpleRootBot) that sends message activities to a skill (SkillBot) that echoes it back.
it shows how to implement single sign on between a simple skill consumer (SimpleRootBot) and a skill (SkillBot)

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## Key concepts in this sample

The solution includes a parent bot (`SimpleRootBot`) and a skill bot (`SkillBot`) and shows how the parent bot can support a user signin and call a skill on behalf of the user, without the user being required to authenticate again into the skill.

- `SimpleRootBot`: this project is a simple skill consumer bot, and supports:
    - `login` command that gets the user to sign into the skill consumer bot's aad application.
    - `token` command that gets displays the user's token
    - `logout` command that logs the user out of the parent bot
- `SkillBot`: this project shows a simple skill that supports OAuth for AADV2 and can respond to the following commands
    - `skill login` command that gets the skill consumer bot to signin into the skill's aadV2 app, on behalf of the user. The user is not shown a signin card, unless SSO fails.
    - `skill token` command that gets displays the user's token from the skill
    - `skill logout` command that logs the user out of the skill

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- Create a bot registration in the azure portal for the `SkillBot` and update [SkillBot/appsettings.json](SkillBot/appsettings.json) with the `MicrosoftAppId` and `MicrosoftAppPassword` of the new bot registration
- Update the `BotFrameworkSkills` section in [SimpleRootBot/appsettings.json](SimpleRootBot/appsettings.json) with the app ID for the skill you created in the previous step
- Create a bot registration in the azure portal for the `SimpleRootBot` and update [SimpleRootBot/appsettings.json](SimpleRootBot/appsettings.json) with the `MicrosoftAppId` and `MicrosoftAppPassword` of the new bot registration
- Update the `BotFrameworkSkills` section in [SimpleRootBot/appsettings.json](SimpleRootBot/appsettings.json) with the app ID for the skill you created in the previous step
- Add the `SimpleRootBot` `MicrosoftAppId` to the `AllowedCallers` list in [SkillBot/appsettings.json](SkillBot/appsettings.json) 
- Update the `BotFrameworkSkills` section in [SimpleRootBot/appsettings.json](SimpleRootBot/appsettings.json) with the app ID for the skill you created in the previous step

- Setup the 2 AAD applications for SSO as per steps given in [docs](https://www.insertdocs.foo). You will end up with 2 aad applications - one for the skill consumer and one for the skill.
- Create an aadv2 connection in the bot registration for the `SkillBot` and fill in values from the aadv2 application created for SSO, as per the [docs](https://www.insertdocs.foo).Update [SkillBot/appsettings.json](SkillBot/appsettings.json) with the `ConnectionName`  
- Create an aadv2 connection in the bot registration for the `SimpleRootBot` and fill in values from the aadv2 application created for SSO, as per the [docs](https://www.insertdocs.foo).Update [SimpleRootBot/appsettings.json](SkillBot/appsettings.json) with the `ConnectionName`  

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

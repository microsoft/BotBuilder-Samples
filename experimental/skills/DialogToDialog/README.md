# DialogToDialog (**DRAFT**)

Bot Framework v4 Skills with Dialogs sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to use skills from a rootbot.

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## Key concepts in this sample

The solution uses dialogs and includes a parent bot (`DialogRootBot`) and a skill bot (`DialogSkillBot`) and shows how the parent bot can post activities to the skill bot and returns the skill responses to the user.

- `DialogRootBot`: this project shows how to consume a skill bot using a `SkillDialog` and includes:
    - A [root dialog](DialogRootBot/Dialogs/MainDialog.cs) that can call different tasks on a skill using a [SkillDialog](DialogRootBot/Dialogs/SkillDialog.cs):
        - Event Tasks
        - Message Tasks
        - Invoke Tasks
    - How to send an `EndOfConversation` activity to remotely let a skill that it needs to end a conversation.
    - How to Implement a [ClaimsValidator](DialogRootBot/Authentication/AllowedCallersClaimsValidator.cs) that allows a parent bot to validate that a response is coming from a skill that is allowed to talk to the parent.
    - A sample [SkillDialog](DialogRootBot/Dialogs/SkillDialog.cs) that can be used to keep track of multiturn interactions with a skill using the dialog stack.
    - A [Logger Middleware](DialogRootBot/Middleware/LoggerMiddleware.cs) that shows how to handle and log activities coming from a skill
    - A [SkillConversationIdFactory](DialogRootBot/SkillConversationIdFactory.cs) based on IStorage to create and maintain conversation IDs to interact with a skill.
    - A [SkillsConfiguration](DialogRootBot/SkillsConfiguration.cs) class that can load skill definitions from appsettings.
    - A [startup](DialogRootBot/Startup.cs) class that shows how to register the different skills components for DI.
    - A [SkillController](DialogRootBot/Controllers/SkillController.cs) that handles skill responses.

- `DialogSkillBot`: this project shows a simple echo skill that receives message activities from the parent bot and echoes what the user said. This project includes:
    - A sample [IBot](DialogSkillBot/Bots/SkillBot.cs) that shows how to handle and return EndOfConversation based on the status of the dialog in the skill.
    - An [ActivityRouterDialog](DialogSkillBot/Bots/ActivityRouterDialog.cs) that handles Events, Messages and Invoke activities coming from a parent and perform different tasks. 
    - How to receive and return values in a skill.
    - A [sample skill manifest](DialogSkillBot/wwwroot/manifest/dialogchildbot-manifest-1.0.json) that describes what the skill can do.

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- Create a bot registration in the azure portal for the DialogSkillBot and update [DialogSkillBot/appsettings.json](DialogSkillBot/appsettings.json) with the AppId and password.
- Create a bot registration in the azure portal for the DialogRootBot and update [DialogRootBot/appsettings.json](DialogRootBot/appsettings.json) with the AppId and password. 
- Update the BotFrameworkSkills section in [DialogRootBot/appsettings.json](DialogRootBot/appsettings.json) with the AppId for the skill you created in the previou step.
- (Optional) Configure the bot registration for [DialogSkillBot](DialogSkillBot) with an OAuth connection if you want to test acquiring OAuth tokens from the skill.
- (Optional) Configure the LuisAppId, LuisAPIKey and LuisAPIHostName section in the [DialogSkillBot configuration](DialogSkillBot/appsettings.json) if you want to run message activities through LUIS.
- Open the `DialogToDialog.sln` solution and configure it to [start debugging with multiple processes](https://docs.microsoft.com/en-us/visualstudio/debugger/debug-multiple-processes?view=vs-2019#start-debugging-with-multiple-processes)

# SkillDialog

Bot Framework v4 Skills with Dialogs sample.

This bot has been created using the [Bot Framework](https://dev.botframework.com); it shows how to use a skill dialog from a root bot.

## Prerequisites

- [Node.js](https://nodejs.org) version 10.14 or higher.

    ```bash
    # Determine node version.
    node --version
    ```

## Key concepts in this sample

The solution uses dialogs, within both a parent bot (`DialogRootBot`) and a skill bot (`DialogSkillBot`). It demonstrates how to post activities from the parent bot to the skill bot and return the skill responses to the user.

- `DialogRootBot`: this project shows how to consume a skill bot using a `SkillDialog`. It includes:
  - A [root dialog](dialogRootBot/dialogs/mainDialog.js) that can call different actions on a skill using a `SkillDialog`:
    - To send events activities.
    - To send message activities.
    - To cancel a `SkillDialog` using `CancelAllDialogsAsync` that automatically sends an `EndOfConversation` activity to remotely let a skill know that it needs to end a conversation.
  - A sample [AllowedSkillsClaimsValidator](dialogRootBot/authentication/allowedSkillsClaimsValidator.js) class that shows how to validate that responses sent to the bot are coming from the configured skills.
  - A [Logger Middleware](dialogRootBot/middleware/loggerMiddleware.js) that shows how to handle and log activities coming from a skill.
  - A [SkillConversationIdFactory](dialogRootBot/skillConversationIdFactory.js) used to create and maintain conversation IDs to interact with a skill.
  - A [SkillsConfiguration](dialogRootBot/skillsConfiguration.js) class that can load skill definitions from the `.env` file.

- `DialogSkillBot`: this project shows a modified CoreBot that acts as a skill. It receives event and message activities from the parent bot and executes the requested tasks. This project includes:
  - An [ActivityRouterDialog](dialogSkillBot/dialogs/activityRouterDialog.js) that handles Event and Message activities coming from a parent and performs different tasks.
    - Event activities are routed to specific dialogs using the parameters provided in the `values` property of the activity.
    - Message activities are sent to LUIS if configured and trigger the desired tasks if the intent is recognized.
  - A sample [activityHandler](dialogSkillBot/bots/skillBot.js) that uses the `runDialog` method on `ActivityRouterDialog`.

    Note: Starting in Bot Framework 4.8, the `runDialog` helper method adds support to automatically send `EndOfConversation` with return values when the bot is running as a skill and the current dialog ends. It also handles reprompt messages to resume a skill where it left of.
  - A sample [AllowedCallersClaimsValidator](dialogSkillBot/authentication/allowedCallersClaimsValidator.js) that shows how to validate that the skill is only invoked from a list of allowed callers
  - A [sample skill manifest](dialogSkillBot/manifest/dialogchildbot-manifest-1.0.json) that describes what the skill can do.

## To try this sample

- Clone the repository.

  ```bash
  git clone https://github.com/microsoft/botbuilder-samples.git
  ```

- Create a bot registration in the azure portal for the `dialogSkillBot` and update [dialogSkillBot/.env](dialogSkillBot/.env) with the AppId and password.
- Create a bot registration in the azure portal for the `dialogRootBot` and update [dialogRootBot/.env](dialogRootBot/.env) with the AppId and password.
- Update the BotFrameworkSkills section in [dialogRootBot/.env](dialogRootBot/.env) with the AppId for the skill you created in the previous step.
- (Optional) Configure the LuisAppId, LuisAPIKey and LuisAPIHostName section in the [dialogSkillBot/.env](dialogSkillBot/.env) if you want to run message activities through LUIS.

For each bot directory, `dialogSkillBot` and `dialogRootBot` as `<botDirectory>`:

- In a terminal, navigate to `samples/javascript_nodejs/81.<botDirectory>`

    ```bash
    cd samples/javascript_nodejs/81.<botDirectory>
    ```

- Install modules

    ```bash
    npm install
    ```

- Start the bot

    ```bash
    npm start
    ```

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.8.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`, the `MicrosoftAppId` and `MicrosoftAppPassword` for the `DialogRootBot`

## Deploy the bots to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

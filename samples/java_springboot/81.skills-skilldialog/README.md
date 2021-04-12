# SkillDialog

Bot Framework v4 Skills with Dialogs sample.

This bot has been created using the [Bot Framework](https://dev.botframework.com); it shows how to use a skill dialog from a root bot.

## Prerequisites

- Java 1.8+
- Install [Maven](https://maven.apache.org/)
- An account on [Azure](https://azure.microsoft.com) if you want to deploy to Azure.

## Key concepts in this sample

The solution uses dialogs, within both a parent bot (`dialog-root-bot`) and a skill bot (`dialog-skill-bot`).
It demonstrates how to post activities from the parent bot to the skill bot and return the skill responses to the user.

- `dialog-root-bot`: this project shows how to consume a skill bot using a `SkillDialog`. It includes:
  - A [Main Dialog](dialog-root-bot/dialogs/main_dialog.py) that can call different actions on a skill using a `SkillDialog`:
    - To send events activities.
    - To send message activities.
    - To cancel a `SkillDialog` using `CancelAllDialogsAsync` that automatically sends an `EndOfConversation` activity to remotely let a skill know that it needs to end a conversation.
  - A sample [AdapterWithErrorHandler](dialog-root-bot/adapter_with_error_handler.py) adapter that shows how to handle errors, terminate skills and send traces back to the emulator to help debugging the bot.
  - A sample [AllowedSkillsClaimsValidator](dialog-root-bot/authentication/allowed_skills_claims_validator.py) class that shows how to validate that responses sent to the bot are coming from the configured skills.
  - A [Logger Middleware](dialog-root-bot/middleware/logger_middleware.py) that shows how to handle and log activities coming from a skill.
  - A [SkillConversationIdFactory](dialog-root-bot/skill_conversation_id_factory.py) based on `Storage` used to create and maintain conversation IDs to interact with a skill.
  - A [SkillConfiguration](dialog-root-bot/config.py) class that can load skill definitions from the `DefaultConfig` class.
  - An [app.py](dialog-root-bot/app.py) class that shows how to register the different root bot components. This file also creates a `SkillHandler` and `aiohttp_channel_service_routes` which are used to handle responses sent from the skills.
- `dialog_skill_bot`: this project shows a modified CoreBot that acts as a skill. It receives event and message activities from the parent bot and executes the requested tasks. This project includes:
  - An [ActivityRouterDialog](dialog-skill-bot/dialogs/activity_router_dialog.py) that handles Event and Message activities coming from a parent and performs different tasks.
    - Event activities are routed to specific dialogs using the parameters provided in the `Values` property of the activity.
    - Message activities are sent to LUIS if configured and trigger the desired tasks if the intent is recognized.
  - A sample [ActivityHandler](dialog-skill-bot/bots/skill_bot.py) that uses the `run_dialog` method on `DialogExtensions`.

    Note: Starting in Bot Framework 4.8, the `DialogExtensions` class was introduced to provide a `run_dialog` method wich adds support to automatically send `EndOfConversation` with return values when the bot is running as a skill and the current dialog ends. It also handles reprompt messages to resume a skill where it left of.
  - A sample [SkillAdapterWithErrorHandler](dialog-skill-bot/skill_adapter_with_error_handler.py) adapter that shows how to handle errors, terminate the skills, send traces back to the emulator to help debugging the bot and send `EndOfConversation` messages to the parent bot with details of the error.
  - A sample [AllowedCallersClaimsValidator](dialog-skill-bot/authentication/allow_callers_claims_validation.py) that shows how to validate that the skill is only invoked from a list of allowed callers
  - An [app.py](dialog-skill-bot/app.py) class that shows how to register the different skill components.
  - A [sample skill manifest](dialog-skill-bot/wwwroot/manifest/dialogchildbot-manifest-1.0.json) that describes what the skill can do.



## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- Create a bot registration in the azure portal for the `dialog-skill-bot` and update [dialog-skill-bot/config.py](dialog-skill-bot/config.py) with the `MicrosoftAppId` and `MicrosoftAppPassword` of the new bot registration
- Create a bot registration in the azure portal for the `dialog-root-bot` and update [dialog-root-bot/config.py](dialog-root-bot/config.py) with the `MicrosoftAppId` and `MicrosoftAppPassword` of the new bot registration
- Update the `SKILLS.app_id` in [dialog-root-bot/config.py](dialog-root-bot/config.py) with the `MicrosoftAppId` for the skill you created in the previous step
- (Optionally) Add the `dialog-root-bot` `MicrosoftAppId` to the `AllowedCallers` comma separated list in [dialog-skill-bot/config.py](dialog-skill-bot/config.py)

## Running the sample

- In a terminal, navigate to `samples\python\81.skills-skilldialog\dialog-skill-bot`

    ```bash
    cd samples\python\81.skills-skilldialog\dialog-skill-bot
    ```

- Activate your desired virtual environment

- Run `pip install -r requirements.txt` to install all dependencies

- Run your bot with `python app.py`

- Open a **second** terminal window and navigate to `samples\python\81.skills-skilldialog\dialog-root-bot`

    ```bash
    cd samples\python\81.skills-skilldialog\dialog-root-bot
    ```

- Activate your desired virtual environment

- Run `pip install -r requirements.txt` to install all dependencies

- Run your bot with `python app.py`


## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.7.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`, the `MicrosoftAppId` and `MicrosoftAppPassword` for the `dialog-root-bot`

## Deploy the bots to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

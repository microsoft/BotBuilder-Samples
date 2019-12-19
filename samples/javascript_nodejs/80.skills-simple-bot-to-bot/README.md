# simple-bot-to-bot Echo Skill

Bot Framework v4 Skills Echo sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple RootBot that sends message activities to a SkillBot that echoes it back.

## Prerequisites

- [Node.js](https://nodejs.org) version 10.14 or higher

    ```bash
    # determine node version
    node --version

## Key concepts in this sample

The solution includes a parent bot (`simple-root-bot`) and a skill bot (`echo-skill-bot`) and shows how the parent bot can post activities to the skill bot.

- `simple-root-bot`: this project shows how to consume an echo skill and includes:
  - A [RootBot](simple-root-bot/rootBot.js) that calls the echo skill and keeps the conversation active until the user says "end" or "stop". [RootBot](simple-root-bot/rootBot.js) also keeps track of the conversation with the skill and handles the `EndOfConversation` activity received from the skill to terminate the conversation
  - A simple [SkillConversationIdFactory](simple-root-bot/skillConversationIdFactory.js) based on an in memory dictionary that creates and maintains conversation IDs used to interact with a skill
  - A [SkillsConfiguration](simple-root-bot/skillsConfiguration.js) class that can load skill definitions from `.env`
  - An [AllowedSkillsClaimsValidator](simple-root-bot/authentication/allowedSkillsClaimsValidator.js) class that is used to authenticate that responses sent to the bot are coming from the configured skills
  - An [index.js](simple-root-bot/index.js) class that shows how to register the different skill components. This file also reigisters a `skillEndpoint` that is used to handle responses sent from the skills
- `echo-skill-bot`: this project shows a simple echo skill that receives message activities from the parent bot and echoes what the user said. This project includes:
  - A sample [EchoBot](echo-skill-bot/bot.js) that shows how to send `EndOfConversation` based on the message sent to the skill and yield control back to the parent bot
  - A sample [AllowedCallersClaimsValidator](echo-skill-bot/authentication/allowedCallersClaimsValidator.js) that shows how validate that the skill is only invoked from a list of allowed callers
  - A [sample skill manifest](echo-skill-bot/manifest/echoskillbot-manifest-1.0.json) that describes what the skill can do

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- Create a bot registration in the azure portal for the `echo-skill-bot` and update [echo-skill-bot/.env](echo-skill-bot/.env) with the `MicrosoftAppId` and `MicrosoftAppPassword` of the new bot registration
- Create a bot registration in the azure portal for the `simple-root-bot` and update [simple-root-bot/.env](simple-root-bot/.env) with the `MicrosoftAppId` and `MicrosoftAppPassword` of the new bot registration
- Update the `BotFrameworkSkills` section in [simple-root-bot/.env](simple-root-bot/.env) with the `AppId` for the skill you created in the previous step
- (Optionally) Add the `simple-root-bot` `MicrosoftAppId` to the `AllowedCallers` comma separated list in [echo-skill-bot/.env](echo-skill-bot/.env)
- In a terminal, navigate to `samples\javascript_nodejs\80.skills-simple-bot-to-bot\echo-skill-bot`

    ```bash
    cd samples\javascript_nodejs\80.skills-simple-bot-to-bot\echo-skill-bot
    ```

- Install npm modules and start the bot

    ```bash
    npm install
    npm start
    ```

- Open a **second** terminal window and navigate to `samples\javascript_nodejs\80.skills-simple-bot-to-bot\simple-root-bot`

    ```bash
    cd samples\javascript_nodejs\80.skills-simple-bot-to-bot\simple-root-bot
    ```

- Install npm modules and start the bot

    ```bash
    npm install
    npm start
    ```

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.7.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`, the `MicrosoftAppId` and `MicrosoftAppPassword` for the `simple-root-bot`

## Deploy the bots to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

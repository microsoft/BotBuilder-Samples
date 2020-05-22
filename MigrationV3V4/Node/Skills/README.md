# Bot Framework V3 Skills

This sample was created using `botbuilder` [v3.30.0](https://www.npmjs.com/package/botbuilder/v/3.30.0) and [v4.7.2](https://www.npmjs.com/package/botbuilder/v/4.7.2) SDKs. It shows how to create a simple skill consumer (v4-root-bot) which sends message activities to any of two V3 bots converted to skills (v3-skill-bot and v3-booking-bot-skill).

Steps to convert a bot to a skill using SDK v3.30.0:
1)	When instantiating the ChatConnector using `new builder.ChatConnector()` in the v3 bot's `app.js`, enable skills and add skill authentication:

    ```js
    var connector = new builder.ChatConnector({
        appId: process.env.MICROSOFT_APP_ID,
        appPassword: process.env.MICROSOFT_APP_PASSWORD,
        enableSkills: true,
        authConfiguration: new skills.AuthenticationConfiguration([], allowedCallersClaimsValidator)
    });
    ```

2)	Create a Skills Validator, similar to `v3-booking-bot-skill`'s `allowedCallersClaimsValidator.js`. The `allowedCallers` array can restrict which appId's can access the skill. Set this to "*" to accept calls from any skill consumer, or set it to a comma-delimited list of the app IDs of the allowed consumers. In these samples, we're only authorizing the parent bot, via `process.env.ROOT_BOT_APP_ID` which is set in the `.env` file.
3)	Add code to send the skill host and `endOfConversation` activity when the skill is finished and to properly handle `endOfConversation` from the skill consumer.

(Note: these changes will not affect the bot's behavior as a normal v3 bot, and it can still be used that way, too.)

## Prerequisites

The minimum prerequisites to run this sample are:
* Latest Node.js with NPM. Download it from [here](https://nodejs.org/en/download/).
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.
* **[Recommended]** Visual Studio Code for IntelliSense and debugging, download it from [here](https://code.visualstudio.com/) for free.

## Key concepts in this sample

The solution includes a parent bot (`v4-root-bot`) built using [v4.7.2](https://www.npmjs.com/package/botbuilder/v/4.7.2) and two V3 skill bots (v3-skill-bot and v3-booking-bot-skill) built using [v3.30.0](https://www.npmjs.com/package/botbuilder/v/3.30.0). This example demonstrates how a parent bot can post activities to skill bots and returns the skill responses to the user.

- `v4-root-bot`: this project is based on [80.skills-simple-bot-to-bot](https://github.com/microsoft/BotBuilder-Samples/tree/master/samples/javascript_nodejs/80.skills-simple-bot-to-bot). It shows how to consume the two V3 skills and includes:
  - A [RootBot](v4-root-bot/index.js) which calls the user's chosen skill and keeps the conversation active until the user says "end" or "stop". [RootBot](v4-root-bot/index.js) also keeps track of the conversation with the skill and handles the `endOfConversation` activity received from the skill to terminate the conversation.
  - A simple [SkillConversationIdFactory](v4-root-bot/skillConversationIdFactory.js) based on an in memory JavaScript `object` that creates and maintains conversation IDs used to interact with a skill.
  - A [SkillsConfiguration](v4-root-bot/skillsConfiguration.js) class that loads skill definitions from `.env`.
  - An [AllowedSkillsClaimsValidator](v4-root-bot/authentication/allowedSkillsClaimsValidator.js) is used to validate responses sent to the bot are coming from the configured skills.
  - An [Index](v4-root-bot/index.js) class that shows how to instantiate all of the key botbuilder classes and set up the necessary skill components.

- `v3-skill-bot`: this project shows a simple echo skill that receives message activities from the parent bot and echoes what the user said. This project was based on C#'s [EchoBot](https://github.com/microsoft/BotBuilder-V3/tree/master/CSharp/EchoBot) and includes:
  - A sample [app.js](v3-skill-bot/app.js) that shows how to send `endConversation()` based on the message sent to the skill and yield control back to the parent bot.
  - [app.js](v3-skill-bot/app.js) also shows how to simply validate a skill is only called from specified appId's with `allowedCallers: [process.env.ROOT_BOT_APP_ID]`

- `v3-booking-skill-bot`: this project shows a simple flight and hotel booking skill that receives message activities from the parent bot and sends the user's booking details back to the parent when finished.
  - A sample [allowedCallersClaimsValidator.js](v3-booking-bot-skill/allowedCallersClaimsValidator.js) that shows how to provide custom validation, ensuring the skill is only invoked from a list of allowed callers.
  - The function, `endConversation()` in [app.js](v3-booking-skill-bot/app.js) that shows how to send EndOfConversation with results back to the parent bot.
    - In [hotels.js](v3-booking-skill-bot/hotels.js), the dialog is ended with `session.endDialogWithResult(result);`, the results are passed to the last step in [app.js](v3-booking-skill-bot/app.js)'s WaterfallDialog, which calls `endConversation()`, which then sends the results back to the root bot and the root bot displays them.

## To try this sample

- Clone the repository.

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- Open the `\MigrationV3V4\Node\Skills\` folder to find this README and a folder for each bot
- Create a bot registration in the azure portal for each of the bots `v4-root-bot`, `v3-skill-bot`, and `v3-booking-skill-bot`, keeping track of the Application IDs and Passwords for each.

### Root bot setup

For `v4-root-bot`, fill in the '.env' values based on your bot's specific configuration:

```
MicrosoftAppId={V4_BOT_APP_ID}
MicrosoftAppPassword={V4_BOT_PASSWORD}
SkillHostEndpoint={V3_SKILL_BOT_ENDPOINT ex: http://{HOST}{PORT}/api/skills/}

SkillSimpleId={V3_SIMPLE_SKILL_BOT_NAME ex: 'v3-skill-bot'}
SkillSimpleAppId={V3_SIMPLE_SKILL_BOT_APP_ID}
SkillSimpleEndpoint={V3_SIMPLE_SKILL_BOT_ENDPOINT ex: http://{HOST}{PORT}/api/messages}

SkillBookingId={V3_BOOKING_SKILL_BOT_NAME ex: 'v3-booking-bot-skill'}
SkillBookingAppId={V3_BOOKING_SKILL_BOT_APP_ID}
SkillBookingEndpoint={V3_BOOKING_SKILL_BOT_ENDPOINT ex: http://{HOST}{PORT}/api/messages}
```

### Skill bots setup

For both `v3-skill-bot` and `v3-booking-bot-skill`, fill in the '.env' values based on your bot's specific configuration:

```
MICROSOFT_APP_ID={V3_BOT_APP_ID}
MICROSOFT_APP_PASSWORD={V3_BOT_PASSWORD}
ROOT_BOT_APP_ID={V4_BOT_APP_ID}
```

## Test run

1. Launch v4 bot and v3 bot(s) ('npm run start' from their respective root folders)
2. Send a test message to v4 bot in order to invoke v3 bot as skill. You should be prompted to enter a specific test in order to invoke one of the two skills and enter their respective dialog flows. Enter 'end' at any time to return to the parent bot.

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.7.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases).

### Connect to the bot using Bot Framework Emulator

- Launch the Bot Framework Emulator.
- Select File -> Open Bot.
- Enter a bot URL of `http://localhost:3978/api/messages`, and use the `MicrosoftAppId` and `MicrosoftAppPassword` for the `SimpleRootBot`.

## Deploy the bots to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

# dice roller skill
Bot Framework v4 dice roller skill sample

This sample demonstrates how to implement a Cortana Skill that properly handles EndOfConversation events.

## Prerequisites
- [Node.js][4] version 10.14 or higher

```bash
# determine node version
node --version
```

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to `samples/javascript_nodejs/50.diceroller-skill`
    ```bash
    cd samples/javascript_nodejs/50.diceroller-skill
    ```
- Install modules
    ```bash
    npm install
    ```
- Start the bot
    ```bash
    npm start
    ```

# Testing the bot using Bot Framework Emulator **v4**
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/microsoft/botframework-emulator/releases)

## Connect to bot using Bot Framework Emulator **v4**
- Launch Bot Framework Emulator
- File -> Open Bot Configuration and navigate to `samples/javascript_nodejs/50.diceroller-skill`
- Select `diceroller-skill.bot` file

# Cortana Skills
Cortana skills are standard BotBuilder bots that require a few additional considerations specific to Cortana.

The first thing to understand about a Cortana skill is that Cortana follows a very rigid turn based model of speaking where the user sends a single message to the bot, then the bot sends a single reply to the user, then the user sends a message back to the bot, the bot then sends a reply, and so on. The important thing to note from the bots perspective is that once you've sent a message to the user you are not allowed to send another message to the user until they've replied. You can work around this to some extent using the [inputHint](https://docs.microsoft.com/en-us/javascript/api/botframework-schema/activity?view=botbuilder-ts-latest#inputhint) property off the outgoing activity but in general your skill needs to conform to this back and forth conversation flow.

Another thing unique to Cortana skills is the use of the [EndOfConversation](https://docs.microsoft.com/en-us/javascript/api/botframework-schema/activitytypes?view=botbuilder-ts-latest) activity to indicate that the current skill invocation is finished. This activity can be sent from Cortana to the bot to indicate that the user closed the Cortana window in the UI, and it can be sent from the bot to Cortana to indicate that the Cortana window should be closed. It's worth noting that Cortana in some cases will re-use the same conversation ID on multiple invocations. This can potentially lead to skills starting off in the wrong state so as a best practice your skill should include logic to clear its conversation state anytime an `EndOfConversation` activity is detected. The sample includes a `CortanaSkill` base class that you can derive your bots main dialog from
and automatically pickup the logic to clear your bots conversation state anytime an `EndOfConversation` activity is detected.

Cortana skills tend to be more multi-modal in their use of both speech and text. You can use the activities `speak` field to send Cortana standard [Speech Synthesis Markup Language(SSML)](https://docs.microsoft.com/en-us/cortana/skills/speech-synthesis-markup-language) that should be spoken to the user. The sample includes a simple `ssml` module that helps make composing valid SSML easier.

When creating skills targeted at Cortana for the desktop you'll want to fill in both the `text` and `speak` fields of the outgoing activity and you'll find the thing you want to show to the user and speak to the user are often quite different.  The sample includes a simple `Language Generation (LG)` module that simplifies composing activities containing both `text` and `speak` fields.

# Deploy this bot to Azure
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. In order to install this and other tools, you can read [Installing CLI Tools](../../../Installing_CLI_tools.md).

To clone this bot, run
```
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id> --appId <YOUR APP ID> --appSecret <YOUR APP SECRET PASSWORD>
```

**NOTE**: You can obtain your `appId` and `appSecret` at the Microsoft's [Application Registration Portal](https://apps.dev.microsoft.com/)


# Further reading
- [Dialog class reference](https://docs.microsoft.com/en-us/javascript/api/botbuilder-dialogs/dialog)
- [WaterfallDialog class reference](https://docs.microsoft.com/en-us/javascript/api/botbuilder-dialogs/waterfall)
- [Manage complex conversation flows with dialogs](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-dialog-manage-complex-conversation-flow?view=azure-bot-service-4.0&tabs=javascript)

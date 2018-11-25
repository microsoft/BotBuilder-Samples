# generator-botbuilder
Yeoman generator for [Microsoft Bot Framework v4][1].  Will let you quickly set up a conversational AI bot
using core AI capabilities.

# About
generator-botbuilder will help you build new conversational AI bots using the [Microsoft Bot Framework v4][1].

# Templates
The generator supports three different template options.  The table below can help guide which template is right for you.

|  Template  |  Description  |
| ---------- |  ---------  |
| Empty | A good template if you are familiar with Bot Framework v4, and simple want a basic skeleton project.  Also a good option if you want to take sample code from the documentation and paste it into a minimal bot in order to learn. |
| Echo | A good template if you want a little more than "Hello World!", but not much more.  This template handles the very basics of sending messages to a bot, and having the bot process the messages by repeating them back to the user.  This template produces a bot that simply "echos" back to the user anything the user says to the bot. |
| Basic | Our most advanced template, the Basic template provides 6 core features every bot is likely to have.  This template covers the basic features of a Conversational-AI bot using LUIS[2].  See the **Basic Bot Features** table below for more details. |

## How to Choose a Template
| Template | When This Template is a Good Choice |
| -------- | -------- |
| Echo   | You are new to Bot Framework v4 and want a working bot with minimal features. |
| Basic  | You understand some of the core concepts of Bot Framework v4 and are beyond the concepts introduced in the Echo template.  You're familiar with or are ready to learn concepts such as language understanding using LUIS, managing multi-turn conversations with Dialogs, handling user initiated Dialog interruptions, and using Adaptive Cards to welcome your users. |
| Empty  | You are a seasoned Bot Framework v4 developer.  You've built bots before, and want the minimum skeleton of a bot. |

## Template Overview
### Echo Template
The Echo bot template is slightly more than the a classic "Hello World!" example, but not by much.  This template shows the basic structure of a bot, how a bot recieves messages from a user, and how a bot sends messages to a user.  The bot will "echo" back to the user, what the user says to the bot.  It is a good choice for first time, new to Bot Framework v4 developers.

### Basic Template
The Basic bot template consists of set of core features most every bot is likely to have.  Building off of the core message processing features found in the Empty template, this template adds a number of more sophisticated features.  The table below lists these features and provides links to additional documentation.

| Basic&nbsp;Bot&nbsp;Features | Description |
| ------------------ | ----------- |
| [Send and receive messages][40] | The primary way your bot will communicate with users, and likewise receive communication, is through message activities. Some messages may simply consist of plain text, while others may contain richer content such as cards or attachments. |
| [Proactive messaging][41] using [Adaptive Cards][42] | The primary goal when creating any bot is to engage your user in a meaningful conversation. One of the best ways to achieve this goal is to ensure that from the moment a user first connects to your bot, they understand your bot’s main purpose and capabilities.  We refer to this as "welcoming the user."  The Basic template uses an [Adaptive Card][42] to implement this behavior.  |
| [Language understanding using LUIS][43] | The ability to understand what your user means conversationally and contextually can be a difficult task, but can provide your bot a more natural conversation feel. Language Understanding, called LUIS, enables you to do just that so that your bot can recognize the intent of user messages, allow for more natural language from your user, and better direct the conversation flow. |
| [Multi-turn conversation support using Dialogs][44] | The ability to manage conversations is an important part of the bot/user interation.  Bot Framework introduces the  concept of a Dialog to handle this conversational pattern.  Dialog objects process inbound Activities and generate outbound responses. The business logic of the bot runs either directly or indirectly within Dialog classes.  |
| [Managing conversation state][45] | A key to good bot design is to track the context of a conversation, so that your bot remembers things like the answers to previous questions. |
| [How to handle user-initiated interruptions][46] | While you may think that your users will follow your defined conversation flow step by step, chances are good that they will change their minds or ask a question in the middle of the process instead of answering the question. Handling interruptions means making sure your bot is prepared to handle situations like this. |


# Features by Template
|  Feature  |  Empty  |  Echo   |  Basic  |
| --------- | :-----: | :-----: | :-----: |
| Generate code in JavaScript or TypesScript | X | X | X |
| Support local development and testing using the [Microsoft Bot Framework Emulator v4][3] | X | X | X |
| Core bot message processing |  | X | X |
| Deploy your bot to Microsoft Azure |  | X | X |
| Welcome new users using Adaptive Card technology |  |  | X |
| Support AI-based greetings using [LUIS][2] |  |  | X |
| Use Dialogs to manage more in-depth conversations |  |  | X |
| Manage conversation state |  |  | X |
| Handle user interruptions |  |  | X |


# Installation
1.  Install [Yeoman][9] using [npm][10] (we assume you have pre-installed [node.js][11]).
    ```bash
    # Make sure both are installed globally
    npm install -g yo
    ```

2.  Install generator-botbuilder by typing the following in your console:
    ```bash
    # Make sure both are installed globally
    npm install -g generator-botbuilder
    ```

3.  Verify that Yeoman and generator-botbuilder have been installed correctly by typing the following into your console:
    ```bash
    yo botbuilder --help
    ```


# Usage
## Creating a New Bot Project
When the generator is launched, it will prompt for the information required to create a new bot.

```bash
# Run the generator in interactive mode
yo botbuilder
```

## Generator Command Line Options
The generator supports a number of command line options that can be used to change the generator's default options or to pre-seed a prompt.

| Command&nbsp;line&nbsp;Option  | Description |
| ------------------- | ----------- |
| --help, -h        | List help text for all supported command-line options |
| --botname, -N     | The name given to the bot project |
| --description, -D | A brief bit of text that describes the purpose of the bot |
| --language, -L    | The programming language for the project.  Options are `JavaScript` or `TypeScript`. |
| --template, -T    | The template used to generate the project.  Options are `empty`, `echo`, or `basic`.  See https://aka.ms/botbuilder-generator for additional information regarding the different template option and their functional differences. |
| --noprompt        | The generator will not prompt for confirmation before creating a new bot.  Any requirement options not passed on the command line will use a reasonable default value.  This option is intended to enable automated bot generation for testing purposes. |

### Example Using Command Line Options
This example shows how to pass command line options to the generator, setting the default language to TypeScript and the default template to Basic.
```bash
# Run the generator defaulting the language to TypeScript and the template to basic
yo botbuilder --L "TypeScript" --T "basic"
```

## Generating a Bot Using --noprompt
The generator can be run in `--noprompt` mode, which can be used for automated bot creation.  When run in `--noprompt` mode, the generator can be configured using command line options as documented above.  If a command line option is ommitted a reasonable default will be used.  In addition, passing the `--noprompt` option will cause the generator to create a new bot project without prompting for confirmation before generating the bot.

### Default Options
| Command&nbsp;line&nbsp;Option  | Default Value |
| ------------------- | ----------- |
| --botname, -N     | `my-chat-bot` |
| --description, -D | "Demonstrate the core capabilities of the Microsoft Bot Framework" |
| --language, -L    | `JavaScript` |
| --template, -T    | `echo` |


### Examples Using --noprompt
This example shows how to run the generator in --noprompt mode, setting all required options on the command line.
```bash
# Run the generator, setting all command line options
yo botbuilder --noprompt -N "my-first-bot" -D "A bot that demonstrates core AI capabilities" -L "JavaScript" -T "Echo"
```

This example shows how to run the generator in --noprompt mode, using all the default command line options.  The generator will create a bot project using all the default values specified in the **Default Options** table above.
```bash
# Run the generator using all default options
yo botbuilder --noprompt
```


# Running Your Bot
## Running Your Bot Locally
To run your bot locally, type the following in your console:
```bash
# From the directory that contains your bot
npm start
```

## Interacting With Your Bot Using the Emulator
Launch the [Microsoft Bot Framework Emulator v4][3] and open the generated project's `.bot` file.

Once the Emulator is connected, you can interact with and receive messages from your bot.

## Developing Your Bot Locally
It's often easier to develop the capabilities of your bot locally, and to use the Microsoft Bot Framework Emulator to test your changes.  When the generator generated your bot project it added a file watcher to the project.  When run, the watcher which will cause nodejs to reload the bot whenever any of the bot's source files change.  Causing nodejs to reload your bot under these circumstances will ensure you are always running the latest version of your bot.  Enable the watch feature by typing the following in your console:

```bash
# From the directory that contains your bot
npm run watch
```
When you run the `watch` task, nodejs will reload your bot anytime a file in your project changes.  When using the Emulator to interact with
your bot, you will need to click the Emulator's 'Start Over' tab in order to force the Emulator to also reload the latest version of your bot.


# Deploying Your Bot to Microsoft Azure
For bots generated using the `Basic` template, the generator will create a `DEPLOYMENT.md` file found in the `/deploymentScripts` folder.  This file provides instructions for how to deploy the bot to Microsoft Azure.  The deployment process assume you have an account on Microsoft Azure and are able to log into the [Microsoft Azure Portal][4].

If you are new to Microsoft Azure, please refer to [Getting started with Azure][5] for guidance on how to get started on Azure.

# Logging Issues and Providing Feedback
Issues and feedback about the botbuilder generator can be submitted through the project's [GitHub issues][12] page.


[1]: https://dev.botframework.com
[2]: https://www.luis.ai
[3]: https://www.github.com/microsoft/botframework-emulator
[4]: https://portal.azure.com
[5]: https://azure.microsoft.com/en-us/get-started/
[6]: https://github.com/motdotla/dotenv
[7]: http://restify.com
[8]: https://github.com/remy/nodemon
[9]: http://yeoman.io
[10]: https://www.npmjs.com
[11]: https://nodejs.org/en/
[12]: https://github.com/Microsoft/botbuilder-samples/issues
[40]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-send-messages?view=azure-bot-service-4.0&tabs=javascript
[41]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-send-welcome-message?view=azure-bot-service-4.0&tabs=csharp%2Ccsharpmulti%2Ccsharpwelcomeback
[42]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-send-welcome-message?view=azure-bot-service-4.0&tabs=csharp%2Ccsharpmulti%2Ccsharpwelcomeback#using-adaptive-card-greeting
[43]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&tabs=cs
[44]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-dialog?view=azure-bot-service-4.0
[45]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-state?view=azure-bot-service-4.0&tabs=csharp
[46]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-handle-user-interrupt?view=azure-bot-service-4.0&tabs=csharptab

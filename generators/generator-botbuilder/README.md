# generator-botbuilder

Yeoman generator for [Bot Framework v4](https://dev.botframework.com).  Will let you quickly set up a conversational AI bot
using core AI capabilities.

## About

`generator-botbuilder` will help you build new conversational AI bots using the [Bot Framework v4](https://dev.botframework.com).

## Templates

The generator supports three different template options.  The table below can help guide which template is right for you.

|  Template  |  Description  |
| ---------- |  ---------  |
| Echo&nbsp;Bot | A good template if you want a little more than "Hello World!", but not much more.  This template handles the very basics of sending messages to a bot, and having the bot process the messages by repeating them back to the user.  This template produces a bot that simply "echoes" back to the user anything the user says to the bot. |
| Core&nbsp;Bot | Our most advanced template, the Core template provides 6 core features every bot is likely to have.  This template covers the core features of a Conversational-AI bot using [LUIS](https://www.luis.ai).  See the **Core Bot Features** table below for more details. |
| Empty&nbsp;Bot | A good template if you are familiar with Bot Framework v4, and simply want a basic skeleton project.  Also a good option if you want to take sample code from the documentation and paste it into a minimal bot in order to learn. |

### How to Choose a Template

| Template | When This Template is a Good Choice |
| -------- | -------- |
| Echo&nbsp;Bot  | You are new to Bot Framework v4 and want a working bot with minimal features. |
| Core&nbsp;Bot | You understand some of the core concepts of Bot Framework v4 and are beyond the concepts introduced in the Echo Bot template.  You're familiar with or are ready to learn concepts such as language understanding using LUIS, managing multi-turn conversations with Dialogs, handling user initiated Dialog interruptions, and using Adaptive Cards to welcome your users. |
| Empty&nbsp;Bot  | You are a seasoned Bot Framework v4 developer.  You've built bots before, and want the minimum skeleton of a bot. |

### Template Overview

#### Echo Bot Template

The Echo Bot template is slightly more than the a classic "Hello World!" example, but not by much.  This template shows the basic structure of a bot, how a bot recieves messages from a user, and how a bot sends messages to a user.  The bot will "echo" back to the user, what the user says to the bot.  It is a good choice for first time, new to Bot Framework v4 developers.

#### Core Bot Template

The Core Bot template consists of set of core features most every bot is likely to have.  Building off of the core message processing features found in the Echo Bot template, this template adds a number of more sophisticated features.  The table below lists these features and provides links to additional documentation.

| Core&nbsp;Bot&nbsp;Features | Description |
| ------------------ | ----------- |
| [Send and receive messages](https://docs.microsoft.com/azure/bot-service/bot-builder-howto-send-messages?view=azure-bot-service-4.0&tabs=javascript) | The primary way your bot will communicate with users, and likewise receive communication, is through message activities. Some messages may simply consist of plain text, while others may contain richer content such as cards or attachments. |
| [Proactive messaging](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-proactive-message?view=azure-bot-service-4.0) using [Adaptive Cards](https://docs.microsoft.com/azure/bot-service/bot-builder-send-welcome-message?view=azure-bot-service-4.0?#using-adaptive-card-greeting) | The primary goal when creating any bot is to engage your user in a meaningful conversation. One of the best ways to achieve this goal is to ensure that from the moment a user first connects to your bot, they understand your bot’s main purpose and capabilities.  We refer to this as "welcoming the user."  The Core template uses an [Adaptive Card](http://adaptivecards.io) to implement this behavior.  |
| [Language understanding using LUIS](https://docs.microsoft.com/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0) | The ability to understand what your user means conversationally and contextually can be a difficult task, but can provide your bot a more natural conversation feel. Language Understanding, called LUIS, enables you to do just that so that your bot can recognize the intent of user messages, allow for more natural language from your user, and better direct the conversation flow. |
| [Multi-turn conversation support using Dialogs](https://docs.microsoft.com/azure/bot-service/bot-builder-concept-dialog?view=azure-bot-service-4.0) | The ability to manage conversations is an important part of the bot/user interation.  Bot Framework introduces the  concept of a Dialog to handle this conversational pattern.  Dialog objects process inbound Activities and generate outbound responses. The business logic of the bot runs either directly or indirectly within Dialog classes.  |
| [Managing conversation state](https://docs.microsoft.com/azure/bot-service/bot-builder-howto-v4-state?view=azure-bot-service-4.0) | A key to good bot design is to track the context of a conversation, so that your bot remembers things like the answers to previous questions. |
| [How to handle user-initiated interruptions](https://docs.microsoft.com/azure/bot-service/bot-builder-howto-handle-user-interrupt?view=azure-bot-service-4.0) | While you may think that your users will follow your defined conversation flow step by step, chances are good that they will change their minds or ask a question in the middle of the process instead of answering the question. Handling interruptions means making sure your bot is prepared to handle situations like this. |

#### Empty Bot Template

The Empty Bot template is the minimal skeleton code for a bot.  It provides a stub `onTurn` handler but does not perform any actions.  If you are experienced writing bots with Bot Framework v4 and want the minimum scaffolding, the Empty template is for you.

## Features by Template

|  Feature  |  Empty&nbsp;Bot  |  Echo&nbsp;Bot   |  Core&nbsp;Bot  |
| --------- | :-----: | :-----: | :-----: |
| Generate code in JavaScript or TypesScript | X | X | X |
| Support local development and testing using the [Bot Framework Emulator v4](https://www.github.com/microsoft/botframework-emulator) | X | X | X |
| Core bot message processing |  | X | X |
| Deploy your bot to Microsoft Azure |  | X | X |
| Welcome new users using Adaptive Card technology |  |  | X |
| Support AI-based greetings using [LUIS](https://www.luis.ai) |  |  | X |
| Use Dialogs to manage more in-depth conversations |  |  | X |
| Manage conversation state |  |  | X |
| Handle user interruptions |  |  | X |


## Installation

1. Install [Yeoman](http://yeoman.io) using [npm](https://www.npmjs.com) (we assume you have pre-installed [node.js](https://nodejs.org/)).

    ```bash
    # Make sure both are installed globally
    npm install -g yo
    ```

2. Install generator-botbuilder by typing the following in your console:

    ```bash
    # Make sure both are installed globally
    npm install -g generator-botbuilder
    ```

3. Verify that Yeoman and generator-botbuilder have been installed correctly by typing the following into your console:

    ```bash
    yo botbuilder --help
    ```


## Usage

### Creating a New Bot Project

When the generator is launched, it will prompt for the information required to create a new bot.

```bash
# Run the generator in interactive mode
yo botbuilder
```

### Generator Command Line Options

The generator supports a number of command line options that can be used to change the generator's default options or to pre-seed a prompt.

| Command&nbsp;line&nbsp;Option  | Description |
| ------------------- | ----------- |
| --help, -h        | List help text for all supported command-line options |
| --botname, -N     | The name given to the bot project |
| --description, -D | A brief bit of text that describes the purpose of the bot |
| --language, -L    | The programming language for the project.  Options are `JavaScript` or `TypeScript`. |
| --template, -T    | The template used to generate the project.  Options are `empty`, `echo`, or `core`.  See [https://aka.ms/botbuilder-generator](https://aka.ms/botbuilder-generator) for additional information regarding the different template option and their functional differences. |
| --noprompt        | The generator will not prompt for confirmation before creating a new bot.  Any requirement options not passed on the command line will use a reasonable default value.  This option is intended to enable automated bot generation for testing purposes. |

#### Example Using Command Line Options

This example shows how to pass command line options to the generator, setting the default language to TypeScript and the default template to Core.

```bash
# Run the generator defaulting the language to TypeScript and the template to core
yo botbuilder --L "TypeScript" --T "core"
```

### Generating a Bot Using --noprompt

The generator can be run in `--noprompt` mode, which can be used for automated bot creation.  When run in `--noprompt` mode, the generator can be configured using command line options as documented above.  If a command line option is ommitted a reasonable default will be used.  In addition, passing the `--noprompt` option will cause the generator to create a new bot project without prompting for confirmation before generating the bot.

#### Default Options

| Command&nbsp;line&nbsp;Option  | Default Value |
| ------------------- | ----------- |
| --botname, -N     | `my-chat-bot` |
| --description, -D | "Demonstrate the core capabilities of the Microsoft Bot Framework" |
| --language, -L    | `JavaScript` |
| --template, -T    | `echo` |

#### Examples Using --noprompt

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

## Running Your Bot

### Running Your Bot Locally

To run your bot locally, type the following in your console:

```bash
# install modules
npm install
```

```bash
# run the bot
npm start
```

### Interacting With Your Bot Using the Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

Once the Emulator is connected, you can interact with and receive messages from your bot.

### Developing Your Bot Locally

It's often easier to develop the capabilities of your bot locally, and to use the Microsoft Bot Framework Emulator to test your changes.  When the generator generated your bot project it added a file watcher to the project.  When run, the watcher which will cause nodejs to reload the bot whenever any of the bot's source files change.  Causing nodejs to reload your bot under these circumstances will ensure you are always running the latest version of your bot.  Enable the watch feature by typing the following in your console:

```bash
# From the directory that contains your bot
npm run watch
```

When you run the `watch` task, nodejs will reload your bot anytime a file in your project changes.  When using the Emulator to interact with
your bot, you will need to click the Emulator's 'Start Over' tab in order to force the Emulator to also reload the latest version of your bot.

#### Lint Compliant Code

The code generated by the botbuilder generator is lint compliant.  Depending on whether the bot was generated using JavaScript or TypeScript, there is either a `.eslint` or `.tslint` file that contains the linting rules used to lint the generated code.  To use lint as your develop your bot:

```bash
npm run lint
```

## Deploy Your Bot to Azure

After creating the bot and testing it locally, you can deploy it to Azure to make it accessible from anywhere.
To learn how, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete set of deployment instructions.

If you are new to Microsoft Azure, please refer to [Getting started with Azure](https://azure.microsoft.com/get-started/) for guidance on how to get started on Azure.

## Optionally Using Development Builds

Development builds are based off of "work in progress" code.  This means they may or may not be stable and may have incomplete documentation.  These builds are better suited for more experienced users and developers, although everyone is welcome to give them a shot and provide feedback.

You can get the latest development builds of `generator-botbuilder` from the [BotBuilder MyGet](https://botbuilder.myget.org/gallery) feed.  To install the latest development build, follow the following steps:

```bash
# configure npm to pull from the developer builds registry
npm config set registry https://botbuilder.myget.org/F/aitemplates/npm/
```

```bash
# installing using npm
npm install -g generator-botbuilder
```

```bash
# reset npm to use the public registry
npm config set registry https://registry.npmjs.org
```

Now when `yo botbuilder` is run, it will use the development build.  To remove the development build, run the following:

```bash
# installing using npm
npm uninstall -g generator-botbuilder
```

## Logging Issues and Providing Feedback

Issues and feedback about the botbuilder generator can be submitted through the project's [GitHub Issues](https://github.com/Microsoft/botbuilder-samples/issues) page.

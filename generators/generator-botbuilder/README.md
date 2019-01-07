# generator-botbuilder
<<<<<<< HEAD
Yeoman generator for [Microsoft Bot Framework][1].  Will let you quickly set up a conversational AI bot
using core AI capabilities.

# About
generator-botbuilder will help you build new converstational AI bots using the [Microsoft Bot Framework][1].

# Features
The generator supports two different templates.  The features the generator produces depends on the template you choose.  The `Echo` template produces a "Hello World!" capable bot.  It is the simpliest bot you can write, providing the minimal features required by a bot.  The `Basic` template builds on the capabilities of the `Echo` template and add features typically found in most every bot.  Here is a table that shows what features exist in the different templates.

|  Feature  |  Echo   |  Basic  |
| --------- | :-----: | :-----: |
| Core bot message processing | X | X |
| Send messages to the bot  | X | X |
| Manage bot state | X | X |
| Choose between JavaScript or TypesScript programming language | X | X |
| Support for local development and testing using the [Microsoft Bot Framework Emulator][3] | X | X |
| AI-based greeting, using [LUIS][2] |  | X |
| Use Dialogs to manage conversation flow |  | X |
| Display a welcome card using Adaptive Card technology |  | X |
| Handle user interruptions | | X |
| Optionally deploy your bot to Microsoft Azure | X | X |

## Dependencies
* [dotenv][6] for managing environmental variables
* [restify][7] for hosting the API
* [nodemon][8] (dev mode only) for monitoring code changes and auto restarting the bot server


# Installation
First, install [Yeoman][9] using [npm][10] (we assume you have pre-installed [node.js][11]).
```bash
# Make sure both are installed globally
npm install -g yo
```

Now install generator-botbuilder by typing the following in your console:
```bash
# Make sure both are installed globally
npm install -g generator-botbuilder
```

You can verify that Yeoman and generator-botbuilder have been installed correctly by typing the following into your console:
```bash
yo botbuilder --help
```


# Usage
You can create a bot by running the generator in either interactive mode, or noprompt mode.  Interactive mode will prompt you for the information the generator needs to generate a new bot.  In noprompt mode the generator will generate a new bot using a reasonable set of default options.  The user will not be prompted for information when running the generator in noprompt mode.

## Creating a new bot project, interactively
When the generator is run interactively, it will prompt the user for all the information required to create a new bot.
=======
Yeoman generator for [Bot Framework v4][1].  Will let you quickly set up a conversational AI bot
using core AI capabilities.

# About
generator-botbuilder will help you build new conversational AI bots using the [Bot Framework v4][1].

# Templates
The generator supports three different template options.  The table below can help guide which template is right for you.

|  Template  |  Description  |
| ---------- |  ---------  |
| Empty&nbsp;Bot | A good template if you are familiar with Bot Framework v4, and simple want a basic skeleton project.  Also a good option if you want to take sample code from the documentation and paste it into a minimal bot in order to learn. |
| Echo&nbsp;Bot | A good template if you want a little more than "Hello World!", but not much more.  This template handles the very basics of sending messages to a bot, and having the bot process the messages by repeating them back to the user.  This template produces a bot that simply "echos" back to the user anything the user says to the bot. |
| Basic&nbsp;Bot | Our most advanced template, the Basic template provides 6 core features every bot is likely to have.  This template covers the basic features of a Conversational-AI bot using [LUIS][2].  See the **Basic Bot Features** table below for more details. |

## How to Choose a Template
| Template | When This Template is a Good Choice |
| -------- | -------- |
| Echo&nbsp;Bot  | You are new to Bot Framework v4 and want a working bot with minimal features. |
| Basic&nbsp;Bot | You understand some of the core concepts of Bot Framework v4 and are beyond the concepts introduced in the Echo Bot template.  You're familiar with or are ready to learn concepts such as language understanding using LUIS, managing multi-turn conversations with Dialogs, handling user initiated Dialog interruptions, and using Adaptive Cards to welcome your users. |
| Empty&nbsp;Bot  | You are a seasoned Bot Framework v4 developer.  You've built bots before, and want the minimum skeleton of a bot. |

## Template Overview
### Echo Bot Template
The Echo Bot template is slightly more than the a classic "Hello World!" example, but not by much.  This template shows the basic structure of a bot, how a bot recieves messages from a user, and how a bot sends messages to a user.  The bot will "echo" back to the user, what the user says to the bot.  It is a good choice for first time, new to Bot Framework v4 developers.

### Basic Bot Template
The Basic Bot template consists of set of core features most every bot is likely to have.  Building off of the core message processing features found in the Echo Bot template, this template adds a number of more sophisticated features.  The table below lists these features and provides links to additional documentation.

| Basic&nbsp;Bot&nbsp;Features | Description |
| ------------------ | ----------- |
| [Send and receive messages][40] | The primary way your bot will communicate with users, and likewise receive communication, is through message activities. Some messages may simply consist of plain text, while others may contain richer content such as cards or attachments. |
| [Proactive messaging][41] using [Adaptive Cards][42] | The primary goal when creating any bot is to engage your user in a meaningful conversation. One of the best ways to achieve this goal is to ensure that from the moment a user first connects to your bot, they understand your bot’s main purpose and capabilities.  We refer to this as "welcoming the user."  The Basic template uses an [Adaptive Card][42] to implement this behavior.  |
| [Language understanding using LUIS][43] | The ability to understand what your user means conversationally and contextually can be a difficult task, but can provide your bot a more natural conversation feel. Language Understanding, called LUIS, enables you to do just that so that your bot can recognize the intent of user messages, allow for more natural language from your user, and better direct the conversation flow. |
| [Multi-turn conversation support using Dialogs][44] | The ability to manage conversations is an important part of the bot/user interation.  Bot Framework introduces the  concept of a Dialog to handle this conversational pattern.  Dialog objects process inbound Activities and generate outbound responses. The business logic of the bot runs either directly or indirectly within Dialog classes.  |
| [Managing conversation state][45] | A key to good bot design is to track the context of a conversation, so that your bot remembers things like the answers to previous questions. |
| [How to handle user-initiated interruptions][46] | While you may think that your users will follow your defined conversation flow step by step, chances are good that they will change their minds or ask a question in the middle of the process instead of answering the question. Handling interruptions means making sure your bot is prepared to handle situations like this. |

### Empty Bot Template
The Empty Bot template is the minimal skeleton code for a bot.  It provides a stub `onTurn` handler but does not perform any actions.  If you are experienced writing bots with Bot Framework v4 and want the minimum scaffolding, the Empty template is for you.


# Features by Template
|  Feature  |  Empty&nbsp;Bot  |  Echo&nbsp;Bot   |  Basic&nbsp;Bot  |
| --------- | :-----: | :-----: | :-----: |
| Generate code in JavaScript or TypesScript | X | X | X |
| Support local development and testing using the [Bot Framework Emulator v4][3] | X | X | X |
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
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

```bash
# Run the generator in interactive mode
yo botbuilder
```

<<<<<<< HEAD
## Generator Commandline Options
The generator supports a number of commandline options that can be used to change the generator's default options.
=======
## Generator Command Line Options
The generator supports a number of command line options that can be used to change the generator's default options or to pre-seed a prompt.
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

| Command&nbsp;line&nbsp;Option  | Description |
| ------------------- | ----------- |
| --help, -h        | List help text for all supported command-line options |
| --botname, -N     | The name given to the bot project |
<<<<<<< HEAD
| --description, -D | A brief bit of text that describes the purpose of your bot |
| --language, -L    | The programming language the generator should use to generate the project.  Supported options are JavaScript or TypeScript. |
| --template, -T    | The template used to generate your project.  Supported options are `echo` which is the simpliest bot you can build, or `basic` which contains a core set of AI capabilities demonstrating basic bot functionality. |
| --noprompt        | If passed on the command-line, will tell the generator to *not* prompt the user for any information.  This option is intented to allow automated bot generation for testing purposes. *NOTE* when using the --noprompt option, if no template is specified, the `basic` template is used.|

## Generating a project using --noprompt
In the following example, all required options are specified on the command-line.  In addition, passing the `--noprompt` option will cause the
generator to create a new bot project without asking you to verify any of the options specified on the command line.

```bash
# Run the generator
yo botbuilder -N "myFirstBot" -D "A bot that demonstrates core AI capabilities" -L "JavaScript" -T "Basic" --noprompt
```
### Generating a project using default options
In the following example, a new bot based off of the `basic` template is generated using a reasonable set of defaults.

=======
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
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
```bash
# Run the generator using all default options
yo botbuilder --noprompt
```

<<<<<<< HEAD
The table below describes the reasonable set of defaults used when `--noprompt` command-line option is used without specifying any additional options.

| Commandline Option | Description |
| ------------------ | ----------- |
| --botname, -N      | `myChatBot` |
| --description, -D  | `Demonstrate the core capabilties of the Microsoft Bot Framework` |
| --language, -L     | `JavaScript` |
| --template, -T     | `basic` |


# Running your bot
## Running your bot locally
To run your bot locally, type the following in your console:

```bash
# From the directory that contains your bot
npm start
```

## Interacting with your bot using the emulator
Launch the [Microsoft Bot Framework Emulator][3] and open the generated project's `.bot` file.

Once the Emulator is connected, you can interact with and receive messages from your bot.

## Developing your bot locally
It's often easeier to develop the capabilities of your bot locally, and to use the Microsoft Bot Framework Emulator to test your changes.  When the generator generated your bot project it added a file watcher to the project.  When run, the watcher which will cause nodejs to reload the bot whenever any of the bot's files change.  Causing nodejs to reload your bot under these circumstances will ensure you are always running the latest version of your bot.  Enable the watch feature by typing the following in your conole:
=======

# Running Your Bot
## Running Your Bot Locally
To run your bot locally, type the following in your console:
```bash
# install modules
npm install
```
```bash
# run the bot
npm start
```

## Interacting With Your Bot Using the Emulator
Launch the [Bot Framework Emulator v4][3] and open the generated project's `.bot` file.

Once the Emulator is connected, you can interact with and receive messages from your bot.

## Developing Your Bot Locally
It's often easier to develop the capabilities of your bot locally, and to use the Microsoft Bot Framework Emulator to test your changes.  When the generator generated your bot project it added a file watcher to the project.  When run, the watcher which will cause nodejs to reload the bot whenever any of the bot's source files change.  Causing nodejs to reload your bot under these circumstances will ensure you are always running the latest version of your bot.  Enable the watch feature by typing the following in your console:
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

```bash
# From the directory that contains your bot
npm run watch
```
<<<<<<< HEAD
When you run the `watch` task, nodejs will reload your bot anytime a file in your project changes.  When using the Emulator to interact with
your bot, you will need to click the Emulator's 'Start Over' tab in order to force the Emulator to also reload the latest version of your bot.



# Deploying your bot to Microsoft Azure
For bots generated using the `Basic` template, the generator will create a `DEPLOYMENT.md` file found in the `/deploymentScripts` folder.  This file provides instructions for how to deploy the bot to Microsoft Azure.  The deployment process assume you have an account on Microsoft Azure and are able to log into the [Microsoft Azure Portal][4].

If you are new to Microsoft Azure, please refer to [Getting started with Azure][5] for guidance on how to get started on Azure.


=======

When you run the `watch` task, nodejs will reload your bot anytime a file in your project changes.  When using the Emulator to interact with
your bot, you will need to click the Emulator's 'Start Over' tab in order to force the Emulator to also reload the latest version of your bot.

### Lint Compliant Code
The code generated by the botbuilder generator is lint compliant.  Depending on whether the bot was generated using JavaScript or TypeScript, there is either a `.eslint` or `.tslint` file that contains the linting rules used to lint the generated code.  To use lint as your develop your bot:

```bash
npm run lint
```

# Deploy Your Bot to Azure
After creating the bot and testing it locally, you can deploy it to Azure to make it accessible from anywhere.
To learn how, see [Deploy your bot to Azure][50] for a complete set of deployment instructions.

If you are new to Microsoft Azure, please refer to [Getting started with Azure][5] for guidance on how to get started on Azure.

# Optionally Using Development Builds
Development builds are based off of "work in progress" code.  This means they may or may not be stable and may have incomplete documentation.  These builds are better suited for more experienced users and developers, although everyone is welcome to give them a shot and provide feedback.

You can get the latest development builds of `generator-botbuilder` from the [BotBuilder MyGet][51] feed.  To install the latest development build, follow the following steps:


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

# Logging Issues and Providing Feedback
Issues and feedback about the botbuilder generator can be submitted through the project's [GitHub Issues][12] page.
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145


[1]: https://dev.botframework.com
[2]: https://www.luis.ai
[3]: https://www.github.com/microsoft/botframework-emulator
[4]: https://portal.azure.com
<<<<<<< HEAD
[5]: https://azure.microsoft.com/en-us/get-started/
=======
[5]: https://azure.microsoft.com/get-started/
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
[6]: https://github.com/motdotla/dotenv
[7]: http://restify.com
[8]: https://github.com/remy/nodemon
[9]: http://yeoman.io
[10]: https://www.npmjs.com
<<<<<<< HEAD
[11]: https://nodejs.org/en/
=======
[11]: https://nodejs.org/
[12]: https://github.com/Microsoft/botbuilder-samples/issues
[40]: https://docs.microsoft.com/azure/bot-service/bot-builder-howto-send-messages?view=azure-bot-service-4.0&tabs=javascript
[41]: https://docs.microsoft.com/azure/bot-service/bot-builder-send-welcome-message?view=azure-bot-service-4.0
[42]: https://docs.microsoft.com/azure/bot-service/bot-builder-send-welcome-message?view=azure-bot-service-4.0?#using-adaptive-card-greeting
[43]: https://docs.microsoft.com/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0
[44]: https://docs.microsoft.com/azure/bot-service/bot-builder-concept-dialog?view=azure-bot-service-4.0
[45]: https://docs.microsoft.com/azure/bot-service/bot-builder-howto-v4-state?view=azure-bot-service-4.
[46]: https://docs.microsoft.com/azure/bot-service/bot-builder-howto-handle-user-interrupt?view=azure-bot-service-4.0
[50]: https://aka.ms/azuredeployment
[51]: https://botbuilder.myget.org/gallery
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

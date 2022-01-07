# generator-botbuilder-java

Yeoman generator for [Bot Framework v4](https://dev.botframework.com).  Will let you quickly set up a conversational AI bot
using core AI capabilities.

## About

`generator-botbuilder-java` will help you build new conversational AI bots using the [Bot Framework v4](https://dev.botframework.com).

## Templates

The generator supports three different template options.  The table below can help guide which template is right for you.

|  Template  |  Description  |
| ---------- |  ---------  |
| Echo&nbsp;Bot | A good template if you want a little more than "Hello World!", but not much more.  This template handles the very basics of sending messages to a bot, and having the bot process the messages by repeating them back to the user.  This template produces a bot that simply "echoes" back to the user anything the user says to the bot. |
| Empty&nbsp;Bot | A good template if you are familiar with Bot Framework v4, and simply want a basic skeleton project.  Also a good option if you want to take sample code from the documentation and paste it into a minimal bot in order to learn. |
| Core Bot | A good template if you want to create advanced bots, as it uses multi-turn dialogs and [LUIS](https://www.luis.ai), an AI based cognitive service, to implement language understanding. This template creates a bot that can extract places and dates to book a flight. |

### How to Choose a Template

| Template | When This Template is a Good Choice |
| -------- | -------- |
| Echo&nbsp;Bot  | You are new to Bot Framework v4 and want a working bot with minimal features. |
| Empty&nbsp;Bot  | You are a seasoned Bot Framework v4 developer.  You've built bots before, and want the minimum skeleton of a bot. |
| Core Bot | You are a medium to advanced user of Bot Framework v4 and want to start integrating language understanding as well as multi-turn dialogs in your bots. |

### Template Overview

#### Echo Bot Template

The Echo Bot template is slightly more than the a classic "Hello World!" example, but not by much.  This template shows the basic structure of a bot, how a bot receives messages from a user, and how a bot sends messages to a user.  The bot will "echo" back to the user, what the user says to the bot.  It is a good choice for first time, new to Bot Framework v4 developers.

#### Empty Bot Template

The Empty Bot template is the minimal skeleton code for a bot.  It provides a stub `onTurn` handler but does not perform any actions.  If you are experienced writing bots with Bot Framework v4 and want the minimum scaffolding, the Empty template is for you.

#### Core Bot Template

The Core Bot template uses [LUIS](https://www.luis.ai) to implement core AI capabilities, a multi-turn conversation using Dialogs, handles user interruptions, and prompts for and validate requests for information from the user. This template implements a basic three-step waterfall dialog, where the first step asks the user for an input to book a flight, then asks the user if the information is correct, and finally confirms the booking with the user.  Choose this template if want to create an advanced bot that can extract information from the user's input.

## Installation

1. Install [Yeoman](http://yeoman.io) using [npm](https://www.npmjs.com) (we assume you have pre-installed [node.js](https://nodejs.org/)).

    ```bash
    # Make sure both are installed globally
    npm install -g yo
    ```

2. Install generator-botbuilder-java by typing the following in your console:

    ```bash
    # Make sure both are installed globally
    npm install -g generator-botbuilder-java
    ```

3. Verify that Yeoman and generator-botbuilder-java have been installed correctly by typing the following into your console:

    ```bash
    yo botbuilder-java --help
    ```

## Usage

### Creating a New Bot Project

When the generator is launched, it will prompt for the information required to create a new bot.

```bash
# Run the generator in interactive mode
yo botbuilder-java
```

### Generator Command Line Options

The generator supports a number of command line options that can be used to change the generator's default options or to pre-seed a prompt.

| Command&nbsp;line&nbsp;Option  | Description |
| ------------------- | ----------- |
| --help, -h        | List help text for all supported command-line options |
| --botName, -N     | The name given to the bot project |
| --packageName, -P | The Java package name to use for the bot |
| --template, -T    | The template used to generate the project.  Options are `empty`, or `echo`.  See [https://aka.ms/botbuilder-generator](https://aka.ms/botbuilder-generator) for additional information regarding the different template option and their functional differences. |
| --noprompt        | The generator will not prompt for confirmation before creating a new bot.  Any requirement options not passed on the command line will use a reasonable default value.  This option is intended to enable automated bot generation for testing purposes. |

#### Example Using Command Line Options

This example shows how to pass command line options to the generator, setting the default language to TypeScript and the default template to Core.

```bash
# Run the generator defaulting the pacakge name and the template
yo botbuilder-java --P "com.mycompany.bot" --T "echo"
```

### Generating a Bot Using --noprompt

The generator can be run in `--noprompt` mode, which can be used for automated bot creation.  When run in `--noprompt` mode, the generator can be configured using command line options as documented above.  If a command line option is ommitted a reasonable default will be used.  In addition, passing the `--noprompt` option will cause the generator to create a new bot project without prompting for confirmation before generating the bot.

#### Default Options

| Command&nbsp;line&nbsp;Option  | Default Value |
| ------------------- | ----------- |
| --botname, -N     | `echo` |
| --packageName, -p | `echo` |
| --template, -T    | `echo` |

#### Examples Using --noprompt

This example shows how to run the generator in --noprompt mode, setting all required options on the command line.

```bash
# Run the generator, setting all command line options
yo botbuilder-java --noprompt -N "MyEchoBot" -P "com.mycompany.bot.echo" -T "echo"
```

This example shows how to run the generator in --noprompt mode, using all the default command line options.  The generator will create a bot project using all the default values specified in the **Default Options** table above.

```bash
# Run the generator using all default options
yo botbuilder-java --noprompt
```

## Logging Issues and Providing Feedback

Issues and feedback about the botbuilder generator can be submitted through the project's [GitHub Issues](https://github.com/Microsoft/botbuilder-samples/issues) page.

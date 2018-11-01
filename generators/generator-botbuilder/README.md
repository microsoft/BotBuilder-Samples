# generator-botbuilder
Yeoman generator for [Microsoft Bot Framework][1].  Will let you quickly set up a conversational AI bot
using core AI capabilities.

# About
generator-botbuilder will help you build new conversational AI bots using the [Microsoft Bot Framework][1].

# Features
The generator supports two different templates.  The features the generator produces depends on the template you choose.  The `Echo` template produces a "Hello World!" capable bot.  It is the simplest bot you can write, providing the minimal features required by a bot.  The `Basic` template builds on the capabilities of the `Echo` template and add features typically found in most every bot.  Here is a table that shows what features exist in the different templates.

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

```bash
# Run the generator in interactive mode
yo botbuilder
```

## Generator Commandline Options
The generator supports a number of commandline options that can be used to change the generator's default options.

| Command&nbsp;line&nbsp;Option  | Description |
| ------------------- | ----------- |
| --help, -h        | List help text for all supported command-line options |
| --botname, -N     | The name given to the bot project |
| --description, -D | A brief bit of text that describes the purpose of your bot |
| --language, -L    | The programming language the generator should use to generate the project.  Supported options are JavaScript or TypeScript. |
| --template, -T    | The template used to generate your project.  Supported options are `echo` which is the simplest bot you can build, or `basic` which contains a core set of AI capabilities demonstrating basic bot functionality. |
| --noprompt        | If passed on the command-line, will tell the generator to *not* prompt the user for any information.  This option is intended to allow automated bot generation for testing purposes. *NOTE* when using the --noprompt option, if no template is specified, the `basic` template is used.|

## Generating a project using --noprompt
In the following example, all required options are specified on the command-line.  In addition, passing the `--noprompt` option will cause the
generator to create a new bot project without asking you to verify any of the options specified on the command line.

```bash
# Run the generator
yo botbuilder -N "myFirstBot" -D "A bot that demonstrates core AI capabilities" -L "JavaScript" -T "Basic" --noprompt
```
### Generating a project using default options
In the following example, a new bot based off of the `basic` template is generated using a reasonable set of defaults.

```bash
# Run the generator using all default options
yo botbuilder --noprompt
```

The table below describes the reasonable set of defaults used when `--noprompt` command-line option is used without specifying any additional options.

| Commandline Option | Description |
| ------------------ | ----------- |
| --botname, -N      | `myChatBot` |
| --description, -D  | `Demonstrate the core capabilities of the Microsoft Bot Framework` |
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
It's often easier to develop the capabilities of your bot locally, and to use the Microsoft Bot Framework Emulator to test your changes.  When the generator generated your bot project it added a file watcher to the project.  When run, the watcher which will cause nodejs to reload the bot whenever any of the bot's files change.  Causing nodejs to reload your bot under these circumstances will ensure you are always running the latest version of your bot.  Enable the watch feature by typing the following in your console:

```bash
# From the directory that contains your bot
npm run watch
```
When you run the `watch` task, nodejs will reload your bot anytime a file in your project changes.  When using the Emulator to interact with
your bot, you will need to click the Emulator's 'Start Over' tab in order to force the Emulator to also reload the latest version of your bot.



# Deploying your bot to Microsoft Azure
For bots generated using the `Basic` template, the generator will create a `DEPLOYMENT.md` file found in the `/deploymentScripts` folder.  This file provides instructions for how to deploy the bot to Microsoft Azure.  The deployment process assume you have an account on Microsoft Azure and are able to log into the [Microsoft Azure Portal][4].

If you are new to Microsoft Azure, please refer to [Getting started with Azure][5] for guidance on how to get started on Azure.




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
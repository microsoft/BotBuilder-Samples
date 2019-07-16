# .NET Core SDK Templates
.NET Core Templates for [Bot Framework v4][1].  These templates will let you quickly create conversational AI bots
using core AI capabilities.

## About
.NET Core Templates will help you to quickly build new conversational AI bots using [Bot Framework v4][1].

## Templates
There are three different template options.  The table below can help guide which template is right for you.

|  Template  |  Description  |
| ---------- |  ---------  |
| Echo&nbsp;Bot | A good template if you want a little more than "Hello World!", but not much more.  This template handles the very basics of sending messages to a bot, and having the bot process the messages by repeating them back to the user.  This template produces a bot that simply "echoes" back to the user anything the user says to the bot. |
| Core&nbsp;Bot | Our most advanced template, the Core Bot template provides 6 core features every bot is likely to have.  This template covers the core features of a Conversational-AI bot using [LUIS][2].  See the **Core Bot Features** table below for more details. |
| Empty&nbsp;Bot | A good template if you are familiar with Bot Framework v4, and simple want a basic skeleton project.  Also a good option if you want to take sample code from the documentation and paste it into a minimal bot in order to learn. |

### How to Choose a Template
| Template | When This Template is a Good Choice |
| -------- | -------- |
| Echo&nbsp;Bot  | You are new to Bot Framework v4 and want a working bot with minimal features. |
| Core&nbsp;Bot | You understand some of the core concepts of Bot Framework v4 and are beyond the concepts introduced in the Echo Bot template.  You're familiar with or are ready to learn concepts such as language understanding using LUIS, managing multi-turn conversations with Dialogs, handling user initiated Dialog interruptions, and using Adaptive Cards to welcome your users. |
| Empty&nbsp;Bot | You are a seasoned Bot Framework v4 developer.  You've built bots before, and want the minimum skeleton of a bot to help you get started. |

## Template Overview
### Echo Bot Template
The Echo Bot template is slightly more than the a classic "Hello World!" example, but not by much.  This template shows the basic structure of a bot, how a bot recieves messages from a user, and how a bot sends messages to a user.  The bot will "echo" back to the user, what the user says to the bot.  It is a good choice for first time, new to Bot Framework v4 developers.

### Core Bot Template
The Core Bot template consists of set of core features most every bot is likely to have.  Building off of the core message processing features found in the Echo Bot template, this template adds a number of more sophisticated features.  The table below lists these features and provides links to additional documentation.

| Core&nbsp;Bot&nbsp;Features | Description |
| ------------------ | ----------- |
| [Send and receive messages][40] | The primary way your bot will communicate with users, and likewise receive communication, is through message activities. Some messages may simply consist of plain text, while others may contain richer content such as cards or attachments. |
| [Proactive messaging][41] using [Adaptive Cards][42] | The primary goal when creating any bot is to engage your user in a meaningful conversation. One of the best ways to achieve this goal is to ensure that from the moment a user first connects to your bot, they understand your bot’s main purpose and capabilities.  We refer to this as "welcoming the user."  The Core Bot  template uses an [Adaptive Card][42] to implement this behavior.  |
| [Language understanding using LUIS][43] | The ability to understand what your user means conversationally and contextually can be a difficult task, but can provide your bot a more natural conversation feel. Language Understanding, called LUIS, enables you to do just that so that your bot can recognize the intent of user messages, allow for more natural language from your user, and better direct the conversation flow. |
| [Multi-turn conversation support using Dialogs][44] | The ability to manage conversations is an important part of the bot/user interation.  Bot Framework introduces the  concept of a Dialog to handle this conversational pattern.  Dialog objects process inbound Activities and generate outbound responses. The business logic of the bot runs either directly or indirectly within Dialog classes.  |
| [Managing conversation state][45] | A key to good bot design is to track the context of a conversation, so that your bot remembers things like the answers to previous questions. |
| [How to handle user-initiated interruptions][46] | While you may think that your users will follow your defined conversation flow, step by step, chances are good that they will change their minds or ask a question in the middle of the process instead of answering the question. Handling interruptions means making sure your bot is prepared to handle situations like this. |
| [How to unit test a bot][47] | Optionally, the Core Bot template can generate a corresponding test project that shows how to use the testing framework introduced in Bot Framework version 4.5.  The testing project provides a complete set of units tests for Core Bot.  It shows how to write unit tests to test the various features of Core Bot. To generate a corresponding test project for Core Bot, run the template with the `--include-tests` command line option.  See below for an example of how to do this.  |

### Empty Bot Template
The Empty Bot template is the minimal skeleton code for a bot.  It provides a stub `onTurn` handler but does not perform any actions.  If you are experienced writing bots with Bot Framework v4 and want the minimum scaffolding, the Empty Bot template is for you.


## Features by Template
|  Feature  | Empty Bot | Echo Bot | Core Bot |
| --------- | :-------: | :------: | :------: |
| Generate code in C# | X | X | X |
| Support local development and testing using the [Bot Framework Emulator v4][3] | X | X | X |
| Core bot message processing |  | X | X |
| Deploy your bot to Microsoft Azure |  | X | X |
| Welcome new users using Adaptive Card technology |  |  | X |
| Support AI-based greetings using [LUIS][2] |  |  | X |
| Use Dialogs to manage more in-depth conversations |  |  | X |
| Manage conversation state |  |  | X |
| Handle user interruptions |  |  | X |
| Unit test a bot using Bot Framework Testing framework (optional) |  |  | X |


## Installation
1.  Install [.NET Core SDK][4] version 2.1 or higher
	```bash
	# determine dotnet version
	dotnet --version
	```
1.  Install Bot Framework CSharp templates by typing the following in your console:
    ```bash
    # Installs all three templates (echo, core, empty)
    dotnet new -i Microsoft.Bot.Framework.CSharp.EchoBot
    dotnet new -i Microsoft.Bot.Framework.CSharp.CoreBot
    dotnet new -i Microsoft.Bot.Framework.CSharp.EmptyBot
    ```
1.  Verify the templates have been installed correctly by typing the following into your console:
    ```bash
    dotnet new --list
    ```
    Sample output from `dotnet new --list`.  **Note:**  _Version information for the templates may be different._

    ```
    Templates                                         Short Name         Language          Tags
    ----------------------------------------------------------------------------------------------------------------------------
    Bot Framework Core Bot (v0.1.2)                   corebot            [C#]              Bot/Bot Framework/AI/Core Bot
    Bot Framework Echo Bot (v0.1.2)                   echobot            [C#]              Bot/Bot Framework/AI/Echo Bot
    Bot Framework Empty Bot (v0.1.2)                  emptybot           [C#]              Bot/Bot Framework/AI/Empty Bot
    Console Application                               console            [C#], F#, VB      Common/Console
    Class library                                     classlib           [C#], F#, VB      Common/Library
    Unit Test Project                                 mstest             [C#], F#, VB      Test/MSTest
    NUnit 3 Test Project                              nunit              [C#], F#, VB      Test/NUnit
    NUnit 3 Test Item                                 nunit-test         [C#], F#, VB      Test/NUnit
    xUnit Test Project                                xunit              [C#], F#, VB      Test/xUnit
    Razor Page                                        page               [C#]              Web/ASP.NET
    ```


### Alternate Installation
The above installation steps will install all three Bot Framework templates.  If you prefer to install one template or a subset of the three templates, install them individually by following the steps below.

```bash
# Install EchoBot template
dotnet new -i Microsoft.Bot.Framework.CSharp.EchoBot
```

```bash
# Install CoreBot template
dotnet new -i Microsoft.Bot.Framework.CSharp.CoreBot
```

```bash
# Install EmptyBot template
dotnet new -i Microsoft.Bot.Framework.CSharp.EmptyBot
```



## Usage
### Creating a New Bot Project

#### Create EchoBot
```bash
# Generate an Echo Bot
dotnet new echobot -n MyEchoBot
```

#### Create CoreBot
```bash
# Generate a Core Bot
dotnet new corebot -n MyCoreBot
```

#### Create CoreBot with CoreBot.Test project
```bash
# Generate a Core Bot
dotnet new corebot -n MyCoreBotWithTests --include-tests
```

#### Create EmptyBot
```bash
# Generate an Empty Bot
dotnet new emptybot -n MyEmptyBot
```

## Overridding .NET Core 2.2 Dependencies
The templates default to using .NET Core 2.2.x.  This can be overridden on the command line by using the `--framework` option. The current templates support `netcoreapp2.1` and `netcoreapp2.2` (the default).

Here are some different examples that show how to specify different .NET Core dependencies:

### Example Creating EchoBot Using .NET Core 2.1
```bash
# Generate an Echo Bot (netcoreapp2.1)
dotnet new echobot --framework netcoreapp2.1 -n MyEchoBot
```

### Example Creating CoreBot and CoreBot.Tests Using .NET Core 2.1
```bash
# Generate an Core Bot and Core Bot Tests (netcoreapp2.1)
dotnet new corebot --framework netcoreapp2.1 -n MyCoreBotWithTests --include-tests
```

## Running Your Bot
### Running Your Bot Locally
To run your bot locally, type the following in your console:

```bash
# change into the project's folder.  e.g. EchoBot
cd EchoBot
```

```bash
# run the bot
dotnet run
```

### Interacting With Your Bot Using the Emulator
- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

Once the Emulator is connected, you can interact with and receive messages from your bot.

### Developing Your Bot Locally
It's often easier to develop the capabilities of your bot locally, and to use the Bot Framework Emulator to test your changes.

## Testing CoreBot
If you use the `--include-tests` command line option when you generated a CoreBot.  The CoreBot template will generate two projects `CoreBot` and `CoreBot.Test`.  To run the CoreBot unit tests:

```bash
# build the CoreBot project
cd CoreBot
dotnet build
```

```bash
# build the CoreBot.Tests project
cd ../CoreBot.Tests
dotnet build
```

```bash
# run the unit tests
dotnet test
```

## Deploy Your Bot to Azure
After creating the bot and testing it locally, you can deploy it to Azure to make it accessible from anywhere.
To learn how, see [Deploy your bot to Azure][50] for a complete set of deployment instructions.

If you are new to Microsoft Azure, please refer to [Getting started with Azure][5] for guidance on how to get started on Azure.

## Optionally Using Development Builds
Development builds are based off of "work in progress" code.  This means they may or may not be stable and may have incomplete documentation.  These builds are better suited for more experienced users and developers, although everyone is welcome to give them a shot and provide feedback.

You can get the latest development builds from the [BotBuilder MyGet][51] feed.  The development builds for each of the templates can be found below:

| Template | Development Build Page |
| -------- | -------------------------- |
| Echo&nbsp;Bot | https://aka.ms/dotnetcore-echobot-daily |
| Core&nbsp;Bot | https://aka.ms/dotnetcore-corebot-daily |
| Empty&nbsp;Bot | https://aka.ms/dotnetcore-emptybot-daily |



To install the latest development build:
```bash
# install the development build of Echo Bot template
dotnet new -i Microsoft.Bot.Framework.CSharp.EchoBot --nuget-source https://botbuilder.myget.org/F/aitemplates/api/v3/index.json
```

To see a list of currently installed templates:
```bash
# list installed templates
dotnet new --list
```

To uninstall the development build:
```bash
# uninstall the development build of Echo Bot template
dotnet new -u Microsoft.Bot.Framework.CSharp.EchoBot
```

## Logging Issues and Providing Feedback
Issues and feedback about the .NET Core Templates can be submitted through the project's [GitHub Issues][12] page.


[1]: https://dev.botframework.com
[2]: https://www.luis.ai
[3]: https://www.github.com/microsoft/botframework-emulator
[4]: https://portal.azure.com
[5]: https://azure.microsoft.com/get-started/
[12]: https://github.com/Microsoft/botbuilder-samples/issues
[40]: https://docs.microsoft.com/azure/bot-service/bot-builder-howto-send-messages?view=azure-bot-service-4.0
[41]: https://docs.microsoft.com/azure/bot-service/bot-builder-send-welcome-message?view=azure-bot-service-4.0
[42]: https://docs.microsoft.com/azure/bot-service/bot-builder-send-welcome-message?view=azure-bot-service-4.0?#using-adaptive-card-greeting
[43]: https://docs.microsoft.com/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0
[44]: https://docs.microsoft.com/azure/bot-service/bot-builder-concept-dialog?view=azure-bot-service-4.0
[45]: https://docs.microsoft.com/azure/bot-service/bot-builder-howto-v4-state?view=azure-bot-service-4.
[46]: https://docs.microsoft.com/azure/bot-service/bot-builder-howto-handle-user-interrupt?view=azure-bot-service-4.0
[47]: https://aka.ms/cs-unit-test-docs
[50]: https://aka.ms/azuredeployment
[51]: https://botbuilder.myget.org/gallery/aitemplates

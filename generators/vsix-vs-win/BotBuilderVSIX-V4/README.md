# Microsoft Bot Builder V4 Templates
The Microsoft Bot Builder V4 Templates are available for .NET, and will let you quickly set up a conversational AI bot
using core AI capabilities. They are available as a [VSIX][1] package.

# About
Microsoft Bot Builder V4 Templates will help you build new conversational AI bots using the [Microsoft Bot Framework v4][2].

# Templates
The extension supports three different template options.  The table below can help guide which template is right for you.

|  Template  |  Description  |
| ---------- |  ---------  |
| Empty | A good template if you are familiar with Bot Framework v4, and simple want a basic skeleton project.  Also a good option if you want to take sample code from the documentation and paste it into a minimal bot in order to learn. |
| Echo | A good template if you want a little more than "Hello World!", but not much more.  This template handles the very basics of sending messages to a bot, and having the bot process the messages by repeating them back to the user.  This template produces a bot that simply "echos" back to the user anything the user says to the bot. |

## How to Choose a Template
| Template | When This Template is a Good Choice |
| -------- | -------- |
| Echo   | You are new to Bot Framework v4 and want a working bot with minimal features. |
| Empty  | You are a seasoned Bot Framework v4 developer.  You've built bots before, and want the minimum skeleton of a bot. |

## Template Overview
### Echo Template
The Echo bot template is slightly more than the a classic "Hello World!" example, but not by much.  This template shows the basic structure of a bot, how a bot recieves messages from a user, and how a bot sends messages to a user.  The bot will "echo" back to the user, what the user says to the bot.  It is a good choice for first time, new to Bot Framework v4 developers.

### Empty Template
The Empty bot template is the minimal skeleton code for a bot.  It provides a stub `onTurn` handler but does not perform any actions.  If you are experienced at writing bots with Bot Framework v4 and want the minimum scaffolding, the Empty template is for you.

# Installation

Prerequisites

* [Visual Studio 2017 or greater][3]
* [Azure account][4]
* [Azure PowerShell][5]

Install the template.

* You can download Microsoft Bot Builder V4 Templates [here][6]. Click the `.vsix` file to install the extension into Visual Studio.

# Usage
## Creating a New Bot Project
In Visual Studio go to **File** > **New** > **Project...**.

Select template of choice from Bot Builder v4 templates, then click **OK**.

# Running Your Bot
## Running Your Bot Locally

## Visual Studio
* Navigate to the folder containing the `.csproj` file and open it in Visual Studio.
* Run the project (press `F5` key)

## .NET Core CLI
* Install the [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x).
* Using the command line, navigate to your project's root folder.
* Type `dotnet run`.

## Interacting With Your Bot Using the Emulator
Launch the [Microsoft Bot Framework Emulator v4][7] and open the generated project's `.bot` file.

Once the Emulator is connected, you can interact with and receive messages from your bot.

# Deploying Your Bot to Microsoft Azure
See [Deploy your C# bot to Azure][8] for instructions.

The deployment process assumes you have an account on Microsoft Azure and are able to log into the [Microsoft Azure Portal][9].

If you are new to Microsoft Azure, please refer to [Getting started with Azure][10] for guidance on how to get started on Azure.

# Logging Issues and Providing Feedback
Issues and feedback about the templates can be submitted through the project's [GitHub issues][11] page.


[1]: https://docs.microsoft.com/en-us/visualstudio/extensibility/anatomy-of-a-vsix-package?view=vs-2017
[2]: https://dev.botframework.com
[3]: https://visualstudio.microsoft.com/downloads/
[4]: https://azure.microsoft.com/en-us/free/
[5]: https://docs.microsoft.com/en-us/powershell/azure/overview?view=azurermps-6.13.0&viewFallbackFrom=azurermps-6.8.1
[6]: https://marketplace.visualstudio.com/items?itemName=BotBuilder.botbuilderv4
[7]: https://www.github.com/microsoft/botframework-emulator
[8]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-deploy-azure?view=azure-bot-service-4.0
[9]: https://portal.azure.com
[10]: https://azure.microsoft.com/get-started/
[11]: https://github.com/Microsoft/botbuilder-samples/issues

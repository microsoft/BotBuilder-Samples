# Prompt users for input

This sample demonstrates how to create your own prompts with an ASP.Net Core 2 bot.
The bot maintains conversation state to track and direct the conversation and ask the user questions.
The bot maintains user state to track the user's answers.

# To try this sample

- Clone the samples repository

```bash
git clone https://github.com/Microsoft/BotFramework-Samples.git
```

- [Optional] Update the `appsettings.json` file under `BotFramework-Samples\SDKV4-Samples\dotnet_core\PromptUsersForInput\` with your botFileSecret.

# Prerequisites

## Visual Studio

- Navigate to the v4 C# samples folder (`BotFramework-Samples\SDKV4-Samples\dotnet_core\PromptUsersForInput`) and open the PromptUsersForInput.csproj in Visual Studio.
- Hit F5.

## Visual Studio Code

- Open the `BotFramework-Samples\SDKV4-Samples\dotnet_core\PromptUsersForInput` folder.
- Bring up a terminal, navigate to `BotFramework-Samples\SDKV4-Samples\dotnet_core\PromptUsersForInput` folder.
- Type 'dotnet run'.

## Testing the bot using Bot Framework Emulator

[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://aka.ms/botframeworkemulator).

## Connect to bot using Bot Framework Emulator **V4**

- Launch the Bot Framework Emulator.

- Select the **File** > **Open bot configuration** menu item and navigate to the `BotFramework-Samples\SDKV4-Samples\dotnet_core\PromptUsersForInput` folder.
- Select the `custom-prompt.bot` file.

# Bot state

A bot is inherently stateless. Once your bot is deployed, it may not run in the same process or on the same machine from one turn to the next.
However, your bot may need to track the context of a conversation, so that it can manage its behavior and remember answers to previous questions.

In this example, the bot's state is used to a track number of messages.

- We use the bot's turn handler and user and conversation state properties to manage the flow of the conversation and the collection of input.
- We ask the user a series of questions; parse, validate, and normalize their answers; and then save their input.

This sample is intended to be run and tested locally and is not designed to be deployed to Azure.

# Further reading

- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction)

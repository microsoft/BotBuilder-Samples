# Save user and conversation data

This sample demonstrates how to save user and conversation data in an ASP.Net Core 2 bot.
The bot maintains conversation state to track and direct the conversation and ask the user questions.
The bot maintains user state to track the user's answers.

# To try this sample

- Clone the samples repository

```bash
git clone https://github.com/Microsoft/BotFramework-Samples.git
```

- [Optional] Update the `appsettings.json` file under `BotFramework-Samples\SDKV4-Samples\dotnet_core\BotState\` with your botFileSecret.

# Prerequisites

## Visual Studio

- Navigate to the v4 C# samples folder (`BotFramework-Samples\SDKV4-Samples\dotnet_core\BotState`) and open the PromptUsersForInput.csproj in Visual Studio.
- Hit F5.

## Visual Studio Code

- Open the `BotFramework-Samples\SDKV4-Samples\dotnet_core\BotState` folder.
- Bring up a terminal, navigate to `BotFramework-Samples\SDKV4-Samples\dotnet_core\BotState` folder.
- Type 'dotnet run'.

## Testing the bot using Bot Framework Emulator

[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://aka.ms/botframeworkemulator).

## Connect to bot using Bot Framework Emulator **V4**

- Launch the Bot Framework Emulator.

- Select the **File** > **Open bot configuration** menu item and navigate to the `BotFramework-Samples\SDKV4-Samples\dotnet_core\BotState` folder.
- Select the `state.bot` file.

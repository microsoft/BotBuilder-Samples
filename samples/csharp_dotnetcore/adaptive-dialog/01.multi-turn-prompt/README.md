# multi-turn prompt

This sample demonstrates using [Adaptive dialog][1] and [Language Generation][2] features to achieve the same functionality that the waterfall based cards sample [here][3].

This bot will ask for the user's name and age, then store the responses. It demonstrates a multi-turn dialog flow using a text prompt, a number prompt, and state accessors to store and retrieve values.

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `samples/csharp_dotnetcore/adaptive-dialog/01.multi-turn-prompt`
- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/adaptive-dialog/01.multi-turn-prompt` folder
  - Select `MultiTurnPromptBot.csproj` file
  - Press `F5` to run the project


## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Additional resources
- [Adaptive dialogs](https://aka.ms/adaptive-dialogs)
- [.lu file format](https://aka.ms/lu-file-format)
- [.lg file format](https://aka.ms/lg-file-format)
- [.qna file format](https://aka.ms/qna-file-format)

[1]:https://aka.ms/adaptive-dialogs
[2]:https://aka.ms/language-generation
[3]:../05.multi-turn-prompt
[4]:https://botbuilder.myget.org/gallery/botbuilder-declarative

This sample demonstrates using [Adaptive dialog][1],  [Language Generation][2] PREVIEW features with [LUIS][5] to demonstrate an end-to-end ToDo bot in action.

This sample uses preview packages available on the [BotBuilder MyGet feed][4].

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```
- Create a [LUIS application](#LUIS-Setup)

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `experimental/adaptive-dialog/csharp_dotnetcore/todo-bot`
- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `experimental/adaptive-dialog/csharp_dotnetcore/todo-bot` folder
  - Select `ToDoBotWithLUIS.csproj` file
  - Press `F5` to run the project

  
## To debug adaptive dialogs
- You can install and use [this visual studio code extension][extension] to debug Adaptive dialogs. 

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## LUIS Setup
### Using LUIS portal
- Navigate and sign in to [Luis.ai][5]
- Under "My apps", click on "Import new app"
- Click on "Choose app file (JSON format) ..."
- Select `botbuilder-samples/experimental/adaptive-dialog/csharp_dotnetcore/todo-bot/CognitiveModels/ToDoLuisBot.luis.json
- Once the application is imported
    - Click on 'Train' to train the application
    - Click on 'Publish' to publish the application.
- Update appsettings.json
    - You can get your 'Authoring key' by following instructions [here][9]
    - You can get your application id and endpoint region by following instructions [here][10]

### Using CLI
- Install [nodejs][8] version 8.5 or higher
- Install [botbuilder-tools][7] CLI
```bash
> npm i -g ludown luis-apis
```
- In a command prompt, navigate to `botbuilder-samples/experimental/adaptive-dialog/csharp_dotnetcore/todo-bot`
- To parse ToDoBot.lu to a LUIS json model
```bash
> ludown parse toluis --in ./Dialogs/RootDialog/RootDialog.lu -o CognitiveModels -n ToDoLuisBot --out ToDoLuisBot.luis.json
```
- To create a new LUIS application using this model. Note: You see [here][9] for instructions on getting your authoirng key.
```bash
> luis import application --in ./CognitiveModels/ToDoLuisBot.luis.json --authoringKey <YOUR-AUTHORING-KEY>
```
- Copy the relevant Application Id, endpoint information as well as your authoring key to appsettings.json.
- To train and publish the LUIS application,
```bash
> luis train version --appId <YOUR-APP-ID> --versionId 0.1 --wait --authoringKey <YOUR-AUTHORING-KEY>
> luis publish version --appId <YOUR-APP-ID> --versionId 0.1 --wait --authoringKey <YOUR-AUTHORING-KEY>
```

[1]:../../README.md
[2]:../../language-generation/README.md
[3]:../../../../samples/csharp_dotnetcore/06.using-cards
[4]:https://botbuilder.myget.org/gallery/botbuilder-declarative
[5]:https://luis.ai
[6]:#LUIS-Setup
[7]:https://github.com/Microsoft/botbuilder-tools
[8]:https://nodejs.org/en/
[9]:https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-how-to-account-settings#authoring-key
[10]:https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-concept-keys
[extension]:https://marketplace.visualstudio.com/items?itemName=tomlm.vscode-dialog-debugger
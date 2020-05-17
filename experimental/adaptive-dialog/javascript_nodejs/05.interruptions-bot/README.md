# Interruptions Bot

Bot Framework v4 Interruption bot sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a bot that uses [LUIS](https://luis.ai) and adaptive dialogs to achieve advanced LU concepts such as interruption handling, flexible entity extraction.

## Prerequisites

- [Node.js](https://nodejs.org) version 10.14 or higher

    ```bash
    # determine node version
    node --version
    ```
- Install required [Luis applications](#LUIS-Setup) required for this sample.

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `experimental/adaptive-dialog/javascript_nodejs/05.interruptions-bot`

    ```bash
    cd experimental/adaptive-dialog/javascript_nodejs/05.interruptions-bot
    ```

- Install modules

    ```bash
    npm install
    ```

- Start the bot

    ```bash
    npm start
    ```

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## LUIS Setup
### Using CLI
- Install [nodejs][2] version 10.14 or higher
- Install required CLI tools
```bash
> npm i -g @microsoft/botframework-cli
```
- In a command prompt, navigate to `experimental/adaptive-dialog/javascript_nodejs/05.interruptions-bot`
- Run luis:build to create/ update, train and publish LUIS applications for each .lu file for this bot. 
- Get your [LUIS authoring key](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/luis-concept-keys)
```bash
> bf luis:build --authoringKey <your-key> --botName InterruptionBotSample --in . --out generated --log
```
- This command writes out a bunch of .dialog files (which are useful if you are using declarative form of adaptive dialogs) as well as luis.settings.\<youralias>.\<region>.json file. 
- Add the application IDs for the created applications from luis.settings.\<youralias>.\<region>.json to appsettings.

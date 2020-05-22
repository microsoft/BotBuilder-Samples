# QnAMaker Bot

Bot Framework v4 qnamaker bot sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to consume [QnAMaker.ai](https://qnamaker.ai) KBs within your adaptive dialog based bot.

## Prerequisites

- [Node.js](https://nodejs.org) version 10.14 or higher

    ```bash
    # determine node version
    node --version
    ```
- Install required [QnA Maker KB](#QnAMaker-Setup) required for this sample.

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `experimental/adaptive-dialog/javascript_nodejs/07.qnamaker`

    ```bash
    cd experimental/adaptive-dialog/javascript_nodejs/07.qnamaker
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

## QnAMaker Setup
### Using CLI
- Install [nodejs][2] version 10.14 or higher
- Install required CLI tools
```bash
> npm i -g @microsoft/botframework-cli
```
- In a command prompt, navigate to `07.qnamaker`
- Run qnamaker:build to create/ update, train and publish QnA Maker KBs required to run this bot. The content for the KB comes from .qna files under dialogs.
- Get your [QnA Maker subscription key](https://docs.microsoft.com/en-us/azure/cognitive-services/QnAMaker/how-to/set-up-qnamaker-service-azure#create-a-new-qna-maker-service)
```bash
> bf qnamaker:build --subscriptionKey <your-key> --botName QnAMakerBotSample --in . --out generated --log 
```
- This command writes out a bunch of .dialog files (which are useful if you are using declarative form of adaptive dialogs) as well as qnamaker.settings.\<youralias>.\<region>.json file. 
- Add the KB IDs for the created applications from qnamaker.settings.\<youralias>.\<region>.json to appsettings.json
```jsonc
// Example qnamaker.settings.<alias>.<region>.json file.
{
    "qna": {
        // set this value to KnowledgeBaseId in .env file.
        "QnAMakerSample_en_us_qna": "",
        // set this value to HostName in .env file.
        "hostname": ""
    }
}
```
- Get your EndpointKey using the following command
```bash
> bf qnamaker:endpointkeys:list --subscriptionKey <your-subscription-key>
```
This command will write out your primary and secondary keys. Copy either key to `EndpointKey` in the .env file.

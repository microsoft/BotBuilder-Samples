# basic-bot
This bot has been created using [Microsoft Bot Framework](https://dev.botframework.com) to demonstrate:
- [LUIS](https://luis.ai) to implement core AI capabilities
- A multi-turn conversation using Dialogs
- Handling user interruptions for Help and Cancel
- Prompting for and validating requests for information from the user

# Prerequisites
- Ensure you have [Node](https://nodejs.org/en/download/) installed in order to run [Bot Builder Tools](https://github.com/Microsoft/botbuilder-tools).
- Ensure you have the [Azure AZ CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) installed.
- Ensure you have the Bot Builder MSBot tool via this command:
```bash
npm install -g msbot
```

## Setting up LUIS
Ensure you have a LUIS account and know the authoring key.
- Navigate to [LUIS portal](https://www.luis.ai).
- Click the `Sign in` button and login.
- Click on your name in the upper right hand corner and the `Settings` drop down menu.
- Copy the `Authoring Key` to the LUIS_AUTHORING_KEY environment variable.
  
# To try this sample
Open your shell and follow these steps:
- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- Navigate to sample
```bash
cd 13.Basic-Bot-Template
```
- Create and provision services for your sample bot.
```bash
msbot clone services --location westus --luisAuthoringKey %LUIS_AUTHORING_KEY% --name basic-bot --folder DeploymentScripts/MsbotClone
```

This will result in the creation of a basic-bot.bot file that keeps track of all your service information to use in your code and that can be used to debug with the Bot Builder Emulator.  

## Visual Studio
- Open BasicBot.csproj to launch Visual Studio
- Press F5 to build and run the sample.

## Visual Studio Code
- Launch Visual Studio Code in the sample directory.
```bash
code
```
- Hit F5 to build and run your bot.

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://aka.ms/botframework-emulator) is a desktop application that allows bot developers to test and debug
their bots on localhost or running remotely through a tunnel.
- Install the [Bot Framework Emulator](https://aka.ms/botframework-emulator).
### Connect to bot using Bot Framework Emulator
- Launch the Bot Framework Emulator
- File -> Open bot and navigate to 13.Basic-Bot-Template
- Select `basic-bot.bot` file
- You can then interact and debug your bot throgh the emulator.

# Further reading
- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot basics](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [LUIS](https://luis.ai)
- [Prompt Types](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0&tabs=javascript)
- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [QnA Maker](https://qnamaker.ai)


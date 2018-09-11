# How To Deploy
This bot relies on the [LUIS][1] cognitive service to function. This document will show how to use simple command line tools to setup LUIS for use with this bot.

# Install required tools
To successfully setup and configure the services this bot depends on, you need to install the MSBOT, LUIS, and Ludown CLI tools.  See the documentation for the [Bot Framework CLI Tools][5] for additional information on what CLI tools are available to help you build your bot.

```bash
npm i -g msbot luis-apis ludown
```

# Keeping track of service references using .bot file
We highly recommend you can keep track of all the services your bot depends on in a .bot file. You can use either the `msbot` CLI tool or use [Bot Framework Emulator][7] to manage your .bot file.  This project was created with a bot file named `basic-bot.bot`

# Configure the LUIS service
To create a new LUIS application for this bot,
- Create an account with [LUIS][1]. If you already have an account, login to your account.
- Click on your name on top right corner of the screen -> settings and grab your authoring key.

To create the LUIS application this bot uses a .bot file to store dependent service configuration.  To update the .bot file run the following commands from a terminal window:
- Navigate to the basic-bot folder
- Run the following command.  Be sure to specify your LUIS authoring key.
```bash
luis import application --in cognitiveModels\greeting.luis --authoringKey <YOUR-LUIS-AUTHORING-KEY> --endpointBasePath https://westus.api.cognitive.microsoft.com/luis/api/v2.0 --msbot | msbot connect luis --stdin --name basic-bot-LUIS
```

Note: Should you so desire, you can create the LUIS application in a region of your choice. See [here][2] to learn more about available LUIS authoring regions. You can specify a different region (westus or westeurope or australiaeast) via https://**LUIS-Authoring-Region**.api.cognitive.microsoft.com/luis/api/v2.0 in the above command.

## Train and publish the LUIS models
You need to train and publish the LUIS models that were created for this sample to work. You can do so using the following CLI commands

```bash
msbot get service --name "basic-bot-LUIS" | luis train version --wait --stdin
msbot get service --name "basic-bot-LUIS" | luis publish version --stdin
```

[1]: https://www.luis.ai
[2]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-concept-intelligence
[3]: https://portal.azure.com
[4]: https://azure.microsoft.com/en-us/get-started/
[5]: https://github.com/microsoft/botbuilder-tools
[6]: https://dev.botframework.com
[7]: https://www.github.com/microsoft/botframework-emulator

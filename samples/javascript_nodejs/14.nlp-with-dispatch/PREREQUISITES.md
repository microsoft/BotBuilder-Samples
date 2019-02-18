# Bot Prerequisites
This bot has prerequisites that must be installed in order for the bot to function properly.

This document will enumerate the required prerequisites and show how to install them.

## Overview
This bot uses [QnA Maker Service][1], an AI based cognitive service, to implement a powerful question and answer service from your semi-structured content.  The Bot Framework provides a set of CLI tools that will help setup QnA Maker so the bot can be run and tested locally.  Additionally, prerequisites are provided that will enable the bot to be deployed to Azure using additional CLI tools.

## Prerequisites
- [Node.js][4] version 10.14 or higher.
- If you don't have an Azure subscription, create a [free account][5].
- Install the latest version of the [Azure CLI][6] tool. Version 2.0.54 or higher.
- Install latest version of the `MSBot` CLI tool. Version 4.3.2 or higher.
    ```bash
    # install msbot CLI tool
    npm install -g msbot
    ```
- Install latest version of the `QnAMaker` CLI tool. Version 1.1.0 or higher.
    ```bash
    # install QnA Maker CLI tool
    npm install -g qnamaker
    ```
- Install latest version of `luis api` tool.  Version 2.2.0 or higher.
    ```bash
    # install the luis api CLI tool
    npm install -g luis-apis
    ```
- Install [.NET Core version 2.2][8] or higher (required for Dispatch CLI)
- Install latest version of the `Dispatch` tool. Version 1.2.0 or higher.
    ```bash
    # install the dispatch CLI tool
    npm install -g botdispatch
    ```
- If you don't have a LUIS Account, create a free LUIS Account.
    - Navigate to [LUIS portal][9].
    - Click the `Login / Sign up` button.
    - Click `Create a LUIS app now` button.
    - From the `My Apps` page, click your account name in the upper right of the main menu.
    - Click `Settings` to display the User Settings page.
    - Copy the `Authoring Key`, which you will need to run CLI tools.


You now have installed all the prerequites.

[Return to README.md][3]

# Further reading
[Dispatch CLI][1] is a tool to create and evaluate LUIS models used for NLP (Natural Language Processing). Dispatch works across multiple bot modules such as LUIS applications, QnA knowledge bases and other NLP sources (added to dispatch as a file type).

Use the Dispatch model in cases when:
- Your bot consists of multiple modules and you need assistance in routing user's utterances to these modules and evaluate the bot integration.
- Evaluate quality of intents classification of a single LUIS model.
- Create a text classification model from text files.



[1]: https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Dispatch
[3]: ./README.md
[4]: https://nodejs.org
[5]: https://azure.microsoft.com/free/
[6]: https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest
[7]: https://www.qnamaker.ai
[8]: https://dotnet.microsoft.com/download
[9]: https://luis.ai

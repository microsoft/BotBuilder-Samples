# Bot Prerequisites
This bot has prerequisites that must be installed in order for the bot to function properly.

This document will enumerate the required prerequisites and show how to install them.

## Overview
This bot uses [QnA Maker][1] to power a question and answer service from your semi-structured content.  The Bot Framework provides a set of CLI tools that will help setup QnA Maker so the bot can be run and tested locally.  Additionally, prerequisites are provided that will enable the bot to be deployed to Azure using additional CLI tools.

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

[Return to README.md][3]

[1]: https://www.qnamaker.ai
[3]: ./README.md
[4]: https://nodejs.org
[5]: https://azure.microsoft.com/free/
[6]: https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest
[7]: https://www.qnamaker.ai/Create

# Bot Prerequisites
This bot has prerequisites that must be installed in order for the bot to function properly.

This document will enumerate the required prerequisites and show how to install them.

## Overview
This bot uses [Azure Bot Service][1] to configure an authentication channel.  The authentication channel will provide the capabilities of handling the OAuth flow for our bot.  The prerequisites outlined below will provide the CLI tools and account information the sample will use to provision an Azure Bot Service.  Once the Azure Bot Service has been provisioned, the sample will walk you through the steps required to setup an authentication channel.

## Prerequisites
- [Node.js][4] version 8.5 or higher.
- If you don't have an Azure subscription, create a [free account][5].
- Install the latest version of the [Azure CLI][6] tool. Version 2.0.52 or higher.
- Install the latest `botservice` extension for the Azure CLI tool.  Version 0.4.3 or higher.
    1. Ensure any previous version of the `botservice` extension is removed.
        ```bash
        az extension remove -n botservice
        ```
    1. Install the latest `botservice` extension.
        ```bash
        az extension add -n botservice
        ```
    1. Verify you are running version 0.4.3 or higher of the `botservice` extension.
        ```bash
        az extension list
        ```
        To verify the extension version, you should see the following:
        ```json
        {
            "extensionType": "whl",
            "name": "botservice",
            "version": "0.4.3"
        }
        ```
- Install latest version of the `MSBot` CLI tool. Version 4.2.0 or higher.
    ```bash
    npm install -g msbot
    ```
[Return to README.md][3]


[1]: https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0
[3]: ./README.md
[4]: https://nodejs.org
[5]: https://azure.microsoft.com/free/
[6]: https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest

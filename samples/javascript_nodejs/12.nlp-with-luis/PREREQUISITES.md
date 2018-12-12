# Bot Prerequisites
This bot has prerequisites that must be installed in order for the bot to function properly.

This document will enumerate the required prerequisites and show how to install them.

## Overview
This bot uses [LUIS][1], an AI based cognitive service, to implement language understanding.  The Bot Framework provides a set of CLI tools that will help setup LUIS so the bot can be run and tested locally.  Additionally, prerequisites are provided that will enable the bot to be deployed to Azure using additional CLI tools.

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
- If you don't have a LUIS Account, create a free LUIS Account.
    - Navigate to [LUIS portal][1].
    - Click the `Login / Sign up` button.
    - Click `Create a LUIS app now` button.
    - From the `My Apps` page, click your account name in the upper right of the main menu.
    - Click `Settings` to display the User Settings page.
    - Copy the `Authoring Key`, which you will need to run CLI tools.

[Return to README.md][3]


[1]: https://www.luis.ai
[3]: ./README.md
[4]: https://nodejs.org
[5]: https://azure.microsoft.com/free/
[6]: https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest

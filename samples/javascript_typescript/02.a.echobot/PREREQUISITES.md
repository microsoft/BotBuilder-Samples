# Azure Deployment Prerequisites
This bot has prerequisite requirements in order to deploy the bot to Azure.

This document will enumerate the required prerequisites and show how to install them.

## Overview
There are a small set of CLI tools that will automate the process of deploying this bot to Azure.  These CLI tools are only require for deployment.  If you only plan to run the bot locally, these prerequisites are not required.

## Prerequisites
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

[Return to README.md][3]


[3]: ./README.md
[4]: https://nodejs.org
[5]: https://azure.microsoft.com/free/
[6]: https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest

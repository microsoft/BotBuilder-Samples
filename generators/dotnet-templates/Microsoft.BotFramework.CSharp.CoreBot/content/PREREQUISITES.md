# __PROJECT_NAME__ Prerequisites
This bot has prerequisites that must be installed in order for the bot to function properly.

This document will enumerate the required prerequisites and show how to install them.

## Overview
This bot uses [LUIS][1], an AI based cognitive service, to implement language understanding.  The Bot Framework provides a set of CLI tools that will help setup LUIS so the bot can be run and tested locally.  Additionally, prerequisites are provided that will enable the bot to be deployed to Azure using additional CLI tools.

## Prerequisites
### Install .NET Core and CLI Tooling
- [.NET Core SDK][4] version __NETCORE_VERSION__
	```bash
	# determine dotnet version
	dotnet --version
	```
- If you don't have an Azure subscription, create a [free account][5].
- Install the latest version of the [Azure CLI][6] tool. Version 2.0.54 or higher.
- Install latest version of the `MSBot` CLI tool. Version 4.3.2 or higher.
    ```bash
    # install msbot CLI tool
    npm install -g msbot
    ```
- Install latest version of `luis api` tool.  Version 2.2.0 or higher.
    ```bash
    # install the luis api CLI tool
    npm install -g luis-apis
    ```

### Create a LUIS Application to enable language understanding
- If you don't have a LUIS Account, create a free LUIS Account.
    - Navigate to [LUIS portal][1].
    - Click the `Login / Sign up` button.

- From the LUIS portal, import a new LUIS App.
    - Click `Import new app`.
    - Click the `Choose app file (JSON format)...` button.
	- Navigate to `__PROJECT_NAME__/CognitiveModels` folder.
    - Select `FlightBooking.json`.
    - Click `Done` to create your app.

The above steps will create a LUIS application by importing an existing LUIS model found in `FlightBooking.json`.  This is the language model used by __PROJECT_NAME__.  The use of LUIS allows the bot to demonstrate NLP support.  The `FlightBooking.json` models language used to book a flight reservation.

- Train and Publish your LUIS application model
    - From the Intents page, click the `Train` button.
    - Wait for training to complete.
    - Click the `Publish` button to display the `Publish this app` modal.
    - Click `Publish`.

### Configure the bot to use your new LUIS app
Now that a LUIS application has been created, the bot needs to be configured to use this app.

Bot configuration settings are stored in `__PROJECT_NAME__/appsettings.json`.  There are three key/value pairs the bot requires in order to use the previously created LUIS application.  They are:

```json
    "BotServices": {
        "Luis-Booking-AppId": "",
        "Luis-Booking-AuthoringKey": "",
        "Luis-Booking-Region": ""
    }
```
To find these settings, click the `Manage` button.

- The `Application ID` can be found in `Application Information` page.
- The `Authoring Key` can be found in `Keys and Endpoints` page.
- The `Region` can be found in `Keys and Endpoints`, under the `Region` column.

Use those three values to update the `Luis-Booking-AppId`, `Luis-Booking-AuthoringKey`, and `Luis-Booking-Region` values of `__PROJECT_NAME__/appsettings.json`.

Once `__PROJECT_NAME__/appsettings.json` has been updated, the bot is ready to run using LUIS.



[Return to README.md][3]

#### Generated with `dotnet new corebot` vX.X.X

[1]: https://www.luis.ai
[3]: ./README.md
[4]: https://nodejs.org
[5]: https://azure.microsoft.com/free/
[6]: https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest

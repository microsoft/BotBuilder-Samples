# Teams Search Auth Config Bot

Bot Framework v4 sample for Teams expands the [50.teams-messaging-extensions-search](https://github.com/microsoft/BotBuilder-Samples/tree/master/samples/javascript_nodejs/50.teams-messaging-extensions-search) sample to include a configuration page and Bot Service authentication.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to use a Messaging Extension configuration page, as well as how to sign in from a search Messaging Extension.

## Prerequisites

- Microsoft Teams is installed and you have an account
- [NodeJS](https://nodejs.org/en/)
- [ngrok](https://ngrok.com/) or equivalent tunnelling solution

## To try this sample

> Note these instructions are for running the sample on your local machine, the tunnelling solution is required because
the Teams service needs to call into the bot.

1) Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

2) In a terminal, navigate to `samples/javascript_nodejs/52.teams-messaging-extensions-search-auth-config`

3) Install modules

    ```bash
    npm install
    ```

4) Run ngrok - point to port 3978

    ```bash
    ngrok http -host-header=rewrite 3978
    ```

5) Create [Bot Framework registration](https://docs.microsoft.com/en-us/microsoftteams/platform/bots/how-to/create-a-bot-for-teams#register-your-web-service-with-the-bot-framework), using the current https URL you were given by running ngrok.  NOTE: this bot requires an **OAuth Connection Settings** which can only be added from the Azure Portal. Ensure that you've [enabled the Teams Channel](https://docs.microsoft.com/en-us/azure/bot-service/channel-connect-teams?view=azure-bot-service-4.0)

6) Register an AADV2 application, and add it to the Bot Registration's **OAuth Connection Settings**. See [Add authentication to your bot via Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-authentication) for more information.

7) Update the `.env` configuration for the bot to use the app id and app password from the Bot Framework registration. (Note the app password is referred to as the client secret in the azure portal and you can always create a new client secret anytime.)  Also add the AADV2 ConnectionName, and ngrok SiteUrl.

8) *This step is specific to Teams.* **Edit** the `manifest.json` contained in the  `teamsAppManifest` folder to replace your app if from Bot Framework everywhere you see the place holder string `<<YOUR-MICROSOFT-BOT-ID>>`.
**Zip** up the contents of the `teamsAppManifest` folder to create a `manifest.zip`. **Upload** the `manifest.zip` to Teams (in the Apps view click "Upload a custom app")

9) Run your bot at the command line:

    ```bash
    npm start
    ```

## Interacting with the search/auth/config bot

Once the Messaging Extension is installed, click the icon for **Config Auth Search** in the Compose Box's Messaging Extension menu to display the search window.  Left click to choose **Settings** and view the Config page.

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Create a bot for Microsoft Teams](https://docs.microsoft.com/en-us/microsoftteams/platform/bots/how-to/create-a-bot-for-teams#register-your-web-service-with-the-bot-framework)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)

## TeamsLinkUnfurlingBot

Bot Framework v4 Teams link unfurling bot sample for Teams.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple bot that performs link unfurling in Teams.

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

1) In a terminal, navigate to `samples/javascript_nodejs/55.teams-link-unfurling`

1) Install modules

    ```bash
    npm install
    ```

1) Run ngrok - point to port 3978

    ```bash
    ngrok http -host-header=rewrite 3978
    ```
1) Create [Bot Framework registration](https://docs.microsoft.com/en-us/microsoftteams/platform/bots/how-to/create-a-bot-for-teams#register-your-web-service-with-the-bot-framework), using the current https URL you were given by running ngrok. Ensure that you've [enabled the Teams Channel](https://docs.microsoft.com/en-us/azure/bot-service/channel-connect-teams?view=azure-bot-service-4.0)

1) Update the `.env` configuration for the bot to use the app id and app password from the Bot Framework registration. (Note the app password is referred to as the client secret in the azure portal and you can always create a new client secret anytime.)

1) *This step is specific to Teams.* **Edit** the `manifest.json` contained in the  `teamsAppManifest` folder to replace your app id from Bot Framework everywhere you see the place holder string `<<YOUR-MICROSOFT-BOT-ID>>`.
**Zip** up the contents of the `teamsAppManifest` folder to create a `manifest.zip`. **Upload** the `manifest.zip` to Teams (in the Apps view click "Upload a custom app")

1) Run your bot at the command line:

    ```bash
    npm start
    ```

### Interacting with the bot

> Note the Teams `manifest.json` for this sample also inclues a Search Query. This Messaging Extension is only introduced in order to enable installation, because there is no mechanism for installing a link unfurling feature in isolation.

If you copy and paste a link from https://www.BotFramework.com into the compose message area the link will unfurl. 

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Create a bot for Microsoft Teams](https://docs.microsoft.com/en-us/microsoftteams/platform/bots/how-to/create-a-bot-for-teams#register-your-web-service-with-the-bot-framework)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)

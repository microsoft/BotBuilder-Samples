# Teams File Upload Bot

Bot Framework v4 file upload bot sample for Teams.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to
upload files to Teams from a bot and how to receive a file sent to a bot as an attachment.

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

1) In a terminal, navigate to `samples/javascript_nodejs/56.teams-file-upload`

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

1) *This step is specific to Teams.* **Edit** the `manifest.json` contained in the  `teamsAppManifest` folder to replace your app if from Bot Framework everywhere you see the place holder string `<<YOUR-MICROSOFT-BOT-ID>>`.
**Zip** up the contents of the `teamsAppManifest` folder to create a `manifest.zip`. **Upload** the `manifest.zip` to Teams (in the Apps view click "Upload a custom app")

1) Run your bot at the command line:

    ```bash
    npm start
    ```

## Interacting with the file upload bot

> Note this `manifest.json` specified that the bot will be installed in "personal" scope which is why you immediately entered a one on one chat conversation with the bot. Please refer to Teams documentation for more details.

Sending a message to the bot will cause it to respond with a card that will prompt you to upload a file. The file that's being uploaded is the `teams-logo.png` in the `Files` directory in this sample. The `Accept` and `Decline` events illustrated in this sample are specific to Teams. You can message the bot again to receive another prompt.

You can also send a file to the bot as an attachment in the message compose section in Teams. This will be delivered to the bot as a Message Activity and the code in this sample fetches and saves the file.

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Create a bot for Microsoft Teams](https://docs.microsoft.com/en-us/microsoftteams/platform/bots/how-to/create-a-bot-for-teams#register-your-web-service-with-the-bot-framework)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)



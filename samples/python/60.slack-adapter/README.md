# Slack Adapter

Bot Framework v4 echo bot using Slack Adapter sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple echo bot that connects with Slack to respond messages.

## Create a Slack Application for your bot

1. Log into [Slack](https://slack.com/signin) and then go to [create a Slack application](https://api.slack.com/apps) channel.

2. Click the 'Create new app' button.

3. Enter an App Name and select a Development Slack Team. If you are not already a member of a Development Slack Team, [create or join one](https://slack.com/).

4. Click **Create App**. Slack will create your app and generate a Client ID and Client Secret.

### Update your Slack app

Navigate back to the [Slack api dashboard]([https://api.slack.com/apps]) and select your app.  You now need to configure the URL in 2 locations for your app and subscribe to the appropriate events.

1. In the **Oauth & Permissions** tab:
  - The 'Redirect URL' should be your bots url, plus the 'api/slack' endpoint you specified in your newly created controller. e.g. https://xxxxx.ngrok.io/api/slack
  - Verify **Bot Token Scopes** for `chat:write`, `im:history`, and `im:read`

2. In the **Event Subscriptions** tab, fill in the 'Request URL' with the same URL you used in step 1.

3. Enable events using the toggle at the top of the page.

4. Expand the **'Subscribe to bot events'** section and use the 'Add Bot User Event' button to subscribe to `im_created` and `message.im`.

### Gather required configuration settings for your bot

Once your app has been created, you need to collect some information that will be needed by your bot to connect to Slack.

1. Note the 'Verification Token' and the 'Signing Secret' from the **Basic Information** tab and keep them for later when we configure our bot settings. 

2. Navigate to the **Install App** page under the 'Settings' menu and follow the instructions to install your app into a Slack team.  Once installed, copy the 'Bot User OAuth Access Token' and, again, keep this for later when we configure out bot settings.

## Add Slack app settings to your bot's configuration file

Add the 3 settings shown below to your `config.py` file in your bot project, populating each one with the values gathered earlier when creating your Slack app.

```
    SLACK_VERIFICATION_TOKEN = os.environ.get("SlackVerificationToken", "")
    SLACK_BOT_TOKEN = os.environ.get("SlackBotToken", "")
    SLACK_CLIENT_SIGNING_SECRET = os.environ.get("SlackClientSigningSecret", "")
```

### Obtain a URL for your bot

Now that you have created a Slack app and wired up the adapter in your bot project, the final step is to point the Slack app to the correct endpoint on your bot and subscribe your app to ensure your bot receives messages.  To do this your bot must be running, so that Slack can verify the URL to the endpoint is valid.

Run the bot from a terminal.

  1) From a terminal, navigate to `botbuilder-samples\samples\python\60.slack-adapter` folder
  1) Activate your desired virtual environment
  1) In the terminal, type `pip install -r requirements.txt`
  1) Run your bot with `python app.py`

Next, you can use a tool such as [Ngrok](https://www.ngrok.com) (which you will likely already have installed if you have used the Bot Framework emulator previously) to tunnel through to your bot running locally and provide you with a publicly accessible URL for this. 

Use the following command in a terminal window.

```
ngrok.exe http 3978 -host-header="localhost:3978"
```

## Test your bot in Slack

Your Slack app is now configured and you can now login to the Slack workspace you installed your app into and you will see it listed under the 'Apps' section of the left hand menu.  

Select your app and try:

* Sending a message, which you should see echoed back to you in the IM window.
* Sending an attachment.
* Sending a message containing a URL (link sharing).
* Sending the command "/test" which will prompt the sample bot to return a Slack interactive message.

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

# Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)

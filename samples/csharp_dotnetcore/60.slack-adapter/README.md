# Slack Adapter

Bot Framework v4 echo bot using Slack Adapter sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple echo bot that connects with Slack to respond messages.

## Prerequisites

* [.NET Core SDK](https://dotnet.microsoft.com/download) version 3.1
* [ngrok](https://ngrok.com/) or equivalent tunnelling solution
* Access to a Slack workspace with sufficient permissions to login to create / manage applications at  [https://api.slack.com/apps](https://api.slack.com/apps). If you do not have access to a Slack environment you can create a workspace for free at https://www.slack.com

## To try this sample

* Clone the repository

    ```bash
    git clone https://github.com/Microsoft/BotBuilder-Samples.git
    ```

### Create a Slack Application for your bot

1. Log into [Slack](https://slack.com/signin) and then go to [create a Slack application](https://api.slack.com/apps) channel.

2. Click the 'Create new app' button.

3. Enter an App Name and select a Development Slack Team. If you are not already a member of a Development Slack Team, [create or join one](https://slack.com/).

4. Click **Create App**. Slack will create your app and generate a Client ID and Client Secret.

#### Gather required configuration settings for your bot

Once your app has been created, you need to collect some information that will be needed by your bot to connect to Slack.

1. Note the 'Verification Token' and the 'Signing Secret' from the **Basic Information** tab and keep them for later when we configure our bot settings.

2. Navigate to the **Install App** page under the 'Settings' menu and follow the instructions to install your app into a Slack team.  Once installed, copy the 'Bot User OAuth Access Token' and, again, keep this for later when we configure out bot settings.

### Add Slack app settings to your bot's configuration file

Add the 3 settings shown below to your `appsettings.json` file in your bot project, populating each one with the values gathered earlier when creating your Slack app.

```json
  "SlackVerificationToken": "",
  "SlackBotToken": "",
  "SlackClientSigningSecret": ""
```

### Obtain a URL for your bot

Now that you have created a Slack app and wired up the adapter in your bot project, the final step is to point the Slack app to the correct endpoint on your bot and subscribe your app to ensure your bot receives messages.  To do this your bot must be running, so that Slack can verify the URL to the endpoint is valid.

Run the bot from a terminal or from Visual Studio:

  A) From a terminal, navigate to `samples/csharp_dotnetcore/60.slack-adapter`

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/60.slack-adapter` folder
  - Select `SlackAdapterBot.csproj` file
  - Press <kbd>F5</kbd> to run the project

Next, you can use a tool such as [Ngrok](https://www.ngrok.com) (which you will likely already have installed if you have used the Bot Framework emulator previously) to tunnel through to your bot running locally and provide you with a publicly accessible URL for this.

Use the following command in a terminal window.

```
ngrok.exe http 3978 -host-header="localhost:3978"
```

### Update your Slack app

Navigate back to the [Slack api dashboard]([https://api.slack.com/apps]) and select your app.  You now need to configure 2 URLs for your app and subscribe to the appropriate events.

1. In the **Oauth & Permissions** tab, the 'Redirect URL' should be your bots url, plus the 'api/slack' endpoint you specified in your newly created controller. e.g. https://xxxxx.ngrok.io/api/slack

2. In the **Event Subscriptions** tab, fill in the 'Request URL' with the same URL you used in step 1.

3. Enable events using the toggle at the top of the page.

4. Expand the 'Subscribe to bot events' section and use the 'Add Bot User Event' button to subscribe to 'im_created' and 'message.im'.

## Test your bot in Slack

Your Slack app is now configured and you can now login to the Slack workspace you installed your app into and you will see it listed under the 'Apps' section of the left hand menu.

Select your app and try:

* Sending a message, which you should see echoed back to you in the IM window.
* Sending an attachment.
* Sending a message containing a URL (link sharing).
* Sending the command "/test" which will prompt the sample bot to return a Slack interactive message.

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

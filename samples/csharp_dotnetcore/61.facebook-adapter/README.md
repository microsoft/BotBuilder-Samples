# Facebook Adapter

Bot Framework v4 echo bot using facebook Adapter sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple echo bot that connects with Facebook to respond to messages.

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 3.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## To try this sample

- Clone the repository

   ```bash
    git clone https://github.com/Microsoft/BotBuilder-Samples.git
   ```

- In a terminal, navigate to `BotBuilder-Samples/samples/csharp_dotnetcore/55.facebook-adapter`

- Connect the bot with Facebook by following the instructions below.

- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `BotBuilder-Samples/samples/csharp_dotnetcore/55.facebook-adapter` folder
  - Select `FacebookAdapterBot.csproj` file
  - Press <kbd>F5</kbd> to run the project


### Connect the bot with Facebook

Populate settings for Facebook App secret, Access Token and Verify Token in appsettings.json file. 

Instructions about how to create / configure a Facebook App for your bot and where to obtain the values for the settings can be found in the documentation covering [connecting a bot to Facebook using the Facebook adapter](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-facebook?view=azure-bot-service-4.0#connect-a-bot-to-facebook-using-the-facebook-adapter).

```json
{
  "FacebookVerifyToken": "",
  "FacebookAppSecret": "",
  "FacebookAccessToken": ""
}
```

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

### Testing your bot

You can test your bot is connected to Facebook correctly by sending a message via the Facebook Page you associated with your new Facebook App.  

1. Navigate to your Facebook Page.

2. Click **Add a Button** button.

![Add a button](/MicrosoftDocs/bot-docs/tree/live/articles/media/bot-service-channel-connect-facebook/add-button.png)

3. Select **Contact You** and **Send Message** and click **Next**.

![Add a button](/MicrosoftDocs/bot-docs/tree/live/articles/media/bot-service-channel-connect-facebook/button-settings.png)

4. When asked **Where would you like this button to send people to?** select **Messenger** and click **Finish**.

![Add a button](/MicrosoftDocs/bot-docs/tree/live/articles/media/bot-service-channel-connect-facebook/button-settings-2.png)

5. Hover over the new **Send Message** button that is now shown on your Facebook Page and clikc **Test Button** from the popup menu.  This will start a new conversation with your app via Facebook Messenger, which you can use to test messaging your bot. Once the message is receieved by your bot it will send a message back to you, echoing the text from your message.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)

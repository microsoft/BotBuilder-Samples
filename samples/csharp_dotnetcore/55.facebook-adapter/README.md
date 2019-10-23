# Facebook Adapter

Bot Framework v4 echo bot using facebook Adapter sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple echo bot that connects with Facebook to respond to messages.

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

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

1 - Create a Facebook Account for Developers (https://developers.facebook.com/).

2 - Create a new App. Give a name to the app and click Create App ID button.

3 - In the Dashboard go to Add a Product and select Messenger by clicking on Set Up button

    A) In the Access Tokens section, select a Facebook Page or create a new one. This is the page where the bot will be tested later.

    B) After selecting the page, the permissions should be edited, click on the button "Add or Remove Pages", select the page just created as option and continue.

    C) A Page Access Token is generated. Copy it, it will be needed to connect the adapter.

4 - Get the app credentials. Go to Settings, Basic and copy the App Secret.

5 - Set the tokens in appsettings.json file: 
    
        FacebookVerifyToken (create one. It will be used to validate received messages)
        FacebookAppSecret (the one obtained in step 4)
        FacebookAccessToken (the one obtained in step 3.c)

6 - Using a tunneling tool like [Ngrok](https://ngrok.com/download), expose the bot's endpoint.

7 - Go back to the Facebook for Developers page and click on Messenger, Settings.
    In the Webhooks section, click on Subscribe To Events button.

    A) Complete the Callback URL with the ngrok https URL adding   '/api/messages'. 
        Fill in the Verify Token with the one setted on your bot.
        Subscribe to the following events: messages, messaging_postbacks, messaging_optins, messaging_deliveries

    B) Click Verify and Save button.

8 - Subscribe the webhook to the Page.

9 - Go to the Page and click Add a Button.

    A) Select a Send Message button.

    B) Select Messenger.

    C) And click Finish button.

10 - Finally, click on the button created and test your bot!

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

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

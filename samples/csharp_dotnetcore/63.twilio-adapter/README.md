# Twilio Adapter

Bot Framework v4 echo bot using Twilio Adapter sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple echo bot that connects with Twilio to respond SMS messages.

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

- Connect the bot with Twilio by following the instructions below.

- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal, navigate to `samples/csharp_dotnetcore/63.twilio-adapter`

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/63.twilio-adapter` folder
  - Select `TwilioAdapterBot.csproj` file
  - Press <kbd>F5</kbd> to run the project

- Test the bot sending a SMS message to the Twilio Number.

### Connect the bot with Twilio

Populate settings for Twilio Number, Account SID, Auth Token and Validation URL in appsettings.json file.

Instructions about how to create / configure a Twilio number for your bot and where to obtain the values for the settings can be found in the documentation covering [connecting a bot to Twilio using the Twilio adapter](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-twilio?view=azure-bot-service-4.0#connect-a-bot-to-twilio-using-the-twilio-adapter).

```json
  "TwilioNumber": "",
  "TwilioAccountSid": "",
  "TwilioAuthToken": "",
  "TwilioValidationUrl", ""
```

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

You can now test your bot is connected to Twilio correctly by sending an SMS message to your Twilio number.  Once the message is receieved by your bot it will send a message back to you, echoing the text from your message.

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

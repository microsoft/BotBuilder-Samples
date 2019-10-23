# Twilio Adapter

Bot Framework v4 echo bot using Twilio Adapter sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple echo bot that connects with Twilio to respond SMS messages.

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

- In a terminal, navigate to `BotBuilder-Samples/samples/csharp_dotnetcore/52. twilio-adapter`

- Connect the bot with Twilio by following the instructions below.

- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `BotBuilder-Samples/samples/csharp_dotnetcore/52. twilio-adapter` folder
  - Select `TwilioAdapterBot.csproj` file
  - Press <kbd>F5</kbd> to run the project

- Test the bot sending a SMS message to the Twilio Number.

### Connect the bot with Twilio

1 - Create an Account in [Twilio](https://www.twilio.com/console).

_**Note**: It will require to validate a phone number. This number is the one to use for the testing (sending messages to Twilio)._

2 - Copy the _Account SID_ and the _Auth Token_ from the Dashboard. These credentials will be needed to connect the bot to Twilio.

3 - Get a Twilio Number (_This is the number that will receive the messages_).

_**Note**: Make sure the chosen number has SMS capability enabled._

4 - Using a tunneling tool like [Ngrok](https://ngrok.com/download), expose the bot's endpoint.

_**Note**:  Some important fact is that if ngrok will be used to test the bot locally, `http` should be used instead of the `https` in the URL._
Read more about this in the [Twilio documentation.](https://www.twilio.com/docs/usage/tutorials/how-to-secure-your-csharp-aspnet-core-app-by-validating-incoming-twilio-requests#use-the-filter-attribute-with-our-twilio-webhooks)

5 - Configure the Messaging Webhook with the URL generated in the previous step adding '/api/messages' to it.

6 - Back in the bot project, set the credentials in _appsettings.json_.

    TwilioNumber (the one obtained in step 3)
    TwilioAccountSid (the one obtained in step 2)
    TwilioAuthToken (the one obtained in step 2)
    TwilioValidationUrl (the one configured in step 5)


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
- [Restify](https://www.npmjs.com/package/restify)
- [dotenv](https://www.npmjs.com/package/dotenv)
# Proactive messages

Bot Framework v4 proactive messages bot sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to send proactive messages to users by capturing a conversation reference, then using it later to initialize outbound messages.

This sample is a Spring Boot app and uses the Azure CLI and azure-webapp Maven plugin to deploy to Azure.

## Concepts introduced in this sample

Typically, each message that a bot sends to the user directly relates to the user's prior input. In some cases, a bot may need to send the user a message that is not directly related to the current topic of conversation. These types of messages are called proactive messages.

Proactive messages can be useful in a variety of scenarios. If a bot sets a timer or reminder, it will need to notify the user when the time arrives. Or, if a bot receives a notification from an external system, it may need to communicate that information to the user immediately. For example, if the user has previously asked the bot to monitor the price of a product, the bot can alert the user if the price of the product has dropped by 20%. Or, if a bot requires some time to compile a response to the user's question, it may inform the user of the delay and allow the conversation to continue in the meantime. When the bot finishes compiling the response to the question, it will share that information with the user.

This project has a notify endpoint that will trigger the proactive messages to be sent to
all users who have previously messaged the bot.

## Prerequisites

- Java 1.8+
- Install [Maven](https://maven.apache.org/)
- An account on [Azure](https://azure.microsoft.com) if you want to deploy to Azure.

## To try this sample
- From the root of this project folder:
  - Build the sample using `mvn package`
  - Run it by using `java -jar .\target\bot-proactive-sample.jar`

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the latest Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

## Connect to the bot using Bot Framework Emulator
  - Launch Bot Framework Emulator
  - File -> Open Bot
  - Enter a Bot URL of `http://localhost:3978/api/messages`

With the Bot Framework Emulator connected to your running bot, the sample will not respond to an HTTP GET that will trigger a proactive message.  The proactive message can be triggered from the command line using curl or similar tooling, or can be triggered by opening a browser windows and navigating to `http://localhost:3978/api/notify`.

### Using curl

- Send a get request to `http://localhost:3978/api/notify` to proactively message users from the bot.

   ```bash
    curl get http://localhost:3978/api/notify
   ```

- Using the Bot Framework Emulator, notice a message was proactively sent to the user from the bot.

### Using the Browser

- Launch a web browser
- Navigate to `http://localhost:3978/api/notify`
- Using the Bot Framework Emulator, notice a message was proactively sent to the user from the bot.

## Interacting with the bot

In addition to responding to incoming messages, bots are frequently called on to send "proactive" messages based on activity, scheduled tasks, or external events.

In order to send a proactive message using Bot Framework, the bot must first capture a conversation reference from an incoming message using `activity.getConversationReference()`. This reference can be stored for later use.

To send proactive messages, acquire a conversation reference, then use `adapter.continueConversation()` to create a TurnContext object that will allow the bot to deliver the new outgoing message.

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Spring Boot](https://spring.io/projects/spring-boot)
- [Maven Plugin for Azure App Service](https://github.com/microsoft/azure-maven-plugins/tree/develop/azure-webapp-maven-plugin)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Send proactive messages](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-proactive-message?view=azure-bot-service-4.0&tabs=csharp)
- [continueConversation Method](https://docs.microsoft.com/en-us/dotnet/api/microsoft.bot.builder.botadapter.continueconversationasync?view=botbuilder-dotnet-stable#Microsoft_Bot_Builder_BotAdapter_ContinueConversationAsync_System_Security_Claims_ClaimsIdentity_Microsoft_Bot_Schema_Activity_Microsoft_Bot_Builder_BotCallbackHandler_System_Threading_CancellationToken)
- [getConversationReference Method](https://docs.microsoft.com/en-us/dotnet/api/microsoft.bot.schema.activity.getconversationreference?view=botbuilder-dotnet-stable#Microsoft_Bot_Schema_Activity_GetConversationReference)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Azure for Java cloud developers](https://docs.microsoft.com/en-us/azure/java/?view=azure-java-stable)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)


# Prompt users for input

Bot Framework v4 Prompt Users for Input bot sample

This bot has been created using [Bot Framework](https://dev.botframework.com). The bot maintains conversation state to track and direct the conversation and ask the user questions. The bot maintains user state to track the user's answers.

This sample is a Spring Boot app and uses the Azure CLI and azure-webapp Maven plugin to deploy to Azure.

## Prerequisites

- Java 1.8+
- Install [Maven](https://maven.apache.org/)
- An account on [Azure](https://azure.microsoft.com) if you want to deploy to Azure.

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the latest Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## To try this sample
- From the root of this project folder:
  - Build the sample using `mvn package`
  - Run it by using `java -jar .\target\bot-promptusersforinput-sample.jar`

- Test the bot using Bot Framework Emulator

  [Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

  - Install the Bot Framework Emulator version 4.3.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

  - Connect to the bot using Bot Framework Emulator

    - Launch Bot Framework Emulator
    - File -> Open Bot
    - Enter a Bot URL of `http://localhost:3978/api/messages`
    
## Interacting with the bot

A bot is inherently stateless. Once your bot is deployed, it may not run in the same process or on the same machine from one turn to the next.
However, your bot may need to track the context of a conversation, so that it can manage its behavior and remember answers to previous questions.

In this example, the bot's state is used to a track number of messages.

- We use the bot's turn handler and user and conversation state properties to manage the flow of the conversation and the collection of input.
- We ask the user a series of questions; parse, validate, and normalize their answers; and then save their input.

This sample is intended to be run and tested locally and is not designed to be deployed to Azure.

## Further reading

- [Spring Boot](https://spring.io/projects/spring-boot)
- [Maven Plugin for Azure App Service](https://github.com/microsoft/azure-maven-plugins/tree/develop/azure-webapp-maven-plugin)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure for Java cloud developers](https://docs.microsoft.com/en-us/azure/java/?view=azure-java-stable)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)


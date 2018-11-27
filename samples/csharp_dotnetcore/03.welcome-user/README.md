This sample creates an echo bot that welcomes user when they join the conversation. The welcoming pattern shown in this bot is applicable for personal (1:1) conversation with bots.

## To try this sample
- Clone the repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- [Optional] Update the `appsettings.json` file under `botbuilder-samples/samples/csharp_dotnetcore/WelcomeUser.csproj` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.

# Running Locally

## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/03.welcome-user`) and open WelcomeUser.csproj in Visual Studio
- Run the project (press `F5` key).

## .NET Core CLI
- Install the [.NET Core CLI tools](https://docs.microsoft.com/dotnet/core/tools/?tabs=netcore2x).
- Using the command line, navigate to `botbuilder-samples/samples/csharp_dotnetcore/03.welcome-user`
- Type `dotnet run`.

# Deploy this bot to Azure
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. In order to install this and other tools, you can read [Installing CLI Tools](../../../Installing_CLI_tools.md).

To clone this bot, run

```bash
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id> --appId <YOUR APP ID> --appSecret <YOUR APP SECRET PASSWORD>
```

**NOTE**: You can obtain your `appId` and `appSecret` at the Microsoft's [Application Registration Portal](https://apps.dev.microsoft.com/)


# Testing the bot using Bot Framework Emulator V4
Microsoft Bot Framework Emulator is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframework-emulator)

## Connect to bot using Bot Framework Emulator V4
Launch Bot Framework Emulator
File -> Open Bot Configuration and navigate to botbuilder-samples/samples/csharp_dotnetcore/03.welcome-user folder
Select welcome-user.bot file

# ConversationUpdate Activity Type
The [ConversationUpdate](https://docs.microsoft.com/azure/bot-service/bot-service-activity-spec?view=azure-bot-service-4.0#conversation-update-activity) Activity type describes a change in conversation members, for example when a new user (and/or) a bot joins the conversation. The channel sends this activity when a user (and/or) bot joins the conversation. It is recommended that you test your bot behavior on the target channel.

Bots that are added directly by a user, are mostly personal (1:1) conversation bots. It is a best practice to send a welcome message to introduce the bot tell a bit about its functionality. To do this, ensure that your bot responds to the `ConversationUpdate` message. Use the `membersAdded` field to identify the list of channel participants (bots or users) that were added to the conversation.

Your bot may proactively send a welcome message to a personal chat the first time a user initiates a personal chat with your bot. Use `UserState` to persist a flag indicating first user interaction with a bot.

## A note about Bot Framework Emulator and Web Test in Azure Bot Service
The Bot Framework Emulator is following standard [Activity protocol](https://docs.microsoft.com/azure/bot-service/bot-service-activity-spec) for Activity messages sent to your bot. With that said, the emulator has unique behavior that is useful for testing and debugging your bot. For example, pressing the `Start Over` button sends a `ConversationUpdate` Activity with a fresh set of identifiers (conversation, from, recipient) to which your bot may reply.

The Web Test in Azure Bot Service is where you may test your bot using the Web Chat control. When testing your bot in Azure Bot Service Web Test, your bot receives a `ConversationUpdate` Activity only after the first time the user sends a message. Your bot will receive two activities for `ConversationUpdate` (one for the new user and one for the bot) and also a `Message` Activity containing the utterance (text) the user sent.

In other channels such as Teams, Skype, or Slack, you can expect to receive the `ConversationUpdate` just once in the lifetime of the bot for a given user, and it may arrive as soon as the user joins the channel or sent when the user first interacts with the bot.
​
# Further reading
- [Azure Bot Service](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Channels and Bot Connector Service](https://docs.microsoft.com/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Activity Processing](https://docs.microsoft.com/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)

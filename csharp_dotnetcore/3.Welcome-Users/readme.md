
This sample shows how to create a simple echo bot that welcomes user when they join the conversation. The welcoming pattern shown in this bot is applicable for peronsal (1:1) conversation with bots.


## To try this sample
- Clone the repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
 ### Visual studio
- Navigate to the samples folder (`BotBuilder-Samples\csharp_dotnetcore\3.Welcome-Users`) and open 3.Welcome-Users.sln in Visual studio 
- Hit F5
 ### Visual studio code
- Open `BotBuilder-Samples\csharp_dotnetcore\3.Welcome-Users` folder
- Bring up a terminal, navigate to BotBuilder-Samples\csharp_dotnetcore\3.Welcome-Users
- Type 'dotnet run'.

# Testing the bot using Bot Framework Emulator
Microsoft Bot Framework Emulator is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from here

## Connect to bot using Bot Framework Emulator V4
Launch Bot Framework Emulator
File -> Open Bot Configuration and navigate to samples\2.echobot-with-state folder
Select Welcome-users.bot file

# ConversationUpdate Activity Type
 The [ConversationUpdate](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-activity-spec?view=azure-bot-service-3.0#conversation-update-activity) activity type describe a change in a conversation's members, for example when a new user (and/or) a bot joins the conversation. It is up to the channel to send this activity when a user joins the conversation. Therefore, the why this actvity is sent changes from channel to channel. It is recomended that you test your bot behaivor on the target channel. 

 Bots that are added directly by a user, are mostly personal (1:1) conversation bots. It is a best practice to send a welcome message to the General channel to introduce the bot tell a bit about its functionality. To do this, ensure that your bot responds to the `ConversationUpdate` message. Use the `membersAdded` field to identify the list of channel participants (bots or users) that were added to the conversation.

Your bot should also proactively send a welcome message to a personal chat the first time (and only the first time) a user initiates a personal chat with your bot. Use `UserState` to persist a flag indicating first user interaction with a bot. 

## A note about Bot Framework Emulator and Web Test in Azure Bot Service 
The Bot Framework Emulator is following stadnard [Activity protocol](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-activity-spec) for Activity messages sent to your bot. With that said, the emulator has unique behaivor that is useful for testing and debugging your bot. For example, pressing the `Start Over` button reset identifires (conversation, from, recipient) and sends a `ConversationUpdate`) message to which your bot can reply. 

The Web Test in Azure Bot Service is where you may test your bot using the Web Chat control. In Azure Bot Service Web Test, your bot will receive a `ConversationUpdate` message only after the first time the user sends a message. Your bot will receive two messages for `ConversationUpdate` (one for the new user and one for the bot) and also a `Message` message containing the utterance (text) the user sent. 

In other channels such as Teams, Skype, or Slack, you can expect to receive the `ConversationUpdate` just once in the lifetime of the bot for a given user, and it may arrive as soon as the user joins the channel or sent when the user first interacts with the bot. 

	
# Further reading
- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot basics](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Channels and Bot Connector service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)

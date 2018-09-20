This sample demonstrates the use of SendConversationHistoryAsync API to upload conversation history stored in the conversation Transcript.

# To try this sample
- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```

# Prerequisites
## Visual Studio
- Navigate to the samples folder (`BotBuilder-Samples\csharp_dotnetcore\60.ConversationHistory`) and open ConversationHistory.csproj in Visual Studio.
- Set the BLOB store connection-string in appsettings.json
- Hit F5.

## Visual Studio Code
- Open `BotBuilder-Samples\csharp_dotnetcore\60.ConversationHistory` sample folder.
- Set the BLOB store connection-string in appsettings.json
- Bring up a terminal, navigate to `BotBuilder-Samples\csharp_dotnetcore\60.ConversationHistory` folder.
- Type 'dotnet run'.

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot 
developers to test and debug their bots on localhost or running remotely through a tunnel.
- Install the Bot Framework emulator from [here](https://aka.ms/botframeworkemulator).

## Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator.
- File -> Open bot and navigate to `BotBuilder-Samples\csharp_dotnetcore\60.ConversationHistory` folder.
- Select `ConversationHistory.bot` file.

# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot Storage](https://docs.microsoft.com/en-us/azure/bot-service/dotnet/bot-builder-dotnet-state?view=azure-bot-service-3.0&viewFallbackFrom=azure-bot-service-4.0)

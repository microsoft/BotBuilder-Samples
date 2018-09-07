This sample demonstrates how to send proactive messages to users by
capturing a conversation reference, then using it later to initialize
outbound messages using ASP.Net Core 2.

# Concepts introduced in this sample
**Proactive messaging**. Often bots send _reactive messages_, a message based on the user's last input. However, we might need to send a proactive message, one not based on the user's input.

For example, when your bot needs to perform a task that can take an indeterminate amount of time, you can store information about the task, tell the user that the bot will get back to them when the task finishes, and let the conversation proceed. When the task completes, the bot can send the confirmation message proactively on the same conversation.



# To try this sample

- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```


## Visual studio
- Navigate to the samples folder (`BotBuilder-Samples\csharp_dotnetcore\17.Proactive-Messages`) and open `ProactiveBot.csproj` in Visual studio 
- Hit F5

## Visual studio code
- Open `BotBuilder-Samples\csharp_dotnetcore\17.Proactive-Messages` sample folder.
- Bring up a terminal, navigate to `BotBuilder-Samples\csharp_dotnetcore\17.Proactive-Messages` folder
- type `dotnet run`

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://aka.ms/botframeworkemulator).

### Connect to bot using Bot Framework Emulator **V4**
- Launch Bot Framework Emulator
- File -> Open bot and navigate to `BotBuilder-Samples\csharp_dotnetcore\11.QnAMaker` folder
- Select `Proactive.bot` file

# Further reading

- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)

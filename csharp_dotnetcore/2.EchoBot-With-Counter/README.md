This sample shows a simple echo bot using Bot storage with ASP.Net Core 2.

# To try this sample
- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```

# Prerequisites
## Visual studio
- Navigate to the samples folder (`BotBuilder-Samples\csharp_dotnetcore\1.Console-EchoBot`) and open EchoBotWithCounter.csproj in Visual studio 
- Hit F5

## Visual studio code
- Open `BotBuilder-Samples\csharp_dotnetcore\1.Console-EchoBot` sample folder.
- Bring up a terminal, navigate to `BotBuilder-Samples\csharp_dotnetcore\1.Console-EchoBot` folder
- type 'dotnet run'

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://aka.ms/botframeworkemulator).

## Connect to bot using Bot Framework Emulator **V4**
- Launch Bot Framework Emulator
- File -> Open bot and navigate to `BotBuilder-Samples\csharp_dotnetcore\1.Console-EchoBot` folder
- Select EchoBotWithCounter.bot file

# Further reading
- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot Storage](https://docs.microsoft.com/en-us/azure/bot-service/dotnet/bot-builder-dotnet-state?view=azure-bot-service-3.0&viewFallbackFrom=azure-bot-service-4.0)

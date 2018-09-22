This sample demonstrates a the use of prompts with ASP.Net Core 2.

# To try this sample
- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- [Optional] Update the `appsettings.json` file under `botbuilder-samples\samples\csharp_dotnetcore\04.simple-prompt` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.
# Prerequisites
## Visual Studio
- Navigate to the samples folder (`BotBuilder-Samples\samples\csharp_dotnetcore\04.simple-prompt`) and open SimplePromptBot.csproj in Visual Studio.
- Hit F5.

## Visual Studio Code
- Open `BotBuilder-Samples\samples\csharp_dotnetcore\04.simple-prompt` sample folder.
- Bring up a terminal, navigate to `BotBuilder-Samples\samples\csharp_dotnetcore\04.simple-prompt` folder.
- Type 'dotnet run'.

## Update packages
- In Visual Studio right click on the solution and select "Restore NuGet Packages".
  **Note:** this sample requires `Microsoft.Bot.Builder`, `Microsoft.Bot.Builder.Dialogs` and `Microsoft.Bot.Builder.Integration.AspNet.Core`.

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot 
developers to test and debug their bots on localhost or running remotely through a tunnel.
- Install the Bot Framework emulator from [here](https://aka.ms/botframeworkemulator).

## Connect to bot using Bot Framework Emulator V4
- Launch the Bot Framework Emulator.
- File -> Open bot and navigate to `BotBuilder-Samples\samples\csharp_dotnetcore\04.simple-prompt` folder.
- Select `BotConfiguration.bot` file.

# Further reading
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot Storage](https://docs.microsoft.com/en-us/azure/bot-service/dotnet/bot-builder-dotnet-state?view=azure-bot-service-3.0&viewFallbackFrom=azure-bot-service-4.0)

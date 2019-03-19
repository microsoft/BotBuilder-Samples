This sample creates a complex conversation with dialogs and ASP.Net Core 2.

# To try this sample
- Clone the samples repository
```bash
git clone https://github.com/Microsoft/BotFramework-Samples.git
```
- [Optional] Update the `appsettings.json` file under `BotFramework-Samples/SDKV4-Samples/dotnet_core/ComplexDialogBot/` with your botFileSecret.
For Azure Bot Service bots, you can find the botFileSecret under application settings.

# Running Locally
## Visual Studio
- Navigate to the samples folder (`BotFramework-Samples/SDKV4-Samples/dotnet_core/ComplexDialogBot/`) and open ComplexDialogBot.csproj in Visual Studio.
- Run the project (press `F5` key).

## .NET Core CLI
- Install the [.NET Core CLI tools](https://docs.microsoft.com/dotnet/core/tools/?tabs=netcore2x).
- Using the command line, navigate to `BotFramework-Samples/SDKV4-Samples/dotnet_core/ComplexDialogBot/` folder.
- Type `dotnet run`.

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot
developers to test and debug their bots on localhost or running remotely through a tunnel.
- Install the Bot Framework emulator from [here](https://aka.ms/botframeworkemulator).

## Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator.
- File -> Open bot and navigate to `BotFramework-Samples/SDKV4-Samples/dotnet_core/ComplexDialogBot` folder.
- Select `complex-dialog.bot` file.

# Further reading
- [Azure Bot Service](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot Storage](https://docs.microsoft.com/azure/bot-service/dotnet/bot-builder-dotnet-state?view=azure-bot-service-3.0&viewFallbackFrom=azure-bot-service-4.0)

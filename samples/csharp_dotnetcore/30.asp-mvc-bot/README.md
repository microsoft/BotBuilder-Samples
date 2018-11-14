This sample demonstrates a how to build a bot with ASP.Net Core 2 MVC. Each bot is implemented as an MVC Controller.

Other than the obvious advantages of simplicity and familiarity, this arrangement allows the application to leverage more of ASP framework including such things as [routing](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-2.1).
It is also easy to freely mix and match bot implementation with more typical web development, and significantly, it is easy to host multiple bots running at different endpoints in the same project.

This approach aligns with the regular ASP development methodology. For example, [dependency injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1) is something the application developer can opt into over time rather than being forced into on day one.

This sample also demonstrates how the inheritance mechanism of object-oriented programming can be used to model the Bot Framework Protocol itself and cleanly separate out the infrastructural aspects of the application from the essential business logic.

# To try this sample
- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- [Optional] Update the `appsettings.json` file under `botbuilder-samples/samples/csharp_dotnetcore/30.asp-mvc-bot` with your botFileSecret.  For Azure Bot Service bots, you can find the botFileSecret under application settings.
# Running Locally
## Visual Studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/30.asp-mvc-bot`) and open EchoBotWithCounter.csproj in Visual Studio.
- Run the project (press `F5` key).

## .NET Core CLI
- Install the [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x). 
- Using the command line, navigate to `botbuilder-samples/samples/csharp_dotnetcore/30.asp-mvc-bot` folder.
- Type `dotnet run`.

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot 
developers to test and debug their bots on localhost or running remotely through a tunnel.
- Install the Bot Framework emulator from [here](https://aka.ms/botframeworkemulator).

## Connect to bot using Bot Framework Emulator **V4**
- Launch the Bot Framework Emulator.
- File -> Open bot and navigate to `botbuilder-samples/samples/csharp_dotnetcore/30.asp-mvc-bot` folder.
- Select `asp-mvc-bot.bot` file.

# Deploy this bot to Azure
You can use the [MSBot](https://github.com/microsoft/botbuilder-tools) Bot Builder CLI tool to clone and configure any services this sample depends on. In order to install this and other tools, you can read [Installing CLI Tools](../../../Installing_CLI_tools.md).

To clone this bot, run
```bash
msbot clone services -f deploymentScripts/msbotClone -n <BOT-NAME> -l <Azure-location> --subscriptionId <Azure-subscription-id>
```
# Further reading
- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)

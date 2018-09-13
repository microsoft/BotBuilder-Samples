

# Concepts introduced in this sample

This sample shows how to integrate and consume Facebook specific payloads, such as postbacks, quick replies and optin events. 
Since Bot Framework supports multiple Facebook pages for a single bot, we also show how to know the page to which the message was sent, so developers can have custom behavior per page.

# To try this sample

- Clone the samples repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```

## Visual Studio
- Navigate to the samples folder (`BotBuilder-Samples\csharp_dotnetcore\81.Facebook-Events`) and open Facebook-Events-Bot.csproj in Visual Studio 
- Hit F5

## Visual Studio Code
- Open `BotBuilder-Samples\csharp_dotnetcore\81.Facebook-Events` sample folder.
- Bring up a terminal, navigate to BotBuilder-Samples\81.Facebook-Events folder
- type 'dotnet run'

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator from [here](https://aka.ms/botframeworkemulator).

### Connect to bot using Bot Framework Emulator V4
- Launch Bot Framework Emulator
- File -> Open bot and navigate to `BotBuilder-Samples\csharp_dotnetcore\81.Facebook-Events` folder
- Select BotConfiguration.bot file

### Publish to Azure 

To publish your bot to Azure, follow the [Azure Bot Service documentation](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-deploy-azure?view=azure-bot-service-3.0) which provides simple steps to get your bot in the cloud in a matter of minutes.

### Enable Facebook Channel

The final step to test Facebook-specific features is to publish your bot for the Facebook channel. The Bot Framework makes this very easy, and the detailed steps are explained in the [Bot Framework Channel Documentation](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-facebook?view=azure-bot-service-3.0).

# Further reading

- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
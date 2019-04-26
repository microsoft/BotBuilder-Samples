# Scale Out

Bot Framework v4 bot Scale Out sample

This sample demonstrates a custom storage solution that supports a deployment scaled out across multiple machines.

The custom storage solution is implemented against memory for testing purposes and against Azure Blob Storage.

The sample shows how storage solutions with different policies can be implemented and integrated with the framework.

The solution makes use of the standard HTTP ETag/If-Match mechanisms commonly found on cloud storage technologies.

Refer to the Bot Builder V4 documentation for a design walk-through.

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

- Update `appsettings.json` with required configuration settings
  - MicrosoftAppId
  - MicrosoftAppPassword
  - ConnectionName


## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `samples/csharp_dotnetcore/42.scaleout`
- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/42.scaleout` folder
  - Select `ScaleoutBot.csproj` file
  - Press `F5` to run the project

## Testing the bot using Bot Framework Emulator

[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/microsoft/botframework-emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Further reading

- [Implementing custom storage for you bot](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-custom-storage?view=azure-bot-service-4.0)
- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot Storage](https://docs.microsoft.com/en-us/azure/bot-service/dotnet/bot-builder-dotnet-state?view=azure-bot-service-3.0&viewFallbackFrom=azure-bot-service-4.0)
- [HTTP ETag](https://en.wikipedia.org/wiki/HTTP_ETag)

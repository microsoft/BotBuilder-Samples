## Overview

Example demonstrating saving user scoped information (User State) into Azure Table, CosmosDb or Azure Sql using [Microsoft.Bot.Builder.Azure](https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/3.16.3.40383)

## Purpose

This bot will ask the user their name and city of residence.  This information is then stored in the ```IBotDataStore<BotData>``` configured by the developer in Global.asax.cs.  Two classes are stored within the UserState:

```cs
    public class GreetingState
    {
        public string Name { get; set; }
        public string City { get; set; }
    }
```
```cs
    public class TestDataClass
    {
        public string TestStringField { get; set; }
        public int TestIntField { get; set; }
        public Tuple<string, string> TestTuple { get; set; }
    }
```

If [V4StateBot](../V4StateBotFromV3Providers/V4StateBot.sln) is then connected to the same storage provider, this User State information can be automatically read using the provided [Bot.Builder.Azure.V3V4](../V4StateBotFromV3Providers/Bot.Builder.Azure.V3ToV4/Bot.Builder.Azure.V3V4.csproj) library.

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `MigrationV3V4/CSharp/V3StateBot` folder
  - Select `V3StateBot.sln` file
  - Edit web.config TableStorageConnectionString, BotDataContextConnectionString or CosmosDb app settings.
  - Modify Global.asax.cs so the storage provider you are using is properly registered.
  - Press `F5` to run the project
  
## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
## Overview

The V4StateBot solution is a demonstration of how to use a Bot Builder V3 ```IBotDataStore<BotData>``` copied from [Microsoft.Bot.Builder.Azure](https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/3.16.3.40383) and shimmed into a Bot Builder V4 solution for User State.  This example demonstrates retrieving previoulsy stored User State, as well as saving new User State into Azure Table, CosmosDb or Azure Sql using copied code from [Microsoft.Bot.Builder.Azure](https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/3.16.3.40383)

## Purpose

If the corresponding [V3StateBot](..\V3StateBot\V3StateBot.sln) was previously run against the same data store, User State should be automatically loaded into the V4StateBot.  If this bot is run against a clean store, then it will ask the user their name and city of residence.  This information is then stored in the ```IBotDataStore<BotData>``` configured by the developer in appsettings.json.  Two classes are stored within the UserState:

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


## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `MigrationV3V4/CSharp/V4StateBot` folder
  - Select `V4StateBot.sln` file
  - Edit appsettings.json ConnectionStrings.SqlBotData, ConnectionStrings.AzureTable or CosmosDb app settings.
  - Modify Startup.cs so the storage provider you are using is properly registered.
  - Press `F5` to run the project
  
## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Further reading

- [Differences between the v3 and v4 .NET SDK](https://docs.microsoft.com/en-us/azure/bot-service/migration/migration-about)
- [.NET migration quick reference](https://docs.microsoft.com/en-us/azure/bot-service/migration/net-migration-quickreference)
- [Migrate a .NET v3 bot to a Framework v4 bot](https://docs.microsoft.com/en-us/azure/bot-service/migration/conversion-framework)
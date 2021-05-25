# Bot Framework Migration from V3 to V4


## Overview

This area contains samples for migration workflows from **Bot Framework V3 SDK** to  **Bot Framework V4 SDK** for [.NET](https://github.com/Microsoft/botbuilder-dotnet) and [JS](https://github.com//microsoft/botbuilder-js). 

## Goals of this work

In some cases, since the V4 SDK contains substantial improvements that are not compatible with V3, some migration may require potential re-write or substantial effort. This section contains examples and best practices to support [Migration][1] documentation.

## Samples

### CSharp

- [ContosoHelpdeskChatBot-V3](cs#1)
    - This V3 bot was taken from the [Azure with Intelligent Apps](https://github.com/Microsoft/intelligent-apps/tree/master/ContosoHelpdeskChatBot/ContosoHelpdeskChatBot) example.  This is the bot we've used to demonstrate migration concepts.  The V3 source code is here for reference.

- [ContosoHelpdeskChatBot-V4NetFramework](cs#3)
    - Example of ContosoHelpdeskChatBot migrated to use Bot Builder V4 targeting netframework.

- [ContosoHelpdeskChatBot-V4NetCore](cs#2)
    - Example of ContosoHelpdeskChatBot migrated to use Bot Builder V4 targeting netcoreapp2.1.

- [V3StateBot](cs#4)
    - Example demonstrating saving user scoped information (User State) into Azure Table, CosmosDb and Azure Sql using [Microsoft.Bot.Builder.Azure](https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/3.16.3.40383)

- [V4StateBotFromV3Providers](cs#5)
    - The V4StateBot solution contains to projects:
        - Bot.Builder.Azure.V3V4 library contains code ported from [Microsoft.Bot.Builder.Azure](https://github.com/microsoft/BotBuilder-Azure/tree/master/CSharp/Library/Microsoft.Bot.Builder.Azure) but targeting netcoreapp2.1 This library can be used to create a v4 storage provider that wraps a v3 store.
        - V4StateBot is an example demonstrating using the Bot.Builder.Azure.V3V4 library to connect to v3 storage providers.

### Node

- [core-MultiDialogs-v3](js#1)
    - This V3 bot was taken from the [v3-sdk-samples](https://github.com/microsoft/BotBuilder-Samples/tree/v3-sdk-samples/Node/core-MultiDialogs) branch.  This is the bot we've used to demonstrate migration concepts.  The V3 source code is here for reference.

- [core-MultiDialogs-v3](js#2)
    - Example of core-MultiDialogs migrated to use Bot Builder V4.

[cs#1]:csharp/ContosoHelpdeskChatBot-V3
[cs#2]:csharp/ContosoHelpdeskChatBot-V4NetCore
[cs#3]:csharp/ContosoHelpdeskChatBot-V4NetFramework
[cs#4]:csharp/V3StateBot
[cs#5]:csharp/V4StateBotFromV3Providers

[js#1]:node/core-MultiDialogs-v3
[js#2]:node/core-MultiDialogs-v4

[1]: https://docs.microsoft.com/en-us/azure/bot-service/migration/conversion-framework?view=azure-bot-service-4.0

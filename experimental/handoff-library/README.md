# Handoff Library

Bot Framework v4 Handoff Library

The library implements the additions to the Bot Framework SDK to support handoff to an agent (a.k.a. 'escalation').

The library is intended to be merged in a future release of the Bot Framework SDK.

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## To try the library

- Clone the repository

    ```bash
    https://github.com/microsoft/BotBuilder-Samples.git
    ```
- Build the solution

- Deploy and run the Handoff bot

- Using the DirectLine API, [start a conversation](https://docs.microsoft.com/en-us/azure/bot-service/rest-api/bot-framework-rest-direct-line-3-0-start-conversation?view=azure-bot-service-4.0).

- [Send a message](https://docs.microsoft.com/en-us/azure/bot-service/rest-api/bot-framework-rest-direct-line-3-0-send-activity?view=azure-bot-service-4.0) to the bot containing the word 'human' ()

- The bot should receive the handoff activity when the handoff has taken place

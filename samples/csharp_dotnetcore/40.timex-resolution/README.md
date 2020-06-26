# Timex Resolution

This bot has been created using [Bot Framework](https://dev.botframework.com), is shows how to use TIMEX expressions.

## Concepts introduced in this sample

### What is a TIMEX expression?

A TIMEX expression is an alpha-numeric expression derived in outline from the standard date-time representation ISO 8601.
The interesting thing about TIMEX expressions is that they can represent various degrees of ambiguity in the date parts. For example, May 29th, is not a
full calendar date because we haven't said which May 29th - it could be this year, last year, any year in fact.
TIMEX has other features such as the ability to represent ranges, date ranges, time ranges and even date-time ranges.

### Where do TIMEX expressions come from?

TIMEX expressions are produced as part of the output of running a DateTimeRecognizer against some natural language input. As the same
Recognizers are run in LUIS the result returned in the JSON from a call to LUIS also contains the TIMEX expressions.

### What can the library do?

It turns out that TIMEX expressions are not that simple to work with in code. This library attempts to address that. One helpful way to
think about a TIMEX expression is as a partially filled property bag. The properties might be such things as "day of week" or "year."
Basically the more properties we have captured in the expression the less ambiguity we have.

The library can do various things:

- Parse TIMEX expressions to give you the properties contained there in.
- Generate TIMEX expressions based on setting raw properties.
- Generate natural language from the TIMEX expression. (This is logically the reverse of the Recognizer.)
- Resolve TIMEX expressions to produce example date-times. (This produces the same result as the Recognizer (and therefore LUIS)).
- Evaluate TIMEX expressions against constraints such that new more precise TIMEX expressions are produced.

### Where is the source code?

The TIMEX expression library is contained in the same GitHub repo as the recognizers. Refer to the further reading section below.

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- Run the bot from a terminal or from Visual Studio:

  A) From a terminal, navigate to `samples/csharp_dotnetcore/40.timex-resolution`

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/40.timex-resolution` folder
  - Select `Timex-Resolution.csproj` file
  - Press `F5` to run the project

## Testing the bot using Bot Framework Emulator

[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the latest Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [TIMEX](https://en.wikipedia.org/wiki/TimeML#TIMEX3)
- [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601)
- [Recognizers Text](https://github.com/Microsoft/recognizers-text)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)

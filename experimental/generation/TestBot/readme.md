# TestBot

This bot allows you to easily run Bot Framework Adaptive Dialog declarative tests that use only standard SDK components.
It starts a web server you can connect the Bot Framework Emulator to at `http://localhost:5000/api/messages` and interact with your bot.

## Setup

In order to setup testbot:

1. Ensure you have the [dotnet] runtime installed on your machine.
2. Ensure you have [git] installed on your machine.
3. Open a shell window:
   1. Change to the directory where you want the repo located.
   2. Run `git clone https://github.com/microsoft/BotBuilder-Samples.git`.
4.Add the MyGet feed to NuGet: `nuget sources add -name "MyGet" -source "https://botbuilder.myget.org/F/botbuilder-v4-dotnet-daily/api/v3/index.json"`.
5. To use LUIS you need to register your LUIS endpoint key by running `dotnet user-secrets --id RunBot set luis:endpointKey <yourKey>` once.

## Usage

Execute the command `dotnet run --project <pathToRepo>/experimental/generation/TestBot/TestBot.csproj --root <directoryWithTests>` which will run each `*.test.dialog` script and report any errors. 


Command line args:

* **--root <PATH>**: Absolute path to the root directory for declarative resources all *.test.dialog will be run.  Defaults to the current directory.
* **--region <REGION>**: LUIS endpoint region.  Default is `westus`.
* **--environment <ENVIRONMENT>**: LUIS environment settings to use.
  Default is the user alias.
These parameters are used to find your `luis.settings.<environment>.<region>.json` settings file for LUIS.

## Generating Tests from Transcripts

A simple way to test your bot is to generate a test file from a transcript using [`bf
dialog:generate:test`](../generator/packages/cli/readme.md#bf-dialoggeneratetest-transcript-dialog)
To do this:
1. Test your `bot.main.dialog` in the [Bot Framework Emulator][emulator].
2. Save the transcript as something like `bot.transcript`.
3. Generate a test script using `bf dialog:generate:test bot.transcript bot.main.dialog`.
4. Run the test script using TestBot.

[dotnet]:https://dotnet.microsoft.com/download
[git]:https://git-scm.com/downloads
[samples]:https://github.com/microsoft/BotBuilder-Samples.git
[emulator]:https://github.com/Microsoft/BotFramework-Emulator
[generation]:https://github.com/microsoft/BotBuilder-Samples/tree/master/experimental/generation/generator

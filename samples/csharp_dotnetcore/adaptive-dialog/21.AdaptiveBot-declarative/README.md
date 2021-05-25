# AdaptiveBot

A sample bot that demonstrates various declaratively defined [adaptive dialogs][1].

## Prerequisites
- [.NET Core SDK][4] version 3.1
	```bash
	# determine dotnet version
	dotnet --version
	```

## To try this sample

- Run the bot from a terminal or from Visual Studio:

	A) From a terminal, navigate to `samples/csharp_dotnetcore/adaptive-dialog/21.AdaptiveBot-declarative`
	```bash
	# run the bot
	dotnet run
	```

	B) Or from Visual Studio
	- Launch Visual Studio
	- File -> Open -> Project/Solution
	- Navigate to `samples/csharp_dotnetcore/adaptive-dialog/21.AdaptiveBot-declarative` folder
	- Select `AdaptiveBot.csproj` file
	- Press `F5` to run the project

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator][5] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the latest Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Further reading

- [Adaptive dialogs](https://aka.ms/adaptive-dialogs)
- [Language generation](https://aka.ms/language-generation)
- [Adaptive Expressions](https://aka.ms/adaptive-expressions)
- [.lu file format](https://aka.ms/lu-file-format)
- [.lg file format](https://aka.ms/lg-file-format)
- [.qna file format](https://aka.ms/qna-file-format)

[1]:https://aka.ms/adaptive-dialogs


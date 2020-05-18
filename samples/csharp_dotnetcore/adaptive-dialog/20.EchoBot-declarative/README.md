# EchoBot Declarative sample

Bot Framework Adaptive Dialog declarative Echo bot. This bot is built using declaratively defined [adaptive dialog][1].

## Prerequisites
- [.NET Core SDK][4] version 3.1
	```bash
	# determine dotnet version
	dotnet --version
	```

# To try this sample
- In a terminal, navigate to `samples/csharp_dotnetcore/adaptive-dialog/20.EchoBot-declarative`
    ```bash
    # change into project folder
	cd # EchoBot
    ```
- Run the bot from a terminal or from Visual Studio, choose option A or B.

	A) From a terminal
	```bash
	# run the bot
	dotnet run
	```

	B) Or from Visual Studio
	- Launch Visual Studio
	- File -> Open -> Project/Solution
	- Navigate to `samples/csharp_dotnetcore/adaptive-dialog/20.EchoBot-declarative` folder
	- Select `EchoBot.csproj` file
	- Press `F5` to run the project

# Testing the bot using Bot Framework Emulator
[Bot Framework Emulator][5] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here][6]

## Connect to the bot using Bot Framework Emulator
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
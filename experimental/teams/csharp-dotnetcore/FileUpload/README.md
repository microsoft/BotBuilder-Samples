# EchoBot

Bot Framework v4 echo bot sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple bot that accepts input from the user and echoes it back.

## Prerequisites
- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

- Open Notepad (or another text editor) to save some values as you complete the setup.

- Ngrok setup
1. Download and install [Ngrok](https://ngrok.com/download)
2. In terminal navigate to the directory where Ngrok is installed
3. Run this command: ```ngrok http -host-header=rewrite 3978 ```
4. Copy the https://xxxxxxxx.ngrok.io address and put it into notepad. **NOTE** You want the https address.

- Azure setup
1. Login to the [Azure Portal]((https://portal.azure.com) 
2. (optional) create a new resource group if you don't currently have one
3. Go to your resource group 
4. Click "Create a new resource" 
5. Search for "Bot Channel Registration" 
6. Click Create 
7. Enter bot name, subscription
8. In the "Messaging endpoint url" enter the ngrok address from earlier. 
8a. Finish the url with "/api/messages. It should look like ```https://xxxxxxxxx.ngrok.io/api/messages```
9. Click the "Microsoft App Id and password" box 
10. Click on "Create New" 
11. Click on "Create App ID in the App Registration Portal" 
12. Click "New registration" 
13. Enter a name 
14. Under "Supported account types" select "Accounts in any organizational directory and personal Microsoft accounts" 
15. Click register 
16. Copy the application (client) ID and put it in Notepad. Label it "Microsoft App ID" 
17. Go to "Certificates & Secrets" 
18. Click "+ New client secret" 
19. Enter a description 
20. Click "Add" 
21. Copy the value and put it into Notepad. Label it "Password"
22. (back in the channel registration view) Copy/Paste the Microsoft App ID and Password into their respective fields 
23. Click Create 
24. Go to "Resource groups" on the left 
25. Select the resource group that the bot channel reg was created in 
26. Select the bot channel registration 
27. Go to Settings  
28. Select the "Teams" icon under "Add a featured channel 
29. Click Save 

- Updating Sample Project Settings
1. Open the project 
2. Open appsettings.json 
3. Enter the app id under the MicrosoftAppId and the password under the MicrosoftAppPassword 
4. Save the close the file 
5. Build the project
6. Under the TeamsAppManifest open the manifest.json file 
7. Update the ```botId``` with the Microsoft App ID from before 
8. Update the ```id``` with the Microsoft App ID from before 

- Uploading the bot to Teams
1. In file explorer navigate to the TeamsAppManifest folder in the project 
2. Select the 3 files and zip them 
3. Open Teams 
4. Click on "Apps" 
5. Select "Upload a custom app" on the left at the bottom 
6. Select the zip  
7. Select for you  
8. (optionally) click install if prompted 
9. Click open 

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `samples/csharp_dotnetcore/02.echo-bot`
- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/48.teams-file-bot` folder
  - Select `EchoBot.csproj` file
  - Press `F5` to run the project
  
- Interacting with the bot
1. Send a message to your bot in Teams
2. Confirm you are getting a 200 back in Ngrok
3. Click Accept on the card that is shown
4. Confirm you see a 2nd 200 in Ngrok
5. In Teams go to Files -> OneDrive -> Applications

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Restify](https://www.npmjs.com/package/restify)
- [dotenv](https://www.npmjs.com/package/dotenv)

# TeamsFileUploadBot

Bot Framework v4 file upload bot sample for Teams.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple bot that accepts input from the user and echoes it back.

## Prerequisites


- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```
  
- Microsoft Teams is installed and you have an account

## To try this sample

### Clone the repo
- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

### Ngrok
- Download and install [ngrok](https://ngrok.com/download)
- In terminal navigate to where ngrok is installed and run: 

```bash
ngrok http -host-header=rewrite 3978
```
- Copy/paste the ```https``` **NOT** the ```http``` web address into notepad as you will need it later

### Creating the bot registration
- Create a new bot [here](https://dev.botframework.com/bots/new)
- Enter a```Display name``` and ```Bot handle```
- In the ```Messaging endpoint``` enter the https address from Ngrok and add ```/api/messages``` to the end
  - EX: ```https://7d899fbb.ngrok.io/api/messages``` 
- Open the ```Create Microsoft App ID and password``` link in a new tab
- Click on the ```New registration``` button 
- Enter a name, and select the ```Accounts in any organizational directory (Any Azure AD directory - Multitenant) and personal Microsoft accounts (e.g. Skype, Xbox)```
- Click ```Register```
- Copy & paste the ```Application (client) ID``` field into notepad. This is your botID.
- Click on ```Certificates & secrets``` tab on the left
- Click ```New client secret```
- Enter a name, select `Never`, and click ```Add```
- Copy & paste the password into notepad. This is your app password.
- Go back to the bot registration tab and enter the ```botID``` into the app ID field
- Scroll down, agree to the Terms, and click ```Register```
- Click the ```Microsoft Teams``` icon on the next screen
- Click ```Save```

### Visual Studio
- Launch Visual Studio
- Navigate to and open the `samples/csharp_dotnet/57.teams-conversation-bot` directory
- Open the ```appsettings.json``` file
- Paste your botID value into the ```MicrosoftAppId``` field 
- Put the password into the ```MicrosoftAppPassword``` field
- Save the file
- Open the ```manifest.json```
- Replace your botID everywhere you see the place holder string ```<<YOUR-MICROSOFT-BOT-ID>>```


- Run the bot:

 A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/57.teams-conversation-bot` folder
  - Select `TeamsConversationBot.csproj` file
  - Press `F5` to run the project

### Teams - App Studio
- Launch Microsoft Teams
- In the bar at the top of Teams search for and select ```App Studio``` 
- Click the ```Manifest editor``` tab
- Click ```Import an existing app```
- Navigate to and select the `manifest.json` file from the previous step
- Click on the `TeamsConversationBot` card
- Click ```Test and distribute``` on the left hand side
- Click the ```Install``` button

| To install bot in a personal chat... | To install in a group chat... | To install in team chat... |
|:-------------------- | :------------------------- | :-----------------------|
| 1. Click ```Add``` button| 1. Click the down arrow to the right of the ```Add``` button <br> 2. Click ```Add to Chat``` <br> 3. Search for and select your group chat <br> 4. Click the ```Set up bot``` button <br> **Note:** There must be at least 1 message in a group chat for it to be searchable |  1. Click the down arrow to the right of the ```Add``` button <br> 2. Click ```Add to Team``` <br> 3. Search for and select your team <br> 4. Click the ```Set up a bot``` button  |

### Interacting with the bot

You can interact with this bot by sending it a message, or selecting a command from the command list. The bot will respond to the following strings. 

1. **Show Welcome**
  - **Result:** The bot will send the welcome card for you to interact with
  - **Valid Scopes:** personal, group chat, team chat
2. **MentionMe**
  - **Result:** The bot will respond to the message and mention the user
  - **Valid Scopes:** personal, group chat, team chat
3. **MessageAllMembers**
  - **Result:** The bot will send a 1-on-1 message to each memeber in the current conversation (aka on the converstation's roster).
  - **Valid Scopes:** personal, group chat, team chat

You can select an option from the coammn list by typing ```@TeamsConversationBot``` into the compose message area and ```What can I do?``` text above the compose area.


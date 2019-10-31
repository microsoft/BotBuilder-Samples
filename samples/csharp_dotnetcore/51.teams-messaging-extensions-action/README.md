# TeamsMessagingExtensionsActionBot

Bot Framework v4 Teams Messaging Extension Action sample.

This Messaging Extension has been created using [Bot Framework](https://dev.botframework.com). It shows how to create a simple Survey that accepts input from users and shows responses in a Task Module.

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
- Navigate to and open the `samples/csharp_dotnet/51.teams-messaging-extensions-action` directory
- Open the ```appsettings.json``` file
- Paste your botID value into the ```MicrosoftAppId``` field 
- Put the password into the ```MicrosoftAppPassword``` field
- Save the file
- Open the ```manifest.json```
- Replace your botID everywhere you see the place holder string ```<YOUR-BOT-ID>```


- Run the bot:

 A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/51.teams-messaging-extensions-action` folder
  - Select `TeamsMessagingExtensionsAction.csproj` file
  - Press `F5` to run the project

### Teams - App Studio
- Launch Microsoft Teams
- In the bar at the top of Teams search for and select ```App Studio``` 
- Click the ```Manifest editor``` tab
- Click ```Import an existing app```
- Navigate to and select the `manifest.json` file from the previous step
- Click on the `Action Messaging Extension` card
- Click ```Test and distribute``` on the left hand side
- Click the ```Install``` button
- Click ```Add``` button

#### * Note: Submitting survey results from within a Group Chat context will not function as expected.  Only users who have the Messaging Extension installed within that group will be allowed to submit responses.  Microsoft is aware of this bug, and a fix will be released soon.

### Interacting with the Messaging Extension

1. Selecting the **Create Survey** command from the Compose Box command list. The survey parameters dialog will be displayed and can be submitted. 

or

2. Selecting the **Share Message** command from the Message command list.  


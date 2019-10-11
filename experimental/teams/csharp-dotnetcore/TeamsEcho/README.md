# EchoBot

Bot Framework v4 echo bot sample for Teams.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple bot that accepts input from the user and echoes it back.

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```
- Microsoft Teams is installed and you have an account

## To try this sample

### Ngrok
- Download and install [ngrok](https://ngrok.com/download)
- In terminal navigate to where ngrok is installed and run: 

```bash
ngrok http -host-header=rewrite 3978
```
- Copy/paste the ```https``` web address into notepad as you will need it later

### Microsoft Teams Setup
- Launch Microsoft Teams. In the search bar at the top of Teams search for and select ```App Studio```.
- Click the ```Manifest editor``` tab near the top of the screen.
- Click the ```Create a new app``` button on the left hand side.
- Under the ```Details``` section fill in the following fields 
  - In the Short name field enter ```EchoBot```
  - Click the ```Generate``` button under App ID 
  - Package Name
  - Version 
  - Short description
  - Long description
  - Developer name
  - Website 
  - Privacy statement web address
  - Terms of use web address
- Under the ```Capabilities``` tab on the left hand side click the ```Bots``` tab
- Click the ```Set up``` button
- Under the ```New bot``` tab Fill in the following fields
  - Put ```EchoBot``` into the Name field
  - Under ```Scope``` check all 3 boxes ```Personal```, ```Team```, ```Group Chat```
  - Click the ```Create bot``` button
- Copy the Bot ID (string under ```EchoBot```) and paste it into notepad as you will need it later
- Click the ```Generate new password``` button (copy/paste) the password into notepad as you will need it later)
- Under Messaging endpoint paste the https ngrok url and add ```/api/messages``` to the end
  - EX: ```https://ca7f8a7e.ngrok.io/api/messages```
- Press Enter to save the address

### Bot Setup
- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

- In Visual Studio navigate to the ```52.teams-echo-bot``` folder and open the ```appsettings.json``` file
- Put the  you saved earlier from Teams in the ```MicrosoftAppId``` field
- Put the password into the ```MicrosoftAppPassword``` field
- Save

- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `experimental/teams/csharp-dotnetcore/Teams/TeamsEchoBot` folder
  - Select `TeamsEchoBot.csproj` file
  - Press `F5` to run the project

### Finishing Teams Setup
- Back in Teams click ```Test and distribute``` on the left hand side under ```Finish``` section
- Click the ```Install``` button

| To install bot in a personal chat... | To install in a group chat... | To install in team chat... |
|:-------------------- | :------------------------- | :-----------------------|
| 1. Click ```Add``` button| 1. Click the down arrow to the right of the ```Add``` button <br> 2. Click ```Add to Chat``` <br> 3. Search for and select your group chat <br> 4. Click the ```Set up bot``` button <br> **Note:** There must be at least 1 message in a group chat for it to be searchable |  1. Click the down arrow to the right of the ```Add``` button <br> 2. Click ```Add to Team``` <br> 3. Search for and select your team <br> 4. Click the ```Set up a bot``` button  |

**Note:** If you send an unsupported string in a group chat or personal chat the bot will respond with an error message. This is because it's missing data that comes with messages that orignates from a team or group chat.

|Supported strings in personal chat | Supported strings in group chat | supported strings in team chat|
|:----------------------------- | :-------------------------------|:----------------------------------|
| N/A | ```show members``` |  ```show members``` <br> ```show channels``` <br> ```show details``` |

### Place holder for potential errors
- If your tenant admin has things disabled
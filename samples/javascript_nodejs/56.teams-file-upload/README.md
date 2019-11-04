## Teams File Upload Bot

Bot Framework v4 file upload bot sample for Teams.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to
upload files to Teams from a bot and how to receive a file send to a bot as an attachment.

## Prerequisites

- Node
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
  - Put ```TeamsConversationBot``` into the Name field
  - Under ```Scope``` check all 3 boxes ```Personal```, ```Team```, ```Group Chat```
  - Click the ```Create bot``` button
- Copy the Bot ID (string under ```TeamsConversationBot```) and paste it into notepad as you will need it later
- Click the ```Generate new password``` button (copy/paste) the password into notepad as you will need it later)
- Under Messaging endpoint paste the https ngrok url and add ```/api/messages``` to the end
  - EX: ```https://ca7f8a7e.ngrok.io/api/messages```
- Press Enter to save the address

### Bot Setup
- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```

- In Visual Studio navigate to the ```5NNNNNNNNN.teams-conversation-bot``` folder and open the ```appsettings.json``` file
- Put the  you saved earlier from Teams in the ```MicrosoftAppId``` field
- Put the password into the ```MicrosoftAppPassword``` field
- Save

- Run the bot from a terminal.

  ```bash
  # run the bot
  node index.js
  ```

### Finishing Teams Setup
- Back in Teams click ```Test and distribute``` on the left hand side under ```Finish``` section
- Click the ```Install``` button

| To install bot in a personal chat... | To install in a group chat... | To install in team chat... |
|:-------------------- | :------------------------- | :-----------------------|
| 1. Click ```Add``` button| 1. Click the down arrow to the right of the ```Add``` button <br> 2. Click ```Add to Chat``` <br> 3. Search for and select your group chat <br> 4. Click the ```Set up bot``` button <br> **Note:** There must be at least 1 message in a group chat for it to be searchable |  1. Click the down arrow to the right of the ```Add``` button <br> 2. Click ```Add to Team``` <br> 3. Search for and select your team <br> 4. Click the ```Set up a bot``` button  |

### Interacting with the bot

If you send a message to the bot it will respond with a card that will prompt you to upload a file. The file that's being uploaded is the `teams-logo.png` in the `Files` directory in this sample. You can message the bot again to receive another prompt. 

You can also send a file to the bot as an attachment in the message compose section in Teams.

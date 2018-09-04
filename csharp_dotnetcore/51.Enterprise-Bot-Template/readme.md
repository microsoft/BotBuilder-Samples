# Enterprise Bot Template


## Concepts introduced in this sample
- Dialogs
- Template Manager
- Dispatch
- Middleware


## To try this sample
1. Install VSIX template
2. Install the Bot CLI Tools (run this script to install the correct versions)

        npm i -g ludown@1.0.30 luis-apis@2.0.3 qnamaker@1.0.32 botdispatch@1.0.7 msbot@1.0.51

3. Register a new Microsoft App
    - https://apps.dev.microsoft.com/#/appList
    - Select "Add an app"
    - Enter an Application Name
    - Click Create
    - Copy the Application Id for later use
  
5. Setup Azure Powershell (Only required once per machine)
    - To login, run:
            
            Connect-AzureRmAccount to login
    - To select your Azure subscription, run:

            Select-AzureRmSubscription -Subscription "subscription-name"
 
4. Update deployment parameters in azuredeploy.parameters.json
    - **MSA App Id** - See step 3.
    - **AppInsights Location** - This should be updated depending on your region and availability.
    - **Cognitive Services Location** - This should be updated depending on your region and availability.

5. Run Deploy.ps1 from any command line tool.
    - During deployment, you will be asked to enter your LUIS Authoring Key. This can be found in the user settings page in the luis.ai portal

6. Run your bot project and type "hi" to verify your services are correctly configured

## Prerequisites
- NodeJS & Node Package Manager

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

## Connect to bot using Bot Framework Emulator V4
- Launch Bot Framework Emulator
- File -> Open bot and navigate to your .bot file
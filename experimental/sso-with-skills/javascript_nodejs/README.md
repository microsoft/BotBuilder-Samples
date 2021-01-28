# SSO with Simple Skill Consumer and Skill

Bot Framework v4 Skills SSO Skills sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple RootBot that sends message activities to a SkillBot that echoes it back.

## Prerequisites

- [Node.js](https://nodejs.org) version 10.14 or higher

    ```bash
    # determine node version
    node --version

## Key concepts in this sample

The solution includes a parent bot (`rootBot`) and a skill bot (`echo-skillBot`) and shows how the parent bot can post activities to the skill bot.

- [RootBot](rootBot/rootBot.js): this project is a simple skill consumer bot, and supports:
  - `login` command that gets the user to sign into the skill consumer bot's aad application.
  - `token` command that displays the user's token.
  - `logout` command that logs the user out of the skill consumer.
- [SkillBot](skillBot/skillBot.js) this project shows a simple skill that supports OAuth for AADV2 and can respond to the following commands:
  - `skill login` command that gets the skill consumer bot to sign into the skill's aadV2 app, on behalf of the user. The user is not shown a signin card, unless SSO fails.
  - `skill token` command that displays the user's token from the skill.
  - `skill logout` command that logs the user out of the skill.

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- Create a bot registration in the azure portal for the `skillBot` and update [skillBot/.env](skillBot/.env) with the `MicrosoftAppId` and `MicrosoftAppPassword` of the new bot registration
- Create a bot registration in the azure portal for the `rootBot` and update [rootBot/.env](rootBot/.env) with the `MicrosoftAppId` and `MicrosoftAppPassword` of the new bot registration
- Update the `SkillAppId` variable in [rootBot/.env](rootBot/.env) with the `AppId` for the skill you created in the previous step
- (Optionally) Add the `rootBot` `MicrosoftAppId` to the `AllowedCallers` comma separated list in [skillBot/.env](skillBot/.env)

- Setup the 2 AAD applications for SSO as per steps given in [SkillBot AAD](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-authentication-sso?view=azure-bot-service-4.0&tabs=srb%2Ccsharp#create-the-azure-ad-identity-application-1) and [RootBot AAD](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-authentication-sso?view=azure-bot-service-4.0&tabs=sb%2Ccsharp#create-the-azure-ad-identity-application). You will end up with 2 aad applications - one for the skill consumer and one for the skill.
- Create an aadv2 connection in the bot registration for the `SkillBot` and fill in values from the aadv2 application created for SSO, as per the [docs](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-authentication-sso?view=azure-bot-service-4.0&tabs=srb%2Ccsharp#create-azure-ad-connection-1). Update [SkillBot/appsettings.json](SkillBot/appsettings.json) with the `ConnectionName`  
- Create an aadv2 connection in the bot registration for the `RootBot` and fill in values from the aadv2 application created for SSO, as per the [docs](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-authentication-sso?view=azure-bot-service-4.0&tabs=sb%2Ccsharp#create-azure-ad-connection). Update [RootBot/appsettings.json](SkillBot/appsettings.json) with the `ConnectionName`  

- In a terminal, navigate to `samples\javascript_nodejs\82.sso-with-skills\skillBot`

    ```bash
    cd samples\javascript_nodejs\82.sso-with-skills\skillBot
    ```

- Install npm modules and start the bot

    ```bash
    npm install
    npm start
    ```

- Open a **second** terminal window and navigate to `samples\javascript_nodejs\82.sso-with-skills\rootBot`

    ```bash
    cd samples\javascript_nodejs\82.sso-with-skills\rootBot
    ```

- Install npm modules and start the bot

    ```bash
    npm install
    npm start
    ```

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.9.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`, the `MicrosoftAppId` and `MicrosoftAppPassword` for the `rootBot`
- Type `login` and complete the sign-in flow.  When the flow is complete, a token should be displayed.
- Type `skill login`.  This should initiate the token exchange between the `SkillBot` and `RootBot`, resulting in a valid token displayed.

## Deploy the bots to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

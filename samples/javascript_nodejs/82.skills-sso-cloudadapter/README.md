# SSO with Skills Sample

Bot Framework v4 skills SSO sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to connect a root bot with a skill bot and exchange OAuth credentials.

## Prerequisites

- [Node.js](https://nodejs.org) version 10.14 or higher.

    ```bash
    # Determine node version.
    node --version
    ```

## Key concepts in this sample

The solution includes a parent bot ([`rootBot`](rootBot/bots/rootBot.js)) and a skill bot ([`skillBot`](skillBot/bots/skillBot.js)) and shows how the skill bot can accept OAuth credentials from the root bot, without needing to send it's own OAuthPrompt.

This is the general authentication flow:

1. Root bot prompts user to authenticate with an OAuth prompt card.
2. Authentication succeeds and the user is granted a token.
3. User performs an action on the skill bot that requires authentication.
4. The skill bot sends an OAuth prompt card to the root bot.
5. The root bot intercepts the OAuth prompt card, aware that the user is already authenticated and that the user should authenticate with the skill via SSO.
6. Instead of showing the OAuth prompt card to the user, the root bot sends a token exchange request invoke activity along with the token to the skill.
7. The skill's OAuth prompt receives the token exchange request and uses the token from the root bot to continue authenticating.

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- Create a bot registration in the azure portal for the **SkillBot** and update the [.env](skillBot/.env) file with the `MicrosoftAppId` and `MicrosoftAppPassword` of the new bot registration.
- Update the `SkillAppId` field in the **RootBot** [.env](rootBot/.env) with the app ID for the skill you created in the previous step.
- Create a bot registration in the azure portal for the **RootBot** and update [.env](rootBot/.env) with the `MicrosoftAppId` and `MicrosoftAppPassword` of the new bot registration.
- Add the RootBot `MicrosoftAppId` to the `AllowedCallers` list in the **SkillBot** [.env](skillBot/.env).
- Create and configure an OAuth connection for **RootBot**:
  1. Create an Azure Active Directory V2 application for the root bot following the steps described in [Create the Azure AD identity for RootBot](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-authentication-sso?view=azure-bot-service-4.0&tabs=csharp%2Ceml#create-the-azure-ad-identity-for-rootbot)
  1. Open the **RootBot** registration in the Azure portal, navigate to the Configuration tab and add a new OAuth Connection Settings using the settings of the app you created in the previous step as described in [Create an OAuth connection for a root bot](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-authentication-sso?view=azure-bot-service-4.0&tabs=csharp%2Ceml#create-an-oauth-connection-settings)
  1. Update the **RootBot** [.env](rootBot/.env) `ConnectionName` property with the name of the connection you created in the previous step.
- Create and configure an OAuth connection for **SkillBot**:
  1. Create an Azure Active Directory V2 application for the skill following the steps described in [Create the Azure AD identity for SkillBot](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-authentication-sso?view=azure-bot-service-4.0&tabs=csharp%2Ceml#create-the-azure-ad-identity-for-skillbot)
  2. Open the **SkillBot** registration in the Azure portal, navigate to the Configuration tab and add a new OAuth Connection Settings using the settings of the app you created in the previous step as described in [Create an OAuth connection for a skill](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-authentication-sso?view=azure-bot-service-4.0&tabs=csharp%2Ceml#create-an-oauth-connection-settings-1)
  3. Update the **SkillBot** [.env](skillBot/.env) `ConnectionName` property with the name of the connection you created in the previous step.
- For each bot directory, `skillBot` and `rootBot` as `<botDirectory>`:

- In a terminal, navigate to `samples/javascript_nodejs/82.skills-sso-cloudadapter/<botDirectory>`

    ```bash
    cd samples/javascript_nodejs/82.skills-sso-cloudadapter/<botDirectory>
    ```

- Install modules

    ```bash
    npm install
    ```

- Start the bot

    ```bash
    npm start
    ```

**Note:** leave the `MicrosoftAppType` and `MicrosoftAppTenantId` empty to try this example, see the [Implement a skill](https://docs.microsoft.com/en-us/azure/bot-service/skill-implement-skill?view=azure-bot-service-4.0&tabs=cs) article for additional information on what authentication types are supported for skills.

## Testing the bot using the Bot Framework Emulator

The [Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.14.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot.
- Enter a Bot URL of `http://localhost:3978/api/messages`, the `MicrosoftAppId` and `MicrosoftAppPassword` for the `RootBot`.
- Click `Connect`.
- Follow the prompts to initiate the token exchange between the `SkillBot` and `RootBot`, resulting in a valid token displayed.

## Deploy the bots to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

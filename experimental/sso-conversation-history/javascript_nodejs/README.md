# WebChat Conversation History

Hosted example: [SSOConversationHistoryAPI](http://SSOConversationHistoryAPI.azurewebsites.net)

## Sample origin

This is a ported version of the [Single sign-on demo for enterprise apps using OAuth](https://github.com/microsoft/BotFramework-WebChat/tree/master/samples/19.a.single-sign-on-for-enterprise-apps).  The code has been modified to include the following:

### CosmosDb storage for Conversation History and User State

- CosmosDbTranscriptLogger: logs all incoming and outgoing activities

- AuthUserState: custom UserState for storing user scoped data based on signed in user id

- UserConversationHistory: retrieves paged transcripts of history for signed in users

### Other changes to support Conversation History 
- HistoryMiddleware: adds AuthUserId to incoming and outgoing activities, ensuring the id is logged with every message

- Running total of numbers stored in AuthuserState for signed in users

- Scroll to Top in WebChat triggers history retrieval

### Support for Enhanced Direct Line Authentication 

- generateDirectLineToken now embeds a generated id beginning with 'dl_'

## Additional Setup

In addition to the below setup and configuration, CosmosDb is required for Conversation History.  How to create a CosmosDb database and container is described here: [how-to-create-container](https://docs.microsoft.com/en-us/azure/cosmos-db/how-to-create-container)  Below are the additional settings that must be added to the bot project's .env:

```json
DB_SERVICE_ENDPOINT=https://YourCosmosDbService.documents.azure.com:443/
AUTH_KEY=YourCosmosDbAuthKey

BOT_STATE_DATABASE=botdbv4Example
BOT_STATE_COLLECTION=BotStateCollectionbotExample

ACTIVITY_LOGS_DATABASE=botdbv4Example
ACTIVITY_LOGS_COLLECTION=ActivityLogsCollectionExample
```


# NOTE: the following describes the original sample setup and configuration
Note: see [Conversation History Pull Request](https://github.com/EricDahlvang/BotFramework-WebChat/pull/1/files) for a diff of changes required to support Conversation History

# [Single sign-on demo for enterprise apps using OAuth](https://github.com/microsoft/BotFramework-WebChat/tree/master/samples/19.a.single-sign-on-for-enterprise-apps)

[![Deploy Status](https://fuselabs.vsrm.visualstudio.com/_apis/public/Release/badge/531382a8-71ae-46c8-99eb-9512ccb91a43/9/9)](https://ssoconversationhistoryapi.azurewebsites.net//)

# Description

In this demo, we will show you how to authorize a user to access resources on an enterprise app with a bot. Two types of resources are used to demonstrate the interoperability of OAuth: [Microsoft Graph](https://developer.microsoft.com/en-us/graph/) and [GitHub API](https://developer.github.com/v3/).

> When dealing with personal data, please respect user privacy. Follow platform guidelines and post your privacy statement online.

## Background

Different companies may use different access delegation technologies to protect their resources. In our demo, we are targeting authorization through [OAuth 2.0](https://tools.ietf.org/html/rfc6749)).

Although OAuth and [OpenID](https://openid.net/) are often related to each other, they solve different problems. OAuth is for authorization and access delegation, while OpenID is for authentication and user identity.

Instead of OpenID, most enterprise apps use OAuth plus a user profile API to identify an individual user. In this demo, we will demonstrate how to use OAuth to obtain access to user profile API and use the API to identifying the accessor.

This demo does not include any threat models and is designed for educational purposes only. When you design a production system, threat-modelling is an important task to make sure your system is secure and provide a way to quickly identify potential source of data breaches. IETF [RFC 6819](https://tools.ietf.org/html/rfc6819) and [OAuth 2.0 for Browser-Based Apps](https://tools.ietf.org/html/draft-ietf-oauth-browser-based-apps-01#section-9) is a good starting point for threat-modelling when using OAuth 2.0.

# Test out the hosted sample

-  [Try out MockBot](https://webchat-sample-sso.azurewebsites.net/)

# How to run locally

This demo integrates with multiple services. There are multiple services you need to setup in order to host the demo.

1. [Clone the code](#clone-the-code)
1. [Setup OAuth via GitHub](#setup-oauth-via-github)
1. [Setup OAuth via Azure Active Directory](#setup-oauth-via-azure-active-directory)
1. [Setup Azure Bot Services](#setup-azure-bot-services)
1. [Prepare and run the code](#prepare-and-run-the-code)

## Clone the code

To host this demo, you will need to clone the code and run locally.

1. Clone this repository
1. Create two files for environment variables, `/bot/.env` and `/rest-api/.env`
   -  In `/rest-api/.env`:
      -  Write `AAD_OAUTH_REDIRECT_URI=http://localhost:3000/api/aad/oauth/callback`
         -  When Azure Active Directory completes the authorization flow, it will send the browser to this URL. This URL must be accessible by the browser from the end-user machine
      -  Write `GITHUB_OAUTH_REDIRECT_URI=http://localhost:3000/api/github/oauth/callback`
         -  Same as Azure Active Directory, this is the URL for GitHub to send its result

## Setup OAuth via GitHub

If you want to authenticate on GitHub, follow the steps below.

1. Sign into GitHub and create a new [OAuth application](https://github.com/settings/developers)
   1. Browse to https://github.com/settings/developers
   1. Select "OAuth Apps"
   1. Click "New OAuth App" button
   1. Fill out "Application name" and "Homepage URL", for example, "Web Chat SSO Sample"
      -  The "Application name" and "Homepage URL" will be shown to the user when they authorize your GitHub OAuth app
   1. In "Application callback URL", enter `http://localhost:3000/api/github/oauth/callback`
   1. Click "Register application"
1. Save the "Client ID" and "Client Secret" to `/rest-api/.env`
   -  `GITHUB_OAUTH_CLIENT_ID=a1b2c3d`
   -  `GITHUB_OAUTH_CLIENT_SECRET=a1b2c3d4e5f6`

## Setup OAuth via Azure Active Directory

If you want to authenticate on Azure Active Directory, follow the steps below.

-  Go to your [Azure Active Directory](https://ms.portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/Overview)
-  Create a new application
   1. Select "App registrations"
   1. Click "New registration"
   1. Fill out "Name", for example, "Web Chat SSO Sample"
   1. In "Redirect URI (optional)" section, add a new entry
      1. Select "Public client (mobile & desktop)" as type
         -  Instead of client secret, we are using PKCE ([RFC 7636](https://tools.ietf.org/html/rfc7636)) to exchange for authorization token, thus, we need to set it to ["Public client" instead of "Web"](https://docs.microsoft.com/en-us/azure/active-directory/develop/v1-protocols-oauth-code#use-the-authorization-code-to-request-an-access-token)
      1. Enter `http://localhost:3000/api/aad/oauth/callback` as the redirect URI
         -  This must match `AAD_OAUTH_REDIRECT_URI` in `/rest-api/.env` we saved earlier
   -  Click "Register"
-  Save the client ID
   1. Select "Overview"
   1. On the main pane, copy the content of "Application (client) ID" to `/rest-api/.env`, it should looks be a GUID
      -  `AAD_OAUTH_CLIENT_ID=12345678abcd-1234-5678-abcd-12345678abcd`

## Setup Azure Bot Services

> We prefer using [Bot Channel Registration](https://ms.portal.azure.com/#create/Microsoft.BotServiceConnectivityGalleryPackage) during development. This will help you diagnose problems locally without deploying to the server and speed up development.

You can follow our instructions on how to [setup a new Bot Channel Registration](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-quickstart-registration?view=azure-bot-service-3.0).

1. Save the Microsoft App ID and password to `/bot/.env`
   -  `MICROSOFT_APP_ID=12345678-1234-5678-abcd-12345678abcd`
   -  `MICROSOFT_APP_PASSWORD=a1b2c3d4e5f6`
1. Save the Web Chat secret to `/rest-api/.env`
   -  `DIRECT_LINE_SECRET=a1b2c3.d4e5f6g7h8i9j0`

> When you are building your production bot, never expose your Web Chat or Direct Line secret to the client. Instead, you should use the secret to generate a limited token and send it to the client. For information, please refer [to this page on how to generate a Direct Line token](https://docs.microsoft.com/en-us/azure/bot-service/rest-api/bot-framework-rest-direct-line-3-0-authentication?view=azure-bot-service-4.0#generate-token) and [Enhanced Direct Line Authentication feature](https://blog.botframework.com/2018/09/25/enhanced-direct-line-authentication-features/).

During development, you will run your bot locally. Azure Bot Services will send activities to your bot through a public URL. You can use [ngrok](https://ngrok.com/) to expose your bot server on a public URL.

1. Run `ngrok http -host-header=localhost:3978 3978`
1. Update your Bot Channel Registration. You can use [Azure CLI](https://aka.ms/az-cli) or [Azure Portal](https://portal.azure.com)
   -  Via Azure CLI
      -  Run `az bot update --resource-group <your-bot-rg> --name <your-bot-name> --subscription <your-subscription-id> --endpoint "https://a1b2c3d4.ngrok.io/api/messages"`
   -  Via Azure Portal
      -  Browse to your Bot Channel Registration
      -  Select "Settings"
      -  In "Configuration" section, set "Messaging Endpoint" to `https://a1b2c3d4.ngrok.io/api/messages`

## Prepare and run the code

1. Under `app`, `bot`, and `rest-api` folder, run the following:
   1. `npm install`
   1. `npm start`
1. Browse to http://localhost:3000/ to start the demo

# Things to try out

-  Notice there are two sign-in buttons on top-right hand corner
   -  After signed in, both the website and the bot get your sign in information
-  Type, "where are my packages" in Web Chat
   -  If not signed in, the bot will present a sign-in button
   -  If signed in, the bot will answer the question
-  Type, "bye" in Web Chat
   -  If signed in, the bot will sign you out

# Code

-  `/app/` is the React app built using `create-react-app` scaffold
-  `/bot/` is the bot server
-  `/rest-api/` is the REST API for handling OAuth requests
   -  `GET /api/aad/oauth/authorize` will redirect to Azure AD OAuth authorize page at https://login.microsoftonline.com/12345678-1234-5678-abcd-12345678abcd/oauth2/v2.0/authorize
   -  `GET /api/aad/oauth/callback` will handle callback from Azure AD OAuth
   -  `GET /api/aad/settings` will send Azure AD OAuth settings to the React app
   -  `GET /api/directline/token` will generate a new Direct Line token for the React app
   -  `GET /api/github/oauth/authorize` will redirect to GitHub OAuth authorize page at https://github.com/login/oauth/authorize
   -  `GET /api/github/oauth/callback` will handle callback from GitHub AD OAuth
   -  `GET /api/github/settings` will send GitHub OAuth settings to the React app
   -  It will serve React app as a static content
   -  During development-time, it will also serve the bot server via `/api/messages`
      -  To enable this feature, add `PROXY_BOT_URL=http://localhost:3978` to `/web/.env`
      -  This will forward all traffic from `https://a1b2c3d4.ngrok.io/api/messages` to `https://localhost:3978/api/messages`

# Overview

This sample includes multiple parts:

-  A basic web page with sign in and sign out button, coded with React
-  Web Chat integrated, coded with pure JavaScript
-  Wiring between the web page and Web Chat through DOM events
   -  When web page sign in, it should emit DOM event `accesstokenchange` with `{ data: { accessToken, provider } }`
   -  When the bot ask for sign in, Web Chat will emit DOM event `signin` with `{ data: { provider: 'aad/github' } }`
   -  When the bot ask for sign out, Web Chat will emit DOM event `signout`
-  For bot, OAuth access token is piggybacked on every user-initiated activity through `channelData.oauthAccessToken` and `channelData.oauthProvider`

## Assumptions

-  Developer has an existing enterprise web app that uses OAuth to access protected resources
   -  We assume the OAuth access token lives in the browser's memory and is accessible through JavaScript
      -  Access token can live in browser memory but must be secured during transmit through the use of TLS
      -  More about security considerations can be found at [IETF RFC 6749 Section 10.3](https://tools.ietf.org/html/rfc6749#section-10.3)
-  Developer know how to alter existing JavaScript code around their existing UI for OAuth

## Goals

-  Website and bot conversation supports both anonymous and authenticated access
   -  Forced page refresh and/or new conversation is not mandated
-  End-user is able to sign in through the web page, and is recognized by the bot immediately
   -  Vice versa, end-user is able to sign in through the bot, and is recognized by the web page immediately
-  End-user is able to sign in through the web page and sign out though the bot
   -  Vice versa, end-user is able to sign in through the bot and sign out through the web page

## Organization of JavaScript code

In our demo, we built an enteprise single-page app using React. Then, we use a `<script>` tag to embed Web Chat. This is for separating the code so developers reading this sample can study the changes and easily understand the interactions between them.

You are not required to code your web app in React or use Web Chat via `<script>` tag. In fact, you can write both your web app and embed Web Chat using either pure JavaScript or React.

### Wiring up components

Since the demo is running in a heterogeneous environment (both React and pure JavaScript), additional wire-ups are required. We use DOM events to wire up the enterprise app (authentication UI) and Web Chat.

In your production system, since you are probably in a homogenous environment (either React or pure JavaScript), you may want to use Redux or other mechanisms to wire up different UI components.

This demo is coded in heterogeneous environment. Web Chat code is written in pure JavaScript, and the website is written in React. This makes the Web Chat integration code easier to extract from the webpage code.

## Content of the `.env` files

The `.env` files hold the environment variables critical to run the service. These are usually security-sensitive information and must not be committed to version control. Although we recommend keeping these keys in Azure Vault, for simplicity of this sample, we would keep them in `.env` files.

To ease the setup of this sample, here is the template of `.env` files.

### `/bot/.env`

```
MICROSOFT_APP_ID=12345678-1234-5678-abcd-12345678abcd
MICROSOFT_APP_PASSWORD=a1b2c3d4e5f6
```

### `/rest-api/.env`

```
AAD_OAUTH_CLIENT_ID=12345678abcd-1234-5678-abcd-12345678abcd
AAD_OAUTH_REDIRECT_URI=http://localhost:3000/api/aad/oauth/callback
DIRECT_LINE_SECRET=a1b2c3.d4e5f6g7h8i9j0
GITHUB_OAUTH_CLIENT_ID=a1b2c3d
GITHUB_OAUTH_CLIENT_SECRET=a1b2c3d4e5f6
GITHUB_OAUTH_REDIRECT_URI=http://localhost:3000/api/github/oauth/callback
```

## OAuth provider support single redirect URI only

In this sample, we do not use OAuth card due to technical limitations on some OAuth providers which support single redirect URI only.

In order to use the website to sign in, the developer will need to set the redirect URI to their own web API.

In order to use the bot to sign in, in the OAuth provider, the developer will need to set the redirect URI to https://token.botframework.com/.auth/web/redirect.

Since some OAuth providers do not support multiple redirect URIs, we prefer using a single redirect URI from the web API to make sure existing authorization flow is not disturbed.

# Frequently asked questions

## How can I reset my authorization?

After having signed in on this app, click the profile photo on the upper-right hand corner, select "Review access on Office.com" or "Review access on GitHub". Then, you will be redirected to the OAuth provider page to remove your authorization.

-  For GitHub, you can click the "Revoke access" button
-  For Azure Active Directory
   1. On the AAD dashboard page, wait until "App permissions" loads. Here you see how many apps you have authorized
   1. Click "Change app permissions"
   1. In the "You can revoke permission for these apps" section, click the "Revoke" button below your app registration

# Further reading

## Related articles

-  [RFC 6749: The OAuth 2.0 Authorization Framework](https://tools.ietf.org/html/rfc6749)
-  [RFC 6819: OAuth 2.0 Threat Model and Security Considerations](https://tools.ietf.org/html/rfc6819)
-  [RFC 7636: Proof Key for Code Exchange by OAuth Public Clients](https://tools.ietf.org/html/rfc7636)
-  [IETF Draft: OAuth 2.0 for Browser-Based Apps](https://tools.ietf.org/html/draft-ietf-oauth-browser-based-apps-01)
-  [Bot Framework Blog: Enhanced Direct Line Authentication feature](https://blog.botframework.com/2018/09/25/enhanced-direct-line-authentication-features/)

## OAuth access token vs. refresh token

To make this demo simpler to understand, instead of refresh token, we are obtaining the access token via Authorization Code Grant flow. Access token is short-lived and considered secure to live inside the browser.

In your production scenario, you may want to obtain the refresh token with "Authorization Code Grant" flow instead of using the access token. We did not use the refresh token in this sample as it requires server-to-server communications and secured persistent storage, it would greatly increase the complexity of this demo.

## Threat model

To reduce complexity, this sample is limited in scope. In your production system, you should consider enhancing it and review its threat model.

-  Refreshing the access token
   -  Using silent prompt for refreshing access token
      -  Some OAuth providers support `?prompt=none` for refreshing access token silently through `<iframe>`
   -  Using Authorization Code Grant flow with refresh token
      -  Save the refresh token on the server side of your web app. Never expose it to the browser or the bot
      -  This will also create a smooth UX by reducing the need for UI popups
-  Threat model
   -  IETF [RFC 6819](https://tools.ietf.org/html/rfc6819) is a good starting point for threat-modelling when using OAuth 2.0

## Mixed conversations

To lower the barrier for the end-user to initiate a conversation with the bot, in this sample, the conversation can be both anonymous or authenticated.

That means at some points of time, the mixed conversation can be authenticated as different users. If it is not a desirable scenario for your use case, you might want to create a new conversation if the user signed out.

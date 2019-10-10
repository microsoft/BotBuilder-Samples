# Teams Search-based Messaging Extensions Bot

Search-based Messaging Extensions allow users to have bots search for information on the user's behalf in Teams. 

___

To create a Teams Bot that contains Search-based Messaging Extensions, users must first create a [Teams Manifest](https://docs.microsoft.com/en-us/microsoftteams/platform/resources/schema/manifest-schema) which will outline where the extension is available.

> _Note_
>
> _The name of the feature was changed from "compose extension" to "messaging extension" in November, 2017, but the manifest name remains the same so that existing extensions continue to function._

### Example Teams Manifest for Search-based Messaging Extensions bot

```json
{
  "$schema": "https://github.com/OfficeDev/microsoft-teams-app-schema/blob/preview/DevPreview/MicrosoftTeams.schema.json",
  "manifestVersion": "devPreview",
  "version": "1.0",
  "id": "<<YOUR_GENERATED_APP_GUID>>",
  "packageName": "com.microsoft.teams.samples.v4bot",
  "developer": {
    "name": "Microsoft Corp",
    "websiteUrl": "https://example.azurewebsites.net",
    "privacyUrl": "https://example.azurewebsites.net/privacy",
    "termsOfUseUrl": "https://example.azurewebsites.net/termsofuse"
  },
  "name": {
    "short": "search-extension-settings",
    "full": "Microsoft Teams V4 Search Messaging Extension Bot and settings"
  },
  "description": {
    "short": "Microsoft Teams V4 Search Messaging Extension Bot and settings",
    "full": "Sample Search Messaging Extension Bot using V4 Bot Builder SDK and V4 Microsoft Teams Extension SDK"
  },
  "icons": {
    "outline": "icon-outline.png",
    "color": "icon-color.png"
  },
  "accentColor": "#abcdef",
  "bots": [
    {
      "botId": "<<YOUR_BOTS_MSA_APP_ID>>",
      "scopes": ["personal", "team"]
    }
  ],
  "composeExtensions": [
    {
      "botId": "<<YOUR_BOTS_MSA_APP_ID>>",
      "canUpdateConfiguration": true,
      "commands": [
        {
          "id": "searchQuery",
          "context": ["compose", "commandBox"],
          "description": "Test command to run query",
          "title": "Search",
          "type": "query",
          "parameters": [
            {
              "name": "searchQuery",
              "title": "Search Query",
              "description": "Your search query",
              "inputType": "text"
            }
          ]
        }
      ],
      "messageHandlers": [
        {
          "type": "link",
          "value": {
            "domains": [
              "*.ngrok.io"
            ]
          }
        }
      ]
    }
  ],
  "validDomains": [
    "*.ngrok.io"
  ]
}
```
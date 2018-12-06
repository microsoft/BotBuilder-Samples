# Cognitive Services Prerequisits
This bot relies on the use of the [LUIS][1] cognitive service for language understanding.

Follow the instructions to setup and configure a LUIS application before you run the bot.

## Overview
This sample uses LUIS, an AI based cognitive service, to implement language understanding.

- Provision a LUIS service application
- Import a sample language model to seed the application
- Train and publish a languge model
- Get the service application API configuration values
- Configure the bot to use the LUIS service application


### Provision a LUIS service application
Use the LUIS portal to provision a LUIS service application.  This service application will be trained with data from the sample and then used by the sample to enable rich language understanding capabilities.

1.  Provision a LUIS service application
    - Navigate to [LUIS portal][1].
    - Click the `Sign in` button.
    - Click on `My Apps` top-level menu.
1.  Import a sample language model to seed the application
    - Click on the `Import new app` button.
    - Click on the `Choose app file (JSON format)...`
    - Navigate to `<root_project_folder>/cognitiveModels` folder.
    - Select `basicBot.json`.
    - Optionally provide a name for the LUIS app.
    - Click `Done`.  After processing, `App Assets` will be displayed showing the Intents from the imported model.
1.  Train and Publish the language model
    - Click on the `Train` button. To train your language model.
    - Click on the `Publish` button.  To display the _Publish this app_ dialog.
    - Select `Production` from the Environment drop down.
    - Click `Publish`.  LUIS will send a _Publishing Complete_ notification when the publish step has finished.

### Get the service application API configuration values
The LUIS service application is now ready for use by the bot.  In order for the bot to talk to the LUIS application it needs three values that will be used to configure the bot.  These values are found on the portal.  The steps below will show how to find these values.  These values will be used to configure the bot so it can use the LUIS service application.

1.  Get the Application ID
    - Click the `Manage` menu on the top right of the LUIS portal.
    - Click the `Application Information` menu in the left-hand nav.  This displays the _Application Information_ page.
    - Notice the `Application ID`.  In steps that follow you'll use this Application ID to configure the bot.
1.  Get the Authoring Key
    - Click the `Keys and Endpoints` menu in the left-hand nav.  This displays the _Keys and Endpoint settings_ page.
    - Notice the `Authoring Key` found at the top of the page.  In steps that follow you'll use this Authoring Key to configure the bot.
1.  Get the Region
    - Also on the _Keys and Endpoint settings_ page.
    - Notice the `Endpoint` URL.  The first part of the URL contains the name of the region in which the service application was provisioned.  In steps that follow you'll use the region to configure the bot.

### Configure the bot to use the LUIS service application
Now that the LUIS service application has been provisioned, and the location of the `Application ID`, `Endpoint URL`, and `Subscription Key` are known (see previous section), the bot can now be configured to use this information.  The steps that follow show how to use the `<%= botname %>.bot` file to configure the bot to use the LUIS service application.
1.  Open `<%= botname %>.bot` with a text editor.
1.  Find the `services` section and within that section there is a section for LUIS.  The LUIS section looks as follows:
    ```javascript
    {
        "type": "luis",
        "name": "<%= botname %>-LUIS",
        "appId": "____ADD_APPLICATION_ID_HERE____",
        "version": "0.1",
        "authoringKey": "____ADD_AUTHORING_KEY_HERE____",
        "subscriptionKey": "",
        "region": "____ADD_REGION_HERE____",
        "id": "588"
    }
    ```
1.  Update the `"appId": "____ADD_APPLICATION_ID_HERE____"` key/value pair to use the `Application ID` value.
1.  Update the `"authoringKey": "____ADD_AUTHORING_KEY_HERE____"` key/value pair to use the `Authoring Key` value.
1.  Update the `"region": "____ADD_REGION_HERE____"` key/value pair to use the first part of the `Endpoint` URL as the value.
    For example, for the following Endpoint URL:
    https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/{LuisAppID}?subscription-key={LuisSubscriptionKey}&timezoneOffset=-360&q=
    **For this example, the region value would be `westus`**
1.  Save the changes made to `<%= botname %>.bot`.

When the above steps are completed, the LUIS section of the `<%= botname %>.bot` should now be using the `Application ID`, `Subscription Key`, and `Region` retrieved from the LUIS portal.  [See an example .bot service configuration for using LUIS here][2].

## Cognitive Services Prerequisits are Complete
With the above instructions completed, the `<%= botname %>` with use the `<%= botname %>.bot` file to retrieve the LUIS service application settings, enabling the bot to use a rich AI language model for language understanding.
[Return to README.md][3]


[1]: https://www.luis.ai
[2]: https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&tabs=js#configure-your-bot-to-use-your-luis-app
[3]: ./README.md

# NamedPipeBot

Bot Framework v4 named pipe bot sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to use named pipes to connect the bot with the Direct Line App Service Extension.

## Prerequisites

- [Node.js](https://nodejs.org) version 10.14 or higher

    ```bash
    # determine node version
    node --version
    ```

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `samples/javascript_nodejs/83.named-pipe-sample/named-pipe-bot`

    ```bash
    cd samples/javascript_nodejs/83.named-pipe-sample/named-pipe-bot
    ```

- Install modules

    ```bash
    npm install
    ```

- Start the bot

    ```bash
    npm start
    ```

## Try the bot using BotFramework Emulator

The bot should respond using the emulator, however, the named pipes functionality must be tested using webSockets.
Follow the sections below to connect the bot to the DirectLine App Service Extension and test the sample with a WebChat client.

## Deploy the bot to Azure

To test named pipes, the bot needs to be deployed in Azure.
Create the resources in Azure and deploy your bot. See [Deploy your bot to Azure](https://learn.microsoft.com/en-us/azure/bot-service/provision-and-publish-a-bot?view=azure-bot-service-4.0&tabs=multitenant%2Cjavascript) for a complete list of deployment instructions.
Make sure the `web.config` file is generated and add the following settings:
- Enable websockets:
    ```xml
    <webSocket enabled="true" />
    ```
- Add the _AspNetCore_ handler and rule needed by Direct Line App Service extension to service requests:
    ```xml
    <handlers>
      <!-- Indicates that the server.js file is a node.js site to be handled by the iisnode module -->
      <add name="aspNetCore" path="*/.bot/*" verb="*" modules="AspNetCoreModule" resourceType="Unspecified" />
    </handlers>
    <rewrite>
      <rules>
        <!-- Do not interfere with Direct Line App Service extension requests. (This rule should be as high in the rules section as possible to avoid conflicts.) -->
        <rule name ="DLASE" stopProcessing="true">
          <conditions>
            <add input="{REQUEST_URI}" pattern="^/.bot"/>
          </conditions>
        </rule>
      </rules>
    </rewrite>
    ```

## Configure DirectLine App Service Extension

Follow the instructions in [Enable DL ASE](https://learn.microsoft.com/en-us/azure/bot-service/bot-service-channel-directline-extension-node-bot?view=azure-bot-service-4.0#enable-bot-direct-line-app-service-extension) to configure the DirectLine App Service extension in the bot.

> **Note:** The AppService must have CORS Allowed Origins set as '*' for the webchat client to communicate with the bot.
Set this value in the AppService configuration under the API settings:
![corsConfig](media/corsConfig.png)

> **Note:** The `WEBSITE_NAME` and `DIRECT_LINE_SECRET` settings must be filled with the AppService name and the DirectLine secret in the `.env` file or in the AppService configuration in Azure Portal.

## Start the WebChat client

Followed this [README](../webchat-client/README.md) to run the WebChat client included in this sample and communicate with your bot.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
-  [Direct Line App Service Extension](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-directline-extension?view=azure-bot-service-4.0)
-  [Cross-Origin Resource Sharing (CORS)](https://docs.microsoft.com/en-us/learn/modules/set-up-cors-website-storage/)
- [Restify](https://www.npmjs.com/package/restify)
- [dotenv](https://www.npmjs.com/package/dotenv)

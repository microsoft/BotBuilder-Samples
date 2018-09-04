The example shows the use of the botbuilder-js SDK for the browser using the [BotFramework-WebChat](https://github.com/Microsoft/BotFramework-WebChat) and a custom [WebChatAdapter](/src/webChatAdapter.js). After running the bot, to see it in action, visit `http://localhost:8080`

## To try this sample
- Clone the repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- In a terminal, navigate to javascript_es6\1.a.browser-echobot
    ```bash
    cd javascript_es6\1.a.browser-echobot
    ```
- Point to the myget feed 
    ```bash
    npm config set registry https://botbuilder.myget.org/F/botbuilder-v4-js-daily/npm/
    ```
- Install modules and start the bot
    ```bash
    npm i & npm start
    ```
- To reset registry, you can do
    ```bash
    npm config set registry https://registry.npmjs.org/
    ```

### Visual Stydio Code:
- open `javascript_es6\1.a.browser-echobot` folder 
- bring up a terminal, navigate to `javascript_es6\1.a.browser-echobot` folder
- type `npm i` to install the sample's dependencies
- type `npm start` to run the bot
- navigate to `http://localhost:8080`


# Adapters
Developers can use the [BotAdapter](https://github.com/Microsoft/botbuilder-js/blob/master/doc/botbuilder/classes/botbuilder.botadapter.md) abstract base class to implement their own custom adapters. Implementing a custom adapter allows users to connect bots to connect to channels not supported by the [Bot Framework](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-manage-channels?view=azure-bot-service-4.0). In this sample, a custom [WebChatAdapter](./src/WebChatAdapter.js) has been implemented so that the entirety of the bot can live in a user's browser.


# Further reading

- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot State](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-storage-concept?view=azure-bot-service-4.0)
- [Write directly to storage](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-storage?view=azure-bot-service-4.0&tabs=jsechoproperty%2Ccsetagoverwrite%2Ccsetag)
- [Managing conversation and user state](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-state?view=azure-bot-service-4.0&tabs=js)
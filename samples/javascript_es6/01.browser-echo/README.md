# browser echo sample

Bot Framework v4 browser bot sample

The example shows the use of the `botbuilder-js` SDKs for the browser using the [BotFramework-WebChat](https://github.com/Microsoft/BotFramework-WebChat) and a custom [WebChatAdapter](/src/webChatAdapter.js).

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `samples/javascript_es6/01.browser-echo`

    ```bash
    cd samples/javascript_es6/01.browser-echo
    ```

- Install modules

    ```bash
    npm install
    ```

- Build the sample

    ```bash
    npm run build
    ```

- Start the bot

    ```bash
    npm start
    ```

- Launch a web browser and navigate to [http://localhost:8080](http://localhost:8080).

## Adapters

Developers can use the [BotAdapter](https://docs.microsoft.com/en-us/javascript/api/botbuilder-core/botadapter) abstract base class to implement their own custom adapters.
Implementing a custom adapter allows users to connect bots to channels not supported by the [Bot Framework](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-manage-channels?view=azure-bot-service-4.0).
In this sample, a custom [WebChatAdapter](./src/WebChatAdapter.js) has been implemented so that the entirety of the bot is hosted in a user's browser.

Hosting a bot in the browser provides these benefits:

- A bot hosted in the user's browser has improved latency as there is no round-trip from the browser to a server hosting the bot.
- One engineering team in charge of bot design and the website. This can lead towards a more integrated UX and speed up development.
- A browser hosted bot can offload some of the work done by your servers by passing it to the user's machine.

## Further reading

- [Azure Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Bot State and storage](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-storage-concept?view=azure-bot-service-4.0)

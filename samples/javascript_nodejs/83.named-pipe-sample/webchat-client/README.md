# Sample - WebChat app with Direct Line App Service Extension protocol

## Description

A simple web page with Web Chat connected to a bot via [Direct Line App Service Extension protocol](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-directline-extension?view=azure-bot-service-4.0).

# How to run

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```

- In a terminal, navigate to `samples/javascript_nodejs/83.named-pipe-sample/webchat-client`

    ```bash
    cd samples/javascript_nodejs/83.named-pipe-sample/webchat-client
    ```

- In the `index.html` file, replace the `<BOT-HOST-NAME>` tag with the name of the Azure App Service.
-  Execute the following commands:

    ```bash
    npx webpack
    ```
    ```bash
    npx serve
    ```
-  Browse to [http://localhost:3000/](http://localhost:3000/)

# Things to try out

-  Type `hello`: you should be able to type to the bot and receive a response in plain text.

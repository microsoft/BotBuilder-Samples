This sample shows how to create a simple Echo bot that you can talk to from the terminal window..

# To try this sample
- Clone the repository
    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to javascript_nodejs/4.simple-prompt-bot
    ```bash
    cd javascript_nodejs/4.simple-prompt-bot
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

# Testing the bot 

Open the simple-prompt-bot.bot file in the Botframework Emulator.

Once connected, the bot will present you with a menu of sample prompts. If it does not, send "menu"

# Prompts

There are lots of prompts.

# Further reading
- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot basics](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Channels and Bot Connector service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
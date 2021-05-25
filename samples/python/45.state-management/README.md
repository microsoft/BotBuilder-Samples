# Save user and conversation data

This sample demonstrates how to save user and conversation data in a Python bot.
The bot maintains conversation state to track and direct the conversation and ask the user questions.
The bot maintains user state to track the user's answers.

## Running the sample
- Clone the repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- Bring up a terminal, navigate to `botbuilder-samples\samples\python\45.state-management` folder
- Activate your desired virtual environment
- In the terminal, type `pip install -r requirements.txt`
- Run your bot with `python app.py`

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to bot using Bot Framework Emulator
- Launch Bot Framework Emulator
- Paste this URL in the emulator window - http://localhost:3978/api/messages


## Bot State

A key to good bot design is to track the context of a conversation, so that your bot remembers things like the answers to previous questions. Depending on what your bot is used for, you may even need to keep track of state or store information for longer than the lifetime of the conversation. A bot's state is information it remembers in order to respond appropriately to incoming messages. The Bot Builder SDK provides classes for storing and retrieving state data as an object associated with a user or a conversation.

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

# Further reading

- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot State](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-storage-concept?view=azure-bot-service-4.0)
- [Write directly to storage](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-storage?view=azure-bot-service-4.0&tabs=csharpechorproperty%2Ccsetagoverwrite%2Ccsetag)
- [Managing conversation and user state](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-state?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)

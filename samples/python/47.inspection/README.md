# Inspection Bot

Bot Framework v4 Inspection Middleware sample.

This bot demonstrates a feature called Inspection. This feature allows the Bot Framework Emulator to debug traffic into and out of the bot in addition to looking at the current state of the bot. This is done by having this data sent to the emulator using trace messages.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a simple bot that accepts input from the user and echoes it back. Included in this sample are two counters maintained in User and Conversation state to demonstrate the ability to look at state.

This runtime behavior is achieved by simply adding a middleware to the Adapter. In this sample you can find that being done in `app.py`.

More details are available [here](https://github.com/microsoft/BotFramework-Emulator/blob/master/content/CHANNELS.md)

## Running the sample
- Clone the repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- Bring up a terminal, navigate to `botbuilder-samples\samples\python\47.inspection` folder
- Activate your desired virtual environment
- In the terminal, type `pip install -r requirements.txt`
- Run your bot with `python app.py`

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.5.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

### Special Instructions for Running Inspection

- In the emulator, select Debug -> Start Debugging.
- Enter the endpoint url (http://localhost:8080)/api/messages, and select Connect.
- The result is a trace activity which contains a statement that looks like /INSPECT attach < identifier >
- Right click and copy that response.
- In the original Live Chat session paste the value.
- Now all the traffic will be replicated (as trace activities) to the Emulator Debug tab.

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

# Further reading

- [Getting started with the Bot Inspector](https://github.com/microsoft/BotFramework-Emulator/blob/master/content/CHANNELS.md)
- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot State](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-storage-concept?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)

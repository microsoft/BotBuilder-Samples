# Proactive messages

Bot Framework v4 proactive messages bot sample

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to send proactive messages to users by capturing a conversation reference, then using it later to initialize outbound messages.

## Concepts introduced in this sample

Typically, each message that a bot sends to the user directly relates to the user's prior input. In some cases, a bot may need to send the user a message that is not directly related to the current topic of conversation. These types of messages are called proactive messages.

Proactive messages can be useful in a variety of scenarios. If a bot sets a timer or reminder, it will need to notify the user when the time arrives. Or, if a bot receives a notification from an external system, it may need to communicate that information to the user immediately. For example, if the user has previously asked the bot to monitor the price of a product, the bot can alert the user if the price of the product has dropped by 20%. Or, if a bot requires some time to compile a response to the user's question, it may inform the user of the delay and allow the conversation to continue in the meantime. When the bot finishes compiling the response to the question, it will share that information with the user.

This project has a notify endpoint that will trigger the proactive messages to be sent to
all users who have previously messaged the bot.

## Running the sample
- Clone the repository
```bash
git clone https://github.com/Microsoft/botbuilder-samples.git
```
- Activate your desired virtual environment
- Bring up a terminal, navigate to `botbuilder-samples\samples\python\16.proactive-messages` folder
- In the terminal, type `pip install -r requirements.txt`
- In the terminal, type `python app.py`

## Testing the bot using Bot Framework Emulator
[Microsoft Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to bot using Bot Framework Emulator
- Launch Bot Framework Emulator
- File -> Open Bot
- Paste this URL in the emulator window - http://localhost:3978/api/messages

With the Bot Framework Emulator connected to your running bot, the sample will now respond to an HTTP GET that will trigger a proactive message.  The proactive message can be triggered from the command line using `curl` or similar tooling, or can be triggered by opening a browser windows and nagivating to `http://localhost:3978/api/notify`.

### Using curl

- Send a get request to `http://localhost:3978/api/notify` to proactively message users from the bot.

   ```bash
    curl get http://localhost:3978/api/notify
   ```

- Using the Bot Framework Emulator, notice a message was proactively sent to the user from the bot.

### Using the Browser

- Launch a web browser
- Navigate to `http://localhost:3978/api/notify`
- Using the Bot Framework Emulator, notice a message was proactively sent to the user from the bot.

## Proactive Messages

In addition to responding to incoming messages, bots are frequently called on to send "proactive" messages based on activity, scheduled tasks, or external events.

In order to send a proactive message using Bot Framework, the bot must first capture a conversation reference from an incoming message using `TurnContext.get_conversation_reference()`. This reference can be stored for later use.

To send proactive messages, acquire a conversation reference, then use `adapter.continue_conversation()` to create a TurnContext object that will allow the bot to deliver the new outgoing message.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Send proactive messages](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-proactive-message?view=azure-bot-service-4.0&tabs=js)

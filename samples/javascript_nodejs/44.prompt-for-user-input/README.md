# Prompt users for input

This sample demonstrates how to create your own prompts with a bot.
The bot maintains conversation state to track and direct the conversation and ask the user questions.
The bot maintains user state to track the user's answers.



# Bot state

A bot is inherently stateless. Once your bot is deployed, it may not run in the same process or on the same machine from one turn to the next.
However, your bot may need to track the context of a conversation, so that it can manage its behavior and remember answers to previous questions.

In this example, the bot's state is used to a track number of messages.

- We use the bot's turn handler and user and conversation state properties to manage the flow of the conversation and the collection of input.
- We ask the user a series of questions; parse, validate, and normalize their answers; and then save their input.

This sample is intended to be run and tested locally and is not designed to be deployed to Azure.

# Further reading

- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction)
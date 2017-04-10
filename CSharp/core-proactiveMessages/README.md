# botFramework-proactiveMessages

What is this:

A common ask from developers using Microsoft Bot Framework is how to proactively initiate a message to the user. In other words, let's say some background process or notification causes the bot to initiate the message to the user, rather than just replying to a message received.

A few things have to be considered: First, do we know that user? We need to have the user ID before we do that, which means that the user must have had a message sent to the bot. Second, is the user currently having a conversation with the bot? If so, this notification will come as an interruption, therefore we need to think how we want this interruption to work, if we stop the current conversation and then come back to it, etc.

All samples below include a custom web api controller that also triggers the same proactive messages in order to demonstrate that any code, even if outside the actual dialogs can do the same.

## examples

**simpleSendMessage**: This is the simplest form of having the bot initiating the message ot the user. Basically the message is sent as an "ad hoc", which means it won't change the context of the current conversation and that conversation will keep flowing as usual. So if the bot is asking: 

- "What pizza toppings would you want?"

The notification could happen before the user even has a change to reply the original question, but the user will still be able to reply and continue the original conversation as usual

**startNewDialog**: In this case we want more than just send an ad hoc message: We want to interrupt the current conversation, initiate a new one and have the original conversation wait until the new one is finished. For example, let's say the user is asking for the quote of a given Stock, but an alert for another stock price arrives:

- "What's the current price of MSFT?"

- "Oh, sorry to interrupt but you asked me to warn you when GOOG reached the price of X. Do you want to sell it now?"

- "Yes"

- "Ok, sold"

- "The current price of MSFT is Y"

So in this case, the bot completely blocks the current dialogs, adds another one on top of the stack and gives control to it until that dialog says it's done. Also note the use of the dialog stack, which can be used for things such as resetting the current stack if we have no intention of coming back to the original conversation after the interruption.

**startNewDialogWithPrompt**: This sample is very similar to the previous one, with one difference: When the new dialog comes up and interrupts a conversation, it uses a prompt with a choice right away. This is just to demonstrate that this new dialog still can make use of common mechanisms such as promptDialog and can even call them right at the initiation (rather than the message received callback)

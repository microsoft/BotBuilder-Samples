# Managing state
See [here][1] for an overview of state management in BotBuilder V4 SDK. 

State within a bot follows the same paradigms as modern web applications, and the Bot Framework SDK provides some abstractions to make state management easier.

As with web apps, a bot is inherently stateless; a different instance of your bot may handle any given turn of the conversation. For some bots, this simplicity is preferred - the bot can either operate without additional information, or the information required is guaranteed to be within the incoming message. For others, state (such as where in the conversation we are or previously received data about the user) is necessary for the bot to have a useful conversation.

[Adaptive dialogs][2] provides a way to access and manage memory. All adaptive dialogs by default use this model so all components that consume or contribute to memory have a common way to access and write information to appropriate scopes. 

All memory properties, in all memory scopes, are property bags, meaning you can store arbitrary information on them.

See [here][3] for guidance on when to use each scope

## User scope
User scope is persistent data scoped to the id of the user you are conversing with.  
    
Examples:

    user.name
    user.address.city

## Conversation scope
Conversation scope is persistent data scoped to the id of the conversation you are having.  

Examples:

    conversation.hasAcceptedTOU
    conversation.dateStarted
    conversation.lastMaleReference
    conversation.lastFemaleReference
    conversation.lastLocationReference

## Dialog scope
Dialog scope is persistent data scoped for a giving executing dialog, providing an memory space for each dialog to have internal persistent bookkeeping. Dialog scope is cleared when dialog ends. 

Examples:

    dialog.orderStarted
    dialog.shoppingCart

### Dialog sub-scopes
Two interesting sub-scopes under dialog are 'result' and 'options'. Options scope by default carry parameters/ input to the specific dialog being executed. Result scope captures any values returned by a dialog. You can access these via dialog.result and dialog.options.

## Turn scope
Turn scope is *non-persistent* data scoped for *only the current turn*, providing a place to share data for the lifetime of the current turn.  

Examples:

    turn.DialogEvent
    turn.recognitionResult

### Turn sub-scopes
- Each incoming [activity][5] to the bot is available via turn.activity scope.
- For each turn of the conversation, if a [recognizer][4] is run, then the output intents and entities from that recognizer are automatically set and available under _turn.recognized.intents_ and _turn.recognized.entities_
- The payload of an event raised by the system (or via user code) is available under turn.dialogEvents.\<eventName\>.value scope.

## Settings scope
This represents any settings that are made available to the bot via the platform specific settings configuration system - e.g. appsettings.json, .env etc. 

## Memory short-hands
There are few short-hand notations supported to access specific memory scopes. 

| Symbol | Usage            | Expansion                           | Notes                                                                                                                 |
|--------|------------------|-------------------------------------|-----------------------------------------------------------------------------------------------------------------------|
| $      | $userName        | dialog.userName                     | Short hand notation that can be used to simply get or set a value on the dialog scope                                 |
| #      | #intentName      | turn.recognized.intents.intentName  | Short hand used to denote a named intent returned by the recognizer                                                   |
| @      | @entityName      | turn.recognized.entities.entityName | @entityName will always **only** return the first value found for the entity immaterial of the value's cardinality    |
| @@     | @@entityName     | turn.recognized.entities.entityName | @@entityName will return the actual value of the entity, preserving the value's cardinality                           | 
| ~      | ~propertyName    | dialog.instance.propertyName        | Used to refer to instance properties (e.g. minValue, maxValue etc)                                                    |
| %      | %optionsName     | dialog.options.optionsName          | Used to refer to options passed in to this dialog by the caller                                                       |

[1]:https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-state?view=azure-bot-service-4.0
[2]:../README.md
[3]:https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-state?view=azure-bot-service-4.0#when-to-use-each-type-of-state
[4]:./recognizers-rules-steps-reference.md#Recognizers
[5]:https://github.com/Microsoft/BotBuilder/blob/master/specs/botframework-activity/botframework-activity.md

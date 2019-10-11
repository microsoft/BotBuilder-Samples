# Interruption

Interruption is a technique available in Adaptive dialogs that enables the bot to understand and respond to user input that does not directly pertain to the specific piece of information the bot is prompting the user for. 

You can use interruption as a technique to
•	detect and handle user's response as a locally relevant intent within the scope of the active dialog (dialog that prompted user for information)
•	detect that a different dialog within the bot would be better suited to handle the specific user input and use the consultation mechanism baked into Adaptive dialogs to have a different dialog handle user input.

Here is an example conversation that demonstrate bot handling interruption locally within the context of the active dialog - 

<a name="local_interrupt"></a>
```markdown
user: hi
bot: hello, I'm the demo bot. What is your name? 

> User said something that's a locally relevant intent that should be handled within the context of the active dialog
user: why do you need my name?      
bot: I need your name to be able to address you correctly 

> Bot continues to prompt for the missing information
bot: what is your name? 
user: I'm not going to give you my name

> User said something that is a locally relevant intent that the bot handles locally. In this case, bot decides to set user name to 'Human' and move on and does not re-prompt
bot: No worries. I'll call you 'Human' for now. 
bot: You can always say 'My name is <your name> to reintroduce yourself to me'
bot: Hello Human, nice to meet you! 
```

Here's another example conversation that demonstrates bot handling user response locally within the active dialog

<a name="local_intent_handling"></a>

```markdown
user: hi
bot: Hello, I'm the demo bot. What is your name?
user: I'm Vishwac
bot: Hello Vishwac, nice to meet you!
```

Here is another example conversation that demonstrates bot handling interruption by consulting a different dialog that is better suited to respond - 

<a name="global_interrupt"></a>
```markdown
user: book a flight
bot: Sure. Happy to help with that. What is your destination city? 
user: New york
bot: Got it. I have New york as your destination city. 
bot: What is your departure city? 
> User said something that the current dialog cannot handle. Via interruption technique, bot can handle this using the 'weather' dialog that is better suited to respond.
user: How's the weather there next thursday? 

> Weather dialog responds but carries forward context from prior conversation
bot: Its forecast to be 72' and sunny in New York for next thursday.

> Bot resumes the previous converstaion
bot: Let's continue booking your flight to New york

> Bot continues to re-prompt for missing information
bot: What is your departure city? 
user: ...
```

Interruption as a concept is **only** available within `input` actions - e.g. TextInput, NumberInput, ChoiceInput etc. 

Interruption is controlled via the `AllowInterruption` property on input actions. 

## Allow interruption property

There are three possible configurations for `AllowInterruption` property 
- Never: The active input action will always consume user input. No interruptions are possible while this input is being executed. 
- Always: The active input action will always consult up its parent dialog chain to see if thers is a better handler for the given user input. If no one up the parent dialog chain consumes the user input, the current active input action will take it. 
- NotRecognized: Input actions have their own internal recognizer that is looking for a specific type of information. E.g. NumberInput is looking for a number response from user etc. Setting this property will give the current active input action a chance to recognize against user input and will consult up the parent dialog chain if its own recognizer does not find what it is looking for.

## Enabling interruptions
You can set `AllowInterruption` to either `Always` or `NotRecognized`. Here are some additional configuration considerations you might want to add to successfully handle interruption. 

### For AllowInterruption = Always or AllowInterruption = NotRecognized
With this mode, you are explicitly consulting up your parent dialog chain on every turn of user response to the active input action. With this setting,  
1. Consider adding an intent event with associated steps to catch locally relevant things that your bot should handle within the context of the active dialog that holds the input action. 
    - In the example listed [here](#local_interrupt), this would be things like user saying 'why do you need my name' as well as 'I will not give you my name' that needs to be handled locally within the active dialog. 
2. Consider adding an intent event with associated steps to catch user responding to input actions in the active dialog. 
    - In this intent event action, use SetProperty (if required) to save any entities that were recognized by the recognizer. 
    - Consider adding LUIS entity types - e.g. prebuilt LUIS entity types like personName, age, number, dateTime etc or ClosedList entity type (if you are using choice input action) so the parent dialog's recognizer can appropriately detect entities that pull values your input action is looking for from the user utterance. 
    - In the example listed [here](#local_intent_handling), this would cover things like user saying 'I'm vishwac'. In this case, you can use the recognizer to pull 'vishwac' as the user name instead of verbatim taking the entirity of user's response as their name which would be incorrect.
3. Consider adding an 'interruption' intent that includes examples of things users can say that would be better served by a different dialog within your bot handling that user input. **Do not** add an intent action in that specific dialog to handle this. By not doing this, your dialog is signalling that consultation should move up to the next level up the parent dialog chain. 
    - In the example listed [here](#global_interrupt), this would cover things like user saying 'How is the weather there next thursday' which would be better served by the `weather` dialog handling it rather than it being considered as a valid response to the bot asking for departure city.

**Note:** AllowInteruption = NotRecognized mode does not help in cases where intents you would like to fulfill within the child dialog or up the parent dialog chain expects values of the specific type the active input action is prompting user for.

## Interruption behavior
With `AllowInterruption = Always` or `AllowInterruption = NotRecognized`, the active input action will initiate consultation up its parent dialog chain starting with the immediate parent dialog that contains this active input action. 

Consultation **stops** when an intent event up the parent dialog chain (including the immediate) parent fires. 

The active input will assume the user utterance to be consumed if there are any actions that are excuted in the consultaton bubble. 

Here's an example: 

```markdown
user: hi
bot: hello, what is your name?
user: why do you need my name? 
> bot comes back with a response here via a SendActivity action inside a 'whyName' intent event handler in the parent dialog that contains this active input step. 
> this stops consultation bubbling. 
bot: I need your name to be able to address you correctly. 
> The user input `why do you need my name?` is considered consumed and the active input step issues a re-prompt
bot: what is your name? 
user: ...
```

In some cases, you might want to have more explicit control of when the user input is considered consumed. To achieve this, you can use the `SetProperty` action and set property `turn.processInput` to value `true` to signal the active input action should continue to process user uttearnces and not consider it consumed. 

Here is an example: 
```markdown
user: hi
bot: hello, what is your name? 
user: vishwac
> Assuming the input action that is asking the user for their name has AllowInterruption = Always or AllowInterruption = NotRecognized, the parent dialog's recognizer might come back with a 'None' intent. In the intent event action for 'None' intent, you might want to perform some other actions (e.g. send a trace, log something, make a HTTP call etc) and subsequently have the input re-evaluate user utterance. 
> To do this, you would add a intent event for 'None', add the actions you would like to have your bot perform and use SetProperty action to set turn.processInput = true. 
> This will cause the active input action to continue to use the user utterance 'vishwac' and process it using its own recognizer. 
bot: Hello vishwac, nice to meet you. 
```

You can checkout the `InterruptionSample` to see these concepts in action. 
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

Allow interruption property supports an expression. Expressions are defined using [Common Expression Language][1]. 

## Enabling interruptions

`AllowInterruptions` defaults to `true`. To turn off interruptions for a specific input, simply set `AllowInterruption = false`.

- To allow interruptions only when a specific intent is detected `AllowInterruptions = #Interruption_intent`
- To allow interruptions for all intents except a specific intent `AllowInterruptions = !#Input_intent`
- To allow interruptions only if specific entities are not detected `AllowInterruption = !@entity1 || !@entity2`
- To allow interruptions only if we have a high confidence intent recogition `AllowInterruption = #Interruption_intent && #Interruption_intent.Score >= 0.8`


## Interruption behavior
When the `AllowInterruptions` property evaluates to `true`, the active input action will initiate consultation up its parent dialog chain starting with the immediate parent dialog that contains this active input action. 

Consultation **stops** when an intent event up the parent dialog chain (including the immediate) parent fires. 

The active input will assume the user utterance to be consumed if there are any actions that are excuted in the consultaton bubble path. When this happens, the active input will always issue a `re-prompt` and not try to re-process the activity for that turn of the conversaiton. To override this, you can set `turn.activityProcessed = false` in the interruption handler to have the active input re-recognize the activity for that turn of the conversation.

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

In some cases, you might want to have more explicit control of when the user input is considered consumed. To achieve this, you can use the `SetProperty` action and set property `turn.activityProcessed` to value `false` to signal the active input action should continue to process user uttearnces and not consider it consumed. 

Here is an example: 
```markdown
user: hi
bot: hello, what is your name? 
user: vishwac
> Assuming the input action that is asking the user for their name has AllowInterruption = true, the parent dialog's recognizer might come back with a 'None' intent. In the intent event action for 'None' intent, you might want to perform some other actions (e.g. send a trace, log something, make a HTTP call etc) and subsequently have the input re-evaluate user utterance. 
> To do this, you would add a intent event for 'None', add the actions you would like to have your bot perform and use SetProperty action to set turn.activityProcessed = false. 
> This will cause the active input action to continue to use the user utterance 'vishwac' and process it using its own recognizer. 
bot: Hello vishwac, nice to meet you. 
```

[1]:../../common-expression-language/README.md
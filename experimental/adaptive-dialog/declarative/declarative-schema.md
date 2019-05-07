
## Rules
_Rules_ enable you to catch and respond to events. The broadest rule is the EventRule that allows you to catch and attach a set of steps to execute when a specific event is emitted by any sub-system. Adaptive dialogs supports couple of other specialized rules to wrap common events that your bot would handle. There are a number of built in rules and you can easily create new rules.

### Microsoft.EventRule rule
The **Microsoft.EventRule** rule is used to trigger based on events.

| Property   | Description                                                                 |
|------------|-----------------------------------------------------------------------------|
| events     | array of events to trigger on                                               |
| steps      | collection of dialogs/dialog steps to add to the plan if conditions are met |
| constraint | additional contraint to evalute on top of the event                         |

Example
```json
{
    "$type":"Microsoft.EventRule",
    "events": ["ActivityReceived"],
    "constraint":"user.age > 18",
    "steps": [
        ...
    ]
}
```
#### Events
You can use the EventRule to listen to and act on any of these events.  

| Event               | Description                                       |
|---------------------|---------------------------------------------------|
| BeginDialog         | Fired when a dialog is start                      |
| ActivityReceived    | Fired when a new activity comes in                |
| RecognizedIntent    | Fired when an intent is recognized                |
| UnknownIntent       | Fired when an intent is not handled or recognized |
| StepsStarted         | Fired when a plan is started                      |
| StepsSaved           | Fires when a plan is saved                        |
| StepsEnded           | Fires when a plan successful ends                 |
| StepsResumed         | Fires when a plan is resumed from an interruption |
| ConsultDialog       | fired when consulting                             |
| CancelDialog       | fired when dialog canceled                             |


### Microsoft.IntentRule
The **Microsoft.IntentRule** rule is triggered if an intent and/or entities are  recognized.

| Property   | Description                                                                        |
|------------|------------------------------------------------------------------------------------|
| intent     | intent which should be recognized                                                  |
| entities   | array of names of entities which must be recognized                                |
| constraint | additional expression as a constraint expressed against memory (OPTIONAL)          |
| steps      | a dialogId or inline Dialog or array of dialogIds/dialogs/steps to add to the plan |


Example for intent rule 
```json
{
    "$type": "Microsoft.IntentRule",
    "intent": "AddItem",
    "steps": [
        "ToDoLuisBot.AddItem"
    ]
}
```

Example fo intent rule and setting entities (entities from recognizer)
```json
{
    "$type":"Microsoft.IntentRule",
    "intent":"MyIntent",
    "entities": [ "user", "userName_patternAny"]
}
```

Example for  IntentRule with only contraints 
```json
{
    "$type":"Microsoft.IntentRule",
    "intent":"MyIntent",
     "constraint":"turn.DialogEvent.Value.Entities.UserName != null || turn.DialogEvent.Value.Entities.UserName_patternAny != null"
}
```
example for intenet rule with entieid and constrains
```json
{
    "$type":"Microsoft.IntentRule",
    "intent":"MyIntent",
    "entities": [ "entityx", "entity"],
     "constraint":"turn.DialogEvent.Value.Entities.UserName != null || turn.DialogEvent.Value.Entities.UserName_patternAny != null"
}
```

### Microsoft.UnKnownIntentRule rule
Use this rule to catch and respond to a case when a 'recognizedIntent' event was not caught and handled by any of the other rules. This is especially helpful to capture and handle cases where your dialog wishes to participate in consultation.


| Property   | Description                                                                 |
|------------|-----------------------------------------------------------------------------|
| steps      | collection of dialogs/dialog steps to add to the plan if conditions are met |

Example
```json
{
    "$type": "Microsoft.UnknownIntentRule",
    "steps": [
        {
            "$type": "Microsoft.SendActivity",
            "activity": "To use thie bot..."
        }
    ]
}
```

## Recognizers
Recognizers are components which inspect input and generates intents and entities as output.

### Microsoft.LuisRecognizer
The **Microsoft.LuisRecognizer** is a component for using luis.ai service to generate intents 
and entities.

| Property                  | Description        |
|---------------------------|--------------------|
| applicationId | Application ID     |
| endpoint      | Endpoint to use    |
| endpointKey   | endpointKey to use |

Example:
```json
{
    "$type":"Microsoft.LuisRecognizer",
    "applicationId":"12312313123",
    "endpoint":"http://../",
    "endpointKey":"123123123123"
}
```

### Microsoft.RegexRecognizer
The **Microsoft.RegexRecognizer** is a component for using regular expressions to generate 
intents and entities.

| property | description                        |
|----------|------------------------------------|
| intents  | Map of intentName -> regex pattern |

Example:
```json
{
    "$type":"Microsoft.RegexRecognizer",
    "intents": {
        "Greeting":"/greeting/",
        "TellMeAjoke":".*joke.*",
        "Help":"/help/",
    }
}
```

# Input/Prompt Dialogs
Dialogs are used to model interactions with the user. Input/Prompt dialogs are dialogs which gather typed information with simple validation. All dialogs input are part *Microsoft.Bot.Builder.Dialogs.Adaptive.Input*

## Microsoft.TextInput Dialog
The **Microsoft.TextInput** dialog is used to gather text input from user, with validation option and matches pattern.

| Property        | Description                                                                              |
|-----------------|------------------------------------------------------------------------------------------|
| property        | the property this input dialog is bound to and the dialog will set at end of dialog      |
| pattern         | and optional regex to validate input                                                     |
| Prompt          | a response, LG use is optional, sent to user to start the prompt                         |
| retryPrompt     | a response, LG use is optional, sent to prompt the user to try again                     |
| noMatchResponse | a response, LG use is optional, sent to tell the user the value didn't match the pattern |

Example:
```json
{
    "$type":"Microsoft.TextInput",
    "property": "user.name",
    "pattern":"[0-9a-zA-Z].*",
    "Prompt":"What is your name?",
    "retryPrompt":"Let's try again, what is your name?",
    "noMatchResponse":"That response didn't match the pattern"
}
```


## Microsoft.NumberInput Dialog
The **Microsoft.IntegerInput** dialog is used to gather a integer input from the user.

| Property         | Description                                                                            |
|------------------|----------------------------------------------------------------------------------------|
| property         | the property this input dialog is bound to                                             |
| minValue         | The min value which is valid                                                           |
| maxValue         | The max value which is valid                                                           |
| prompt           | the LG response  to start the prompt                                                   |
| retryPrompt      | the LG response  to prompt the user to try again                                       |
| invalidPrompt    | a response, LG use is optional, sent to tell the user the when value is not a number   |
| Precision        | Defaults to 0. Denotes the precision digits to capture. e.g. Precision: 2 would capture values up to 2 decimal places |

Example:
```json
{
    "$type":"Microsoft.IntegerInput",
    "property": "user.age",
    "minValue": 0,
    "maxValue": 120,
    "prompt":"What is your age?",
    "retryPrompt":"Let's try again, what is your age?",
    "invalidPrompt":"I didn't recognize an age."
}
```

## Microsoft.ChoiceInput
The **Microsoft.ChoiceInput** represents a dialog which gathers a choice responses

| Property         | Description                                                                        |
|------------------|------------------------------------------------------------------------------------|
| property         | the property this input dialog is bound to                                         |
| prompt           | the LG response  to start the prompt                                               |
| retryPrompt      | the LG response  to prompt the user to try again                                   |
| invalidPrompt    | a response, LG use is optional, sent to tell the user when then input was not recognized or not valid for the input typ     |
| style            | the kind of choice list style to generate: Inline; List; SugestedActions; HeroCard |
| choices          | array representing possible values sent to the user                                |

Example:
```json
{
      "$type": "Microsoft.ChoiceInput",
      "property": "user.pizzaSize",
      "choices": [
        {
          "value": "Small"
        },
        {
          "value": "Medium"
        },
        {
          "value": "Large"
        }
      ],
      "prompt": "What size of pizza you want?",
      "retryPrompt": ""
      "style": "List",
      "alwaysPrompt": true
    },
```


## Microsoft.ConfirmInput
The **Microsoft.ConfirmInput** This represents a dialog which gathers a yes/no style responses

| Property         | Description                                                                        |
|------------------|------------------------------------------------------------------------------------|
| property         | the property this input dialog is bound to                                         |
| prompt           | the LG response  to start the prompt                                               |
| retryPrompt      | the LG response  to prompt the user to try again                                   |
| invalidPrompt    | a response, LG use is optional, sent to tell the user when then input was not recognized or not valid for the input typ     |

Example:
```json
{
      "$type": "Microsoft.ConfirmInput",
      "property": "user.confirmed",
      "prompt": "Are you sure?",
      "retryPrompt": "let's try again, are you sure?",
      "invalidPrompt": "I didn't recognize a yes/no response",
      "alwaysPrompt": true
    },
```


# Dialog Steps
Dialog Steps are special dialog primitives which are used to control the flow of the conversation. Steps help put together the flow of conversation when a specific event is captured via a Rule. Note: unlike Waterfall dialog where each step is a function, each step in an Adaptive dialog is in itself. This enables Adaptive dialogs by design to have a much cleaner ability to handle and deal with interruptions. Adaptive dialogs support the following 

## sending a response
### Microsoft.SendActivity Step
The **Microsoft.SendActivity** step gives you the ability to send an activity to the user.  The activity can be a string, an inline LG template, or an actual Activity definition. 

| Property          | Description                            |
|-------------------|----------------------------------------|
| activity (string) | an inline LG template for the activity |
| activity (object) | an Activity object definition          |

Example with LG template inline 
```json
{
    "$type":"Microsoft.SendActivity",
    "activity":"Hi {user.name}. How are you?"
}
```

Example with activity, using the special typing indicator   
```json
{
    "$type":"Microsoft.SendActivity",
    "activity": 
    {
        "type":"typing"
    }
}
```

Example with Language Generation template. You can use Language Generation to send cards. See [here][100] to learn more about Language Generation. 
```json
{
    "$type":"Microsoft.SendActivity",
    "activity":"[cardTemplate]"
}
```
## Tracing and logging
Set of declarative steps used to emit Bot Framework traces that get routed as a transcript and logging for the bot. 

### Microsoft.TraceActivity Step
The **Microsoft.TraceActivity** step sends an TraceActivity to the transcript. 

| Property                | Description                            |
|-------------------------|----------------------------------------|
| name (string)           | name of the trace activity     |
| valueType (string)      | Value type of the trace activity          |
| valueProperty (string)  | Property path to memory object to send as the value of the trace activity |

Example 
```json
{
    "$type":"Microsoft.TraceActivity",
    "name" : "Trace1",
    "valyeType":"this is the actual value to be trace",
    "valueProperty" : "user.name"
}
```

## Memory manipulation 
Set of declarative steps used to declaratively manipulate bot's memory

### Microsoft.SaveEntity Step
The **Microsoft.SaveEntity** saves a memory property as an entity - used to extract an entity returned by recognizer into memory. 

| Property                | Description                                      |
|-------------------------|--------------------------------------------------|
| entity (string)         | name of the entity to save activity              |
| property (string)       | name of the property in memory to save th entity |

Example 
```json
{
    "$type":"Microsoft.SaveEntity",
    "entity" : "ToCity",
    "property" : "user.destination"
}
```

### Microsoft.EditArray Step
The **Microsoft.EditArray** modifies an array in memory. 

| Property                | Description                                                     |
|-------------------------|-----------------------------------------------------------------|
| ChangeType              | the array operation to perform: Push; Pop; Take; Remove; Clear  |
| arrayProperty           | the name of the (target) aray in memory to manipulat            |
| itemProperty            | Memory expression for the item use to update the array          |

Example 
```json
    {
        "$type": "Microsoft.EditArray",
        "changeType": "Push",
        "arrayProperty": "user.lists.{dialog.listName}",
        "itemProperty": "dialog.item"
    },
```

### Microsoft.InitProperty Step
The **Microsoft.InitProperty** innitial a property to either an object or array. 

| Property                | Description                                                     |
|-------------------------|-----------------------------------------------------------------|
| property (string)       | the memroy property to set   |
| type (string)           | type of value to set the property to, object or array            |

Example 
```json
    {
        "$type": "Microsoft.InitProperty",
        "property": "user.lists",
        "type": "array",
    },
```

### Microsoft.SetProperty Step
The **Microsoft.SetProperty** sets memory to the value of an expression. 

| Property                | Description                                                     |
|-------------------------|-----------------------------------------------------------------|
| property                | the memroy property to set   |
| value                   | type of value to set the property             |

Example 
```json
    {
        "$type": "Microsoft.SetProperty",
        "property": "user.age",
        "type": "-1",
    },
```

### Microsoft.DeleteProperty Step
The **Microsoft.DeleteProperty**  removes a property from memory. 

| Property                | Description                                                     |
|-------------------------|-----------------------------------------------------------------|
| property                | the memroy property to remove   |

Example 
```json
    {
        "$type": "Microsoft.DeleteProperty",
        "property": "user.age",
    },
```

## Conversational flow and dialog management
Set of steps to control the flow of a given set of steps, within a dialog, and steps to control a dialog.

### Microsoft.IfCondition Step
The **Microsoft.IfCondition** step gives you the ability to inspect memory and **branch** between dialogs.

| Property   | Description                                                                       |
|------------|-----------------------------------------------------------------------------------|
| condition  | a expression to evaluate                                                          |
| steps      | a dialogId, dialog, or array of dialogs (steps) to execute if expression is true |
| elseSteps  | a dialogId, dialog, or array of dialogs (steps) to execute if expression is true |

Example with LG template inline 
```json
{
    "$type": "Microsoft.IfCondition",
    "condition": "user.age < 18 ",
    "steps": [
        "UnderAgeDialog"
    ],
    "elseSteps" : [
        "UnderAgeDialog"
    ]
}
```

### Microsoft.SwitchCondition Step
The **Microsoft.SwitchCondition** step  conditionally decides which step to execute next.

| Property   | Description                                                                       |
|------------|-----------------------------------------------------------------------------------|
| condition  | a expression to evaluate                                                          |
| cases      | aray defining the set of steps to execute per  case                               |
| default    | set of steps to execute for the default case                                      |

example for a swtich condition on type of helps
```json
    {
      "$type": "Microsoft.SwitchCondition",
      "condition": "user.helpType",
      "cases": {
       "Customer Support": [
        {
         "$type": "Microsoft.SendActivity",
         "activity": "Case Customer Support"
        }
       ],
       "ecommarce": [
        {
         "$type": "Microsoft.SendActivity",
         "activity": "Case  2"
        }
       ],
       "Other": [
        {
          "steps": [
          {
            "$type": "Microsoft.CallDialog",
            "dialog": "ecommarceDialog",
            "options": {
             "a": "a"
            },
            "property": "dialog.result"
           }
          ]
      }
    ],
     "defult": [
       {
        "$type": "Microsoft.SendActivity",
        "activity": "Default"
       }
      ]
     }
```

### Microsoft.BeginDialog Step
The **Microsoft.BeginDialog** begins another dialog. When the new dialog completes, it will return to the caller. 
You can bind peopertyies to dialog, which the dialog will set before returning to the caller.   

> NOTE: When the called dialog is completed, the caller will continue.

| Property        | Description                                                     |
|-----------------|-----------------------------------------------------------------|
| property        | the property in memory to store the result of the called dialog |
| dialog (string) | the id (string) of a dialog to be called                        |
| dialog (object) | an inline dialog definition to be called                        |

Example BeginDialog using dialogid
```json
{
    "$type":"Microsoft.BeginDialog",
    "property": "user.address",
    "dialog": "GetAddressDialog"
}
```
Example BeginDialog using inline dialog definition
```json
{
    "$type":"Microsoft.BeginDialog",
    "property": "user.cat.name",
    "dialog": {
        "$type":"Microsoft.TextPrompt",
        "initialPrompt":"What is your cat's name?"
    }
}
```

### Microsoft.EndDialog Step
The **Microsoft.EndDialog** step gives you the ability to **end** the calling dialog returning a result to caller.  

> NOTE: EndDialog ends the *caller* and returns a value

| Property | Description                                                                     |
|----------|---------------------------------------------------------------------------------|
| property   | an expression against memory which will be returned as the result to the caller |

Example returning the dialog's .address property as the result of the dialog to the caller.
```json
{
    "$type":"Microsoft.EndDialog",
    "property":"dialog.address"
}
```

### Microsoft.CancelAllDialogs Step
The **Microsoft.CancelAllDialogs** step cancels all of the current dialogs by emitting an event that propogate through the dialog stack. This event can be caught (at each dialog) to prevent cancelation from propagating.  

| Property | Description                                                                     |
|----------|---------------------------------------------------------------------------------|
| eventName   | Name of the event which is emitted |
| eventValue   | object value which is a payload to communicate to the parent dialogs so that they can decide how to process it |



### Microsoft.ReplaceDialog Step
The **Microsoft.ReplaceDialog** step gives you the ability to **replace** the current dialog
with another dialog and bind it's result to memory.  Some dialogs don't have data binding built in, and GotoDialog gives you the 
ability to do that. 

> NOTE: When the called dialog is completed, the caller will **NOT** continue as it has been replaced.

| Property        | Description                                                     |
|-----------------|-----------------------------------------------------------------|
| dialog (string) | the id (string) of a dialog to be called                        |
| options (object) | Options to pass to the dialog                        |
| property (string) |The property to bind to the dialog and store the result in |

Example CallDialog using dialogid
```json
{
    "$type":"Microsoft.ReplaceDialog",
    "property": "user.address",
    "dialog": "GetAddressDialog"
}
```

## Custom code and HTTP request
Even when using declarative form of dialogs, you can use custom code and make HTTP calls. 

See CustomStep.dialog in Sample #13

## Microsoft.HttpRequest Step
The **Microsoft.HttoRequest** step makes an http call

| Property        | Description                                                     |
|-----------------|-----------------------------------------------------------------|
| method (string) | the HTTP method for the http call: GET; POST                    |
| url (string) | The url to call (supports data binding)                    |
| Body  (object) | The body to send in the HTTP call  (supports data binding)                        |
| header  (object) | Http headers to include with the HTTP request (supports data binding)t                        |
| responseTypes (object) | Describes how to parse the response from the http request. If Activity or Activities, then the they will be sent to the user. |
| property (string) | The property to store the result of the HTTP call in (as object or string) |

Example CallDialog using dialogid
```json
{
    "$type": "Microsoft.HttpRequest",
    "url": "http://petstore.swagger.io/v2/pet",
    "method": "POST",
    "header": {
        "test": "test",
        "test2": "test2"
    },
    "body": {
        "id": "{dialog.petid}",
        "category": {
            "id": 0,
            "name": "string"
        },
        "name": "{dialog.petname}",
        "photoUrls": [ "string" ],
        "tags": [
            {
                "id": 0,
                "name": "string"
            }
        ],
        "status": "available"
    },
    "property": "dialog.postResponse"
},
```
[100]:../../language-generation
# Adaptive dialog: Recognizers, Generators, Triggers, Actions and Inputs

This document describes the constituent parts of [Adaptive][7] dialog. 

- [Recognizers](#Recognizers)
- [Generator](#Generator)
- [Triggers](#Triggers)
- [Actions](#Actions)
- [Inputs](#Inputs)

## Recognizers
_Recognizers_ provide the functionality of understanding and extracting meaningful pieces of information from a user's input. All recognizers emit events - of specific interest is the 'recognizedIntent' event that fires when the recognizer picks up an intent (or extracts entities) from a given user utterance.

Adaptive Dialogs support the following recognizers - 
- [RegEx recognizer](#RegEx-recognizer)
- [LUIS recognizer](#LUIS-recognizer)
- [Multi-language recogizer](#Multi-language-recognizer)
- [CrossTrainedRecognizerSet](#cross-trained-recognizer-set)
- [RecognizerSet](#recognizer-set)
- [ValueRecognizer](#value-recognizer)
- [QnA recognizer](#qna-maker-recognizer)

### RegEx recognizer
RegEx recognizer enables you to extract intent and entities based on regular expression patterns. 

``` C#
var rootDialog = new AdaptiveDialog("rootDialog")
{
    Recognizer = new RegexRecognizer()
    {
        Intents = new List<IntentPattern>()
        {
            new IntentPattern()
            { 
                Intent = "AddIntent", 
                Pattern = "(?i)(?:add|create) .*(?:to-do|todo|task)(?: )?(?:named (?<title>.*))?"
            },
            new IntentPattern()
            { 
                Intent = "HelpIntent", 
                Pattern = "(?i)help" 
            },
            new IntentPattern()
            { 
                Intent = "CancelIntent", 
                Pattern = "(?i)cancel|never mind" 
            }
        },
        Entities = new List<EntityRecognizer>()
        {
            new ConfirmationEntityRecognizer(),
            new DateTimeEntityRecognizer(),
            new NumberEntityRecognizer()
        }
    }
}
```

**Note:** RegEx recognizer will emit a 'None' intent when the input utterance does not match any defined intent.
**Note:** RegEx recognizer is useful for testing and quick prototyping. For creating sophisticated conversations and production bots we recommend using a LUIS recognizer.

### LUIS recognizer
[LUIS.ai][1] is a machine learning-based service to build natural language into apps, bots, and IoT devices. Using a LUIS recognizer enables you to extract intents and entities based on a LUIS application, which you can train in advance. 

``` C#
var rootDialog = new AdaptiveDialog("rootDialog")
{
    Recognizer = new LuisAdaptiveRecognizer() 
    {
        ApplicationId = "<LUIS-APP-ID>",
        EndpointKey = "<ENDPOINT-KEY>",
        Endpoint = "<ENDPOINT-URI>"
    }
}
```
# QnA Maker Recognizer
[QnAMaker.ai](https://qnamaker.ai) is one of the cognitive services that enables you to create rich question-answer pairs from existing content - documents, urls, pdfs etc. You can use the QnA Maker recognizer to integrate with the service. 

```C#
var adaptiveDialog = new AdaptiveDialog()
{
    Recognizer = new QnAMakerRecognizer()
    {
        KnowledgeBaseId = "<KBID>",
        HostName = "<HostName>",
        EndpointKey = "<Key>"
    }
};
```

**Note:** This recognizer will return `QnAMatch` intent with the top answer promoted as an entity as well as the entire qna maker response available under `answer` property in the recognition result.

# Value recognizer

When using post back option with [adaptive cards](https://adaptivecards.io), the bot recieves an activity with a value property set to the actual payload returned by the card. Value recognizer looks at the `value` property in an activity and transforms it to a recognition result that includes `intent` and `entities`. 

Given this adaptive card json, with the value recognizer, you can simply handle user clicking on submit button like any other intent.

```json
{
    "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
    "type": "AdaptiveCard",
    "version": "1.0",
    "body": [
        {
            "type": "TextBlock",
            "size": "Medium",
            "weight": "Bolder",
            "text": " Info Form",
            "horizontalAlignment": "Center"
        },
        {
            "type": "Input.Text",
            "placeholder": "Name",
            "style": "text",
            "maxLength": 0,
            "id": "SimpleVal"
        },
        {
            "type": "Input.Text",
            "placeholder": "Homepage",
            "style": "Url",
            "maxLength": 0,
            "id": "UrlVal"
        },
        {
            "type": "Input.Text",
            "placeholder": "Email",
            "style": "Email",
            "maxLength": 0,
            "id": "EmailVal"
        }
    ],
    "actions": [
        {
            "type": "Action.Submit",
            "title": "Submit",
            "data": {
                "id": "1234567890",
                "intent": "userProfile-card"
            }
        }
    ]
}
```

```C#
var adaptiveDialog = new AdaptiveDialog()
{
    Recognizer = new ValueRecognizer(),
    Triggers = new List<OnCondition>()
    {
        new OnIntent()
        {
            Intent = "userProfile-card",
            Actions = new List<Dialog>()
            {
                // All properties posted back by adaptive card are transformed to entiies.
                // You can refer to entities using the @entityName shortcut or the long form turn.recognized.entities.entityName
                new SendActivity("I have ID: ${@id} \n Name : ${@SimpleVal} \n Homepage: ${@UrlVal} \n Email : ${@EmailVal}")
            }
        }
    }
};
```


### Multi-language recognizer
When building a sophisticated multi-lingual bot, you will typically have one recognizer tied to a specific language x locale. The Multi-language recognizer enables you to easily specify the recognizer to use based on the [locale][2] property on the incoming activity from a user. 

``` C#
var rootDialog = new AdaptiveDialog("rootDialog")
{
    Recognizer = new MultiLanguageRecognizer()
    {
        Recognizers = new Dictionary<string, Recognizer>()
        {
            {
                "en",
                new RegexRecognizer()
                {
                    Intents = new List<IntentPattern>()
                    {
                        new IntentPattern()
                        {
                            Intent = "AddIntent",
                            Pattern = "(?i)(?:add|create) .*(?:to-do|todo|task)(?: )?(?:named (?<title>.*))?"
                        },
                        new IntentPattern()
                        {
                            Intent = "HelpIntent",
                            Pattern = "(?i)help"
                        },
                        new IntentPattern()
                        {
                            Intent = "CancelIntent",
                            Pattern = "(?i)cancel|never mind"
                        }
                    },
                    Entities = new List<EntityRecognizer>()
                    {
                        new ConfirmationEntityRecognizer(),
                        new DateTimeEntityRecognizer(),
                        new NumberEntityRecognizer()
                    }
                }
            },
            {
                "fr",
                new LuisAdaptiveRecognizer()
                {
                    ApplicationId = "<LUIS-APP-ID>",
                    EndpointKey = "<ENDPOINT-KEY>",
                    Endpoint = "<ENDPOINT-URI>"
                }
            }
        }
    }
};
```

# Recognizer set

Sometimes you might need to run more than one recognizer on every turn of the conversation. Recognizer set does exactly that. All recognizer are run on each turn of the conversation and the result is a union of all recognition results. 

```C#
var adaptiveDialog = new AdaptiveDialog()
{
    Recognizer = new RecognizerSet()
    {
        Recognizers = new List<Recognizer>()
        {
            new ValueRecognizer(),
            new QnAMakerRecognizer()
            {
                KnowledgeBaseId = "<KBID>",
                HostName = "<HostName>",
                EndpointKey = "<Key>"
            }
        }
    }
};
```

# Cross trained recognizer set

Cross trained recognizer set compares recognition results from more than one recognizer to decide a winner. Given a collection of recognizers, cross-trained recognizer will 
    - Promote the recognition result of one of the recognizer if all other recognizers defer recognition to a single recognizer. To defer recognition, a recognizer can return `None` intent or an explicit `DeferToRecognizer_recognizerId` as intent.
    - Returns an `OnChooseIntent` intent which denotes confusability among recognizers. Each recognizer's results are returned via `turn.recognized.candidates`. This enables you to then use rules or other disambiguation techniques to decide an actual winner.

```C#
var adaptiveDialog = new AdaptiveDialog()
{
    Recognizer = new CrossTrainedRecognizerSet()
    {
        Recognizers = new List<Recognizer>()
        {
            new LuisAdaptiveRecognizer()
            {
                Id = "Luis-main-dialog",
                ApplicationId = "<LUIS-APP-ID>",
                EndpointKey = "<ENDPOINT-KEY>",
                Endpoint = "<ENDPOINT-URI>"
            },
            new QnAMakerRecognizer()
            {
                Id = "qna-main-dialog",
                KnowledgeBaseId = "<KBID>",
                HostName = "<HostName>",
                EndpointKey = "<Key>"
            }
        }
    }
};
``` 

## Generator
_Generator_ ties a specific language generation system to an Adaptive Dialog. This, along with Recognizer enables clean separation and encapsulation of a specific dialog's language understanding and language generation assets. With the [Language Generation][10] PREVIEW feature, you can set the generator to a _.lg_ file or set the generator to a [TemplateEngine][11] instance where you explicitly manage the one or more _.lg_ files that power this specific adaptive dialog. 

```C#
var myDialog = new AdaptiveDialog(nameof(AdaptiveDialog)) 
{
    Generator = new TemplateEngineLanguageGenerator(new TemplateEngine().AddFile(Path.Combine(".", "myDialog.lg")))
};
```

## Triggers
_Triggers_ enable you to catch and respond to events. The broadest trigger is the OnEvent that allows you to catch and attach a set of steps to execute when a specific event is emitted by any sub-system. Adaptive dialogs support a couple of other specialized rules to wrap common events that your bot would handle.

See table below for all triggers and their base events supported by Adaptive dialogs. 

<a name="events"></a>

| Trigger Name                 | Description                          | Base event       | Constraint                             |
| ---------------------------- | ------------------------------------ | ---------------- | -------------------------------------- |
| **_Base trigger_**           |                                      |                  |                                        |
| OnCondition                  | Base trigger                         | N/A              |                                        |
| **_Recognizer events_**      |                                      |                  |                                        |
| OnIntent                     | Intent event handler                 | RecognizedIntent | turn.recognized.intent == 'IntentName' |
| OnUnknownIntent              | Unknown intent event handler         | UnknownIntent    |                                        |
| OnQnAMatch | QnA Match intent. Only available if QnA recognizer is configured on the dialog | RecognizedIntent | turn.recognized.intent == 'QnAMatch' |
| OnChooseIntent | Only available if a cross trained recognizer is configured on the dialog. | RecognizedIntent | turn.recognized.intent == 'ChooseIntent' |
| **_Dialog events_**          |                                      |                  |                                        |
| OnBeginDialog                | Begin dialog handler                 | BeginDialog      | N/A                                    |
| OnRepromptDialog             | On reprompt                          | RepromptDialog   | N/A                                    |
| OnCancelDialog               | On dialog cancelled                  | CancelDialog     | N/A                                    |
| OnError                      | On error event                       | Error            | N/A                                    |
| OnCustomEvent                | On custom event raised via EmitEvent | N/A              | N/A                                    |
| **_Activity events_**        |                                      |                  |                                        |
| OnActivity                   | Generic activity handler             | ActivityRecieved | N/A                                    |
| OnConversationUpdateActivity | Conversation update activity handler | ActivityRecieved | ActivityTypes.ConversationUpdate       |
| OnEndOfConversationActivity  | End of conversation activity handler | ActivityRecieved | ActivityTypes.EndOfConversation        |
| OnEventActivity              | Event activity handler               | ActivityRecieved | ActivityTypes.Event                    |
| OnHandoffActivity            | Hand off activity handler            | ActivityRecieved | ActivityTypes.Handoff                  |
| OnInvokeActivity             | Invoke activity handler              | ActivityRecieved | ActivityTypes.Invoke                   |
| OnMessageActivity            | Message activity handler             | ActivityRecieved | ActivityTypes.Message                  |
| OnMessageDeletionActivity    | Message deletion activity handler    | ActivityRecieved | ActivityTypes.MessageDeletion          |
| OnMessageReactionActivity    | Message reaction activity handler    | ActivityRecieved | ActivityTypes.MessageReaction          |
| OnMessageUpdateActivity      | Message update activity handler      | ActivityRecieved | ActivityTypes.MessageUpdate            |
| OnTypingActivity             | Typing activity handler              | ActivityRecieved | ActivityTypes.Typing                   |

### OnIntent
Intent rule enables you to catch and react to 'recognizedIntent' events emitted by a recognizer. All built-in recognizers emit this event when they successfully process an input utterance. 

``` C#
// Create root dialog as an Adaptive dialog.
var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog));

// Add a regex recognizer
rootDialog.Recognizer = new RegexRecognizer()
{
    Intents = new List<IntentPattern>()
    {
        new IntentPattern()
        {
            Intent = "HelpIntent",
            Pattern = "(?i)help"
        },
        new IntentPattern()
        {
            Intent = "CancelIntent",
            Pattern = "(?i)cancel|never mind"
        }
    }
};

// Create an intent rule with the intent name
var helpTrigger = new OnIntent("HelpIntent");

// Create steps when the helpRule triggers
var helpActions = new List<Dialog>();
helpActions.Add(new SendActivity("Hello, I'm the samples bot. At the moment, I respond to only help!"));
helpTrigger.Actions = helpActions;

// Add the help rule to root dialog
rootDialog.Triggers.Add(helpTrigger);
```
**Note:** You can use the OnIntent to also trigger on 'entities' generated by a recognizer, but it has to be within the context of an OnIntent. 

### OnUnknownIntent
Use this trigger to catch and respond to a case when a 'recognizedIntent' event was not caught and handled by any of the other trigger. This is especially helpful to capture and handle cases where your dialog wishes to participate in consultation.

``` C# 
// Create root dialog as an Adaptive dialog.
var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog));

// Add a regex recognizer
rootDialog.Recognizer = new RegexRecognizer()
{
    Intents = new List<IntentPattern>()
    {
        new IntentPattern()
        {
            Intent = "HelpIntent",
            Pattern = "(?i)help"
        },
        new IntentPattern()
        {
            Intent = "CancelIntent",
            Pattern = "(?i)cancel|never mind"
        }
    }
};

// Create an intent rule with the intent name
var helpTrigger = new OnIntent("HelpIntent");

// Create steps when the helpRule triggers
var helpActions = new List<Dialog>();
helpActions.Add(new SendActivity("Hello, I'm the samples bot. At the moment, I respond to only help!"));
helpTrigger.Actions = helpActions;

// Add the help rule to root dialog
rootDialog.Triggers.Add(helpTrigger);

// Add a trigger to capture unhandled intents. Unknown intent trigger fires when a recognizedIntent event is raised
// by the recognizer is not handled by any trigger added to the dialog.
// Given the RegEx recognizer added to this dialog, this trigger will fire when user says 'cancel'.
// Although the recognizer returned 'cancel' intent, we have no trigger attached to handled it. 
// This trigger will also fire when user says 'yo'. The recognizer will return a 'none' intent however that
// intent is not caught by a trigger added to this dialog. 
var unhandledIntentTrigger = new OnUnknownIntent();
var unhandledIntentActions = new List<Dialog>();
unhandledIntentActions.Add(new SendActivity("Sorry, I did not recognize that"));
unhandledIntentTrigger.Actions = unhandledIntentActions;

rootDialog.Triggers.Add(unhandledIntentTrigger);
```

### OnActivity
Activity triggers enables you to associate steps to any incoming activity from the client. Please see [here][15] for Bot Framework Activity definition.

```C#
var myDialog = new AdaptiveDialog(nameof(AdaptiveDialog)) 
{
    Generator = new TemplateEngineLanguageGenerator(new TemplateEngine().AddFile(Path.Combine(".", "myDialog.lg"))),
    Triggers = new List<OnCondition>() {
        new OnConversationUpdateActivity() {
            Actions = new List<Dialog>() {
                new SendActivity("${Welcome-user()}")
            }
        }
    }
};
```

## Inputs 
_Inputs_ are wrappers around [prompts][2] that you can use in an adaptive dialog step to ask and collect a piece of input from a user, validate and accept it into memory. Inputs include these pre-built features: 
- Accepts a property to bind to the new [state management][6] scopes. 
- Performs existential check before prompting. 
- Grounds input to the specified property if the input from user matches the type of entity expected. 
- Accepts constraints - min, max, etc. 
- Handle locally relevant intents within a dialog as well as use interruption as a technique to bubble up user response to an appropriate parent dialog that can handle it. 

See [here](./all-about-interruptions.md) to learn more about interruption. 

Adaptive dialogs support the following inputs - 
- [TextInput](#TextInput)
- [ChoiceInput](#ChoiceInput)
- [ConfirmInput](#ConfirmInput)
- [NumberInput](#NumberInput)
- [DateTimeInput](#DateTimeInput)
- [OAuth input](#OAuth)
- [AttachmentInput](#AttachmentInput)

### TextInput
Use text input when you want to verbatim accept user input as a value for a specific piece of information your bot is trying to collect. E.g. user's name, subject of an email etc. 

| Property             | Description                                                                                                                                                            |
| -------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| TextOutputFormat     | Indicates how the output from the text input should be post-processed. Options are None, Trim, LowerCase, UpperCase                                                    |
| Value                | Denotes the value (as an expression) to set to the Property. Value expression is evaluated on every turn                                                               |
| DefaultValue         | Default value the property is set to if max turn count is reached                                                                                                      |
| AlwaysPrompt         | Denotes if we should always execute this prompt even if the property has an existing value set                                                                         |
| AllowInterruptions   | Denotes if this input is interruptable. Expression.                                                                                                                    |
| Prompt               | Initial prompt response to ask for user input                                                                                                                          |
| UnrecognizedPrompt   | Prompt string to use if the input was unrecognized                                                                                                                     |
| InvalidPrompt        | Response when input is not recognized or not valid for the expected input type                                                                                         |
| Validations          | List of expressions used to validate if user provided input meets required constraints. You can use turn.value to examine the user input in the validation expressions |
| MaxTurnCount         | Denotes the maxinum number of attempts this specific input will execute to resolve a value                                                                             |
| DefaultValueResponse | Response to send when MaxTurnCount has been reached and the default value is used                                                                                      |
| Property             | Property this input dialog is bound to                                                                                                                                 |
| Pattern              | Optional regex to validate input                                                                                                                                       |


``` C#
// Create root dialog as an Adaptive dialog.
// Create an adaptive dialog.
var getUserNameDialog = new AdaptiveDialog("GetUserNameDialog");

// Add an intent trigger.
getUserNameDialog.Triggers.Add(new OnIntent()
{
    Intent = "GetName",
    Actions = new List<Dialog>()
    {
        // Add TextInput step. This step will capture user's input and set it to 'user.name' property.
        // See ./memory-model-overview.md for available memory scopes.
        new TextInput()
        {
            Property = "user.name",
            Prompt = new ActivityTemplate("Hi, What is your name?")
        }
    }
});
```

### ChoiceInput
Choice input asks for a choice from a set of options. 

| Property             | Description                                                                                                                                                            |
| -------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Choices              | Array representing possible choices                                                                                                                                    |
| ChoiceOutputFormat   | Indicates how the output is formated. Options are Value or Index                                                                                                       |
| ChoicesProperty      | Expression collection of choices to present to user                                                                                                                    |
| Style                | Rendering style for available choices: Inline; List; SugestedActions; HeroCard                                                                                         |
| DefaultLocale        | Sets the default locale for input processing. Supported locales are Spanish, Dutch, English, French, German, Japanese, Portuguese, Chinese                             |
| ChoiceOptions        | ChoiceOptions controls display options for customizing language                                                                                                        |
| RecognizerOptions    | Customize how to use the choices to recognize the response from the user                                                                                               |
| Value                | Denotes the value (as an expression) to set to the Property. Value expression is evaluated on every turn                                                               |
| DefaultValue         | Default value the property is set to if max turn count is reached                                                                                                      |
| AlwaysPrompt         | Denotes if we should always execute this prompt even if the property has an existing value set                                                                         |
| AllowInterruptions   | Denotes if this input is interruptable. Expression.                                                                                                                    |
| Prompt               | Initial prompt response to ask for user input                                                                                                                          |
| UnrecognizedPrompt   | Prompt string to use if the input was unrecognized                                                                                                                     |
| InvalidPrompt        | Response when input is not recognized or not valid for the expected input type                                                                                         |
| Validations          | List of expressions used to validate if user provided input meets required constraints. You can use turn.value to examine the user input in the validation expressions |
| MaxTurnCount         | Denotes the maxinum number of attempts this specific input will execute to resolve a value                                                                             |
| DefaultValueResponse | Response to send when MaxTurnCount has been reached and the default value is used                                                                                      |
| Property             | Property this input dialog is bound to                                                                                                                                 |

``` C#
// Create an adaptive dialog.
var getUserFavoriteColor = new AdaptiveDialog("GetUserColorDialog");
getUserFavoriteColor.Triggers.Add(new OnIntent()
{
    Intent = "GetColor",
    Actions = new List<Dialog>()
    {
        // Add choice input.
        new ChoiceInput()
        {
            // Output from the user is automatically set to this property
            Property = "user.favColor",
            
            // List of possible styles supported by choice prompt. 
            Style = Bot.Builder.Dialogs.Choices.ListStyle.Auto,
            Prompt = new ActivityTemplate("What is your favorite color?"),
            Choices = new ChoiceSet(new List<Choice>()
            {
                new Choice("Red"),
                new Choice("Blue"),
                new Choice("Green")
            })
        }
    }
});
```

### ConfirmInput
As the name implies, asks user for confirmation. 


| Property             | Description                                                                                                                                                            |
| -------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Value                | Denotes the value (as an expression) to set to the Property. Value expression is evaluated on every turn                                                               |
| DefaultValue         | Default value the property is set to if max turn count is reached                                                                                                      |
| AlwaysPrompt         | Denotes if we should always execute this prompt even if the property has an existing value set                                                                         |
| AllowInterruptions   | Denotes if this input is interruptable. Expression.                                                                                                                    |
| Prompt               | Initial prompt response to ask for user input                                                                                                                          |
| UnrecognizedPrompt   | Prompt string to use if the input was unrecognized                                                                                                                     |
| InvalidPrompt        | Response when input is not recognized or not valid for the expected input type                                                                                         |
| Validations          | List of expressions used to validate if user provided input meets required constraints. You can use turn.value to examine the user input in the validation expressions |
| MaxTurnCount         | Denotes the maxinum number of attempts this specific input will execute to resolve a value                                                                             |
| DefaultValueResponse | Response to send when MaxTurnCount has been reached and the default value is used                                                                                      |
| Property             | Property this input dialog is bound to                                                                                                                                 |

``` C#
// Create adaptive dialog.
var ConfirmationDialog = new AdaptiveDialog("ConfirmationDialog") {
    Triggers = new List<OnCondition>() 
    {
        new OnUnknownIntent()
        {
            Actions = new List<Dialog>()
            {
                // Add confirmation input.
                new ConfirmInput()
                {
                    Property = "turn.contoso.travelBot.confirmOutcome",
                    // Since this prompt is built as a generic confirmation wrapper, the actual prompt text is 
                    // read from a specific memory location. The caller of this dialog needs to set the prompt
                    // string to that location before calling the "ConfirmationDialog".
                    // All prompts support rich language generation based resolution for output generation. 
                    // See ../../language-generation/README.md to learn more.
                    Prompt = new ActivityTemplate("${turn.contoso.travelBot.confirmPromptMessage}")
                }
            }
        }
    }
};
```

### NumberInput

Asks for a number.

| Property             | Description                                                                                                                                                            |
| -------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| NumberOutputFormat   | Controls the output format of the value recognized by input. Possible options are Float, Integer                                                                       |
| DefaultLocale        | Sets the default locale for input processing. Supported locales are Spanish, Dutch, English, French, German, Japanese, Portuguese, Chinese                             |
| Value                | Denotes the value (as an expression) to set to the Property. Value expression is evaluated on every turn                                                               |
| DefaultValue         | Default value the property is set to if max turn count is reached                                                                                                      |
| AlwaysPrompt         | Denotes if we should always execute this prompt even if the property has an existing value set                                                                         |
| AllowInterruptions   | Denotes if this input is interruptable. Expression.                                                                                                                    |
| Prompt               | Initial prompt response to ask for user input                                                                                                                          |
| UnrecognizedPrompt   | Prompt string to use if the input was unrecognized                                                                                                                     |
| InvalidPrompt        | Response when input is not recognized or not valid for the expected input type                                                                                         |
| Validations          | List of expressions used to validate if user provided input meets required constraints. You can use turn.value to examine the user input in the validation expressions |
| MaxTurnCount         | Denotes the maxinum number of attempts this specific input will execute to resolve a value                                                                             |
| DefaultValueResponse | Response to send when MaxTurnCount has been reached and the default value is used                                                                                      |
| Property             | Property this input dialog is bound to                                                                                                                                 |

``` C#
var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
{
    Generator = new TemplateEngineLanguageGenerator(),
    Triggers = new List<OnCondition> ()
    {
        new OnUnknownIntent()
        {
            Actions = new List<Dialog>()
            {
                new NumberInput() {
                    Property = "user.favoriteNumber",
                    Prompt = new ActivityTemplate("Give me your favorite number (1-10)"),
                    // You can refer to incoming user message via turn.activity.text
                    UnrecognizedPrompt = new ActivityTemplate("Sorry, '{turn.activity.text}' did not include a valid number"),
                    // You can provide a list of validation expressions. Use turn.value to refer to any value extracted by the recognizer.
                    Validations = new List<String> () {
                        "int(this.value) >= 1",
                        "int(this.value) <= 10"
                    },
                    InvalidPrompt = new ActivityTemplate("Sorry, {this.value} does not work. Can you give me a different number that is between 1-10?"),
                    MaxTurnCount = 2,
                    DefaultValue = "9",
                    DefaultValueResponse = new ActivityTemplate("Sorry, we have tried for '${%MaxTurnCount}' number of times and I'm still not getting it. For now, I'm setting '${%property}' to '${%DefaultValue}'"),
                    AllowInterruptions = "false",
                    AlwaysPrompt = true,
                    OutputFormat = NumberOutputFormat.Integer
                },
                new SendActivity("Your favorite number is {user.favoriteNumber}")
            }
        }
    }
};
```

### DateTimeInput
Asks for a date/time.

| Property             | Description                                                                                                                                                            |
| -------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| DefaultLocale        | Sets the default locale for input processing. Supported locales are Spanish, Dutch, English, French, German, Japanese, Portuguese, Chinese                             |
| Value                | Denotes the value (as an expression) to set to the Property. Value expression is evaluated on every turn                                                               |
| DefaultValue         | Default value the property is set to if max turn count is reached                                                                                                      |
| AlwaysPrompt         | Denotes if we should always execute this prompt even if the property has an existing value set                                                                         |
| AllowInterruptions   | Denotes if this input is interruptable. Expression.                                                                                                                    |
| Prompt               | Initial prompt response to ask for user input                                                                                                                          |
| UnrecognizedPrompt   | Prompt string to use if the input was unrecognized                                                                                                                     |
| InvalidPrompt        | Response when input is not recognized or not valid for the expected input type                                                                                         |
| Validations          | List of expressions used to validate if user provided input meets required constraints. You can use turn.value to examine the user input in the validation expressions |
| MaxTurnCount         | Denotes the maxinum number of attempts this specific input will execute to resolve a value                                                                             |
| DefaultValueResponse | Response to send when MaxTurnCount has been reached and the default value is used                                                                                      |
| Property             | Property this input dialog is bound to                                                                                                                                 |

``` C#
var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
{
    Generator = new TemplateEngineLanguageGenerator(_templateEngine),
    Triggers = new List<OnCondition>()
    {
        new OnUnknownIntent()
        {
            Actions = new List<Dialog>()
            {
                new DateTimeInput()
                {
                    Property = "$userDate",
                    Prompt = new ActivityTemplate("Give me a date"),
                },
                new SendActivity("You gave me ${$userDate}")
            }
        }
    }
};
```

### OAuth
Use to ask user to sign in. 

| Property       | Description                                                                             |
| -------------- | --------------------------------------------------------------------------------------- |
| ConnectionName | Name of the OAuth connection configured in Azure Bot Service settings page for the bot. |
| Title          | Title text to display in the sign in card.                                              |
| Text           | Text to display in the sign in card.                                                    |
| TokenProperty  | Property path to store the auth token                                                   |

```C#
var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
{
    Generator = new TemplateEngineLanguageGenerator(_templateEngine),
    Triggers = new List<OnCondition>()
    {
        new OnUnknownIntent()
        {
            Actions = new List<Dialog>()
            {
                new OAuthInput()
                {
                    // Name of the connection configured on Azure Bot Service for the OAuth connection.
                    ConnectionName = "GitHub",
                    
                    // Title of the sign in card.
                    Title = "Sign in",
                    
                    // Text displayed in sign in card.
                    Text = "Please sign in to your GitHub account.",

                    // Property path to store the authorization token.
                    TokenProperty = "user.authToken"
                },
                new SendActivity("You are signed in with token = ${user.authToken}")
            }
        }
    }
};
```

### AttachmentInput
Use to request an attachment from user as input.

| Property             | Description                                                                                                                                                            |
| -------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| DefaultLocale        | Sets the default locale for input processing. Supported locales are Spanish, Dutch, English, French, German, Japanese, Portuguese, Chinese                             |
| Value                | Denotes the value (as an expression) to set to the Property. Value expression is evaluated on every turn                                                               |
| DefaultValue         | Default value the property is set to if max turn count is reached                                                                                                      |
| AlwaysPrompt         | Denotes if we should always execute this prompt even if the property has an existing value set                                                                         |
| AllowInterruptions   | Denotes if this input is interruptable. Expression.                                                                                                                    |
| Prompt               | Initial prompt response to ask for user input                                                                                                                          |
| UnrecognizedPrompt   | Prompt string to use if the input was unrecognized                                                                                                                     |
| InvalidPrompt        | Response when input is not recognized or not valid for the expected input type                                                                                         |
| Validations          | List of expressions used to validate if user provided input meets required constraints. You can use turn.value to examine the user input in the validation expressions |
| MaxTurnCount         | Denotes the maxinum number of attempts this specific input will execute to resolve a value                                                                             |
| DefaultValueResponse | Response to send when MaxTurnCount has been reached and the default value is used                                                                                      |
| Property             | Property this input dialog is bound to                                                                                                                                 |
``` C#
var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
{
    Generator = new TemplateEngineLanguageGenerator(_templateEngine),
    Triggers = new List<OnCondition>()
    {
        new OnUnknownIntent()
        {
            Actions = new List<Dialog>()
            {
                new AttachmentInput()
                {
                    Property = "$userAttachmentCarImage",
                    Prompt = new ActivityTemplate("Please give me an image of your car. Drag drop the image to the chat canvas."),
                    OutputFormat = AttachmentOutputFormat.All
                },
                new SendActivity("You gave me ${$userAttachmentCarImage}")
            }
        }
    }
};
```

## Actions
_Actions_ help put together the flow of conversation when a specific event is captured via a Trigger. **_Note:_** unlike Waterfall dialog where each step is a function, each action in an Adaptive dialog is in itself a dialog. This enables adaptive dialogs by design to: 
- have a simpler way to handle interruptions.
- branch conditionally based on context or state.

Adaptive dialogs support the following actions - 
- Sending a response
    - [SendActivity](#SendActivity)
- Memory manipulation
    - [EditArray](#EditArray)
    - [InitProperty](#InitProperty)
    - [SetProperty](#SetProperty)
    - [DeleteProperty](#DeleteProperty)
    - [DeleteProperties](#DeleteProperties)
    - [SetProperties](#SetProperties)
- Conversational flow and dialog management
    - [IfCondition](#IfCondition)
    - [SwitchCondition](#SwitchCondition)
    - [EndTurn](#EndTurn)
    - [BeginDialog](#BeginDialog)
    - [EndDialog](#EndDialog)
    - [CancelAllDialog](#CancelAllDialog)
    - [ReplaceDialog](#ReplaceDialog)
    - [RepeatDialog](#RepeatDialog)
    - [EditActions](#EditSteps)
    - [EmitEvent](#EmitEvent)
    - [ForEach](#ForEach)
    - [ForEachPage](#ForEachPage)
    - [BreakLoop](#break-loop)
    - [ContinueLoop](#continue-loop)
    - [DeleteActivity](#delete-activity)
    - [GetActivityMembers](#get-activity-members)
    - [GetConversationMembers](#get-conversation-members)
    - [GotoAction](#goto-action)
    - [SignOutUser](#signout-user)
    - [UpdateActivity](#update-activity)
- Roll your own code
    - [CodeAction](#CodeStep)
    - [HttpRequest](#HttpRequest)
- Tracing and logging
    - [TraceActivity](#TraceActivity)
    - [LogAction](#LogStep)

### SendActivity
Used to send an activity to user. 

``` C#
// Example of a simple SendActivity step
var greetUserDialog = new AdaptiveDialog("greetUserDialog");
greetUserDialog.Triggers.Add(new OnIntent()
{
    Intent = "greetUser", 
    Actions = new List<Dialog>() {
        new SendActivity("Hello")
    }
});

// Example that includes reference to property on bot state.
var greetUserDialog = new AdaptiveDialog("greetUserDialog");
greetUserDialog.Triggers.Add(new OnIntent()
{
    Intent = "greetUser",
    Actions = new List<Dialog>()
    {
        new TextInput()
        {
            Property = "user.name",
            Prompt = new ActivityTemplate("What is your name?")
        },
        new SendActivity("Hello, ${user.name}")
    }
});
```
See [here][3] to learn more about using language generation instead of hard coding actual response text in SendActivity.

### EditArray
Used to perform edit operations on an array property.

``` C#
var addToDoDialog = new AdaptiveDialog("addToDoDialog");
addToDoDialog.Triggers.Add(new OnIntent()
{
    Intent = "addToDo", 
    Actions = new List<Dialog>() {
        // Save the userName entitiy from a recognizer.
        new SaveEntity("dialog.addTodo.title", "@todoTitle"),
        new TextInput()
        {
            Prompt = new ActivityTemplate("What is the title of your todo?"),
            Property = "dialog.addTodo.title"
        },
        // Add the current todo to the todo's list for this user.
        new EditArray() 
        {
            ItemsProperty = "user.todos",
            Value = "=dialog.addTodo.title"
            ChangeType = EditArray.ArrayChangeType.Push
        },
        new SendActivity("Ok, I have added ${dialog.addTodo.title} to your todos."),
        new SendActivity("You now have ${count(user.todos)} items in your todo.")
}));
```

### InitProperty
Initializes a property in memory. Can either initialize an array or object.

``` C#
new InitProperty()
{
    Property = "user.todos",
    Type = "array" // this can either be "array" or "object"
}
```

### SetProperty 
Used to set a property's value in memory. The value can either be an explict string or an expression. See [here][5] to learn more about the common expressions language.

``` C#
new SetProperty() 
{
    Property = "user.firstName",
    // If user name is Vishwac Kannan, this sets first name to 'Vishwac'
    Value = "=split(user.name, ' ')[0]"
},
```

### DeleteProperty
Removes a property from memory.

``` C#
new DeleteProperty 
{
    Property = "user.firstName"
}
```

### DeleteProperties
Delete more than one property in a single action. 

```C#
new DeleteProperties()
{
    Properties = new List<StringExpression>()
    {
        new StringExpression("user.name"),
        new StringExpression("user.age")
    }
}
```
### SetProperties

Initialize one or more properties in a single action. 

```C#
new SetProperties()
{
    Assignments = new List<PropertyAssignment>()
    {
        new PropertyAssignment()
        {
            Property = "user.name",
            Value = "Vishwac"
        },
        new PropertyAssignment()
        {
            Property = "user.age",
            Value = "=coalesce($age, 30)"
        }
    }
}
```
### IfCondition
Used to represent a branch in the conversational flow based on a specific condition. Conditions are expressed using the common expression language. See [here][5] to learn more about the common expression language.

``` C#
var addToDoDialog = new AdaptiveDialog("addToDoDialog");
addToDoDialog.Triggers.Add(new OnIntent()
{
    Intent = "addToDo",
    Actions = new List<Dialog>() 
    {
        // Save the userName entitiy from a recognizer.
        new SaveEntity("dialog.addTodo.title", "@todoTitle"),
        new TextInput()
        {
            Prompt = new ActivityTemplate("What is the title of your todo?"),
            Property = "dialog.addTodo.title"
        },
        // Add the current todo to the todo's list for this user.
        new EditArray() 
        {
            ItemsProperty = "user.todos",
            Value = "=dialog.addTodo.title"
            ChangeType = EditArray.ArrayChangeType.Push
        },
        new SendActivity("Ok, I have added ${dialog.addTodo.title} to your todos."),
        new IfCondition()
        {
            Condition = "toLower(dialog.addTodo.title) == 'call santa'",
            Actions = new List<Dialog>()
            {
                new SendActivity("Yes master. On it right now \\[You have unlocked an easter egg] :)")
            }
        },
        new SendActivity("You now have ${count(user.todos)} items in your todo.")
    }
});
```

### SwitchCondition
Used to represent branching in conversational flow based on the outcome of an expression evaluation. See [here][5] to learn more about the common expression language.

``` C#
// Create an adaptive dialog.
var cardDialog = new AdaptiveDialog("cardDialog");
cardDialog.Triggers.Add(new OnIntent()
{
    Intent = "ShowCards",
    Actions = new List<Dialog>() 
    {
        // Add choice input.
        new ChoiceInput()
        {
            // Output from the user is automatically set to this property
            Property = "turn.cardDialog.cardChoice",
            // List of possible styles supported by choice prompt. 
            Style = ListStyle.Auto,
            Prompt = new ActivityTemplate("What card would you like to see?"),
            Choices = new ChoiceSet(new List<Choice>() {
                new Choice("Adaptive card"),
                new Choice("Hero card"),
                new Choice("Video card")
            })
        }, 
        // Use SwitchCondition step to dispatch to right dialog based on choice input.
        new SwitchCondition()
        {
            Condition = "turn.cardDialog.cardChoice",
            Cases = new List<Case>() 
            {
                new Case("Adaptive card",  new List<Dialog>() { new SendActivity("${AdativeCardRef()}") } ),
                new Case("Hero card", new List<Dialog>() { new SendActivity("${HeroCard()}") } ),
                new Case("Video card",     new List<Dialog>() { new SendActivity("${VideoCard()}") } ),
            },
            Default = new List<Dialog>()
            {
                new SendActivity("[AllCards]")
            }
        }
}));
```

### EndTurn
Ends the current turn of conversation.

``` C#
new EndTurn()
```

### BeginDialog
Invoke and begin a new dialog. Begin dialog takes the target dialog's name and the target dialog can be any type of dialog including Adaptive dialog or Waterfall dialog etc.

``` C#
new BeginDialog("BookFlightDialog")
{
    // Any value returned by BookFlightDialog will be captured in the property specified here.
    ResultProperty = "$bookFlightResult"
}
```


### EndDialog
Ends the active dialog. 

**Note:** Adaptive dialogs by default will end automatically if the dialog has run out of steps to execute. To override this behavior, set the `AutoEndDialog` property on Adaptive Dialog to false. 

``` C#
new EndDialog()
{
    // Value property indicates value to return to the caller.
    Value = "=$userName"
}
```

### CancelAllDialog
Cancels all active dialogs including any active parent dialogs. 

``` C#
new CancelAllDialog()
```

### ReplaceDialog
Replace current dialog with a new dialog by name.

``` C#
// This sample illustrates the use of ReplaceDialog tied to explicit user confirmation 
// to switch to a different dialog.

// Create an adaptive dialog.
var getUserName = new AdaptiveDialog("getUserName");
getUserName.Triggers.Add(new OnIntent()
{
    Intent = "getUserName",
    Actions = new List<Dialog>() 
    {
        new TextInput()
        {
            Property = "user.name",
            Prompt = new ActivityTemplate("What is your name?")
        },
        new SendActivity("Hello ${user.name}, nice to meet you!")
    }
});

getUserName.Triggers.Add(new OnIntent()
{
    Intent = "GetWeather",
    Actions = new List<Dialog>()
    {
        // confirm with user that they do want to switch to another dialog
        new ChoiceInput()
        {
            Prompt = new ActivityTemplate("Are you sure you want to switch to talk about the weather?"),
            Property = "turn.contoso.getweather.confirmchoice",
            Choices = new ChoiceSet(new List<Choice>()
            {
                new Choice("Yes"),
                new Choice("No")
            })
        },
        new SwitchCondition()
        {
            Condition = "turn.contoso.getweather.confirmchoice",
            Cases = new List<Case>()
            {
                // Call ReplaceDialog to switch to a different dialog. 
                // BeginDialog will keep current dialog in the stack to be resumed after child dialog ends.
                // ReplaceDialog will remove current dialog from the stack and add the new dialog.
                { 
                    Value = "Yes", 
                    Actions = new List<Dialog>() 
                    { 
                        new ReplaceDialog("getWeatherDialog")
                    } 
                },
                {
                    Value = "No", 
                    Actions = new List<Dialog>() 
                    { 
                        new EndDialog() 
                    } 
                }
            }
        }
    }
});
```

### RepeatDialog
Repeat dialog will restart the parent dialog. This is particularly useful if you are trying to have a conversation where the bot is paging results to the user that they can navigate through. 

**Note:** CAUTION: Remember to use EndTurn() or one of the Inputs to collect information from the user so you do not accidentally end up implementing an infinite loop. 

``` C#
var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
{
    Triggers = new List<OnCondition>()
    {
        new OnUnknownIntent()
        {
            Actions = new List<Dialog>()
            {
                new TextInput()
                {
                    Prompt = new ActivityTemplate("Give me your favorite color. You can always say cancel to stop this."),
                    Property = "turn.favColor",
                },
                new EditArray()
                {
                    ArrayProperty = "user.favColors",
                    Value = "=turn.favColor",
                    ChangeType = EditArray.ArrayChangeType.Push
                },
                // This is required because TextInput will skip prompt if the property exists - which it will from the previous turn. 
                // Alternately you can also set `AlwaysPrompt = true` on the TextInput.
                new DeleteProperty() {
                    Property = "turn.favColor"
                },
                // Repeat dialog step will restart this dialog.
                new RepeatDialog()
            }
        },
        new OnIntent("CancelIntent")
        {
            Actions = new List<Dialog>()
            {
                new SendActivity("You have ${count(user.favColors)} favorite colors - ${join(user.favColors, ',', 'and')}"),
                new EndDialog()
            }
        }
    },
    Recognizer = new RegexRecognizer()
    {
        Intents = new List<IntentPattern>()
        {
            new IntentPattern()
            { 
                Intent = "HelpIntent", 
                Pattern = "(?i)help" 
            },
            new IntentPattern()
            { 
                Intent = "CancelIntent", 
                Pattern = "(?i)cancel|never mind" 
            }
        }
    }
};
```

### CodeAction
As the name implies, this action enables you to call a custom piece of code. 

``` C#
// Example customCodeStep method
private async Task<DialogTurnResult> CodeActionSampleFn(DialogContext dc, System.Object options)
{
    await dc.Context.SendActivityAsync(MessageFactory.Text("In custom code step"));
    // This code step decided to just return the input payload as the result.
    return dc.EndDialogAsync(options)
}

// Adaptive dialog that calls a code step.
var rootDialog = new AdaptiveDialog(rootDialogName) {
    Triggers = new List<OnCondition>()
    {
        new OnUnknownIntent()
        {
            Actions = new List<Dialog>()
            {
                new CodeAction(CodeActionSampleFn),
                new SendActivity("After code step")
            }
        }
    }
};
```

### HttpRequest
Use this to make HTTP requests to any endpoint. 

``` C#
new HttpRequest()
{
    // Set response from the http request to turn.httpResponse property in memory.
    ResultProperty = "turn.httpResponse",
    Method = HttpRequest.HttpMethod.POST,
    Header = new Dictionary<string,string> (), /* request header */
    Body = { }                                 /* request body */
}; 
```
### TraceActivity
Sends a trace activity with a payload you specify. 

**Note:** Trace activities can be captured as transcripts and are only sent to Emulator to help with debugging.

``` C#
new TraceActivity()
{
    // Name of the trace event.
    Name = "contoso.TraceActivity",
    ValueType = "Object",
    // Property from memory to include in the trace
    ValueProperty = "user"
}
```

### LogStep
Writes out to console and can also send the message as a trace activity

``` C#
new LogStep()
{
    Text = new TextTemplate("Hello"),
    // Automatically sends the provided text as a trace activity
    TraceActivity = true
}
```

### EditActions
Used to edit the current plan. Specifically helpful when handling interruption. Provides ability to end the current set of steps being executed or add to the begining or end of current plan. 

``` C#
var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
{
    Generator = new TemplateEngineLanguageGenerator(),
    Recognizer = new RegexRecognizer()
    {
        Intents = new List<IntentPattern>()
        {
            new IntentPattern()
            {
                Intent = "appendSteps",
                Pattern = "(?i)append"
            },
            new IntentPattern()
            {
                Intent = "insertSteps",
                Pattern = "(?i)insert"
            },
            new IntentPattern()
            {
                Intent = "endSteps",
                Pattern = "(?i)end"
            }
        }
    },
    Triggers = new List<OnCondition>()
    {
        new OnUnknownIntent()
        {
            Actions = new List<Dialog>()
            {
                new ChoiceInput() 
                {
                    Prompt = new ActivityTemplate("What type of EditAction would you like to see?"),
                    Property = "$userChoice",
                    AlwaysPrompt = true,
                    Choices = new ChoiceSet(new List<Choice>()
                    {
                        new Choice("Append actions"),
                        new Choice("Insert actions"),
                        new Choice("End actions"),
                    })
                },
                new SendActivity("This messge is after your EditActions choice..")
            }
        },
        new OnIntent()
        {
            Intent = "appendSteps",
            Actions = new List<Dialog>() {
                new SendActivity("In append steps .. Steps specified via EditSteps will be added to the current plan."),
                new EditActions()
                {
                    Actions = new List<Dialog>() {
                        // These steps will be appended to the current set of steps being executed. 
                        new SendActivity("I was appended!")
                    },
                    ChangeType = ActionChangeType.AppendActions
                }
            }
        },
        new OnIntent() {
            Intent = "insertSteps",
            Actions = new List<Dialog>() {
                new SendActivity("In insert steps .. "),
                new EditActions()
                {
                    Actions = new List<Dialog>() {
                        // These steps will be inserted before the current steps being executed. 
                        new SendActivity("I was inserted")
                    },
                    ChangeType = ActionChangeType.InsertActions
                }
            }
        },
        new OnIntent()
        {
            Intent = "endSteps",
            Actions = new List<Dialog>()
            {
                new SendActivity("In end steps .. "),
                new EditActions()
                {
                    // The current sequence will be ended. This is especially useful if you are looking to end an active interruption.
                    ChangeType = ActionChangeType.EndSequence
                }
            }
        }
    }
};
```

### EmitEvent
Used to raise a custom event that your bot can respond to. You can control bubbling behavior on the event raised so it can be contained just to your own dialog or bubbled up the parent chain.

```C#
var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
    {
        Generator = new TemplateEngineLanguageGenerator(),
        Triggers = new List<OnCondition>()
        {
            new OnUnknownIntent()
            {
                Actions = new List<Dialog>()
                {
                    new TextInput()
                    {
                        Prompt = new ActivityTemplate("What's your name?"),
                        Property = "user.name",
                        AlwaysPrompt = true,
                        OutputFormat = "toLower(this.value)"
                    },
                    new EmitEvent()
                    {
                        EventName = "contoso.custom",
                        EventValue = "=user.name",
                        BubbleEvent = true,
                    },
                    new SendActivity("Your name is ${user.name}"),
                    new SendActivity("And you are ${$userType}")
                }
            },
            new OnCustomEvent()
            {
                Event = "contoso.custom",
                
                // You can use conditions (expression) to examine value of the event as part of the trigger selection process.
                Condition = "turn.dialogEvent.value && (substring(turn.dialogEvent.value, 0, 1) == 'v')",
                Actions = new List<Dialog>()
                {
                    new SendActivity("In custom event: '${turn.dialogEvent.name}' with the following value '${turn.dialogEvent.value}'"),
                    new SetProperty()
                    {
                        Property = "$userType",
                        Value = "VIP"
                    }
                }
            },
            new OnCustomEvent()
            {
                Event = "contoso.custom",

                // You can use conditions (expression) to examine value of the event as part of the trigger selection process.
                Condition = "turn.dialogEvent.value && (substring(turn.dialogEvent.value, 0, 1) == 's')",
                Actions = new List<Dialog>()
                {
                    new SendActivity("In custom event: '${turn.dialogEvent.name}' with the following value '${turn.dialogEvent.value}'"),
                    new SetProperty()
                    {
                        Property = "$userType",
                        Value = "Special"
                    }
                }
            },
            new OnCustomEvent()
            {
                Event = "contoso.custom",
                Actions = new List<Dialog>()
                {
                    new SendActivity("In custom event: '${turn.dialogEvent.name}' with the following value '${turn.dialogEvent.value}'"),
                    new SetProperty()
                    {
                        Property = "$userType",
                        Value = "regular customer"
                    }
                }
            }
        }
    };
```

### ForEach
Used to apply steps to each item in a collection. 
```C#
var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
{
    Generator = new TemplateEngineLanguageGenerator(),
    Triggers = new List<OnCondition>()
    {
        new OnUnknownIntent()
        {
            Actions = new List<Dialog>()
            {
                new SetProperty()
                {
                    Property = "turn.colors",
                    Value = "=createArray('red', 'blue', 'green', 'yellow', 'orange', 'indigo')"
                },
                new Foreach()
                {
                    ItemsProperty = "turn.colors",
                    Actions = new List<Dialog>()
                    {
                        // By default, dialog.foreach.value holds the value of the item
                        // dialog.foreach.index holds the index of the item.
                        // You can use short hands to refer to these via 
                        //      $foreach.value
                        //      $foreach.index
                        new SendActivity("${$foreach.index}: Found '${$foreach.value}' in the collection!")
                    }
                }
            }
        }
    }
};
```

### ForEachPage
Used to apply steps to items in a collection. Page size denotes how many items from the collection are selected at a time.

```C#
var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
{
    Generator = new TemplateEngineLanguageGenerator(),
    Triggers = new List<OnCondition>()
    {
        new OnUnknownIntent()
        {
            Actions = new List<Dialog>()
            {
                new SetProperty()
                {
                    Property = "turn.colors",
                    Value = "=createArray('red', 'blue', 'green', 'yellow', 'orange', 'indigo')"
                },
                new ForeachPage()
                {
                    ItemsProperty = "turn.colors",
                    PageSize = 2,
                    Actions = new List<Dialog>()
                    {
                        // By default, dialog.foreach.page holds the value of the page
                        //      $foreach.page
                        new SendActivity("Page content: ${join($foreach.page, ', ')}")
                    }
                }
            }
        }
    }
};
```

### Break Loop

Break out of a loop

```C#
var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
{
    Generator = new TemplateEngineLanguageGenerator(),
    Triggers = new List<OnCondition>()
    {
        new OnUnknownIntent()
        {
            Actions = new List<Dialog>()
            {
                new SetProperty()
                {
                    Property = "turn.colors",
                    Value = "=createArray('red', 'blue', 'green', 'yellow', 'orange', 'indigo')"
                },
                new Foreach()
                {
                    ItemsProperty = "turn.colors",
                    Actions = new List<Dialog>()
                    {
                        new IfCondition()
                        {
                            Condition = "$foreach.value == 'green'",
                            Actions = new List<Dialog>()
                            {
                                new BreakLoop()
                            },
                            ElseActions = new List<Dialog>()
                            {
                                // By default, dialog.foreach.value holds the value of the item
                                // dialog.foreach.index holds the index of the item.
                                // You can use short hands to refer to these via 
                                //      $foreach.value
                                //      $foreach.index
                                new SendActivity("${$foreach.index}: Found '${$foreach.value}' in the collection!")
                            }
                        },
                    }
                }
            }
        }
    }
};
```

### Continue Loop

Continue current loop without processing rest of the statements within the loop.

```C#
var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
{
    Generator = new TemplateEngineLanguageGenerator(),
    Triggers = new List<OnCondition>()
    {
        new OnUnknownIntent()
        {
            Actions = new List<Dialog>()
            {
                new SetProperty()
                {
                    Property = "turn.colors",
                    Value = "=createArray('red', 'blue', 'green', 'yellow', 'orange', 'indigo')"
                },
                new Foreach()
                {
                    ItemsProperty = "turn.colors",
                    Actions = new List<Dialog>()
                    {
                        new IfCondition()
                        {
                            // Skip items at even position in the collection.
                            Condition = "$foreach.index % 2 == 0",
                            Actions = new List<Dialog>()
                            {
                                new ContinueLoop()
                            },
                            ElseActions = new List<Dialog>()
                            {
                                // By default, dialog.foreach.value holds the value of the item
                                // dialog.foreach.index holds the index of the item.
                                // You can use short hands to refer to these via 
                                //      $foreach.value
                                //      $foreach.index
                                new SendActivity("${$foreach.index}: Found '${$foreach.value}' in the collection!")
                            }
                        },
                    }
                }
            }
        }
    }
};
```

### Goto action
Jump to a labelled action within the current action scope.

```C#
var adaptiveDialog = new AdaptiveDialog()
{
    Triggers = new List<OnCondition>()
    {
        new OnBeginDialog()
        {
            Actions = new List<Dialog>()
            {
                new GotoAction()
                {
                    ActionId = "end"
                },
                new SendActivity("this will be skipped."),
                new SendActivity()
                {
                    Id = "end",
                    Activity = new ActivityTemplate("The End.")
                }
            }
        }
    }
};
```

### Sign out user
Sign out current signed in user.

```C#
new SignOutUser()
{
    UserId = "userid",
    ConnectionName = "connection-name"
}
```

### Delete act
Delete a activity that was sent. 

```C#
new DeleteActivity ()
{
    ActivityId = "id"
}
```

### Update activity
Update an activity that was sent. 

```C#
new UpdateActivity ()
{
    ActivityId = "id",
    Activity = new ActivityTemplate("updated value")
}
```

### Get activity members
Calls BotFrameworkAdapter.GetActivityMembers() and sets the result to a memory property.

```C#
new GetActivityMembers()
{
    Property = "turn.activityMemebrs"
}
```

### Get conversation members
Calls BotFrameworkAdapter.GetConversationMembers () and sets the result to a memory property.

```C#
new GetConversationMembers()
{
    Property = "turn.convMembers"
}
```



[1]:https://luis.ai
[2]:https://github.com/Microsoft/BotBuilder/blob/master/specs/botframework-activity/botframework-activity.md#locale
[3]:./language-generation.md
[4]:./memory-model-overview.md#turn-scope
[5]:../../common-expression-language/README.md
[6]:./memory-model-overview.md
[7]:./anatomy-and-runtime-behavior.md
[10]:../../language-generation/README.md
[11]:../../language-generation/docs/api-reference.md

[15]:https://github.com/Microsoft/botframework-sdk/blob/master/specs/botframework-activity/botframework-activity.md

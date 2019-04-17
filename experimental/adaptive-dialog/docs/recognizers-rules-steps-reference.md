# Adaptive dialog: Recognizers, rules, steps and inputs

This document describes the constituent parts of Adaptive dialog. 

## Recognizers
_Recognizers_ help understand and extract meaningful pieces of information from user's input. All recognizers emit events - of specific interest is the 'recognizedIntent' event that fires when the recognizer picks up an intent (or extracts entities) from given user utterance.

Adaptive Dialogs support the following recognizers - 
    - [RegEx recognizer](#RegEx-recognizer)
    - [LUIS recognizer](#LUIS-recognizer)
    - [Multi-language recogizer](#Multi-language-recognizer)

### RegEx recognizer
RegEx recognizer enables you to extract intent and entities based on regular expression patterns. 

``` C#
var rootDialog = new AdaptiveDialog("rootDialog")
{
    Recognizer = new RegexRecognizer()
    {
        Intents = new Dictionary<string, string>()
        {
            { "AddIntent", "(?i)(?:add|create) .*(?:to-do|todo|task)(?: )?(?:named (?<title>.*))?"},
            {  "DeleteIntent", "(?i)(?:delete|remove|clear) .*(?:to-do|todo|task)(?: )?(?:named (?<title>.*))?" },
            { "ShowIntent", "(?i)(?:show|see|view) .*(?:to-do|todo|task)|(?i)(?:show|see|view)" },
            { "ClearIntent", "(?i)(?:delete|remove|clear) (?:all|every) (?:to-dos|todos|tasks)" },
            { "HelpIntent", "(?i)help" },
            { "CancelIntent", "(?i)cancel|never mind" }
        }
    }
}
```

**Note:** RegEx recognizer will emit a 'None' intent when the input utterance does not match any defined intent.

### LUIS recognizer
[LUIS.ai][1] is a machine learning-based service to build natural language into apps, bots, and IoT devices. LUIS recognizer enables you to extract intents and entities based on a LUIS application. 

``` C#
var rootDialog = new AdaptiveDialog("rootDialog")
{
    Recognizer = new LuisRecognizer(new LuisApplication("<LUIS-APP-ID>", "<AUTHORING-SUBSCRIPTION-KEY>", "<ENDPOINT-URI>"))
}
```

### Multi-language recognizer
When building sophisticated multi-lingual bot, you will typically have one recognizer tied to a specific language x locale. Multi-language recognizer enables you to easily specify the recognizer to use based on the [locale][2] property on the incoming activity from user. 

``` C#
var rootDialog = new AdaptiveDialog("rootDialog")
{
    Recognizer = new MultiLanguageRecognizer()
    {
        Recognizers = new Dictionary<String, IRecognizer>()
        {
            { "en", new RegexRecognizer()
                    {
                        Intents = new Dictionary<string, string>()
                        {
                            { "AddIntent", "(?i)(?:add|create) .*(?:to-do|todo|task)(?: )?(?:named (?<title>.*))?"},
                            {  "DeleteIntent", "(?i)(?:delete|remove|clear) .*(?:to-do|todo|task)(?: )?(?:named (?<title>.*))?" },
                            { "ShowIntent", "(?i)(?:show|see|view) .*(?:to-do|todo|task)|(?i)(?:show|see|view)" },
                            { "ClearIntent", "(?i)(?:delete|remove|clear) (?:all|every) (?:to-dos|todos|tasks)" },
                            { "HelpIntent", "(?i)help" },
                            { "CancelIntent", "(?i)cancel|never mind" }
                        }
                    } 
            },
            { "fr", new LuisRecognizer(new LuisApplication("<LUIS-APP-ID>", "<AUTHORING-SUBSCRIPTION-KEY>", "<ENDPOINT-URI>")) }
        }
    }
}
```

## Rules
_Rules_ enable you to catch and respond to events. The broadest rule is the EventRule that allows you to catch and attach a set of steps to execute when a specific event is emitted by any sub-system. Adaptive dialogs supports couple of other specialized rules to wrap common events that your bot would handle.

Adaptive dialogs support the following Rules - 
    - [Intent rule](#Intent-rule)
    - [Event rule](#Event-rule) - catch and respond to a specific event. 
    - [Unknown intent rule](#Unknown-intent-rule) - is used to catch and respond to a case when a 'recognizedIntent' event was not caught and handled by any of the other rules. This is especially helpful to capture and handle cases where your dialog wishes to participate in consultation.   

At the core, all rules are event handlers. Here are the set of events that can be handled via a rule. 

| Event               | Description                                       |
|---------------------|---------------------------------------------------|
| BeginDialog         | Fired when a dialog is start                      |
| ActivityReceived    | Fired when a new activity comes in                |
| RecognizedIntent    | Fired when an intent is recognized                |
| UnknownIntent       | Fired when an intent is not handled or recognized |
| StepsStarted        | Fired when a plan is started                      |
| StepsSaved          | Fires when a plan is saved                        |
| StepsEnded          | Fires when a plan successful ends                 |
| StepsResumed        | Fires when a plan is resumed from an interruption |
| ConsultDialog       | fired when consulting                             |
| CancelDialog        | fired when dialog canceled                        |

### Intent rule
Intent rule enables you to catch and react to 'recognizedIntent' event emitted by a recognizer. All built-in recognizers emit this event when they successfully process an input utterance. 

``` C#
// Create root dialog as an Adaptive dialog.
var rootDialog = new AdaptiveDialog("rootDialog");

// Rules collection for root dialog.
var rootDialogRules = new List<IRule>();

// Create an intent rule with the intent name. 
// This rule is matched when a 'recognizedIntent' rule is fired with 'intents=['Book_Flight']' as the payload.
var bookFlightRule = new IntentRule("Book_Flight");

// Create steps when the bookFlightRule triggers.
var bookFlightSteps = new List<IDialog>();
bookFlightSteps.Add(new SendActivity("I can help you book a flight"));
bookFlightRule.Steps = bookFlightSteps;

// Add the bookFlightRule to rootDialog.
rootDialogRules.Add(bookFlightRule);

// Add the rootDialogRules to root dialog.
rootDialog.Rules = rootDialogRules;
```
### UnknownIntentRule

### Event rule

## Steps
  Adaptive dialogs support the following steps - 
    - Sending a response
        - SendActivity
    - Tracing and logging
        - TraceActivity
        - LogStep
    - Memory manipulation
        - SaveEntity - used to extract an entity returned by recognizer into memory.
        - EditArray
        - InitProperty
        - SetProperty - used to set a property's value in memory. See [here](../CommonExpressionLanguage) to learn more about expressions.
        - DeleteProperty
    - Conversational flow and dialog management
        - IfCondition - used to evaluate an expression. See [here](../CommonExpressionLanguage) to learn more about expressions.
        - SwitchCondition
        - EndTurn
        - BeginDialog
        - EndDialog
        - CancelAllDialog
        - ReplaceDialog
        - RepeatDialog
    - Eventing
        - EmitEvent
    - Roll your own code
        - CodeStep
        - HttpRequest
## Inputs 
Adaptive dialogs support the following inputs - 
    - TextInput
    - ChoiceInput
    - ConfirmInput
    - NumberInput

## Events

[1]:https://luis.ai
[2]:https://github.com/Microsoft/BotBuilder/blob/master/specs/botframework-activity/botframework-activity.md#locale
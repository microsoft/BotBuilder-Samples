# Adaptive dialog: Recognizers, rules, steps and inputs

This document describes the constituent parts of [Adaptive][7] dialog. 

- [Recognizers](#Recognizers)
- [Rules](#Rules)
- [Steps](#Steps)
- [Inputs](#Inputs)

## Recognizers
_Recognizers_ provide the functionality of understanding and extracting meaningful pieces of information from a user's input. All recognizers emit events - of specific interest is the 'recognizedIntent' event that fires when the recognizer picks up an intent (or extracts entities) from a given user utterance.

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
**Note:** RegEx recognizer is useful for testing and quick prototyping. For creating sophisticated conversations and production bots we recommend using a LUIS recognizer.

### LUIS recognizer
[LUIS.ai][1] is a machine learning-based service to build natural language into apps, bots, and IoT devices. Using a LUIS recognizer enables you to extract intents and entities based on a LUIS application, which you can train in advance. 

``` C#
var rootDialog = new AdaptiveDialog("rootDialog")
{
    Recognizer = new LuisRecognizer(new LuisApplication("<LUIS-APP-ID>", "<AUTHORING-SUBSCRIPTION-KEY>", "<ENDPOINT-URI>"))
}
```

### Multi-language recognizer
When building a sophisticated multi-lingual bot, you will typically have one recognizer tied to a specific language x locale. The Multi-language recognizer enables you to easily specify the recognizer to use based on the [locale][2] property on the incoming activity from a user. 

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
                            { "DeleteIntent", "(?i)(?:delete|remove|clear) .*(?:to-do|todo|task)(?: )?(?:named (?<title>.*))?" },
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
_Rules_ enable you to catch and respond to events. The broadest rule is the EventRule that allows you to catch and attach a set of steps to execute when a specific event is emitted by any sub-system. Adaptive dialogs support a couple of other specialized rules to wrap common events that your bot would handle.

Adaptive dialogs support the following Rules - 
- [Intent rule](#Intent-rule)
- [Unknown intent rule](#Unknown-intent-rule) 
- [Event rule](#Event-rule)

At the core, all rules are event handlers. Here are the set of events that can be handled via a rule.
<a name="events"></a>

| Event               | Description                                       |
|---------------------|---------------------------------------------------|
| BeginDialog         | Fired when a dialog is start                      |
| ActivityReceived    | Fired when a new activity comes in                |
| RecognizedIntent    | Fired when an intent is recognized                |
| StepsStarted        | Fired when a plan is started                      |
| StepsSaved          | Fires when a plan is saved                        |
| StepsEnded          | Fires when a plan successful ends                 |
| StepsResumed        | Fires when a plan is resumed from an interruption |
| ConsultDialog       | fired when consulting                             |
| CancelDialog        | fired when dialog canceled                        |
| UnknownIntent       | Fired when an utterance is not recognized         |

### Intent rule
Intent rule enables you to catch and react to 'recognizedIntent' events emitted by a recognizer. All built-in recognizers emit this event when they successfully process an input utterance. 

``` C#
// Create root dialog as an Adaptive dialog.
var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog));
// Add a regex recognizer
rootDialog.Recognizer = new RegexRecognizer()
{
    Intents = new Dictionary<string, string>()
    {
        { "HelpIntent", "(?i)help" },
        { "CancelIntent", "(?i)cancel|never mind" }
    }
};
// Create an intent rule with the intent name
var helpRule = new IntentRule("HelpIntent");

// Create steps when the bookFlightRule triggers
var helpSteps = new List<IDialog>();
helpSteps.Add(new SendActivity("Hello, I'm the samples bot. At the moment, I respond to only help!"));
helpRule.Steps = helpSteps;

// Add the bookFlight rule to root dialog
rootDialog.AddRule(helpRule);
```
**Note:** You can use the IntentRule to also trigger on 'entities' generated by a recognizer, but it has to be within the context of an IntentRule. 

### UnknownIntentRule
Use this rule to catch and respond to a case when a 'recognizedIntent' event was not caught and handled by any of the other rules. This is especially helpful to capture and handle cases where your dialog wishes to participate in consultation.

``` C# 
var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog));
// Add a regex recognizer
rootDialog.Recognizer = new RegexRecognizer()
{
    Intents = new Dictionary<string, string>()
    {
        { "HelpIntent", "(?i)help" },
        { "CancelIntent", "(?i)cancel|never mind" }
    }
};
// Create an intent rule with the intent name
var helpRule = new IntentRule("HelpIntent");

// Create steps when the bookFlightRule triggers
var helpSteps = new List<IDialog>();
helpSteps.Add(new SendActivity("Hello, I'm the samples bot. At the moment, I respond to only help!"));
helpRule.Steps = helpSteps;

// Add the bookFlight rule to root dialog
rootDialog.AddRule(helpRule);

// Add a rule to capture unhandled intents. Unknown intent rule fires when a recognizedIntent event is raised
// by the recognizer is not handled by any rule added to the dialog.
// Given the RegEx recognizer added to this dialog, this rule will fire when user says 'cancel'.
// Although the recognizer returned 'cancel' intent, we have no rule attached to handled it. 
// This rule will also fire when user says 'yo'. The recognizer will return a 'none' intent however that
// intent is not caught by a rule added to this dialog. 
var unhandledIntentRule = new UnknownIntentRule();
var unhandledIntentSteps = new List<IDialog>();
unhandledIntentSteps.Add(new SendActivity("Sorry, I did not recognize that"));
unhandledIntentRule.Steps = unhandledIntentSteps;

rootDialog.AddRule(unhandledIntentRule);
```

## Inputs 
_Inputs_ are wrappers around [prompts][2] that you can use in an adaptive dialog step to ask and collect a piece of input from a user, validate and accept it into memory. Inputs include these pre-built features: 
- Accepts a property to bind to the new [state management][6] scopes. 
- Performs existential check before prompting. 
- Grounds input to the specified property if the input from user matches the type of entity expected. 
- Accepts constraints - min, max, etc. 

Adaptive dialogs support the following inputs - 
- [TextInput](#TextInput)
- [ChoiceInput](#ChoiceInput)
- [ConfirmInput](#ConfirmInput)
- [NumberInput](#NumberInput)

### TextInput
Use text input when you want to verbatim accept user input as a value for a specific piece of information your bot is trying to collect. E.g. user's name, subject of an email etc. 

**Note:** By default, TextInput will trigger a consultation before accepting user input. This is to allow a parent dialog to handle the specific user input in case their recognizer had a more definitive match via an intent or entity.

| Property         | Description                                                                        |
|------------------|------------------------------------------------------------------------------------|
| property         | Property this input dialog is bound to                                             |
| prompt           | Initial prompt response to ask for user input                                      |
| retryPrompt      | Response on retry                                                                  |
| invalidPrompt    | Response when input is not recognized or not valid for the expected input type     |
| pattern          | Optional regex to validate input                                                   |
| noMatchResponse  | If a pattern is specified, response to user when input does not match the pattern  |

``` C#
// Create an adaptive dialog.
var getUserNameDialog = new AdaptiveDialog("GetUserNameDialog");

// Add an intent rule.
getUserNameDialog.AddRule(new IntentRule("GetName", 
    steps: new List<IDialog>() {
        // Add TextInput step. This step will capture user's input and set it to 'user.name' property.
        // See ./memory-model-overview.md for available memory scopes.
        new TextInput()
        {
            Property = "user.name",
            Prompt = new ActivityTemplate("Hi, What is your name?")
        }
}));
```

### ChoiceInput
Choice input asks for a choice from a set of options. 

**Note:** By default, ChoiceInput does not trigger consultation if the choice input recognizer has a high confidence match against provided choices. As an example, if one of your choices were cancel and user said 'cancel', choice input will pick this up although the expected behavior might be for the parent to capture this and handle this as global 'cancel' message from the user (or initiate disambiguation).

| Property         | Description                                                                        |
|------------------|------------------------------------------------------------------------------------|
| property         | Property this input dialog is bound to                                             |
| prompt           | Initial prompt response to ask for user input                                      |
| retryPrompt      | Response on retry                                                                  |
| invalidPrompt    | Response when input is not recognized or not valid for the expected input type     |
| style            | Rendering style for available choices: Inline; List; SugestedActions; HeroCard     |
| choices          | Array representing possible choices                                                |

``` C#
// Create an adaptive dialog.
var getUserFavoriteColor = new AdaptiveDialog("GetUserColorDialog");
getUserFavoriteColor.AddRule(new IntentRule("GetColor", 
    steps: new List<IDialog>() {
        // Add choice input.
        new ChoiceInput()
        {
            // Output from the user is automatically set to this property
            Property = "user.favColor",
            // List of possible styles supported by choice prompt. 
            Style = ListStyle.Auto,
            Prompt = new ActivityTemplate("What is your favorite color?"),
            Choices = new List<Choice>()
            {
                new Choice("Red"),
                new Choice("Blue"),
                new Choice("Green")
            }
        }
}));
```

### ConfirmInput
As the name implies, asks user for confirmation. 

**Note:** By default, ConfirmInput does not trigger consultation if the confirm input recognizer has a high confidence match against provided response. 

| Property         | Description                                                                        |
|------------------|------------------------------------------------------------------------------------|
| property         | Property this input dialog is bound to                                             |
| prompt           | Initial prompt response to ask for user input                                      |
| retryPrompt      | Response on retry                                                                  |
| invalidPrompt    | Response when input is not recognized or not valid for the expected input type     |

``` C#
// Create adaptive dialog.
var ConfirmationDialog = new AdaptiveDialog("ConfirmationDialog") {
    Steps = new List<IDialog> {
        // Add confirmation input.
        new ConfirmInput()
        {
            Property = "turn.contoso.travelBot.confirmOutcome",
            // Since this prompt is built as a generic confirmation wrapper, the actual prompt text is 
            // read from a specific memory location. The caller of this dialog needs to set the prompt
            // string to that location before calling the "ConfirmationDialog".
            // All prompts support rich language generation based resolution for output generation. 
            // See ../../language-generation/README.md to learn more.
            Prompt = new ActivityTemplate("{turn.contoso.travelBot.confirmPromptMessage}")
        }
    }
}
```

### NumberInput

Asks for a number.

**Note:** By default, ConfirmInput does not trigger consultation if the confirm input recognizer has a high confidence match against provided response. 

| Property         | Description                                                                        |
|------------------|------------------------------------------------------------------------------------|
| property         | Property this input dialog is bound to                                             |
| prompt           | Initial prompt response to ask for user input                                      |
| retryPrompt      | Response on retry                                                                  |
| invalidPrompt    | Response when input is not recognized or not valid for the expected input type     |
| minValue         | The min value which is valid                                                       |
| maxValue         | The max value which is valid                                                       |


``` C#
// Create adaptive dialog.
var getNumberDialog = new AdaptiveDialog("ConfirmationDialog") {
    Steps = new List<IDialog> {
        new NumberInput()
        {
            Property = "turn.contoso.travelBot.numberPrompt",
            Prompt = new ActivityTemplate("{turn.contoso.travelBot.numberMessage}")
        }
    }
}
```

## Steps
_Steps_ help put together the flow of conversation when a specific event is captured via a Rule. **_Note:_** unlike Waterfall dialog where each step is a function, each step in an Adaptive dialog is in itself a dialog. This enables adaptive dialogs by design to: 
- have a simpler way to handle interruptions.
- branch conditionally based on context or state.

Adaptive dialogs support the following steps - 
- Sending a response
    - [SendActivity](#SendActivity)
- Memory manipulation
    - [SaveEntity](#SaveEntity)
    - [EditArray](#EditArray)
    - [InitProperty](#InitProperty)
    - [SetProperty](#SetProperty)
    - [DeleteProperty](#DeleteProperty)
- Conversational flow and dialog management
    - [IfCondition](#IfCondition)
    - [SwitchCondition](#SwitchCondition)
    - [EndTurn](#EndTurn)
    - [BeginDialog](#BeginDialog)
    - [EndDialog](#EndDialog)
    - [CancelAllDialog](#CancelAllDialog)
    - [ReplaceDialog](#ReplaceDialog)
    - [RepeatDialog](#RepeatDialog)
- Roll your own code
    - [CodeStep](#CodeStep)
    - [HttpRequest](#HttpRequest)
- Tracing and logging
    - [TraceActivity](#TraceActivity)
    - [LogStep](#LogStep)

### SendActivity
Used to send an activity to user. 

``` C#
// Example of a simple SendActivity step
var greetUserDialog = new AdaptiveDialog("greetUserDialog");
greetUserDialog.AddRule(new IntentRule("greetUser", 
    steps: new List<IDialog>() {
        new SendActivity("Hello")
}));

// Example that includes reference to property on bot state.
var greetUserDialog = new AdaptiveDialog("greetUserDialog");
greetUserDialog.AddRule(new IntentRule("greetUser", 
    steps: new List<IDialog>() {
        new TextInput("user.name", "What is your name?"),
        new SendActivity("Hello, {user.name}")
}));
```
See [here][3] to learn more about using language generation instead of hard coding actual response text in SendActivity.

### SaveEntity
Use this step to save an entity (from a recognizer) into a different memory scope. By default, entities from recognizer are available under the [turn scope][4] and the lifetime of all information under that scope is the end of that turn of conversation. 

``` C#
var greetUserDialog = new AdaptiveDialog("greetUserDialog");
greetUserDialog.AddRule(new IntentRule("greetUser", 
    steps: new List<IDialog>() {
        // Save the userName entitiy from a recognizer.
        new SaveEntity("user.name", "turn.entities.userName[0]"),
        // Ask user for their name. All inputs by default will only initiate a prompt if the property does not exist.
        new TextInput()
        {
            Prompt = new ActivityTemplate("What is your name?"),
            Property = "user.name"
        },
        new SendActivity("Hello, {user.name}")
}));
```

### EditArray
Used to perform edit operations on an array property.

``` C#
var addToDoDialog = new AdaptiveDialog("addToDoDialog");
addToDoDialog.AddRule(new IntentRule("addToDo", 
    steps: new List<IDialog>() {
        // Save the userName entitiy from a recognizer.
        new SaveEntity("dialog.addTodo.title", "turn.entities.todoTitle[0]"),
        new TextInput()
        {
            Prompt = new ActivityTemplate("What is the title of your todo?"),
            Property = "dialog.addTodo.title"
        },
        // Add the current todo to the todo's list for this user.
        new EditArray(EditArray.ArrayChangeType.Push, "user.todos", "dialog.addTodo.title"),
        new SendActivity("Ok, I have added {dialog.addTodo.title} to your todos."),
        new SendActivity("You now have {count(user.todos)} items in your todo.")
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
    Value = new ExpressionEngine().Parse("split(user.name, ' ')[0]")
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

### IfCondition
Used to represent a branch in the conversational flow based on a specific condition. Conditions are expressed using the common expression language. See [here][5] to learn more about the common expression language.

``` C#
var addToDoDialog = new AdaptiveDialog("addToDoDialog");
addToDoDialog.AddRule(new IntentRule("addToDo",
    steps: new List<IDialog>() {
    // Save the userName entitiy from a recognizer.
    new SaveEntity("dialog.addTodo.title", "turn.entities.todoTitle[0]"),
    new TextInput()
    {
        Prompt = new ActivityTemplate("What is the title of your todo?"),
        Property = "dialog.addTodo.title"
    },
    // Add the current todo to the todo's list for this user.
    new EditArray(EditArray.ArrayChangeType.Push, "user.todos", "dialog.addTodo.title"),
    new SendActivity("Ok, I have added {dialog.addTodo.title} to your todos."),
    new IfCondition()
    {
        Condition = new ExpressionEngine().Parse("toLower(dialog.addTodo.title) == 'call santa'"),
        Steps = new List<IDialog>()
        {
            new SendActivity("Yes master. On it right now [You have unlocked an easter egg] :)")
        }
    },
    new SendActivity("You now have {count(user.todos)} items in your todo.")
}));
```

### SwitchCondition
Used to represent branching in conversational flow based on the outcome of an expression evaluation. See [here][5] to learn more about the common expression language.

``` C#
// Create an adaptive dialog.
var cardDialog = new AdaptiveDialog("cardDialog");
cardDialog.AddRule(new IntentRule("ShowCards",
    steps: new List<IDialog>() {
        // Add choice input.
        new ChoiceInput()
        {
            // Output from the user is automatically set to this property
            Property = "turn.cardDialog.cardChoice",
            // List of possible styles supported by choice prompt. 
            Style = ListStyle.Auto,
            Prompt = new ActivityTemplate("What card would you like to see?"),
            Choices = new List<Choice>()
            {
                new Choice("Adaptive card"),
                new Choice("Hero card"),
                new Choice("Video card")
            }
        }, 
        // Use SwitchCondition step to dispatch to right dialog based on choice input.
        new SwitchCondition()
        {
            Condition = "turn.cardDialog.cardChoice",
            Cases = new List<Case>() 
            {
                new Case("'Adaptive card'",  new List<IDialog>() { new SendActivity("[AdativeCardRef]") } ),
                new Case("'Hero card'", new List<IDialog>() { new SendActivity("[HeroCard]") } ),
                new Case("'Video card'",     new List<IDialog>() { new SendActivity("[VideoCard]") } ),
            },
            Default = new List<IDialog>()
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
```


### EndDialog
Ends the active dialog. 

**Note:** Adaptive dialogs by default will end automatically if the dialog has run out of steps to execute. To override this behavior, set the `AutoEndDialog` property on Adaptive Dialog to false. 

``` C#
new EndDialog()
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
getUserName.AddRule(new IntentRule("getUserName",
steps: new List<IDialog>() {
    new TextInput()
    {
        Property = "user.name",
        Prompt = new ActivityTemplate("What is your name?")
    },
    new SendActivity("Hello {user.name}, nice to meet you!")
}));

getUserName.AddRule(new IntentRule("GetWeather",
steps: new List<IDialog>()
{
    // confirm with user that they do want to switch to another dialog
    new ChoiceInput()
    {
        Prompt = new ActivityTemplate("Are you sure you want to switch to talk about the weather?"),
        Property = "turn.contoso.getweather.confirmchoice",
        Choices = new List<Choice>()
        {
            new Choice("Yes"),
            new Choice("No")
        }
    },
    new SwitchCondition()
    {
        Condition = "turn.contoso.getweather.confirmchoice",
        Cases = new Dictionary<String, List<IDialog>>()
        {
            // Call ReplaceDialog to switch to a different dialog. 
            // BeginDialog will keep current dialog in the stack to be resumed after child dialog ends.
            // ReplaceDialog will remove current dialog from the stack and add the new dialog.
            { "Yes", new List<IDialog>() { new ReplaceDialog("getWeatherDialog")} },
            { "No", new List<IDialog>() { new EndDialog() } }
        }
    }
}));

```

### RepeatDialog
Repeat dialog will restart the parent dialog. This is particularly useful if you are trying to have a conversation where the bot is paging results to the user that they can navigate through. 

**Note:** CAUTION: Remember to use EndTurn() or one of the Inputs to collect information from the user so you do not accidentally end up implementing an infinite loop. 

``` C#
var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
{
    Steps = new List<IDialog>()
    {
        new TextInput()
        {
            Prompt = new ActivityTemplate("Give me your favorite color. You can always say cancel to stop this."),
            Property = "turn.favColor",
        },
        new EditArray()
        {
            ArrayProperty = "user.favColors",
            ItemProperty = "turn.favColor",
            ChangeType = EditArray.ArrayChangeType.Push
        },
        // This is required because TextInput will skip prompt if the property exists - which it will from the previous turn.
        new DeleteProperty() {
            Property = "turn.favColor"
        },
        // Repeat dialog step will restart this dialog.
        new RepeatDialog()
    },
    Recognizer = new RegexRecognizer()
    {
        Intents = new Dictionary<string, string>()
        {
            { "HelpIntent", "(?i)help" },
            { "CancelIntent", "(?i)cancel|never mind" }
        }
    },
    Rules = new List<IRule>()
    {
        new IntentRule("CancelIntent")
        {
            Steps = new List<IDialog>()
            {
                new SendActivity("You have {count(user.favColors)} favorite colors - {join(user.favColors, ',', 'and')}"),
                new EndDialog()
            }
        }
    }
};
```


### CodeStep
As the name implies, this step enables you to call a custom piece of code. 

``` C#
// Example customCodeStep method
private async Task<DialogTurnResult> CodeStepSampleFn(DialogContext dc, System.Object options)
{
    await dc.Context.SendActivityAsync(MessageFactory.Text("In custom code step"));
    // This code step decided to just return the input payload as the result.
    return new DialogTurnResult(DialogTurnStatus.Complete, options);
}

// Adaptive dialog that calls a code step.
var rootDialog = new AdaptiveDialog(rootDialogName) {
    Steps = new List<IDialog>() {
        new CodeStep(CodeStepSampleFn),
        new SendActivity("After code step")
    }
};
```
You can use `Property` on the code step to bind to a specific property in memory as both input to and output from the step, or you can use `InputProperties` and `OutputProperty` to bind your memory to specific input to the step and output from the step.

``` C#
new CodeStep(CodeStepSampleFn) {
    // Pass user.age to the code step function (via options)
    // Any result returned by the code step is set to user.age
    Property = "user.age"
}
```

### HttpRequest
Use this to make HTTP requests to any endpoint. 

``` C#
new HttpRequest()
{
    // Set response from the http request to turn.httpResponse property in memory.
    Property = "turn.httpResponse",
    Method = HttpRequest.HttpMethod.POST,
    Header = new Dictionary<string,string> (), /* request header */
    Body = { }                                 /* request body */
}); 
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

[1]:https://luis.ai
[2]:https://github.com/Microsoft/BotBuilder/blob/master/specs/botframework-activity/botframework-activity.md#locale
[3]:./language-generation.md
[4]:./memory-model-overview.md#turn-scope
[5]:../../common-expression-language/README.md
[6]:./memory-model-overview.md
[7]:./anatomy-and-runtime-behavior.md

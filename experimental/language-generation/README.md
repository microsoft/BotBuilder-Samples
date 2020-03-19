# Language Generation ***_[PREVIEW]_***

> See [here](#Change-Log) for what's new in **4.8.0 PREVIEW** release.

Language Generation (LG) was created to let developers extract embedded strings from their code and resource files and manage them through a LG runtime and file format. Developers can now create a more natural conversation experience by defining multiple variations on a phrase, executing simple expressions based on context, and referring to conversational memory.

LG can be used to enhance the entire conversational experience. Using LG one can:

- achieve a coherent personality, tone of voice for their bot
- separate business logic from presentation
- include variations and sophisticated composition based resolution for any of their bot's replies
- construct speak .vs. display adaptations
- construct cards, suggested actions and attachments

At the core of LG lies template expansion and entity substitution. You can provide one-off variation for expansion as well as conditionally expand a template. The output from LG can be a simple text string, multi-line response or a complex object payload that a layer above LG will use to construct an [activity][1].

Below is a sample of a simple greeting LG template. Notice that all of the greetings reference the user's name in memory with the variable `${user.name}`.

```markdown
# greetingTemplate
- Hello ${user.name}, how are you?
- Good morning ${user.name}.It's nice to see you again.
- Good day ${user.name}. What can I do for you today?
```

## Language Generation in action

You can use Language Generation in a variety of ways when developing bots. To start, create one or more [.lg file(s)][3] to cover all possible scenarios where you would use the language generation sub-system with your bot's replies to user.

Then make sure you include the platform specific Language Generation library.

For C#, add Microsoft.Bot.Builder.LanguageGeneration.
For NodeJS, add botbuilder-lg

Parse and load templates in your .lg file

For C#

```c#
    Templates _lgTemplates = Templates.ParseFile(filePath, importResolver?);
```

For NodeJS

```typescript
     let lgTemplates = Templates.parseFile(filePath, importResolver?);
```

When you need template expansion, use `Evaluate` and pass in the relevant template name

For C#

```c#
    var lgOutput = _lgTemplates.Evaluate("<TemplateName>", evalData);
```

For NodeJS

```typescript
    let lgOutput = _lgTemplates.evaluate("<TemplateName>", evalData)
```

If your template needs specific properties to be passed for resolution/ expansion, you can pass them in on the call to `Evaluate`

For C#

```c#
    var lgOutput = _lgTemplates.Evaluate("WordGameReply", new { GameName = "MarcoPolo" } );
```

For NodeJS

```typescript
    let lgOutput = _lgTemplates.evaluate("WordGameReply", { GameName = "MarcoPolo" } )
```

## Multi-lingual generation and language fallback policy

Your bot might target more than one spoken/display language. To do this, you can manage separate instances of TemplateEngine, one per target language. See the [05.a.multi-turn-prompt-with-language-fallback sample][25] for an example of how to add language fallback to your bot.

## Grammar check and correction

The current library does not include any capabilities for grammar check or correction.

## Expand api

To get all possible expansions of a template, you can use `ExpandTemplate`.
For C#

```c#
    var results = _lgTemplates.ExpandTemplate("WordGameReply", { GameName = "MarcoPolo" } )
```

For NodeJS

```typescript
    const results = _lgTemplates.expandTemplate("WordGameReply", { GameName = "MarcoPolo" } )
```

As an example, given this LG content,

```
# Greeting
- Hi
- Hello

#TimeOfDay
- Morning
- Evening

# FinalGreeting
- ${Greeting()} ${TimeOfDay()}

# TimeOfDayWithCondition
- IF: ${time == 'morning'}
    - ${Greeting()} Morning
- ELSEIF: ${time == 'evening'}
    - ${Greeting()} Evening
- ELSE:
    - ${Greeting()} Afternoon
```

The call `ExpandTemplate("FinalGreeting")`, results in 4 evaluations: `"Hi Morning", "Hi Evening", "Hello Morning", "Hello Evening"`,

The call `ExpandTemplate("TimeOfDayWithCondition", new { time = "evening" })` with scope, results in 2 expansions: `"Hi Evening", "Hello Evening"`

## Packages

Preview packages:

- C# -> [NuGet][14]
- JS -> [npm][15]

Nightlies:

- C# -> [BotBuilder MyGet feed][12]
- JS -> [BotBuilder MyGet feed][13]

## Additional resources

- [API reference][2]
- [.lg file format][3]

## Change Log
### 4.8 PREVIEW
- \[**BREAKING CHANGES**\]:
    - `ActivityFactory` 
        - has been moved to `Microsoft.Bot.Builder`
        - `CreateActivity` renamed to `FromObject`
    - `TemplateEngine` 
        - has been renamed to `Templates`
        - `EvaluateTemplate` renamed to `Evaluate`
        - `Evaluate` renamed to `EvaluateText`
        - `AddFile` has been replaced by `ParseFile`
        - `AddFiles` has been deprecated. You no longer can load multiple .lg files. Instead, you should use [import][50] support in your .lg files.
    - Bounding character for expressions has been changed from **@**{expression} to **$**{expression}

    |  Old  | New |
    |-------|-----|
    |  # myTemplate <br/> - I have @{user.name} as your name |  # myTemplate <br/> - I have ${user.name} as your name |
    | # myTemplate <br/> - @{ackPhrase()} <br/><br/> # ackPhrase <br/> - hi <br/>- hello | # myTemplate <br/> - ${ackPhrase()} <br/><br/> # ackPhrase <br/> - hi <br/>- hello |
    | # myTemplate <br/> - IF : @{user.name == null} <br/>&nbsp;&nbsp;&nbsp;&nbsp;- hello<br/>- ELSE : <br/>&nbsp;&nbsp;&nbsp;&nbsp;- null | # myTemplate <br/> - IF : ${user.name == null} <br/>&nbsp;&nbsp;&nbsp;&nbsp;- hello<br/>- ELSE : <br/>&nbsp;&nbsp;&nbsp;&nbsp;- null |
- New sample ([C#]][s1-c#], [JS][s1-JS]) that demonstrates how to extend the set of prebuilt expression functions and using custom functions in LG.

### 4.7 PREVIEW
- \[**BREAKING CHANGES**\]:
    - Old way to refer to a template via `[TemplateName]` notation is deprecated in favor of `${TemplateName()}` notation. There are no changes to how structured response templates are defined.
    - All expressions must now be enclosed within `${<expression>}`. The old notation `{<expression>}` is no longer supported.
    - `ActivityBuilder` has been deprecated and removed in favor of `ActivityFactory`. Note that by stable release, functionality offered by `ActivityFactory` is likely to move into `MessageFactory`.

    |  Old  | New |
    |-------|-----|
    | # myTemplate <br/> - I have {user.name} as your name |  # myTemplate <br/> - I have @{user.name} as your name |
    | # myTemplate <br/> - [ackPhrase] <br/><br/> # ackPhrase <br/> - hi <br/>- hello | # myTemplate <br/> - @{ackPhrase()} <br/><br/> # ackPhrase <br/> - hi <br/>- hello | 

- \[**NEW**\]:
    - Language generation preview is now available for JavaScript as well. Checkout packages [here][15]. Samples are [here][26]
    - New `ActivityFactory` class that helps transform structured response template output from LG into a Bot framework activity.
    - Bug fixes and stability improvements.

### 4.6 PREVIEW 2
- \[**BREAKING CHANGES**\]:
    - Old `display || speak` notation is deprecated in favor of structured template support. See below for more details on structured template. 
    - Old `Chatdown` style cards are deprecated in favor of structured template support. See below for more details on structured template. 
- \[**NEW**\]:
    - Structured Template support in .lg file format. See [here](./docs/structured-response-template.md) to learn more about Structured Template definition.
    - ActivityGenerator.GenerateFromLG static method to transform output from LG sub-system into a full blown [Bot Framework Activity][1]

### 4.6 PREVIEW
- \[**NEW**\] [VS code extension][22] for LG (syntax highlighting, auto-suggest (including expressions, pre-built functions, template names etc), validation)
- LG file format:
    - Support for [Switch..Case..Default][20]
    - Support for [import reference][21] to another .lg file.
- [API changes][2]: 
    - Dropped FromFile and FromText methods in favor of AddFile and AddFiles. 
    - Added ability to provide a delegate to externally resolve import references found in content. 
- \[**NEW**\] Translate functionality in [MSLG CLI][23]

### 4.5 PREVIEW
- Initial preview release


[1]:https://github.com/Microsoft/BotBuilder/blob/master/specs/botframework-activity/botframework-activity.md
[2]:./docs/api-reference.md
[3]:./docs/lg-file-format.md
[6]:https://github.com/microsoft/botframework-cli/tree/master/packages/chatdown
[7]:https://github.com/microsoft/botframework-cli/blob/master/packages/chatdown/docs/chatdown-format.md
[8]:https://github.com/microsoft/botframework-cli/blob/master/packages/chatdown/docs/examples/CardExamples.chat
[9]:https://github.com/microsoft/botframework-cli/blob/master/packages/chatdown/docs/chatdown-format.md#message-commands
[10]:https://github.com/microsoft/botframework-cli/blob/master/packages/chatdown/docs/chatdown-format.md#message-cards
[11]:https://github.com/microsoft/botframework-cli/blob/master/packages/chatdown/docs/chatdown-format.md#message-attachments
[12]:https://botbuilder.myget.org/F/botbuilder-v4-dotnet-daily/api/v3/index.json
[13]:https://botbuilder.myget.org/gallery/botbuilder-v4-js-daily
[14]:https://www.nuget.org/packages/Microsoft.Bot.Builder.LanguageGeneration/4.7.0-preview
[15]:https://www.npmjs.com/package/botbuilder-lg
[20]:./docs/lg-file-format.md#Switch..Case
[21]:./docs/lg-file-format.md#Importing-external-references
[22]:https://aka.ms/lg-vscode-extension
[23]:https://github.com/microsoft/botbuilder-tools/tree/V.Future/packages/MSLG
[25]:./csharp_dotnetcore/05.a.multi-turn-prompt-with-language-fallback/
[26]:./javascript_nodejs/
[50]:./docs/lg-file-format.md#importing-external-references
[s1-c#]:./csharp_dotnetcore/20.extending-with-custom-functions/README.md
[s1-JS]:./javascript_nodejs/20.custom-functions/README.md
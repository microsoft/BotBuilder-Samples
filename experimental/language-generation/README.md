# Language Generation ***_[PREVIEW]_***

> See [here](#Change-Log) for what's new in **4.7.0 PREVIEW** release.

Language Generation (LG) lets developers extract embedded strings from their code and resource files and manage them through a Language Generation runtime and file format. Developers can now create a more natural converstion natural conversation experience by defining multiple variations on a phrase, executing simple expressions based on context, and referring to conversational memory.

At the core of language generation lies template expansion and entity substitution. You can provide one-of variation for expansion as well as conditionally expand a template. The output from language generation can be a simple text string or multi-line response or a complex object payload that a layer above language generation will use to construct a full blown [activity][1].

Language generation is achieved through:
- Markdown based [.lg][3] file that describes the templates and their composition.
- full access to current bot's memory.
- parser and runtime libraries that help achieve runtime resolution.

Below is a sample of a simple greeting LG template. Notice that all of the following grettings make use of the user's name in memory by referencing the variable `@{user.name}`.
```markdown
# greetingTemplate
- Hello @{user.name}, how are you?
- Good morning @{user.name}.It's nice to see you again.
- Good day @{user.name}. What can I do for you today?
```

LG can be used to enhance the entire conversational experience. Using LG developers now can:
- achieve a coherent personality, tone of voice for your bot
- separate business logic from presentation
- include variations and sophisticated composition based resolution for any of your bot's replies
- construct speak .vs. display adaptations
- construct cards, suggested actions and attachments.

## Language Generation in action

You can use Language Generation in a variety of ways when developing bots. To start, analyze your current bot's code (or the new bot you plan to develop) and create [.lg file][3] to cover all possible scenarios where you would use the language generation sub-system with your bot's replies to user.

Then make sure you include the platform specific Language Generation library.

For C#, add Microsoft.Bot.Builder.LanguageGeneration.
For NodeJS, add botbuilder-lg

Load the template manager with your .lg file(s)

For C#

```c#
    // multi lg files
    TemplateEngine lgEngine = new TemplateEngine().AddFiles(filePaths, importResolver?);

    // single lg file
    TemplateEngine lgEngine = new TemplateEngine().AddFile(filePath, importResolver?);
```

For NodeJS

```typescript
    // multi lg files
    let lgEngine = new TemplateEngine().addFiles(filePaths, importResolver?);

    // single lg file
    let lgEngine = new TemplateEngine().addFile(filePath, importResolver?);
```

When you need template expansion, call the templateEngine and pass in the relevant template name

For C#

```c#
    await turnContext.SendActivityAsync(lgEngine.EvaluateTemplate("<TemplateName>", entitiesCollection));
```

For NodeJS

```typescript
    await turnContext.sendActivity(ActivityFactory.createActivity(lgEngine.evaluateTemplate("<TemplateName>", entitiesCollection)));
```

If your template needs specific entity values to be passed for resolution/ expansion, you can pass them in on the call to `evaluateTemplate`

For C#

```c#
    await turnContext.SendActivityAsync(lgEngine.EvaluateTemplate("WordGameReply", new { GameName = "MarcoPolo" } ));

```

For NodeJS

```typescript
    await turnContext.sendActivity(ActivityFactory.createActivity(lgEngine.evaluateTemplate("WordGameReply", { GameName = "MarcoPolo" } )));
```

## Multi-lingual generation and language fallback policy
Quite often your bot might target more than one spoken/ display language. To do this, you can manage separate instances of TemplateEngine, one per target language. See [here][25] for an example.

## Grammar check and correction

The current library does not include any capabilities for grammar check or correction.

## Expand api

If you need to know the expand result of the evaluation of a template, `ExpandTemplate` is what you want.
For C#

```c#
    var results = lgEngine.ExpandTemplate("WordGameReply", { GameName = "MarcoPolo" } )
```

For NodeJS

```typescript
    const results = lgEngine.expandTemplate("WordGameReply", { GameName = "MarcoPolo" } )
```

For example:

```
# Greeting
- Hi
- Hello

#TimeOfDay
- Morning
- Evening

# FinalGreeting
- @{Greeting()} @{TimeOfDay()}

# TimeOfDayWithCondition
- IF: @{time == 'morning'}
    - @{Greeting()} Morning
- ELSEIF: @{time == 'evening'}
    - @{Greeting()} Evening
- ELSE:
    - @{Greeting()} Afternoon
```

If you call `lgEngine.ExpandTemplate("FinalGreeting")`, you would get four items: `"Hi Morning", "Hi Evening", "Hello Morning", "Hello Evening"`,

If you call `lgFile.ExpandTemplate("TimeOfDayWithCondition", new { time = "evening" })` with scope, you would get two expanded results: `"Hi Evening", "Hello Evening"`

## Packages
Stable release packages:
- C# -> [NuGet][14]
- JS -> [npm][15]

Nightly release packages:
- C# -> [BotBuilder MyGet feed][12]
- JS -> [BotBuilder MyGet feed][13]

## Additional resources
- Language Generation [API reference][2] documents

## Change Log
### 4.7 PREVIEW
- \[**BREAKING CHANGES**\]:
    - Old way to refer to a template via `[TemplateName]` notation is deprecated in favor of `@{TemplateName()}` notation. There are no changes to how structured response templates are defined.
    - All expressions must now be enclosed within `@{<expression>}`. The old notation `{<expression>}` is no longer supported.
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
[6]:https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown
[7]:https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown#chat-file-format
[8]:https://github.com/Microsoft/botbuilder-tools/blob/master/packages/Chatdown/Examples/CardExamples.chat
[9]:https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown#message-commands
[10]:https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown#message-cards
[11]:https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown#message-attachments
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

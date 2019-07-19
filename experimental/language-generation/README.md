# Language Generation ***_[PREVIEW]_***

Learning from our customers experiences and bringing together capabilities first implemented by Cortana and Cognition teams, we are introducing Language Generation; which allows the developer to extract the embedded strings from their code and resource files and manage them through a Language Generation runtime and file format.  Language Generation enable customers to define multiple variations on a phrase, execute simple expressions based on context, refer to conversational memory, and over time will enable us to bring additional capabilities all leading to a more natural conversational experience.

At the core of language generation lies template expansion and entity substitution. You can provide one-of variation for expansion as well as conditionally expand a template. The output from language generation can be a simple text string or multi-line response or a complex object payload that a layer above language generation will use to construct a full blown [activity][1].

Language generation is achieved through:

- markdown based .lg file that describes the templates and their composition. See [here][3] for the .lg file format.
- full access to current bots memory so you can data bind language to the state of memory.
- parser and runtime libraries that help achieve runtime resolution. See [here][2] for API-reference.

```markdown
# greetingTemplate
- Hello {user.name}, how are you?
- Good morning {user.name}. It's nice to see you again.
- Good day {user.name}. What can I do for you today?
```

You can use language generation to:

- achieve a coherent personality, tone of voice for your bot
- separate business logic from presentation
- include variations and sophisticated composition based resolution for any of your bot's replies
- construct speak .vs. display adaptations
- construct cards, suggested actions and attachments.

## Speak .vs. display adaptation

By design, the .lg file format does not explicitly support the ability to provide speak .vs. display adaptation. The file format supports simple constructs that are composable and supports resolution on multi-line text and so you can have syntax and semantics for speak .vs. display adaptation, cards, suggested actions etc that can be interpreted as simple text and transformed into the Bot Framework [activity][1] by a layer above language generation.

Bot Builder SDK supports a short hand notation that can parse and transform a piece of text separated by `displayText`||`spokenText` into speak and display text.

```markdown
# greetingTemplate
- hi || hi there
- hello || hello, what can I help with today
```

You can use the `TextMessageActivityGenerator.CreateActityFromText` method to transform the command into a Bot Framework activity to post back to the user.

## Using Chatdown style cards

[Chatdown][6] introduced a simple markdown based way to write mock conversations. Also introduced as part of the [.chat][7] file format was the ability to express different [message commands][9] via simple text representation. Message commands include [cards][10], [Attachments][11] and suggested actions.

You can include message commands via multi-line text in the .lg file format and use the `TextMessageActivityGenerator.CreateActityFromText` method to transform the command into a Bot Framework activity to post back to the user.

See [here][8] for examples of how different card types are represented in .chat file format.

Here is an example of a card definition.

```markdown
    # HeroCardTemplate(buttonsCollection)
    - ```
    [Herocard
        title=@{[TitleText]}
        subtitle=@{[SubText]}
        text=@{[DescriptionText]}
        images=@{[CardImages]}
        buttons=@{join(buttonsCollection, '|')]
    ```

    # TitleText
    - Here are some [TitleSuffix]

    # TitleSuffix
    - cool photos
    - pictures
    - nice snaps

    # SubText
    - What is your favorite?
    - Don't they all look great?
    - sorry, some of them are repeats

    # DescriptionText
    - This is description for the hero card

    # CardImages
    - https://picsum.photos/200/200?image=100
    - https://picsum.photos/300/200?image=200
    - https://picsum.photos/200/200?image=400
```

## Language Generation in action

When building a bot, you can use language generation in several different ways. To start with, examine your current bot's code (or the new bot you plan to write) and create [.lg file][3] to cover all possible scenarios where you would like to use the language generation sub-system with your bot's replies to user.

Then make sure you include the platform specific language generation library.

For C#, add Microsoft.Bot.Builder.LanguageGeneration.
For NodeJS, add botbuilder-lg

Load the template manager with your .lg file(s)

For C#

```
    TemplateEngine lgEngine = new TemplateEngine().AddFiles(pathToLGFiles); 
```

For NodeJS
```
    let lgEngine = new TemplateEngine().addFiles(pathToLGFiles);

When you need template expansion, call the templateEngine and pass in the relevant template name

For C#

```c#
    await turnContext.SendActivityAsync(lgEngine.EvaluateTemplate("<TemplateName>", entitiesCollection));
```

For NodeJS

```node
    await turnContext.sendActivity(lgEngine.evaluateTemplate("<TemplateName>", entitiesCollection));
```

If your template needs specific entity values to be passed for resolution/ expansion, you can pass them in on the call to `evaluateTemplate`

For C#

```c#
    await turnContext.SendActivityAsync(lgEngine.EvaluateTemplate("WordGameReply", new { GameName = "MarcoPolo" } ));

```

For NodeJS

```node
    await turnContext.sendActivity(lgEngine.evaluateTemplate("WordGameReply", { GameName = "MarcoPolo" } ));
```

## Grammar check and correction

The current library does not include any capabilities for grammar check or correction.

## Packages

Packages for C# are available under the [BotBuilder MyGet feed][12]

[1]:https://github.com/Microsoft/BotBuilder/blob/master/specs/botframework-activity/botframework-activity.md
[2]:./docs/api-reference.md
[3]:./docs/lg-file-format.md
[6]:https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown
[7]:https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown#chat-file-format
[8]:https://github.com/Microsoft/botbuilder-tools/blob/master/packages/Chatdown/Examples/CardExamples.chat
[9]:https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown#message-commands
[10]:https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown#message-cards
[11]:https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown#message-attachments
[12]:https://botbuilder.myget.org/feed/botbuilder-declarative/package/nuget/Microsoft.Bot.Builder.LanguageGeneration

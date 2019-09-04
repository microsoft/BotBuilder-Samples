# Language generation

Adaptive dialogs natively support and work with the new Language Generation system. 

See [here][1] to learn more about Language Generation preview.

Once you have authored your .lg files for your Adaptive dialogs (or your bot project), you can simply register as middleware and use  `ILanguageGenerator` and `IMessageActivityGenerator` for full-blown generation support (including template resolution) anywhere in your bot.

All [sample projects][2] follow the exact same pattern.


In adapter
```C#
this.Use(new RegisterClassMiddleware<IMessageActivityGenerator>(new TextMessageActivityGenerator()));
```

In any Adaptive Dialog
```C#
var adaptiveDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
{ 
    Generator = new TemplateEngineLanguageGenerator(new TemplateEngine().AddFile(lgFilePath)),
    Rules = new List<IRule> () {
        // Rules
    }    
}
```

With this, you can simply refer to a LG template anywhere in your bot's response. 

``` C#
new SendActivity("[MyLGTemplateName]")
```

**Note:** By default, your bot's entire state is passed to the LG sub-system for resolution. So your teamplates can refer all properties that are available in memory just the way your code does.

``` markdown
# EchoTemplate
> The reference to {turn.Activity.Text} is valid in the LG template because bot state is passed in on all template evaluation calls.
- [EchoPrefix], "{turn.Activity.Text}"

# EchoPrefix
- I heard you say
- You said
- Roger
```

[1]:../../language-generation/language-generation.md
[2]:../csharp_dotnetcore



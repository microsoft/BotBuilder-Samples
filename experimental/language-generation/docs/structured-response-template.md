# Structured response template
Structured response template enable you to define a complex structure that supports all the goodness of LG (templating, composition, substitution) while leaving the interpretation of the structured response up to the caller of the LG library. 

For bot applications, we will natively support ability to - 
- activity definition
- card definition
- any [chatdown][1] style constructs

[Bot Framework activity][2] includes several fields that are of interest and might require the abilty for the user to customize or control but to start with the following are most widely used properties that should be configurable via an Activity template definition - 

| Property          | Use case                                                                                                                          |
|-------------------|-----------------------------------------------------------------------------------------------------------------------------------|
| Text              | Display text used by the channel to render visually                                                                               |
| Speak             | Spoken text used by the channel to render audibly                                                                                 |
| Attachments       | List of attachments with their type. Used by channels to render as UI cards or other generic file attachment types                |
| SuggestedActions  | List of actions rendered as suggestions to user.                                                                                  | 
| InputHint         | Controls audio capture stream state on devices that support spoken input. Possible values can be accepting, expecting, ignoring   |

There is no default fallback behavior implemented by the template resolver. If a property is not specified, then it remains un-specified. E.g. If only `Text` property is specified, the `Speak` property **is not** automatically assigned to be the `Text` property etc. 

# Definition
Here's the definition of a structured template - 

```markdown
# TemplateName
> this is a comment
[Structure-name
    Property1 = <plain text> .or. <plain text with template reference> .or. <expression> 
    Property2 = list of values are denoted via '|'. e.g. a | b
> this is a comment about this specific property
    Property3 = Nested structures are achieved through composition
]
```

Here's an example of a basic Text template composition: 

```markdown
# AskForAge.prompt
[Activity
    Text = @{GetAge()}
    Speak = @{GetAge()}
]

# GetAge
- how old are you?
- what is your age?
```

Here's an example of text with suggested action

```markdown
# AskForAge.prompt
[Activity
    Text = @{GetAge()}
    SuggestedActions = 10 | 20 | 30
]
```

Here's an example of a Hero card definition

```markdown
# HeroCard (params)
[Herocard   
    title = @{params.title}
    subtitle = Microsoft Bot Framework
    text = Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.
> @{templateName()} always returns only one resolution from the template.
> all(templateName) returns a list of all resolutions from the template.(TODO)
    images = @{all(HeroImages)}
    buttons = Show more cards
]

# HeroImages
- https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg
- https://sec.ch9.ms/ch9/buildreactionbotframework_akajhsdi7_.jpg
```

Here is a richer example that puts them all together including a hero card can be defined as below

```markdown
# AskForAge.prompt
[Activity
    Text = @{GetAge()}
    Speak = @{GetAge()}
    Attachments = @{HeroCard()}
    SuggestedActions = 10 | 20 | 30
    InputHint = expecting
]

# GetAge
- how old are you?
- what is your age?

# HeroCard (params)
[Herocard   
    title = @{params.title}
    subtitle = Microsoft Bot Framework
    text = Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.
    images = @{all(HeroImages)}
    buttons = Show more cards
]

# HeroImages
- https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg
- https://sec.ch9.ms/ch9/buildreactionbotframework_akajhsdi7_.jpg
```

By default any template reference is evaluated once during evaluation of a structured template. 

As an example, this returns the same resolution text for both `Speak` and `Text` properties. (TODO)

```markdown
# AskForAge.prompt
[Activity
    Text = @{GetAge()}
    Speak = @{GetAge()}
]

# GetAge
- how old are you?
- what is your age?
```

You can use the TemplateName!() (with trailing '!' after a template name) to request a new evaluation on each reference within a structured template.(TODO)

In this example, `Speak` and `Text` could come back with different resolutions because `GetAge` is re-evalauted on each instance.

```markdown
[Activity
    Text = @{GetAge()}
    Speak = @{GetAge!()}
]

# GetAge
- how old are you?
- what is your age?
```

Some times you might want to come back with a carousel of cards. Here's an example that achieves that - 

```markdown
# AskForAge.prompt
[Activity
> Defaults to carousel layout in case of list of cards
    Attachments = @{foreach($cardValues, item, HeroCard(item)}
]

# AskForAge.prompt_2
[Activity
> Explicitly specify an attachment layout
    Attachments = @{foreach($cardValues, item, HeroCard(item)}
    AttachmentLayout = list    
]

# HeroCard (title, subtitle, text)
[Herocard   
    title = @{title}
    subtitle = @{subtitle}
    text = @{text}
    images = @{all(HeroImages)}
    buttons = Show more cards
]

# HeroImages
- https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg
- https://sec.ch9.ms/ch9/buildreactionbotframework_akajhsdi7_.jpg
```

Use of '|' will make a definition a list. You can use '\\' as the escape character

```markdown
# AskForAge.prompt
[Activity
> With '|' you are making attachments a list. 
        Attachments = @{HeroCard()} |
> You can use '\' as an escape character
        Suggestions = 10 \\| cards | 20 \\| cards
]
```
# Structured template composition
The following composition behavior is supported with structured template - 

1. Composition will be structure context aware, if the target template being referred is also a structured template, then 
    - the structure type must match e.g. ActivityTemplate can be referred to in another ActivityTemplate etc. 
    - the specific property under holds the template reference must exist in the target template.
2. References to simple or conditional response template can exist anywhere inside a structured template. 

As an example, reference to another structured template in `Speak` property, will result in resolution against that specific property and would result in a `property not found in template` error if the property does not exist in the target structured template. 

Here's an example: With this template definitions,  

```markdown
# T1
[Activity
    Text = @{T2()}
    Speak = foo bar @{T3().speak}
]

# T2
- This is awesome

# T3
[Activity
    Speak = I can also speak!
]
```

Call to `evaluateTemplate('T1')` would result in the following internal structure: 

```markdown
[Activity
    Text = This is awesome
    Speak = I can also speak!
]
```

As another example, `evaluateTemplate('T4')` with the following definition will result in `Property 'Text' not found in structured template 'T5'`

```markdown
# T4
[Activity
    Text = @{T5()}
]

# T5
[Activity
    Speak = I can also speak!
]
```
# Full reference to another structured template
You can include reference to a full structured template 
    - as a property in another structured template
    - as a reference in another simple or conditional response template

Here is an example of this style of reference in action: 

```markdown
# ST1
[MyStruct
    Text = foo
    @{ST2()}
]
# ST2
[MyStruct
    Speak = bar
]
```

With this content, call to `evaluateTemplate('ST1')` will result in the following internal structure

```markdown
[MyStruct
    Text = foo
    Speak = bar
    
]
```

When the same property exists in both the calling template as well as callee, the content in caller will trump any content in the callee.

Here is an example:

```markdown
# ST1
[MyStruct
    Text = foo
    @{ST2()}
]
# ST2
[MyStruct
    Speak = bar
    Text = zoo
]
```

With this content, call to `evaluateTemplate('ST1')` will result in the following internal structure 

```markdown
[MyStruct
    Text = foo
    Speak = bar
]
```
Note that this style of composition can only exists at the root level. If there is a reference to another structured template within a property, then the resolution is contextual to that property. 
# Chatdown style content as structured activity template
It is a natural extension to also define full [chatdown][1] style templates using the structured template definition capability. This helps eliminate the need to always define chatdown style cards in a multi-line definition

## Existing chatdown style constructs supported
1. Typing
2. Suggestions
3. HeroCard
4. SigninCard
5. ThumbnailCard
6. AudioCard
7. VideoCard
8. AnimationCard
9. MediaCard
8. SigninCard
9. OAuthCard
10. Attachment
11. AttachmentLayout
12. [New] CardAction 
13. [New] AdaptiveCard
14. [New] EventActivity

## Improvements to chatown style constructs

### CardAction
```markdown
# CardAction (title, type, value)
[CardAction
> type can be 'openUrl', 'imBack', 'postBack', 'messageBack' 
    Type = @{if(type == null, 'imBack', type)}
> description that appears on button
    Title = @{title}
> payload to return as object.
    Value = @{value}
]
```

### Suggestions
Suggestions can now support a full blown CardAction structure. 

```markdown
# AskForColor
[Activity
    SuggestedActions = @{CardAction('red')} | @{CardAction('blue') | @{CardAction('See all choices', 'openUrl', 'http://contoso.com/color/choices')}}
]
```

### Adaptive card
Adaptive cards today are rendered via `[Attachment=cardpath.json adaptive]` notation. You can define adaptive cards inline and consume them via the `json()` function.


```markdown
    # GetColor.prompt
    [Activity
        Attachments = @{json(GetColor.adaptive.card())
    ]

    # GetColor.adaptive.card
    - ```json
    {
        // adaptive card definition
    }
    ```
```

### All card types
Buttons in any of the card types will also support full blown CardAction definition. 

Here's an example: 
```markdown
# HeroCardTemplate
[HeroCard
    title = BotFramework Hero Card
    subtitle = Microsoft Bot Framework
    text = Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.
    image = https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg
    buttons = {CardAction('Show more cards')} | {CardAction('See our library', 'postBack', 'http://contoso.com/cards/all')}
]
```

### Event activity
[Bot framework activity protocol][2] supports ability for bot to send a custom event activity to the client. We will add support for it via structured LG using the following definition. This should set the outgoing activities `type` property to `event` and can have a `name` property and a `value` property.

```markdown
[EventActivity
    name = some name
    value = some value
]
```


[1]:https://github.com/microsoft/botbuilder-tools/tree/master/packages/Chatdown
[2]:https://github.com/Microsoft/botframework-sdk/blob/master/specs/botframework-activity/botframework-activity.md
[3]:https://github.com/microsoft/BotBuilder-Samples/tree/master/experimental/common-expression-language
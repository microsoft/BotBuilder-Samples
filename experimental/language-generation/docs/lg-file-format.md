# .LG file format
.lg file will be a lot similar to the [.lu][1] file. As an overarching goal, we use simple markdown conventions as much as possible and add additional syntax and semantics only where needed. 

.lg files help describe language generation templates with entity references and their composition. The rest of this document covers the various concepts expressed via the .lg file format. See [here][2] for API-reference.

## Comments
Comments are prefixed with '>' character. All lines that have this prefix will be skipped by the parser. 

```markdown
> this is a comment.
```
## Escape character
- Use '\\' as escape character. E.g. "You can say cheese and tomato \\[toppings are optional\\]"

## Template 
At the core of rule based LG is the concept of a template. Each template has a 
- Name
- List of one-of variation text values .or. 
- A collection of conditions, each with a
    - Condition expression which is expressed using the [Common expression language][3] and 
    - List of one-of variation text values per condition.

Templates are defined via # \<TemplateName\> notation. Here is an example of a simple template that includes 2 variations. 
Templates follow the markdown header definition. Variations are expressed as markdown list; so you can prefix them using either '-' or '*' or '+'.

```markdown
> Greeting template with 2 variations. One of the variation is picked up by the template resolution runtime.
# GreetingPrefix
- Hi
- Hello
```
## Conditional response templates

### If..Else

Here are few examples of a conditional template. All conditions are expressed using the [Common expression language][3]. Condition expressions are enclosed in curly brackets - {}. Conditions are evaluated in the order specified via the IF ... ELSE or IF ... ELSEIF ... ELSE prefixes.

Here is an example that shows the simple IF ... ELSE conditional response template definition. 

<a name="conditional-response-template"></a>
```markdown
> time of day greeting reply template with conditions. 
# timeOfDayGreeting
- IF: {timeOfDay == 'morning'}
    - good morning
- ELSE: 
    - good evening
```

Here's another example that shows IF ... ELSEIF ... ELSE conditional response template definition. 

```markdown
# timeOfDayGreeting
- IF: {timeOfDay == 'morning'}
    - good morning
- ELSEIF: {timeOfDay == 'afternoon'}
    - good afternoon
- ELSE: 
    - good evening
```

### Switch..Case
Apart from IF ... ELSEIF ... ELSE construct, you can also use the SWITCH ... CASE ... DEFAULT construct. All conditions are expressed using the [Common expression language][3]. Condition expressions are enclosed in curly brackets - {}

Here's how you can specify SWITCH ... CASE block in LG. 

```markdown
# TestTemplate
SWITCH: {condition}
- CASE: {case-expression-1}
    - output1
- CASE: {case-expression-2}
    - output2
- DEFAULT:
   - final output
```

Here's an example:

```markdown
# greetInAWeek
SWITCH: {dayOfWeek(utcNow())}
- CASE: {0}
    - Happy Sunday!
-CASE: {6}
    - Happy Saturday!
-DEFAULT:  
    - Let's keep it up and work Hard!
```

### References to templates
Variation text can include references to another named template to aid with composition and resolution of sophisticated responses. 
Reference to another named template are denoted using markdown link notation by enclosing the target template name in square brackets - [TemplateName]. 

```markdown
> Example of a template that includes composition reference to another template
# GreetingReply
- [GreetingPrefix], [timeOfDayGreeting]

# GreetingPrefix
- Hi
- Hello

# timeOfDayGreeting
- IF: {timeOfDay == 'morning'}
    - good morning
- ELSEIF: {timeOfDay == 'afternoon'}
    - good afternoon
- ELSE: 
    - good evening
```

Calling the `GreetingReply` template can result in one of the following expansion resolutions - 

```
Hi, good morning
Hi, good afternoon
Hi, good evening
Hello, good morning
Hello, good afternoon
Hello, good evening
```

### Parametrization of templates
To aid with contextual re-usability, templates can be parametrized. With this different callers to the template can pass in different values for use in expansion resolution.

Here is an example of a template parametrization. 

```markdown
# timeOfDayGreetingTemplate (param1)
- IF: {param1 == 'morning'}
    - good morning
- ELSEIF: {param1 == 'afternoon'}
    - good afternoon
- ELSE: 
    - good evening

# morningGreeting
- timeOfDayGreetingTemplate('morning')

# timeOfDayGreeting
- timeOfDayGreetingTemplate(timeOfDay)
```

## Entities 
- When used directly within a one-of variation text, entity references are denoted by enclosing them in curly brackets - {`entityName`}
- When used as a parameter within a 
    - [pre-built function][4] or 
    - as a parameter to [template resolution call](#Parametrization-of-templates) or 
    - a condition in [conditional response template](#conditional-response-template)
they are simply expressed as `entityName`.

## Multi-line text in variations
Each one-of variation can include multi-line text enclosed in ```...```. 

Here is an example - 
```markdown
    # MultiLineExample
    - ```This is a multi-line list
        - one
        - two
        ```
    - ```This is a multi-line variation
        - three
        - four
    ```
```

Multi-line variation can request template expansion and entity substitution by enclosing the requested operation in @{}.

Here is an example - 
```markdown
# MultiLineExample
    - ```
        Here is what I have for the order
        - Title: @{reservation.title}
        - Location: @{reservation.location}
        - Date/ time: @{reservation.dateTimeReadBack}
    ```
```

With multi-line support, you can have the language generation sub-system fully resolve a complex JSON or XML (e.g. SSML wrapped text to control bot's spoken reply). 

Here is an example of complex object that your bot's code will parse out and render appropriately. 

```markdown
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

    # CardImages
    - https://picsum.photos/200/200?image=100
    - https://picsum.photos/300/200?image=200
    - https://picsum.photos/200/200?image=400

    # ImageGalleryTemplate
    - ```
    {
        "titleText": "@{[TitleText]}",
        "subTitle": "@{[SubText]}",
        "images": [
            {
            "type": "Image",
            "url": "@{[CardImages]}"
            },
            {
            "type": "Image",
            "url": "@{[CardImages]}"
            }
        ]
    }
    ```
```

Calling the `ImageGalleryTemplate` for template resolution will result in a json string that has a 'titleText', 'subTitle' and randomly selected images from the `CardImages` template.

## Using pre-built functions in variations
[Pre-built functions][4] supported by the [Common expression language][3] can also be used inline in a one-of variation text to achieve even more powerful text composition. To use an expression inline, simply wrap it in curly brackets - {}.

Here is an example that illustrates that - 

```markdown
# RecentTasks
- IF: {count(recentTasks) == 1}
    - Your most recent task is {recentTasks[0]}. You can let me know if you want to add or complete a task.
- ELSEIF: {count(recentTasks) == 2}
    - Your most recent tasks are {join(recentTasks, ',', 'and')}. You can let me know if you want to add or complete a task.
- ELSEIF: {count(recentTasks) > 2}
    - Your most recent {count(recentTasks)} tasks are {join(recentTasks, ',', 'and')}. You can let me know if you want to add or complete a task.
- ELSE:
    - You don't have any tasks.
```

The above example uses the [join][5] pre-built function to list all values in the `recentTasks` collection. 

## Importing external references
Often times for organization purposes and to help with re-usability, you might want to break the language generation templates into separate files and refer them from one another. In order to help with this scenario, you can use markdown-style links to import templates defined in another file. 

```markdown
[Link description](filePathOrUri)
```

Note: All templates defined in the target file will be pulled in. So please ensure that your template names are unique across files being pulled in. 

```markdown
[Shared](../shared/common.lg)
```

[1]:https://github.com/Microsoft/botbuilder-tools/blob/master/packages/Ludown/docs/lu-file-format.md
[2]:./api-reference.md
[3]:../../common-expression-language#readme
[4]:../../common-expression-language/prebuilt-functions.md
[5]:../../common-expression-language/prebuilt-functions.md#join
[6]:https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown
[7]:https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown#chat-file-format
[8]:https://github.com/Microsoft/botbuilder-tools/blob/master/packages/Chatdown/Examples/CardExamples.chat
[9]:https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown#message-commands
[10]:https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown#message-cards
[11]:https://github.com/Microsoft/botbuilder-tools/tree/master/packages/Chatdown#message-attachments

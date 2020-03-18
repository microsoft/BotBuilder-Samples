# .LG file format

.lg files help describe Language Generation templates with entity references and their composition. This document covers the various concepts expressed via the .lg file format.

**Concepts:**
- [.LG file format](#lg-file-format)
- [Comments](#comments)
- [Escape character](#escape-character)
- [Templates](#templates)
  - [Simple response template](#simple-response-template)
  - [Conditional response template](#conditional-response-template)
    - [If..Else](#ifelse)
    - [Switch..Case](#switchcase)
  - [Structured response template](#structured-response-template)
- [Template composition and expansion](#template-composition-and-expansion)
  - [References to templates](#references-to-templates)
  - [Entities](#entities)
  - [Using pre-built functions in variations](#using-pre-built-functions-in-variations)
  - [Multi-line text in variations](#multi-line-text-in-variations)
- [Parametrization of templates](#parametrization-of-templates)
- [Importing external references](#importing-external-references)
- [LG specific adaptive expression functions](#functions-injected-by-LG)

## Comments

Comments are prefixed with `>` character. All lines that have this prefix will be skipped by the parser.

```markdown
> this is a comment.
```

## Escape character

Use `\` as escape character.

```markdown
# TemplateName
- You can say cheese and tomato \[toppings are optional\]
```

## Templates

At the core of language generation system is the concept of a template. Each template has a name and one of the following:

- list of one-of variation text values
- structured content definition
- a collection of conditions, each with:
    - an [adaptive expression][3]
    - a list of one-of variation text values per condition.

Template names follow the markdown header definition

```markdown
# TemplateName
```

Variations are expressed as a Markdown list. You can prefix lists using the '-', '*', or '+' character.

```markdown
# Template1
- one
- two

# Template2
* one
* two

# Template3
+ one
+ two
```

### Simple response template

A simple response template includes one or more variations of text that must be used for composition and expansion. 

One of the variations provided will be picked at random by the LG library.

Here is an example of a simple template that includes two variations.

```markdown
> Greeting template with 2 variations.
# GreetingPrefix
- Hi
- Hello
```

### Conditional response template

Conditional response templates enable you to author content that is selecting based on a condition. All conditions are expressed using [adaptive expressions][3].

#### If Else

IF ... ELSEIF ... ELSE construct lets you to build a template that picks a collection based on a cascading order of conditions. Evaluation is top-down and stops when a condition evaluates to `true` or the ELSE block is hit.

Here is an example that shows the simple IF ... ELSE conditional response template definition.

```markdown
> time of day greeting reply template with conditions.
# timeOfDayGreeting
- IF: ${timeOfDay == 'morning'}
    - good morning
- ELSE:
    - good evening
```

Here is another example that shows IF ... ELSEIF ... ELSE conditional response template definition. Note that you can include references to other simple or conditional response templates in the variation for any of the conditions.

```markdown
# timeOfDayGreeting
- IF: ${timeOfDay == 'morning'}
    - ${morningTemplate()}
- ELSEIF: ${timeOfDay == 'afternoon'}
    - ${afternoonTemplate()}
- ELSE:
    - I love the evenings! Just saying. ${eveningTemplate()}
```

#### Switch..Case

The SWITCH ... CASE ... DEFAULT construct lets you design a conditional template that matches an expression's value to a case clause and produces output based on that case. Condition expressions are enclosed in curly brackets - ${}.

Here's how you can specify SWITCH ... CASE block in LG.

```markdown
# TestTemplate
- SWITCH: ${condition}
- CASE: ${case-expression-1}
    - output1
- CASE: ${case-expression-2}
    - output2
- DEFAULT:
   - final output
```

Here's a more complicated example:

```markdown
> Note: any of the cases can include reference to one or more templates
# greetInAWeek
- SWITCH: ${dayOfWeek(utcNow())}
- CASE: ${0}
    - Happy Sunday!
-CASE: ${6}
    - Happy Saturday!
-DEFAULT:  
    - ${apology-phrase()}, ${defaultResponseTemplate()}
```

### Structured response template

Structured response templates let you define a complex structure that supports major LG functionality, like templating, composition, and substitution, while leaving the interpretation of the structured response up to the caller of the LG library.

For bot applications, we natively support:

- activity definition
- card definition
- any [chatdown][12] style constructs

Read the [structure response templates](./structured-response-template.md) article for more information.

## Template composition and expansion

### References to templates

Variation text can include references to another named template to aid with composition and resolution of sophisticated responses. References to other named templates are denoted using _- ${TemplateName()}._

```markdown
> Example of a template that includes composition reference to another template
# GreetingReply
- ${GreetingPrefix()}, ${timeOfDayGreeting()}

# GreetingPrefix
- Hi
- Hello

# timeOfDayGreeting
- IF: ${timeOfDay == 'morning'}
    - good morning
- ELSEIF: ${timeOfDay == 'afternoon'}
    - good afternoon
- ELSE:
    - good evening
```

Calling the `GreetingReply` template can result in one of the following expansion resolutions:

```
Hi, good morning
Hi, good afternoon
Hi, good evening
Hello, good morning
Hello, good afternoon
Hello, good evening
```

## Entities

When used directly within a one-of variation text, entity references are denoted by enclosing them in curly brackets -  ${`entityName`}.

Entities are expressed as `entityName`. - e.g. ${entityName == null} - when used as a parameter:
    - within a [pre-built function][4]
    - within a condition in a [conditional response template](#conditional-response-template)
    - to [template resolution call](#Parametrization-of-templates)

## Using pre-built functions in variations

[Pre-built functions][4] supported by the [adaptive expressions][3] can also be used inline in a one-of variation text to achieve even more powerful text composition. To use an expression inline, simply wrap it in curly brackets - ${}.

```markdown
# RecentTasks
- IF: ${count(recentTasks) == 1}
    - Your most recent task is ${recentTasks[0]}. You can let me know if you want to add or complete a task.
- ELSEIF: ${count(recentTasks) == 2}
    - Your most recent tasks are ${join(recentTasks, ', ', ' and ')}. You can let me know if you want to add or complete a task.
- ELSEIF: ${count(recentTasks) > 2}
    - Your most recent ${count(recentTasks)} tasks are ${join(recentTasks, ', ', ' and ')}. You can let me know if you want to add or complete a task.
- ELSE:
    - You don't have any tasks.
```

The example uses the [join][5] pre-built function to list all values in the `recentTasks` collection.

Given templates and prebuilt functions share the same invocation signature, a template name cannot be the same as a prebuilt function name. See [here][4] for list of prebuilt functions. 

In cases where you must have the template name be the same as a pre-built function name you can use `lg.xxx`, as seen in the example below, to refer to the LG template and avoid possible collisions with pre-built functions.

```markdown
> Custom length function with one parameter.
# length(a)
- This is use's customized length function

# myfunc1
> will call prebuilt function length, and return 2
- ${length('hi')}

# mufunc2
> this calls the lg template and output 'This is use's customized length function'
- ${lg.length('hi')}
```

## Functions injected by LG
[Adaptive expressions][3] provide an ability to inject custom set of functions. Language generation takes advantage of this facility and adds [these functions][13]

## Multi-line text in variations
Each one-of variation can include multi-line text enclosed in triple quotes.

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

Multi-line variation can request template expansion and entity substitution by enclosing the requested operation in ${}.

```markdown
# MultiLineExample
    - ```
        Here is what I have for the order
        - Title: ${reservation.title}
        - Location: ${reservation.location}
        - Date/ time: ${reservation.dateTimeReadBack}
    ```
```

With multi-line support, you can have the language generation sub-system fully resolve a complex JSON or XML (e.g. SSML wrapped text to control bot's spoken reply).

## Parametrization of templates

To aid with contextual re-usability, templates can be parametrized. Different callers to the template can pass in different values for use in expansion resolution.

```markdown
# timeOfDayGreetingTemplate (param1)
- IF: ${param1 == 'morning'}
    - good morning
- ELSEIF: ${param1 == 'afternoon'}
    - good afternoon
- ELSE: 
    - good evening

# morningGreeting
- ${timeOfDayGreetingTemplate('morning')}

# timeOfDayGreeting
- ${timeOfDayGreetingTemplate(timeOfDay)}
```

## Importing external references

You may want to break the language generation templates into separate files and refer them from one another for organizational and reusability purposes. You can use Markdown-style links to import templates defined in another file.

```markdown
[Link description](filePathOrUri)
```

All templates defined in the target file will be pulled in. Ensure that your template names are unique (or namespaced via a # <namespace>.<templatename> convention) across files being pulled in.

```markdown
[Shared](../shared/common.lg)
```

## Additional Resources

- Language Generation [API reference][2]


[1]:https://github.com/Microsoft/botbuilder-tools/blob/master/packages/Ludown/docs/lu-file-format.md
[2]:./api-reference.md
[3]:../../common-expression-language#readme
[4]:../../common-expression-language/prebuilt-functions.md
[5]:../../common-expression-language/prebuilt-functions.md#join
[6]:https://github.com/microsoft/botframework-cli/tree/master/packages/chatdown
[7]:https://github.com/microsoft/botframework-cli/blob/master/packages/chatdown/docs/chatdown-format.md
[8]:https://github.com/microsoft/botframework-cli/blob/master/packages/chatdown/docs/examples/CardExamples.chat
[9]:https://github.com/microsoft/botframework-cli/blob/master/packages/chatdown/docs/chatdown-format.md#message-commands
[10]:https://github.com/microsoft/botframework-cli/blob/master/packages/chatdown/docs/chatdown-format.md#message-cards
[11]:https://github.com/microsoft/botframework-cli/blob/master/packages/chatdown/docs/chatdown-format.md#message-attachments
[12]:https://github.com/microsoft/botframework-cli/blob/master/packages/chatdown/docs/chatdown-format.md
[13]:./Functions-injected-from-LG.md

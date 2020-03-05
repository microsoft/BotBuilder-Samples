# Adaptive expressions ***_[PREVIEW]_***

Bots use expressions to evaluate the outcome of a condition based on runtime information available in memory to the dialog or the [Language Generation](../language-generation) system. These evaluations determine how your bot reacts to user input and other factors that impact bot functionality.

Adaptive expressions were created to address this core need as well as provide an adaptive expression language that can used with the Bot Framework SDK and other conversational AI components, like [Bot Framework Composer](https://github.com/microsoft/BotFramework-Composer#microsoft-bot-framework-composer-preview), [Language Generation](../language-generation), [Adaptive dialogs](../adaptive-dialog), and [Adaptive Cards](https://docs.microsoft.com/adaptive-cards/).

An adaptive expression can contain one or more explicit values, [pre-built functions](./prebuilt-functions.md) or [custom functions](./extend-functions.md). Consumers of adaptive expressions also have the capability to inject additional supported functions. For example, all Language Generation templates are available as functions as well as additional functions that are only available within that component's use of adaptive expressions.

## Operators

### Arithmetic operators

| Operator    |                                  Functionality                                            |   Prebuilt function equivalent    |
|-----------|-------------------------------------------------------------------------------------------|-----------------------------------|
|+          | Addition. E.g. A + B                                                    |[add][1]                           |
|-            | Subtraction. E.g. A – B                                                |[sub][2]                           |
|unary +    | Positive value E.g. +1, +A                                                    |N/A                                |
|unary -    | Negative value E.g. –2, -B                                            |N/A                                |
|*            | Multiplication. E.g. A * B                                            |[mul][3]                           |
|/            | Division. E.g. A / B                                                    |[div][4]                           |
|^            | Exponentiation. E.g. A ^ B                                            |[exp][5]                           |
|%            | Modulus. E.g. A % B                                                    |[mod][6]                           |

### Comparison operators

| Operator    |                                  Functionality                                            |   Prebuilt function equivalent    |
|-----------|-------------------------------------------------------------------------------------------|-----------------------------------|
|==            | Equals. E.g. A == B                                                    |[equals][7]                        |
|!=            | Not equals. E.g. A != B                                                |[not][8]([equals][7]())            |
|>            | Greater than. A > B                                                    |[greater][9]                       |
|<            | Less than. A < B                                                        |[less][10]                         |
|>=         | Greater than or equal. A >= B                                        |[greaterOrEquals][11]              |
|<=            | Less than or equal. A <= B                                            |[lessOrEquals][12]                 |

### Logical operators

| Operator    |                                  Functionality                                            |   Prebuilt function equivalent    |
|-----------|-------------------------------------------------------------------------------------------|-----------------------------------|
|&&            |And. E.g. exp1 && exp2                                                    |[and][13]                          |
|\|\|        |Or. E.g. exp1 \|\| exp2                                                    |[or][14]                           |
|!            |Not. E.g. !exp1                                                            |[not][8]                           |


### Other operators and expression syntax

| Operator    |                                  Functionality                                            |   Prebuilt function equivalent    |
|-----------|-------------------------------------------------------------------------------------------|-----------------------------------|
|&, +            |Concatenation operators. Operands will always be cast to string – E.g. A & B, 'foo' + ' bar' => 'foo bar', 'foo' + 3 => 'foo3', 'foo' + (3 + 3) => 'foo6'                |N/A                                |
|'            |Used to wrap a string literal. E.g. 'myValue'                                                |N/A                                |
|"            |Used to wrap a string literal. E.g. "myValue"                                                |N/A                                |
|[]            |Used to refer to an item in a list by its index. E.g. A[0]                                    |N/A                                |
|${}        |Used to denote an expression. E.g. ${A == B}.                                              |N/A                                |
|${}        |Used to denote a variable in template expansion. E.g. ${myVariable}                        |N/A                                |
|()            |Enforces precedence order and groups sub expressions into larger expressions. E.g. (A+B)*C    |N/A                                |
|.            |Property selector. E.g. myObject.Property1                                                    |N/A                                |
|\            |Escape character for templates, expressions.                                               |N/A                                |

## Variables

Variables are always referenced by their name in the format `${myVariable}`. Variables can be referenced either using the property selector operator in the form of `myParent.myVariable`, using the item index selection operator like in `myParent.myList[0]`, or using the [getProperty](./prebuilt-functions.md#getProperty) function. 

There are two special variables, `[]` and  `{}`. `[]` represents an empty list and `{}` represents a empty object.

## Explicit values

Explicit values can be enclosed in either single quotes 'myExplicitValue' or double quotes "myExplicitValue".

## Additional resources

- [NuGet AdaptiveExpressions](https://www.nuget.org/packages/AdaptiveExpressions) package for C#
- [npm adaptive-expressions](https://www.npmjs.com/package/adaptive-expressions) package for Javascript
- [API reference](./api-reference.md) for Adaptive Expressions
- [Pre-built functions](./prebuilt-functions.md) supported by the Adaptive Expressions library
- [Extend functions](./extend-functions.md)

<!--
## Change Log
### 4.6 PREVIEW
- Added 50+ new [prebuilt functions](prebuilt-functions.md)

### 4.5 PREVIEW
- Initial preview release
-->
[1]:prebuilt-functions.md#add
[2]:prebuilt-functions.md#sub
[3]:prebuilt-functions.md#mul
[4]:prebuilt-functions.md#div
[5]:prebuilt-functions.md#exp
[6]:prebuilt-functions.md#mod
[7]:prebuilt-functions.md#equals
[8]:prebuilt-functions.md#not
[9]:prebuilt-functions.md#greater
[10]:prebuilt-functions.md#less
[11]:prebuilt-functions.md#greaterOrEquals
[12]:prebuilt-functions.md#essOrEquals
[13]:prebuilt-functions.md#and
[14]:prebuilt-functions.md#or
[15]:https://botbuilder.myget.org/feed/botbuilder-declarative/package/nuget/Microsoft.Bot.Builder.Expressions
[20]:https://github.com/microsoft/BotBuilder-Samples/blob/master/experimental/language-generation/README.md

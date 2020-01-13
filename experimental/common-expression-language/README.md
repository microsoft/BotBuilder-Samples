# Common Expression Language ***_[PREVIEW]_***

> See [here](#Change-Log) for what's new in 4.6 PREVIEW release.

Bots, like any other application, require use of expressions to evaluate outcome of a condition based on runtime information available in memory or to the dialog or the language generation system. 

Common expression language was put together to address this core need as well as to rationalize and snap to a common expression language that will be used across Bot Framework SDK and other conversational AI components that need an expression language.

See [here](./api-reference.md) for API reference.

***_An expression is a sequence that can contain one or more [operators](#Operators), [variables](#Variables), [explicit values](#Explicit-values), [pre-built functions](./prebuilt-functions.md) or [Language Generation templates](../fileformats/lg/README.md#Template)._***

## Operators

| Operator	|                                  Functionality                                            |   Prebuilt function equivalent    |
|-----------|-------------------------------------------------------------------------------------------|-----------------------------------|
|+          |Arithmetic operator – addition. E.g. A + B	                                                |[add][1]                           |
|-	        |Arithmetic operator – subtraction. E.g. A – B	                                            |[sub][2]                           |
|unary +    |Arithmetic operator – positive E.g. +1, +A	                                                |N/A                                |
|unary -	|Arithmetic operator – negative value E.g. –2, -B	                                        |N/A                                |
|*	        |Arithmetic operator – multiplication. E.g. A * B	                                        |[mul][3]                           |
|/	        |Arithmetic operator – division. E.g. A / B	                                                |[div][4]                           |
|^	        |Arithmetic operator – exponentiation. E.g. A ^ B	                                        |[exp][5]                           |
|%	        |Arithmetic operator – modulus. E.g. A % B	                                                |[mod][6]                           |
|==	        |Comparison operator – equals. E.g. A == B	                                                |[equals][7]                        |
|!=	        |Comparison operator – Not equals. E.g. A != B	                                            |[not][8]([equals][7]())            |
|>	        |Comparison operator – Greater than. A > B	                                                |[greater][9]                       |
|<	        |Comparison operator – Less than. A < B	                                                    |[less][10]                         |
|>= 	    |Comparison operator – Greater than or equal. A >= B	                                    |[greaterOrEquals][11]              |
|<=	        |Comparison operator – Less than or equal. A <= B	                                        |[lessOrEquals][12]                 |
|&	        |Concatenation operator. Operands will always be cast to string – E.g. A & B	            |N/A                                |
|&&	        |Logical operator – AND. E.g. exp1 && exp2	                                                |[and][13]                          |
|\|\|	    |Logical operator – OR. E.g. exp1 \|\| exp2	                                                |[or][14]                           |
|!	        |Logical operator – NOT. E.g. !exp1	                                                        |[Not][8]                           |
|'	        |Used to wrap a string literal. E.g. 'myValue'	                                            |N/A                                |
|"	        |Used to wrap a string literal. E.g. "myValue"	                                            |N/A                                |
|[]	        |Used to refer to an item in a list by its index. E.g. A[3]	                                |N/A                                |
|@{}	    |Used to denote an expression. E.g. @{A == B}.                                              |N/A                                |
|@{}	    |Used to denote a variable in template expansion. E.g. @{myVariable}	                    |N/A                                |
|()	        |Enforces precedence order and groups sub expressions into larger expressions. E.g. (A+B)*C	|N/A                                |
|.	        |Property selector. E.g. myObject.Property1	                                                |N/A                                |
|\	        |Escape character for templates, expressions.                                               |N/A                                |

## Variables
Variables are always referenced by their name. E.g. @{myVariable}
Variables can be complex objects. In which case they are referenced either using the property selector operator e.g. myParent.myVariable or using the item index selection operator. E.g. myParent.myList[0]. or using the [parameters](TODO) function. 

There are two special variables, `[]` and  `{}`.
`[]` represents an empty list, `{}` represents a empty object.

## Explicit values
Explicit values are enclosed in single quotes 'myExplicitValut' or double quotes - "myExplicitValue".

## Pre-built functions
See [Here](./prebuilt-functions.md) for a complete list of prebuilt functions supported by the common expression language library. 

## Packages
Packages for C# are available under the [BotBuidler MyGet feed][15]

## Change Log
### 4.6 PREVIEW
- Added 50+ new [prebuilt functions](prebuilt-functions.md)

### 4.5 PREVIEW
- Initial preview release

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

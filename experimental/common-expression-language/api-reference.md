# API reference for Expression

For Nuget packages, see [this MyGet C# feed][1] and [this MyGet js feed][2]

### ExpressionEngine Class

#### Constructors
```C#
/// <summary>
/// Constructor
/// </summary>
/// <param name="lookup">If present delegate to lookup evaluation information from type string.</param>
public ExpressionEngine(EvaluatorLookup lookup = null)
```
#### Methods
```C#
/// <summary>
/// Parse the input into an expression.
/// </summary>
/// <param name="expression">Expression to parse.</param>
/// <returns>Expresion tree.</returns>
public Expression Parse(string expression)
```

### Expression Class

#### Fields
```C#
/// <summary>
/// Type of expression.
/// </summary>
public string Type { get; }

/// <summary>
/// Evaluator of this expression
/// </summary>
public ExpressionEvaluator Evaluator { get; }

/// <summary>
/// Children expressions.
/// </summary>
public Expression[] Children { get; set; }

/// <summary>
/// Expected result of evaluating expression.
/// </summary>
public ReturnType ReturnType { get; }
```

#### Contructor
```C#
/// <summary>
/// Expression constructor.
/// </summary>
/// <param name="type">Type of expression from <see cref="ExpressionType"/>.</param>
/// <param name="evaluator">Information about how to validate and evaluate expression.</param>
/// <param name="children">Child expressions.</param>
public Expression(string type, ExpressionEvaluator evaluator = null, params Expression[] children)
```

#### Methods

```C#
/// <summary>
/// Evaluate the expression.
/// </summary>
/// <param name="state">
/// Global state to evaluate accessor expressions against.  Can be <see cref="IDictionary{String}{Object}"/>, <see cref="IDictionary"/> otherwise reflection is used to access property and then indexer.
/// </param>
/// <returns>Computed value and an error string.  If the string is non-null, then there was an evaluation error.</returns>
public (object value, string error) TryEvaluate(object state)
```

```C#
/// <summary>
/// Evaluate the expression.
/// </summary>
/// <param name="state">
/// Global state to evaluate accessor expressions against.  Can be <see cref="IDictionary{String}{Object}"/>, <see cref="IDictionary"/> otherwise reflection is used to access property and then indexer.
/// </param>
/// <returns>Computed value and an error string.  If the string is non-null, then there was an evaluation error.</returns>
public (object value, string error) TryEvaluate(IMemory state)
```


```C#
/// <summary>
/// Evaluate the expression.
/// </summary>
/// <typeparam name="T">type of result of the expression.</typeparam>
/// <param name="state">
/// Global state to evaluate accessor expressions against.  Can be <see cref="IDictionary{String}{Object}"/>, <see cref="IDictionary"/> otherwise reflection is used to access property and then indexer.
/// </param>
/// <returns>Computed value and an error string.  If the string is non-null, then there was an evaluation error.</returns>
public (T value, string error) TryEvaluate<T>(object state)
```


```C#
/// <summary>
/// Evaluate the expression.
/// </summary>
/// <typeparam name="T">type of result of the expression.</typeparam>
/// <param name="state">
/// Global state to evaluate accessor expressions against.  Can be <see cref="IDictionary{String}{Object}"/>, <see cref="IDictionary"/> otherwise reflection is used to access property and then indexer.
/// </param>
/// <returns>Computed value and an error string.  If the string is non-null, then there was an evaluation error.</returns>
public (T value, string error) TryEvaluate<T>(IMemory state)
```

[1]:https://botbuilder.myget.org/feed/botbuilder-v4-dotnet-daily/package/nuget/Microsoft.Bot.Expressions
[2]:https://botbuilder.myget.org/feed/botbuilder-v4-js-daily/package/npm/botframework-expressions
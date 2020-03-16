# API reference for Expression

For Nuget packages, see [this MyGet C# feed][1] and [this MyGet js feed][2]

### Expression Class

#### Constructors
```C#
/// <summary>
/// Initializes a new instance of the <see cref="Expression"/> class.
/// Built-in expression constructor.
/// </summary>
/// <param name="type">Type of built-in expression from <see cref="ExpressionType"/>.</param>
/// <param name="children">Child expressions.</param>
public Expression(string type, params Expression[] children)

/// <summary>
/// Initializes a new instance of the <see cref="Expression"/> class.
/// Expression constructor.
/// </summary>
/// <param name="evaluator">Information about how to validate and evaluate expression.</param>
/// <param name="children">Child expressions.</param>
public Expression(ExpressionEvaluator evaluator, params Expression[] children)
```
#### Methods
```C#
/// <summary>
/// Parse an expression string into an expression object.
/// </summary>
/// <param name="expression">expression string.</param>
/// <param name="lookup">Optional function lookup when parsing the expression. Default is Expression.Lookup which uses Expression.Functions table.</param>
/// <returns>expression object.</returns>
public static Expression Parse(string expression, EvaluatorLookup lookup = null)

/// <summary>
/// Lookup a ExpressionEvaluator (function) by name.
/// </summary>
/// <param name="functionName">function name.</param>
/// <returns>ExpressionEvaluator.</returns>
public static ExpressionEvaluator Lookup(string functionName)

/// <summary>
/// Evaluate the expression.
/// </summary>
/// <param name="state">
/// Global state to evaluate accessor expressions against.  Can be <see cref="System.Collections.Generic.IDictionary{String, Object}"/>,
/// <see cref="System.Collections.IDictionary"/> otherwise reflection is used to access property and then indexer.
/// </param>
/// <returns>Computed value and an error string.  If the string is non-null, then there was an evaluation error.</returns>
public (object value, string error) TryEvaluate(object state)


/// <summary>
/// Evaluate the expression.
/// </summary>
/// <param name="state">
/// Global state to evaluate accessor expressions against.  Can be <see cref="System.Collections.Generic.IDictionary{String, Object}"/>,
/// <see cref="System.Collections.IDictionary"/> otherwise reflection is used to access property and then indexer.
/// </param>
/// <returns>Computed value and an error string.  If the string is non-null, then there was an evaluation error.</returns>
public (object value, string error) TryEvaluate(IMemory state)

/// <summary>
/// Evaluate the expression.
/// </summary>
/// <typeparam name="T">type of result of the expression.</typeparam>
/// <param name="state">
/// Global state to evaluate accessor expressions against.  Can be <see cref="System.Collections.Generic.IDictionary{String, Object}"/>,
/// <see cref="System.Collections.IDictionary"/> otherwise reflection is used to access property and then indexer.
/// </param>
/// <returns>Computed value and an error string.  If the string is non-null, then there was an evaluation error.</returns>
public (T value, string error) TryEvaluate<T>(object state)

/// <summary>
/// Evaluate the expression.
/// </summary>
/// <typeparam name="T">type of result of the expression.</typeparam>
/// <param name="state">
/// Global state to evaluate accessor expressions against.  Can be <see cref="System.Collections.Generic.IDictionary{String, Object}"/>,
/// <see cref="System.Collections.IDictionary"/> otherwise reflection is used to access property and then indexer.
/// </param>
/// <returns>Computed value and an error string.  If the string is non-null, then there was an evaluation error.</returns>
public (T value, string error) TryEvaluate<T>(IMemory state)
```

#### Fields
``` C#
/// <summary>
/// Gets type of expression.
/// </summary>
/// <value>
/// Type of expression.
/// </value>
public string Type => Evaluator.Type;

/// <summary>
/// Gets expression evaluator.
/// </summary>
/// <value>
/// expression evaluator.
/// </value>
public ExpressionEvaluator Evaluator { get; }

/// <summary>
/// Gets or sets children expressions.
/// </summary>
/// <value>
/// Children expressions.
/// </value>
public Expression[] Children { get; set; }

/// <summary>
/// Gets expected result of evaluating expression.
/// </summary>
/// <value>
/// Expected result of evaluating expression.
/// </value>
public ReturnType ReturnType => Evaluator.ReturnType;
```

[1]:https://botbuilder.myget.org/feed/botbuilder-v4-dotnet-daily/package/nuget/AdaptiveExpressions
[2]:https://botbuilder.myget.org/feed/botbuilder-v4-js-daily/package/npm/adaptive-expressions
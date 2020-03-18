# API reference for Expression

For Nuget packages, see [this MyGet C# feed][1] and [this MyGet js feed][2]

## Expression Class

### Constructors
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
### Methods
```C#
/// <summary>
/// Parse an expression string into an expression object.
/// </summary>
/// <param name="expression">expression string.</param>
/// <param name="lookup">Optional function lookup when parsing the expression. Default is Expression.Lookup which uses Expression.Functions table.</param>
/// <returns>expression object.</returns>
public static Expression Parse(string expression, EvaluatorLookup lookup = null);

/// <summary>
/// Lookup a ExpressionEvaluator (function) by name.
/// </summary>
/// <param name="functionName">function name.</param>
/// <returns>ExpressionEvaluator.</returns>
public static ExpressionEvaluator Lookup(string functionName);

/// <summary>
/// Make an expression and validate it.
/// </summary>
/// <param name="type">Type of expression from <see cref="ExpressionType"/>.</param>
/// <param name="children">Child expressions.</param>
/// <returns>New expression.</returns>
public static Expression MakeExpression(string type, params Expression[] children);

/// <summary>
/// Make an expression and validate it.
/// </summary>
/// <param name="evaluator">Information about how to validate and evaluate expression.</param>
/// <param name="children">Child expressions.</param>
/// <returns>New expression.</returns>
public static Expression MakeExpression(ExpressionEvaluator evaluator, params Expression[] children);

/// <summary>
/// Construct an expression from a <see cref="EvaluateExpressionDelegate"/>.
/// </summary>
/// <param name="function">Function to create an expression from.</param>
/// <returns>New expression.</returns>
public static Expression LambaExpression(EvaluateExpressionDelegate function);

/// <summary>
/// Construct an expression from a lambda expression over the state.
/// </summary>
/// <remarks>Exceptions will be caught and surfaced as an error string.</remarks>
/// <param name="function">Lambda expression to evaluate.</param>
/// <returns>New expression.</returns>
public static Expression Lambda(Func<object, object> function);

/// <summary>
/// Construct and validate an Set a property expression to a value expression.
/// </summary>
/// <param name="property">property expression.</param>
/// <param name="value">value expression.</param>
/// <returns>New expression.</returns>
public static Expression SetPathToValue(Expression property, Expression value);

/// <summary>
/// Construct and validate an Set a property expression to a value expression.
/// </summary>
/// <param name="property">property expression.</param>
/// <param name="value">value object.</param>
/// <returns>New expression.</returns>
public static Expression SetPathToValue(Expression property, object value);

/// <summary>
/// Construct and validate an Equals expression.
/// </summary>
/// <param name="children">Child clauses.</param>
/// <returns>New expression.</returns>
public static Expression EqualsExpression(params Expression[] children);

/// <summary>
/// Construct and validate an And expression.
/// </summary>
/// <param name="children">Child clauses.</param>
/// <returns>New expression.</returns>
public static Expression AndExpression(params Expression[] children);

/// <summary>
/// Construct and validate an Or expression.
/// </summary>
/// <param name="children">Child clauses.</param>
/// <returns>New expression.</returns>
public static Expression OrExpression(params Expression[] children);

/// <summary>
/// Construct and validate a Not expression.
/// </summary>
/// <param name="child">Child clauses.</param>
/// <returns>New expression.</returns>
public static Expression NotExpression(Expression child);

/// <summary>
/// Construct a constant expression.
/// </summary>
/// <param name="value">Constant value.</param>
/// <returns>New expression.</returns>
public static Expression ConstantExpression(object value);

/// <summary>
/// Construct and validate a property accessor.
/// </summary>
/// <param name="property">Property to lookup.</param>
/// <param name="instance">Expression to get instance that contains property or null for global state.</param>
/// <returns>New expression.</returns>
public static Expression Accessor(string property, Expression instance = null);

/// <summary>
/// Validate immediate expression.
/// </summary>
public void Validate();

/// <summary>
/// Recursively validate the expression tree.
/// </summary>
public void ValidateTree();

/// <summary>
/// Evaluate the expression.
/// </summary>
/// <param name="state">
/// Global state to evaluate accessor expressions against.  Can be <see cref="System.Collections.Generic.IDictionary{String, Object}"/>,
/// <see cref="System.Collections.IDictionary"/> otherwise reflection is used to access property and then indexer.
/// </param>
/// <returns>Computed value and an error string.  If the string is non-null, then there was an evaluation error.</returns>
public (object value, string error) TryEvaluate(object state);

/// <summary>
/// Evaluate the expression.
/// </summary>
/// <param name="state">
/// Global state to evaluate accessor expressions against.  Can be <see cref="System.Collections.Generic.IDictionary{String, Object}"/>,
/// <see cref="System.Collections.IDictionary"/> otherwise reflection is used to access property and then indexer.
/// </param>
/// <returns>Computed value and an error string.  If the string is non-null, then there was an evaluation error.</returns>
public (object value, string error) TryEvaluate(IMemory state);

/// <summary>
/// Evaluate the expression.
/// </summary>
/// <typeparam name="T">type of result of the expression.</typeparam>
/// <param name="state">
/// Global state to evaluate accessor expressions against.  Can be <see cref="System.Collections.Generic.IDictionary{String, Object}"/>,
/// <see cref="System.Collections.IDictionary"/> otherwise reflection is used to access property and then indexer.
/// </param>
/// <returns>Computed value and an error string.  If the string is non-null, then there was an evaluation error.</returns>
public (T value, string error) TryEvaluate<T>(object state);

/// <summary>
/// Evaluate the expression.
/// </summary>
/// <typeparam name="T">type of result of the expression.</typeparam>
/// <param name="state">
/// Global state to evaluate accessor expressions against.  Can be <see cref="System.Collections.Generic.IDictionary{String, Object}"/>,
/// <see cref="System.Collections.IDictionary"/> otherwise reflection is used to access property and then indexer.
/// </param>
/// <returns>Computed value and an error string.  If the string is non-null, then there was an evaluation error.</returns>
public (T value, string error) TryEvaluate<T>(IMemory state);
```
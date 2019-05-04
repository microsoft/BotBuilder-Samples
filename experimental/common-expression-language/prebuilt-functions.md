# Common Expression Language
## Pre-built functions 
This document lists the available prebuilt functions ordered by their general purpose,
or you can browse the functions based on [alphabetical order](#alphabetical-list).

- [String functions](#String-functions)
- [Collection functions](#Collection-functions)
- [Logical comparison functions](#Logical-comparison-functions)
- [Conversion functions](#Conversion-functions)
- [Math functions](#Math-functions)
- [Date and time functions](#Date-and-time-functions)

### String functions
|Function	|Explanation|
|-----------|-----------|
|[length](#length)| Returns the length of a string |
|[replace](#replace)|	Replace a substring with the specified string, and return the updated string. case sensitive|
|[replaceIgnoreCase](#replaceIgnoreCase)|	Replace a substring with the specified string, and return the updated string. Case in-sensitive	|
|[split](#split)	|Returns an array that contains substrings based on the delimiter specified.|
|[substring](#substring)	|Returns characters from a string. Substring(sourceString, startPos, endPos). startPos cannot be less than 0. endPos greater than source strings length will be taken as the max length of the string	|
|[toLower](#toLower)	|Convert a string to all upper case characters	|
|[toUpper](#toUpper)	|Convert a string to all lower case characters	|
|[trim](#trim)	|Remove leading and trailing white spaces from a string	|


### Collection functions
|Function	|Explanation|
|-----------|-----------|
|[contains](#contains)	|Works to find an item in a string or to find an item in an array or to find a parameter in a complex object. E.g. contains(‘hello world, ‘hello); contains([‘1’, ‘2’], ‘1’); contains({“foo”:”bar”}, “foo”)	|
|[empty](#empty)	|Check if the collection is empty	|
|[first](#first)	|Returns the first item from the collection	|
|[join](#join) 	|Return a string that has all the items from an array and has each character separated by a delimiter. Join(collection, delimiter). Join(createArray(‘a’,’b’), ‘.’) = “a.b”	|
|[last](#last) 	|Returns the last item from the collection	|
|[count](#count)	|Returns the number of items in the collection	|
|[foreach](#foreach)	|Operate on each element and return the new collection	|


### Logical comparison functions
|Function	|Explanation|
|-----------|-----------|
|[and](#and)	|Logical and. Returns true if all specified expressions evaluate to true.	|
|[equals](#equals)	|Comparison equal. Returns true if specified values are equal	|
|[greater](#greater)	|Comparison greater than	|
|[greaterOrEquals](#greaterOrEquals)	| Comparison greater than or equal to. greaterOrEquals(exp1, exp2)	|
|[if](#if)	| if(exp, valueIfTrue, valueIfFalse)	|
|[less](#less)	|	Comparison less than opearation|
|[lessOrEquals](#lessOrEquals)	|	Comparison less than or equal operation|
|[not](#not)	|	Logical not opearator|
|[or](#or)	| Logical OR opearation.	|
|[exists](#exists) | Evaluates an expression for truthiness |

### Conversion functions
|Function	|Explanation|
|-----------|-----------|
|[float](#float)	|Return floating point representation of the specified string or the string itself if conversion is not possible	|
|[int](#int)	|Return integer representation of the specified string or the string itself if conversion is not possible	|
|[string](#string)	|Return string version of the specified value	|
|[bool](#bool)	|Return Boolean representation of the specified string. Bool(‘true’), bool(1)	|
|[createArray](#createArray)	|Create an array from multiple inputs	|

### Math functions
|Function	|Explanation|
|-----------|-----------|
|[add](#add)	|Mathematical and. Accepts two parameters	|
|[div](#div)	|Mathematical division	|
|[max](#max)	|Returns the largest value from a collection	|
|[min](#min)	|Returns the smallest value from a collection	|
|[mod](#mod)	|Returns remainder from dividing two numbers	|
|[mul](#mul)	|Mathematical multiplication	|
|[rand](#rand)	|Returns a random number between specified min and max value – rand(\<minValue\>, \<maxValue\>)	|
|[sub](#sub)	|Mathematical subtraction	|
|[sum](#sum)	|Returns sum of numbers in an array	|
|[exp](#exp)	|Exponentiation function. Exp(base, exponent)	|

### Date and time functions
|Function	|Explanation|
|-----------|-----------|
|[addDays](#addDays)	|Add number of specified days to a given timestamp	|
|[addHours](#addHours)	|Add specified number of hours to a given timestamp	|
|[addMinutes](#addMinutes)	|Add specified number of minutes to a given timestamp	|
|[addSeconds](#addSeconds)	|Add specified number of seconds to a given timestamp	|
|[dayOfMonth](#dayOfMonth)	|Returns day of month for a given timestamp or timex expression.	|
|[dayOfWeek](#dayOfWeek)	|Returns day of the week for a given timestamp	|
|[dayOfYear](#dayOfYear)	|Returns day of the year for a given timestamp	|
|[formatDateTime](#formatDateTime)	|Return a timestamp in the specified format.|
|[subtractFromTime](#subtractFromTime)	|Subtract a number of time units from a timestamp.|
|[utcNow](#utcNow)	|Returns current timestamp as string	|
|[dateReadBack](#dateReadBack)	|Uses the date-time library to provide a date readback. dateReadBack(currentDate, targetDate). E.g. dateReadBack(‘2016/05/30’,’2016/05/23’)=>"Yesterday"	|
|month	|Returns the month of given timestamp	|
|date	|Returns date for a given timestamp	|
|year	|Returns year for the given timestamp	|
|getTimeOfDay	|Returns time of day for a given timestamp (midnight = 12AM, morning = 12:01AM – 11:59PM, noon = 12PM, afternoon = 12:01PM -05:59PM, evening = 06:00PM – 10:00PM, night = 10:01PM – 11:59PM) 	|

<a name="alphabetical-list"></a>

<a name="add"></a>

### add

Return the result from adding two numbers.

```
add(<summand_1>, <summand_2>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*summand_1*>, <*summand_2*> | Yes | Integer, Float, or mixed | The numbers to add |
|||||

| Return value | Type | Description |
| ------------ | -----| ----------- |
| <*result-sum*> | Integer or Float | The result from adding the specified numbers |
||||

*Example*

This example adds the specified numbers:

```
add(1, 1.5)
```

And returns this result: `2.5`

<a name="addDays"></a>

### addDays

Add a number of days to a timestamp.

```
addDays('<timestamp>', <days>, '<format>'?)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string that contains the timestamp |
| <*days*> | Yes | Integer | The positive or negative number of days to add |
| <*format*> | No | String | Either a [single format specifier](https://docs.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings) or a [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is ["o"](https://docs.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings) (yyyy-MM-ddTHH:mm:ss:fffffffK), which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) and preserves time zone information. |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updated-timestamp*> | String | The timestamp plus the specified number of days  |
||||

*Example 1*

This example adds 10 days to the specified timestamp:

```
addDays('2018-03-15T13:00:00Z', 10)
```

And returns this result: `"2018-03-25T00:00:0000000Z"`

*Example 2*

This example subtracts five days from the specified timestamp:

```
addDays('2018-03-15T00:00:00Z', -5)
```

And returns this result: `"2018-03-10T00:00:0000000Z"`

<a name="addHours"></a>

### addHours

Add a number of hours to a timestamp.

```
addHours('<timestamp>', <hours>, '<format>'?)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string that contains the timestamp |
| <*hours*> | Yes | Integer | The positive or negative number of hours to add |
| <*format*> | No | String | Either a [single format specifier](https://docs.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings) or a [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is ["o"](https://docs.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings) (yyyy-MM-ddTHH:mm:ss:fffffffK), which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) and preserves time zone information. |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updated-timestamp*> | String | The timestamp plus the specified number of hours  |
||||

*Example 1*

This example adds 10 hours to the specified timestamp:

```
addHours('2018-03-15T00:00:00Z', 10)
```

And returns this result: `"2018-03-15T10:00:0000000Z"`

*Example 2*

This example subtracts five hours from the specified timestamp:

```
addHours('2018-03-15T15:00:00Z', -5)
```

And returns this result: `"2018-03-15T10:00:0000000Z"`

<a name="addMinutes"></a>

### addMinutes

Add a number of minutes to a timestamp.

```
addMinutes('<timestamp>', <minutes>, '<format>'?)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string that contains the timestamp |
| <*minutes*> | Yes | Integer | The positive or negative number of minutes to add |
| <*format*> | No | String | Either a [single format specifier](https://docs.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings) or a [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is ["o"](https://docs.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings) (yyyy-MM-ddTHH:mm:ss:fffffffK), which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) and preserves time zone information. |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updated-timestamp*> | String | The timestamp plus the specified number of minutes |
||||

*Example 1*

This example adds 10 minutes to the specified timestamp:

```
addMinutes('2018-03-15T00:10:00Z', 10)
```

And returns this result: `"2018-03-15T00:20:00.0000000Z"`

*Example 2*

This example subtracts five minutes from the specified timestamp:

```
addMinutes('2018-03-15T00:20:00Z', -5)
```

And returns this result: `"2018-03-15T00:15:00.0000000Z"`

<a name="addSeconds"></a>

### addSeconds

Add a number of seconds to a timestamp.

```
addSeconds('<timestamp>', <seconds>, '<format>'?)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string that contains the timestamp |
| <*seconds*> | Yes | Integer | The positive or negative number of seconds to add |
| <*format*> | No | String | Either a [single format specifier](https://docs.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings) or a [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is ["o"](https://docs.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings) (yyyy-MM-ddTHH:mm:ss:fffffffK), which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) and preserves time zone information. |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updated-timestamp*> | String | The timestamp plus the specified number of seconds  |
||||

*Example 1*

This example adds 10 seconds to the specified timestamp:

```
addSeconds('2018-03-15T00:00:00Z', 10)
```

And returns this result: `"2018-03-15T00:00:10.0000000Z"`

*Example 2*

This example subtracts five seconds to the specified timestamp:

```
addSeconds('2018-03-15T00:00:30Z', -5)
```

And returns this result: `"2018-03-15T00:00:25.0000000Z"`

<a name="and"></a>

### and

Check whether all expressions are true.
Return true when all expressions are true,
or return false when at least one expression is false.

```
and(<expression1>, <expression2>, ...)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*expression1*>, <*expression2*>, ... | Yes | Boolean | The expressions to check |
|||||

| Return value | Type | Description |
| ------------ | -----| ----------- |
| true or false | Boolean | Return true when all expressions are true. Return false when at least one expression is false. |
||||

*Example 1*

These examples check whether the specified Boolean values are all true:

```
and(true, true)
and(false, true)
and(false, false)
```

And returns these results:

* First example: Both expressions are true, so returns `true`.
* Second example: One expression is false, so returns `false`.
* Third example: Both expressions are false, so returns `false`.

*Example 2*

These examples check whether the specified expressions are all true:

```
and(equals(1, 1), equals(2, 2))
and(equals(1, 1), equals(1, 2))
and(equals(1, 2), equals(1, 3))
```

And returns these results:

* First example: Both expressions are true, so returns `true`.
* Second example: One expression is false, so returns `false`.
* Third example: Both expressions are false, so returns `false`.

<a name="average
<a name="bool"></a>

### bool

Return the Boolean version for a value.

```
bool(<value>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*> | Yes | Any | The value to convert |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| true or false | Boolean | The Boolean version for the specified value |
||||

*Example*

These examples convert the specified values to Boolean values:

```
bool(1)
bool(0)
```

And returns these results:

* First example: `true`
* Second example: `false`

<a name="contains"></a>

### contains

Check whether a collection has a specific item.
Return true when the item is found,
or return false when not found.
This function is case-sensitive.

```
contains('<collection>', '<value>')
contains([<collection>], '<value>')
```

Specifically, this function works on these collection types:

* A *string* to find a *substring*
* An *array* to find a *value*
* A *dictionary* to find a *key*

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection*> | Yes | String, Array, or Dictionary | The collection to check |
| <*value*> | Yes | String, Array, or Dictionary, respectively | The item to find |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| true or false | Boolean | Return true when the item is found. Return false when not found. |
||||

*Example 1*

This example checks the string "hello world" for
the substring "world" and returns true:

```
contains('hello world', 'world')
```

*Example 2*

This example checks the string "hello world" for
the substring "universe" and returns false:

```
contains('hello world', 'universe')
```
<a name="count"></a>

### count

Return the number of items in a collection.

```
count('<collection>')
count([<collection>])
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection*> | Yes | String or Array | The collection with the items to count |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*length-or-count*> | Integer | The number of items in the collection |
||||

*Example*

These examples count the number of items in these collections:

```
count('abcd')
count(createArray(0, 1, 2, 3))
```

And return this result: `4`

<a name="createArray"></a>

### foreach

Operate on each element and return the new collection

```
foreach([<collection>], <iteratorName>, <function>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection*> | Yes | Array | The collection with the items |
| <*iteratorName*> | Yes | String | The key item of arrow function |
| <*function*> | Yes | Any | function that can contains iteratorName |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*new-collection*> | Array | the new collection which each element has been evaluated with the function  |
||||

*Example*

These examples generate new collections:

```
foreach(createArray(0, 1, 2, 3), x, x + 1)
```

And return this result: `[1, 2, 3, 4]`

<a name="createArray"></a>

### createArray

Return an array from multiple inputs.
For single input arrays, see [array()](#array).

```
createArray('<object1>', '<object2>', ...)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*object1*>, <*object2*>, ... | Yes | Any, but not mixed | At least two items to create the array |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| [<*object1*>, <*object2*>, ...] | Array | The array created from all the input items |
||||

*Example*

This example creates an array from these inputs:

```
createArray('h', 'e', 'l', 'l', 'o')
```

And returns this result: `["h", "e", "l", "l", "o"]`

<a name="date
<a name="dateReadBack
<a name="dayOfMonth"></a>

### dayOfMonth

Return the day of the month from a timestamp.

```
dayOfMonth('<timestamp>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string that contains the timestamp |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*day-of-month*> | Integer | The day of the month from the specified timestamp |
||||

*Example*

This example returns the number for the day
of the month from this timestamp:

```
dayOfMonth('2018-03-15T13:27:36Z')
```

And returns this result: `15`

<a name="dayOfWeek"></a>

### dayOfWeek

Return the day of the week from a timestamp.

```
dayOfWeek('<timestamp>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string that contains the timestamp |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*day-of-week*> | Integer | The day of the week from the specified timestamp where Sunday is 0, Monday is 1, and so on |
||||

*Example*

This example returns the number for the day of the week from this timestamp:

```
dayOfWeek('2018-03-15T13:27:36Z')
```

And returns this result: `3`

<a name="dayOfYear"></a>

### dayOfYear

Return the day of the year from a timestamp.

```
dayOfYear('<timestamp>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string that contains the timestamp |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*day-of-year*> | Integer | The day of the year from the specified timestamp |
||||

*Example*

This example returns the number of the day of the year from this timestamp:

```
dayOfYear('2018-03-15T13:27:36Z')
```

And returns this result: `74`

<a name="div"></a>

### div

Return the integer result from dividing two numbers.
To get the remainder result, see [mod()](#mod).

```
div(<dividend>, <divisor>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*dividend*> | Yes | Integer or Float | The number to divide by the *divisor* |
| <*divisor*> | Yes | Integer or Float | The number that divides the *dividend*, but cannot be 0 |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*quotient-result*> | Integer | The integer result from dividing the first number by the second number |
||||

*Example*

Both examples divide the first number by the second number:

```
div(10, 5)
div(11, 5)
```

And return this result: `2`

<a name="empty"></a>

### empty

Check whether a collection is empty.
Return true when the collection is empty,
or return false when not empty.

```
empty('<collection>')
empty([<collection>])
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection*> | Yes | String, Array, or Object | The collection to check |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| true or false | Boolean | Return true when the collection is empty. Return false when not empty. |
||||

*Example*

These examples check whether the specified collections are empty:

```
empty('')
empty('abc')
```

And returns these results:

* First example: Passes an empty string, so the function returns `true`.
* Second example: Passes the string "abc", so the function returns `false`.

<a name="equals"></a>

### equals

Check whether both values, expressions, or objects are equivalent.
Return true when both are equivalent, or return false when they're not equivalent.

```
equals('<object1>', '<object2>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*object1*>, <*object2*> | Yes | Various | The values, expressions, or objects to compare |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| true or false | Boolean | Return true when both are equivalent. Return false when not equivalent. |
||||

*Example*

These examples check whether the specified inputs are equivalent.

```
equals(true, 1)
equals('abc', 'abcd')
```

And returns these results:

* First example: Both values are equivalent, so the function returns `true`.
* Second example: Both values aren't equivalent, so the function returns `false`.

<a name="exists"></a>

### exists

Evaluates an expression for truthfulness.

```
exists(expression)
```

| Parameter | Required | Type | Description |
|-----------|----------|------|-------------|
| expression | Yes | expression | expression to evaluate for truthiness |
|||||

| Return value | Type | Description |
|--------------|------|-------------|
| <*true or false*> | Boolean | Result of evaluating the expression | 

*Example*

```
exists(foo.bar)
exists(foo.bar2)
```
With foo = {"bar":"value"}

The first example returns TRUE 
The second example returns FALSE

<a name="exp"></a>

### exp

Return exponentiation of one number to another.

```
exp(realNumber, exponentNumber)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| realNumber | Yes | Number | real number to compute exponent of |
| exponentNumber | Yes | Number | exponent number |
|||||

| Return value | Type | Description |
| ------------ | -----| ----------- |
| <*result-exp*> | Integer or Float | The result from computing exponent of realNumber |
||||

*Example*

This example computes the exponent:

```
exp(2, 2)
```

And returns this result: `4`

<a name="first"></a>

### first

Return the first item from a string or array.

```
first('<collection>')
first([<collection>])
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection*> | Yes | String or Array | The collection where to find the first item |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*first-collection-item*> | Any | The first item in the collection |
||||

*Example*

These examples find the first item in these collections:

```
first('hello')
first(createArray(0, 1, 2))
```

And return these results:

* First example: `"h"`
* Second example: `0`


<a name="float"></a>

### float

Convert a string version for a floating-point
number to an actual floating point number.
You can use this function only when passing custom
parameters to an app, such as a logic app.

```
float('<value>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*> | Yes | String | The string that has a valid floating-point number to convert |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*float-value*> | Float | The floating-point number for the specified string |
||||

*Example*

This example creates a string version for this floating-point number:

```
float('10.333')
```

And returns this result: `10.333`

<a name="formatDateTime"></a>

### formatDateTime

Return a timestamp in the specified format.

```
formatDateTime('<timestamp>', '<format>'?)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string that contains the timestamp |
| <*format*> | No | String | Either a [single format specifier](https://docs.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings) or a [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is ["o"](https://docs.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings) (yyyy-MM-ddTHH:mm:ss:fffffffK), which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) and preserves time zone information. |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*reformatted-timestamp*> | String | The updated timestamp in the specified format |
||||

*Example*

This example converts a timestamp to the specified format:

```
formatDateTime('03/15/2018 12:00:00', 'yyyy-MM-ddTHH:mm:ss')
```

And returns this result: `"2018-03-15T12:00:00"`


<a name="getTimeOfDay

<a name="greater"></a>

### greater

Check whether the first value is greater than the second value.
Return true when the first value is more,
or return false when less.

```
greater(<value>, <compareTo>)
greater('<value>', '<compareTo>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*> | Yes | Integer, Float, or String | The first value to check whether greater than the second value |
| <*compareTo*> | Yes | Integer, Float, or String, respectively | The comparison value |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| true or false | Boolean | Return true when the first value is greater than the second value. Return false when the first value is equal to or less than the second value. |
||||

*Example*

These examples check whether the first value is greater than the second value:

```
greater(10, 5)
greater('apple', 'banana')
```

And return these results:

* First example: `true`
* Second example: `false`

<a name="greaterOrEquals"></a>

### greaterOrEquals

Check whether the first value is greater than or equal to the second value.
Return true when the first value is greater or equal,
or return false when the first value is less.

```
greaterOrEquals(<value>, <compareTo>)
greaterOrEquals('<value>', '<compareTo>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*> | Yes | Integer, Float, or String | The first value to check whether greater than or equal to the second value |
| <*compareTo*> | Yes | Integer, Float, or String, respectively | The comparison value |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| true or false | Boolean | Return true when the first value is greater than or equal to the second value. Return false when the first value is less than the second value. |
||||

*Example*

These examples check whether the first value is greater or equal than the second value:

```
greaterOrEquals(5, 5)
greaterOrEquals('apple', 'banana')
```

And return these results:

* First example: `true`
* Second example: `false`

<a name="if"></a>

### if

Check whether an expression is true or false.
Based on the result, return a specified value.

```
if(<expression>, <valueIfTrue>, <valueIfFalse>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*expression*> | Yes | Boolean | The expression to check |
| <*valueIfTrue*> | Yes | Any | The value to return when the expression is true |
| <*valueIfFalse*> | Yes | Any | The value to return when the expression is false |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*specified-return-value*> | Any | The specified value that returns based on whether the expression is true or false |
||||

*Example*

This example returns `"yes"` because the
specified expression returns true.
Otherwise, the example returns `"no"`:

```
if(equals(1, 1), 'yes', 'no')
```

<a name="int"></a>

### int

Return the integer version for a string.

```
int('<value>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*> | Yes | String | The string to convert |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*integer-result*> | Integer | The integer version for the specified string |
||||

*Example*

This example creates an integer version for the string "10":

```
int('10')
```

And returns this result: `10`

<a name="join"></a>

### join

Return a string that has all the items from an array
and has each character separated by a *delimiter*.

```
join([<collection>], '<delimiter>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection*> | Yes | Array | The array that has the items to join |
| <*delimiter*> | Yes | String | The separator that appears between each character in the resulting string |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*char1*><*delimiter*><*char2*><*delimiter*>... | String | The resulting string created from all the items in the specified array |
||||

*Example*

This example creates a string from all the items in this
array with the specified character as the delimiter:

```
join(createArray('a', 'b', 'c'), '.')
```

And returns this result: `"a.b.c"`

<a name="last"></a>

### last

Return the last item from a collection.

```
last('<collection>')
last([<collection>])
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection*> | Yes | String or Array | The collection where to find the last item |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*last-collection-item*> | String or Array, respectively | The last item in the collection |
||||

*Example*

These examples find the last item in these collections:

```
last('abcd')
last(createArray(0, 1, 2, 3))
```

And returns these results:

* First example: `"d"`
* Second example: `3`

### length

Return the length of a string

```
length('<str>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*str*> | Yes | String | The string where to get length |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*length*> | Integer | The length of this string |
||||

*Example*

These examples get the length of strings:

```
length('hello')
length('hello world')
```

And returns these results:

* First example: `5`
* Second example: `11`

<a name="less"></a>

### less

Check whether the first value is less than the second value.
Return true when the first value is less,
or return false when the first value is more.

```
less(<value>, <compareTo>)
less('<value>', '<compareTo>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*> | Yes | Integer, Float, or String | The first value to check whether less than the second value |
| <*compareTo*> | Yes | Integer, Float, or String, respectively | The comparison item |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| true or false | Boolean | Return true when the first value is less than the second value. Return false when the first value is equal to or greater than the second value. |
||||

*Example*

These examples check whether the first value is less than the second value.

```
less(5, 10)
less('banana', 'apple')
```

And return these results:

* First example: `true`
* Second example: `false`

<a name="lessOrEquals"></a>

### lessOrEquals

Check whether the first value is less than or equal to the second value.
Return true when the first value is less than or equal,
or return false when the first value is more.

```
lessOrEquals(<value>, <compareTo>)
lessOrEquals('<value>', '<compareTo>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*> | Yes | Integer, Float, or String | The first value to check whether less than or equal to the second value |
| <*compareTo*> | Yes | Integer, Float, or String, respectively | The comparison item |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| true or false  | Boolean | Return true when the first value is less than or equal to the second value. Return false when the first value is greater than the second value. |
||||

*Example*

These examples check whether the first value is less or equal than the second value.

```
lessOrEquals(10, 10)
lessOrEquals('apply', 'apple')
```

And return these results:

* First example: `true`
* Second example: `false`

<a name="max"></a>

### max

Return the highest value from a list or array with
numbers that is inclusive at both ends.

```
max(<number1>, <number2>, ...)
max([<number1>, <number2>, ...])
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*number1*>, <*number2*>, ... | Yes | Integer, Float, or both | The set of numbers from which you want the highest value |
| [<*number1*>, <*number2*>, ...] | Yes | Array - Integer, Float, or both | The array of numbers from which you want the highest value |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*max-value*> | Integer or Float | The highest value in the specified array or set of numbers |
||||

*Example*

These examples get the highest value from the set of numbers and the array:

```
max(1, 2, 3)
max(createArray(1, 2, 3))
```

And return this result: `3`

<a name="min"></a>

### min

Return the lowest value from a set of numbers or an array.

```
min(<number1>, <number2>, ...)
min([<number1>, <number2>, ...])
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*number1*>, <*number2*>, ... | Yes | Integer, Float, or both | The set of numbers from which you want the lowest value |
| [<*number1*>, <*number2*>, ...] | Yes | Array - Integer, Float, or both | The array of numbers from which you want the lowest value |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*min-value*> | Integer or Float | The lowest value in the specified set of numbers or specified array |
||||

*Example*

These examples get the lowest value in the set of numbers and the array:

```
min(1, 2, 3)
min(createArray(1, 2, 3))
```

And return this result: `1`

<a name="mod"></a>

### mod

Return the remainder from dividing two numbers.
To get the integer result, see [div()](#div).

```
mod(<dividend>, <divisor>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*dividend*> | Yes | Integer or Float | The number to divide by the *divisor* |
| <*divisor*> | Yes | Integer or Float | The number that divides the *dividend*, but cannot be 0. |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*modulo-result*> | Integer or Float | The remainder from dividing the first number by the second number |
||||

*Example*

This example divides the first number by the second number:

```
mod(3, 2)
```

And return this result: `1`

<a name="month
<a name="mul"></a>

### mul

Return the product from multiplying two numbers.

```
mul(<multiplicand1>, <multiplicand2>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*multiplicand1*> | Yes | Integer or Float | The number to multiply by *multiplicand2* |
| <*multiplicand2*> | Yes | Integer or Float | The number that multiples *multiplicand1* |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*product-result*> | Integer or Float | The product from multiplying the first number by the second number |
||||

*Example*

These examples multiple the first number by the second number:

```
mul(1, 2)
mul(1.5, 2)
```

And return these results:

* First example: `2`
* Second example `3`

<a name="not"></a>

### not

Check whether an expression is false.
Return true when the expression is false,
or return false when true.

```
not(<expression>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*expression*> | Yes | Boolean | The expression to check |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| true or false | Boolean | Return true when the expression is false. Return false when the expression is true. |
||||

*Example 1*

These examples check whether the specified expressions are false:

```
not(false)
not(true)
```

And return these results:

* First example: The expression is false, so the function returns `true`.
* Second example: The expression is true, so the function returns `false`.

*Example 2*

These examples check whether the specified expressions are false:

```
not(equals(1, 2))
not(equals(1, 1))
```

And return these results:

* First example: The expression is false, so the function returns `true`.
* Second example: The expression is true, so the function returns `false`.

<a name="or"></a>

### or

Check whether at least one expression is true.
Return true when at least one expression is true,
or return false when all are false.

```
or(<expression1>, <expression2>, ...)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*expression1*>, <*expression2*>, ... | Yes | Boolean | The expressions to check |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| true or false | Boolean | Return true when at least one expression is true. Return false when all expressions are false. |
||||

*Example 1*

These examples check whether at least one expression is true:

```
or(true, false)
or(false, false)
```

And return these results:

* First example: At least one expression is true, so the function returns `true`.
* Second example: Both expressions are false, so the function returns `false`.

*Example 2*

These examples check whether at least one expression is true:

```
or(equals(1, 1), equals(1, 2))
or(equals(1, 2), equals(1, 3))
```

And return these results:

* First example: At least one expression is true, so the function returns `true`.
* Second example: Both expressions are false, so the function returns `false`.

<a name="rand"></a>

### rand

Return a random integer from a specified range,
which is inclusive only at the starting end.

```
rand(<minValue>, <maxValue>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*minValue*> | Yes | Integer | The lowest integer in the range |
| <*maxValue*> | Yes | Integer | The integer that follows the highest integer in the range that the function can return |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*random-result*> | Integer | The random integer returned from the specified range |
||||

*Example*

This example gets a random integer from the specified range, excluding the maximum value:

```
rand(1, 5)
```

And returns one of these numbers as the result: `1`, `2`, `3`, or `4`

<a name="replace"></a>

### replace

Replace a substring with the specified string,
and return the result string. This function
is case-sensitive.

```
replace('<text>', '<oldText>', '<newText>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*text*> | Yes | String | The string that has the substring to replace |
| <*oldText*> | Yes | String | The substring to replace |
| <*newText*> | Yes | String | The replacement string |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updated-text*> | String | The updated string after replacing the substring <p>If the substring is not found, return the original string. |
||||

*Example*

This example finds the "old" substring in "the old string"
and replaces "old" with "new":

```
replace('the old string', 'old', 'new')
```

And returns this result: `"the new string"`

<a name="replace"></a>

### replaceIgnoreCase

Replace a substring with the specified string,
and return the result string. This function
is case-insensitive.

```
replaceIgnoreCase('<text>', '<oldText>', '<newText>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*text*> | Yes | String | The string that has the substring to replace |
| <*oldText*> | Yes | String | The substring to replace |
| <*newText*> | Yes | String | The replacement string |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updated-text*> | String | The updated string after replacing the substring <p>If the substring is not found, return the original string. |
||||

*Example*

This example finds the "old" substring in "the old string"
and replaces "old" with "new":

```
replace('the old string', 'old', 'new')
```

And returns this result: `"the new string"`

<a name="split"></a>

### split

Return an array that contains substrings, separated by commas,
based on the specified delimiter character in the original string.

```
split('<text>', '<delimiter>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*text*> | Yes | String | The string to separate into substrings based on the specified delimiter in the original string |
| <*delimiter*> | Yes | String | The character in the original string to use as the delimiter |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| [<*substring1*>,<*substring2*>,...] | Array | An array that contains substrings from the original string, separated by commas |
||||

*Example*

This example creates an array with substrings from the specified
string based on the specified character as the delimiter:

```
split('a_b_c', '_')
```

And returns this array as the result: `["a","b","c"]`

<a name="string"></a>

### string

Return the string version for a value.

```
string(<value>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*> | Yes | Any | The value to convert |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*string-value*> | String | The string version for the specified value |
||||

*Example 1*

This example creates the string version for this number:

```
string(10)
```

And returns this result: `"10"`

*Example 2*

This example creates a string for the specified JSON object
and uses the backslash character (\\)
as an escape character for the double-quotation mark (").

```
string( { "name": "Sophie Owen" } )
```

And returns this result: `"{ \\"name\\": \\"Sophie Owen\\" }"`

<a name="sub"></a>

### sub

Return the result from subtracting the second number from the first number.

```
sub(<minuend>, <subtrahend>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*minuend*> | Yes | Integer or Float | The number from which to subtract the *subtrahend* |
| <*subtrahend*> | Yes | Integer or Float | The number to subtract from the *minuend* |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*result*> | Integer or Float | The result from subtracting the second number from the first number |
||||

*Example*

This example subtracts the second number from the first number:

```
sub(10.3, .3)
```

And returns this result: `10`

<a name="substring"></a>

### substring

Return characters from a string,
starting from the specified position, or index.
Index values start with the number 0.

```
substring('<text>', <startIndex>, <length>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*text*> | Yes | String | The string whose characters you want |
| <*startIndex*> | Yes | Integer | A positive number equal to or greater than 0 that you want to use as the starting position or index value |
| <*length*> | Yes | Integer | A positive number of characters that you want in the substring |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*substring-result*> | String | A substring with the specified number of characters, starting at the specified index position in the source string |
||||

*Example*

This example creates a five-character substring from the specified string,
starting from the index value 6:

```
substring('hello world', 6, 5)
```

And returns this result: `"world"`

<a name="subtractFromTime"></a>

### subtractFromTime

Subtract a number of time units from a timestamp.
See also [getPastTime](#getPastTime).

```
subtractFromTime('<timestamp>', <interval>, '<timeUnit>', '<format>'?)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string that contains the timestamp |
| <*interval*> | Yes | Integer | The number of specified time units to subtract |
| <*timeUnit*> | Yes | String | The unit of time to use with *interval*: "Second", "Minute", "Hour", "Day", "Week", "Month", "Year" |
| <*format*> | No | String | Either a [single format specifier](https://docs.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings) or a [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is ["o"](https://docs.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings) (yyyy-MM-ddTHH:mm:ss:fffffffK), which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) and preserves time zone information. |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updated-timestamp*> | String | The timestamp minus the specified number of time units |
||||

*Example 1*

This example subtracts one day from this timestamp:

```
subtractFromTime('2018-01-02T00:00:00Z', 1, 'Day')
```

And returns this result: `"2018-01-01T00:00:00:0000000Z"`

*Example 2*

This example subtracts one day from this timestamp:

```
subtractFromTime('2018-01-02T00:00:00Z', 1, 'Day', 'D')
```

And returns this result using the optional "D" format: `"Monday, January, 1, 2018"`

<a name="sum"></a>

### sum

Return the result from adding numbers in a list.

```
add([<list of numbers>])
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| [\<list of numbers\>] | Yes | List | The numbers to add |
|||||

| Return value | Type | Description |
| ------------ | -----| ----------- |
| <*result-sum*> | Integer or Float | The result from adding the specified numbers |
||||

*Example*

This example adds the specified numbers:

```
add([1, 1.5])
```

And returns this result: `2.5`

<a name="toLower"></a>

### toLower

Return a string in lowercase format. If a character
in the string doesn't have a lowercase version,
that character stays unchanged in the returned string.

```
toLower('<text>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*text*> | Yes | String | The string to return in lowercase format |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*lowercase-text*> | String | The original string in lowercase format |
||||

*Example*

This example converts this string to lowercase:

```
toLower('Hello World')
```

And returns this result: `"hello world"`

<a name="toUpper"></a>

### toUpper

Return a string in uppercase format. If a character
in the string doesn't have an uppercase version,
that character stays unchanged in the returned string.

```
toUpper('<text>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*text*> | Yes | String | The string to return in uppercase format |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*uppercase-text*> | String | The original string in uppercase format |
||||

*Example*

This example converts this string to uppercase:

```
toUpper('Hello World')
```

And returns this result: `"HELLO WORLD"`

<a name="trim"></a>

### trim

Remove leading and trailing whitespace from a string,
and return the updated string.

```
trim('<text>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*text*> | Yes | String | The string that has the leading and trailing whitespace to remove |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updatedText*> | String | An updated version for the original string without leading or trailing whitespace |
||||

*Example*

This example removes the leading and trailing
whitespace from the string " Hello World  ":

```
trim(' Hello World  ')
```

And returns this result: `"Hello World"`

<a name="utcNow"></a>

### utcNow

Return the current timestamp.

```
utcNow('<format>')
```

Optionally, you can specify a different format with the <*format*> parameter.


| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*format*> | No | String | Either a [single format specifier](https://docs.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings) or a [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is ["o"](https://docs.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings) (yyyy-MM-ddTHH:mm:ss:fffffffK), which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) and preserves time zone information. |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*current-timestamp*> | String | The current date and time |
||||

*Example 1*

Suppose today is April 15, 2018 at 1:00:00 PM.
This example gets the current timestamp:

```
utcNow()
```

And returns this result: `"2018-04-15T13:00:00.0000000Z"`

*Example 2*

Suppose today is April 15, 2018 at 1:00:00 PM.
This example gets the current timestamp using the optional "D" format:

```
utcNow('D')
```

And returns this result: `"Sunday, April 15, 2018"`



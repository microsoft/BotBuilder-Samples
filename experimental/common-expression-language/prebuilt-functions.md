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
|[replace](#replace)| Replace a substring with the specified string, and return the updated string. case sensitive|
|[replaceIgnoreCase](#replaceIgnoreCase)|	Replace a substring with the specified string, and return the updated string. Case in-sensitive	|
|[split](#split)	|Returns an array that contains substrings based on the delimiter specified.|
|[substring](#substring) |Returns characters from a string. Substring(sourceString, startPos, endPos). startPos cannot be less than 0. endPos greater than source strings length will be taken as the max length of the string	|
|[toLower](#toLower) |Convert a string to all lower case characters |
|[toUpper](#toUpper) |Convert a string to all upper case characters |
|[trim](#trim) |Remove leading and trailing white spaces from a string |
|[addOrdinal](#addOrdinal) | Return the ordinal number of the input number |
|[endsWith](#endsWith) | Check whether a string ends with a specific substring. Return true when the substring is found, or return false when not found. This function is not case-sensitive. |
|[startsWith](#startsWith) |Check whether a string starts with a specific substring. Return true when the substring is found, or return false when not found. This function is not case-sensitive. |
|[countWord](#countWord)| Return the number of words in the given string |
|[concat](#concat) | Combine two or more strings and return the resulting string |
|[newGuid](#newGuid) | Return a new Guid string|
|[indexOf](#indexOf)| Return the starting position or index value for a substring. Or Searches for the specified object and returns the zero-based index of the first occurrence within the entire list. This function is not case-sensitive, and indexes start with the number 0.|
|[lastIndexOf](#lastIndexOf)| Return the starting position or index value for the last occurrence of a substring. Or searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the list.This function is not case-sensitive, and indexes start with the number 0. |

### Collection functions
|Function	|Explanation|
|-----------|-----------|
|[contains](#contains)	|Works to find an item in a string or to find an item in an array or to find a parameter in a complex object. E.g. contains('hello world', 'hello'); contains(createArray('1','2'), '1'); contains(json("{'foo':'bar'}"), 'foo')	|
|[empty](#empty)	|Check if the collection is empty	|
|[first](#first)	|Returns the first item from the collection	|
|[join](#join) 	|Return a string that has all the items from an array and has each character separated by a delimiter. Join(collection, delimiter). Join(createArray('a','b'), '.') = "a.b"	|
|[last](#last) 	|Returns the last item from the collection	|
|[count](#count)	|Returns the number of items in the collection	|
|[foreach](#foreach)	|Operate on each element and return the new collection	|
|[union](#union) | Return a collection that has all the items from the specified collections |
|[skip](#skip) | Remove items from the front of a collection, and return *\all the other* items |
|[take](#take) | Return items from the front of a collection |
|[intersection](#intersection) | Return a collection that has only the common items across the specified collections |
|[subArray](#subArray) | Returns a sub-array from specified start and end position. Index values start with the number 0. |
|[select](#select) | Operate on each element and return the new collection of transformed elements |
|[where](#where) | Filter on each element and return the new collection of filtered elements which match specific condition |
|[sortBy](#sortBy) | Sort elements in the collection with ascending order and return the sorted collection |
|[sortByDescending](#sortByDescending) | Sort elements in the collection with descending order and return the sorted collection |
|[indicesAndValues](#indicesAndValues) | Turned an array or object into an array of objects with index and value property |
|[flatten](#flatten) | Flatten arrays into an array with non-array values |
|[unique](#unique) | Remove all duplicates from an array |

### Logical comparison functionsl
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
|[bool](#bool)	|Return Boolean representation of the specified string. Bool('true'), bool(1)	|
|[createArray](#createArray)	|Create an array from multiple inputs	|
|[json](#json)  | Return the JavaScript Object Notation (JSON) type value or object for a string or XML.    |
|[array](#array)| Return an array from a single specified input. For multiple inputs, see [createArray](#createArray). |
|[base64](#base64) | Return the base64-encoded version for a string. |
|[base64ToBinary](#base64ToBinary) | Return the binary version for a base64-encoded string. |
|[base64ToString](#base64ToString) | Return thr string version for a base64-encoded string. |
|[binary](#binary) | Return the binary version for an input value. |
|[dataUri](#dataUri) | Return the URI for an input value. |
|[dataUriToBinary](#dataUriToBinary) | Return the binary version of a data URI. |
|[dataUriToString](#dateUriToString) | Return the string version of a data URI. |
|[uriComponent](#uriComponent) | Return the URI-encoded version for an input value by replacing URL-unsafe characters with escape characters. |
|[uriComponentToString](#uriComponentToString) | Return the string version for a URI-encoded string. |
|[xml](#xml) | [C# only] Return the XML version for a string. |

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
|[range](#range) | Return an integer array that starts from a specified integer. |
|[exp](#exp)	|Exponentiation function. Exp(base, exponent)	|
|[average](#average)	| Return the average number of an numeric array.	|

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
|[dateReadBack](#dateReadBack)	|Uses the date-time library to provide a date readback. |
|[month](#month)	|Returns the month of given timestamp	|
|[date](#date)	|Returns date for a given timestamp	|
|[year](#year)	|Returns year for the given timestamp	|
|[getTimeOfDay](#getTimeOfDay)	|Returns time of day for a given timestamp (midnight = 12AM, morning = 12:01AM – 11:59PM, noon = 12PM, afternoon = 12:01PM -05:59PM, evening = 06:00PM – 10:00PM, night = 10:01PM – 11:59PM) 	|
|[getFutureTime](#getFutureTime) | Return the current timestamp plus a specified time units.    |
|[getPastTime](#getPastTime) | Return the current timestamp minus a specified time units.   |
|[addToTime](#addToTime)    | Add a number of time units to a timestamp.    |
|[convertFromUTC](#convertFromUTC)  | Convert a timestamp from Universal Time Coordinated(UTC).  |
|[convertToUTC](#convertToUTC)  | Convert a timestamp To Universal Time Coordinated(UTC).   |
|[startOfDay](#startOfDay)  | Return the start of the day for a timestamp. |
|[startOfHour](#startOfHour)    | Return the start of the hour for a timestamp.  |
|[startOfMonth](#startOfMonth)  | Return the start of the month for a timestamp. |
|[ticks](#ticks)    | Return the ticks property value for a specified timestamp. |

### URI parsing functions
|Function	|Explanation|
|-----------|-----------|
|[uriHost](#uriHost)    | Return the host value for a uniform resource identifier(URI). |
|[uriPath](#uriPath)    | Return the path value for a unifor resource identifier(URI).  |
|[uriPathAndQuery](#uriPathAndQuery)   | Rtuen the path and query values for a uniform resource identifier(URI).   |
|[uriPort](#uriPort)    | Return the port value for a uniform resource identifier(URI). |
|[uriQuery](#uriQuery)  | Return the query value for a uniform resouce identifier(URI). |
|[uriScheme](#uriScheme)| Return the scheme value for a uniform resource identifier(uri).   |

### Object manipulation and construction functions
|Function	|Explanation|
|-----------|-----------|
|[addProperty](#addProperty)    | Add a property and its value, or name-value pair, to a JSON object abd return the updated object. |
|[removeProperty](#removeProperty)  | Remove a property from JSON object and retuen the updated object. |
|[setProperty](#setProperty)    | Set the value for a JSON object's property and return the updated object. |
|[getProperty](#getProperty)    | Return the value of the given property in a JSON object.  |
|[coalesce](#coalesce)  | Return the first non-null value from one or more parameters.  |
|[xPath](#xPath)    | [C# only] Check XML for nodes or values that match an XPath(XML Path Language) expression, and return the matching nodes or values. |
|[jPath](#jPath)    | Check JSON or JSON string for nodes or value that match a path expression, and return the matching nodes. |
|[setPathToValue](#setPathToValue)    | Set the value for a specific path and return the value. |

### Regex functions
|Function	|Explanation|
|-----------|-----------|
|[isMatch](#isMatch)	|test a given string ia match a common regex pattern	|

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
| <*timestamp*> | Yes | String | The string that contains the timestamp which must be standard UTC ISO format like YYYY-MM-DDTHH:mm:ss.fffZ |
| <*days*> | Yes | Integer | The positive or negative number of days to add |
| <*format*> | No | String | A [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is UTC ISO format like YYYY-MM-DDTHH:mm:ss.fffZ, which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601). |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updated-timestamp*> | String | The timestamp plus the specified number of days  |
||||

*Example 1*

This example adds 10 days to the specified timestamp:

```
addDays('2018-03-15T13:00:00.000Z', 10)
```

And returns this result: `"2018-03-25T00:00:00.000Z"`

*Example 2*

This example subtracts five days from the specified timestamp:

```
addDays('2018-03-15T00:00:00.000Z', -5)
```

And returns this result: `"2018-03-10T00:00:00.000Z"`

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
| <*format*> | No | String | A [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is UTC ISO format like YYYY-MM-DDTHH:mm:ss.fffZ, which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601). |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updated-timestamp*> | String | The timestamp plus the specified number of hours  |
||||

*Example 1*

This example adds 10 hours to the specified timestamp:

```
addHours('2018-03-15T00:00:00.000Z', 10)
```

And returns this result: `"2018-03-15T10:00:00.000Z"`

*Example 2*

This example subtracts five hours from the specified timestamp:

```
addHours('2018-03-15T15:00:00.000Z', -5)
```

And returns this result: `"2018-03-15T10:00:00.000Z"`

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
| <*format*> | No | String | A [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is UTC ISO format like YYYY-MM-DDTHH:mm:ss.fffZ, which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601). |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updated-timestamp*> | String | The timestamp plus the specified number of minutes |
||||

*Example 1*

This example adds 10 minutes to the specified timestamp:

```
addMinutes('2018-03-15T00:10:00.000Z', 10)
```

And returns this result: `"2018-03-15T00:20:00.000Z"`

*Example 2*

This example subtracts five minutes from the specified timestamp:

```
addMinutes('2018-03-15T00:20:00.000Z', -5)
```

And returns this result: `"2018-03-15T00:15:00.000Z"`

<a name="addOrdinal"></a>

### addOrdinal

Return the ordinal number of the input number.

```
addOrdinal(<number>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*number*>| Yes | Integer| The numbers to convert to an ordinal number |
|||||

| Return value | Type | Description |
| ------------ | -----| ----------- |
| <*result*> | String | The ordinal number converted from the input number |
||||

*Example*

```
addOrdinal(11)
addOrdinal(12)
addOrdinal(13)
addOrdinal(21)
addOrdinal(22)
addOrdinal(23)
```

And these return the results:
`"11th", "12th", "13th", "21st", "22nd", "23rd"`

<a name="addProperty"></a>

### addProperty

Add a property and its value, or name-value pair, to a JSON object, and return the updated object. If the object already exists at runtime, the function throws an error.

```
addProperty('<object>', '<property>', value)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*object*> | Yes | Object | The JSON Object where you want to add a property |
|<*property*>| Yes | String | The name of the property to add |
|<*value*>| Yes | Any | The value of the property |

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updated-object*> | Object | The updated JSON object after adding a new property |
||||

*Example*
This example adds the accountNumber property to the customerProfile object, which is converted to JSON with the JSON() function. The function assigns a value that is generated by the guid() function, and returns the updated object:

```
addProperty(json('customerProfile'), 'accountNumber', guid())
```

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
| <*format*> | No | String | A [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is UTC ISO format like YYYY-MM-DDTHH:mm:ss.fffZ, which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601). |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updated-timestamp*> | String | The timestamp plus the specified number of seconds  |
||||

*Example 1*

This example adds 10 seconds to the specified timestamp:

```
addSeconds('2018-03-15T00:00:00.000Z', 10)
```

And returns this result: `"2018-03-15T00:00:10.000Z"`

*Example 2*

This example subtracts five seconds to the specified timestamp:

```
addSeconds('2018-03-15T00:00:30.000Z', -5)
```

And returns this result: `"2018-03-15T00:00:25.000Z"`

<a name="addToTime"></a>

### addToTime

Add a number of time units to a timestamp. See also [getFutureTime()](#getFutureTime)

```
addToTime('<timestamp>', '<interval>', <timeUnit>, '<format>'?)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string that contains the timestamp |
| <*interval*> | Yes | Integer | The number of specified time units to add |
| <*timeUnit*> | Yes | String | The unit of time to use with *interval*: "Second", "Minute", "Hour", "Day", "Week", "Month", "Year" |
| <*format*> | No | String | A [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is UTC ISO format like YYYY-MM-DDTHH:mm:ss.fffZ, which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601). |

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updated-timestamp*> | String | The timestamp plus the number of specified time units with given format. |
||||

*Example 1*
This example adds one day to specified timestamp.

```
addToTime('2018-01-01T00:00:00.000Z', 1, 'Day')
```

And returns this result `'2018-01-02T00:00:00.000Z'`.

*Example 2*
This example adds two weeks to the specified timestamp, with given 'MM-DD-YY' format

```
addToTime('2018-01-01T00:00:00.000Z', 2, 'Week', 'MM-DD-YY')
```

And returns this result using the optional 'MM-DD-YY' format `'01-15-18'`.


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

<a name="average"></a>

### average

Return the average number of an numeric array.

```
average(<numericArray>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*numericArray*> | Yes | Array of Number | The input array to calculate the average |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*average-of-array*> | Number | The average value of the given array |
||||

*Example*

```
average(createArray(1,2,3))
```

And it returns the result: `2`

<a name="base64"></a>

### base64

Return the base64-encoded version for a string.

```
base64('<value>')
```
 
| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*> | Yes | String | The input string |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*base64-string*> | String | The base64-encoded version for the input string |
||||

*Example*

This example converts the "hello" string to a base64-encoded string:

```
base64('hello')
```

And returns this result: `"aGVsbG8="`

<a name="base64ToBinary"></a>

### base64ToBinary

Return the binary version for a base64-encoded string.

```
base64ToBinary('<value>')
```
 
| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*> | Yes | String | The base64-encoded string to convert |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*binary-for-base64-string*> | String | The binary version for the base64-encoded string |
||||

*Example*

This example converts the "aGVsbG8=" base64-encoded string to a binary string:

```
base64ToBinary('aGVsbG8=')
```

And returns this result: `"0110000101000111010101100111001101100010010001110011100000111101"`

<a name="base64ToString"></a>

### base64ToString

Return the string version for a base64-encoded string, effectively decoding the base64 string. Use this function rather than [decodeBase64](#decodeBase64). Although both functions work the same way, base64ToString() is preferred.

```
base64ToString('<value>')
```
 
| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*> | Yes | String | The base64-encoded string to decode |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*decoded-base64-string*> | String | The string version for a base64-encoded string |
||||

*Example*

This example converts the "aGVsbG8=" base64-encoded string to just a string:

```
base64ToString('aGVsbG8=')
```

And returns this result: `"hello"`

<a name="binary"></a>

### binary

Return the binary version for a string.

```
bianry('<value>')
```
 
| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*> | Yes | String | The string to convert |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*binary-for-input-value*> | String | The binary version for the specified string |
||||

*Example*

This example converts the "hello" string to a binary string:

```
binary('hello')
```

And returns this result: `"0110100001100101011011000110110001101111"`

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

<a name="coalesce"></a>

### coalesce

Return the first non-null value from one or more parameters. Empty strings, empty arrays, and empty objects are not null.

```
coalesce(<object_1>, <object_2>, ...)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*object_1*>, <*object_2*>, ... | Yes | Any, can mix types | One or more items to check for null|
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*first-non-null-item*> | Any | The first item or value that is not null. If all parameters are null, this function returns null. |
||||

*Example*
These examples return the first non-null value from the specified values, or null when all the values are null:
```
coalesce(null, true, false)
coalesce(null, 'hello', 'world')
coalesce(null, null, null)
```
And returns these results:

- First example: true
- Second example: "hello"
- Third example: null

<a name="concat"></a>

### concat

Combine two or more strings, and return the combined string.

```
concat('<text1>', '<text2>', ...)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*text1*>, <*text2*>, ... | Yes | String | At least two strings to combine |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*text1text2...*> | String | The string created from the combined input strings |
||||

*Example*

This example combines the strings "Hello" and "World":

```
concat('Hello', 'World')
```

And returns this result: `"HelloWorld"`

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

<a name="countWord"></a>

### countWord

Return the number of words in a string

```
countWord('<text>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*text*> | Yes | String | The string contains some words to count |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*count*> | Integer | The number of words in the string |
||||

*Example*

```
countWord("hello word")
```

And it returns the result: `2`

<a name="convertFromUTC"></a>

### convertFromUTC

Convert a timestamp from Universal Time Coordinated(UTC) to target time zone.

```
convertFromUTC('<timestamp>', '<destinationTimeZone>', '<format>'?)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string contains the timestamp |
| <*destinationTimeZone*> | Yes | String | The name for the target time zone. Support Windows time zone and Iana time zone |
| <*format*> | No | String | A [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is UTC ISO format like YYYY-MM-DDTHH:mm:ss.fffZ, which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601). |

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*converted-timestamp*> | String | The timestamp converted to the target time zone. |
||||

```
convertFromUTC("convertFromUTC('2018-02-02T02:00:00.000Z', 'Pacific Standard Time', 'MM-DD-YY')"
convertFromUTC('2018-02-02T02:00:00.000Z', 'Pacific Standard Time')
```

And return these results:
02-01-18
2018-02-01T18:00:00.000-08:00

<a name="convertToUTC"></a>

### convertToUTC
Convert a timestamp to Universal Time Coordinated(UTC) from source time zone.

```
convertToUTC('<timestamp>', '<sourceTimeZone>', '<format>'?)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string contains the timestamp |
| <*sourceTimeZone*> | Yes | String | The name for the target time zone. Support Windows time zone and Iana time zone |
| <*format*> | No | String | A [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is UTC ISO format like YYYY-MM-DDTHH:mm:ss.fffZ, which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601). |

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*converted-timestamp*> | String | The timestamp converted to the target time zone. |
||||

```
convertToUTC('01/01/2018 00:00:00',', 'Pacific Standard Time')
```

And return these results:
2018-01-01T08:00:00.000Z

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

<a name="dataUri"></a>

### dataUri

Return a data uniform resource identifier (URI) for a string.

```
dataUri('<value>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*>| Yes | String | The string to convert |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| [<*date-uri*>] | String | The data URI for the input string |
||||

*Example*

```
dataUri('hello')
```

And returns this result: `"data:text/plain;charset=utf-8;base64,aGVsbG8="`

<a name="dataUriToBinary"></a>

### dataUriToBinary

Return the binary version for a data uniform resource identifier (URI). Use this function rather than decodeDataUri(). Although both functions work the same way, dataUriBinary() is preferred.

```
dataUriToBinary('<value>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*>| Yes | String | The data URI to convert |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| [<*binary-for-data-uri*>] | String | The binary version for the data URI |
||||

*Example*

This example creates a binary version for this data URI:

```
dataUriToBinary('data:text/plain;charset=utf-8;base64,aGVsbG8=')
```

And returns this result:

`"01100100011000010111010001100001001110100111010001100101011110000111010000101111011100000 1101100011000010110100101101110001110110110001101101000011000010111001001110011011001010111 0100001111010111010101110100011001100010110100111000001110110110001001100001011100110110010 10011011000110100001011000110000101000111010101100111001101100010010001110011100000111101"`

<a name="dataUriToString"></a>

### dataUriToString

Return the string version for a data uniform resource identifier (URI).

```
dataUriToString('<value>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*>| Yes | String | The data URI to convert |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| [<*string-for-data-uri*>] | String | The string version for the data URI |
||||

*Example*

This example creates a string for this data URI:

```
dataUriToString('data:text/plain;charset=utf-8;base64,aGVsbG8=')
```

And returns this result: `"hello"`

<a name="date"></a>

### date

Return the date of a specified timestamp in "M/dd/yyyy" format.

```
date('<timestramp>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string contains the timestamp |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*date*> | String | The date of the specified timestamp |
||||

```
date('2018-03-15T13:00:00.000Z')
```

And it returns the result: `"3-15-2018"`

<a name="dateReadBack"></a>

### dateReadBack

Uses the date-time library to provide a date readback.

```
dateReadBack('<currentDate>', '<targetDate>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*currentDate*> | Yes | String | The string contains the current date |
| <*targetDate*> | Yes | String | The string contains the target date |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*date-readback*> | String | The readback between current date and the target date  |
||||

*Example 1*

```
dateReadBack('2018-03-15T13:00:00.000Z', '2018-03-16T13:00:00.000Z')
```

And it returns the result: ```tomorrow```

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
| <*dividend*> | Yes | Number | The number to divide by the *divisor* |
| <*divisor*> | Yes | Number | The number that divides the *dividend*, but cannot be 0 |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*quotient-result*> | Number | The result from dividing the first number by the second number |
||||

*Example*

Both examples divide the first number by the second number:

```
div(10, 5)
div(11, 5)
```

And return this result: `2`

If one of the parameters is Float type, the result would be a Float.

*Example*

```
div(11.2, 3)
```

And return the result `5.5`

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
| <*collection*> | Yes | Any | The collection to check |
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

<a name="endsWith"></a>

### endsWith

Check whether a string ends with a specific substring. Return true when the substring is found, or return false when not found. This function is not case-sensitive.

```
endsWith('<text>', '<searchText>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*text*> | Yes | String | The string to check |
| <*searchText*> | Yes | String | The ending substring to find |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| true or false | Boolean | Return true when the ending substring is found. Return false when not found |
||||

*Example 1*

This example checks whether the "hello world" string ends with the "world" string:

```
endsWith('hello world', 'world')
```

And it returns the result: `true`

*Example 2*

This example checks whether the "hello world" string ends with the "world" string:

```
endsWith('hello world', 'universe')
```

And it returns the result: `false`

<a name="equals"></a>

### equals

Check whether both values, expressions, or objects are equivalent.
Return true when both are equivalent, or return false when they're not equivalent.

```
equals('<object1>', '<object2>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*object1*>, <*object2*> | Yes | Any | The values, expressions, or objects to compare |
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
| <*result-exp*> | Number | The result from computing exponent of realNumber |
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

<a name="flatten"/>

### flatten

Flatten an array into non-array values.  With an optional depth flatten only to that depth.

```
flatten([<collection>], '<depth>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection*> | Yes | Array | The collection to flatten |
| <*depth*> | No | Number | Maximum depth to flatten, or infinity if not set|
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*new-collection*> | Array | new collection whose elements have been flattened to non-array to the specified depth |
||||

*Example 1*

```
flatten(createArray(1, createArray(2), createArray(createArray(3, 4), createArray(5, 6)))
```
This example will flatten the array to: ```[1, 2, 3, 4, 5, 6]```

*Example 2*

```
flatten(createArray(1, createArray(2), createArray(createArray(3, 4), createArray(5, 6)))
```

This example will only flatten the first level to: ```[1, 2, [3, 4], [5, 6]]```

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

<a name="foreach"></a>

### foreach

Operate on each element and return the new collection

```
foreach([<collection/instance>], <iteratorName>, <function>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection/instance*> | Yes | Array or Object | The collection with the items |
| <*iteratorName*> | Yes | Iterator Name | The key item of arrow function |
| <*function*> | Yes | Expression | function that can contains iteratorName |
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



These examples generate new collections from instance:

```
foreach(json("{'name': 'jack', 'age': '15'}"), x, concat(x.key, ':', x.value))
```

And return this result: `['name:jack', 'age:15']`

<a name="formatDateTime"></a>

### formatDateTime

Return a timestamp in the specified format.

```
formatDateTime('<timestamp>', '<format>'?)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string that contains the timestamp |
| <*format*> | No | String | A [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is UTC ISO format like YYYY-MM-DDTHH:mm:ss.fffZ, which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601). |
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

<a name="getFutureTime"></a>

### getFutureTime

Return the current timestamp plus the specified time units.

```
getFutureTime(<interval>, <timeUnit>, '<format>'?)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*interval*> | Yes | Integer | The number of specific time units to add |
| <*timeUnit*> | Yes | String | The unit of time to use with *interval*: "Second", "Minute", "Hour", "Day", "Week", "Month", "Year" |
| <*format*> | No | String | A [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is UTC ISO format like YYYY-MM-DDTHH:mm:ss.fffZ, which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601). |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updated-timestamp*> | String | The current timestamp plus the specified number of time units |
||||

*Example 1*

Suppose the current timestamp is "2019-03-01T00:00:00.000Z". This example adds five days to that timestamp:

```
getFutureTime(2, 'Week')
```

And returns this result: `"2019-03-15T00:00:00.000Z"`

*Example 2*

Suppose the current timestamp is "2018-03-01T00:00:00.000Z". This example adds five days and converts the result to "MM-DD-YY" format:

```
getFutureTime(5, 'Day', 'MM-DD-YY')
```

And returns this result: `'03-06-18'`.

<a name="getPastTime"></a>

### getPastTime

Return the current timestamp minus the specified time units.

```
getPastTime(<interval>, <timeUnit>, '<format>'?)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*interval*> | Yes | Integer | The number of specific time units to substract |
| <*timeUnit*> | Yes | String | The unit of time to use with *interval*: "Second", "Minute", "Hour", "Day", "Week", "Month", "Year" |
| <*format*> | No | String | A [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is UTC ISO format like YYYY-MM-DDTHH:mm:ss.fffZ, which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601). |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updated-timestamp*> | String | The current timestamp minus the specified number of time units |
||||

*Example 1*

Suppose the current timestamp is "2018-02-01T00:00:00.000Z". This example adds five days to that timestamp:

```
getPastTime(5, 'Day')
```

And returns this result: `"2019-01-27T00:00:00.000Z"`

*Example 2*

Suppose the current timestamp is "2018-03-01T00:00:00.000Z". This example adds five days and converts the result to "MM-DD-YY" format:

```
getPastTime(5, 'Day', 'MM-DD-YY')
```

And returns this result: `'02-26-18'`.

<a name="getProperty"></a>

### getProperty

Retrieve the value of the specified property from the JSON object.

```
getProperty(<JSONObject>, '<Property>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*JSONObject*> | Yes | Object | The JSON Object contains the property and value you want to get |
| <*property*> | Yes | String | The specified property you want to get from the JSON object |

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| value | Object | The value of the specified property you want to get in the JSON object|
||||

*Example*

item = {'name': 'myName', 'age': 18, 'state': ['single', 'junior', 'Grade A']}
getProperty(item, 'state')
And return with result, ['single', 'junior', 'Grade A'].

<a name="getTimeOfDay"></a>

### getTimeOfDay

Returns time of day for a given timestamp (midnight = 12AM, morning = 12:01AM – 11:59PM, noon = 12PM, afternoon = 12:01PM -05:59PM, evening = 06:00PM – 10:00PM, night = 10:01PM – 11:59PM).

```
getTimeOfDay('<timestamp>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string contains the specified timestamp |

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*time-of-day*> | String | the time of day for the specified timestamp (midnight = 12AM, morning = 12:01AM – 11:59PM, noon = 12PM, afternoon = 12:01PM -05:59PM, evening = 06:00PM – 10:00PM, night = 10:01PM – 11:59PM)|
||||

*Example*

```
getTimeOfDay('2018-03-15T08:00:00.000Z')
```

And it returns the result: "morning"

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

<a name="indexOf"></a>

### indexOf

Return the starting position or index value for a substring. This function is not case-sensitive, and indexes start with the number 0.

```
indexOf('<text>', '<searchText>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*text*> | Yes | String or Array   | The string that has the substring to find |
| <*searchText*> | Yes | String | The substring to find |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*index-value*> | Integer | The starting position or index value for the specified substring.
If the string is not found, return the number -1. |
||||

*Example*

This example finds the starting index value for the "world" substring in the "hello world" string:

```
indexOf('hello world', 'world')
```

And returns this result: `6`

This example finds the starting index value for the "def" substring in the Array ['abc', 'def', 'ghi']
```
indexOf(createArray('abc', 'def', 'ghi'), 'def')
```

And returns this result: `1`

<a name="indicesAndValues"></a>

### indicesAndValues

Turn an array or object into an array of objects with index (current index) and value property.  For arrays the index is the position in the array.  For objects it is the key for the value.

```
indicesAndValues('<collection or object>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection or object*> | Yes | Array | Original array or object |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*collection*> | Array | New array that each item has two properties, one is index with the position in an array or the key for an object, the other one is the corresponding value |
||||

*Example*

Suppose there is a list { items: ["zero", "one", "two"] }

```
indicesAndValues(items)
```

returns a new list:
```
[
  {
    index: 0,
    value: 'zero'
  },
  {
    index: 1,
    value: 'one'
  },
  {
    index: 2,
    value: 'two'
  }
]
```

second example:

```
where(indicesAndValues(items), elt, elt.index >= 1)
```

And returns a new list: 
```
[
  {
    index: 1,
    value: 'one'
  },
  {
    index: 2,
    value: 'two'
  }
]
```

Another example, with the same list `items`.

```
join(foreach(indicesAndValues(items), item, item.value), ',')
```

will return `zero,one,two`, and this expression has the same effect with `join(items, ',')`


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

<a name="intersection"></a>

### intersection

Return a collection that has only the common items across the specified collections. To appear in the result, an item must appear in all the collections passed to this function. If one or more items have the same name, the last item with that name appears in the result.

```
intersection([<collection1>], [<collection2>], ...)
intersection('<collection1>', '<collection2>', ...)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection1*>, <*collection2*>  | Yes | Array or Object, but not both | The collections from where you want only the common items |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*common-items*> | Array or Object, respectively | A collection that has only the common items across the specified collections |
||||

*Example*

This example finds the common items across these arrays:

```
intersection(createArray(1, 2, 3), createArray(101, 2, 1, 10), createArray(6, 8, 1, 2))
```

And returns an array with only these items: `[1, 2]`

<a name="isMatch"></a>

### isMatch

return a given string is match a common regex pattern

```
isMatch('<target_string>', '<pattern>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*target_string*> | Yes | String | the string to be matched |
| <*pattern*> | Yes | String | regex pattern |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*boolean-result*> | Boolean | Is the string matched the pattern |
||||

*Examples*


```
isMatch('ab', '^[a-z]{1,2}$')
isMatch('FUTURE', '(?i)fortune|future')
isMatch('12abc34', '([0-9]+)([a-z]+)([0-9]+)')
isMatch('abacaxc', 'ab.*?c')
```

And returns the same result: `true`

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


<a name="jPath"></a>

### jPath

Check JSON or JSON string for nodes or value that match a path expression, and return the matching nodes.

```
jPath(<json>, '<path>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*json*> | Yes | Any | The json object or string to search for nodes or values that match path expression value |
| <*path*> | Yes | Any | The path expression used to find matching json nodes or values |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
|[ <*json-node*>] | Array | An list of json nodes or value that matches the specified path expression |
||||

*C# Example*

Given jsonStr is 
```json
{
    "Stores": [
        "Lambton Quay",
        "Willis Street"
    ],
    "Manufacturers": [
        {
            "Name": "Acme Co",
            "Products": [
                {
                    "Name": "Anvil",
                    "Price": 50
                }
            ]
        },
        {
            "Name": "Contoso",
            "Products": [
                {
                    "Name": "Elbow Grease",
                    "Price": 99.95
                },
                {
                    "Name": "Headlight Fluid",
                    "Price": 4
                }
            ]
        }
    ]
}
```

and the path expression is 
"$..Products[?(@.Price >= 50)].Name" 

```
jPath(jsonStr, path)
```

And it returns this result: `["Anvil", "Elbow Grease"]`

Given jsonStr is 

```json
{
    "automobiles": [
        {
            "maker": "Nissan",
            "model": "Teana",
            "year": 2011
        },
        {
            "maker": "Honda",
            "model": "Jazz",
            "year": 2010
        },
        {
            "maker": "Honda",
            "model": "Civic",
            "year": 2007
        },
        {
            "maker": "Toyota",
            "model": "Yaris",
            "year": 2008
        },
        {
            "maker": "Honda",
            "model": "Accord",
            "year": 2011
        }
    ],
    "motorcycles": [
        {
            "maker": "Honda",
            "model": "ST1300",
            "year": 2012
        }
    ]
}
```

And the path expression is `.automobiles{.maker === "Honda" && .year > 2009}.model`.

```
jPath(jsonStr, path)
```

And it returns this result: `['Jazz', 'Accord']`

<a name="json"></a>

### json

Return the JavaScript Object Notation (JSON) type value or object for a string or XML.

```
json('<value>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*> | Yes | String or XML | The string or XML to convert |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*JSON-result*> | String | The resulting string created from all the items in the specified array |
||||

*Example 1*

This example converts this string to JSON:

```
json('{"fullName": "Sophia Owen"}')
```

And returns this result:

```
{
  "fullName": "Sophia Owen"
}
```

*Example 2*

This example converts this XML to JSON:

```
json(xml('<?xml version="1.0"?> <root> <person id='1'> <name>Sophia Owen</name> <occupation>Engineer</occupation> </person> </root>'))
```

And returns this result:

```
{
   "?xml": { "@version": "1.0" },
   "root": {
      "person": [ {
         "@id": "1",
         "name": "Sophia Owen",
         "occupation": "Engineer"
      } ]
   }
}
```



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

<a name="lastIndexOf"></a>

### lastIndexOf

Return the starting position or index value for the last occurrence of a substring. This function is not case-sensitive, and indexes start with the number 0.

```
lastIndexOf('<text>', '<searchText>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*text*> | Yes | String or Array | The string that has the substring to find |
| <*searchText*> | Yes | String | The substring to find |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*ending-index-value*> | Integer | The starting position or index value for the last occurrence of the specified substring.
If the string is not found, return the number -1. |
||||

*Example*

This example finds the starting index value for the last occurrence of the "world" substring in the "hello world" string:

```
lastIndexOf('hello world', 'world')
```

And returns this result: `6`

This example finds the starting index value for the last occurrence of "def" substring in the Array ['abc', 'def', 'ghi', 'def']
```
lastIndexOf(createArray('abc', 'def', 'ghi', 'def'), 'def')
```

And returns this result: `3`

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
| <*number1*>, <*number2*>, ... | Yes | Number | The set of numbers from which you want the highest value |
| [<*number1*>, <*number2*>, ...] | Yes | Array - Number | The array of numbers from which you want the highest value |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*max-value*> | Number | The highest value in the specified array or set of numbers |
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
| <*number1*>, <*number2*>, ... | Yes | Number | The set of numbers from which you want the lowest value |
| [<*number1*>, <*number2*>, ...] | Yes | Array - Number | The array of numbers from which you want the lowest value |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*min-value*> | Number | The lowest value in the specified set of numbers or specified array |
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
| <*dividend*> | Yes | Number | The number to divide by the *divisor* |
| <*divisor*> | Yes | Number | The number that divides the *dividend*, but cannot be 0. |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*modulo-result*> | Number | The remainder from dividing the first number by the second number |
||||

*Example*

This example divides the first number by the second number:

```
mod(3, 2)
```

And return this result: `1`

<a name="month"></a>

### month

Return the month of the specified timestamp

```
month('<timestamp>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string contains the timestamp |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*number-of-month*> | Integer | The number of the month in the specified timestamp |
||||

*Example*

```
month('2018-03-15T13:01:00.000Z')
```

And it returns the result: ```3```

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

<a name="newGuid"></a>

### newGuid

Return a new Guid string.

```
newGuid()
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*Guid-string*> | String | A new guid string, length is 36 and looks like *xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx*|
||||

*Example*

```
newGuid()
```

And it returns a result which follows the format: `xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx`

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

<a name="range"></a>

### range

Return an integer array that starts from a specified integer.

```
range(<startIndex>, <count>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*startIndex*> | Yes | Integer | An integer value that starts the array as the first item |
| <*count*> | Yes | Integer | The number of integers in the array |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*range-result*> | Integer | The array with integers starting from the specified index |
||||

*Example*

This example creates an integer array that starts from the specified index and has the specified number of integers:

```
range(1, 4)
```

And returns this result: `[1, 2, 3, 4]`

<a name="removeProperty"></a>

### removeProperty

Remove a property from an object and return the updated object.

```
removeProperty(<object>, '<property>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*object*> | Yes | Object | The JSON object from where you want to remove a property |
| <*property*> | Yes | String | The name for the property to remove |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updated-object*> | Object | The updated JSON object without the specified property |
||||

*Example*

This example removes the ```"accountLocation"``` property from a ```"customerProfile"``` object, which is converted to JSON with the JSON() function, and returns the updated object:

```
removeProperty(json('customerProfile'), 'accountLocation')
```

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


*Example 1*

This example finds the "old" substring in "the old string"
and replaces "old" with "new":

```
replace('the old string', 'old', 'new')
```

And returns this result: `"the new string"`

*Example 2*

When dealing with escape characters, the expression engine handles the unescape for you. Here is some example about the replace function with escape character cases:

```
replace('hello\"', '\"', '\n')
replace('hello\n', '\n', '\\\\')
@"replace('hello\\', '\\', '\\\\')"
@"replace('hello\n', '\n', '\\\\')"
```

And returns these results: `"hello\n"`, `"hello\\"`, `@"hello\\"`, `@"hello\\"`

<a name="replaceIgnoreCase"></a>

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

<a name="select"></a>

### select

Operate on each element and return the new collection of transformed elements.

```
select([<collection/instance>], <iteratorName>, <function>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection/instance*> | Yes | Array | The collection with the items |
| <*iteratorName*> | Yes | Iterator Name | The key item of arrow function |
| <*function*> | Yes | Expression | function that can contains iteratorName |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*new-collection*> | Array | the new collection which each element has been evaluated with the function  |
||||

*Example*

These examples generate new collections:

```
select(createArray(0, 1, 2, 3), x, x + 1)
```

And return this result: `[1, 2, 3, 4]`

These examples generate new collections from instance:

```
select(json("{'name': 'jack', 'age': '15'}"), x, concat(x.key, ':', x.value))
```

And return this result: `['name:jack', 'age:15']`

<a name="setPathToValue"></a>

### setPathToValue

Retrieve the value of the specified property from the JSON object.

```
setPathToValue(<path>, <value>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*Path*> | Yes | Object | the path which you want to set |
| <*value*> | Yes | Object | the value you want to set to the path |

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| value | Object | the value be set, same with the second parameter|
||||

*Example*
```
setPathToValue(path.x, 1)
```

And return with result: 1, and path.x has been set to 1.

```
setPathToValue(path.array[0], 7) + path.array[0]
```

return the result: 14

<a name="setProperty"></a>

### setProperty

Set the value for an object's property and return the updated object. To add a new property, you can use this function or the [addProperty()](#addProperty) function.

```
setProperty(<object>, '<property>', <value>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*object*> | Yes | Object | The JSON object from where you want to set a property |
| <*property*> | Yes | String | The name for the property to set |
| <*value*> | Yes | Any | The value to set for the specified property |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updated-object*> | Object | The updated JSON object whose property you set |
||||

*Example*

This example sets the ```"accountNumber"``` property on a ```"customerProfile"``` object, which is converted to JSON with the JSON() function. The function assigns a value generated by guid() function, and returns the updated JSON object:

```
setProperty(json('customerProfile'), 'accountNumber', guid())
```

<a name="skip"></a>

### skip

Remove items from the front of a collection, and return all the other items.

```
skip([<collection>], <count>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection*> | Yes | Array | The collection whose items you want to remove |
| <*count*> | Yes | Integer | A positive integer for the number of items to remove at the front |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updatedCollection*> | Array | The updated collection after removing the specified items |
||||

*Example*

This example removes one item, the number 0, from the front of the specified array:

```
skip(createArray(0, 1, 2, 3), 1)
```

And returns this array with the remaining items: `[1,2,3]`

<a name="sortBy"></a>

### sortBy

Sort elements in the collection with ascending order and return the sorted collection.

```
sortBy([<collection>], '<property>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection*> | Yes | String or Array | The collection to sort |
| <*property*> | No | String | Sort by this specific property of the object element in the collection if set|
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*new-collection*> | Array | the new collection whose elements have been sorted |
||||

*Example1*

This example generates new sorted collection:

```
sortBy(createArray(1, 2, 0, 3))
```

And return this result: `[0, 1, 2, 3]`

*Example2*
Suppose you have this collection:

```
{
  'nestedItems': [
    {'x': 2},
    {'x': 1},
    {'x': 3}
  ]
}
```

This example generates new sorted collection based on object property 'x':

```
sortBy(nestedItems, 'x')
```

And return this result:

```
{
  'nestedItems': [
    {'x': 1},
    {'x': 2},
    {'x': 3}
  ]
}
```

<a name="sortByDescending"></a>

### sortByDescending

Sort elements in the collection with descending order and return the sorted collection.

```
sortBy([<collection>], '<property>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection*> | Yes | String or Array | The collection to sort |
| <*property*> | No | String | Sort by this specific property of the object element in the collection if set|
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*new-collection*> | Array | the new collection whose elements have been sorted |
||||

*Example1*

This example generates new sorted collection:

```
sortByDescending(createArray(1, 2, 0, 3))
```

And return this result: `[3, 2, 1, 0]`

*Example2*
Suppose you have this collection:

```
{
  'nestedItems': [
    {'x': 2},
    {'x': 1},
    {'x': 3}
  ]
}
```

This example generates new sorted collection based on object property 'x':

```
sortByDescending(nestedItems, 'x')
```

And return this result:

```
{
  'nestedItems': [
    {'x': 3},
    {'x': 2},
    {'x': 1}
  ]
}
```

<a name="split"></a>

### split

Return an array that contains substrings, separated by commas,
based on the specified delimiter character in the original string.

```
split('<text>', '<delimiter>'?)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*text*> | Yes | String | The string to separate into substrings based on the specified delimiter in the original string, if the text is a null value, it will be taken as an empty string |
| <*delimiter*> | No | String | The character in the original string to use as the delimiter, if no delimiter given or delimiter is a null value, the default value will be an empty string |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| [<*substring1*>,<*substring2*>,...] | Array | An array that contains substrings from the original string, separated by commas |
||||

*Examples*

This example creates an array with substrings from the specified
string based on the specified character as the delimiter:

```
split('a_b_c', '_')
split('hello', '')
split('', 'e')
split('', '')
split('hello')
```

And returns these arrays as the result: `["a", "b", "c"]`, `["h", "e", "l", "l", "o"]`, `[""]`, `[ ]`, `["h", "e", "l", "l", "o"]`.

<a name="startOfDay"></a>

### startOfDay

Return the start of the day for a timestamp.

```
startOfDay('<timestamp>', '<format>'?)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string that contains the timestamp |
| <*format*> | No | String | A [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is UTC ISO format like YYYY-MM-DDTHH:mm:ss.fffZ, which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601). |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| '<*updated-timestamp*>'| String | The specified timestamp but starting at the zero-hour mark for the day |
||||

*Example*

This example finds the start of the day for this timestamp:

```
startOfDay('2018-03-15T13:30:30.000Z')
```

And returns this result: `"2018-03-15T00:00:00.000Z"`

<a name="startOfHour"></a>

### startOfHour

Return the start of the hour for a timestamp.

```
startOfHour('<timestamp>', '<format>'?)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string that contains the timestamp |
| <*format*> | No | String | A [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is UTC ISO format like YYYY-MM-DDTHH:mm:ss.fffZ, which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601). |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| '<*updated-timestamp*>'| String | The specified timestamp but starting at the zero-minute mark for the day |
||||

*Example*

This example finds the start of the hour for this timestamp:

```
startOfHour('2018-03-15T13:30:30.000Z')
```

And returns this result: `"2018-03-15T13:00:00.000Z"`

<a name="startOfMonth"></a>

### startOfMonth

Return the start of the month for a timestamp.

```
startOfDay('<timestamp>', '<format>'?)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string that contains the timestamp |
| <*format*> | No | String | A [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is UTC ISO format like YYYY-MM-DDTHH:mm:ss.fffZ, which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601). |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| '<*updated-timestamp*>'| String | The specified timestamp but starting on the first day of the month at the zero-hour mark |
||||

*Example*

This example finds the start of the month for this timestamp:

```
startOfDay('2018-03-15T13:30:30.000Z')
```

And returns this result: `"2018-03-01T00:00:00.000Z"`

<a name="startsWith"></a>

### startsWith

Check whether a string starts with a specific substring. Return true when the substring is found, or return false when not found. This function is not case-sensitive.

```
startsWith('<text>', '<searchText>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*text*> | Yes | String | The string to check |
| <*searchText*> | Yes | String | The starting substring to find |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| true or false | Boolean | Return true when the starting substring is found. Return false when not found |
||||

*Example 1*

This example checks whether the "hello world" string starts with the "world" string:

```
startsWith('hello world', 'hello')
```

And it returns the result: `true`

*Example 2*

This example checks whether the "hello world" string starts with the "world" string:

```
startsWith('hello world', 'greeting')
```

And it returns the result: `false`

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
| <*minuend*> | Yes | Number | The number from which to subtract the *subtrahend* |
| <*subtrahend*> | Yes | Number | The number to subtract from the *minuend* |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*result*> | Number | The result from subtracting the second number from the first number |
||||

*Example*

This example subtracts the second number from the first number:

```
sub(10.3, .3)
```

And returns this result: `10`

<a name="subArray"></a>

### subArray

Returns a sub-array from specified start and end position. Index values start with the number 0.

```
subArray(<Array>, <startIndex>, <endIndex>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*array*> | Yes | Array | The array whose items you want |
| <*startIndex*> | Yes | Integer | A positive number equal to or greater than 0 that you want to use as the starting position or index value |
| <*endIndex*> | Yes | Integer |  A positive number equal to or greater than 0 that you want to use as the ending position or index value|
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*sub-array-result*> | Array | A sub-array with the specified number of items, starting at the specified index position in the source string |
||||

*Example*

This example creates a sub-array from the specified array,
starting from the index value 2 and ending at the index of 5:

```
subArray(createArray('H','e','l','l','o'), 2, 5)
```

And returns this result: `["l", "l", "o"]`

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
| <*format*> | No | String | A [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is UTC ISO format like YYYY-MM-DDTHH:mm:ss.fffZ, which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601). |
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

And returns this result: `"2018-01-01T00:00:00:000Z"`

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
| [\<list of numbers\>] | Yes | Array - Number | The numbers to add |
|||||

| Return value | Type | Description |
| ------------ | -----| ----------- |
| <*result-sum*> | Number | The result from adding the specified numbers |
||||

*Example*

This example adds the specified numbers:

```
add(createArray(1, 1.5))
```

And returns this result: `2.5`

<a name="take"></a>

### take

Return items from the front of a collection.

```
take('<collection>', <count>)
take([<collection>], <count>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection*> | Yes | String or Array | The collection whose items you want |
| <*count*> | Yes | Integer | A positive integer for the number of items that you want from the front |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*subset*> or [<*subset*>]| String or Array, respectively | A string or array that has the specified number of items taken from the front of the original collection |
||||

*Example*

These examples get the specified number of items from the front of these collections:

```
take('abcde', 3)
take(createArray(0, 1, 2, 3, 4), 3)
```

And return these results:

- First example: `"abc"`
- Second example: `[0, 1, 2]`

<a name='ticks'></a>

### ticks

Return the ticks property value for a specified timestamp. A tick is 100-nanosecond interval.

```
ticks('<timestamp>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*>| Yes | string | The string for a timestamp |
|||||

| Return value | Type | Description |
| ------------ | -----| ----------- |
| <*ticks-number*> | Integer | The number of ticks since the specified timestamp |
||||

*Example*
The example convert a timestamp to its ticks property

```
ticks('2018-01-01T08:00:00.000Z')
```

And returns this result: `636503904000000000`

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

<a name="union"></a>

### union

Return a collection that has all the items from the specified collections. To appear in the result, an item can appear in any collection passed to this function. If one or more items have the same name, the last item with that name appears in the result.

```
union('<collection1>', '<collection2>', ...)
union([<collection1>], [<collection2>], ...)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection1*>, <*collection2*>, ...| Yes | Array or Object, but not both | The collections from where you want all the items |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*updatedCollection*> | Array or Object, respectively | A collection with all the items from the specified collections - no duplicates |
||||

*Example*

This example gets all the items from these collections:

```
union(createArray(1, 2, 3), createArray(1, 2, 10, 101))
```

And returns this result: `[1, 2, 3, 10, 101]`

<a name="unique"/>

### unique

```
unique([<collection>])
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection*> | Yes | Array | The collection to modify |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*new-collection*> | Array | New collection with duplicate element removed |
||||

*Example 1*

```
unique(createArray(1, 2, 1))
```

This will remove the duplicate 1 and produce: ```[1, 2]```

<a name="uriComponent"></a>

### uriComponent

Return the binary version for a uniform resource identifier (URI) component.

```
uriComponent('<value>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*> | Yes | String | The string to convert to URI-encoded format |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*encoded-uri*> | String | The URI-encoded string with escape characters |
||||

*Example*

This example creates a URI-encoded version for this string:

```
uriComponent('https://contoso.com')
```

And returns this result: `"http%3A%2F%2Fcontoso.com"`

<a name="uriComponentToString"></a>

### uriComponentToString

Return the string version for a uniform resource identifier (URI) encoded string, effectively decoding the URI-encoded string.

```
uriComponentToString('<value>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*> | Yes | String | The URI-encoded string to decode |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*binary-for-encoded-uri*> | String | The decoded version for the URI-encoded string |
||||

*Example*

This example creates the decoded string version for this URI-encoded string:

```
uriComponentToString('http%3A%2F%2Fcontoso.com')
```

And returns this result: `"https://contoso.com"`

<a name="uriHost"></a>

### uriHost

Return the host value for a unified resource identifier(URI).

```
uriHost('<uri>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*uri*> | Yes | String | The URI whose host value you want |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*host-value*> | String | The host value for the specified URI |
||||

*Example*

This example finds the host value for this URI:

```
uriHost('https://www.localhost.com:8080')
```

And returns this result: `"www.localhost.com"`

<a name="uriPath"></a>

### uriPath

Return the path value for a unified resource identifier(URI).

```
uriPath('<uri>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*uri*> | Yes | String | The URI whose path value you want |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*path-value*> | String | the path value for the specified URI |
||||

*Example*

This example finds the path value for this URI:

```
uriPath('http://www.contoso.com/catalog/shownew.htm?date=today')
```

And returns this result: `"/catalog/shownew.htm"`

<a name="uriPathAndQuery"></a>

### uriPathAndQuery

Return the path and query value for a unified resource identifier(URI).

```
uriPathAndQuery('<uri>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*uri*> | Yes | String | The URI whose path and query value you want |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*path-query-value*> | String | the path and query value for the specified URI |
||||

*Example*

This example finds the path and query value for this URI:

```
uriPathAndQuery('http://www.contoso.com/catalog/shownew.htm?date=today')
```

And returns this result: `"/catalog/shownew.htm?date=today"`

<a name="uriPort"></a>

### uriPort

Return the port value for a unified resource identifier(URI).

```
uriPort('<uri>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*uri*> | Yes | String | The URI whose path value you want |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*port-value*> | String | the port value for the specified URI |
||||

*Example*

This example finds the portvalue for this URI:

```
uriPort('http://www.localhost:8080')
```

And returns this result: `8080`

<a name="uriQuery"></a>

### uriQuery

Return the query value for a unified resource identifier(URI).

```
uriQuery('<uri>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*uri*> | Yes | String | The URI whose query value you want |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*query-value*> | String | the query value for the specified URI |
||||

*Example*

This example finds the query value for this URI:

```
uriQuery('http://www.contoso.com/catalog/shownew.htm?date=today')
```

And returns this result: `"?date=today"`

<a name="uriScheme"></a>

### uriScheme

Return the scheme value for a unified resource identifier(URI).

```
uriScheme('<uri>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*uri*> | Yes | String | The URI whose query value you want |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*scheme-value*> | String | the scheme value for the specified URI |
||||

*Example*

This example finds the scheme value for this URI:

```
uriQuery('http://www.contoso.com/catalog/shownew.htm?date=today')
```

And returns this result: `"http"`

<a name="utcNow"></a>

### utcNow

Return the current timestamp.

```
utcNow('<format>')
```

Optionally, you can specify a different format with the <*format*> parameter.


| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*format*> | No | String | A [custom format pattern](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings). The default format for the timestamp is UTC ISO format like YYYY-MM-DDTHH:mm:ss.fffZ, which complies with [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601). |
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

And returns this result: `"2018-04-15T13:00:00.000Z"`

*Example 2*

Suppose today is April 15, 2018 at 1:00:00 PM.
This example gets the current timestamp using the optional "D" format:

```
utcNow('D')
```

And returns this result: `"Sunday, April 15, 2018"`

<a name="where"></a>

### where

Filter on each element and return the new collection of filtered elements which match specific condition.

```
where([<collection/instance>], <iteratorName>, <function>)
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*collection/instance*> | Yes | Array | The collection with the items |
| <*iteratorName*> | Yes | Iterater Name | The key item of arrow function |
| <*function*> | Yes | Expression | condition function which is used to filter items|
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*new-collection/new-object*> | Array/Object | the new collection which each element has been filtered with the function  |
||||

*Example*

These examples generate new collections:

```
where(createArray(0, 1, 2, 3), x, x > 1)
```

And return this result: `[2, 3]`

These examples generate new object:

```
where(json("{'name': 'jack', 'age': '15'}"), x, x.value == 'jack')
```

And return this result: `{'name': 'jack'}`


<a name="xml"></a>

### xml

[C# only] Return the XML version for a string that contains a JSON object.

```
xml('<value>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*value*> | Yes | String | The string with the JSON object to convert
The JSON object must have only one root property, which can't be an array. 
Use the backslash character (\) as an escape character for the double quotation mark ("). |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*xml-version*> | Object | The encoded XML for the specified string or JSON object |
||||

*Example 1*

This example creates the XML version for this string, which contains a JSON object:

`xml(json('{ \"name\": \"Sophia Owen\" }'))`

And returns this result XML:

```
<name>Sophia Owen</name>
```

*Example 2*

Suppose you have this JSON object:

```
{
  "person": {
    "name": "Sophia Owen",
    "city": "Seattle"
  }
}
```

This example creates XML for a string that contains this JSON object:

`xml(json('{\"person\": {\"name\": \"Sophia Owen\", \"city\": \"Seattle\"}}'))`

And returns this result XML:

```
<person>
  <name>Sophia Owen</name>
  <city>Seattle</city>
<person
```

<a name="xPath"></a>

### xPath

[C# only] Check XML for nodes or values that match an XPath (XML Path Language) expression, and return the matching nodes or values. An XPath expression, or just "XPath", helps you navigate an XML document structure so that you can select nodes or compute values in the XML content.

```
xPath('<xml>', '<xpath>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*xml*> | Yes | Any | The XML string to search for nodes or values that match an XPath expression value |
| <*xPath*> | Yes | Any | The XPath expression used to find matching XML nodes or values |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*xml-node*> | XML | An XML node when only a single node matches the specified XPath expression |
| <*value*> | Any | The value from an XML node when only a single value matches the specified XPath expression |
<*[<xml-node1>, <xml-node2>, ...] -or- [<value1>, <value2>, ...]*> | Array | An array with XML nodes or values that match the specified XPath expression |
||||

*Example 1*

This example finds nodes that match the <name></name> node in the specified arguments, and returns an array with those node values:

```
xpath(items, '/produce/item/name')
```

Here are the arguments:

- The "items" string, which contains this XML: 

```
"<?xml version="1.0"?> <produce> <item> <name>Gala</name> <type>apple</type> <count>20</count> </item> <item> <name>Honeycrisp</name> <type>apple</type> <count>10</count> </item> </produce>"
```

Here is the result array with the nodes that match ```<name></name```:

```
[ <name>Gala</name>, <name>Honeycrisp</name> ]
```

*Example 2*

Following on Example 1, this example finds nodes that match the <count></count> node and adds those node values with the sum() function:

```
xpath(xml(parameters('items')), 'sum(/produce/item/count)')
```

And returns this result: ```30```

<a name="year"></a>

### year

Return the year of the specified timestamp

```
year('<timestamp>')
```

| Parameter | Required | Type | Description |
| --------- | -------- | ---- | ----------- |
| <*timestamp*> | Yes | String | The string contains the timestamp |
|||||

| Return value | Type | Description |
| ------------ | ---- | ----------- |
| <*year*> | Integer | The year in the specified timestamp |
||||

*Example*

```
year('2018-03-15T00:00:00.000Z')
```

And it returns the result: ```2018```

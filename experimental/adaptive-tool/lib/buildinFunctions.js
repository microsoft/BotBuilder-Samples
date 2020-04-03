"use strict";
/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
const adaptive_expressions_1 = require("adaptive-expressions");
class FunctionEntity {
    constructor(params, returntype, introduction) {
        this.params = params;
        this.returntype = returntype;
        this.introduction = introduction;
    }
}
exports.FunctionEntity = FunctionEntity;
// https://github.com/microsoft/BotBuilder-Samples/blob/master/experimental/common-expression-language/prebuilt-functions.md
exports.buildInfunctionsMap = new Map([
    [
        'add',
        new FunctionEntity(['num1: number', 'num2: number'], adaptive_expressions_1.ReturnType.Number, 'Return the result from adding two numbers.'),
    ],
    [
        'div',
        new FunctionEntity(['dividend: number', 'divisor: number'], adaptive_expressions_1.ReturnType.Number, 'Return the integer result from dividing two numbers. To get the remainder result, see mod().'),
    ],
    [
        'mod',
        new FunctionEntity(['dividend: number', 'divisor: number'], adaptive_expressions_1.ReturnType.Number, 'Return the remainder from dividing two numbers. To get the integer result, see div().'),
    ],
    [
        'mul',
        new FunctionEntity(['multiplicand1: number', 'multiplicand2: number'], adaptive_expressions_1.ReturnType.Number, 'Return the product from multiplying two numbers.'),
    ],
    [
        'sub',
        new FunctionEntity(['minuend: number', 'subtrahend: number'], adaptive_expressions_1.ReturnType.Number, 'Return the result from subtracting the second number from the first number.'),
    ],
    [
        'exp',
        new FunctionEntity(['minuend: number', 'subtrahend: number'], adaptive_expressions_1.ReturnType.Number, 'Return exponentiation of one number to another.'),
    ],
    [
        'concat',
        new FunctionEntity(['...strings: string[]'], adaptive_expressions_1.ReturnType.String, 'Combine two or more strings and return the resulting string. E.g. concat(‘hello’, ‘world’, ‘…’)'),
    ],
    [
        'not',
        new FunctionEntity(['expression: bool'], adaptive_expressions_1.ReturnType.Boolean, 'Check whether an expression is false. Return true when the expression is false, or return false when true.'),
    ],
    [
        'and',
        new FunctionEntity(['...input: any[]'], adaptive_expressions_1.ReturnType.Boolean, 'Check whether all expressions are true. Return true when all expressions are true, or return false when at least one expression is false.'),
    ],
    [
        'or',
        new FunctionEntity(['...input: any[]'], adaptive_expressions_1.ReturnType.Boolean, 'Check whether at least one expression is true. Return true when at least one expression is true, or return false when all are false.'),
    ],
    [
        'equals',
        new FunctionEntity(['...input: any[]'], adaptive_expressions_1.ReturnType.Boolean, 'Comparison equal. Returns true if specified values are equal'),
    ],
    [
        'greater',
        new FunctionEntity(['value: any', 'compareTo: any'], adaptive_expressions_1.ReturnType.Boolean, 'Check whether the first value is greater than the second value. Return true when the first value is more, or return false when less.'),
    ],
    [
        'greaterOrEquals',
        new FunctionEntity(['value: any', 'compareTo: any'], adaptive_expressions_1.ReturnType.Boolean, 'Check whether the first value is greater than or equal to the second value. Return true when the first value is greater or equal, or return false when the first value is less.'),
    ],
    [
        'less',
        new FunctionEntity(['value: any', 'compareTo: any'], adaptive_expressions_1.ReturnType.Boolean, 'Check whether the first value is less than the second value. Return true when the first value is less, or return false when the first value is more.'),
    ],
    [
        'lessOrEquals',
        new FunctionEntity(['value: any', 'compareTo: any'], adaptive_expressions_1.ReturnType.Boolean, 'Check whether the first value is less than or equal to the second value. Return true when the first value is less than or equal, or return false when the first value is more.'),
    ],
    [
        'join',
        new FunctionEntity(['collection: Array', 'delimiter: string'], adaptive_expressions_1.ReturnType.String, 'Return a string that has all the items from an array and has each character separated by a delimiter.'),
    ],
    ['empty', new FunctionEntity(['collection: any'], adaptive_expressions_1.ReturnType.Boolean, 'Check if the collection is empty')],
    ['newGuid', new FunctionEntity([], adaptive_expressions_1.ReturnType.String, 'Return new guid string')],
    [
        'min',
        new FunctionEntity(['...numbers: number[]'], adaptive_expressions_1.ReturnType.Number, 'Returns the smallest value from a collection'),
    ],
    [
        'max',
        new FunctionEntity(['...numbers: number[]'], adaptive_expressions_1.ReturnType.Number, 'Returns the largest value from a collection'),
    ],
    [
        'average',
        new FunctionEntity(['...numbers: number[]'], adaptive_expressions_1.ReturnType.Number, 'Returns the average value from a collection'),
    ],
    [
        'sum',
        new FunctionEntity(['...numbers: number[]'], adaptive_expressions_1.ReturnType.Number, 'Return the result from adding numbers in a list.'),
    ],
    [
        'exists',
        new FunctionEntity(['expression: expression'], adaptive_expressions_1.ReturnType.Boolean, 'Returns the smallest value from a collection'),
    ],
    ['length', new FunctionEntity(['str: string'], adaptive_expressions_1.ReturnType.Number, 'Returns the length of a string')],
    [
        'replace',
        new FunctionEntity(['text: string', 'oldText: string', 'newText: string'], adaptive_expressions_1.ReturnType.String, 'Replace a substring with the specified string, and return the updated string. case sensitive'),
    ],
    [
        'replaceIgnoreCase',
        new FunctionEntity(['text: string', 'oldText: string', 'newText: string'], adaptive_expressions_1.ReturnType.String, 'Replace a substring with the specified string, and return the updated string. Case in-sensitive'),
    ],
    [
        'split',
        new FunctionEntity(['text: string', 'delimiter: string'], adaptive_expressions_1.ReturnType.Object, 'Returns an array that contains substrings based on the delimiter specified.'),
    ],
    [
        'substring',
        new FunctionEntity(['text: string', 'startIndex: number', 'length?: number'], adaptive_expressions_1.ReturnType.String, 'Returns characters from a string. Substring(sourceString, startPos, endPos). startPos cannot be less than 0. endPos greater than source strings length will be taken as the max length of the string'),
    ],
    ['toLower', new FunctionEntity(['text: string'], adaptive_expressions_1.ReturnType.String, 'Convert a string to all upper case characters')],
    ['toUpper', new FunctionEntity(['text: string'], adaptive_expressions_1.ReturnType.String, 'Convert a string to all lower case characters')],
    [
        'trim',
        new FunctionEntity(['text: string'], adaptive_expressions_1.ReturnType.String, 'Remove leading and trailing white spaces from a string'),
    ],
    [
        'count',
        new FunctionEntity(['collection: string|Array'], adaptive_expressions_1.ReturnType.Number, 'Returns the number of items in the collection'),
    ],
    [
        'contains',
        new FunctionEntity(['collection: stirng|Array|Map', 'value: stirng|Array|Map'], adaptive_expressions_1.ReturnType.Boolean, 'Works to find an item in a string or to find an item in an array or to find a parameter in a complex object. E.g. contains(‘hello world, ‘hello); contains([‘1’, ‘2’], ‘1’); contains({“foo”:”bar”}, “foo”)'),
    ],
    [
        'first',
        new FunctionEntity(['collection: string|Array'], adaptive_expressions_1.ReturnType.Object, 'Returns the first item from the collection'),
    ],
    [
        'last',
        new FunctionEntity(['collection: string|Array'], adaptive_expressions_1.ReturnType.Object, 'Returns the last item from the collection'),
    ],
    [
        'foreach',
        new FunctionEntity(['collection: Array | Object', 'iteratorName: string', 'function: any'], adaptive_expressions_1.ReturnType.Object, 'Operate on each element and return the new collection'),
    ],
    [
        'addDays',
        new FunctionEntity(['timestamp: string', 'days: number', 'format?: string'], adaptive_expressions_1.ReturnType.String, 'Add number of specified days to a given timestamp'),
    ],
    [
        'addHours',
        new FunctionEntity(['timestamp: string', 'hours: number', 'format?: string'], adaptive_expressions_1.ReturnType.String, 'Add specified number of hours to a given timestamp'),
    ],
    [
        'addMinutes',
        new FunctionEntity(['timestamp: string', 'minutes: number', 'format?: string'], adaptive_expressions_1.ReturnType.String, 'Add specified number of minutes to a given timestamp'),
    ],
    [
        'addSeconds',
        new FunctionEntity(['timestamp: string', 'seconds: number', 'format?: string'], adaptive_expressions_1.ReturnType.String, 'Add specified number of seconds to a given timestamp'),
    ],
    [
        'dayOfMonth',
        new FunctionEntity(['timestamp: string'], adaptive_expressions_1.ReturnType.Number, 'Returns day of month for a given timestamp or timex expression.'),
    ],
    [
        'dayOfWeek',
        new FunctionEntity(['timestamp: string'], adaptive_expressions_1.ReturnType.Number, 'Return the day of the week from a timestamp.'),
    ],
    [
        'dayOfYear',
        new FunctionEntity(['timestamp: string'], adaptive_expressions_1.ReturnType.Number, 'Return the day of the year from a timestamp.'),
    ],
    ['month', new FunctionEntity(['timestamp: string'], adaptive_expressions_1.ReturnType.Number, 'Returns the month of given timestamp')],
    [
        'date',
        new FunctionEntity(['timestamp: string'], adaptive_expressions_1.ReturnType.Number, 'Return the date of a specified timestamp in "M/dd/yyyy" format.'),
    ],
    ['year', new FunctionEntity(['timestamp: string'], adaptive_expressions_1.ReturnType.Number, 'Returns year for the given timestamp')],
    ['utcNow', new FunctionEntity(['format?: string'], adaptive_expressions_1.ReturnType.String, 'Returns current timestamp as string')],
    [
        'formatDateTime',
        new FunctionEntity(['timestamp: string', 'format?: string'], adaptive_expressions_1.ReturnType.String, 'Return a timestamp in the specified format.'),
    ],
    [
        'subtractFromTime',
        new FunctionEntity(['timestamp: string', 'interval: number', 'timeUnit: string', 'format?: string'], adaptive_expressions_1.ReturnType.String, 'Subtract a number of time units from a timestamp.'),
    ],
    [
        'dateReadBack',
        new FunctionEntity(['currentDate: string', 'targetDate: string'], adaptive_expressions_1.ReturnType.String, 'Uses the date-time library to provide a date readback. dateReadBack(currentDate, targetDate). E.g. dateReadBack(‘2016/05/30’,’2016/05/23’)=>"Yesterday"'),
    ],
    [
        'getTimeOfDay',
        new FunctionEntity(['timestamp: string'], adaptive_expressions_1.ReturnType.String, 'Returns time of day for a given timestamp (midnight = 12AM, morning = 12:01AM – 11:59PM, noon = 12PM, afternoon = 12:01PM -05:59PM, evening = 06:00PM – 10:00PM, night = 10:01PM – 11:59PM)'),
    ],
    [
        'float',
        new FunctionEntity(['value: string'], adaptive_expressions_1.ReturnType.Number, 'Return floating point representation of the specified string or the string itself if conversion is not possible'),
    ],
    [
        'int',
        new FunctionEntity(['value: string'], adaptive_expressions_1.ReturnType.Number, 'Return integer representation of the specified string or the string itself if conversion is not possible'),
    ],
    ['string', new FunctionEntity(['value: any'], adaptive_expressions_1.ReturnType.String, 'Return string version of the specified value')],
    [
        'bool',
        new FunctionEntity(['value: any'], adaptive_expressions_1.ReturnType.Boolean, 'Return Boolean representation of the specified string. Bool(‘true’), bool(1)'),
    ],
    ['createArray', new FunctionEntity(['...objects: any[]'], adaptive_expressions_1.ReturnType.Object, 'Create an array from multiple inputs')],
    [
        'if',
        new FunctionEntity(['expression: boolean', 'valueIfTrue: any', 'valueIfFalse: any'], adaptive_expressions_1.ReturnType.Object, 'if(exp, valueIfTrue, valueIfFalse)'),
    ],
    [
        'rand',
        new FunctionEntity(['minValue: number', 'maxValue: number'], adaptive_expressions_1.ReturnType.Number, 'Returns a random number between specified min and max value – rand(<minValue>, <maxValue>)'),
    ],
    [
        'json',
        new FunctionEntity(['value: string|XML'], adaptive_expressions_1.ReturnType.String, 'Return the JavaScript Object Notation (JSON) type value or object for a string or XML.'),
    ],
    [
        'getProperty',
        new FunctionEntity(['jsobObject: any', 'property: string'], adaptive_expressions_1.ReturnType.Object, 'Return the value of the given property in a JSON object.'),
    ],
    [
        'addProperty',
        new FunctionEntity(['jsobObject: any', 'property: string', 'value: any'], adaptive_expressions_1.ReturnType.Object, 'Add a property and its value, or name-value pair, to a JSON object, and return the updated object. If the object already exists at runtime, the function throws an error.'),
    ],
    [
        'removeProperty',
        new FunctionEntity(['jsobObject: any', 'property: string'], adaptive_expressions_1.ReturnType.Object, 'Remove a property from an object and return the updated object.'),
    ],
    [
        'setProperty',
        new FunctionEntity(['jsobObject: any', 'property: string', 'value: any'], adaptive_expressions_1.ReturnType.Object, "Set the value for an object's property and return the updated object. To add a new property, you can use this function or the addProperty() function."),
    ],
    [
        'endsWith',
        new FunctionEntity(['text: string', 'value: string'], adaptive_expressions_1.ReturnType.Boolean, 'Return if a text is end with another string'),
    ],
    [
        'startsWith',
        new FunctionEntity(['text: string', 'value: string'], adaptive_expressions_1.ReturnType.Boolean, 'Return if a text is end with another string'),
    ],
    ['countWord', new FunctionEntity(['text: string'], adaptive_expressions_1.ReturnType.Number, 'Returns the word count')],
    ['addOrdinal', new FunctionEntity(['num: number'], adaptive_expressions_1.ReturnType.String, 'e.g. addOrdinal(10) = 10th')],
    [
        'indexOf',
        new FunctionEntity(['text: string', 'value: string'], adaptive_expressions_1.ReturnType.Number, 'Returns the index of the value from the text'),
    ],
    [
        'lastIndexOf',
        new FunctionEntity(['text: string', 'value: string'], adaptive_expressions_1.ReturnType.Number, 'Returns the last index of the value from the text'),
    ],
    [
        'union',
        new FunctionEntity(['...values: Array[]'], adaptive_expressions_1.ReturnType.Object, 'Produces the set union of two sequences by using the default equality comparer.'),
    ],
    [
        'intersection',
        new FunctionEntity(['...values: Array[]'], adaptive_expressions_1.ReturnType.Object, ' Produces the set intersection of two sequences by using the default equality comparer to compare values.'),
    ],
    [
        'skip',
        new FunctionEntity(['array: Array', 'length: number'], adaptive_expressions_1.ReturnType.Object, 'Bypasses a specified number of elements in a sequence and then returns the remaining elements.'),
    ],
    [
        'take',
        new FunctionEntity(['array: Array', 'length: number'], adaptive_expressions_1.ReturnType.Object, 'Returns a specified number of contiguous elements from the start of a sequence.'),
    ],
    [
        'subArray',
        new FunctionEntity(['array: Array', 'startIndex: number', 'endIndex: number'], adaptive_expressions_1.ReturnType.Object, 'Returns the sub array from start index to end index'),
    ],
    ['array', new FunctionEntity(['value: any'], adaptive_expressions_1.ReturnType.Object, 'Create a new array with single value ')],
    ['binary', new FunctionEntity(['value: string'], adaptive_expressions_1.ReturnType.String, 'Return the binary version for an input value.')],
    [
        'dataUri',
        new FunctionEntity(['value: string'], adaptive_expressions_1.ReturnType.String, 'Return a data uniform resource identifier (URI) for a string.'),
    ],
    [
        'dataUriToBinary',
        new FunctionEntity(['value: string'], adaptive_expressions_1.ReturnType.String, 'Return the binary version for a data uniform resource identifier (URI). Use this function rather than decodeDataUri(). Although both functions work the same way, dataUriBinary() is preferred.'),
    ],
    [
        'dataUriToString',
        new FunctionEntity(['value: string'], adaptive_expressions_1.ReturnType.String, 'Return the string version for a data uniform resource identifier (URI).'),
    ],
    [
        'base64',
        new FunctionEntity(['value: string'], adaptive_expressions_1.ReturnType.String, 'Return the base64-encoded version for a string.'),
    ],
    [
        'base64ToBinary',
        new FunctionEntity(['value: string'], adaptive_expressions_1.ReturnType.String, 'Return the binary version for a base64-encoded string.'),
    ],
    [
        'base64ToString',
        new FunctionEntity(['value: string'], adaptive_expressions_1.ReturnType.String, 'Return the string version for a base64-encoded string, effectively decoding the base64 string. Use this function rather than decodeBase64. Although both functions work the same way, base64ToString() is preferred.'),
    ],
    [
        'uriComponent',
        new FunctionEntity(['value: string'], adaptive_expressions_1.ReturnType.String, 'Return the binary version for a uniform resource identifier (URI) component.'),
    ],
    [
        'uriComponentToString',
        new FunctionEntity(['value: string'], adaptive_expressions_1.ReturnType.String, 'Return the string version for a uniform resource identifier (URI) encoded string, effectively decoding the URI-encoded string.'),
    ],
    //TODO. Make sure xml can be used in both browser/node environment
    //['xml', new FunctionEntity(['xmlStr: string]'], ReturnType.Object, 'Return the XML version for a string.')],
    [
        'range',
        new FunctionEntity(['startIndex: number', 'count: number'], adaptive_expressions_1.ReturnType.Object, 'Return an integer array that starts from a specified integer.'),
    ],
    [
        'getFutureTime',
        new FunctionEntity(['interval: number', 'timeUnit: string', 'format?: string'], adaptive_expressions_1.ReturnType.String, 'Return the current timestamp plus the specified time units.'),
    ],
    [
        'getPastTime',
        new FunctionEntity(['interval: number', 'timeUnit: string', 'format?: string'], adaptive_expressions_1.ReturnType.String, 'Return the current timestamp minus the specified time units.'),
    ],
    [
        'addToTime',
        new FunctionEntity(['timestamp: string', 'interval: number', 'timeUnit: string', 'format?: string'], adaptive_expressions_1.ReturnType.String, 'Add a number of time units to a timestamp. See also getFutureTime()'),
    ],
    [
        'convertFromUtc',
        new FunctionEntity(['timestamp: string', 'destinationTimeZone: string', 'format?: string'], adaptive_expressions_1.ReturnType.String, 'Convert a timestamp from Universal Time Coordinated(UTC) to target time zone.'),
    ],
    [
        'convertToUtc',
        new FunctionEntity(['timestamp: string', 'sourceTimeZone: string', 'format?: string'], adaptive_expressions_1.ReturnType.String, 'Convert a timestamp to Universal Time Coordinated(UTC) from source time zone.'),
    ],
    [
        'startOfDay',
        new FunctionEntity(['timestamp: string', 'format?: string'], adaptive_expressions_1.ReturnType.String, 'Return the start of the day for a timestamp.'),
    ],
    [
        'startOfHour',
        new FunctionEntity(['timestamp: string', 'format?: string'], adaptive_expressions_1.ReturnType.String, 'Return the start of the hour for a timestamp.'),
    ],
    [
        'startOfMonth',
        new FunctionEntity(['timestamp: string', 'format?: string'], adaptive_expressions_1.ReturnType.String, 'Return the start of the month for a timestamp.'),
    ],
    [
        'ticks',
        new FunctionEntity(['timestamp: string'], adaptive_expressions_1.ReturnType.Number, 'Return the ticks property value for a specified timestamp. A tick is 100-nanosecond interval.'),
    ],
    [
        'uriQuery',
        new FunctionEntity(['uri: string'], adaptive_expressions_1.ReturnType.String, 'Return the query value for a unified resource identifier(URI).'),
    ],
    [
        'uriHost',
        new FunctionEntity(['uri: string'], adaptive_expressions_1.ReturnType.String, 'Return the host value for a unified resource identifier(URI).'),
    ],
    [
        'uriPath',
        new FunctionEntity(['uri: string'], adaptive_expressions_1.ReturnType.String, 'Return the path value for a unified resource identifier(URI).'),
    ],
    [
        'uriPathAndQuery',
        new FunctionEntity(['uri: string'], adaptive_expressions_1.ReturnType.String, 'Return the path and query value for a unified resource identifier(URI).'),
    ],
    [
        'uriScheme',
        new FunctionEntity(['uri: string'], adaptive_expressions_1.ReturnType.String, 'Return the scheme value for a unified resource identifier(URI).'),
    ],
    [
        'uriPort',
        new FunctionEntity(['uri: string'], adaptive_expressions_1.ReturnType.String, 'Return the port value for a unified resource identifier(URI).'),
    ],
    [
        'coalesce',
        new FunctionEntity(['...object: any[]'], adaptive_expressions_1.ReturnType.Number, 'Return the first non-null value from one or more parameters. Empty strings, empty arrays, and empty objects are not null.'),
    ],
    /* TODO. Make sure xpath can be used in both browser/node environment
    [
      'xpath',
      new FunctionEntity(
        ['xml: any', 'xpath: any'],
        ReturnType.Object,
        'Check XML for nodes or values that match an XPath (XML Path Language) expression, and return the matching nodes or values. An XPath expression, or just "XPath", helps you navigate an XML document structure so that you can select nodes or compute values in the XML content.'
      ),
    ],
    */
    [
        'select',
        new FunctionEntity(['collection: Array | Object', 'iteratorName: string', 'function: any'], adaptive_expressions_1.ReturnType.Object, 'Operate on each element and return the new collection'),
    ],
    [
        'where',
        new FunctionEntity(['collection: Array | Object', 'iteratorName: string', 'confitionFunction: any'], adaptive_expressions_1.ReturnType.Object, 'Filter on each element and return the new collection of filtered elements which match specific condition'),
    ],
    [
        'sortBy',
        new FunctionEntity(['collection: Array', 'property: string'], adaptive_expressions_1.ReturnType.Object, 'Sort elements in the collection with ascending order and return the sorted collection.'),
    ],
    [
        'sortByDescending',
        new FunctionEntity(['collection: Array', 'property: string'], adaptive_expressions_1.ReturnType.Object, 'Sort elements in the collection with descending order and return the sorted collection.'),
    ],
    [
        'indicesAndValues',
        new FunctionEntity(['collection: Array'], adaptive_expressions_1.ReturnType.Object, 'Turned an array into an array of objects with index (current index) and value property.'),
    ],
    [
        'jPath',
        new FunctionEntity(['json: Object', 'path: string'], adaptive_expressions_1.ReturnType.Object, 'Check JSON or JSON string for nodes or value that match a path expression, and return the matching nodes.'),
    ],
    [
        'setPathToValue',
        new FunctionEntity(['path: any', 'value: Object'], adaptive_expressions_1.ReturnType.Object, 'Retrieve the value of the specified property from the JSON object.'),
    ],
    [
        'isMatch',
        new FunctionEntity(['targetString: string', 'pattern: string'], adaptive_expressions_1.ReturnType.Object, 'test a given string ia match a common regex pattern.'),
    ],
    // Functions injected from LG library
    // https://github.com/microsoft/BotBuilder-Samples/blob/master/experimental/language-generation/docs/Functions-injected-from-LG.md
    [
        'template',
        new FunctionEntity(['templateName: string', '...params: any[]'], adaptive_expressions_1.ReturnType.Object, 'Return the evaluated result of given template name and params.'),
    ],
    [
        'fromFile',
        new FunctionEntity(['filePath: string'], adaptive_expressions_1.ReturnType.String, 'Return the evaluated result of the expression in the given file.'),
    ],
    [
        'isTemplate',
        new FunctionEntity(['templateName: string'], adaptive_expressions_1.ReturnType.Boolean, 'Return whether a given template name is included in the evaluator.'),
    ],
    [
        'ActivityAttachment',
        new FunctionEntity(['content: Object', 'type: string'], adaptive_expressions_1.ReturnType.Boolean, 'Return an activityAttachment constructed from an object and a type.'),
    ],
]);
//# sourceMappingURL=buildinFunctions.js.map
# Timex Resolution

This bot has been created using [Bot Framework](https://dev.botframework.com), is shows how to use TIMEX expressions.

A number of topics are covered within this readme.

* [Concepts introduced in this sample](#Concepts-introduced-in-this-sample)
* [To try this sample](#to-try-this-sample)
* [Testing the bot using Bot Framework Emulator](#Testing-the-bot-using-Bot-Framework-Emulator)
* [Experimenting with Recognizers](#experimenting-with-recognizers)
* [Representing ambiguity](#representing-ambiguity)
* [Representing duration](#representing-duration)
* [Representing ranges](#representing-ranges)
* [Special concepts](#special-concepts)
* [Limitations](#limitations)
* [Resolution](#resolution)
* [TIMEX resolution using the TimexExpressions library](#TIMEX-resolution-using-the-TimexExpressions-library)

## Concepts introduced in this sample

### What is a TIMEX expression?

Natural language has many different ways to express ambiguous dates, ranges of dates and times and durations. However, these concepts cannot be represented with the regular [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) date time representation. This is why TIMEX exists. TIMEX takes ISO 8601 as a starting point and provides additional mechanisms for these concepts. The resulting format normalizes and formalizes these concepts such that they can be processed with regular program logic.

Whenever the ISO 8601 representation is sufficient the TIMEX value is identical. TIMEX is often refered to as an expression because unlike a discrete data time value it can represent a set of values. This will be clear when we look at some examples.

TIMEX expressions can be additionally described with a type. The notion of type is more descriptive than it is constraining. You can, for instance, look at a TIMEX expression and infer its type. This can be useful in application logic. For example, if a bot is expecting a datetimerange but only has a daterange then it can prompt the user according. Again this will be clear when when we look at some examples.

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/microsoft/botbuilder-samples.git
    ```
Unlike many of the other Bot Buildersamples,
the code in this sample is not a bot. It is a console application that demonstrates some significant aspects of the TIMEX helper library. To run this sample:

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `samples/csharp_dotnetcore/40.timex-resolution` folder
  - Select `Timex-Resolution.csproj` file
  - Press `F5` to run the project

## Experimenting with Recognizers

TIMEX has been introduced into the bot authoring platform by the [Text Recognizers](https://github.com/Microsoft/Recognizers-Text) package. One of the best ways to understand the TIMEX behavior is to experiment directly with the Recognizers. You can do this by install the appropriate Recognizer package, instantiating the date time recognizer and calling it with your test string. In Node you can use the following 3 lines of code, but the behavior is identical in C#, Python and Java.

```
  const Recognizer = require('@microsoft/recognizers-text-date-time')
  const result = Recognizer.recognizeDateTime("next Wednesday 4pm", Recognizer.Culture.English);
  console.log(JSON.stringify(result, null, 2));
```

Alternatively you can use [LUIS](https://www.luis.ai/home). When you add a datetime2 datatype to your LUIS model you are asking LUIS to run this exact same recognizer on the input.

## Representing ambiguity

We'll start with an example. The computer scientist Alan Turning was born on June 23rd in 1912. The ISO 8601 representation of this date would be:

```
1912-06-23
```
and this happens to also be the correct TIMEX respresentation. However, with TIMEX we can also represent Alan's birthday:

```
XXXX-06-23
```

Here the XXXX means "any." In other words we can represent the concept of June 23rd without being specific about the year. There are many June 23rds, one every year. This is what we mean here when we say "June 23rd" is ambiguous; we know its June 23rd we just don't know which June 23rd.

TIMEX introduces its own syntax here as an extension to the ISO 8601 format. The XXXX in the previous example, appears in place of the 4 characters that make up the year component. This at first appears to resemble the approach of regular expressions or COBOL pictures but there is less flexibility. In the case of the year component you only ever see XXXX and never a partial pattern such as 19XX.

A time can also be included, for example, 4pm:

```
XXXX-06-23T16
```

This wildcard mechanism is only defined for the date part and not the time part. If you want to represent ambiguity in the time part then you will need multiple TIMEX expressions as we will see later.

In terms of type, all the examples so far have been dates and date-times. (When we say "type" it is just something we have inferred from the underlying TIMEX expression - its does not add any additional semantics, it just might be a classificaton application logic could make use of. For example, if you know you have a date when you wanted a datetime then you know you are missing the time component.) It is possible to also just have the time component by itself, for example 4pm would be represented as:

```
T16
```

The use of X to blank out the characters is also applies to the representation of weeks, for example, "Monday," blanks out both the year and the ISO week number:

```
XXXX-WXX-1
```

Here XXXX means *any* year and WXX means *any* week, so what remains is the day component. And with the ISO week starting on a Monday and indexing from 1 we get Monday. 

The Text.Recognizers are written to attempt to resolve to a specific date if they can. This can result in some surprisingly subtle behavior. For exampe, if we had said "this Monday" it would have resolved to a specific date such as this:

```
2020-06-15
```

Although we onlt had a small change in the text, we ended out with quite a significant change in the resulting TIMEX.

Similarly for weeks, the Text.Recognizers will attempt to resolve when they see a word like "this" that grounds things. For example, the phrase "this week" might result to a TIMEX like this:

```
2020-W25
```

In general care should be taken with logic based around weeks of the year as this can be a source of bugs in application code. How the ISO standard defines this is well documented online. Its important to note the ISO week (and therefore the TIMEX week) starts on a Monday with days of the week being numbered from 1.

Note TIMEX also applies the ISO 8601 syntax of week number to describe the week *within* a month. Representing "the first week of June" as follows:

```
XXXX-06-W01
```

## Representing Duration

TIMEX expressions can represent duration. There are both date durations and time durations. A date duration is indicated by a leading P and a time duration is indicated by a leading PT. Durations have units, formatted as a letter following the value, and for date durations these are D, W, M and Y for day, week, month and year respectively. For time durations these are S, M and H for seconds, minutes and hours. The value is not padded and may contain a decimal point.

Here are some examples:

1 week is:
```
P1W
```
16 years is:
```
P16Y
```
Decimal is supported, so the recognizers would take the string "three and a half months" and return the TIMEX:

```
P3.5M
```

30 second is:
```
PT30S
```
and 4 hours is:
```
PT4H
```

Given the nature of the problem and the fact that decimal is supported, means there are many different ways to express the same duration, for example PT0.5H is the same as PT30M just as P1W is the same as P7D. The Text.Recognizers make no attempt to normalize such things, instead they just try to render the original natural language utterance as directly as possible. The Text.Recognizers also don't try to merge separate durations they find in the input. For example, the text "4 hours 30 minutes" will result in two results.

## Representing Ranges 

### Date Ranges

TIMEX expressions often represent ranges. For example the TIMEX expression:

```
1912
```

is intended to represents all the possible dates in the year 1912. As such it is described as a daterange. The month of June in that year is represented as follows:

```
1912-06
```

This is also a daterange and intended to represent all the dates of June in 1912. The year could be blanked out here too, as follows:

```
XXXX-06
```

Where this means all the dates in June and that's any June in any year not one particular June.

TIMEX breaks with the ISO 8601 pattern when it comes to representing seasons and uses a special two letter codes for Summer, Fall, Winter and Spring: SU, FA, WI and SP respectively. Again these are all date ranges.

More complex data ranges are captured with a begin, end and duration structure. Each of these components in themselves is a TIMEX expression. For example, teh American President JFK was born on May 29th 1917 and died November 22nd 1963, his lifespan can can be captured with the following TIMEX:

```
(1917-05-29,1963-11-22,P16978D)
```

The start and end dates should be read as inclusive.

Natural language contains many different ways to refer to date ranges, for example a Text Recognizer can take the natural language phrase "next two weeks" and assuming today as the start would produce the following TIMEX:

```
(2020-06-19,2020-07-03,P2W)
```

Expressing the duration in addition to the begin and end seems redundant but that is the way it is done. The duration unit used is somewhat arbitrary, in this last example, P14D would have been equivalent.

### Time Ranges

Time ranges are very similar, for example given the text "9am to 5pm" the Text.Recognizers would produce the following:

```
(T09,T17,PT8H)
```

There are a number of well known time range constants in TIMEX. These represent the concepts of morning, afternoon, evening, daytime and nighttime, encoded as two characters MO, AF, EV, DT and NI respectively. Although these are time ranges they don't bother with the (start, end, duration) syntax, they simply start with a T followed by the two character code. For example, morning is respresents like this:

```
TMO
```

### Date Time Range

Date and time can be combined in the same range and the Text.Recognizers would recognize the text "this friday 9am to next friday 5pm" as the TIMEX:

```
(2020-06-19T09,2020-06-26T17,PT176H)
```

When the time component of a date time range can be represented with the shortened 2 characters the resulting expression is simplified appropriately. For example "this afternoon" is a date time range that would resemble this:

```
2020-06-19TAF
```

If there is ambiguity in the date portion that will follow the blanking out approach described earlier. For example, "friday evening" because it doesn't say which Friday is ambiguous and evening is a time range, so we end up with a date time range TIMEX that looks like this:

```
XXXX-WXX-5TEV
```

## Special concepts

TIMEX includes a special representation "now" meaning the present moment. It looks like this:

```
PRESENT_REF
```

## Limitations

TIMEX as it is currently defined and implemented has some inherent limitations. Perhaps the most obvious is that although TIMEX manages to capture ambiguity in the date component it can't in the time component.

Consider the text "Wednesday 4 O'clock," we are not saying which particular Wednesday, it could be *any* Wednesday. As we saw in the earlier examples TIMEX deals with this by blanking out the parts of a date that could vary, specifically for Wednesday we have:

```
XXXX-WXX-3
```

Meaning the 3 day of any week of any year.

However, the language used for the time component is also ambiguous. When we see "4 O'clock" it could mean either 4am or 4pm.

The solution in TIMEX is to provide multiple TIMEX expressions. In fact if you hand this string to the Text.Recognizers it will return two distinct TIMEX expressions in its results:

```
XXXX-WXX-3T04
XXXX-WXX-3T16
```

It is instructive to compare that with the result if teh language had not beed ambiguous. Trying the Text.Recognizer with the text "next Wednesday 4pm" and we will see a result resembling this:

```
2020-06-24T16
```

Note the Text.Recognizers generally look for "last", "this" and "next" and include that in the scope when they can.

This leads us to another conceptual limitation of the current TIMEX implementation and that is the concept of *relative time* is not captured. Specifically, looking at the last example, we said *next* Wednesday and this was recognized *and resolved* to a specific date. This is often what we want, however, sometimes we actually wanted to recognize the concept of *next* in and of itself rather than it being resolved. For example in a booking application we might want to travel out on a particular date and return on the "next Friday." We would want to calculate exactly which Friday the input refered to relative to the particular departure date and almost definitely not the not the current booking date! (Perhaps it would be more correct to have said "following Friday" but applying such strictness to our human users is a challenge. And anyhow, ultimately natural language is always understood by common usage in particular context rather than strict logical structures.)

## Resolution

A TIMEX expression such as:

```
XXXX-WXX-3T16
```

Means Wednesday 4PM, however, something like a travel booking scenario would generally require something less ambiguous, that is to say, we need to know exactly which Wednesday. That is, we need to *resolve* the TIMEX expression to a specific date time.

This can be achieved by constraining the orignal TIMEX expression with a date range, naturally expressed in TIMEX. So for example, applying the TIMEX date range:

```
2020-W27
```

Meaning the 27th week of the year.

Will result in the specific or definite date time:

```
2020-07-01T16
```

This idea generalizes to dealing with collections. Both the original TIMEX could have been a set of TIMEX expressions and the constraint could have been a set of TIMEX ranges.

As we could now have various differing time parts it makes sense to also have time ranges amongst the constraints.

All the TIMEX expressions in the original data should be read as *or* and the TIMEX expressions in the constrains should be read as *and*.

For example give the original set of TIMEX expressions:

```
XXXX-WXX-3T04
XXXX-WXX-3T16
```

Meaning Wednesday at 4AM *or* Wednesday ay 4PM.

And applying the constraints:

```
2020-W27
TAF
```

Meaning the 27th week of the year *and* in the afternoon.

And we get to the specific date:

```
2020-07-01T16
```

Another aspect to resolution is whether the original TIMEX expression is missing some part. For example, we might have just been given a date part to resolve. In this case the resolution is the process of adding the time part. For example, the original TIMEX expression:

```
2020-07-01
```

Can be resolved with the constraint of T14 to get to the specific date time of:

```
2020-07-01T14
```

## TIMEX resolution using the TimexExpressions library

The approach to dealing with TIMEX expressions outlines in above is implemented in the [TimexExpressions](https://github.com/microsoft/Recognizers-Text/tree/master/.NET/Microsoft.Recognizers.Text.DataTypes.TimexExpression) library current available for .NET and JavaScript. And examples of its usage are included in this sample.

This library includes support for parsing TIMEX expressions to extract their component parts. As described above, the datatype can be inferred from the underlying TIMEX expression. This notion of datatype has more in common with tagging. That is, a particular TIMEX instance can often be multiple types. For example, a datetime is also a date and also a time. This allows application code (and the resolution logic itself) to switch appropriately gives the content of the underlying TIMEX.

It also includes support for generating natural language from the underlying TIMEX. Its behavior is to act as the reverse of the Text Recognizer.

This library can also be used to generate locally the examples that LUIS provides in its results. In outline, this is basically forward and backward in time. So "Wednesday" would result in two examples, the date of last Wednesday and the date of this Wednesday. This way of resolution works perfectly well for many simple scenarios. However, its not as flexible nor as complete as the application of ranges described above.

So in summary the library can:

- Parse TIMEX expressions to give you the properties contained there in.
- Generate TIMEX expressions based on setting raw properties.
- Generate natural language from the TIMEX expression. (This is logically the reverse of the Recognizer.)
- Resolve TIMEX expressions to produce example date-times. (This produces the same result as the Recognizer (and therefore LUIS)).
- Evaluate TIMEX expressions against constraints such that new more precise TIMEX expressions are produced.
- It make take several steps, but ultimately you can resolve to a datetime instance, which is probably what your application is looking for.

The code of sample 40 includes examples of all these different features.

### Where is the source code?

The TIMEX expression library is contained in the same GitHub repo as the recognizers. Refer to the further reading section below.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [TIMEX](https://en.wikipedia.org/wiki/TimeML#TIMEX3)
- [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601)
- [Recognizers Text](https://github.com/Microsoft/recognizers-text)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)

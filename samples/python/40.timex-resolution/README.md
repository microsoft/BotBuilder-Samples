# Timex Resolution

This sample shows how to use TIMEX expressions.

## Concepts introduced in this sample

### What is a TIMEX expression?

A TIMEX expression is an alpha-numeric expression derived in outline from the standard date-time representation ISO 8601.
The interesting thing about TIMEX expressions is that they can represent various degrees of ambiguity in the date parts. For example, May 29th, is not a
full calendar date because we haven't said which May 29th - it could be this year, last year, any year in fact.
TIMEX has other features such as the ability to represent ranges, date ranges, time ranges and even date-time ranges.

### Where do TIMEX expressions come from?

TIMEX expressions are produced as part of the output of running a DateTimeRecognizer against some natural language input. As the same
Recognizers are run in LUIS the result returned in the JSON from a call to LUIS also contains the TIMEX expressions.

### What can the library do?

It turns out that TIMEX expressions are not that simple to work with in code. This library attempts to address that. One helpful way to
think about a TIMEX expression is as a partially filled property bag. The properties might be such things as "day of week" or "year."
Basically the more properties we have captured in the expression the less ambiguity we have.

The library can do various things:

- Parse TIMEX expressions to give you the properties contained there in.
- Generate TIMEX expressions based on setting raw properties.
- Generate natural language from the TIMEX expression. (This is logically the reverse of the Recognizer.)
- Resolve TIMEX expressions to produce example date-times. (This produces the same result as the Recognizer (and therefore LUIS)).
- Evaluate TIMEX expressions against constraints such that new more precise TIMEX expressions are produced.

### Where is the source code?

The TIMEX expression library is contained in the same GitHub repo as the recognizers. Refer to the further reading section below.

## Running the sample
- Clone the repository
```bash
git clone https://github.com/Microsoft/botbuilder-python.git
```
- Activate your desired virtual environment
- Bring up a terminal, navigate to `botbuilder-samples\samples\python\40.timex-resolution` folder
- In the terminal, type `pip install -r requirements.txt`
- In the terminal, type `python main.py`

## Further reading

- [TIMEX](https://en.wikipedia.org/wiki/TimeML#TIMEX3)
- [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601)
- [Recognizers Text](https://github.com/Microsoft/recognizers-text)

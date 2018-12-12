// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const Recognizers = require('@microsoft/recognizers-text-date-time');

// TIMEX expressions are designed to represent ambiguous rather than definite dates.
// For example:
// "Monday" could be any Monday ever.
// "May 5th" could be any one of the possible May 5th in the past or the future.
// TIMEX does not represent ambiguous times. So if the natural language mentioned 4 o'clock
// it could be either 4AM or 4PM. For that the recognizer (and by extension LUIS) would return two TIMEX expressions.
// A TIMEX expression can include a date and time parts. So ambiguity of date can be combined with multiple results.
// Code that deals with TIMEX expressions is frequently dealing with sets of TIMEX expressions.

module.exports.dateAmbiguity = () => {
    // Run the recognizer.
    const result = Recognizers.recognizeDateTime('Either Saturday or Sunday would work.', Recognizers.Culture.English);

    // We should find two results in this example.
    result.forEach(result => {
        // The resolution includes two example values: going backwards and forwards from NOW in the calendar.

        // Each result includes a TIMEX expression that captures the inherent date but not time ambiguity.
        // We are interested in the distinct set of TIMEX expressions.

        // There is also either a "value" property on each value or "start" and "end".
        // If you use ToString() on a TimeProperty object you will get same "value".
        const distinctTimexExpressions = new Set(
            result.resolution.values
                .filter(({ timex }) => timex !== undefined)
                .map(({ timex }) => timex)
        );

        // The TIMEX expression captures date ambiguity so there will be a single distinct expression for each result.
        console.log(`${ result.text } ( ${ Array.from(distinctTimexExpressions).join(',') } )`);
    });
};

module.exports.timeAmbiguity = () => {
    // Run the recognizer.
    const result = Recognizers.recognizeDateTime('We would like to arrive at 4 o\'clock or 5 o\'clock.', Recognizers.Culture.English);

    // We should find two results in this example.
    result.forEach(result => {
        // The resolution includes two example values: one for AM and one for PM.

        // Each result includes a TIMEX expression that captures the inherent date but not time ambiguity.
        // We are interested in the distinct set of TIMEX expressions.

        // There is also either a "value" property on each value or "start" and "end".
        // If you use ToString() on a TimeProperty object you will get same "value".
        const distinctTimexExpressions = new Set(
            result.resolution.values
                .filter(({ timex }) => timex !== undefined)
                .map(({ timex }) => timex)
        );

        // The TIMEX expression captures date ambiguity so there will be a single distinct expression for each result.
        console.log(`${ result.text } ( ${ Array.from(distinctTimexExpressions).join(',') } )`);
    });
};

module.exports.dateTimeAmbiguity = () => {
    // Run the recognizer.
    const result = Recognizers.recognizeDateTime("It will be ready Wednesday at 5 o'clock.", Recognizers.Culture.English);

    // We should find two results in this example.
    result.forEach((result) => {
        // The resolution includes four example values: backwards and forward in the calendar and then AM and PM.

        // Each result includes a TIMEX expression that captures the inherent date but not time ambiguity.
        // We are interested in the distinct set of TIMEX expressions.
        const distinctTimexExpressions = new Set(
            result.resolution.values
                .filter(({ timex }) => timex !== undefined)
                .map(({ timex }) => timex)
        );

        // TIMEX expressions don't capture time ambiguity so there will be two distinct expressions for each result.
        console.log(`${ result.text } ( ${ Array.from(distinctTimexExpressions).join(',') } )`);
    });
};

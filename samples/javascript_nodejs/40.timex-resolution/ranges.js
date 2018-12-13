// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const Recognizers = require('@microsoft/recognizers-text-date-time');

module.exports.dateRange = () => {
    // Run the recognizer.
    const result = Recognizers.recognizeDateTime('Some time in the next two weeks.', Recognizers.Culture.English);

    // We should find a single result in this example.
    result.forEach(result => {
        // The resolution includes a single value because there is no ambiguity.

        // We are interested in the distinct set of TIMEX expressions.
        const distinctTimexExpressions = new Set(
            result.resolution.values
                .filter(({ timex }) => timex !== undefined)
                .map(({ timex }) => timex)
        );

        // The TIMEX expression captures date ambiguity so there will be a single distinct expression for each result.
        console.log(`${ result.text } ( ${ Array.from(distinctTimexExpressions).join(',') } )`);
    });
};

module.exports.timeRange = () => {
    // Run the recognizer.
    const result = Recognizers.recognizeDateTime('Some time between 6pm and 6:30pm.', Recognizers.Culture.English);

    // We should find a single result in this example.
    result.forEach(result => {
        // The resolution includes a single value because there is no ambiguity.

        // We are interested in the distinct set of TIMEX expressions.
        const distinctTimexExpressions = new Set(
            result.resolution.values
                .filter(({ timex }) => timex !== undefined)
                .map(({ timex }) => timex)
        );

        // The TIMEX expression captures date ambiguity so there will be a single distinct expression for each result.
        console.log(`${ result.text } ( ${ Array.from(distinctTimexExpressions).join(',') } )`);
    });
};

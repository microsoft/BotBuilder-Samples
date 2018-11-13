// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const Recognizers = require('@microsoft/recognizers-text-date-time');

module.exports.dateRange = function () {

    // Run the recognizer.
    var result = Recognizers.recognizeDateTime('Some time in the next two weeks.', Recognizers.Culture.English);

    // We should find a single result in this example.
    result.forEach ((result) => {

        // The resolution includes a single value because there is no ambiguity.
        var distinctTimexExpressions = new Set();
        var values = result.resolution["values"];
        values.forEach ((value) => {

            // We are interested in the distinct set of TIMEX expressions.
            var timex = value["timex"];
            if (timex != undefined)
            {
                distinctTimexExpressions.add(timex);
            }
        });

        // The TIMEX expression captures date ambiguity so there will be a single distinct expression for each result.
        console.log(`${result.text} ( ${(Array.from(distinctTimexExpressions).join(','))} )`);
    });
}

module.exports.timeRange = function () {

    // Run the recognizer.
    var result = Recognizers.recognizeDateTime('Some time between 6pm and 6:30pm.', Recognizers.Culture.English);

    // We should find a single result in this example.
    result.forEach ((result) => {

        // The resolution includes a single value because there is no ambiguity.
        var distinctTimexExpressions = new Set();
        var values = result.resolution["values"];
        values.forEach ((value) => {

            // We are interested in the distinct set of TIMEX expressions.
            var timex = value["timex"];
            if (timex != undefined)
            {
                distinctTimexExpressions.add(timex);
            }
        });

        // The TIMEX expression captures date ambiguity so there will be a single distinct expression for each result.
        console.log(`${result.text} ( ${(Array.from(distinctTimexExpressions).join(','))} )`);
    });
}

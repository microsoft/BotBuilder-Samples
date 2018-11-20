// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { TimexProperty, valueResolver } = require('@microsoft/recognizers-text-data-types-timex-expression');

// Given the TIMEX expressions it is easy to create the computed example values that the recognizer gives.

module.exports.examples = function () {

    if (valueResolver === undefined) {
        // see issue: https://github.com/Microsoft/Recognizers-Text/issues/974
        console.error('ERR: package does not include valueResolver');
        return;
    }

    // When you give the recognzier the text "Wednesday 4 o'clock" you get these distinct TIMEX values back.
    
    var today = new Date();
    var resolution = valueResolver.resolve([ "XXXX-WXX-3T04", "XXXX-WXX-3T16" ], today);

    Console.WriteLine(resolution.values.length);

    Console.WriteLine(resolution.values[0].timex);
    Console.WriteLine(resolution.values[0].type);
    Console.WriteLine(resolution.values[0].value);

    Console.WriteLine(resolution.values[1].timex);
    Console.WriteLine(resolution.values[1].type);
    Console.WriteLine(resolution.values[1].value);

    Console.WriteLine(resolution.values[2].timex);
    Console.WriteLine(resolution.values[2].type);
    Console.WriteLine(resolution.values[2].value);

    Console.WriteLine(resolution.values[3].timex);
    Console.WriteLine(resolution.values[3].type);
    Console.WriteLine(resolution.values[3].value);
}

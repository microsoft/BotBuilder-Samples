// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { valueResolver } = require('@microsoft/recognizers-text-data-types-timex-expression');

// Given the TIMEX expressions it is easy to create the computed example values that the recognizer gives.

module.exports.examples = () => {
    if (valueResolver === undefined) {
        // see issue: https://github.com/Microsoft/Recognizers-Text/issues/974
        console.error('ERR: package does not include valueResolver');
        return;
    }

    // When you give the recognzier the text "Wednesday 4 o'clock" you get these distinct TIMEX values back.
    const today = new Date();
    const resolution = valueResolver.resolve(['XXXX-WXX-3T04', 'XXXX-WXX-3T16'], today);

    console.log(resolution.values.length);

    resolution.values.forEach(({ timex, type, value }) => {
        console.log(timex);
        console.log(type);
        console.log(value);
    });
};

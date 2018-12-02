// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { TimexProperty } = require('@microsoft/recognizers-text-data-types-timex-expression');

// The TimexProperty class takes a TIMEX expression as a string argument in its constructor.
// This pulls all the component parts of the expression into properties on this object. You can
// then manipulate the TIMEX expression via those properties.
// The "Types" property infers a datetimeV2 type from the underlying set of properties.
// If you take a TIMEX with date components and add time components you add the
// inferred type datetime (its still a date).
// Logic can be written against the inferred type, perhaps to have the bot ask the user for disamiguation.

const describe = (t) => {
    var msg = `${ t.timex } `;

    if (t.types.has('date')) {
        if (t.types.has('definite')) {
            msg = msg.concat('We have a definite calendar date. ');
        } else {
            msg = msg.concat('We have a date but there is some ambiguity. ');
        }
    }

    if (t.types.has('time')) {
        msg = msg.concat('We have a time.');
    }

    console.log(msg);
};

module.exports.examples = () => {
    describe(new TimexProperty('2017-05-29'));
    describe(new TimexProperty('XXXX-WXX-6'));
    describe(new TimexProperty('XXXX-WXX-6T16'));
    describe(new TimexProperty('T12'));
};

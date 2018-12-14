// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { TimexProperty } = require('@microsoft/recognizers-text-data-types-timex-expression');

// This langauge generation capabilitis are the logical opposite of what the recognizer does.
// As an experiment try feeding the result of lanaguage generation back into a recognizer.
// You should get back the same TIMEX expression in the result.

const describe = t => {
    // Note natural language is often relative, for example the senstence "Yesterday all my troubles seemed so far away."
    // Having your bot say something like "next Wednesday" in a response can make it sound more natural.
    const referenceDate = new Date();

    console.log(`${ t.timex } ${ t.toNaturalLanguage(referenceDate) }`);
};

module.exports.examples = () => {
    describe(new TimexProperty('2017-05-29'));
    describe(new TimexProperty('XXXX-WXX-6'));
    describe(new TimexProperty('XXXX-WXX-6T16'));
    describe(new TimexProperty('T12'));

    const today = new Date();

    describe(TimexProperty.fromDate(today));

    today.setDate(today.getDate() + 1);

    describe(TimexProperty.fromDate(today));
};

/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
const now = new Date();
const tomorrow = formatDate(new Date().setDate(now.getDate() + 1));
const dayAfterTomorrow = formatDate(new Date().setDate(now.getDate() + 2));

function formatDate(date) {
    const d = new Date(date);
    let month = '' + (d.getMonth() + 1);
    let day = '' + d.getDate();
    const year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [year, month, day].join('-');
}

module.exports = [
    {
        expectedResult: tomorrow,
        initialData: null,
        name: 'tomorrow',
        steps: [
            ['hi', 'On what date would you like to travel?'],
            ['tomorrow', null]
        ]
    },
    {
        expectedResult: dayAfterTomorrow,
        initialData: null,
        name: 'the day after tomorrow',
        steps: [
            ['hi', 'On what date would you like to travel?'],
            ['the day after tomorrow', null]
        ]
    },
    {
        expectedResult: dayAfterTomorrow,
        initialData: null,
        name: 'two days from now',
        steps: [
            ['hi', 'On what date would you like to travel?'],
            ['two days from now', null]
        ]
    },
    {
        expectedResult: tomorrow,
        initialData: { date: tomorrow },
        name: 'valid input given (tomorrow)',
        steps: [
            ['hi', null]
        ]
    },
    {
        expectedResult: tomorrow,
        initialData: {},
        name: 'retry prompt',
        steps: [
            ['hi', 'On what date would you like to travel?'],
            ['bananas', 'I\'m sorry, for best results, please enter your travel date including the month, day and year.'],
            ['tomorrow', null]
        ]
    },
    {
        expectedResult: '2055-05-05',
        initialData: {},
        name: 'fuzzy time',
        steps: [
            ['hi', 'On what date would you like to travel?'],
            ['may 5th', 'I\'m sorry, for best results, please enter your travel date including the month, day and year.'],
            ['may 5th 2055', null]
        ]
    }
];

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
        name: 'tomorrow',
        initialData: null,
        steps: [
            ['hi', 'On what date would you like to travel?'],
            ['tomorrow', null]
        ],
        expectedResult: tomorrow
    },
    {
        name: 'the day after tomorrow',
        initialData: null,
        steps: [
            ['hi', 'On what date would you like to travel?'],
            ['the day after tomorrow', null]
        ],
        expectedResult: dayAfterTomorrow
    },
    {
        name: 'two days from now',
        initialData: null,
        steps: [
            ['hi', 'On what date would you like to travel?'],
            ['two days from now', null]
        ],
        expectedResult: dayAfterTomorrow
    },
    {
        name: 'valid input given (tomorrow)',
        initialData: { date: tomorrow },
        steps: [
            ['hi', null]
        ],
        expectedResult: tomorrow
    },
    {
        name: 'retry prompt',
        initialData: {},
        steps: [
            ['hi', 'On what date would you like to travel?'],
            ['bananas', 'I\'m sorry, for best results, please enter your travel date including the month, day and year.'],
            ['tomorrow', null]
        ],
        expectedResult: tomorrow
    },
    {
        name: 'fuzzy time',
        initialData: {},
        steps: [
            ['hi', 'On what date would you like to travel?'],
            ['may 5th', 'I\'m sorry, for best results, please enter your travel date including the month, day and year.'],
            ['may 5th 2055', null]
        ],
        expectedResult: '2055-05-05'
    }
];

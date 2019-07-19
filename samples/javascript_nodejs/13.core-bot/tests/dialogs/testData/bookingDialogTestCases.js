/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
const now = new Date();
const today = formatDate(new Date());
const tomorrow = formatDate(new Date().setDate(now.getDate() + 1));

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
        name: 'Full flow',
        initialData: {},
        steps: [
            ['hi', 'To what city would you like to travel?'],
            ['Seattle', 'From what city will you be travelling?'],
            ['New York', 'On what date would you like to travel?'],
            ['tomorrow', `Please confirm, I have you traveling to: Seattle from: New York on: ${ tomorrow }. Is this correct? (1) Yes or (2) No`],
            ['yes', null]
        ],
        expectedStatus: 'complete',
        expectedResult: {
            destination: 'Seattle',
            origin: 'New York',
            travelDate: tomorrow
        }
    },
    {
        name: 'Full flow with \'no\' at confirmation',
        initialData: {},
        steps: [
            ['hi', 'To what city would you like to travel?'],
            ['Seattle', 'From what city will you be travelling?'],
            ['New York', 'On what date would you like to travel?'],
            ['tomorrow', `Please confirm, I have you traveling to: Seattle from: New York on: ${ tomorrow }. Is this correct? (1) Yes or (2) No`],
            ['no', null]
        ],
        expectedStatus: 'complete',
        expectedResult: undefined
    },
    {
        name: 'Destination given',
        initialData: {
            destination: 'Bahamas'
        },
        steps: [
            ['hi', 'From what city will you be travelling?'],
            ['New York', 'On what date would you like to travel?'],
            ['tomorrow', `Please confirm, I have you traveling to: Bahamas from: New York on: ${ tomorrow }. Is this correct? (1) Yes or (2) No`],
            ['yes', null]
        ],
        expectedStatus: 'complete',
        expectedResult: {
            origin: 'New York',
            destination: 'Bahamas',
            travelDate: tomorrow
        }
    },
    {
        name: 'Destination and origin given',
        initialData: {
            destination: 'Seattle',
            origin: 'New York'
        },
        steps: [
            ['hi', 'On what date would you like to travel?'],
            ['tomorrow', `Please confirm, I have you traveling to: Seattle from: New York on: ${ tomorrow }. Is this correct? (1) Yes or (2) No`],
            ['yes', null]
        ],
        expectedStatus: 'complete',
        expectedResult: {
            destination: 'Seattle',
            origin: 'New York',
            travelDate: tomorrow
        }
    },
    {
        name: 'All booking details given for today',
        initialData: {
            destination: 'Seattle',
            origin: 'Bahamas',
            travelDate: today
        },
        steps: [
            ['hi', `Please confirm, I have you traveling to: Seattle from: Bahamas on: ${ today }. Is this correct? (1) Yes or (2) No`],
            ['yes', null]
        ],
        expectedStatus: 'complete',
        expectedResult: {
            destination: 'Seattle',
            origin: 'Bahamas',
            travelDate: today
        }
    },
    {
        name: 'Cancel on origin prompt',
        initialData: {},
        steps: [
            ['hi', 'To what city would you like to travel?'],
            ['cancel', 'Cancelling...']
        ],
        expectedStatus: 'complete',
        expectedResult: undefined
    },
    {
        name: 'Cancel on destination prompt',
        initialData: {},
        steps: [
            ['hi', 'To what city would you like to travel?'],
            ['Seattle', 'From what city will you be travelling?'],
            ['cancel', 'Cancelling...']
        ],
        expectedStatus: 'complete',
        expectedResult: undefined
    },
    {
        name: 'Cancel on date prompt',
        initialData: {},
        steps: [
            ['hi', 'To what city would you like to travel?'],
            ['Seattle', 'From what city will you be travelling?'],
            ['New York', 'On what date would you like to travel?'],
            ['cancel', 'Cancelling...']
        ],
        expectedStatus: 'complete',
        expectedResult: undefined
    },
    {
        name: 'Cancel on confirm prompt',
        initialData: {},
        steps: [
            ['hi', 'To what city would you like to travel?'],
            ['Seattle', 'From what city will you be travelling?'],
            ['New York', 'On what date would you like to travel?'],
            ['tomorrow', `Please confirm, I have you traveling to: Seattle from: New York on: ${ tomorrow }. Is this correct? (1) Yes or (2) No`],
            ['cancel', 'Cancelling...']
        ],
        expectedStatus: 'complete',
        expectedResult: undefined
    }
];

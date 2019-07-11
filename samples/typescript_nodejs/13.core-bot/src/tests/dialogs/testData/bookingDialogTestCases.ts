/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
let bookingDialogNow = new Date();
let bookingDialogToday = formatBookingDialogDate(new Date());
let bookingDialogTomorrow = formatBookingDialogDate(new Date().setDate(bookingDialogNow.getDate() + 1));

function formatBookingDialogDate(date) {
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
        expectedResult: {
            destination: 'Seattle',
            origin: 'New York',
            travelDate: bookingDialogTomorrow
        },
        expectedStatus: 'complete',
        initialData: {},
        name: 'Full flow',
        steps: [
            ['hi', 'To what city would you like to travel?'],
            ['Seattle', 'From what city will you be travelling?'],
            ['New York', 'On what date would you like to travel?'],
            ['tomorrow', `Please confirm, I have you traveling to: Seattle from: New York on: ${ bookingDialogTomorrow }. Is this correct? (1) Yes or (2) No`],
            ['yes', null]
        ]
    },
    {
        expectedResult: undefined,
        expectedStatus: 'complete',
        initialData: {},
        name: 'Full flow with \'no\' at confirmation',
        steps: [
            ['hi', 'To what city would you like to travel?'],
            ['Seattle', 'From what city will you be travelling?'],
            ['New York', 'On what date would you like to travel?'],
            ['tomorrow', `Please confirm, I have you traveling to: Seattle from: New York on: ${ bookingDialogTomorrow }. Is this correct? (1) Yes or (2) No`],
            ['no', null]
        ]
    },
    {
        expectedResult: {
            destination: 'Bahamas',
            origin: 'New York',
            travelDate: bookingDialogTomorrow
        },
        expectedStatus: 'complete',
        initialData: {
            destination: 'Bahamas'
        },
        name: 'Destination given',
        steps: [
            ['hi', 'From what city will you be travelling?'],
            ['New York', 'On what date would you like to travel?'],
            ['tomorrow', `Please confirm, I have you traveling to: Bahamas from: New York on: ${ bookingDialogTomorrow }. Is this correct? (1) Yes or (2) No`],
            ['yes', null]
        ]
    },
    {
        expectedResult: {
            destination: 'Seattle',
            origin: 'New York',
            travelDate: bookingDialogTomorrow
        },
        expectedStatus: 'complete',
        initialData: {
            destination: 'Seattle',
            origin: 'New York'
        },
        name: 'Destination and origin given',
        steps: [
            ['hi', 'On what date would you like to travel?'],
            ['tomorrow', `Please confirm, I have you traveling to: Seattle from: New York on: ${ bookingDialogTomorrow }. Is this correct? (1) Yes or (2) No`],
            ['yes', null]
        ]
    },
    {
        expectedResult: {
            destination: 'Seattle',
            origin: 'Bahamas',
            travelDate:  bookingDialogToday
        },
        expectedStatus: 'complete',
        initialData: {
            destination: 'Seattle',
            origin: 'Bahamas',
            travelDate:  bookingDialogToday
        },
        name: 'All booking details given for today',
        steps: [
            ['hi', `Please confirm, I have you traveling to: Seattle from: Bahamas on: ${  bookingDialogToday }. Is this correct? (1) Yes or (2) No`],
            ['yes', null]
        ]
    },
    {
        expectedResult: undefined,
        expectedStatus: 'cancelled',
        initialData: {},
        name: 'Cancel on origin prompt',
        steps: [
            ['hi', 'To what city would you like to travel?'],
            ['cancel', 'Cancelling...']
        ]
    },
    {
        expectedResult: undefined,
        expectedStatus: 'cancelled',
        initialData: {},
        name: 'Cancel on destination prompt',
        steps: [
            ['hi', 'To what city would you like to travel?'],
            ['Seattle', 'From what city will you be travelling?'],
            ['cancel', 'Cancelling...']
        ]
    },
    {
        expectedResult: undefined,
        expectedStatus: 'cancelled',
        initialData: {},
        name: 'Cancel on date prompt',
        steps: [
            ['hi', 'To what city would you like to travel?'],
            ['Seattle', 'From what city will you be travelling?'],
            ['New York', 'On what date would you like to travel?'],
            ['cancel', 'Cancelling...']
        ]
    },
    {
        expectedResult: undefined,
        expectedStatus: 'cancelled',
        initialData: {},
        name: 'Cancel on confirm prompt',
        steps: [
            ['hi', 'To what city would you like to travel?'],
            ['Seattle', 'From what city will you be travelling?'],
            ['New York', 'On what date would you like to travel?'],
            ['tomorrow', `Please confirm, I have you traveling to: Seattle from: New York on: ${ bookingDialogTomorrow }. Is this correct? (1) Yes or (2) No`],
            ['cancel', 'Cancelling...']
        ]
    }
];

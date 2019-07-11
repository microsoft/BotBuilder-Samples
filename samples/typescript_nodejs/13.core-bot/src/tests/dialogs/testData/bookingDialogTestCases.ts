/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */
let bookingDialogNow = new Date();
let bookingDialogToday = formatBookingDialogDate(new Date());
let bookingDialogTomorrow = formatBookingDialogDate(new Date().setDate(bookingDialogNow.getDate() + 1));

function formatBookingDialogDate(date) {
    let d = new Date(date);
    let month = '' + (d.getMonth() + 1);
    let day = '' + d.getDate();
    let year = d.getFullYear();

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
            ['tomorrow', `Please confirm, I have you traveling to: Seattle from: New York on: ${ bookingDialogTomorrow }. Is this correct? (1) Yes or (2) No`],
            ['yes', null]
        ],
        expectedStatus: 'complete',
        expectedResult: {
            destination: 'Seattle',
            origin: 'New York',
            travelDate: bookingDialogTomorrow
        }
    },
    {
        name: 'Full flow with \'no\' at confirmation',
        initialData: {},
        steps: [
            ['hi', 'To what city would you like to travel?'],
            ['Seattle', 'From what city will you be travelling?'],
            ['New York', 'On what date would you like to travel?'],
            ['tomorrow', `Please confirm, I have you traveling to: Seattle from: New York on: ${ bookingDialogTomorrow }. Is this correct? (1) Yes or (2) No`],
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
            ['tomorrow', `Please confirm, I have you traveling to: Bahamas from: New York on: ${ bookingDialogTomorrow }. Is this correct? (1) Yes or (2) No`],
            ['yes', null]
        ],
        expectedStatus: 'complete',
        expectedResult: {
            origin: 'New York',
            destination: 'Bahamas',
            travelDate: bookingDialogTomorrow
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
            ['tomorrow', `Please confirm, I have you traveling to: Seattle from: New York on: ${ bookingDialogTomorrow }. Is this correct? (1) Yes or (2) No`],
            ['yes', null]
        ],
        expectedStatus: 'complete',
        expectedResult: {
            destination: 'Seattle',
            origin: 'New York',
            travelDate: bookingDialogTomorrow
        }
    },
    {
        name: 'All booking details given for today',
        initialData: {
            destination: 'Seattle',
            origin: 'Bahamas',
            travelDate:  bookingDialogToday
        },
        steps: [
            ['hi', `Please confirm, I have you traveling to: Seattle from: Bahamas on: ${  bookingDialogToday }. Is this correct? (1) Yes or (2) No`],
            ['yes', null]
        ],
        expectedStatus: 'complete',
        expectedResult: {
            destination: 'Seattle',
            origin: 'Bahamas',
            travelDate:  bookingDialogToday
        }
    },
    {
        name: 'Cancel on origin prompt',
        initialData: {},
        steps: [
            ['hi', 'To what city would you like to travel?'],
            ['cancel', 'Cancelling...']
        ],
        expectedStatus: 'cancelled',
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
        expectedStatus: 'cancelled',
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
        expectedStatus: 'cancelled',
        expectedResult: undefined
    },
    {
        name: 'Cancel on confirm prompt',
        initialData: {},
        steps: [
            ['hi', 'To what city would you like to travel?'],
            ['Seattle', 'From what city will you be travelling?'],
            ['New York', 'On what date would you like to travel?'],
            ['tomorrow', `Please confirm, I have you traveling to: Seattle from: New York on: ${ bookingDialogTomorrow }. Is this correct? (1) Yes or (2) No`],
            ['cancel', 'Cancelling...']
        ],
        expectedStatus: 'cancelled',
        expectedResult: undefined
    }
];

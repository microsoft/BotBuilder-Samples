// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const LUIS_ENTITIES = require('../luisEntities');

// Using text recognizers package to perform timex operations.
var { TimexProperty, creator, resolver }  = require('@microsoft/recognizers-text-data-types-timex-expression').default;
const FOUR_WEEKS = '4';

// Date constraints for reservations
const reservationDateConstraints = [        /* Date for reservations must be        */
    creator.thisWeek,                       /* - a date in this week .OR.           */
    creator.nextWeeksFromToday(FOUR_WEEKS), /* - a date in the next 4 weeks .AND.   */
];
// Time constraints for reservations
const reservationTimeConstraints = [    /* Time for reservations must be   */
    creator.daytime                     /* - daytime or                    */
];

class ReservationProperty {
    /**
     * Reservation Property constructor.
     * 
     * @param {String} id reservation id
     * @param {String} date reservation date
     * @param {String} time reservation time
     * @param {Number} partySize number of guests in reservation
     * @param {String} location reservation location
     */
    constructor(id, date, time, partySize, location, metaData) {
        this.id = id ? id : get_guid();
        this.date = date ? date : '';
        this.time = time ? time : '';
        this.dateLGString = '';
        this.timeLGString = '';
        this.dateTimeLGString = '';
        this.partySize = partySize ? partySize : 0;
        this.location = location ? location : '';
        this.metaData = metaData ? metaData : {};
    }
    /**
     * Helper method to update Reservation property with information passed in via the onTurnProperty object
     * @param {Object} onTurnProperty 
     */
    updateProperties(onTurnProperty) {
        // TODO: implement.
    }
};
/**
 * Static method to create a new instance of Reservation property based on onTurnProperty object
 * @param {Object} onTurnProperty 
 * @returns {ReservationProperty}
 */
ReservationProperty.fromOnTurnProperty = function(onTurnProperty) {
    let reservation = new ReservationProperty();

    if(onTurnProperty === undefined || onTurnProperty.entities.length === 0) return reservation;
    
    // We only will pull number -> party size, datetimeV2 -> date and time, cafeLocation -> location. 
    let numberEntity = onTurnProperty.entities.find(item => item.entityName == LUIS_ENTITIES[1]);
    let dateTimeEntity = onTurnProperty.entities.find(item => item.entityName == LUIS_ENTITIES[2]);
    let locationEntity = onTurnProperty.entities.find(item => item.entityName == LUIS_ENTITIES[3]);

    if(numberEntity !== undefined) reservation.partySize = numberEntity.entityValue[0];
    if(dateTimeEntity !== undefined) {
        // Get parsed date time from TIMEX
        // LUIS returns a timex expression and so get and un-wrap that.
        // Take the first date time since book table scenario does not have to deal with multiple date times or date time ranges.
        if(dateTimeEntity.entityValue[0].timex && dateTimeEntity.entityValue[0].timex[0]) {
            let today = new Date();
            let parsedTimex = new TimexProperty(dateTimeEntity.entityValue[0].timex[0]);
            // see if the date meets our constraints
            if(parsedTimex.dayOfMonth !== undefined && parsedTimex.year  !== undefined && parsedTimex.month  !== undefined) {
                const validDate = resolver.evaluate(dateTimeEntity.entityValue[0].timex, reservationDateConstraints);
                if(validDate && validDate.length !== 0) {
                    // We have a valid date. Accept it.
                    // TODO: Make this into a fully formed date YYYY-MM-DD
                    reservation.date = `${parsedTimex.year}-${parsedTimex.month}-${parsedTimex.dayOfMonth}`;
                }
            }
            // see if the time meets our constraints
            if(parsedTimex.hour !== undefined && parsedTimex.minute  !== undefined && parsedTimex.second  !== undefined) {
                const validtime = resolver.evaluate(dateTimeEntity.entityValue[0].timex, reservationTimeConstraints);
                if(validtime && validtime.length !== 0) {
                    // We have a valid time. Accept it.
                    // TODO: Make this into a fully formed time HH:MM:SS
                    reservation.time = `${validtime[0].hour}:${validtime[0].minute}:${validtime[0].second}`;
                }
            }
            // Get date time LG string if we have both date and time            
            if(reservation.date !== '' && reservation.time !== '') {
                reservation.dateTimeLGString = parsedTimex.toNaturalLanguage(today);
            } else if(reservation.date !== '') {
                reservation.dateLGString = new TimexProperty(reservation.date).toNaturalLanguage(today);
            } else if(reservation.time !== '') {
                reservation.timeLGString = new TimexProperty('xxxx-xx-xxT' + reservation.time).toNaturalLanguage(today);
            }
        }        
    }
    // Take the first found value.
    if(locationEntity !== undefined) reservation.location = locationEntity.entityValue[0][0];
    return reservation;
};

/**
 * Helper function to create a random guid
  * @returns {string} GUID
 */
const get_guid = function () {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

module.exports = ReservationProperty;
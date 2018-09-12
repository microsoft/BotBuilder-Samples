// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { ReservationOutcome, ReservationResult, reservationStatus } = require('./createReservationPropertyResult');
// Using text recognizers package to perform timex operations.
var { TimexProperty, creator, resolver }  = require('@microsoft/recognizers-text-data-types-timex-expression').default;
const { LUIS_ENTITIES } = require('../helpers');

const PARTY_SIZE_ENTITY = LUIS_ENTITIES[1];
const DATE_TIME_ENTITY = LUIS_ENTITIES[2];
const LOCATION_ENTITY = LUIS_ENTITIES[3];
const FOUR_WEEKS = '4';
const MAX_PARTY_SIZE = 10;

// Date constraints for reservations
const reservationDateConstraints = [        /* Date for reservations must be        */
    creator.thisWeek,                       /* - a date in this week .OR.           */
    creator.nextWeeksFromToday(FOUR_WEEKS), /* - a date in the next 4 weeks .AND.   */
];

// Time constraints for reservations
const reservationTimeConstraints = [        /* Time for reservations must be   */
    creator.daytime                         /* - daytime or                    */
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
     * Helper method to evalute if we have all required properties filled.
     * @returns {Boolean} true if we have a complete reservation property
     */
    haveCompleteReservationProperty() {
        return ((this.id !== undefined) && 
                (this.date !== '') &&
                (this.time !== '') &&
                (this.partySize !== 0) && 
                (this.location !== ''));
    }
    /**
     * Helper method to update Reservation property with information passed in via the onTurnProperty object
     * @param {Object} onTurnProperty 
     * @returns {ReservationResult} return result object 
     */
    updateProperties(onTurnProperty) {
        let returnResult = new ReservationResult(this);
        return validate(onTurnProperty, returnResult);
    }
    /**
     * Helper method for Language Generation read out based on current reservation property object
     * @returns {String}
     */
    getMissingPropertyReadOut() {
        if(this.location === '') {
            return `What city? We offer services in Seattle, Bellevue, Redmond and Renton.`;
        } else if (this.date === '') {
            return `When do you want to come in? You can say things like tomorrow, next thursday ...`;
        } else if (this.time === '') {
            return `What time?`;
        } else if (this.partySize === '') {
            return `How many guests?`
        } else return '';
    }
};
/**
 * Static method to create a new instance of Reservation property based on onTurnProperty object
 * @param {Object} onTurnProperty 
 * @returns {ReservationResult} object 
 */
ReservationProperty.fromOnTurnProperty = function(onTurnProperty) {
    let returnResult = new ReservationResult(new ReservationProperty());
    return validate(onTurnProperty, returnResult);
};
/**
 * Static method to create a new instance of Reservation property based on a JSON object
 * @param {Object} obj 
 * @returns {ReservationProperty} object
 */
ReservationProperty.fromJSON = function(obj) {
    if(obj === undefined) return new ReservationProperty();
    const { id, date, time, partySize, location } = obj;
    return new ReservationProperty(id, date, time, partySize, location);
}

module.exports = ReservationProperty;
/**
 * HELPERS
 */
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
/**
 * Helper function to validate input and return results based on validation constraints
 * 
 * @param {Object} onTurnProperty 
 * @param {ReservationResult} return result object 
 */
const validate = function (onTurnProperty, returnResult) {
    if(onTurnProperty === undefined || onTurnProperty.entities.length === 0) return returnResult;
    
    // We only will pull number -> party size, datetimeV2 -> date and time, cafeLocation -> location. 
    let numberEntity = onTurnProperty.entities.find(item => item.entityName == PARTY_SIZE_ENTITY);
    let dateTimeEntity = onTurnProperty.entities.find(item => item.entityName == DATE_TIME_ENTITY);
    let locationEntity = onTurnProperty.entities.find(item => item.entityName == LOCATION_ENTITY);

    if(numberEntity !== undefined) {
        // We only accept MAX_PARTY_SIZE in a reservation.
        if(parseInt(numberEntity.entityValue[0]) > MAX_PARTY_SIZE) {
            returnResult.outcome.push(new ReservationOutcome(`Sorry. ${numberEntity.entityName[0]} does not work. I can only accept up to 10 guests in a reservation.`, PARTY_SIZE_ENTITY));
            returnResult.status = reservationStatus.INCOMPLETE;
        } else {
            returnResult.newReservation.partySize = numberEntity.entityValue[0];
        }
    }
    if(dateTimeEntity !== undefined) {
        // Get parsed date time from TIMEX
        // LUIS returns a timex expression and so get and un-wrap that.
        // Take the first date time since book table scenario does not have to deal with multiple date times or date time ranges.
        if(dateTimeEntity.entityValue[0].timex && dateTimeEntity.entityValue[0].timex[0]) {
            let today = new Date();
            let parsedTimex = new TimexProperty(dateTimeEntity.entityValue[0].timex[0]);
            // see if the date meets our constraints
            if(parsedTimex.dayOfMonth !== undefined && parsedTimex.year  !== undefined && parsedTimex.month  !== undefined) {
                let lDate = new Date(`${parsedTimex.year}-${parsedTimex.month}-${parsedTimex.dayOfMonth}`);
                returnResult.newReservation.date = new Date(lDate.getTime() - (lDate.getTimezoneOffset() * 60000 ))
                                                        .toISOString()
                                                        .split("T")[0];
                returnResult.newReservation.dateLGString = new TimexProperty(returnResult.newReservation.date).toNaturalLanguage(today);
                const validDate = resolver.evaluate(dateTimeEntity.entityValue[0].timex, reservationDateConstraints);
                if(!validDate || (validDate.length === 0)) {
                    // Validation failed!
                    returnResult.outcome.push(new ReservationOutcome(`Sorry. ${returnResult.newReservation.dateLGString} does not work. I can only make reservations for the next 4 weeks.`, DATE_TIME_ENTITY));
                    returnResult.newReservation.date = '';
                    returnResult.status = reservationStatus.INCOMPLETE;
                }
            }
            // see if the time meets our constraints
            if(parsedTimex.hour !== undefined && parsedTimex.minute  !== undefined && parsedTimex.second  !== undefined) {
                const validtime = resolver.evaluate(dateTimeEntity.entityValue[0].timex, reservationTimeConstraints);
                if(!validtime || (validtime.length === 0)) {
                    // Validation failed!
                    returnResult.outcome.push(new ReservationOutcome(`Sorry, that time does not work. I can only make reservations that are in the daytime (6AM - 6PM)`, DATE_TIME_ENTITY));
                    returnResult.newReservation.time = '';
                    returnResult.status = reservationStatus.INCOMPLETE;
                }
            }
            // Get date time LG string if we have both date and time            
            if(returnResult.newReservation.date !== '' && returnResult.newReservation.time !== '') {
                returnResult.newReservation.dateTimeLGString = new TimexProperty(returnResult.newReservation.date + 'T' + returnResult.newReservation.time).toNaturalLanguage(today);
            } 
        }        
    }
    // Take the first found value.
    if(locationEntity !== undefined) returnResult.newReservation.location = locationEntity.entityValue[0][0];
    return returnResult;
}
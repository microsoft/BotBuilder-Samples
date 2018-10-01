// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Using text recognizer package to perform timex operations.
var { TimexProperty, creator, resolver } = require('@microsoft/recognizers-text-data-types-timex-expression').default;
const { ReservationOutcome, ReservationResult, reservationStatus } = require('./createReservationPropertyResult');
const { LUIS_ENTITIES } = require('../helpers');

// Consts for LUIS entities.
const PARTY_SIZE_ENTITY = LUIS_ENTITIES.number;
const DATE_TIME_ENTITY = LUIS_ENTITIES.datetime;
const LOCATION_ENTITY = LUIS_ENTITIES.cafeLocation;
const CONFIRMATION_ENTITY = LUIS_ENTITIES.confirmationList;

const FOUR_WEEKS = '4';
const MAX_PARTY_SIZE = 10;

// Date constraints for reservations
/* Date for reservations must be:
    - a date in this week .OR.
    - a date in the next 4 weeks
*/
const reservationDateConstraints = [
    creator.thisWeek,
    creator.nextWeeksFromToday(FOUR_WEEKS)
];

// Time constraints for reservations
// Time for reservations must be daytime
const reservationTimeConstraints = [
    creator.daytime
];

/**
 * Reservation property class.
 * - This is a self contained class that exposes a bunch of public methods to
 *   evaluate if we have a complete instance (all required properties filled in)
 * - Generate reply text
 *     - based on missing properties
 *     - with all information that's already been captured
 *     - to confirm reservation
 *     - to provide contextual help
 * - Also exposes two static methods to construct a reservations object based on
 *     - LUIS results object
 *     - arbitrary object
 */
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
    constructor(id, date, time, partySize, location, reservationConfirmed, needsChange, metaData) {
        this.id = id || getGuid();
        this.date = date || '';
        this.time = time || '';
        this.dateLGString = '';
        this.timeLGString = '';
        this.dateTimeLGString = '';
        this.partySize = partySize || 0;
        this.location = location || '';
        this.reservationConfirmed = reservationConfirmed || false;
        this.needsChange = needsChange || undefined;
        this.metaData = metaData || {};
    }

    /**
     * Helper method to evaluate if we have all required properties filled.
     *
     * @returns {Boolean} true if we have a complete reservation property
     */
    get haveCompleteReservation() {
        return ((this.id !== undefined) &&
                (this.date !== '') &&
                (this.time !== '') &&
                (this.partySize !== 0) &&
                (this.location !== ''));
    }

    /**
     * Helper method to update Reservation property with information passed in via the onTurnProperty object
     *
     * @param {OnTurnProperty}
     * @returns {ReservationResult}
     */
    updateProperties(onTurnProperty) {
        let returnResult = new ReservationResult(this);
        return validate(onTurnProperty, returnResult);
    }

    /**
     * Helper method for Language Generation read out based on current reservation property object
     *
     * @returns {String}
     */
    getMissingPropertyReadOut() {
        if (this.location === '') {
            return `What city?`;
        } else if (this.date === '') {
            return `When do you want to come in?`;
        } else if (this.time === '') {
            return `What time?`;
        } else if (this.partySize === 0) {
            return `How many guests?`;
        } else return '';
    }

    /**
     * Helper method for Language Generation read out based on properties that have been captured
     *
     * @returns {String}
     */
    getGroundedPropertiesReadOut() {
        let today = new Date();
        if (this.haveCompleteReservation) return this.confirmationReadOut();
        let groundedProperties = '';
        if (this.partySize !== 0) groundedProperties += ` for ${ this.partySize } guests,`;
        if (this.location !== '') groundedProperties += ` in our ${ this.location } store,`;
        if (this.date !== '' && this.time !== '') {
            groundedProperties += ' for ' + new TimexProperty(this.date + 'T' + this.time).toNaturalLanguage(today) + '.';
        } else if (this.date !== '') {
            groundedProperties += ' for ' + new TimexProperty(this.date).toNaturalLanguage(today);
        } else if (this.timeLGString !== '') {
            groundedProperties += ` for ${ this.timeLGString }`;
        }
        if (groundedProperties === '') return groundedProperties;
        return `Ok. I have a table ${ groundedProperties }`;
    }

    /**
     * Helper to generate confirmation read out string.
     *
     * @returns {String}
     */
    confirmationReadOut() {
        let today = new Date();
        return this.partySize + ' at our ' + this.location + ' store for ' + new TimexProperty(this.date + 'T' + this.time).toNaturalLanguage(today) + '.';
    }

    /**
     * Helper to generate help read out string.
     *
     * @returns {String}
     */
    helpReadOut() {
        if (this.location === '') {
            return `We have cafe locations in Seattle, Bellevue, Redmond and Renton.`;
        } else if (this.date === '') {
            return `I can help you reserve a table up to 4 weeks from today.. You can say 'tomorrow', 'next sunday at 3pm' ...`;
        } else if (this.time === '') {
            return `All our cafe locations are open 6AM - 6PM.`;
        } else if (this.partySize === 0) {
            return `I can help you book a table for up to 10 guests..`;
        } else return '';
    }
}

module.exports = ReservationProperty;

/**
 * Static method to create a new instance of Reservation property based on onTurnProperty object
 *
 * @param {OnTurnProperty}
 * @returns {ReservationResult}
 */
ReservationProperty.fromOnTurnProperty = function(onTurnProperty) {
    let returnResult = new ReservationResult(new ReservationProperty());
    return validate(onTurnProperty, returnResult);
};

/**
 * Static method to create a new instance of Reservation property based on a JSON object
 *
 * @param {Object} obj
 * @returns {ReservationProperty}
 */
ReservationProperty.fromJSON = function(obj) {
    if (obj === undefined) return new ReservationProperty();
    const { id, date, time, partySize, location, reservationConfirmed, needsChange } = obj;
    return new ReservationProperty(id, date, time, partySize, location, reservationConfirmed, needsChange);
};

/**
 * HELPERS
 */
/**
 * Helper function to create a random GUID
  * @returns {string} GUID
 */
const getGuid = function() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        var r = Math.random() * 16 | 0; var v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
};
/**
 * Helper function to validate input and return results based on validation constraints
 *
 * @param {Object} onTurnProperty
 * @param {ReservationResult} return result object
 */
const validate = function(onTurnProperty, returnResult) {
    if (onTurnProperty === undefined || onTurnProperty.entities.length === 0) return returnResult;

    // We only will pull number -> party size, datetimeV2 -> date and time, cafeLocation -> location.
    let numberEntity = onTurnProperty.entities.find(item => item.entityName === PARTY_SIZE_ENTITY);
    let dateTimeEntity = onTurnProperty.entities.find(item => item.entityName === DATE_TIME_ENTITY);
    let locationEntity = onTurnProperty.entities.find(item => item.entityName === LOCATION_ENTITY);
    let confirmationEntity = onTurnProperty.entities.find(item => item.entityName === CONFIRMATION_ENTITY);

    if (numberEntity !== undefined) {
        // We only accept MAX_PARTY_SIZE in a reservation.
        if (parseInt(numberEntity.entityValue[0]) > MAX_PARTY_SIZE) {
            returnResult.outcome.push(new ReservationOutcome('Sorry. ' + parseInt(numberEntity.entityValue[0]) + ' does not work. I can only accept up to 10 guests in a reservation.', PARTY_SIZE_ENTITY));
            returnResult.status = reservationStatus.INCOMPLETE;
        } else {
            returnResult.newReservation.partySize = numberEntity.entityValue[0];
        }
    }

    if (dateTimeEntity !== undefined) {
        // Get parsed date time from TIMEX
        // LUIS returns a timex expression and so get and un-wrap that.
        // Take the first date time since book table scenario does not have to deal with multiple date times or date time ranges.
        if (dateTimeEntity.entityValue[0].timex && dateTimeEntity.entityValue[0].timex[0]) {
            let today = new Date();
            let parsedTimex = new TimexProperty(dateTimeEntity.entityValue[0].timex[0]);
            // see if the date meets our constraints
            if (parsedTimex.dayOfMonth !== undefined && parsedTimex.year !== undefined && parsedTimex.month !== undefined) {
                let lDate = new Date(`${ parsedTimex.year }-${ parsedTimex.month }-${ parsedTimex.dayOfMonth }`);
                returnResult.newReservation.date = new Date(lDate.getTime() - (lDate.getTimezoneOffset() * 60000))
                    .toISOString()
                    .split('T')[0];
                returnResult.newReservation.dateLGString = new TimexProperty(returnResult.newReservation.date).toNaturalLanguage(today);
                const validDate = resolver.evaluate(dateTimeEntity.entityValue[0].timex, reservationDateConstraints);
                if (!validDate || (validDate.length === 0)) {
                    // Validation failed!
                    returnResult.outcome.push(new ReservationOutcome(`Sorry. ${ returnResult.newReservation.dateLGString } does not work. I can only make reservations for the next 4 weeks.`, DATE_TIME_ENTITY));
                    returnResult.newReservation.date = '';
                    returnResult.status = reservationStatus.INCOMPLETE;
                }
            }
            // see if the time meets our constraints
            if (parsedTimex.hour !== undefined && parsedTimex.minute !== undefined && parsedTimex.second !== undefined) {
                const validtime = resolver.evaluate(dateTimeEntity.entityValue[0].timex, reservationTimeConstraints);

                returnResult.newReservation.time = ((parseInt(parsedTimex.hour) < 10) ? '0' + parsedTimex.hour : parsedTimex.hour);
                returnResult.newReservation.time += ':';
                returnResult.newReservation.time += ((parseInt(parsedTimex.minute) < 10) ? '0' + parsedTimex.minute : parsedTimex.minute);
                returnResult.newReservation.time += ':';
                returnResult.newReservation.time += ((parseInt(parsedTimex.second) < 10) ? '0' + parsedTimex.second : parsedTimex.second);

                if (!validtime || (validtime.length === 0)) {
                    // Validation failed!
                    returnResult.outcome.push(new ReservationOutcome(`Sorry, that time does not work. I can only make reservations that are in the daytime (6AM - 6PM)`, DATE_TIME_ENTITY));
                    returnResult.newReservation.time = '';
                    returnResult.status = reservationStatus.INCOMPLETE;
                }
            }
            // Get date time LG string if we have both date and time
            if (returnResult.newReservation.date !== '' && returnResult.newReservation.time !== '') {
                returnResult.newReservation.dateTimeLGString = new TimexProperty(returnResult.newReservation.date + 'T' + returnResult.newReservation.time).toNaturalLanguage(today);
            }
        }
    }
    // Take the first found value.
    if (locationEntity !== undefined) {
        let cafeLocation = locationEntity.entityValue[0][0];

        // Capitalize cafe location.
        returnResult.newReservation.location = cafeLocation.charAt(0).toUpperCase() + cafeLocation.substr(1);
    }

    // Accept confirmation entity if available only if we have a complete reservation
    if (confirmationEntity !== undefined) {
        if (confirmationEntity.entityValue[0][0] === 'yes') {
            returnResult.newReservation.reservationConfirmed = true;
            returnResult.newReservation.needsChange = undefined;
        } else {
            returnResult.newReservation.needsChange = true;
            returnResult.newReservation.reservationConfirmed = undefined;
        }
    }

    return returnResult;
};

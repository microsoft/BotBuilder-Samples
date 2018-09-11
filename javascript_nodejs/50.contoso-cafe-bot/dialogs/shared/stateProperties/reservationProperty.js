// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Possible LUIS entities. You can refer to dialogs\mainDialog\resources\entities.lu for list of entities
const LUIS_ENTITIES = ['confirmationList', 'number', 'datetimeV2', 'cafeLocation', 'userName_patternAny'];

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
        this.partySize = partySize ? partySize : 0;
        this.location = location ? location : '';
        this.metaData = metaData ? metaData : {};
    }
    /**
     * Helper method to update Reservation property with information passed in via the onTurnProperty object
     * @param {Object} onTurnProperty 
     */
    updateProperties(onTurnProperty) {

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
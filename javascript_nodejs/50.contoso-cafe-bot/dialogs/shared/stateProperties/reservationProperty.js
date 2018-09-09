// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
        if(!id) throw ('Need id to create reservation');
        this.id = id;
        this.date = date ? date : '';
        this.time = time ? time : '';
        this.partySize = partySize ? partySize : 0;
        this.location = location ? location : '';
        this.metaData = metaData ? metaData : {};
    }
};

module.exports = ReservationProperty;
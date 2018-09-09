// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const reservationProperty = require('./reservationProperty');

/**
 * On turn property class.
 */
class ReservationsProperty {
    /**
     * Reservations property constructor.
     * 
     * @param {reservationProperty []} reservations Array of reservations
     */
    constructor(reservations) {
        if(!reservations) throw ('Need a reservation')
        this.reservations = reservations ? reservations : [];
    }
}

module.exports = ReservationsProperty;
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

class QueryConstraintProperty {
    /**
     * Query Constraint Property constructor.
     * 
     * @param {String} id reservation id
     * @param {String} date reservation date
     * @param {String} time reservation time
     * @param {Number} partySize number of guests in reservation
     * @param {String} location reservation location
     */
    constructor(date, time, location) {
        if(!location || !date || !time) throw ('Need at least one of {location, date, time} to create a query constraint');
        this.location = location ? location : '';
        this.date = date ? date : '';
        this.time = time ? time : '';
    }
};

module.exports = QueryConstraintProperty;
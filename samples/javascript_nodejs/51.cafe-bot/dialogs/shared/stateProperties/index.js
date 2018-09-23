// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

module.exports = {
    EntityProperty: require('./entityProperty'),
    OnTurnProperty: require('./onTurnProperty'),
    UserProfile: require('./UserProfile'),
    Reservation: require('./reservationProperty'),
    ReservationResult: require('./createReservationPropertyResult').ReservationResult,
    ReservationOutcome: require('./createReservationPropertyResult').ReservationOutcome,
    reservationStatusEnum: require('./createReservationPropertyResult').reservationStatus
};

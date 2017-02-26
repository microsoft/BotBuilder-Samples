var util = require('util');

// AirlineCheckin
function AirlineCheckin(intro_message, locale, pnr_number, checkin_url, flight_info) {

    checkParam(intro_message, 'intro_message');
    checkParam(locale, 'locale');
    checkParam(pnr_number, 'pnr_number');
    checkParam(checkin_url, 'checkin_url');
    checkParam(flight_info, 'flight_info');

    checkType(flight_info, Array, 'flight_info');
    flight_info.forEach(function (info, ix) { checkType(info, FlightInfo, 'flight_info[' + ix + ']'); });

    this.type = 'template';
    this.payload = {
        template_type: 'airline_checkin',
        intro_message: intro_message,
        locale: locale,
        pnr_number: pnr_number,
        checkin_url: checkin_url,
        flight_info: flight_info
    };
}

AirlineCheckin.prototype.toString = function () {
    return util.format(
        '%s. Confirmation Number: %s. %s. Check in @ %s',
        this.payload.intro_message,
        this.payload.pnr_number,
        this.payload.flight_info.map(function (info) { return info.toString(); }).join('; '),
        this.payload.checkin_url);
};

// FlightInfo
function FlightInfo(flight_number, departure_airport, arrival_airport, flight_schedule) {
    checkParam(flight_number, 'flight_number');
    checkParam(departure_airport, 'departure_airport');
    checkParam(arrival_airport, 'arrival_airport');
    checkParam(flight_schedule, 'flight_schedule');
    checkType(departure_airport, Airport, 'departure_airport');
    checkType(arrival_airport, Airport, 'arrival_airport');
    checkType(flight_schedule, FlightSchedule, 'flight_schedule');

    this.flight_number = flight_number;
    this.departure_airport = departure_airport;
    this.arrival_airport = arrival_airport;
    this.flight_schedule = flight_schedule;
}

FlightInfo.prototype.toString = function () {
    return util.format(
        'Flight %s from %s (%s) to %s (%s) departing at %s from gate %s at terminal %s and arriving at %s to gate %s at terminal %s. Boarding time is %s',
        this.flight_number,
        this.departure_airport.city,
        this.departure_airport.airport_code,
        this.arrival_airport.city,
        this.arrival_airport.airport_code,
        this.flight_schedule.departure_time,
        this.departure_airport.gate,
        this.departure_airport.terminal,
        this.flight_schedule.arrival_time,
        this.arrival_airport.gate,
        this.arrival_airport.terminal,
        this.flight_schedule.boarding_time
    );
};

// Airport
function Airport(airport_code, city, terminal, gate) {
    checkParam(airport_code, 'airport_code');
    checkParam(city, 'city');
    checkParam(terminal, 'terminal');
    checkParam(gate, 'gate');

    this.airport_code = airport_code;
    this.city = city;
    this.terminal = terminal;
    this.gate = gate;
}

// FlightSchedule
function FlightSchedule(boarding_time, departure_time, arrival_time) {
    checkParam(boarding_time, 'boarding_time');
    checkParam(departure_time, 'departure_time');
    checkParam(arrival_time, 'arrival_time');

    checkType(boarding_time, Date, 'boarding_time');
    checkType(departure_time, Date, 'departure_time');
    checkType(arrival_time, Date, 'arrival_time');

    this.boarding_time = formatDate(boarding_time);
    this.departure_time = formatDate(departure_time);
    this.arrival_time = formatDate(arrival_time);
}

// helpers
function checkParam(param, name) {
    if (param === undefined) {
        throw new Error('Mising Parameter: \'' + name + '\'');
    }
}

function checkType(param, expectedType, name) {
    if (!(param instanceof expectedType)) {
        throw new Error('Expected type \'' + expectedType.name + '\' for parameter \'' + name + '\'');
    }
}

function formatDate(date) {
    return date.toISOString().split('.')[0];
}

// exports
module.exports = {
    AirlineCheckin: AirlineCheckin,
    FlightInfo: FlightInfo,
    FlightSchedule: FlightSchedule,
    Airport: Airport
};
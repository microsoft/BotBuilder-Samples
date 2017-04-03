var _ = require('lodash');
var fs = require('fs');
var util = require('util');
var JsonPath = require('jsonpath');

var Airports = require('./airports.json');

var FindAirportCodeAction = {
    intentName: 'FindAirportByCode',
    friendlyName: 'Find Airport by Code',
    confirmOnContextSwitch: false,              // allow to abandon this action without confirmation
    schema: {
        Code: { type: 'string', message: 'Please provide airport CODE' }
    },
    fulfill: function (parameters, callback) {
        var airportCode = parameters.Code.toUpperCase();
        var airport = _.first(JsonPath.query(Airports, '$.Continents[*].Countries[*].Cities[*].Airports[?(@.Id === "' + airportCode + '")]'));
        if (airport) {
            var countryId = airport.CountryId;
            var cityId = airport.CityId;
            var country = _.first(JsonPath.query(Airports, '$.Continents[*].Countries[?(@.Id === "' + countryId + '")]'));
            var city = _.first(JsonPath.query(Airports, '$.Continents[*].Countries[*].Cities[?(@.Id === "' + cityId + '")]'));
            callback(util.format('%s corresponds to "%s" which is located in %s, %s [%s]',
                parameters.Code.toUpperCase(), airport.Name, city.Name, country.Name, airport.Location));
        } else {
            callback('Airport CODE not found, please try another query!');
        }
    }
};

module.exports = FindAirportCodeAction;